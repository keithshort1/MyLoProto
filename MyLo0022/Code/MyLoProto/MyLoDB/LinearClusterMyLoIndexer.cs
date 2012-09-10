/*

Copyright Keith Short 2012
keith_short@hotmail.com

This file is part of the MyLo application

------------------------------------------------------------------------
*/

using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Npgsql;
using NpgsqlTypes;
using MyLoExceptions;
using System.Diagnostics;

namespace MyLoDBNS
{
    public class LinearClusterMyLoIndexer : IMyLoIndexer
    {
        private const double pi = 3.14159265;
        private NpgsqlConnection _conn;
        private long _userId;
        private MyLoDB _store;

        struct EventCluster
        {
            public int diffIndex;
            public int ecIndex1;
            public int ecIndex2;
            public DateTime startTime;
            public DateTime endTime;
        }

        public LinearClusterMyLoIndexer()
        {
        }

        /// <summary>
        /// Initializes a MyLo datastore indexer
        /// </summary>
        /// <param name="userId">A validated MyLo Account Id</param>
        /// <param name="conn">A validated MyLoDB connection</param>
        public void InitializeMyLoIndexer(long userId, NpgsqlConnection conn, MyLoDB store)
        {
            _conn = conn;
            _userId = userId;
            _store = store;
        }

        

        /// <summary>
        /// Indexes a MyLo datastore using PostgreSQL function 'SetUpIndexCursorsOrdered'
        /// </summary>
        public int ExecuteIndexerOnDataStore()
        {
            int photosIndexed = 0;
            int windowSize = 10;
            double K = Math.Log(24);
            List<EventCluster> ecList1 = BuildFirstLevelCluster(windowSize, K, out photosIndexed);
            if (ecList1.Count != 0)
            {
                windowSize = 4;
                List<EventCluster> ecList2 = BuildNextLevelCluster(ecList1, windowSize, K, "Two");
                if (ecList2.Count != 0)
                {
                    List<EventCluster> ecList3 = BuildNextLevelCluster(ecList2, windowSize, K, "Three");
                }
            }
            return photosIndexed;
        }


        private List<EventCluster> BuildFirstLevelCluster(int windowSize, double K, out int countIndexed)
        {
            try
            {
                DataSet indexDS = new DataSet();
                NpgsqlTransaction t = _conn.BeginTransaction();
                NpgsqlCommand command = new NpgsqlCommand("SetUpIndexCursorsOrdered", _conn);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.Add(new NpgsqlParameter());
                command.Parameters[0].DbType = DbType.Int64;
                command.Parameters[0].Value = _userId;

                NpgsqlCommand updateCommand = new NpgsqlCommand("UpdatePhotoActivityId", _conn);
                updateCommand.CommandType = CommandType.StoredProcedure;
                updateCommand.Parameters.Add(new NpgsqlParameter());
                updateCommand.Parameters.Add(new NpgsqlParameter());
                updateCommand.Parameters.Add(new NpgsqlParameter());
                updateCommand.Parameters[0].DbType = DbType.Int64;
                updateCommand.Parameters[1].DbType = DbType.Guid;
                updateCommand.Parameters[2].DbType = DbType.Int64;
                updateCommand.Parameters[0].Value = _userId;
                updateCommand.Parameters[1].SourceColumn = "uniqueid";
                updateCommand.Parameters[2].SourceColumn = "activityid";

                NpgsqlDataAdapter postgresqlAdapter = new NpgsqlDataAdapter(command);
                postgresqlAdapter.UpdateCommand = updateCommand;

                postgresqlAdapter.Fill(indexDS);

                DataTable photos = indexDS.Tables[0];
                
                double[] diffs = CalculateDiffs(photos);

                List<EventCluster> gapList = BuildEventClusterListFromPhotos(photos, diffs, windowSize, K);

                int eventCount = 0;
                int indexed = 0;
                foreach (EventCluster ec in gapList)
                {
                    long newActivity = AddGeneratedActivity(ec, eventCount, "FirstLevelCluster");
                    Debug.WriteLine("Cluster {0}:", eventCount);

                    for (int l = ec.ecIndex1; l <= ec.ecIndex2; l++)
                    {
                        DataRow photo = photos.Rows[l];
                        indexed++;
                        photo["activityid"] = newActivity;
                        Debug.WriteLine("    Photo : {0}, {1}, {2}", photo["uri"], photo["gpslat"], photo["gpslong"]);
                    }
                    eventCount++;
                }

                t.Commit();

                // now write changes back to the database
                DataSet changeDS = indexDS.GetChanges(DataRowState.Modified);
                if (changeDS != null)
                {
                    postgresqlAdapter.Update(changeDS);
                    indexDS.Merge(changeDS);
                    indexDS.AcceptChanges();
                }
                //_conn.Close();
                countIndexed = indexed;
                return gapList;
            }
            catch (Exception ex)
            {
                _conn.Close();
                throw new MyLoDataStoreException(ex.Message);
            }
        }



        private List<EventCluster> BuildNextLevelCluster(List<EventCluster> ecList, int windowSize, double K, string genedName)
        {
            double[] diffs = CalculateEventClusterDiffs(ecList);
            List<EventCluster> ecNext = BuildEventClusterListFromEventClusters(ecList, diffs, windowSize, K);
            int eventCount = 0;
            NpgsqlTransaction t = _conn.BeginTransaction();
            foreach (EventCluster ec in ecNext)
            {
                long newActivity = AddGeneratedActivity(ec, eventCount, genedName);
                Debug.WriteLine("Cluster {0}:", eventCount);
                eventCount++;
            }
            t.Commit();
            return ecNext;
        }

        private List<EventCluster> BuildEventClusterListFromEventClusters(List<EventCluster> ecList, double[] diffs, int windowSize, double K)
        {
            double localGapMean = 0;
            int lastClusterIndex = 0;
            List<EventCluster> gapList = new List<EventCluster>();
            int eventCount = ecList.Count;

            // ignore the first _diff value as it will always be zero
            for (int i = 1; i < eventCount; i++)
            {
                localGapMean = CalculateLocalGapMean(i, diffs, windowSize, eventCount);
                if (Math.Log(diffs[i]) >= K + localGapMean)
                {
                    EventCluster ec = new EventCluster();
                    ec.diffIndex = i;
                    ec.ecIndex1 = lastClusterIndex;
                    ec.ecIndex2 = i - 1;
                    EventCluster first = ecList[ec.ecIndex1];
                    ec.startTime = first.startTime;
                    EventCluster second = ecList[ec.ecIndex2];
                    ec.endTime = second.endTime;
                    lastClusterIndex = i;
                    gapList.Add(ec);
                }
            }
            return gapList;
        }

        private double[] CalculateEventClusterDiffs(List<EventCluster> eventClusters)
        {
            {
                int i = 0;
                double[] diffs = new double[eventClusters.Count()];
                diffs[0] = 0;
                bool ignoreFirst = true;
                DateTime lastDate;
                DateTime thisDate;
                lastDate = DateTime.MinValue;
                thisDate = DateTime.MinValue;
                TimeSpan timeDiff = TimeSpan.MinValue;

                foreach (EventCluster ec in eventClusters)
                {
                    thisDate = ec.startTime;
                    if (ignoreFirst)
                    {
                        ignoreFirst = false;
                        diffs[i] = 0;
                        lastDate = ec.endTime;
                    }
                    else
                    {
                        timeDiff = thisDate - lastDate;
                        lastDate = ec.endTime;
                        diffs[i] = timeDiff.TotalMinutes;
                    }
                    i++;
                }
                return diffs;
            }
        }

        private List<EventCluster> BuildEventClusterListFromPhotos(DataTable photos, double[] diffs, int windowSize, double K)
        {
            double localGapMean = 0;
            int lastClusterIndex = 0;
            List<EventCluster> gapList = new List<EventCluster>();
            int rowCount = photos.Rows.Count;

            // ignore the first _diff value as it will always be zero
            for (int i = 1; i < rowCount; i++)
            {
                localGapMean = CalculateLocalGapMean(i, diffs, windowSize, rowCount);
                if (Math.Log(diffs[i]) >= K + localGapMean)
                {
                    EventCluster ec = new EventCluster();
                    ec.diffIndex = i;
                    ec.ecIndex1 = lastClusterIndex;
                    ec.ecIndex2 = i - 1;
                    DataRow dr = photos.Rows[ec.ecIndex1];
                    ec.startTime = (DateTime)dr["datetaken"];
                    dr = photos.Rows[ec.ecIndex2];
                    ec.endTime = (DateTime)dr["datetaken"];
                    lastClusterIndex = i;
                    gapList.Add(ec);
                }
            }
            return gapList;
        }

        //private long AddGeneratedActivity(EventCluster ec, int eventCount, DataTable photos)
        private long AddGeneratedActivity(EventCluster ec, int eventCount, string genedName)
        {
            Activity act = new Activity();

            //DataRow p1 = photos.Rows[ec.ecIndex1];
            //string[] parts1 = p1["dateTaken"].ToString().Split('^');

            //DataRow p2 = photos.Rows[ec.ecIndex2];
            //string[] parts2 = p2["dateTaken"].ToString().Split('^');

            act.ActivityKind = "Generated";
            //act.EndDate = (DateTime)p2["dateTaken"];
            act.EndDate = ec.endTime;
            act.Source = "LinearClusterIndexer";
            act.SourceId = genedName + eventCount.ToString();
            act.ActivityName = genedName + eventCount.ToString();
            act.Latitude = 0;
            act.Longitude = 0;

            TimePeriod tpStart = new TimePeriod();
            tpStart.Year = (short)act.StartDate.Year;
            tpStart.Month = (short)act.StartDate.Month;
            tpStart.Hour = (short)act.StartDate.Hour;
            tpStart.Day = act.StartDate.DayOfWeek.ToString();
            tpStart.DayNumber = (short)act.StartDate.Day;
            //tpStart.AltKey = Convert.ToDateTime(parts1[0]);
            tpStart.AltKey = ec.startTime;

            act.StartDate = tpStart.AltKey;

            TimePeriod tpEnd = new TimePeriod();
            tpEnd.Year = (short)act.EndDate.Year;
            tpEnd.Month = (short)act.EndDate.Month;
            tpEnd.Hour = (short)act.EndDate.Hour;
            tpEnd.Day = act.EndDate.DayOfWeek.ToString();
            tpEnd.DayNumber = (short)act.EndDate.Day;
            //tpEnd.AltKey = Convert.ToDateTime(parts1[0]);
            tpEnd.AltKey = ec.endTime;

            //act.StartDate = tpEnd.AltKey;

            Address loc = new Address();
            //return _store.AddActivity(_userId, act, tpStart, tpEnd, loc);
            return _store.AddActivity(_userId, act, tpStart, tpEnd, loc);
        }
            
            

        /// Adds Activity to MyLo datastore using PostgreSQL function 'addActivity'
        /// </summary>
        /// <param name="userId">A validated MyLo account Id</param>
        /// <param name="act">An Activity instance</param>
        /// <param name="startDate">A TimePeriod instance representing the start of the Activity</param>
        /// <param name="endDate">A TimePeriod instance representing the end of the Activity</param>
        /// <param name="loc">A Location instance represents where the Activity takes place</param>
        //public long AddActivity(long userId, Activity act, TimePeriod startDate, TimePeriod endDate, Address loc)
        //{
        //    try
        //    {


        //        NpgsqlCommand command = new NpgsqlCommand("AddActivityHierarchical", _conn);
        //        command.CommandType = CommandType.StoredProcedure;

        //        // create 25 parameters for this function
        //        for (int i = 0; i < 25; i++)
        //        {
        //            command.Parameters.Add(new NpgsqlParameter());
        //        }

        //        command.Parameters[0].DbType = DbType.Int64;
        //        command.Parameters[0].Value = userId;
        //        command.Parameters[1].DbType = DbType.String;
        //        command.Parameters[1].Value = act.ActivityKind;
        //        command.Parameters[2].DbType = DbType.String;
        //        command.Parameters[2].Value = act.Source;
        //        command.Parameters[3].DbType = DbType.String;
        //        command.Parameters[3].Value = act.SourceId;
        //        command.Parameters[4].DbType = DbType.DateTime;
        //        command.Parameters[4].Value = startDate.AltKey;
        //        command.Parameters[5].DbType = DbType.DateTime;
        //        command.Parameters[5].Value = endDate.AltKey;

        //        command.Parameters[6].DbType = DbType.Int16;
        //        command.Parameters[6].Value = startDate.Year;
        //        command.Parameters[7].DbType = DbType.Int16;
        //        command.Parameters[7].Value = startDate.Month;
        //        command.Parameters[8].DbType = DbType.String;
        //        command.Parameters[8].Value = startDate.Day;
        //        command.Parameters[9].DbType = DbType.Int16;
        //        command.Parameters[9].Value = startDate.DayNumber;
        //        command.Parameters[10].DbType = DbType.Int16;
        //        command.Parameters[10].Value = startDate.Hour;

        //        command.Parameters[11].DbType = DbType.Int16;
        //        command.Parameters[11].Value = endDate.Year;
        //        command.Parameters[12].DbType = DbType.Int16;
        //        command.Parameters[12].Value = endDate.Month;
        //        command.Parameters[13].DbType = DbType.String;
        //        command.Parameters[13].Value = endDate.Day;
        //        command.Parameters[14].DbType = DbType.Int16;
        //        command.Parameters[14].Value = endDate.DayNumber;
        //        command.Parameters[15].DbType = DbType.Int16;
        //        command.Parameters[15].Value = endDate.Hour;

        //        command.Parameters[16].DbType = DbType.String;
        //        command.Parameters[16].Value = act.ActivityName;
        //        command.Parameters[17].DbType = DbType.Double;
        //        command.Parameters[17].Value = act.Latitude;
        //        command.Parameters[18].DbType = DbType.Double;
        //        command.Parameters[18].Value = act.Longitude;
        //        command.Parameters[19].DbType = DbType.String;
        //        command.Parameters[19].Value = String.IsNullOrEmpty(loc.Street) ? null : loc.Street;
        //        command.Parameters[20].DbType = DbType.String;
        //        command.Parameters[20].Value = String.IsNullOrEmpty(loc.City) ? null : loc.City;
        //        command.Parameters[21].DbType = DbType.String;
        //        command.Parameters[21].Value = String.IsNullOrEmpty(loc.State) ? null : loc.State;
        //        command.Parameters[22].DbType = DbType.String;
        //        command.Parameters[22].Value = String.IsNullOrEmpty(loc.Zip) ? null : loc.Zip;
        //        command.Parameters[23].DbType = DbType.String;
        //        command.Parameters[23].Value = String.IsNullOrEmpty(loc.Country) ? null : loc.Country;
        //        command.Parameters[24].DbType = DbType.Int64;
        //        command.Parameters[24].Value = 0;
        //        Debug.WriteLine("AddActivity Loc is: {0}, {1}, {2}, {3}, {4}", command.Parameters[19].Value, command.Parameters[20].Value, command.Parameters[21].Value, command.Parameters[22].Value, command.Parameters[23].Value);
        //        Debug.WriteLine("AddActivity Time is: {0}, {1}", command.Parameters[4].Value, command.Parameters[5].Value);
        //        long result = (long)command.ExecuteScalar();
        //        return result;
        //    }
        //    catch (NpgsqlException npex)
        //    {
        //        _conn.Close();
        //        throw new MyLoDataStoreException(npex.Message, npex);
        //    }
        //    catch (Exception ex)
        //    {
        //        _conn.Close();
        //        throw new MyLoDataStoreException(ex.Message);
        //    }
        //}



        /// <summary>
        /// Calculates a Local Gap Mean from an array of differences held in global _diffs in minutes between  photos sorted by timetaken
        /// </summary>
        /// <param name="i">The currently inspected item in diffs</param>
        /// <param name="diffs">An array of doubles representing differences in times</param>
        /// <param name="windowSize">A constant set for the length  of the time window + or -</param>
        /// <param name="n">The length of the _diff array</param>
        private double CalculateLocalGapMean(int i, double[] diffs, int windowSize, int n)
        {
            int minusD = i - windowSize ;
            int plusD = i + windowSize;
            int numerator = 2 * windowSize + 1;

            if (minusD < 0)
            {
                numerator = numerator - windowSize;
                minusD = 0;
            }
            if (plusD > n)
            {
                numerator = numerator - (plusD - n);
                plusD = n;
            }
            double sum = 0;
            for ( int k = minusD; k < plusD ; k++ )
            {
                if (diffs[k] != 0)
                {
                    sum = sum + Math.Log(diffs[k]);
                }
            }
            return sum / numerator; 
        }

        private double[] CalculateDiffs(DataTable photos)
        {
            int i = 0;
            double[] diffs = new double[photos.Rows.Count];
            diffs[0] = 0;
            bool ignoreFirst = true;
            DateTime lastDate;
            DateTime thisDate;
            lastDate = DateTime.MinValue;
            thisDate = DateTime.MinValue;
            TimeSpan diff = TimeSpan.MinValue;

            foreach (DataRow photo in photos.Rows)
            {
                thisDate = (DateTime)photo["dateTaken"];
                if (ignoreFirst)
                {
                    ignoreFirst = false;
                    diffs[i] = 0;
                    lastDate = thisDate;
                }
                else
                {
                    diff = thisDate - lastDate;
                    lastDate = thisDate;
                    diffs[i] = diff.TotalMinutes;
                }
                i++;
            }
            return diffs;
        }
    }
}
