/*

Copyright Keith Short 2012
keith_short@hotmail.com

This file is part of the MyLo application

------------------------------------------------------------------------
*/

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing.Imaging;
using System.Diagnostics;
using MyLoPhotoBrowserNS;
using MyLoExceptions;
using System.IO;
using PhotoLoaderNS;
using MyLoIndexerNS;
using MyLoFacebookContextApp;
using MyLoCalendarContextReaderApp;
using MyLoPhotoViewerNS.BingMapsGeocode;
using GPSlookupNS;
using MyLoDBNS;



namespace MyLoPhotoViewerNS
{
    public partial class MyLoPhotoViewer : Form
    {
        private long _userId;
        private MyLoPhotoBrowser _photoBrowser;
        private Guid _photoId;
        private string _photoUri;
        
        List<string> _timePeriodStrings = new List<string>();
        List<string> _locationStrings = new List<string>();
        Dictionary<string, long> _events = new Dictionary<string, long>();
        List<string> _eventStrings = new List<string>();
        List<string> _partyStrings = new List<string>();
        private string _selectedTimePeriod;
        private string _selectedEvent;
        private string _selectedLocation;
        private string _selectedParty;
        DataTable timePeriodsDT;
        DataTable locationsDT;

        public MyLoPhotoViewer()
        {
            InitializeComponent();
            _userId = 0;
            _photoId = Guid.Empty;
            _photoUri = String.Empty;
            _selectedTimePeriod = String.Empty;
            _selectedEvent = String.Empty;
            _selectedLocation = String.Empty;
            _selectedParty = String.Empty;
            timePeriodsDT = new DataTable();
            locationsDT = new DataTable();
            //ReverseGeocodeRequest reverseGeocodeRequest = new ReverseGeocodeRequest();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            
        }

        private void MessageBox_TextChanged(object sender, EventArgs e)
        {
            
        }


        private void pictureBox1_Click(object sender, EventArgs e)
        {
            
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                _photoBrowser = new MyLoPhotoBrowser();
                _userId = _photoBrowser.UserLogin(this.textBox1.Text);
                MessageBox.Text = String.Format("{0} Signed In to Mylo", this.textBox1.Text);
                PopulateTimePeriodList();
                PopulateLocationList();
                PopulateEventList();
                PopulatePeopleList();
                textBox1.Enabled = false;
                button1.Enabled = false;
                signOutButton.Enabled = true;
            }
            catch (MyLoAccountIdException ex)
            {
                MessageBox.Text = String.Format("{0} is not a valid MyLo account - please try again: {1}", this.textBox1.Text, ex.Message);
            }
        }

        private void PopulateTimePeriodList()
        {
            if (_userId != 0)
            {
                Cursor.Current = Cursors.WaitCursor;
                DataSet results = new DataSet();
                results = _photoBrowser.GetAllTimePeriods();
                _timePeriodStrings = new List<string>();
                timePeriodsDT = results.Tables[0];
                string timePeriodStr = String.Empty;
                _timePeriodStrings.Add(timePeriodStr);
                foreach(DataRow dr in timePeriodsDT.Rows)
                {
                    timePeriodStr = String.Format("{0} {1} {2} {3}", dr["year"], dr["month"], dr["dayname"], dr["daynumber"]);
                    if(!_timePeriodStrings.Contains(timePeriodStr))
                    {
                        _timePeriodStrings.Add(timePeriodStr);
                    }
                }
                timeListBox.DataSource = _timePeriodStrings;
                Cursor.Current = Cursors.Default;
            }
            else
            {
                MessageBox.Text = String.Format("Please Enter a Valid MyLo Account Name");
            }
        }

        private void PopulateLocationList()
        {
            if (_userId != 0)
            {
                Cursor.Current = Cursors.WaitCursor;
                DataSet results = new DataSet();
                results = _photoBrowser.GetAllLocations();
                _locationStrings = new List<string>();
                locationsDT = results.Tables[0];
                string locationStr = String.Empty;
                _locationStrings.Add(locationStr);
                foreach (DataRow dr in locationsDT.Rows)
                {
                    locationStr = String.Format("{0}, {1}", dr["country"], dr["city"]);
                    if (!_locationStrings.Contains(locationStr))
                    {
                        _locationStrings.Add(locationStr);
                    }
                }
                locationListBox.DataSource = _locationStrings;
                Cursor.Current = Cursors.Default;
            }
            else
            {
                MessageBox.Text = String.Format("Please Enter a Valid MyLo Account Name");
            }
        }


        private void PopulateEventList()
        {
            if (_userId != 0)
            {
                Cursor.Current = Cursors.WaitCursor;
                DataSet results = new DataSet();
                _events = new Dictionary<string, long>();
                _eventStrings = new List<string>();
                results = _photoBrowser.GetAllEvents();
                DataTable events = results.Tables[0];
                string eventStr = String.Empty;
                _eventStrings.Add(eventStr);
                foreach (DataRow dr in events.Rows)
                {
                    eventStr = String.Format("{0} {1}                                     {2}", dr["startdatetime"], dr["locationname"], dr["activityid"]);
                    _events.Add(eventStr, (long)dr["activityid"]);
                    _eventStrings.Add(eventStr);
                }
                eventListBox.DataSource = _eventStrings;
                Cursor.Current = Cursors.Default;
            }
            else
            {
                MessageBox.Text = String.Format("Please Enter a Valid MyLo Account Name");
            }
        }


        private void PopulatePeopleList()
        {
            if (_userId != 0)
            {
                Cursor.Current = Cursors.WaitCursor;
                DataSet results = new DataSet();
                _partyStrings = new List<string>();
                results = _photoBrowser.GetAllParties();
                DataTable parties = results.Tables[0];
                string partyStr = String.Empty;
                _partyStrings.Add(partyStr);
                foreach (DataRow dr in parties.Rows)
                {
                    partyStr = String.Format("{0}", dr["name"]);
                    _partyStrings.Add(partyStr);
                }
                peopleListBox.DataSource = _partyStrings;
                Cursor.Current = Cursors.Default;
            }
            else
            {
                MessageBox.Text = String.Format("Please Enter a Valid MyLo Account Name");
            }
        }


        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void queryResultsView_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void query3_Click(object sender, EventArgs e)
        {
            if (_userId != 0)
            {
                Cursor.Current = Cursors.WaitCursor;
                DataSet results  = new DataSet();
                 
                results = _photoBrowser.GetAllPhotos();

                if (results != null)
                {
                    DataTable photos = new DataTable();
                    photos = results.Tables[0];
                    queryResultsView.DataSource = photos;
                    foreach (DataRow dr in photos.Rows)
                    {
                        PictureBox imageControl = new PictureBox();
                        imageControl.Height = 100;
                        imageControl.Width = 100;

                        byte[] byteBLOBData = new byte[1];
                        byteBLOBData = (byte[])(dr["thumbnail"]);
                        MemoryStream stmBLOBData = new MemoryStream(byteBLOBData);
                        stmBLOBData.Position = 0;
                        Image im = Image.FromStream(stmBLOBData);

                        imageControl.Image = im;
                        imageControl.Enabled = true;
                        flowLayoutPanel1.Controls.Add(imageControl);
                    }
                    Cursor.Current = Cursors.Default;
                    MessageBox.Text = String.Format("Found {0} photos.", photos.Rows.Count);
                }
            }
            else
            {
                MessageBox.Text = String.Format("Please Enter a Valid MyLo Account Name");
            }
        }

        private void flowLayoutPanel1_Paint(object sender, PaintEventArgs e)
        {
            
        }

        private void queryButton_Click(object sender, EventArgs e)
        {
            if (_userId != 0)
            {
                // Determine what has been select in the Time, Location and People Lists
                bool timeSelected = _selectedTimePeriod != String.Empty ? true : false;
                bool locationSelected = _selectedLocation != String.Empty ? true : false;
                bool partySelected = _selectedParty != String.Empty ? true : false;

                if (!timeSelected && !locationSelected && !partySelected)
                {
                    MessageBox.Text = String.Format("Select a Time Period, Person or Location to use this query.");
                }
                else
                {
                    Cursor.Current = Cursors.WaitCursor;
                    DataSet results = new DataSet();

                    string country = String.Empty;
                    string city = String.Empty;
                    string[] locParts = _selectedLocation.Split(',');
                    if (locationSelected)
                    {
                        if (locationSelected && locParts.Length == 1)
                        {
                            country = locParts[0];
                        }
                        else
                        {
                            country = locParts[0];
                            city = locParts[1];
                        }
                    }

                    string year = String.Empty;
                    string month = String.Empty;
                    string day = String.Empty;
                    if (timeSelected)
                    {
                        string[] timeParts = _selectedTimePeriod.Split(' ');
                        if (timeParts.Length == 1)
                        {
                            year = timeParts[0];
                        }
                        else if (timeParts.Length == 2)
                        {
                            year = timeParts[0];
                            month = timeParts[1];
                        }
                        else
                        {
                            year = timeParts[0];
                            month = timeParts[1];
                            day = timeParts[2];
                        }
                    }

                    if (timeSelected && !locationSelected && !partySelected && day == String.Empty)
                    {
                        results = _photoBrowser.GetPhotosByTimePeriod(year, month);
                    }
                    else
                    {
                        results = _photoBrowser.GetPhotosByDimensionFields(country, city.Trim(), year, month, day, _selectedParty);
                    }

                    if (results.Tables.Count != 0)
                    {
                        while (flowLayoutPanel1.Controls.Count > 0) { flowLayoutPanel1.Controls.Clear(); }
                        DataTable photos = new DataTable();
                        photos = results.Tables[0];
                        queryResultsView.DataSource = photos;
                        foreach (DataRow dr in photos.Rows)
                        {
                            PictureBox imageControl = new PictureBox();
                            imageControl.Height = 100;
                            imageControl.Width = 100;

                            byte[] byteBLOBData = new byte[1];
                            byteBLOBData = (byte[])(dr["thumbnail"]);
                            MemoryStream stmBLOBData = new MemoryStream(byteBLOBData);
                            stmBLOBData.Position = 0;
                            Image im = Image.FromStream(stmBLOBData);

                            imageControl.Image = im;
                            imageControl.Enabled = true;
                            flowLayoutPanel1.Controls.Add(imageControl);
                        }
                        Cursor.Current = Cursors.Default;
                        MessageBox.Text = String.Format("Found {0} photos.", photos.Rows.Count);
                    }
                }
            }
            else
            {
                MessageBox.Text = String.Format("Please Enter a Valid MyLo Account Name");
            }
        }


        private void timeListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            string selectedTimePeriod = (string)timeListBox.SelectedItem;
            //if (selectedTimePeriod != String.Empty)
            _selectedTimePeriod = selectedTimePeriod;

        }

        private void locationListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            string selectedLocation = (string)locationListBox.SelectedItem;
            //if (selectedLocation != String.Empty)
                _selectedLocation = selectedLocation;
        }

        private void eventListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            string selectedEvent = (string)eventListBox.SelectedItem;
            //if (selectedEvent != String.Empty)
                _selectedEvent = selectedEvent;
        }

        private void peopleListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            string selectedParty = (string)peopleListBox.SelectedItem;
            //if (selectedParty != String.Empty)
                _selectedParty = selectedParty;
        }

        private void queryBySelectedEvent_Click(object sender, EventArgs e)
        {
            if (_userId != 0)
            {
                // Determine what has been selected in the Event List
                long eventId = 0;
                bool eventSelected = _events.TryGetValue(_selectedEvent, out eventId);

                if (eventSelected)
                {
                    Cursor.Current = Cursors.WaitCursor;
                    DataSet results = new DataSet();

                    results = _photoBrowser.GetPhotosByActivity(eventId);

                    if (results.Tables.Count != 0)
                    {
                        while (flowLayoutPanel1.Controls.Count > 0) { flowLayoutPanel1.Controls.Clear(); }
                        DataTable photos = new DataTable();
                        photos = results.Tables[0];
                        queryResultsView.DataSource = photos;
                        foreach (DataRow dr in photos.Rows)
                        {
                            PictureBox imageControl = new PictureBox();
                            imageControl.Height = 100;
                            imageControl.Width = 100;

                            byte[] byteBLOBData = new byte[1];
                            byteBLOBData = (byte[])(dr["thumbnail"]);
                            MemoryStream stmBLOBData = new MemoryStream(byteBLOBData);
                            stmBLOBData.Position = 0;
                            Image im = Image.FromStream(stmBLOBData);

                            imageControl.Image = im;
                            imageControl.Enabled = true;
                            flowLayoutPanel1.Controls.Add(imageControl);
                        }
                        Cursor.Current = Cursors.Default;
                        MessageBox.Text = String.Format("Found {0} photos.", photos.Rows.Count);
                    }
                    else
                    {
                        MessageBox.Text = String.Format("No photos found.");
                    }
                }
                else
                {
                    MessageBox.Text = String.Format("Select an Event for this kind of query.");
                }
            }
            else
            {
                MessageBox.Text = String.Format("Please Enter a Valid MyLo Account Name");
            }
        }

        private void loadPhotosButton_Click(object sender, EventArgs e)
        {
            if (_userId != 0)
            {
                PhotoLoader photoLoader = new PhotoLoader();
                photoLoader.UserLogin(_userId);
                FolderBrowserDialog openFolderDialog1 = new FolderBrowserDialog();
                openFolderDialog1.RootFolder = Environment.SpecialFolder.MyComputer;
                openFolderDialog1.Description =
                    "Select the photo directory that you want to load";

                if (openFolderDialog1.ShowDialog() == DialogResult.OK)
                {
                    string folderName = openFolderDialog1.SelectedPath;
                    Cursor.Current = Cursors.WaitCursor;
                    Stopwatch stopWatch = new Stopwatch();
                    stopWatch.Start();
                    try
                    {
                        int count = photoLoader.StartLoading(folderName);
                        TimeSpan ts = stopWatch.Elapsed;
                        string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
                            ts.Hours, ts.Minutes, ts.Seconds,
                            ts.Milliseconds / 10);
                        textBox3.Text = String.Format("Finished Loading {0}; Time: {1}; Number: {2}", folderName, elapsedTime, count);
                        PopulateTimePeriodList();
                        Cursor.Current = Cursors.Default;
                    }
                    catch (Exception ex)
                    {
                        textBox3.Text = String.Format(ex.Message);
                    }
                }
            }
        }

        private void catalogPhotosButton_Click(object sender, EventArgs e)
        {
            SimpleMyLoIndexer simpleIndexer = new SimpleMyLoIndexer();
            MyLoIndexer indexer = new MyLoIndexer(simpleIndexer);
            indexer.UserLogin(_userId);
            Stopwatch stopWatch = new Stopwatch();
            Cursor.Current = Cursors.WaitCursor;
            stopWatch.Start();
            if (_userId != 0)
            {
                try
                {
                    int count = indexer.StartIndexing();
                    TimeSpan ts = stopWatch.Elapsed;
                    string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
                        ts.Hours, ts.Minutes, ts.Seconds,
                        ts.Milliseconds / 10);
                    textBox3.Text = String.Format("Finished Indexing; Time: {0}; Number: {1}", elapsedTime, count);
                    RefreshAllBoxes();
                    Cursor.Current = Cursors.Default;
                }
                catch (Exception ex)
                {
                    textBox3.Text = String.Format(ex.Message);
                }
            }
            else
            {
                textBox3.Text = String.Format("Please Enter a Valid MyLo Account Name");
            }
        }

        private void getFacebookContextButton_Click(object sender, EventArgs e)
        {
            MyLoForm1 fbContext = new MyLoForm1();
            try
            {
                fbContext.ShowDialog();
                RefreshAllBoxes();
            }
            catch (Exception ex)
            {
                MessageBox.Text = ex.Message;
            }
        }

        private void signOutButton_Click(object sender, EventArgs e)
        {
            _userId = 0;
            MessageBox.Text = String.Format("{0} Signed In to Mylo", this.textBox1.Text);
            while (flowLayoutPanel1.Controls.Count > 0) { flowLayoutPanel1.Controls.Clear(); }
            RefreshAllBoxes();
            textBox1.Enabled = true;
            textBox1.Text = String.Empty;
            button1.Enabled = true;
            signOutButton.Enabled = false;
        }

        private void getCalendarContextButton_Click(object sender, EventArgs e)
        {
            string[] parms = new string[1];
            parms[0] = _userId.ToString();
            MyLoCalendarReaderForm calContext = new MyLoCalendarReaderForm(parms);
            calContext.ShowDialog();
            RefreshAllBoxes();
            textBox3.Text = "Finished getting calendar context";
        }

        private void RefreshAllBoxes()
        {
            eventListBox.DataSource = null;
            timeListBox.DataSource = null;
            peopleListBox.DataSource = null;
            locationListBox.DataSource = null;
            queryResultsView.DataSource = null;
            timeListBox.BeginUpdate();
            PopulateTimePeriodList();
            timeListBox.EndUpdate();
            locationListBox.BeginUpdate();
            PopulateLocationList();
            locationListBox.EndUpdate();
            eventListBox.BeginUpdate();
            PopulateEventList();
            eventListBox.EndUpdate();
            peopleListBox.BeginUpdate();
            PopulatePeopleList();
            peopleListBox.EndUpdate();
        }

        private void label6_Click(object sender, EventArgs e)
        {

        }

        private void buildUsingIntervalGPSfit_Click(object sender, EventArgs e)
        {
            SimpleIntervalGpsFitIndexer simpleGpsFitIndexer = new SimpleIntervalGpsFitIndexer();
            MyLoIndexer indexer = new MyLoIndexer(simpleGpsFitIndexer);
            indexer.UserLogin(_userId);
            Stopwatch stopWatch = new Stopwatch();
            Cursor.Current = Cursors.WaitCursor;
            stopWatch.Start();
            if (_userId != 0)
            {
                try
                {
                    int count = indexer.StartIndexing();
                    TimeSpan ts = stopWatch.Elapsed;
                    string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
                        ts.Hours, ts.Minutes, ts.Seconds,
                        ts.Milliseconds / 10);
                    textBox3.Text = String.Format("Finished Indexing; Time: {0}; Number: {1}", elapsedTime, count);
                    RefreshAllBoxes();
                    Cursor.Current = Cursors.Default;
                }
                catch (Exception ex)
                {
                    textBox3.Text = String.Format(ex.Message);
                }
            }
            else
            {
                textBox3.Text = String.Format("Please Enter a Valid MyLo Account Name");
            }
        }

    }
}
