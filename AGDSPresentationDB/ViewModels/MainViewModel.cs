using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data.SqlClient;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows;
using AGDSPresentationDB.AGDS;
using AGDSPresentationDB.Annotations;
using AGDSPresentationDB.Parser;
using AGDSPresentationDB.Services;
using AGDSPresentationDB.Tools;
using AGDSPresentationDB.Windows;
using Antlr4.Runtime;
using Antlr4.Runtime.Tree;
using GraphSharp.Controls;

namespace AGDSPresentationDB.ViewModels
{
    public class MainViewModel : PropertyChanger
    {
        #region Private members

        private string _dbName;
        private string _searchText;
        private AGDSGraph _graph;

        private Command _loadCommand;
        private Command _openDbSelectCommand;
        private Command _closeCommand;
        private Command _searchCommand;
        private Command _resetCommand;
        private Command _deleteNodeCommand;
        private Command _hideDepthCommand;

        private string _selectedLayout = "LinLog";
        private List<String> _layoutAlgorithmTypes = new List<string>(new[] { "BoundedFR", "Circular", "CompoundFDP", "EfficientSugiyama", "FR", "ISOM", "KK", "LinLog", "Tree" });
        private NodesGraph _visualGraph;
        private NodesGraph _selectedItemsGraph;

        private bool _isGraphAvaliable = true;
        private IWindowService _windowService = new WindowService();
        private int _searchDepth = 3;

        private const int NodesLimit = 300;

        #endregion
        #region Properties
        

        public int SearchDepth
        {
            get { return _searchDepth; }
            set
            {
                _searchDepth = value;
                OnPropertyChanged(nameof(SearchDepth));
            }
        }

        public double MaxDepth
        {
            get
            {
                if (_graph == null)
                {
                    return 0;
                }
                return _graph.MaxDepth;
            }
        }


        public string SearchText
        {
            get { return _searchText; }
            set
            {
                _searchText = value;
                OnPropertyChanged("SearchText");
            }
        }


        public string DbName
        {
            get { return _dbName; }
            set
            {
                _dbName = value;
                OnPropertyChanged(nameof(DbName));
            }
        }
        public ObservableCollection<Node> Nodes
        {
            get
            {
                if (_graph != null)
                {
                    return new ObservableCollection<Node>(_graph.Receptors.Values);
                }
                return new ObservableCollection<Node>();
            }
        }
        public NodesGraph VisualGraph
        {
            get { return _visualGraph; }
            set
            {
                _visualGraph = value;
                OnPropertyChanged(nameof(VisualGraph));
            }
        }

        

        public NodesGraph SelectedItemsGraph
        {
            get { return _selectedItemsGraph; }
            set
            {
                _selectedItemsGraph = value;
                OnPropertyChanged(nameof(SelectedItemsGraph));
            }
        }

        public bool IsGraphAvaliable
        {
            get { return _isGraphAvaliable; }
            set
            {
                _isGraphAvaliable = value;
                OnPropertyChanged(nameof(IsGraphAvaliable));
            }
        }
        public List<string> Layouts
        {
            get { return _layoutAlgorithmTypes; }
        }
        public string SelectedLayout
        {
            get { return _selectedLayout; }
            set
            {
                _selectedLayout = value;
                OnPropertyChanged(nameof(SelectedLayout));
            }
        }

        public AGDSGraph AgdsGraph
        {
            get { return _graph; }
        }
        #region Commands
        public Command LoadCommand
        {
            get { return _loadCommand; }
        }
        public Command OpenDbSelectCommand
        {
            get { return _openDbSelectCommand; }
        }
        public Command CloseCommand
        {
            get { return _closeCommand; }
        }
        public Command SearchCommand
        {
            get { return _searchCommand; }
        }
        public Command ResetCommand
        {
            get { return _resetCommand; }
        }

        public Command DeleteNodeCommand
        {
            get { return _deleteNodeCommand; }
        }

        public Command HideDepthCommand
        {
            get { return _hideDepthCommand; }
        }

        #endregion

        #endregion
        #region Ctor
        public MainViewModel()
        {
            _loadCommand = new Command(LoadDatabase);
            _openDbSelectCommand = new Command(OpenDb);
            _closeCommand = new Command(Close);
            _searchCommand = new Command(Search);
            _resetCommand = new Command(Reset);
            _deleteNodeCommand = new Command(DeleteNode);
            _hideDepthCommand = new Command(HideDepth);
        }
        #endregion
        #region CommandDelegates

        private void Search(object paramerer)
        {
            try
            {
                var querry = (paramerer as string).Trim();
                if (!string.IsNullOrEmpty(querry) && _graph != null)
                {
                    Stream str = new MemoryStream();
                    StreamWriter writer = new StreamWriter(str);
                    writer.Write(querry);
                    writer.Flush();
                    str.Position = 0;
                    
                    AntlrInputStream inputStream = new AntlrInputStream(str);
                    QueryLexer lexer = new QueryLexer(inputStream);
                    CommonTokenStream tokenStream = new CommonTokenStream(lexer);
                    QueryParser parser = new QueryParser(tokenStream);
                    QueryParser.QueryContext context = parser.query();

                    ParseTreeWalker walker = new ParseTreeWalker();
                    InputQueryInterpreter interpreter = new InputQueryInterpreter();
                    walker.Walk(interpreter, context);
                    str.Close();
                    List<Node> selected = _graph.Search(interpreter.TopQuery, SearchDepth);
                    NodesGraph selectedNodesGraph = new NodesGraph(false);
                    selectedNodesGraph.BuildGraphFromSelected(selected);
                    SelectedItemsGraph = selectedNodesGraph;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }


        }

        private void LoadDatabase(object parameter)
        {
            string dbName = parameter as string;
            if (!string.IsNullOrEmpty(dbName))
            {
                try
                {
                    dbName = dbName.Trim();
                    string connectionString =
                        string.Format("Data Source=IGOR-KOMPUTER;Initial Catalog={0};Integrated Security=SSPI;", dbName);
                    using (SqlConnection conn = new SqlConnection(connectionString))
                    {
                        DbDecompositor decompositor = new DbDecompositor(conn);
                        AgdsBuilder builder = new AgdsBuilder(decompositor);
                        _graph = builder.BuildGraph();
                        BuildVisualGraph();
                        OnPropertyChanged("Nodes");
                        OnPropertyChanged("MaxDepth");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(string.Format("Error while creting graph \n{0}\n{1}", ex.Message, ex.StackTrace));
                }
            }
        }

        private void OpenDb(object parameter)
        {
            _windowService.ShowWindow<DatabaseSelectWindow>();
        }

        private void Close(object parameter)
        {
            App.Current.Shutdown();
        }

        private void Reset(object parameter)
        {
            if (_graph != null)
            {
                _graph.Reset();
                SearchText = string.Empty;
            }
        }

        private void DeleteNode(object parameter)
        {
            Node node = parameter as Node;
            if (node != null && _graph != null)
            {
                _graph.DeleteItem(node);
            }
            BuildVisualGraph();
            OnPropertyChanged("Nodes");
        }

        private void HideDepth(object parameter)
        {
            int value = (int)((double)parameter);
            if (_graph != null)
            {
                _graph.HideDepth(value);
            }
        }
        #endregion

        private void BuildVisualGraph()
        {
            if (_graph != null && _graph.AllNodes.Count < NodesLimit)
            {
                NodesGraph visualGraph = new NodesGraph(false);
                visualGraph.BuildGraph(_graph);
                IsGraphAvaliable = true;
                VisualGraph = visualGraph;
            }
            else
            {
                MessageBox.Show("Too big graph. Graph tab will be unavaliable!", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                IsGraphAvaliable = false;
            }
        }

    }

    public enum SearchOption
    {
        Default, Depth, Extended
    }


    public class GraphLayout : GraphLayout<Node, GraphEdge, NodesGraph> { }
}
