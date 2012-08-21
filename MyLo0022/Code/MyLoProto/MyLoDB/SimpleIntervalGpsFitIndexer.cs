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
    public class SimpleIntervalGpsFitIndexer : IMyLoIndexer
    {
        private const double pi = 3.14159265;
        private NpgsqlConnection _conn;
        private long _userId;
        private int _countIndexed;

        public SimpleIntervalGpsFitIndexer()
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
        /// Indexes a MyLo datastore using PostgreSQL function 'SetupIndexCursorsIntervalAndTimePoints'
        /// </summary>
        public int ExecuteIndexerOnDataStore()
        {
            try
            {
                DataSet indexDS = new DataSet();

                NpgsqlTransaction t = _conn.BeginTransaction();
                NpgsqlCommand command = new NpgsqlCommand("SetupIndexCursorsOrdered", _conn);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.Add(new NpgsqlParameter());
                command.Parameters[0].DbType = DbType.Int64;
                command.Parameters[0].Value = _userId;

                NpgsqlCommand updateCommandforPhotos = new NpgsqlCommand("UpdatePhotoActivityId", _conn);
                updateCommandforPhotos.CommandType = CommandType.StoredProcedure;
                updateCommandforPhotos.Parameters.Add(new NpgsqlParameter());
                updateCommandforPhotos.Parameters.Add(new NpgsqlParameter());
                updateCommandforPhotos.Parameters.Add(new NpgsqlParameter());
                updateCommandforPhotos.Parameters[0].DbType = DbType.Int64;
                updateCommandforPhotos.Parameters[1].DbType = DbType.Guid;
                updateCommandforPhotos.Parameters[2].DbType = DbType.Int64;
                updateCommandforPhotos.Parameters[0].Value = _userId;
                updateCommandforPhotos.Parameters[1].SourceColumn = "uniqueid";
                updateCommandforPhotos.Parameters[2].SourceColumn = "activityid";

                NpgsqlDataAdapter postgresqlAdapterForPhotos = new NpgsqlDataAdapter(command);
                postgresqlAdapterForPhotos.UpdateCommand = updateCommandforPhotos;

                NpgsqlCommand commandForActivities = new NpgsqlCommand("SetupIndexCursorsIntervalAndTimePoints", _conn);
                commandForActivities.CommandType = CommandType.StoredProcedure;
                commandForActivities.Parameters.Add(new NpgsqlParameter());
                commandForActivities.Parameters.Add(new NpgsqlParameter());
                commandForActivities.Parameters.Add(new NpgsqlParameter());
                commandForActivities.Parameters[0].DbType = DbType.Int64;
                commandForActivities.Parameters[1].DbType = DbType.DateTime;
                commandForActivities.Parameters[2].DbType = DbType.Int32;
                commandForActivities.Parameters[0].Value = _userId;
                NpgsqlDataAdapter postgresqlAdapterForActivities = new NpgsqlDataAdapter(commandForActivities);

                postgresqlAdapterForPhotos.Fill(indexDS);

                DataTable photos = indexDS.Tables[0];
                _countIndexed = 0;

                foreach (DataRow photo in photos.Rows)
                {
                    DataSet activitiesDS = new DataSet();
                    // TODO - remove the Fill when datetaken time is still within the last retrieved activity interval
                    commandForActivities.Parameters[1].Value = photo["datetaken"];
                    // TODO - make this hours radius a variable set in UI
                    commandForActivities.Parameters[2].Value = 4;
                    postgresqlAdapterForActivities.Fill(activitiesDS);
                    DataTable activities = activitiesDS.Tables[0];

                    // TODO - the function returns the data sorted by duration, but this order is not preserved by ADO.Net!! Need to investigate
                    // and avoid the use of the ADO.Net sorted view.
                    DataView activitiesView = activities.DefaultView;
                    activitiesView.Sort = "duration ASC";
                    DataTable activitiesSorted = activitiesView.ToTable();

                    //foreach (DataRow activity in activities.Rows)
                    foreach (DataRow activity in activitiesSorted.Rows)
                    {
                        if ((Double)photo["gpsLat"] != 0.0)
                        {
                            if (IsSameLocation(photo, activity, 2.0))
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
                            photo["activityid"] = activity["activityid"];
                            _countIndexed++;
                            Debug.WriteLine("Indexed Photoid Time only: {0} to ActivityId: {1}", photo["uniqueid"], activity["activityid"]);
                            Debug.WriteLine("Photo time: {0} Activity time: {1} ", photo["datetaken"], activity["startdatetime"]);
                            Debug.WriteLine("");
                            break;                          
                        }
                    }
                }

                t.Commit();

                // now write changes back to the database
                DataSet changeDS = indexDS.GetChanges(DataRowState.Modified);
                if (changeDS != null)
                {
                    postgresqlAdapterForPhotos.Update(changeDS);
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

    }
}
