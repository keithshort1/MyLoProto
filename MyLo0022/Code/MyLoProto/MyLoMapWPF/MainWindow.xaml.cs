using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
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

namespace MyLoMapWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MyLoMap : Window
    {

        private DataTable _passedTable;

        public DataTable PassedTable
        {
            get { return _passedTable; }
            set { _passedTable = value; }
        }

        public MyLoMap()
        {
            InitializeComponent();
            myloBingMap.Focus();
            myloBingMap.Mode = new AerialMode(true);
            myloBingMap.ZoomLevel = 3;
        }

        public void DisplayMap()
        {
            if (_passedTable != null)
            {
                foreach (DataRow dr in _passedTable.Rows)
                {
                    Pushpin pin = new Pushpin();
                    pin.IsEnabled = true;
                    pin.Location = new Location(Convert.ToDouble(dr["latitude"]), Convert.ToDouble(dr["longitude"]));
                    pin.ToolTip = dr["count"].ToString() + " photos";
                    
                    myloBingMap.Children.Add(pin);
                    myloBingMap.Center = pin.Location;
                }
            }

        }
    }
}
