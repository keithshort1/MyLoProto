using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MyLoFacebookContextReaderNS;
using System.Diagnostics;
using GPSlookupNS;

namespace MyLoTests
{
    [TestClass]
    public class QueryTests
    {
        [TestMethod]
        public void QueryTest01()
        {
            string sparqlQ = "PREFIX mylo: <http://mylo.com/schema/> " +
                             "PREFIX rdfs: <http://www.w3.org/2000/01/rdf-schema#> " +
                             "PREFIX rdf: <http://www.w3.org/1999/02/22-rdf-syntax-ns#> " +
                             "SELECT DISTINCT ?activity ?time ?placename ?sourceId ?lat ?long ?state ?street ?city ?zip ?country " +
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
                BingMapsGPSlookup gps = new BingMapsGPSlookup();
                MyLoFacebookContextReader _fbContext = new MyLoFacebookContextReader(gps);
                _fbContext.InitializeContextFromFile();
                Debug.WriteLine(_fbContext.RunSPARQLQuery(sparqlQ));
            }
            catch (Exception ex)
            {
                Debug.WriteLine("QueryTest01 Error");
                Debug.WriteLine(ex.Message);
            }
        }

        [TestMethod]
        public void QueryTest02()
        {
            string sparqlQ = "PREFIX mylo: <http://mylo.com/schema/> " +
                             "PREFIX rdfs: <http://www.w3.org/2000/01/rdf-schema#> " +
                             "PREFIX rdf: <http://www.w3.org/1999/02/22-rdf-syntax-ns#> " +
                             "PREFIX : <http://graph.facebook.com/schema/~/> " +
                             "SELECT  ?x ?y ?c  " +
                             "WHERE {" +
                             "      ?x rdf:type mylo:Address ." +
                             "      ?x mylo:HasAddressProps :y ." +
                             "      ?y :city ?c ." +
                             "}";
            try
            {
                BingMapsGPSlookup gps = new BingMapsGPSlookup();
                MyLoFacebookContextReader _fbContext = new MyLoFacebookContextReader(gps);
                _fbContext.InitializeContextFromFile();
                Debug.WriteLine(_fbContext.RunSPARQLQuery(sparqlQ));
            }
            catch (Exception ex)
            {
                Debug.WriteLine("QueryTest02 Error");
                Debug.WriteLine(ex.Message);
            }
        }
    }
}
