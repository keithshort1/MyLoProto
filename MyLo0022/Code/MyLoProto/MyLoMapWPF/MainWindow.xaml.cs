using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Maps.MapControl.WPF;
using System.Data;
using MyLoPhotoBrowserNS;
using MapPushpinPhotoDisplay;

namespace MyLoMapWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MyLoMap : Window
    {
        private MyLoPhotoBrowser _photoBrowser;

        private Dictionary<Pair<double, double>, long> _pins = new Dictionary<Pair<double, double>, long>();


        public MyLoPhotoBrowser PassedBrowser
        {
            get { return _photoBrowser; }
            set { _photoBrowser = value; }
        }


        public MyLoMap()
        {
            InitializeComponent();
            myloBingMap.Focus();
            myloBingMap.Mode = new AerialMode(true);
            myloBingMap.ZoomLevel = 4;
        }

        public void DisplayMap()
        {
            DataSet results = new DataSet();

            results = _photoBrowser.GetPhotosGroupedByLocation();

            if (results.Tables.Count != 0)
            {
                DataTable locations = new DataTable();
                locations = results.Tables[0];

                foreach (DataRow dr in locations.Rows)
                {
                    Pushpin pin = new Pushpin();
                    pin.IsEnabled = true;
                    pin.Location = new Location(Convert.ToDouble(dr["latitude"]), Convert.ToDouble(dr["longitude"]));
                    pin.ToolTip = dr["count"].ToString() + " photos";
                    pin.MouseDoubleClick += new MouseButtonEventHandler(Pushpin_MouseDoubleClick);
                    _pins.Add(new Pair<double, double>(pin.Location.Latitude, pin.Location.Longitude), (long)dr["locationid"]);
                    myloBingMap.Children.Add(pin);
                    myloBingMap.Center = pin.Location;
                }
            }
        }

        void Pushpin_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            Pushpin tempPP = new Pushpin();
            tempPP = (Pushpin)sender;
            Pair<double, double> newP = new Pair<double, double>(tempPP.Location.Latitude, tempPP.Location.Longitude);
            long locationId = 0;
            bool pinSelected = _pins.TryGetValue(newP, out locationId);

            if (pinSelected)
            {
                DataSet photoset = new DataSet();
                photoset = _photoBrowser.GetPhotosByLocation(locationId);
                DataTable photoDt = photoset.Tables[0];
                PushpinPhotos ppp = new PushpinPhotos();
                try
                {
                    ppp.PassedTable = photoDt;
                    ppp.buildPhotoLayout();
                    ppp.ShowDialog();
                }
                catch (Exception ex)
                {
                    System.Windows.Forms.MessageBox.Show(ex.Message);
                }
                Location loc = new Location(tempPP.Location.Latitude, tempPP.Location.Longitude);
                myloBingMap.Focus();
                myloBingMap.Center = loc;
            }
        }
    }


    public class Pair<T1, T2>
    {
        public T1 Left { get; private set; }
        public T2 Right { get; private set; }

        public Pair(T1 t1, T2 t2)
        {
            Left = t1;
            Right = t2;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof(Pair<T1, T2>)) return false;
            return Equals((Pair<T1, T2>)obj);
        }

        public bool Equals(Pair<T1, T2> obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return Equals(obj.Left, Left) && Equals(obj.Right, Right);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (Left.GetHashCode() * 397) ^ Right.GetHashCode();
            }
        }
    } 

}
