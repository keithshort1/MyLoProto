using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using GMap.NET.MapProviders;
using GMap.NET;
using GMap.NET.WindowsForms;

namespace MyLoMap
{
    public partial class MyLoMap : Form
    {
        private DataTable _passedTable;

        public DataTable PassedTable 
        {
            get { return _passedTable;  }
            set { _passedTable = value;  }
        }

        public MyLoMap()
        {
            InitializeComponent();
            gMapControl1.SetCurrentPositionByKeywords("USA");
            gMapControl1.MapProvider = GMapProviders.BingMap;
            gMapControl1.MinZoom = 3;
            gMapControl1.MaxZoom = 17;
            gMapControl1.Zoom = 4;

            

            

            //gMapControl1.Position = new PointLatLng(48.53532438626, -123.01548379047);
            //gMapControl1.Manager.Mode = GMapProviders.AccessMode.ServerAndCache;
        }

        public void DisplayMap()
        {
            GMapOverlay overlay1 = new GMapOverlay(gMapControl1, "PhotosByLocation");
            if (_passedTable != null)
            {
                foreach (DataRow dr in _passedTable.Rows)
                {
                    overlay1.Markers.Add(new GMap.NET.WindowsForms.Markers.GMapMarkerGoogleGreen(new PointLatLng(Convert.ToDouble(dr["latitude"]), Convert.ToDouble(dr["longitude"]))));
                }
            }
            gMapControl1.Overlays.Add(overlay1);
        }
    }
}
