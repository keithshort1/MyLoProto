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
    public class SimpleMyLoIndexer : IMyLoIndexer
    {
        private const double pi = 3.14159265;
        private NpgsqlConnection _conn;
        private long _userId;
        private int _countIndexed;

        public SimpleMyLoIndexer()
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
        /// Indexes a MyLo datastore using PostgreSQL function 'SetUpIndexCursorsPhotosAndActivities'
        /// </summary>
        public int ExecuteIndexerOnDataStore()
        {
            try
            {
                DataSet indexDS = new DataSet();
                NpgsqlTransaction t = _conn.BeginTransaction();
                NpgsqlCommand command = new NpgsqlCommand("SetUpIndexCursorsPhotosAndActivities", _conn);
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
                DataTable activities = indexDS.Tables[1];
                _countIndexed = 0;

                foreach (DataRow photo in photos.Rows)
                {
                    foreach (DataRow activity in activities.Rows)
                    {
                        if ((Double)photo["gpsLat"] != 0.0)
                        {
                            // We have GPS coordinates for the photo
                            if (IsSameTime(photo, activity, 4) && IsSameLocation(photo, activity, 1.0))
                            {
                                photo["activityid"] = activity["activityid"];
                                Debug.WriteLine("Indexed Photoid Location and Time: {0} to ActivityId: {1}", photo["uniqueid"], activity["activityid"]);
                                Debug.WriteLine("Photo time: {0} Activity time: {1} ", photo["datetaken"], activity["startdatetime"]);
                                Debug.WriteLine("Photo loc: {0}, {1} Activity loc: {2}, {3} ", photo["gpslat"], photo["gpslong"], activity["latitude"], activity["longitude"]);
                                Debug.WriteLine("");
                                _countIndexed++;
                                break;
                            }
                        }
                        else
                        {
                            // We do NOT have GPS coordinates for the photo
                            if (IsSameTime(photo, activity, 4))
                            {
                                photo["activityid"] = activity["activityid"];
                                _countIndexed++;
                                Debug.WriteLine("Indexed Photoid Time only: {0} to ActivityId: {1}", photo["uniqueid"], activity["activityid"]);
                                Debug.WriteLine("Photo time: {0} Activity time: {1} ", photo["datetaken"], activity["startdatetime"]);
                                Debug.WriteLine("");
                                break;
                            }
                        }
                    }
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
            catch (System.Data.DBConcurrencyException daex)
            {
                _conn.Close();
                Debug.WriteLine("Exception Data {0}", daex.Data);
                throw new MyLoDataStoreException(daex.Message, daex);
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
        /// Decides if two geographical points on two DataRows are within one mile based on their lat/long position
        /// </summary>
        /// <param name="photo">A DataRow containing information read from the Photos table</param>
        /// <param name="activity">A DataRow containing information read from the Activities, Locations and Instant table</param>
        private bool IsSameLocation(DataRow photo, DataRow activity, double miles)
        {
            // prototype hack from http://www.ehow.com/facts_6907238_calculate-distance-between-two-lat_longs.html
            // convert all lat long values to radians
            double photoLatRad = (Double)photo["gpslat"] * pi / 180.0 ;
            double photoLongRad = (Double)photo["gpslong"] * pi / 180.0 ;
            double activityLatRad = (Double)activity["Latitude"] * pi / 180.0 ;
            double activityLongRad = (Double)activity["Longitude"] * pi / 180.0;

            // caluclate latitude and longitude difference
            double dlat = photoLatRad - activityLatRad;
            double dlong = photoLongRad - activityLongRad;

            // interim calculation step 1
            double A = Math.Pow(Math.Sin(dlat / 2), 2) + Math.Cos(photoLatRad) * Math.Cos(activityLatRad) * Math.Pow(Math.Sin(dlong / 2), 2);

            // interim calculation step 2
            double C = 2 * (Math.Atan2(Math.Pow(A, 0.5), Math.Pow((1 - A), 0.5)));

            // calculate distance by multiplying radius of Earth in miles
            double distance = C * 3959;

            return Math.Abs(distance) < miles;
        }



        /// <summary>
        /// Decides if two times on two DataRows are within a specified interval
        /// </summary>
        /// <param name="photo">A DataRow containing information read from the Photos table</param>
        /// <param name="activity">A DataRow containing information read from the Activities, Locations and Instant table</param>
        private bool IsSameTime(DataRow photo, DataRow activity, int interval)
        {
            DateTime photoTimeTaken = (DateTime)photo["datetaken"];
            DateTime activityStarted = (DateTime)activity["startdatetime"];
            if (activity["enddatetime"] != null)
            {
                DateTime activityEnded = (DateTime)activity["enddatetime"];
                if (activityEnded.Year != 1)
                {

                    return photoTimeTaken >= activityStarted && photoTimeTaken <= activityEnded;
                }
                else
                {
                    TimeSpan ts = photoTimeTaken - activityStarted;
                    return Math.Abs(ts.TotalHours) < interval;
                }
            }
            else
            {
                TimeSpan ts = photoTimeTaken - activityStarted;
                return Math.Abs(ts.TotalHours) < interval;
            }
        }
    }
}
