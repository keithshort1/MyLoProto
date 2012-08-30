/*

Copyright Keith Short 2012
keith_short@hotmail.com

This file is part of the MyLo application

------------------------------------------------------------------------
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Text;
using System.IO;
using Npgsql;
using NpgsqlTypes;
using MyLoExceptions;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Xml.Linq;


namespace MyLoDBNS
{
    public class MyLoDB
    {
        private const double pi = 3.14159265;
        private NpgsqlConnection _conn;
        char[] _andChar = new char[4];

        /// <summary>
        /// Creates a new MyLo Store in the given Database
        /// </summary>
        public MyLoDB()
        {
            var xmlElm = XElement.Load(@"../../../configuration.xml");
            XElement element = xmlElm.Element("connectionString");
            _conn = new NpgsqlConnection(element.Value);
            _conn.Open();
            _andChar[0] = ' '; _andChar[1] = 'A';
            _andChar[2] = 'N'; _andChar[3] = 'D';
        }

        /// <summary>
        /// Closee any open connection to the MyLo Store 
        /// </summary>
        public void CloseStore()
        {
            _conn.Close();
        }


        /// <summary>
        /// Calls a PostgreSQL function to retrieve the MyLo Account Id given an Account Name
        /// </summary>
        /// <param name="userName">Input MyLo User Name</param>
        public long GetUserAccount(string userName)
        {
            try
            {
                NpgsqlCommand command = new NpgsqlCommand("GetMyLoAccountId", _conn);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.Add(new NpgsqlParameter());
                command.Parameters[0].DbType = DbType.String;
                command.Parameters[0].Value = userName;

                Object result = command.ExecuteScalar();
                return (long)result;
            }
            catch (Exception ex)
            {
                _conn.Close();
                throw new MyLoAccountIdException("Unknown MyLo Account Name: " + userName + " Inner exception: " + ex);
            }

        }


        /// <summary>
        /// Adds Photo to MyLo datastore using PostgreSQL function 'addPhoto'
        /// </summary>
        /// <param name="userId">An identifier for a MyLoAccount holder</param>
        /// <param name="...">photo meta data</param>
        public void AddPhoto(long userId, Photo photo)
        {
            try
            {
                NpgsqlCommand command = new NpgsqlCommand("AddPhoto", _conn);
                command.CommandType = CommandType.StoredProcedure;

                // create 8 parameters for this function
                for (int i = 0; i < 10; i++)
                {
                    command.Parameters.Add(new NpgsqlParameter());
                }

                command.Parameters[0].DbType = DbType.Int64;
                command.Parameters[0].Value = userId;
                command.Parameters[1].DbType = DbType.String;
                command.Parameters[1].Value = photo.Uri;
                command.Parameters[2].DbType = DbType.Guid;
                command.Parameters[2].Value = photo.Uuid;
                command.Parameters[3].DbType = DbType.Int64;
                command.Parameters[3].Value = photo.CRC;
                command.Parameters[4].DbType = DbType.DateTime;
                command.Parameters[4].Value = photo.DateTaken;
                command.Parameters[5].DbType = DbType.String;
                command.Parameters[5].Value = photo.Camera;
                command.Parameters[6].DbType = DbType.Double;
                command.Parameters[6].Value = photo.GpsLat;
                command.Parameters[7].DbType = DbType.Double;
                command.Parameters[7].Value = photo.GpsLong;
                command.Parameters[8].NpgsqlDbType = NpgsqlDbType.Bytea;
                command.Parameters[8].Value = photo.Thumbnail;
                command.Parameters[9].DbType = DbType.String;
                command.Parameters[9].Value = photo.PhotoIndexKind;

                Object result = command.ExecuteScalar();
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
        /// Adds Party to MyLo datastore using PostgreSQL function 'addParty'
        /// </summary>
        /// <param name="userId">An identifier for a MyLoAccount holder</param>
        /// <param name="act">An Party instance</param>
        public long AddParty(long userId, Party person)
        {
            try
            {
                NpgsqlCommand command = new NpgsqlCommand("AddParty", _conn);
                command.CommandType = CommandType.StoredProcedure;

                // create 2 parameters for this function
                for (int i = 0; i < 2; i++)
                {
                    command.Parameters.Add(new NpgsqlParameter());
                }

                command.Parameters[0].DbType = DbType.Int64;
                command.Parameters[0].Value = userId;
                command.Parameters[1].DbType = DbType.String;
                command.Parameters[1].Value = String.IsNullOrEmpty(person.Name) ? null : person.Name;

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
        /// Adds Party to an Activity in MyLo datastore using PostgreSQL function 'AddPartyToActivityByIds'
        /// </summary>
        /// <param name="userId">An identifier for a MyLoAccount holder</param>
        /// <param name="party">A Person instance with partyId set to identifier</param>
        /// <param name="activity">An Activity instance with AcyivtyId set to identifier</param>
        public long AddPartyToActivityByIds(long userId, Party party, Activity activity)
        {
            try
            {
                NpgsqlCommand command = new NpgsqlCommand("AddPartyToActivityByIds", _conn);
                command.CommandType = CommandType.StoredProcedure;

                // create 2 parameters for this function
                for (int i = 0; i < 4; i++)
                {
                    command.Parameters.Add(new NpgsqlParameter());
                }

                command.Parameters[0].DbType = DbType.Int64;
                command.Parameters[0].Value = userId;
                command.Parameters[1].DbType = DbType.Int64;
                command.Parameters[1].Value = party.PartyId;
                command.Parameters[2].DbType = DbType.String;
                command.Parameters[2].Value = party.PartyKind;
                command.Parameters[3].DbType = DbType.Int64;
                command.Parameters[3].Value = activity.ActivityId;

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
        /// Adds Activity to MyLo datastore using PostgreSQL function 'addActivity'
        /// </summary>
        /// <param name="userId">A validated MyLo account Id</param>
        /// <param name="act">An Activity instance</param>
        /// <param name="startDate">A TimePeriod instance representing the start of the Activity</param>
        /// <param name="endDate">A TimePeriod instance representing the end of the Activity</param>
        /// <param name="loc">A Location instance represents where the Activity takes place</param>
        public long AddActivity(long userId, Activity act, TimePeriod startDate, TimePeriod endDate, Address loc)
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
                command.Parameters[16].Value = act.ActivityName;
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
        /// Indexes a MyLo datastore using PostgreSQL function 'SetUpIndexCursors'
        /// </summary>
        /// <param name="userId">A validated MyLo Account Id</param>
        public int IndexDataStore(long userId, IMyLoIndexer indexer)
        {
            try
            {
                indexer.InitializeMyLoIndexer(userId, _conn);
                return indexer.ExecuteIndexerOnDataStore();
            }
            catch (Exception ex)
            {
                throw new MyLoDataStoreException(ex.Message);
            }
        }


        /// <summary>
        /// Retrieves All Photos in a MyLo DataStore for current account holder
        /// </summary>
        public DataSet GetAllPhotos(long userId)
        {
            DataSet ds = new DataSet();

            NpgsqlCommand command = new NpgsqlCommand("SELECT uri, datetaken, camera, gpslat, gpslong, thumbnail FROM photo WHERE MyLoAccountId = @userid ", _conn);
            command.Parameters.Add(new NpgsqlParameter("userid", DbType.Int64));
            command.Parameters[0].Value = userId;
            NpgsqlDataAdapter da = new NpgsqlDataAdapter(command);
            da.Fill(ds);
            _conn.Close();
            if (ds != null)
            {
                return ds;
            }
            else
            {
                return null;
            }
        }


        /// <summary>
        /// Retrieves Selected Photos in a MyLo DataStore for current account holder
        /// Using Dimension Ids
        /// </summary>
        /// <param name="timePeriodId">An identifier for a Time Period</param>
        /// <param name="partyId">An identifier for a Party</param>
        /// <param name="addressId">An identifier for a Location</param>
        public DataSet GetPhotosByDimensionIds(long timePeriodId, long partyId, long addressId)
        {
            // TODO Unsed - but should finish using style for query by Fileds
            DataSet ds = new DataSet();
            NpgsqlDataAdapter da = new NpgsqlDataAdapter();
            NpgsqlCommand command = new NpgsqlCommand();
            if (timePeriodId != 0 && addressId != 0)
            {
                command.CommandText = "SELECT P.uri, P.datetaken, P.camera, P.gpslat, P.gpslong, P.thumbnail FROM photo AS P " +
                                                                "LEFT JOIN Activity AS A ON P.activityId = A.activityId " +
                                                                "JOIN TimePeriod AS TP ON A.starttimeperiodid = TP.timeperiodid " +
                                                                "LEFT JOIN Address AS L ON A.addressid = L.addressid " +
                                                                "WHERE TP.timeperiodid = @tpid AND L.addressid = @addrid ";
                command.Parameters.Add(new NpgsqlParameter("tpid", DbType.Int64));
                command.Parameters[0].Value = timePeriodId;
                command.Parameters.Add(new NpgsqlParameter("addrid", DbType.Int64));
                command.Parameters[1].Value = addressId;
                command.Connection = _conn;
            }
            else if (timePeriodId != 0 && addressId == 0)
            {
                command.CommandText = "SELECT P.uri, P.datetaken, P.camera, P.gpslat, P.gpslong, P.thumbnail FROM photo AS P " +
                                                                "LEFT JOIN Activity AS A ON P.activityId = A.activityId " +
                                                                "JOIN TimePeriod AS TP ON A.starttimeperiodid = TP.timeperiodid " +
                                                                "LEFT JOIN Address AS L ON A.addressid = L.addressid " +
                                                                "WHERE TP.timeperiodid = @tpid ";
                command.Parameters.Add(new NpgsqlParameter("tpid", DbType.Int64));
                command.Parameters[0].Value = timePeriodId;
                command.Connection = _conn;
            }
            else if (timePeriodId == 0 && addressId != 0)
            {
                command.CommandText = "SELECT P.uri, P.datetaken, P.camera, P.gpslat, P.gpslong, P.thumbnail FROM photo AS P " +
                                                                "LEFT JOIN Activity AS A ON P.activityId = A.activityId " +
                                                                "JOIN TimePeriod AS TP ON A.starttimeperiodid = TP.timeperiodid " +
                                                                "LEFT JOIN Address AS L ON A.addressid = L.addressid " +
                                                                "WHERE L.addressid = @addrid ";
                command.Parameters.Add(new NpgsqlParameter("addrid", DbType.Int64));
                command.Parameters[0].Value = addressId;
                command.Connection = _conn;

            }

            da.SelectCommand = command;

            da.Fill(ds);
            _conn.Close();
            if (ds != null)
            {
                return ds;
            }
            else
            {
                return null;
            }
        }


        /// <summary>
        /// Retrieves Selected Photos in a MyLo DataStore for current account holder
        /// Using Dimension Ids
        /// </summary>
        /// <param name="timePeriodId">An identifier for a Time Period</param>
        public DataSet GetPhotosByTimePeriod(TimePeriod timePeriod)
        {
            DataSet ds = new DataSet();
            NpgsqlDataAdapter da = new NpgsqlDataAdapter();
            NpgsqlCommand command = new NpgsqlCommand();
            command.CommandText = "SELECT P.uri, P.datetaken, P.camera, P.gpslat, P.gpslong, P.thumbnail FROM photo AS P " +
                                                                "JOIN TimePeriod AS TP ON P.timeperiodid = TP.timeperiodid WHERE ";
            int i = 0;
            string whereClause = String.Empty;
            if (timePeriod.Year != 0)
            {
                whereClause += "AND TP.year = @year ";
                command.Parameters.Add(new NpgsqlParameter("year", DbType.Int16));
                command.Parameters[i].Value = timePeriod.Year; i++;
            }
            if (timePeriod.Month != 0)
            {
                whereClause += "AND TP.month = @month ";
                command.Parameters.Add(new NpgsqlParameter("month", DbType.Int16));
                command.Parameters[i].Value = timePeriod.Month; i++;
            }

            command.CommandText += whereClause.TrimStart(_andChar);
            command.Connection = _conn;
            da.SelectCommand = command;

            da.Fill(ds);
            _conn.Close();
            if (ds != null)
            {
                return ds;
            }
            else
            {
                return null;
            }
        }


        /// <summary>
        /// Retrieves All Photos in a MyLo DataStore for current account holder
        /// Grouped by Location ids with count for each Location
        /// </summary>
        public DataSet GetAllPhotosGroupedByLocation(long userid)
        {
            DataSet ds = new DataSet();
            NpgsqlDataAdapter da = new NpgsqlDataAdapter();
            NpgsqlCommand command = new NpgsqlCommand();
            command.CommandText = "WITH locations AS " +
                                    "(WITH photies AS ((select uri, datetaken, gpslat, gpslong, A.geolocationid  FROM photo AS P " +
                                    "       JOIN Activity AS A ON A.activityid = P.activityid " +
                                    "       WHERE A.geolocationid IS NOT NULL AND P.MyLoAccountId = @userid) " +
                                    "UNION " +
                                    "(SELECT uri, datetaken,  gpslat, gpslong, P.geolocationid  FROM photo AS P " +
                                    "       WHERE P.geolocationid IS NOT NULL AND P.MyLoAccountId = @userid))  " +
                                    "SELECT DISTINCT PH.geolocationid, count(*) FROM photies AS PH " +
                                    "GROUP BY geolocationid) " +
                                    "SELECT L.latitude, L.longitude, locations.count, locationid FROM locations " +
                                    "JOIN geolocation AS L ON L.locationid = locations.geolocationid " +
                                    "WHERE L.MyLoAccountId = @userid " +
                                    "ORDER BY locations.geolocationid ";
            command.Parameters.Add(new NpgsqlParameter("userid", DbType.Int64));
            command.Parameters[0].Value = userid;
            command.Connection = _conn;
            da.SelectCommand = command;

            da.Fill(ds);
            _conn.Close();
            if (ds != null)
            {
                return ds;
            }
            else
            {
                return null;
            }
        }


        /// <summary>
        /// Retrieves Selected Photos in a MyLo DataStore for current account holder
        /// Using Dimension Ids
        /// </summary>
        /// <param name="userId">An identifier for a MyLo Account</param>
        /// <param name="timePeriod">An identifier for a Time Period</param>
        /// <param name="person">An identifier for a Party</param>
        /// <param name="address">An identifier for a Location</param>
        public DataSet GetPhotosByDimensionFields(long userId, Address addr, TimePeriod timePeriod, Party person)
        {
            DataSet ds = new DataSet();
            NpgsqlDataAdapter da = new NpgsqlDataAdapter();
            NpgsqlCommand command = new NpgsqlCommand();
            string withClause = "WITH parties AS ( " +
                                        "SELECT PAP.activityid FROM partyactivityparticipation AS PAP " +
                                        "JOIN person AS P ON P.partyid = PAP.partyid " +
                                        "WHERE P.name = @personname) ";
            string selectClause = "SELECT P.uri, P.datetaken, P.camera, P.gpslat, P.gpslong, P.thumbnail FROM photo AS P " +
                                                                "LEFT JOIN Activity AS A ON P.activityId = A.activityId " +
                                                                "JOIN TimePeriod AS TP ON A.starttimeperiodid = TP.timeperiodid " +
                                                                "LEFT JOIN Address AS L ON A.addressid = L.addressid ";
            string joinWithClause = "JOIN parties AS PTY ON PTY.activityid = A.activityid ";

            int i = 0;

            if (person.Name != String.Empty)
            {
                command.Parameters.Add(new NpgsqlParameter("personname", DbType.String));
                command.Parameters[i].Value = person.Name; i++;
                command.CommandText += withClause;
                command.CommandText += selectClause;
                command.CommandText += joinWithClause;
            }
            else
            {
                command.CommandText += selectClause;
            }
            string whereClause = " WHERE P.MyLoAccountId = @userid ";
            command.Parameters.Add(new NpgsqlParameter("userid", DbType.Int64));
            command.Parameters[i].Value = userId; i++;

            if (addr.City != String.Empty)
            {
                whereClause += "AND L.city = @city ";
                command.Parameters.Add(new NpgsqlParameter("city", DbType.String));
                command.Parameters[i].Value = addr.City; i++;
            }
            if (addr.Country != String.Empty)
            {
                whereClause += "AND L.country = @country ";
                command.Parameters.Add(new NpgsqlParameter("country", DbType.String));
                command.Parameters[i].Value = addr.Country; i++;
            }
            if (timePeriod.Year != 0)
            {
                whereClause += "AND TP.year = @year ";
                command.Parameters.Add(new NpgsqlParameter("year", DbType.Int16));
                command.Parameters[i].Value = timePeriod.Year; i++;
            }
            if (timePeriod.Month != 0)
            {
                whereClause += "AND TP.month = @month ";
                command.Parameters.Add(new NpgsqlParameter("month", DbType.Int16));
                command.Parameters[i].Value = timePeriod.Month; i++;
            }
            if (timePeriod.Day != String.Empty)
            {
                whereClause += "AND TP.dayname = @day ";
                command.Parameters.Add(new NpgsqlParameter("day", DbType.String));
                command.Parameters[i].Value = timePeriod.Day; i++;
            }

            command.CommandText += whereClause;
            command.Connection = _conn;
            da.SelectCommand = command;

            da.Fill(ds);
            _conn.Close();
            if (ds != null)
            {
                return ds;
            }
            else
            {
                return null;
            }
        }



        /// <summary>
        /// Retrieves Photos assigned to a selected Activity in a MyLo DataStore for current account holder
        /// </summary>
        /// <param name="userId">An identifier for a MyLo Account</param>
        /// <param name="activityId">An identifier for an Activity</param>
        public DataSet GetPhotosByActivity(long userId, long activityId)
        {
            DataSet ds = new DataSet();

            NpgsqlCommand command = new NpgsqlCommand("SELECT uri, datetaken, camera, gpslat, gpslong, thumbnail FROM photo AS P " +
                                                            "JOIN GetActivityAndSubActivities(@userid, @id) ON P.ActivityId = GetActivityAndSubActivities",
                                                        _conn);
            command.Parameters.Add(new NpgsqlParameter("id", DbType.Int64));
            command.Parameters[0].Value = activityId;
            command.Parameters.Add(new NpgsqlParameter("userid", DbType.Int64));
            command.Parameters[1].Value = userId;
            NpgsqlDataAdapter da = new NpgsqlDataAdapter(command);

            da.Fill(ds);
            _conn.Close();
            if (ds != null)
            {
                return ds;
            }
            else
            {
                return null;
            }
        }


        /// <summary>
        /// Retrieves All TimePeriods in a MyLo DataStore for current account holder
        /// </summary>
        /// <param name="userId">An identifier for a MyLoAccount holder</param>
        public DataSet GetAllTimePeriods(long userId)
        {
            DataSet ds = new DataSet();

            NpgsqlCommand command = new NpgsqlCommand("SELECT DISTINCT timeperiodid, year, month, dayname, daynumber, timeperiodaltkey FROM timeperiod " +
                "WHERE myloaccountid = @userid ORDER BY timeperiodaltkey  ", _conn);
            command.Parameters.Add(new NpgsqlParameter("userid", DbType.Int64));
            command.Parameters[0].Value = userId;
            NpgsqlDataAdapter da = new NpgsqlDataAdapter(command);
            da.Fill(ds);
            _conn.Close();
            if (ds != null)
            {
                return ds;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Retrieves All Locations in a MyLo DataStore for current account holder
        /// </summary>
        /// <param name="userId">An identifier for a MyLoAccount holder</param>
        public DataSet GetAllLocations(long userId)
        {
            DataSet ds = new DataSet();

            NpgsqlCommand command = new NpgsqlCommand("SELECT DISTINCT L.addressid, L.country, L.state, L.city FROM Address AS L " +
                "WHERE L.MyLoAccountId = @userid ORDER BY L.country, L.state, L.city ", _conn);
            command.Parameters.Add(new NpgsqlParameter("userid", DbType.Int64));
            command.Parameters[0].Value = userId;
            NpgsqlDataAdapter da = new NpgsqlDataAdapter(command);
            da.Fill(ds);
            _conn.Close();
            if (ds != null)
            {
                return ds;
            }
            else
            {
                return null;
            }
        }


        /// <summary>
        /// Retrieves All Activities in a MyLo DataStore for current account holder
        /// </summary>
        /// <param name="userId">An identifier for a MyLoAccount holder</param>
        public DataSet GetAllActivities(long userId)
        {
            DataSet ds = new DataSet();

            NpgsqlCommand command = new NpgsqlCommand("SELECT A.activityid, A.activityname, A.startdatetime, A.enddatetime FROM activity AS A " +
                                                        "WHERE A.MyLoAccountId = @userid " +
                                                        "ORDER BY A.startdatetime, A.activityname ", _conn);
            command.Parameters.Add(new NpgsqlParameter("userid", DbType.Int64));
            command.Parameters[0].Value = userId;
            NpgsqlDataAdapter da = new NpgsqlDataAdapter(command);
            da.Fill(ds);
            _conn.Close();
            if (ds != null)
            {
                return ds;
            }
            else
            {
                return null;
            }
        }


        /// <summary>
        /// Retrieves All Activities and the Activity Hierarchy in a MyLo DataStore for current account holder
        /// </summary>
        /// <param name="userId">An identifier for a MyLoAccount holder</param>
        public DataSet GetAllActivityHierarchy(long userId)
        {
            _conn.Open();
            DataSet ds = new DataSet();
            NpgsqlTransaction t = _conn.BeginTransaction();
            NpgsqlCommand command = new NpgsqlCommand("SetUpCursorsActivitiesAndHierarchy", _conn);
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.Add(new NpgsqlParameter());
            command.Parameters[0].DbType = DbType.Int64;
            command.Parameters[0].Value = userId;

            NpgsqlDataAdapter da = new NpgsqlDataAdapter(command);

            da.Fill(ds);
            t.Commit();
            _conn.Close();
            if (ds != null)
            {
                return ds;
            }
            else
            {
                return null;
            }
        }


        /// <summary>
        /// Retrieves All Root Activities in a MyLo DataStore for current account holder
        /// </summary>
        /// <param name="userId">An identifier for a MyLoAccount holder</param>
        public DataSet GetAllRootActivities(long userId)
        {
            DataSet ds = new DataSet();

            NpgsqlCommand command = new NpgsqlCommand("SELECT A.activityid, A.activityname, A.startdatetime, A.enddatetime FROM activity AS A " +
                                                        "JOIN ActivityHierarchy AS AH ON AH.childactivityid = A.activityid " +
                                                        "WHERE A.MyLoAccountId = @userid AND AH.parentactivityid IS NULL " +
                                                        "ORDER BY A.startdatetime, A.activityname ", _conn);
            command.Parameters.Add(new NpgsqlParameter("userid", DbType.Int64));
            command.Parameters[0].Value = userId;
            NpgsqlDataAdapter da = new NpgsqlDataAdapter(command);
            da.Fill(ds);
            _conn.Close();
            if (ds != null)
            {
                return ds;
            }
            else
            {
                return null;
            }
        }


        /// <summary>
        /// Retrieves All Parties in a MyLo DataStore for current account holder
        /// </summary>
        /// <param name="userId">An identifier for a MyLoAccount holder</param>
        public DataSet GetAllParties(long userId)
        {
            DataSet ds = new DataSet();

            NpgsqlCommand command = new NpgsqlCommand("SELECT partyid, name FROM person WHERE MyLoAccountId = @userid ORDER BY name ", _conn);
            command.Parameters.Add(new NpgsqlParameter("userid", DbType.Int64));
            command.Parameters[0].Value = userId;
            NpgsqlDataAdapter da = new NpgsqlDataAdapter(command);
            da.Fill(ds);
            _conn.Close();
            if (ds != null)
            {
                return ds;
            }
            else
            {
                return null;
            }
        }


        /// <summary>
        /// Retrieves All Photos in a MyLo DataStore for current account holder including thumbnails
        /// </summary>
        /// <param name="userId">An identifier for a MyLoAccount holder</param>
        public DataTable GetAllPhotosWithThumbs(long userId)
        {
            DataSet ds = new DataSet();
            
            NpgsqlCommand command = new NpgsqlCommand("SELECT uri, datetaken, camera, gpslat, gpslong, thumbnail FROM photo WHERE MyLoAccountId = @userid", _conn);
            command.Parameters.Add(new NpgsqlParameter("userid", DbType.Int64));
            command.Parameters[0].Value = userId;
            NpgsqlDataAdapter da = new NpgsqlDataAdapter(command);

            da.Fill(ds);
            DataTable photos = ds.Tables[0];
            int count = photos.Rows.Count;

            _conn.Close();

            if (count > 0)
            {
                return photos;
            }
            else
            {
                return null;
            }
        }


        /// <summary>
        /// USed just for tests
        /// </summary>
        /// <param name="userId">An identifier for a MyLoAccount holder</param>
        /// <param name="photoId">An identifier for a Photo</param>
        public DataTable GetThumbnailForPhoto(long userId, Guid photoId)
        {
            try
            {
                DataSet ds = new DataSet();

                NpgsqlCommand command = new NpgsqlCommand("SELECT thumbnail FROM photo_base WHERE uniqueid = @id AND myloaccountid = @userid", _conn);
                command.Parameters.Add(new NpgsqlParameter("id", DbType.Guid));
                command.Parameters[0].Value = photoId;
                command.Parameters.Add(new NpgsqlParameter("userid", DbType.Int64));
                command.Parameters[1].Value = userId;

                NpgsqlDataAdapter da = new NpgsqlDataAdapter(command);

                da.Fill(ds);

                DataTable thumbs = ds.Tables[0];
                int count = thumbs.Rows.Count;

                _conn.Close();

                return thumbs;
            }
            catch (Exception ex)
            {
                throw new MyLoDataStoreException(ex.Message);
            }

        }

    }    

}

