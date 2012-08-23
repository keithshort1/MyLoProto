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
using MyLoDBNS;
using System.Diagnostics;
using MyLoExceptions;
using GPSlookupNS;

namespace MyLoCalendarContextReaderNS
{
    /// <summary>
    /// A MyLoCalendarContextReader encapsulates knowledge of queries into a Calendar system, and the
    /// representation of calendar ebvents in memory for loading into a MyLo store
    /// </summary>
    public class MyLoCalendarContextReader
    {


        private MyLoDB _myloStore;
        private long _userId;

        public MyLoCalendarContextReader()
        {
        }


        public void InitializeContext()
        {
        }

        public void AddCalendarEventToContext()
        {
        }

        /// <summary>
        /// Saves in memory representation of Calendar context items to MyLo database
        /// </summary>
        public void SaveContextToDB(long userId)
        {
            _myloStore = new MyLoDB();
            _userId = userId;
            if (_userId != 0)
            {
                try
                {
                    AddEvents();
                }
                catch (Exception ex)
                {
                    throw new MyLoException("Save Context Error - Calendar Posts: Inner exception: " + ex);
                }
            }
            else
            {
                throw new MyLoException(String.Format("Invalid User Account {0}", _userId.ToString()));
            }

        }

        private void AddEvents()
        {
            Address loc = new Address();
            TimePeriod start = new TimePeriod();
            TimePeriod end = new TimePeriod();
            Party person = new Party();
            Activity activity = new Activity();
            BingMapsGPSlookup gpsl = new BingMapsGPSlookup();

            // Setup the event called "Jeff and Tami's Wedding
            string city, street, zip, country, state;
            gpsl.LatLongToAddressLookup(47.7337578, -122.1469737, out street, out city, out state, out zip, out country);
            loc.Street = street;
            loc.City = city;
            loc.State = state;
            loc.Zip = zip;
            loc.Country = country;

            start.Year = 2012;
            start.Month = 6;
            start.DayNumber = 2;
            start.Day = "Saturday";
            start.Hour = 10;
            start.AltKey = new DateTime(2012, 6, 2, 10, 0, 0, DateTimeKind.Local);

            end.Year = 2012;
            end.Month = 6;
            end.DayNumber = 2;
            end.Day = "Saturday";
            end.Hour = 16;
            end.AltKey = new DateTime(2012, 6, 2, 16, 0, 0, DateTimeKind.Local);

            activity.ActivityKind = "Calendar";
            activity.Source = "Outlook";
            activity.SourceId = "0001";
            activity.StartDate = new DateTime(2012, 6, 2, 10, 0, 0, DateTimeKind.Local);
            activity.EndDate = new DateTime(2012, 6, 2, 16, 0, 0, DateTimeKind.Local);
            activity.ActivityName = "Jeff and Tami's Wedding";
            activity.Latitude = 47.7337578;
            activity.Longitude = -122.1469737;

            activity.ActivityId = _myloStore.AddActivity(_userId, activity, start, end,loc);

            person.Name = "Rebecca Short";
            person.PartyId = _myloStore.AddParty(_userId, person);
            person.PartyKind = "Person";
            long paId = _myloStore.AddPartyToActivityByIds(_userId, person, activity);

            // Setup the event called "Cabo San Lucas Vacation"
            gpsl.LatLongToAddressLookup(22.890980, -109.916676, out street, out city, out state, out zip, out country);
            loc.Street = street;
            loc.City = city;
            loc.State = state;
            loc.Zip = zip;
            loc.Country = country;

            start.Year = 2012;
            start.Month = 2;
            start.DayNumber = 20;
            start.Day = "Monday";
            start.Hour = 10;
            start.AltKey = new DateTime(2012, 2, 20, 10, 0, 0, DateTimeKind.Local);

            end.Year = 2012;
            end.Month = 2;
            end.DayNumber = 26;
            end.Day = "Sunday";
            end.Hour = 18;
            end.AltKey = new DateTime(2012, 2, 26, 18, 0, 0, DateTimeKind.Local);

            activity.ActivityKind = "Calendar";
            activity.Source = "Outlook";
            activity.SourceId = "0002";
            activity.StartDate = new DateTime(2012, 2, 20, 10, 0, 0, DateTimeKind.Local);
            activity.EndDate = new DateTime(2012, 2, 26, 18, 0, 0, DateTimeKind.Local);
            activity.ActivityName = "Cabo San Lucas Vacation";
            activity.Latitude = 22.890980;
            activity.Longitude = -109.916676;

            activity.ActivityId = _myloStore.AddActivity(_userId, activity, start, end, loc);

            person.Name = "Rebecca Short";
            person.PartyKind = "Person";
            person.PartyId = _myloStore.AddParty(_userId, person);
            long paId2 = _myloStore.AddPartyToActivityByIds(_userId, person, activity);


            // Setup the event called "Wine Club and Steven's 50th"
            gpsl.LatLongToAddressLookup(47.6885173, -122.1321265, out street, out city, out state, out zip, out country);
            loc.Street = street;
            loc.City = city;
            loc.State = state;
            loc.Zip = zip;
            loc.Country = country;

            start.Year = 2012;
            start.Month = 6;
            start.DayNumber = 8;
            start.Day = "Friday";
            start.Hour = 19;
            start.AltKey = new DateTime(2012, 6, 8, 19, 0, 0, DateTimeKind.Local);

            end.Year = 2012;
            end.Month = 6;
            end.DayNumber = 8;
            end.Day = "Friday";
            end.Hour = 23;
            end.AltKey = new DateTime(2012, 6, 8, 23, 0, 0, DateTimeKind.Local);

            activity.ActivityKind = "Calendar";
            activity.Source = "Outlook";
            activity.SourceId = "0003";
            activity.StartDate = new DateTime(2012, 6, 8, 19, 0, 0, DateTimeKind.Local);
            activity.EndDate = new DateTime(2012, 6, 8, 23, 0, 0, DateTimeKind.Local);
            activity.ActivityName = "Wine Club and Steven's 50th";
            activity.Latitude = 47.6885173;
            activity.Longitude = -122.1321265;

            activity.ActivityId = _myloStore.AddActivity(_userId, activity, start, end, loc);

            person.Name = "Rebecca Short";
            person.PartyKind = "Person";
            person.PartyId = _myloStore.AddParty(_userId, person);
            long paId3 = _myloStore.AddPartyToActivityByIds(_userId, person, activity);


            // Setup the event called "Maui Vacation"
            gpsl.LatLongToAddressLookup(20.7180257, -156.4472008, out street, out city, out state, out zip, out country);
            loc.Street = street;
            loc.City = city;
            loc.State = state;
            loc.Zip = zip;
            loc.Country = country;

            start.Year = 2011;
            start.Month = 12;
            start.DayNumber = 5;
            start.Day = "Monday";
            start.Hour = 11;
            start.AltKey = new DateTime(2011, 12, 5, 11, 0, 0, DateTimeKind.Local);

            end.Year = 2011;
            end.Month = 12;
            end.DayNumber = 10;
            end.Day = "Saturday";
            end.Hour = 22;
            end.AltKey = new DateTime(2011, 12, 10, 22, 0, 0, DateTimeKind.Local);

            activity.ActivityKind = "Calendar";
            activity.Source = "Outlook";
            activity.SourceId = "0004";
            activity.StartDate = new DateTime(2011, 12, 5, 11, 0, 0, DateTimeKind.Local);
            activity.EndDate = new DateTime(2011, 12, 10, 22, 0, 0, DateTimeKind.Local);
            activity.ActivityName = "Vacation in Maui";
            activity.Latitude = 20.7180257;
            activity.Longitude = -156.4472008;

            activity.ActivityId = _myloStore.AddActivity(_userId, activity, start, end, loc);

            person.Name = "Rebecca Short";
            person.PartyKind = "Person";
            person.PartyId = _myloStore.AddParty(_userId, person);
            long paId4 = _myloStore.AddPartyToActivityByIds(_userId, person, activity);


            // Setup the event called "Mount Haleakela"
            gpsl.LatLongToAddressLookup(20.709722, -156.253333, out street, out city, out state, out zip, out country);
            loc.Street = street;
            loc.City = city;
            loc.State = state;
            loc.Zip = zip;
            loc.Country = country;

            start.Year = 2011;
            start.Month = 12;
            start.DayNumber = 7;
            start.Day = "Wednesday";
            start.Hour = 5;
            start.AltKey = new DateTime(2011, 12, 7, 5, 0, 0, DateTimeKind.Local);

            end.Year = 2011;
            end.Month = 12;
            end.DayNumber = 7;
            end.Day = "Wednesday";
            end.Hour = 14;
            end.AltKey = new DateTime(2011, 12, 7, 14, 0, 0, DateTimeKind.Local);

            activity.ActivityKind = "Calendar";
            activity.Source = "Outlook";
            activity.SourceId = "0005";
            activity.StartDate = new DateTime(2011, 12, 7, 5, 0, 0, DateTimeKind.Local);
            activity.EndDate = new DateTime(2011, 12, 7, 14, 0, 0, DateTimeKind.Local);
            activity.ActivityName = "Mount Haleakala";
            activity.Latitude = 20.709722;
            activity.Longitude = -156.253333;

            activity.ActivityId = _myloStore.AddActivity(_userId, activity, start, end, loc);

            person.Name = "Rebecca Short";
            person.PartyKind = "Person";
            person.PartyId = _myloStore.AddParty(_userId, person);
            long paId5 = _myloStore.AddPartyToActivityByIds(_userId, person, activity);


            // Setup the event called "Florence Vacation"
            gpsl.LatLongToAddressLookup(43.777417, 11.251117, out street, out city, out state, out zip, out country);
            loc.Street = street;
            loc.City = city;
            loc.State = state;
            loc.Zip = zip;
            loc.Country = country;

            start.Year = 2012;
            start.Month = 04;
            start.DayNumber = 21;
            start.Day = "Saturday";
            start.Hour = 12;
            start.AltKey = new DateTime(2012, 4, 21, 12, 0, 0, DateTimeKind.Local);

            end.Year = 2012;
            end.Month = 4;
            end.DayNumber = 25;
            end.Day = "Wednesday";
            end.Hour = 14;
            end.AltKey = new DateTime(2012, 4, 25, 14, 0, 0, DateTimeKind.Local);

            activity.ActivityKind = "Calendar";
            activity.Source = "Outlook";
            activity.SourceId = "0006";
            activity.StartDate = new DateTime(2012, 4, 21, 12, 0, 0, DateTimeKind.Local);
            activity.EndDate = new DateTime(2012, 4, 25, 14, 0, 0, DateTimeKind.Local);
            activity.ActivityName = "Florence Vacation";
            activity.Latitude = 43.777417;
            activity.Longitude = 11.251117;

            activity.ActivityId = _myloStore.AddActivity(_userId, activity, start, end, loc);

            person.Name = "Rebecca Short";
            person.PartyKind = "Person";
            person.PartyId = _myloStore.AddParty(_userId, person);
            long paId6 = _myloStore.AddPartyToActivityByIds(_userId, person, activity);

            // Setup the event called "Italy Vacation"
            //gpsl.LatLongToAddressLookup(43.777417, 11.251117, out street, out city, out state, out zip, out country);
            loc.Country = "Italy";

            start.Year = 2012;
            start.Month = 04;
            start.DayNumber = 21;
            start.Day = "Saturday";
            start.Hour = 12;
            start.AltKey = new DateTime(2012, 4, 21, 12, 0, 0, DateTimeKind.Local);

            end.Year = 2012;
            end.Month = 4;
            end.DayNumber = 28;
            end.Day = "Saturday";
            end.Hour = 18;
            end.AltKey = new DateTime(2012, 4, 28, 18, 0, 0, DateTimeKind.Local);

            activity.ActivityKind = "Calendar";
            activity.Source = "Outlook";
            activity.SourceId = "0007";
            activity.StartDate = new DateTime(2012, 4, 21, 12, 0, 0, DateTimeKind.Local);
            activity.EndDate = new DateTime(2012, 4, 28, 18, 0, 0, DateTimeKind.Local);
            activity.ActivityName = "Italy Vacation";
            activity.Latitude = 0;
            activity.Longitude = 0;

            activity.ActivityId = _myloStore.AddActivity(_userId, activity, start, end, loc);

            person.Name = "Rebecca Short";
            person.PartyKind = "Person";
            person.PartyId = _myloStore.AddParty(_userId, person);
            long paId7 = _myloStore.AddPartyToActivityByIds(_userId, person, activity);
        }
    }
}
