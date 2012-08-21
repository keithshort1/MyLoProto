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
using System.Net;
using System.Xml.Linq;
using System.Diagnostics;
using GPSlookupNS.BingMapsGeocode;
using MyLoExceptions;


namespace GPSlookupNS
{

    /// <summary>
    /// Uses Bing Maps API to request a reverse lookup of GPS coordinates
    /// </summary>
    public class BingMapsGPSlookup : IGPSlookup 
    {
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

            RetrieveFormatedAddressFromBing(latitude, longitude);

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


        private void RetrieveFormatedAddressFromBing(double lat, double lng)
        {
            string results = string.Empty;
            try
            {
                ReverseGeocodeRequest reverseGeocodeRequest = new ReverseGeocodeRequest();

                // Set the credentials using a valid Bing Maps key
                reverseGeocodeRequest.Credentials = new BingMapsGeocode.Credentials();
                reverseGeocodeRequest.Credentials.ApplicationId = "Agc9SoSDINTGiryq7Ns1ddNUS3sQQLXT4eQqXGhkDka9QkMxfudjMs9k5wwPEhDI";

                // Set the point to use to find a matching address
                BingMapsGeocode.Location point = new BingMapsGeocode.Location();
                point.Latitude = lat;
                point.Longitude = lng;

                reverseGeocodeRequest.Location = point;

                // Make the reverse geocode request
                GeocodeServiceClient geocodeService = new GeocodeServiceClient("BasicHttpBinding_IGeocodeService");
                GeocodeResponse geocodeResponse = geocodeService.ReverseGeocode(reverseGeocodeRequest);
                if (geocodeResponse.Results != null)
                {
                    Street = geocodeResponse.Results[0].Address.AddressLine;
                    City = geocodeResponse.Results[0].Address.Locality;
                    State = geocodeResponse.Results[0].Address.AdminDistrict;
                    Zip = geocodeResponse.Results[0].Address.PostalCode;
                    Country = geocodeResponse.Results[0].Address.CountryRegion;
                    Debug.WriteLine("{0}, {1}, {2}, {3}, {4}", Street, City, State, Zip, Country);
                }

                //geocodeService.ReverseGeocodeCompleted += new EventHandler<ReverseGeocodeCompletedEventArgs>(geocodeService_ReverseGeocodeCompleted);
                //geocodeService.ReverseGeocodeAsync(reverseGeocodeRequest);
            }
            catch (Exception ex)
            {
                throw new MyLoException(ex.Message);
            }
        }

        //static void geocodeService_ReverseGeocodeCompleted(object sender, ReverseGeocodeCompletedEventArgs e)
        //{
        //    Street = e.Result.Results[0].Address.Street;
        //    City = e.Result.Results[0].Address.PostalTown;
        //    //txtInfo.Text = string.Format("Location:{3}{0} {2}, {1}", e.Result.Results[0].Address.CountryRegion, e.Result.Results[0].Address.PostalCode, e.Result.Results[0].Address.PostalTown, System.Environment.NewLine);
        //    Zip = e.Result.Results[0].Address.PostalCode;
        //}

    }

}
