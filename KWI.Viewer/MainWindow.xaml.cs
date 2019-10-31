using KWI.Format.Reader;
using KWI.Format.Structure;
using KWI.Format.Structure.BackgroundFrame;
using KWI.Format.Structure.Base;
using KWI.Viewer.MapRender;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace KWI.Viewer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private DVDReader _reader = null;
        private INode _rootNode = null;

        public MainWindow()
        {
            InitializeComponent();

            this.Loaded += MainWindow_Loaded;
        }

        private async void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            _reader = new DVDReader();
            _rootNode = _reader.Read();
            AssetTree.ItemsSource = new INode[] { _rootNode };
            var allData = _rootNode as AllDataFrame;
            var mapData = allData.Childs.OfType<ManagementHeaderRecord<ParcelRelatedDataFrame>>().FirstOrDefault();
            if (mapData != null)
                MapTab.DataContext = new Renderer(MapCanvas, mapData.Frame);
        }
        
        private void AssetTree_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (e.NewValue is RecordBase record)
                record.LoadChilds();
            //Task.Run(() =>
            //{

            //});
        }

    }
}
