using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using MyLoExceptions;
using MyLoCalendarContextReaderNS;


namespace MyLoCalendarContextReaderApp
{
    public partial class MyLoCalendarReaderForm : Form
    {

        private long _userId;


        public MyLoCalendarReaderForm(string[] args)
        {
            InitializeComponent();
            _userId = Convert.ToInt64(args[0]);
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void saveContextButton_Click(object sender, EventArgs e)
        {
            MyLoCalendarContextReader cal = new MyLoCalendarContextReader();
            try
            {
                cal.SaveContextToDB(_userId);
                textBox1.Text = "Context saved to data store";
            }
            catch (Exception ex)
            {
                textBox1.Text = String.Format(ex.Message);
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void getEventsButton_Click(object sender, EventArgs e)
        {
            textBox1.Text = "Loaded  8 events";
        }
    }
}
