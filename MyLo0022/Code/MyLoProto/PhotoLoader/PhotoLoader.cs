/*

Copyright Keith Short 2012
keith_short@hotmail.com

This file is part of the MyLo application
Uses FotoFly v0.5 under Microsoft Public License (Ms-PL)
Uses Kalico ImageLibrary under MIT License
 * Copyright (c) 2009 Fredrik Schultz

------------------------------------------------------------------------
*/

using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.IO;
using FotoFly;
using MyLoDBNS;
using MyLoExceptions;
using Kaliko.ImageLibrary;
using System.Diagnostics;
using System.Drawing;
using System.Xml.Linq;

namespace PhotoLoaderNS
{
    public class PhotoLoader
    {

        private string _topFolder;
        private int _count;
        static System.Collections.Specialized.StringCollection log = new System.Collections.Specialized.StringCollection();
        long _userId;
        private MyLoDB _myLoStore;
        private string _timeIndexKind;


        /// <summary>
        /// Creates a new Loader for the given top level folder
        /// </summary>
        /// <param name="folderUri">Top Level Folder</param>
        public PhotoLoader()
        {
            var xmlElm = XElement.Load(@"../../../configuration.xml");
            XElement element = xmlElm.Element("photoIndex");
            _timeIndexKind = element.Value;
            _count = 0;
            _userId = 0;
            _myLoStore = new MyLoDB();
        }


        public long  UserLogin(string userName)
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

        public void UserLogin(long userId)
        {
            _userId = userId;
        }

        /// <summary>
        /// Start a recursive breadth first traversal of the folders from the given top level folder
        /// </summary>
        /// <param name="folderUri">Current Top Level Folder Name</param>
        public int StartLoading(string folderUri)
        {
            if (_userId != 0)
            {
                _topFolder = folderUri;
                System.IO.DirectoryInfo rootDir = new DirectoryInfo(folderUri);
                MyLoLoadFolder(rootDir);
                return _count;
            }
            else
            {
                throw new MyLoAccountIdException("MyLo User not signed in");
            }
        }




        /// <summary>
        /// Load files in one Folder, then recurse on all contained folders
        /// </summary>
        /// <param name="root">Current Top Level Folder Name</param>
        private void MyLoLoadFolder(DirectoryInfo root)
        {
            System.IO.FileInfo[] files = null;
            System.IO.DirectoryInfo[] subDirs = null;
            // First, process all the files directly under this folder
            try
            {
                files = root.GetFiles("*.*");
            }
            // This is thrown if even one of the files requires permissions greater
            // than the application provides.
            catch (UnauthorizedAccessException e)
            {
                // This code just writes out the message and continues to recurse.
                // You may decide to do something different here. For example, you
                // can try to elevate your privileges and access the file again.
                log.Add(e.Message);
            }

            catch (System.IO.DirectoryNotFoundException e)
            {
                Console.WriteLine(e.Message);
            }

            if (files != null)
            {
                foreach (System.IO.FileInfo fi in files)
                {
                    // a try-catch block is required here to handle the case
                    // where the file has been deleted since the call to MyLoLoadFolder().
                    try
                    {
                        if (fi.Extension == ".jpg" || fi.Extension == ".JPG")
                        {
                            Photo photo = new Photo();
                            Image.GetThumbnailImageAbort myCallback =
                            new Image.GetThumbnailImageAbort(ThumbnailCallback);
                            Bitmap image = new Bitmap(fi.FullName);
                            Image thumb = image.GetThumbnailImage(100, 100, myCallback, IntPtr.Zero);

                            ImageConverter converter = new ImageConverter();
                            photo.Thumbnail = (byte[])converter.ConvertTo(thumb, typeof(byte[]));

                            // This is for testing only - should switch to more reliable Hash such as SHA-256 or MD5
                            //long CRC = CalculateCRCForFile(fi.FullName);
                            photo.CRC = image.GetHashCode();
                            image.Dispose();

                            JpgPhoto photoJpg = new JpgPhoto(fi.FullName);

                            photo.Uri = fi.FullName;
                            photo.Aperture = photoJpg.Metadata.Aperture;
                            photo.Camera = photoJpg.Metadata.CameraModel;
                            photo.DateTaken = photoJpg.Metadata.DateTaken;
                            photo.PhotoIndexKind = _timeIndexKind;
                            DateTime dat = photoJpg.Metadata.DateTaken.Date;
                            TimeSpan tim = photoJpg.Metadata.DateTaken.TimeOfDay;
                            if (photoJpg.Metadata.GpsPosition.Latitude.IsValidCoordinate)
                            {
                                photo.GpsLat = photoJpg.Metadata.GpsPosition.Latitude.Numeric;
                            }
                            if (photoJpg.Metadata.GpsPosition.Longitude.IsValidCoordinate)
                            {
                                photo.GpsLong = photoJpg.Metadata.GpsPosition.Longitude.Numeric;
                            }
                            photo.Uuid = Guid.NewGuid();

                            //foreach (Tag tag in photo.Metadata.Tags)
                            //{
                            //    Console.WriteLine(tag.FullName);
                            //} 
                            long photoId = _myLoStore.AddPhoto(_userId, photo);

                            _count++;

                            // TODO add code to write back GUID and Hash into photo metadata
                        }
                    }
                    catch (MyLoCRCException crcEx)
                    {
                        throw new MyLoException("CRC Calculation Error: Inner exception: " + crcEx);
                    }
                    catch (MyLoDataStoreException dsEx)
                    {
                        throw new MyLoException("DataStore Error: Inner exception: " + dsEx);
                    }
                    catch (Exception ex)
                    {
                        throw new MyLoException("File not Found: " + fi.FullName + " Inner exception: " + ex);
                    }
                    Debug.WriteLine(fi.FullName);
                }

                // Now find all the subdirectories under this directory.
                subDirs = root.GetDirectories();

                foreach (System.IO.DirectoryInfo dirInfo in subDirs)
                {
                    // Resursive call for each subdirectory.
                    MyLoLoadFolder(dirInfo);
                }
            }

        }

        private bool ThumbnailCallback()
        {
            return false;
        }

        /// <summary>
        /// Hack to simulate calculating a CRC for a char array
        /// Modified from http://msdn.itags.org/visual-csharp/54159/
        /// </summary>
        /// <param name="val">a charachter array containing source for CRC</param
        private long ComputeCRC(Char[] val)
        {
            long crc;
            long q;
            char c;

            crc = 0;
            for (int i = 0; i < val.Length; i++)
            {
                c = val[i];
                q = (crc ^ c) & 0x0f;
                crc = (crc >> 4) ^ (q * 0x1081);
                q = (crc ^ (c >> 4)) & 0xf;
                crc = (crc >> 4) ^ (q * 0x1081);
            }
            return crc;
        }



        /// <summary>
        /// Calculates a CRC hashcode from the first 128 bytes of a photo file
        /// </summary>
        /// <param name="root">file name for photo</param
        private long CalculateCRCForFile(string fileName)
        {
            long CRC;
            try
            {
                using (StreamReader sr = new StreamReader(fileName))
                {
                    Char[] filePart = new Char[1024];
                    sr.Read(filePart, 1, 1023);
                    CRC = ComputeCRC(filePart);
                    return CRC;
                }
            }
            catch (Exception ex)
            {
                throw new MyLoCRCException(ex.Message);
            }
        }

    }
}
