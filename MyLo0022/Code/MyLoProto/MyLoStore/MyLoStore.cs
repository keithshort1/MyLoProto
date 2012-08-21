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
using VDS.RDF;
using VDS.RDF.Writing;
using VDS.RDF.Query.Inference;
using VDS.RDF.Parsing;
using VDS.RDF.Query;
using Newtonsoft;
using Npgsql;
using MyLoExceptions;

namespace MyLoStoreNS
{
    public class MyLoStore
    {

        private NpgsqlConnection _conn;
        long _userId;

        /// <summary>
        /// Creates a new MyLo Store in the given Database
        /// </summary>
        /// <param name="dbName">Database Name</param>
        public MyLoStore(string dbName)
        {
            _conn = new NpgsqlConnection(String.Format("Server=localhost;Port=5432;User Id=postgres;Password=123abc;Database={0};", dbName));
            _conn.Open();
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
        /// <param name="dbName">Input MyLo User Name</param>
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
                this._userId = (long)result;
                return this._userId;
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
        /// <param name="root">photo meta data
        public void AddPhotoToDataStore(Guid uuid, double gpsLat, double gpsLong, string ap, string camera, DateTime dateTaken, string fileName, long CRC)
        {
            try
            {
                NpgsqlCommand command = new NpgsqlCommand("AddPhoto", _conn);
                command.CommandType = CommandType.StoredProcedure;

                // creat 8 parameters for this function
                for (int i = 0; i < 8; i++)
                {
                    command.Parameters.Add(new NpgsqlParameter());
                }

                command.Parameters[0].DbType = DbType.Int64;
                command.Parameters[0].Value = _userId;
                command.Parameters[1].DbType = DbType.String;
                command.Parameters[1].Value = fileName;
                command.Parameters[2].DbType = DbType.Guid;
                command.Parameters[2].Value = uuid;
                command.Parameters[3].DbType = DbType.Int64;
                command.Parameters[3].Value = CRC;
                command.Parameters[4].DbType = DbType.DateTime;
                command.Parameters[4].Value = dateTaken;
                command.Parameters[5].DbType = DbType.String;
                command.Parameters[5].Value = camera;
                command.Parameters[6].DbType = DbType.Double;
                command.Parameters[6].Value = gpsLat;
                command.Parameters[7].DbType = DbType.Double;
                command.Parameters[7].Value = gpsLong;

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

    }
}
