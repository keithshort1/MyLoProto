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
using MyLoExceptions;
using MyLoDBNS;
using System.Diagnostics;
using GPSlookupNS;

namespace MyLoFacebookContextReaderNS
{
    /// <summary>
    /// A MyLoFacebookContextReader encapsulates knowledge of Facebook queries, and their
    /// representation in a dotNetRDF graph
    /// </summary>
    public class MyLoFacebookContextReader
    {
        
        private TripleStore _store;
        private Graph _g;
        private MyLoDB _myloStore;
        private long _userId;
        public IGPSlookup gpsLookup;

        /// <summary>
        /// Construictir for MyLoFacebookContextReader
        /// </summary>
        /// <param name="gpsl">A GPSLookup instance implementing IGPSlookup</param>
        public MyLoFacebookContextReader(IGPSlookup gpsl)
        {
            _store = new TripleStore();
            _g = new Graph();
            _g.BaseUri = new Uri("http://facebook.MyLo.com");
            _store.Add(_g);
            gpsLookup = gpsl;
        }

        /// <summary>
        /// Initializes in memory Graph to hold context from previously saved file
        /// </summary>
        public void InitializeContextFromFile()
        {
            try
            {
                FileLoader.Load(_g, @"../../../SavedStore.n3");
                //FileLoader.Load(_g, @"C:\Users\Keith\Desktop\MyLo0001\Code\MyLO\SavedStore.n3");
                
            }
            catch (RdfParseException parseEx)
            {
                Debug.WriteLine("Parser Error");
                Debug.WriteLine(parseEx.Message);
            }
            catch (RdfException rdfEx)
            {
                Debug.WriteLine("RDF Error");
                Debug.WriteLine(rdfEx.Message);
            }
        }

        /// <summary>
        /// Using dotNetRDF Loader from the Facebook endpoint for Locations
        /// </summary>
        /// <param name="accessToken">Facebook OAuth access token</param>
        public void AddFacebookLocationsToContext(string accessToken)
        {
            String uriStrLocations = String.Format(@"https://graph.facebook.com/me/locations?access_token={0}", accessToken);
            String uriStrCheckins = String.Format(@"https://graph.facebook.com/me/checkins?access_token={0}", accessToken);
            Uri uriLocations = new Uri(uriStrLocations);
            Uri uriCheckins = new Uri(uriStrCheckins);
            try
            {
                //UriLoader.Load(_g, uriLocations);
                UriLoader.Load(_g, uriCheckins);
            }
            catch (RdfParseException parseEx)
            {
                Debug.WriteLine("Parser Error");
                Debug.WriteLine(parseEx.Message);
            }
            catch (RdfException rdfEx)
            {
                Debug.WriteLine("RDF Error");
                Debug.WriteLine(rdfEx.Message);
            }
        }


        /// <summary>
        /// Using FQL query against the Facebook endpoint for Events
        /// </summary>
        /// <param name="accessToken">Facebook OAuth access token</param>
        public void AddFacebookEventsToContext(string accessToken)
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
                Debug.WriteLine("Events Parser Error");
                Debug.WriteLine(parseEx.Message);
            }
            catch (RdfException rdfEx)
            {
                Debug.WriteLine("Events RDF Error");
                Debug.WriteLine(rdfEx.Message);
            }
            catch (System.Net.WebException rdfEx)
            {
                Debug.WriteLine("Web URI Exception");
                Debug.WriteLine(rdfEx.Message);
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
            //Debug.WriteLine(result4);

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
        /// Runs the First Pass Alignment rules in N3 format using a Reasoner
        /// </summary>
        public void MyLoFacebookAlignment01()
        {
            Graph rules = new Graph();
            try
            {
                FileLoader.Load(rules, @"../../../MyLoFacebookAlignmentRules01.n3");
            }
            catch (RdfParseException parseEx)
            {
                Debug.WriteLine("Parser Error - reading MyLoFacebookAlignmentRules01.n3");
                Debug.WriteLine(parseEx.Message);
            }
            catch (RdfException rdfEx)
            {
                Debug.WriteLine("RDF Error - reading MyLoFacebookAlignmentRules01.n3");
                Debug.WriteLine(rdfEx.Message);
            }

            SimpleN3RulesReasoner rulesReasoner = new SimpleN3RulesReasoner();
            try
            {
                rulesReasoner.Initialise(rules);
                foreach (IGraph g in _store.Graphs)
                {
                    rulesReasoner.Apply(g);
                }
            }
            catch (Exception rdfEx)
            {
                Debug.WriteLine("Rules Error - applying MyLoFacebookAlignmentRules01");
                Debug.WriteLine(rdfEx.Message);
            }
        }


        /// <summary>
        /// Runs the Second Pass Alignment rules in N3 format using a Reasoner
        /// </summary>
        public void MyLoFacebookAlignment02()
        {
            Graph rules = new Graph();
            try
            {
                FileLoader.Load(rules, @"../../../MyLoFacebookAlignmentRules02.n3");
            }
            catch (RdfParseException parseEx)
            {
                Debug.WriteLine("Parser Error - reading MyLoFacebookAlignmentRules02.n3");
                Debug.WriteLine(parseEx.Message);
            }
            catch (RdfException rdfEx)
            {
                Debug.WriteLine("RDF Error - reading MyLoFacebookAlignmentRules02.n3");
                Debug.WriteLine(rdfEx.Message);
            }

            SimpleN3RulesReasoner rulesReasoner = new SimpleN3RulesReasoner();
            try
            {
                rulesReasoner.Initialise(rules);
                foreach (IGraph g in _store.Graphs)
                {
                    rulesReasoner.Apply(g);
                }
            }
            catch (Exception rdfEx)
            {
                Debug.WriteLine("Rules Error - applying MyLoFacebookAlignmentRules02");
                Debug.WriteLine(rdfEx.Message);
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
                Debug.WriteLine("Parser Error - reading MyLoSchema.n3");
                Debug.WriteLine(parseEx.Message);
            }
            catch (RdfException rdfEx)
            {
                Debug.WriteLine("RDF Error - reading MyLoSchema.n3");
                Debug.WriteLine(rdfEx.Message);
            }

            StaticRdfsReasoner reasoner = new StaticRdfsReasoner();

            try
            {
                reasoner.Initialise(schema);
                foreach (IGraph g in _store.Graphs)
                {
                    reasoner.Apply(g);
                }
            }
            catch (Exception rdfEx)
            {
                Debug.WriteLine("Rules Error - applying RDFS Reasoner");
                Debug.WriteLine(rdfEx.Message);
            }
        }


        /// <summary>
        /// Runs a SPARQL query against the graph
        /// </summary>
        /// <param name="sparqlQ">string containing a valid SPARQL query</param>
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
                Debug.WriteLine("RDF Error");
                Debug.WriteLine(rdfEx.Message);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Non RDF Error");
                Debug.WriteLine(ex.Message);
            }
            return display += execTime;
        }


        /// <summary>
        /// Outputs contents of the Graph to a text document
        /// </summary>
        /// <param name="strWriter">StreamWriter to contain written output of Graph</param>
        public void WriteGraph(StreamWriter strWriter)
        {
            foreach (Triple t in _store.Triples)
            {
                strWriter.WriteLine(t.ToString());

            }
        }


        /// <summary>
        /// Saves Graph containing Facebook context items to a file for later restore
        /// </summary>
        public void SaveContextToFile()
        {
            foreach (IGraph g in _store.Graphs)
            {
                Notation3Writer n3w = new Notation3Writer();
                n3w.Save(_g, @"../../../SavedStore.n3");
            }
            
        }


        /// <summary>
        /// Saves Graph containing Facebook context items to MyLo database
        /// </summary>
        public void SaveContextToDB(string myloUserName)
        {
            _myloStore = new MyLoDB();
            _userId = _myloStore.GetUserAccount(myloUserName);
            if (_userId != 0)
            {
                try
                {
                    AddFacebookPosts();
                }
                catch (Exception ex)
                {
                    throw new MyLoException("Save Context Error - Facebook Posts: Inner exception: " + ex); 
                }
                try
                {
                    AddFacebookCheckins();
                }
                catch (Exception ex)
                {
                    throw new MyLoException("Save Context Error - Facebook Checkins: Inner exception: " + ex);
                }
                try
                {
                    AddFacebookPeople();
                }
                catch (Exception ex)
                {
                    throw new MyLoException("Save Context Error - Facebook Checkins: Inner exception: " + ex);
                }
            }
            else
            {
                throw new MyLoException(String.Format("Invalid User Account {0}", myloUserName));
            }

        }

        /// <summary>
        /// Queries Graph containing Facebook context items for Facebook Post info
        /// and calls MyLoDB method to add to DB
        /// </summary>
        private void AddFacebookPeople()
        {
            string sparqlQ = "PREFIX mylo: <http://mylo.com/schema/> " +
                             "PREFIX rdfs: <http://www.w3.org/2000/01/rdf-schema#> " +
                             "PREFIX rdf: <http://www.w3.org/1999/02/22-rdf-syntax-ns#> " +
                             "SELECT DISTINCT ?fbname ?fbid " +
                             "WHERE {" +
                             "      ?person rdf:type mylo:Person ." +
                             "      ?person mylo:FacebookId ?fbid ." +
                             "      ?person mylo:FacebookName ?fbname ." +
                             "}";
            try
            {
                SparqlQueryParser sparqlparser = new SparqlQueryParser();
                SparqlQuery query = sparqlparser.ParseFromString(sparqlQ);
                Object results = _store.ExecuteQuery(query);
                if (results is SparqlResultSet)
                {
                    Dictionary<string, INode> vals = new Dictionary<string, INode>();
                    SparqlResultSet rset = (SparqlResultSet)results;
                    foreach (SparqlResult r in rset)
                    {
                        foreach (string var in rset.Variables)
                        {
                            if (r.HasValue(var))
                            {
                                vals[var] = r[var];
                            }
                            else
                            {
                                vals[var] = null;
                            }
                        }
                        try
                        {
                            Party person = new Party();
                            if (vals["fbname"] != null)
                            {
                                person.Name = vals["fbname"].ToString();
                                person.PartyKind = "Person";
                                long result = _myloStore.AddParty(_userId, person);
                            } 
                        }
                        catch (MyLoDataStoreException dsEx)
                        {
                            throw new MyLoException("DataStore Error: Inner exception: " + dsEx);
                        }
                        catch (Exception ex)
                        {
                            throw new MyLoException("Add People Error: Inner exception: " + ex);
                        }
                    }
                }
            }
            catch (RdfException rdfEx)
            {
                throw new MyLoException("Save Context People Error: Inner exception: " + rdfEx);
            }
            catch (Exception ex)
            {
                throw new MyLoException("Save Context People Error: Inner exception: " + ex);
            }
        }


        /// <summary>
        /// Queries Graph containing Facebook context items for Facebook Post info
        /// and calls MyLoDB method to add to DB
        /// </summary>
        private void AddFacebookPosts()
        {
            string sparqlQ = "PREFIX mylo: <http://mylo.com/schema/> " +
                             "PREFIX rdfs: <http://www.w3.org/2000/01/rdf-schema#> " +
                             "PREFIX rdf: <http://www.w3.org/1999/02/22-rdf-syntax-ns#> " +
                             "SELECT DISTINCT ?activity ?time ?placename ?sourceId ?lat ?long ?state ?street ?city ?zip ?country " +
                             "WHERE {" +
                             "      ?activity rdf:type mylo:Activity ." +
                             "      ?activity rdf:type mylo:FacebookPost ." +
                             "      ?activity mylo:HasLocation ?loc ." +
                             "      ?activity mylo:HasSourceId ?sourceId ." +
                             "      ?loc mylo:HasLocationName ?placename ." +
                             "      ?loc mylo:HasGpsLatitude ?lat ." +
	                         "      ?loc mylo:HasGpsLongitude ?long ." +
                             "      ?activity mylo:HasTimePeriod ?period ." +
                             "      ?period rdf:type mylo:Instant ." +
                             "      ?period mylo:HasTimeValue ?time ." +
                             "      OPTIONAL { ?loc mylo:HasAddress ?adr }" +
                             "      OPTIONAL { ?adr mylo:HasState ?state }" +
                             "      OPTIONAL { ?adr mylo:HasCity ?city }" +
                             "      OPTIONAL { ?adr mylo:HasZip ?zip }" +
                             "      OPTIONAL { ?adr mylo:HasCountry ?country }" +
                             "      OPTIONAL { ?adr mylo:HasStreet ?street }" +
                             "}";
            try
            {
                SparqlQueryParser sparqlparser = new SparqlQueryParser();
                SparqlQuery query = sparqlparser.ParseFromString(sparqlQ);
                Object results = _store.ExecuteQuery(query);
                if (results is SparqlResultSet)
                {
                    Dictionary<string, INode> vals = new Dictionary<string, INode>();
                    SparqlResultSet rset = (SparqlResultSet)results;
                    foreach (SparqlResult r in rset)
                    {
                        foreach (string var in rset.Variables)
                        {

                            if (r.HasValue(var))
                            {
                                vals[var] = r[var];
                            }
                            else
                            {
                                vals[var] = null;
                            }
                        }
                        try
                        {
                            Activity act = new Activity();
                            Location loc = new Location();
                            TimePeriod startDate = new TimePeriod();
                            TimePeriod endDate = new TimePeriod();
                            if (vals["sourceId"] != null)
                            {
                                act.SourceId = vals["sourceId"].ToString();
                            }
                            if (vals["time"] != null)
                            {
                                string[] parts = vals["time"].ToString().Split('^');
                                startDate.AltKey = Convert.ToDateTime(parts[0]);
                                endDate.AltKey = DateTime.MinValue;
                                act.StartDate = startDate.AltKey;
                                startDate.Hour = (short)startDate.AltKey.Hour;
                                startDate.Year = (short)startDate.AltKey.Year;
                                startDate.Month = (short)startDate.AltKey.Month;
                                startDate.Day = startDate.AltKey.DayOfWeek.ToString();
                                startDate.DayNumber = (short)startDate.AltKey.Day;
                            }
                            if (vals["placename"] != null)
                            {
                                act.LocationName = vals["placename"].ToString();
                            }
                            if (vals["lat"] != null)
                            {
                                string[] parts = vals["lat"].ToString().Split('^');
                                act.Latitude = Convert.ToDouble(parts[0]);
                            }
                            if (vals["long"] != null)
                            {
                                string[] parts = vals["long"].ToString().Split('^');
                                act.Longitude = Convert.ToDouble(parts[0]);
                            }

                            Location locGps = new Location(); ;
                            if (vals["lat"] != null && vals["long"] != null)
                            {
                                locGps = ReverseLookupGPScoordinates(act.Latitude, act.Longitude);
                            }

                            if (vals["street"] != null)
                            {
                                loc.Street = vals["street"].ToString();
                            }
                            else
                            {
                                if (locGps.Street != null || locGps.Street != " ")
                                {
                                    loc.Street = locGps.Street;
                                }
                            }
                            if (vals["city"] != null)
                            {
                                loc.City = vals["city"].ToString();
                            }
                            else
                            {
                                if (locGps.City != null || locGps.City != " ")
                                {
                                    loc.City = locGps.City;
                                }
                            }
                            if (vals["state"] != null)
                            {
                                loc.State = vals["state"].ToString();
                            }
                            else
                            {
                                if (locGps.State != null || locGps.State != " ")
                                {
                                    loc.State = locGps.State;
                                }
                            }
                            if (vals["zip"] != null)
                            {
                                loc.Zip = vals["zip"].ToString();
                            }
                            else
                            {
                                if (locGps.Zip != null || locGps.Zip != " ")
                                {
                                    loc.Zip = locGps.Zip;
                                }
                            }
                            if (vals["country"] != null)
                            {
                                loc.Country = vals["country"].ToString();
                            }
                            else
                            {
                                if (locGps.Country != null || locGps.Country != " ")
                                {
                                    loc.Country = locGps.Country;
                                }
                            }

                            //if (vals["lat"] != null && vals["long"] != null)
                            //{
                            //    loc = ReverseLookupGPScoordinates(act.Latitude, act.Longitude);
                            //}
                            //else
                            //{
                            //    if (vals["street"] != null)
                            //    {
                            //        loc.Street = vals["street"].ToString();
                            //    }
                            //    if (vals["city"] != null)
                            //    {
                            //        loc.City = vals["city"].ToString();
                            //    }
                            //    if (vals["state"] != null)
                            //    {
                            //        loc.State = vals["state"].ToString();
                            //    }
                            //    if (vals["zip"] != null)
                            //    {
                            //        loc.Zip = vals["zip"].ToString();
                            //    }
                            //    if (vals["country"] != null)
                            //    {
                            //        loc.Country = vals["country"].ToString();
                            //    }
                            //}

                            act.Source = "Facebook";
                            act.ActivityKind = "FacebookPost";
                            long result = _myloStore.AddActivity(_userId, act, startDate, endDate, loc);
                        }
                        catch (MyLoDataStoreException dsEx)
                        {
                            throw new MyLoException("DataStore Error: Inner exception: " + dsEx);
                        }
                        catch (Exception ex)
                        {
                            throw new MyLoException("Add Activity Error: Inner exception: " + ex);
                        }
                    }
                    
                }
            }
            catch (RdfException rdfEx)
            {
                throw new MyLoException("Save Context Error: Inner exception: " + rdfEx);
            }
            catch (Exception ex)
            {
                throw new MyLoException("Save Context Error: Inner exception: " + ex);
            }
        }


        /// <summary>
        /// Queries Graph containing Facebook context items for Facebook Checkin info
        /// and calls MyLoDB method to add to DB
        /// </summary>
        private void AddFacebookCheckins()
        {
            string sparqlQ = "PREFIX mylo: <http://mylo.com/schema/> " +
                             "PREFIX rdfs: <http://www.w3.org/2000/01/rdf-schema#> " +
                             "PREFIX rdf: <http://www.w3.org/1999/02/22-rdf-syntax-ns#> " +
                             "SELECT  DISTINCT ?activity ?time ?placename ?sourceId ?lat ?long ?state ?street ?city ?zip ?country " +
                             "WHERE {" +
                             "      ?activity rdf:type mylo:Activity ." +
                             "      ?activity rdf:type mylo:FacebookCheckin ." +
                             "      ?activity mylo:HasLocation ?loc ." +
                             "      ?activity mylo:HasSourceId ?sourceId ." +
                             "      ?loc mylo:HasLocationName ?placename ." +
                             "      ?loc mylo:HasGpsLatitude ?lat ." +
                             "      ?loc mylo:HasGpsLongitude ?long ." +
                             "      ?activity mylo:HasTimePeriod ?period ." +
                             "      ?period rdf:type mylo:Instant ." +
                             "      ?period mylo:HasTimeValue ?time ." +
                             "      OPTIONAL { ?loc mylo:HasAddress ?adr }" +
                             "      OPTIONAL { ?adr mylo:HasState ?state }" +
                             "      OPTIONAL { ?adr mylo:HasCity ?city }" +
                             "      OPTIONAL { ?adr mylo:HasZip ?zip }" +
                             "      OPTIONAL { ?adr mylo:HasCountry ?country }" +
                             "      OPTIONAL { ?adr mylo:HasStreet ?street }" +
                             "}";
            try
            {
                SparqlQueryParser sparqlparser = new SparqlQueryParser();
                SparqlQuery query = sparqlparser.ParseFromString(sparqlQ);
                Object results = _store.ExecuteQuery(query);
                if (results is SparqlResultSet)
                {
                    Dictionary<string, INode> vals = new Dictionary<string, INode>();
                    SparqlResultSet rset = (SparqlResultSet)results;
                    foreach (SparqlResult r in rset)
                    {
                        foreach (string var in rset.Variables)
                        {

                            if (r.HasValue(var))
                            {
                                vals[var] = r[var];
                            }
                            else
                            {
                                vals[var] = null;
                            }
                        }
                        try
                        {
                            Activity act = new Activity();
                            Location loc = new Location();
                            TimePeriod startDate = new TimePeriod();
                            TimePeriod endDate = new TimePeriod();
                            if (vals["sourceId"] != null)
                            {
                                act.SourceId = vals["sourceId"].ToString();
                            }
                            if (vals["time"] != null)
                            {
                                string[] parts = vals["time"].ToString().Split('^');
                                startDate.AltKey = Convert.ToDateTime(parts[0]);
                                //endDate.AltKey = DateTime.MinValue;
                                endDate.AltKey = startDate.AltKey;
                                act.StartDate = startDate.AltKey;
                                act.EndDate = endDate.AltKey;
                                startDate.Hour = (short)startDate.AltKey.Hour;
                                startDate.Year = (short)startDate.AltKey.Year;
                                startDate.Month = (short)startDate.AltKey.Month;
                                startDate.Day = startDate.AltKey.DayOfWeek.ToString();
                                startDate.DayNumber = (short)startDate.AltKey.Day;
                            }
                            if (vals["placename"] != null)
                            {
                                act.LocationName = vals["placename"].ToString();
                            }
                            if (vals["lat"] != null)
                            {
                                string[] parts = vals["lat"].ToString().Split('^');
                                act.Latitude = Convert.ToDouble(parts[0]); ;
                            }
                            if (vals["long"] != null)
                            {
                                string[] parts = vals["long"].ToString().Split('^');
                                act.Longitude = Convert.ToDouble(parts[0]);
                            }

                            Location locGps = new Location();
                            if (vals["lat"] != null && vals["long"] != null)
                            {
                                locGps = ReverseLookupGPScoordinates(act.Latitude, act.Longitude);
                            }

                            if (vals["street"] != null)
                            {
                                loc.Street = vals["street"].ToString();
                            }
                            else
                            {
                                if (locGps.Street != null || locGps.Street != " ")
                                {
                                    loc.Street = locGps.Street;
                                }
                            }
                            if (vals["city"] != null)
                            {
                                loc.City = vals["city"].ToString();
                            }
                            else
                            {
                                if (locGps.City != null || locGps.City != " ")
                                {
                                    loc.City = locGps.City;
                                }
                            }
                            if (vals["state"] != null)
                            {
                                loc.State = vals["state"].ToString();
                            }
                            else
                            {
                                if (locGps.State != null || locGps.State != " ")
                                {
                                    loc.State = locGps.State;
                                }
                            }
                            if (vals["zip"] != null)
                            {
                                loc.Zip = vals["zip"].ToString();
                            }
                            else
                            {
                                if (locGps.Zip != null || locGps.Zip != " ")
                                {
                                    loc.Zip = locGps.Zip;
                                }
                            }
                            if (vals["country"] != null)
                            {
                                loc.Country = vals["country"].ToString();
                            }
                            else
                            {
                                if (locGps.Country != null || locGps.Country != " ")
                                {
                                    loc.Country = locGps.Country;
                                }
                            }




                            //if (vals["lat"] != null && vals["long"] != null)
                            //{
                            //    Location locGps = ReverseLookupGPScoordinates(act.Latitude, act.Longitude);
                            //}
                            //else
                            //{
                            //    if (vals["street"] != null)
                            //    {
                            //        loc.Street = vals["street"].ToString();
                            //    }
                            //    if (vals["city"] != null)
                            //    {
                            //        loc.City = vals["city"].ToString();
                            //    }
                            //    if (vals["state"] != null)
                            //    {
                            //        loc.State = vals["state"].ToString();
                            //    }
                            //    if (vals["zip"] != null)
                            //    {
                            //        loc.Zip = vals["zip"].ToString();
                            //    }
                            //    if (vals["country"] != null)
                            //    {
                            //        loc.Country = vals["country"].ToString();
                            //    }
                            //}

                            act.Source = "Facebook";
                            act.ActivityKind = "FacebookCheckin";
                            Debug.WriteLine("Loc is: {0}, {1}, {2}, {3}, {4}", loc.Street, loc.City, loc.State, loc.Zip, loc.Country);
                            long result = _myloStore.AddActivity(_userId, act, startDate, endDate, loc);
                        }
                        catch (MyLoDataStoreException dsEx)
                        {
                            throw new MyLoException("DataStore Error: Inner exception: " + dsEx);
                        }
                        catch (Exception ex)
                        {
                            throw new MyLoException("Add Activity Error: Inner exception: " + ex);
                        }
                    }

                }
            }
            catch (RdfException rdfEx)
            {
                throw new MyLoException("Save Context RDF Error: Inner exception: " + rdfEx);
            }
            catch (Exception ex)
            {
                throw new MyLoException("Save Context Error: Inner exception: " + ex);
            }
        }


        private Location ReverseLookupGPScoordinates(double latitude, double longitude)
        {
            string city = String.Empty, street = String.Empty, zip = String.Empty, country = String.Empty, state = String.Empty;
            Location loc = new Location();
            gpsLookup.LatLongToAddressLookup(latitude, longitude, out street, out city, out state, out zip, out country);
            loc.Street = street;
            loc.City = city;
            loc.State = state;
            loc.Zip = zip;
            loc.Country = country;
            return loc;
        }

    }
}
