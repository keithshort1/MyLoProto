using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace DisplayOnePhoto
{
    public partial class OnePhoto : Form
    {
        private string _uri;

        public string Uri
        {
            get { return _uri; }
            set { _uri = value; }
        }

        public OnePhoto()
        {
            InitializeComponent();
        }

        private void OnePhoto_Load(object sender, System.EventArgs e)
        {
              pictureBox1.Image = Image.FromFile(_uri);
              uriText.Text = _uri;
              // Center image
              pictureBox1.Left = (this.Width - pictureBox1.Width)/2;
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
