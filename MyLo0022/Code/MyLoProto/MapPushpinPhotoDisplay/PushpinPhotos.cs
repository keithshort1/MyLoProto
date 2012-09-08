using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using DisplayOnePhoto;

namespace MapPushpinPhotoDisplay
{
    public partial class PushpinPhotos : Form
    {

        public DataTable PassedTable
        {
            get { return _passedTable; }
            set { _passedTable = value; }
        }

        private DataTable _passedTable;

        public PushpinPhotos()
        {
            InitializeComponent();
            
        }

        public void buildPhotoLayout()
        {
            if (_passedTable.Rows.Count != 0)
            {
                while (pushpinPhotosLayout.Controls.Count > 0) { pushpinPhotosLayout.Controls.Clear(); }

                foreach (DataRow dr in _passedTable.Rows)
                {
                    PictureBox imageControl = new PictureBox();
                    imageControl.Height = 100;
                    imageControl.Width = 100;

                    byte[] byteBLOBData = new byte[1];
                    byteBLOBData = (byte[])(dr["thumbnail"]);
                    System.IO.MemoryStream stmBLOBData = new System.IO.MemoryStream(byteBLOBData);
                    stmBLOBData.Position = 0;
                    Image im = Image.FromStream(stmBLOBData);

                    imageControl.Image = im;
                    imageControl.Enabled = true;
                    imageControl.MouseClick += new MouseEventHandler(Image_MouseClick);
                    imageControl.Tag = (string)dr["uri"];
                    pushpinPhotosLayout.Controls.Add(imageControl);
                }
                //Cursor.Current = Cursors.Default;
            }
        }

        private void pushpinPhotosLayout_Paint(object sender, PaintEventArgs e)
        {
            
        }

        public void Image_MouseClick(object sender, EventArgs e)
        {
            PictureBox tempImage = (PictureBox)sender;
            string uri = tempImage.Tag.ToString();

            OnePhoto op = new OnePhoto();
            op.Uri = uri;
            op.ShowDialog();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
