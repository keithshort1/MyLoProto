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
using MyLoIndexerNS;
using System.Diagnostics;
using PhotoLoaderNS;
using MyLoExceptions;


namespace MyLoIndexerApp
{
    public partial class MyLoIndexerAppForm : Form
    {
        private long _userId;
        private PhotoLoader _pl;

        public MyLoIndexerAppForm()
        {
            InitializeComponent();
            button1.Enabled = false;
            progressBar1.Visible = false;
            _userId = 0;
            _pl = new PhotoLoader();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (_userId != 0)
            {
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
                        int count = _pl.StartLoading(folderName);
                        TimeSpan ts = stopWatch.Elapsed;
                        string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
                            ts.Hours, ts.Minutes, ts.Seconds,
                            ts.Milliseconds / 10);
                        textBox1.Text = String.Format("Finished Loading {0}; Time: {1}; Number: {2}", folderName, elapsedTime, count);
                        Cursor.Current = Cursors.Default;
                    }
                    catch (Exception ex)
                    {
                        textBox1.Text = String.Format(ex.Message);
                    }
                }
            }
            else
            {
                textBox1.Text = String.Format("Please Enter a Valid MyLo Account Name");
            }
        }

        private void MyLoAccountName_TextChanged(object sender, EventArgs e)
        {
            button1.Enabled = true;
            
        }

        private void progressBar1_Click(object sender, EventArgs e)
        {

        }

        private void IndexerBuilder_Click(object sender, EventArgs e)
        {
            MyLoIndexer mx = new MyLoIndexer();
            _userId = mx.UserLogin(this.MyLoAccountName.Text);
            Stopwatch stopWatch = new Stopwatch();
            Cursor.Current = Cursors.WaitCursor;
            stopWatch.Start();
            if (_userId != 0)
            {
                try
                {
                    int count = mx.StartIndexing();
                    TimeSpan ts = stopWatch.Elapsed;
                    string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
                        ts.Hours, ts.Minutes, ts.Seconds,
                        ts.Milliseconds / 10);
                    textBox1.Text = String.Format("Finished Indexing; Time: {0}; Number: {1}", elapsedTime, count);
                    Cursor.Current = Cursors.Default;
                }
                catch (Exception ex)
                {
                    textBox1.Text = String.Format(ex.Message);
                }
            }
            else
            {
                textBox1.Text = String.Format("Please Enter a Valid MyLo Account Name");
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                _userId = _pl.UserLogin(this.MyLoAccountName.Text);
                textBox1.Text = String.Format("{0} Signed In to Mylo", this.textBox1.Text);
            }
            catch (MyLoAccountIdException ex)
            {
                textBox1.Text = String.Format("{0} is not a valid MyLo account - please try again: {1}", this.textBox1.Text, ex.Message);
            }
        }
    }
}
