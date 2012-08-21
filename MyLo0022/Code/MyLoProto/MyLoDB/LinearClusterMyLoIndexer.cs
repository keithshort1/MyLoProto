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
        private const int windowSize = 10;
        private double K = Math.Log(24);
        private const double pi = 3.14159265;
        private NpgsqlConnection _conn;
        private long _userId;
        private int _countIndexed;
        private double [] _diffs;

        struct EventCluster
        {
            public int diffIndex;
            public int photoIndex1;
            public int photoIndex2;
        }

        public LinearClusterMyLoIndexer()
        {
        }

        /// <summary>
        /// Initializes a MyLo datastore indexer
        /// </summary>
        /// <param name="userId">A validated MyLo Account Id</param>
        /// <param name="conn">A validated MyLoDB connection</param>
        public void InitializeMyLoIndexer(long userId, NpgsqlConnection conn)
        {
            _conn = conn;
            _userId = userId;
        }

        /// <summary>
        /// Indexes a MyLo datastore using PostgreSQL function 'SetUpIndexCursorsOrdered'
        /// </summary>
        //public int ExecuteIndexerOnDataStore()
        //{
        //    try
        //    {
        //        DataSet indexDS = new DataSet();
        //        NpgsqlTransaction t = _conn.BeginTransaction();
        //        NpgsqlCommand command = new NpgsqlCommand("SetUpIndexCursorsOrdered", _conn);
        //        command.CommandType = CommandType.StoredProcedure;
        //        command.Parameters.Add(new NpgsqlParameter());
        //        command.Parameters[0].DbType = DbType.Int64;
        //        command.Parameters[0].Value = _userId;

        //        NpgsqlCommand updateCommand = new NpgsqlCommand("UpdatePhotoActivityId", _conn);
        //        updateCommand.CommandType = CommandType.StoredProcedure;
        //        updateCommand.Parameters.Add(new NpgsqlParameter());
        //        updateCommand.Parameters.Add(new NpgsqlParameter());
        //        updateCommand.Parameters.Add(new NpgsqlParameter());
        //        updateCommand.Parameters[0].DbType = DbType.Int64;
        //        updateCommand.Parameters[1].DbType = DbType.Guid;
        //        updateCommand.Parameters[2].DbType = DbType.Int64;
        //        updateCommand.Parameters[0].Value = _userId;
        //        updateCommand.Parameters[1].SourceColumn = "uniqueid";
        //        updateCommand.Parameters[2].SourceColumn = "activityid";

        //        NpgsqlCommand insertCommand = new NpgsqlCommand("AddGeneratedActivity", _conn);
        //        // create 9 parameters for this function
        //        for (int i = 0; i < 9; i++)
        //        {
        //            insertCommand.Parameters.Add(new NpgsqlParameter());
        //        }

        //        insertCommand.Parameters[0].DbType = DbType.Int64;
        //        insertCommand.Parameters[0].SourceColumn = "userId";
        //        insertCommand.Parameters[1].DbType = DbType.String;
        //        insertCommand.Parameters[1].SourceColumn = "activityKind";
        //        insertCommand.Parameters[2].DbType = DbType.String;
        //        insertCommand.Parameters[2].SourceColumn = "source";
        //        insertCommand.Parameters[3].DbType = DbType.String;
        //        insertCommand.Parameters[3].SourceColumn = "sourceId";
        //        insertCommand.Parameters[4].DbType = DbType.DateTime;
        //        insertCommand.Parameters[4].SourceColumn = "startDate";
        //        insertCommand.Parameters[5].DbType = DbType.DateTime;
        //        insertCommand.Parameters[5].SourceColumn = "endDate";

        //        insertCommand.Parameters[6].DbType = DbType.String;
        //        insertCommand.Parameters[6].SourceColumn = "eventName";
        //        insertCommand.Parameters[7].DbType = DbType.Double;
        //        insertCommand.Parameters[7].SourceColumn = "gpsLat";
        //        insertCommand.Parameters[8].DbType = DbType.Double;
        //        insertCommand.Parameters[8].SourceColumn = "gpsLong";


        //        NpgsqlDataAdapter postgresqlAdapter = new NpgsqlDataAdapter(command);
        //        postgresqlAdapter.UpdateCommand = updateCommand;
        //        postgresqlAdapter.InsertCommand = insertCommand;

        //        postgresqlAdapter.Fill(indexDS);

        //        DataTable photos = indexDS.Tables[0];
        //        DataTable activities = new DataTable();
        //        DataColumn idCol = activities.Columns.Add("activityId", typeof(long));
        //        idCol.Unique = true;
        //        activities.Columns.Add("userId", typeof(long));
        //        activities.Columns.Add("activityKind", typeof(String));
        //        activities.Columns.Add("source", typeof(String));
        //        activities.Columns.Add("sourceId", typeof(String));
        //        activities.Columns.Add("startDate", typeof(DateTime));
        //        activities.Columns.Add("endDate", typeof(DateTime));
        //        activities.Columns.Add("eventName", typeof(String));
        //        activities.Columns.Add("gpsLat", typeof(long));
        //        activities.Columns.Add("gpsLong", typeof(long));
        //        indexDS.Tables.Add(activities);

        //        //string[] masterCols = new string[1];
        //        //masterCols[0] = "activityId";
        //        //string[] detailCols = new string[1];
        //        //detailCols[0] = "activityid";
        //        //DataRelation partOf = new DataRelation("partof", "activities", "photos", masterCols, detailCols, false);
        //        DataRelation partOf;
        //        partOf = new DataRelation("partOf", activities.Columns["activityId"], photos.Columns["activityid"]);
        //        indexDS.Relations.Add(partOf);

        //        List<EventCluster> gapList = new List<EventCluster>();
        //        _countIndexed = 0;

        //        _diffs = CalculateDiffs(photos);

        //        double localGapMean = 0;
        //        int rowCount = photos.Rows.Count;
        //        int lastClusterIndex = 0;
        //        // ignore the first _diff value as it will always be zero
        //        for (int i = 1; i < rowCount; i++)
        //        {
        //            localGapMean = CalculateLocalGapMean(i, windowSize, rowCount);
        //            if (Math.Log(_diffs[i]) >= K + localGapMean)
        //            {
        //                EventCluster ec = new EventCluster();
        //                ec.diffIndex = i;
        //                ec.photoIndex1 = lastClusterIndex;
        //                ec.photoIndex2 = i - 1;
        //                lastClusterIndex = i;
        //                gapList.Add(ec);
        //            }
        //        }

        //        int eventCount = 0;
        //        foreach (EventCluster ec in gapList)
        //        {
                    
        //            Debug.WriteLine("Cluster {0}:", eventCount);
        //            DataRow activity = activities.NewRow();
        //            activity["userId"] = _userId;
        //            activity["activityKind"] = "Generated";
        //            activity["source"] = "LinearClusterIndexer";
        //            activity["sourceId"] = eventCount.ToString();
        //            DataRow p1 = photos.Rows[ec.photoIndex1];
        //            activity["startDate"] = p1["dateTaken"];
        //            DataRow p2 = photos.Rows[ec.photoIndex2];
        //            activity["endDate"] = p2["dateTaken"];
        //            activity["eventName"] = "Test" + eventCount.ToString(); ;
        //            activity["gpsLat"] = 0;
        //            activity["gpsLong"] = 0;
        //            activities.Rows.Add(activity);
        //            for (int l = ec.photoIndex1; l <= ec.photoIndex2; l++)
        //            {
                        
        //                DataRow photo = photos.Rows[l];
        //                _countIndexed++;
        //                // Relationship does this
        //                //photo["activityid"] = activity["activityId"];
        //                Debug.WriteLine("    Photo : {0}, {1}, {2}", photo["uri"], photo["gpslat"], photo["gpslong"]);
        //            }
        //            eventCount++;                   
        //        }

        //        t.Commit();

        //        // now write changes back to the database
        //        DataSet changeDS = indexDS.GetChanges(DataRowState.Modified);
        //        if (changeDS != null)
        //        {
        //            postgresqlAdapter.Update(changeDS);
        //            indexDS.Merge(changeDS);
        //            indexDS.AcceptChanges();
        //        }
        //        _conn.Close();
        //        return _countIndexed;
        //    }
        //    catch (Exception ex)
        //    {
        //        _conn.Close();
        //        throw new MyLoDataStoreException(ex.Message);
        //    }
        //}



        /// <summary>
        /// Indexes a MyLo datastore using PostgreSQL function 'SetUpIndexCursorsOrdered'
        /// </summary>
        public int ExecuteIndexerOnDataStore()
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

                List<EventCluster> gapList = new List<EventCluster>();
                _countIndexed = 0;

                _diffs = CalculateDiffs(photos);

                double localGapMean = 0;
                int rowCount = photos.Rows.Count;
                int lastClusterIndex = 0;
                // ignore the first _diff value as it will always be zero
                for (int i = 1; i < rowCount; i++)
                {
                    localGapMean = CalculateLocalGapMean(i, windowSize, rowCount);
                    if (Math.Log(_diffs[i]) >= K + localGapMean)
                    {
                        EventCluster ec = new EventCluster();
                        ec.diffIndex = i;
                        ec.photoIndex1 = lastClusterIndex;
                        ec.photoIndex2 = i - 1;
                        lastClusterIndex = i;
                        gapList.Add(ec);
                    }
                }

                int eventCount = 0;
                foreach (EventCluster ec in gapList)
                {
                    long newActivity = AddGeneratedActivity(ec, eventCount, photos);
                    Debug.WriteLine("Cluster {0}:", eventCount);

                    for (int l = ec.photoIndex1; l <= ec.photoIndex2; l++)
                    {
                        DataRow photo = photos.Rows[l];
                        _countIndexed++;
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
                _conn.Close();
                return _countIndexed;
            }
            catch (Exception ex)
            {
                _conn.Close();
                throw new MyLoDataStoreException(ex.Message);
            }
        }

        private long AddGeneratedActivity(EventCluster ec, int eventCount, DataTable photos)
        {
            Activity act = new Activity();

            DataRow p1 = photos.Rows[ec.photoIndex1];
            string[] parts1 = p1["dateTaken"].ToString().Split('^');

            DataRow p2 = photos.Rows[ec.photoIndex2];
            string[] parts2 = p2["dateTaken"].ToString().Split('^');

            act.ActivityKind = "Generated";
            act.EndDate = (DateTime)p2["dateTaken"];
            act.Source = "LinearClusterIndexer";
            act.SourceId = eventCount.ToString();
            act.LocationName = "IndexTest01" + eventCount.ToString();
            act.Latitude = 0;
            act.Longitude = 0;

            TimePeriod tpStart = new TimePeriod();
            tpStart.Year = (short)act.StartDate.Year;
            tpStart.Month = (short)act.StartDate.Month;
            tpStart.Hour = (short)act.StartDate.Hour;
            tpStart.Day = act.StartDate.DayOfWeek.ToString();
            tpStart.DayNumber = (short)act.StartDate.Day;
            tpStart.AltKey = Convert.ToDateTime(parts1[0]);

            act.StartDate = tpStart.AltKey;

            TimePeriod tpEnd = new TimePeriod();
            tpEnd.Year = (short)act.EndDate.Year;
            tpEnd.Month = (short)act.EndDate.Month;
            tpEnd.Hour = (short)act.EndDate.Hour;
            tpEnd.Day = act.EndDate.DayOfWeek.ToString();
            tpEnd.DayNumber = (short)act.EndDate.Day;
            tpEnd.AltKey = Convert.ToDateTime(parts1[0]);

            act.StartDate = tpEnd.AltKey;

            Location loc = new Location();
            return AddActivity(_userId, act,tpStart, tpEnd, loc);
        }
            
            
        /// <summary>
        /// Adds Activity to MyLo datastore using PostgreSQL function 'addActivity'
        /// </summary>
        /// <param name="userId">A validated MyLo account Id</param>
        /// <param name="act">An Activity instance</param>
        /// <param name="startDate">A TimePeriod instance representing the start of the Activity</param>
        /// <param name="endDate">A TimePeriod instance representing the end of the Activity</param>
        /// <param name="loc">A Location instance represents where the Activity takes place</param>
        public long AddActivity(long userId, Activity act, TimePeriod startDate, TimePeriod endDate, Location loc)
        {
            try
            {
                NpgsqlCommand command = new NpgsqlCommand("AddActivityHierarchical", _conn);
                command.CommandType = CommandType.StoredProcedure;

                // create 25 parameters for this function
                for (int i = 0; i < 25; i++)
                {
                    command.Parameters.Add(new NpgsqlParameter());
                }

                command.Parameters[0].DbType = DbType.Int64;
                command.Parameters[0].Value = userId;
                command.Parameters[1].DbType = DbType.String;
                command.Parameters[1].Value = act.ActivityKind;
                command.Parameters[2].DbType = DbType.String;
                command.Parameters[2].Value = act.Source;
                command.Parameters[3].DbType = DbType.String;
                command.Parameters[3].Value = act.SourceId;
                command.Parameters[4].DbType = DbType.DateTime;
                command.Parameters[4].Value = startDate.AltKey;
                command.Parameters[5].DbType = DbType.DateTime;
                command.Parameters[5].Value = endDate.AltKey;

                command.Parameters[6].DbType = DbType.Int16;
                command.Parameters[6].Value = startDate.Year;
                command.Parameters[7].DbType = DbType.Int16;
                command.Parameters[7].Value = startDate.Month;
                command.Parameters[8].DbType = DbType.String;
                command.Parameters[8].Value = startDate.Day;
                command.Parameters[9].DbType = DbType.Int16;
                command.Parameters[9].Value = startDate.DayNumber;
                command.Parameters[10].DbType = DbType.Int16;
                command.Parameters[10].Value = startDate.Hour;

                command.Parameters[11].DbType = DbType.Int16;
                command.Parameters[11].Value = endDate.Year;
                command.Parameters[12].DbType = DbType.Int16;
                command.Parameters[12].Value = endDate.Month;
                command.Parameters[13].DbType = DbType.String;
                command.Parameters[13].Value = endDate.Day;
                command.Parameters[14].DbType = DbType.Int16;
                command.Parameters[14].Value = endDate.DayNumber;
                command.Parameters[15].DbType = DbType.Int16;
                command.Parameters[15].Value = endDate.Hour;

                command.Parameters[16].DbType = DbType.String;
                command.Parameters[16].Value = act.LocationName;
                command.Parameters[17].DbType = DbType.Double;
                command.Parameters[17].Value = act.Latitude;
                command.Parameters[18].DbType = DbType.Double;
                command.Parameters[18].Value = act.Longitude;
                command.Parameters[19].DbType = DbType.String;
                command.Parameters[19].Value = String.IsNullOrEmpty(loc.Street) ? null : loc.Street;
                command.Parameters[20].DbType = DbType.String;
                command.Parameters[20].Value = String.IsNullOrEmpty(loc.City) ? null : loc.City;
                command.Parameters[21].DbType = DbType.String;
                command.Parameters[21].Value = String.IsNullOrEmpty(loc.State) ? null : loc.State;
                command.Parameters[22].DbType = DbType.String;
                command.Parameters[22].Value = String.IsNullOrEmpty(loc.Zip) ? null : loc.Zip;
                command.Parameters[23].DbType = DbType.String;
                command.Parameters[23].Value = String.IsNullOrEmpty(loc.Country) ? null : loc.Country;
                command.Parameters[24].DbType = DbType.Int64;
                command.Parameters[24].Value = 0;
                Debug.WriteLine("AddActivity Loc is: {0}, {1}, {2}, {3}, {4}", command.Parameters[19].Value, command.Parameters[20].Value, command.Parameters[21].Value, command.Parameters[22].Value, command.Parameters[23].Value);
                long result = (long)command.ExecuteScalar();
                return result;
            }
            catch (NpgsqlException npex)
            {
                _conn.Close();
                throw new MyLoDataStoreException(npex.Message, npex);
            }
            catch (Exception ex)
            {
                _conn.Close();
                throw new MyLoDataStoreException(ex.Message);
            }
        }



        /// <summary>
        /// Calculates a Local Gap Mean from an array of differences held in global _diffs in minutes between  photos sorted by timetaken
        /// </summary>
        /// <param name="i">The currently inspected item in _diffs</param>
        /// <param name="windowSize">A constant set for the length  of the time window + or -</param>
        /// <param name="n">The length of the _diff array</param>
        private double CalculateLocalGapMean(int i, int windowSize, int n)
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
                if (_diffs[k] != 0)
                {
                    sum = sum + Math.Log(_diffs[k]);
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
