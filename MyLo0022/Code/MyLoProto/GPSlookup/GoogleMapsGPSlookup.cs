/*

Copyright Keith Short 2012
keith_short@hotmail.com

This file is part of the MyLo application

------------------------------------------------------------------------
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Diagnostics;
using MyLoExceptions;
using System.Net;

namespace GPSlookupNS
{

    /// <summary>
    /// Uses Google Maps API to request a reverse lookup of GPS coordinates
    /// </summary>
    public class GoogleMapsGPSlookup : IGPSlookup 
    {
        private string latLongToAddressBaseUri = "http://maps.googleapis.com/maps/api/" +
                                "geocode/xml?latlng={0},{1}&sensor=false";

        //private string addressToLatLongBaseUri = "http://maps.googleapis.com/maps/api/" +
        //                        "geocode/xml?address={0}&sensor=false";


        private string Street;
        private string City;
        private string State;
        private string Zip;
        private string Country;

        public void LatLongToAddressLookup(double latitude, double longitude, out string street, out string city, out string state, out string zip, out string country)
        {
            Street = String.Empty;
            City = String.Empty;
            State = String.Empty;
            Zip = String.Empty;
            Country = String.Empty;
            Debug.WriteLine("{0} , {1}", latitude.ToString(), longitude.ToString());

            RetrieveFormatedAddressFromGoogle(latitude.ToString(), longitude.ToString());

            street = Street;
            city = City;
            state = State;
            zip = Zip;
            country = Country;
        }

        public void AddressToLatLongLookup(string street, string city, string state, string zip, string countrydouble, out double latitude, out double longitude)
        {
            latitude = 0;
            longitude = 0;
        }

        private void RetrieveFormatedAddressFromGoogle(string lat, string lng)
        {
            string requestUri = string.Format(latLongToAddressBaseUri, lat, lng);
            try
            {
                using (WebClient wc = new WebClient())
                {
                    string result = wc.DownloadString(requestUri);
                    DownloadStringCompleted(result);
                }
            }
            catch (Exception ex)
            {
                throw new MyLoException(ex.Message);
            }
        }

        //TODO need to setup wait time and to fail nicely if time response time exceeds limit

        private void DownloadStringCompleted(string result)
        {
            try
            {
                var xmlElm = XElement.Parse(result);

                var status = (from elm in xmlElm.Descendants()
                              where elm.Name == "status"
                              select elm).FirstOrDefault();
                if (status.Value.ToLower() == "ok")
                {
                    string longName = String.Empty;
                    string shortName = String.Empty;
                    IEnumerable<XElement> elements = xmlElm.Element("result").Elements("address_component");
                    foreach (XElement element in elements)
                    {
                        foreach (XElement item in element.Descendants())
                        {
                            //Debug.WriteLine("item: {0}", item);
                            if (item.Name == "long_name")
                            {
                                longName = item.Value;
                            }
                            else if (item.Name == "short_name")
                            {
                                shortName = item.Value;
                            }
                            else if (item.Value == "street_number")
                            {
                                Street = longName;
                            }
                            else if (item.Value == "route")
                            {
                                Street += " " + longName;
                            }
                            else if (item.Value == "locality")
                            {
                                City = longName;
                            }
                            else if (item.Value == "administrative_area_level_1")
                            {
                                State = shortName;
                            }
                            else if (item.Value == "postal_code")
                            {
                                Zip = longName;
                            }
                            else if (item.Value == "country")
                            {
                                Country = longName;
                            }
                        }
                    }
                    Debug.WriteLine("{0}, {1}, {2}, {3}, {4}", Street, City, State, Zip, Country);
                }
                else
                {
                    Debug.WriteLine("No Address Found");
                }
            }
            catch (Exception ex)
            {
                throw new MyLoException(ex.Message);
            }
        }
    }
}
