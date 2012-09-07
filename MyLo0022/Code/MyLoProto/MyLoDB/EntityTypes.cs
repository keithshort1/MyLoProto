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

namespace MyLoDBNS
{
    public class Activity
    {
        public long ActivityId { get; set; }
        public string ActivityKind { get; set; }
        public string Source { get; set; }
        public string SourceId { get; set; }
        public string ActivityName { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public Activity()
        {
            this.ActivityKind = String.Empty;
            this.Source = String.Empty;
            this.SourceId = String.Empty;
            this.ActivityName = String.Empty;
            this.Latitude = 0;
            this.Longitude = 0;
            this.StartDate = DateTime.MinValue;
            this.EndDate = DateTime.MinValue;
        }
    }


    public class Address
    {
        public long AddressId { get; set; }
        public string Street { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Zip { get; set; }
        public string Country { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }

        public Address()
        {
            this.City = String.Empty;
            this.Street = String.Empty;
            this.State = String.Empty;
            this.Zip = String.Empty;
            this.Country = String.Empty;
            this.Latitude = 0;
            this.Longitude = 0;
        }
    }

    public class GeoLocation
    {
        public long LocationId { get; set; }
        public string LocationName { get; set; }
        public string LocationKind { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }

        public GeoLocation()
        {
            this.LocationName = String.Empty;
            this.LocationKind = String.Empty;
            this.Latitude = 0;
            this.Longitude = 0;
        }
    }

    public class Photo
    {
        public string PhotoIndexKind { get; set; }
        public Guid Uuid { get; set; }
        public double GpsLat { get; set; }
        public double GpsLong { get; set; }
        public string Aperture { get; set; }
        public string Camera { get; set; }
        public DateTime DateTaken { get; set; }
        public string Uri { get; set; }
        public long CRC { get; set; }
        public byte[] Thumbnail { get; set; }

        public Photo()
        {
            this.PhotoIndexKind = String.Empty;
            this.Uuid = Guid.Empty;
            this.GpsLat = 0;
            this.GpsLong = 0;
            this.Aperture = String.Empty;
            this.Camera = String.Empty;
            this.DateTaken = DateTime.MinValue;
            this.Uri = String.Empty;
            this.CRC = 0;
            this.Thumbnail = null;
        }
    }

    public class TimePeriod
    {
        public long TimePeriodId { get; set; }
        public DateTime AltKey { get; set; }
        public short Year { get; set; }
        public short Month { get; set; }
        public string Day { get; set; }
        public short DayNumber { get; set; }
        public short Hour { get; set; }

        public TimePeriod()
        {
            this.Hour = 0;
            this.DayNumber = 0;
            this.Hour = 0;
            this.Month = 0;
            this.Day = String.Empty;
        }
    }


    public class Party
    {
        public long PartyId { get; set; }  
        public string Name { get; set; }
        public string PartyKind { get; set; }

        public Party()
        {
            this.Name = String.Empty;
            this.PartyKind = String.Empty;
        }
    }


}
