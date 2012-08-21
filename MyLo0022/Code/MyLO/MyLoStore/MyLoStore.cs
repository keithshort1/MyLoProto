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

namespace MyLoStoreNS
{
    public class MyLoStore
    {
        private TripleStore _store;
        private Graph _g;
        private NpgsqlConnection _conn;
        long _userId;

        /// <summary>
        /// Creates a new MyLo Store in the given Database
        /// </summary>
        /// <param name="dbName">Database Name</param>
        public MyLoStore(string dbName)
        {
            InitializeStore(dbName);
        }

        /// <summary>
        /// Initializes new MyLo Store in the given Database and its corresponding in memory Graph
        /// </summary>
        /// <param name="dbName">Database Name</param>
        private void InitializeStore(string dbName)
        {
            _store = new TripleStore();
            _g = new Graph();
            _conn = new NpgsqlConnection(String.Format("Server=localhost;Port=5432;User Id=postgres;Password=123abc;Database={0};", dbName));
            _conn.Open();
            _g.BaseUri = new Uri("http://facebook.MyLo.com");
            try
            {
                //FileLoader.Load(_g, @"../../../SavedStore.n3");
                FileLoader.Load(_g, @"C:\Users\Keith\Desktop\MyLo0001\Code\MyLO\SavedStore.n3");
                _store.Add(_g);
            }
            catch (RdfParseException parseEx)
            {
                Console.WriteLine("Parser Error");
                Console.WriteLine(parseEx.Message);
            }
            catch (RdfException rdfEx)
            {
                Console.WriteLine("RDF Error");
                Console.WriteLine(rdfEx.Message);
            }
        }

        public void CloseStore()
        {
            _conn.Close();
        }


        /// <summary>
        /// Using dotNetRDF Loader from the Facebook endpoint for Locations
        /// </summary>
        /// <param name="accessToken">Facebook OAuth access token</param>
        public void AddFacebookLocationsToGraph(string accessToken)
        {
            String uriStr = String.Format(@"https://graph.facebook.com/me/locations?access_token={0}", accessToken);
            Uri uri = new Uri(uriStr);
            try
            {
                UriLoader.Load(_g, uri);
            }
            catch (RdfParseException parseEx)
            {
                Console.WriteLine("Parser Error");
                Console.WriteLine(parseEx.Message);
            }
            catch (RdfException rdfEx)
            {
                Console.WriteLine("RDF Error");
                Console.WriteLine(rdfEx.Message);
            }
        }


        /// <summary>
        /// Using FQL query against the Facebook endpoint for Events
        /// </summary>
        /// <param name="accessToken">Facebook OAuth access token</param>
        public void AddFacebookEventsToGraph(string accessToken)
        {
            //String queryStr1 = "{ \"fbEventMember\":\"SELECT eid  FROM event_member WHERE uid=me()\",";
            //String queryStr2 = "\"fbevent\":\"SELECT creator, name, location, start_time, end_time FROM event WHERE eid IN ( SELECT eid FROM #fbEventMember)\"}";
            //String uriStr = String.Format(@"https://graph.facebook.com/fql?q={0}&access_token={1}", queryStr1 + queryStr2, accessToken);

            String queryStr1 = "SELECT creator, name, location, start_time, end_time, eid  FROM event WHERE eid IN ( SELECT eid FROM event_member WHERE uid=me())";
            String uriStr = String.Format(@"https://graph.facebook.com/fql?q={0}&access_token={1}", queryStr1, accessToken);


            //String uriStr = String.Format(@"https://graph.facebook.com/me/events?access_token={0}", accessToken);

            Uri uri = new Uri(uriStr);
            try
            {
                UriLoader.Load(_g, uri);
            }
            catch (RdfParseException parseEx)
            {
                Console.WriteLine("Events Parser Error");
                Console.WriteLine(parseEx.Message);
            }
            catch (RdfException rdfEx)
            {
                Console.WriteLine("Events RDF Error");
                Console.WriteLine(rdfEx.Message);
            }
            catch (System.Net.WebException rdfEx)
            {
                Console.WriteLine("Web URI Exception");
                Console.WriteLine(rdfEx.Message);
            }


            //dynamic result4 = fb.Get("fql",
            //    new
            //    {
            //        q = new
            //        {
            //            fbEventMember = "SELECT uid, eid  FROM event_member WHERE uid=me()",
            //            fbevent = "SELECT name, location, start_time, end_time FROM event WHERE eid IN ( SELECT eid FROM #fbEventMember)",
            //        }
            //    });

            //int friendCount = 0;
            //Console.WriteLine(result4);

            //foreach (var res in result4.data)
            //{
            //    if (res.name == "friend")
            //    {
            //        foreach (var f in res.fql_result_set)
            //        {

            //            string fName = f.name;
            //            friendCount++;
            //        }
            //    }
            //}
        }

        /// <summary>
        /// Runs the Alignment rules in N3 format using a Reasoner
        /// </summary>
        public void MyLoFacebookAlignment()
        {
            Graph rules = new Graph();
            try
            {
                FileLoader.Load(rules, @"../../../MyLoFacebookAlignmentRules.n3");
            }
            catch (RdfParseException parseEx)
            {
                Console.WriteLine("Parser Error - reading MyLoFacebookAlignmentRules.n3");
                Console.WriteLine(parseEx.Message);
            }
            catch (RdfException rdfEx)
            {
                Console.WriteLine("RDF Error - reading MyLoFacebookAlignmentRules.n3");
                Console.WriteLine(rdfEx.Message);
            }

            SimpleN3RulesReasoner rulesReasoner = new SimpleN3RulesReasoner();
            try
            {
                rulesReasoner.Initialise(rules);
                rulesReasoner.Apply(_g);
            }
            catch (Exception rdfEx)
            {
                Console.WriteLine("Rules Error - applying MyLoFacebookAlignmentRules");
                Console.WriteLine(rdfEx.Message);
            }
        }


        /// <summary>
        /// Runs the rules inherent in the MyLo schema in N3 format using a static Reasoner
        /// </summary>
        public void MyLoSchemaReasoner()
        {
            Graph schema = new Graph();
            try
            {
                FileLoader.Load(schema, @"../../../MyLoSchema.n3");
            }
            catch (RdfParseException parseEx)
            {
                Console.WriteLine("Parser Error - reading MyLoSchema.n3");
                Console.WriteLine(parseEx.Message);
            }
            catch (RdfException rdfEx)
            {
                Console.WriteLine("RDF Error - reading MyLoSchema.n3");
                Console.WriteLine(rdfEx.Message);
            }

            StaticRdfsReasoner reasoner = new StaticRdfsReasoner();

            try
            {
                reasoner.Initialise(schema);
                reasoner.Apply(_g);
            }
            catch (Exception rdfEx)
            {
                Console.WriteLine("Rules Error - applying RDFS Reasoner");
                Console.WriteLine(rdfEx.Message);
            } 
        }


        /// <summary>
        /// Runs a SPARQL query against the graph
        /// </summary>
        public string RunSPARQLQuery(string sparqlQ)
        {
            string display = String.Empty;
            string execTime = "\r\nExecution Time: ";
            try
            {
                SparqlQueryParser sparqlparser = new SparqlQueryParser();
                SparqlQuery query = sparqlparser.ParseFromString(sparqlQ);
                Object results = _store.ExecuteQuery(query);
                if (results is SparqlResultSet)
                {
                    //Print out the Results
                    execTime += query.QueryExecutionTime.ToString();
                    SparqlResultSet rset = (SparqlResultSet)results;
                    foreach (SparqlResult result in rset)
                    {
                        display += result.ToString();
                        display += "\r\n";
                    }
                }
            }
            catch (RdfException rdfEx)
            {
                Console.WriteLine("RDF Error");
                Console.WriteLine(rdfEx.Message);
            }
            return display += execTime;
        }

        public void WriteGraph(StreamWriter strWriter)
        {
            foreach (Triple t in _g.Triples)
            {
                strWriter.WriteLine(t.ToString());

            }
        }

        public void SaveGraphToFile()
        {
            Notation3Writer n3w = new Notation3Writer();
            n3w.Save(_g, @"../../../SavedStore.n3");
        }

        public void SaveGraphToDB(string userName)
        {
            throw new NotImplementedException();
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
