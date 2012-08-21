/*

Copyright Keith Short 2012
keith_short@hotmail.com

This file is part of the MyLo application
Uses FotoFly v0.5 under Microsoft Public License (Ms-PL)

------------------------------------------------------------------------
*/

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.IO;
using System.Drawing.Imaging;
using System.Diagnostics;
using MyLoDBNS;
using MyLoExceptions;

namespace MyLoPhotoBrowserNS
{
    public class MyLoPhotoBrowser
    {
        long _userId;
        private MyLoDB _myLoStore;

        /// <summary>
        /// Creates a new Loader for the given top level folder
        /// </summary>
        public MyLoPhotoBrowser()
        {
            _userId = 0;
            _myLoStore = new MyLoDB();
        }


        /// <summary>
        /// Verifies MyLo Account Holder Name and returns internal user Id
        /// </summary>
        /// <param name="userName">MyLo Account Holder Name</param>
        public long UserLogin(string userName)
        {
            try
            {
                _userId = _myLoStore.GetUserAccount(userName);
                return _userId;
            }
            catch (MyLoAccountIdException ex)
            {
                throw new MyLoAccountIdException(ex.Message);
            }
        }


        /// <summary>
        /// Method for returning a all Photos using selection by Dimension(Time Party, Location) in a given context
        /// </summary>
        /// <param name="timePeriodId">An identifier for a Time Period</param>
        /// <param name="eventId">An identifier for a Party</param>
        /// <param name="locationId">An identifier for a Location</param>
        public DataSet GetPhotosByDimensionIds(long timePeriodId, long partyId, long locationId)
        {     
            try
            {
                DataSet ds = new DataSet();
                if (timePeriodId != 0 || partyId != 0 || locationId != 0)
                {
                    ds = _myLoStore.GetPhotosByDimensionIds(timePeriodId, partyId, locationId);
                }
                return ds;
            }
            catch (MyLoException ex)
            {
                throw new Exception(ex.Message);
            }
        }


        /// <summary>
        /// Method for returning a all Photos using slection TimePeriod directly related to Photos on loading in a given context
        /// </summary>
        /// <param name="year">An identifier for a Time Period</param>
        /// <param name="month">An identifier for a Party</param>
        public DataSet GetPhotosByTimePeriod(string year, string month)
        {
            try
            {
                DataSet ds = new DataSet();
                TimePeriod tp = new TimePeriod();
                if (year == String.Empty && month == String.Empty)
                {
                    return ds;
                }
                else
                {
                    if (year != String.Empty)
                    {
                        tp.Year = Convert.ToInt16(year);
                    }
                    else
                    {
                        tp.Year = 0;
                    }
                    if (month != String.Empty)
                    {
                        tp.Month = Convert.ToInt16(month);
                    }
                    else
                    {
                        tp.Month = 0;
                    }
                }
                return ds = _myLoStore.GetPhotosByTimePeriod(tp);
            }
            catch (MyLoException ex)
            {
                throw new Exception(ex.Message);
            }
        }



        /// <summary>
        /// Method for returning a all Photos using slection by Dimension(Time Party, Location) in a given context
        /// </summary>
        /// <param name="country">A country name</param>
        /// <param name="city">A city name</param>
        /// <param name="year">A year</param>
        /// <param name="month">A month</param>
        /// <param name="day">A day name</param>
        /// <param name="name">A unique name for a person</param>
        public DataSet GetPhotosByDimensionFields(string country, string city, string year, string month, string day, string name)
        {
            try
            {
                DataSet ds = new DataSet();
                if (country == String.Empty && city == String.Empty 
                    && year == String.Empty && month == String.Empty 
                    && day == String.Empty && name == String.Empty)
                {
                    return ds;
                }
                else
                {
                    Location loc = new Location();
                    loc.Country = country; loc.City = city; 
                    TimePeriod tp = new TimePeriod();
                    if (year != String.Empty)
                    {
                        tp.Year = Convert.ToInt16(year);
                    }
                    else
                    {
                        tp.Year = 0;
                    }
                    if (month != String.Empty)
                    {
                        tp.Month = Convert.ToInt16(month);
                    }
                    else
                    {
                        tp.Month = 0;
                    }
                    tp.Day = day;
                    Party p = new Party();
                    p.Name = name;
                    return ds = _myLoStore.GetPhotosByDimensionFields(_userId, loc, tp, p);
                }
            }
            catch (MyLoException ex)
            {
                throw new Exception(ex.Message);
            }
        }


        /// <summary>
        /// Method for returning a all Photos for a given event in a given context
        /// </summary>
        public DataSet GetPhotosByActivity(long eventId)
        {
            try
            {
                return _myLoStore.GetPhotosByActivity(_userId, eventId);
            }
            catch (MyLoException ex)
            {
                throw new Exception(ex.Message);
            }
        }


        // <summary>
        /// Method for returning a all Photos in a given context
        /// </summary>
        public DataSet GetAllPhotos()
        {
            try
            {
                return _myLoStore.GetAllPhotos(_userId);
            }
            catch (MyLoException ex)
            {
                throw new Exception(ex.Message);
            }
        }



        /// <summary>
        /// Method for returning all Time Periods in a given context
        /// </summary>
        public DataSet GetAllTimePeriods()
        {
            try
            {
                return _myLoStore.GetAllTimePeriods(_userId);
            }
            catch (MyLoException ex)
            {
                throw new Exception(ex.Message);
            }
        }


        /// <summary>
        /// Method for returning all Parties in a given context
        /// </summary>
        public DataSet GetAllParties()
        {
            try
            {
                return _myLoStore.GetAllParties(_userId);
            }
            catch (MyLoException ex)
            {
                throw new Exception(ex.Message);
            }
        }


        /// <summary>
        /// Method for returning all Locations in a given context
        /// </summary>
        public DataSet GetAllLocations()
        {
            try
            {
                return _myLoStore.GetAllLocations(_userId);
            }
            catch (MyLoException ex)
            {
                throw new Exception(ex.Message);
            }
        }


        /// <summary>
        /// Method for returning all Events in a given context
        /// </summary>
        public DataSet GetAllEvents()
        {
            try
            {
                return _myLoStore.GetAllActivities(_userId);
            }
            catch (MyLoException ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// Test Method for returning a thumbnail given a MyLo Guid
        /// </summary>
        /// <param name="photoId">Internal MyLo Photo Guid</param>
        public Image TestQuery01(Guid photoId)
        {
            DataTable thumbs =  _myLoStore.GetThumbnailForPhoto(_userId, photoId);

            if (thumbs.Rows.Count > 0)
            {
                byte[] byteBLOBData = new byte[1];
                byteBLOBData = (byte[])(thumbs.Rows[0]["thumbnail"]);

                MemoryStream stmBLOBData = new MemoryStream(byteBLOBData);
                stmBLOBData.Position = 0;
                Image im = Image.FromStream(stmBLOBData);
                return im;
            }
            return null;
        }

    }
}
