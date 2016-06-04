using AGDS;
using ConsoleApplication1;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
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


namespace AGDSVisual
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        private List<GraphNode> _treeItems = new List<GraphNode>();
        private List<GraphNode> _searchResult = new List<GraphNode>();
        private double _probability = 0;
        private Iris _searchItem = new Iris();
        AGDSGraph _graph;


        public double Probability
        {
            get { return _probability; }
            set
            {
                _probability = value;
                NotifyPropertyChanged("Probability");
            }
        }

        public List<GraphNode> SearchResult
        {
            get { return _searchResult; }
            set
            {
                _searchResult = value;
                NotifyPropertyChanged("SearchResult");
            }
        }

        public Iris SearchItem
        {
            get { return _searchItem; }
            set { _searchItem = value; }
        }

        public List<GraphNode> TreeItems
        {
            get { return _treeItems; }
            set
            {
                _treeItems = value;
                NotifyPropertyChanged("TreeItems");
            }
        }


        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;

        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(String propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        private void Load(object sender, RoutedEventArgs e)
        {
            CSVDeserializer.CSVSerializer<Iris> serializer = new CSVDeserializer.CSVSerializer<Iris>()
            {
                Separator = '\t'
            };
            IList<Iris> irises;
            System.Windows.Forms.OpenFileDialog openFileDialog = new System.Windows.Forms.OpenFileDialog();
            if (openFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                try
                {
                    Stream openStream;
                    if ((openStream = openFileDialog.OpenFile()) != null)
                    {
                        irises = serializer.Deserialize(stream: openStream);
                        AGDSBuilder builder = new AGDSBuilder(typeof(Iris));
                        _graph = builder.BuildGraph(new List<object>(irises));
                        //List<object> items = graph.FindLike(new Iris()
                        //{
                        //    Class = "Setosa",
                        //    LeafLength = 5.1,
                        //    LeafWidth = 3.5,
                        //    PetalLength = 1.4,
                        //    PetalWidth = 0.2

                        //}, 1);
                        TreeItems = _graph.ParamNode.Children;
                    }
                }
                catch (IOException ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        private void IrisInput_IrisChanged(object sender, EventArgs e)
        {
            if (_graph != null)
            {
               SearchResult = _graph.FindLike(SearchItem, Probability);
               NotifyPropertyChanged("TreeItems");
            }
            else
            {
                MessageBox.Show("Nie wczytano żadnych danych!", "Błąd");
            }
            
        }
    }
}
