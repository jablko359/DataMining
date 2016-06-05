using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Policy;
using AGDSPresentationDB.Parser;
using AGDSPresentationDB.Tools;
using AGDSPresentationDB.ViewModels;
using QuickGraph;

namespace AGDSPresentationDB.AGDS
{
    public class AGDSGraph : PropertyChanger
    {
        private Dictionary<string, Node> _repetors;
        private List<Node> _allNodes;
        private int _maxDepth = 0;

        public const int MaxSearchDepth = 10;
        public const int MinSearchDepth = 1;

        public int MaxDepth
        {
            get { return _maxDepth; }
            set
            {
                _maxDepth = value;
                OnPropertyChanged(nameof(MaxDepth));
            }
        }



        public Dictionary<string, Node> Receptors
        {
            get { return _repetors; }
            set { _repetors = value; }
        }

        public List<Node> AllNodes
        {
            get { return _allNodes; }
            private set { _allNodes = value; }
        }

        public AGDSGraph(Dictionary<string, Node> receptors, List<Node> allNodes)
        {
            _repetors = receptors;
            _allNodes = allNodes;
        }
        #region Searching
    
        public List<Node> Search(Query queries, int maxDepth)
        {
            List<Node> result = SearchNodes(queries, maxDepth);
            foreach (Node node in _allNodes)
            {
                node.IsSelected = false;
            }
            foreach (var repetor in _repetors.Values)
            {
                foreach (var argument in repetor.Nodes.Values)
                {
                    if (argument.Nodes.Values.Any(item => result.Contains(item)))
                    {
                        result.Add(argument);
                        result.Add(repetor);
                    }
                }
            }
            foreach (Node node in result)
            {
                node.IsSelected = true;
            }
            return result;
        }

        private List<Node> SearchNodes(Query queries, int maxDepth)
        {
            ////ToDo parse by tree
            List<Node> results = new List<Node>();
            List<Node> leftResult = new List<Node>();
            List<Node> rightResult = new List<Node>();
            if (queries.LeftQuery != null && queries.RightQuery != null)
            {
                leftResult = SearchNodes(queries.LeftQuery, maxDepth);
                rightResult = SearchNodes(queries.RightQuery, maxDepth);
                switch (queries.Logic)
                {
                    case QueryLogic.And:
                        results = leftResult.Intersect(rightResult).ToList();
                        break;
                    case QueryLogic.Or:
                        results = leftResult.Union(rightResult).ToList();
                        break;
                }
            }
            else if (queries.Expression != null)
            {
                results.AddRange(Find(queries, maxDepth));
            }
            return results;
        }


        private List<Node> Find(Query inputQuery, int maxDepth)
        {
            List<Node> resultGraph = new List<Node>();
            foreach (Node node in _allNodes)
            {
                node.IsSelected = false;
            }
            Dictionary<Query, List<Node>> fullfilledDictionary = new Dictionary<Query, List<Node>>();
            Node receptorNode;
            if (_repetors.TryGetValue(inputQuery.Expression.QueryName.Trim(), out receptorNode))
            {
                receptorNode.IsSelected = true;
                string receptorName = receptorNode.Value.ToString();
                List<Node> items = receptorNode.Nodes.Values.ToList();

                foreach (Node item in items)
                {
                    if (CompareValues(inputQuery.Expression, item))
                    {
                        item.IsSelected = true;
                        foreach (Node node in item.Nodes.Values)
                        {
                            node.IsSelected = true;
                            DbPrimaryKey tableName = node.Value as DbPrimaryKey;
                            if (tableName != null)
                            {
                                node.SetConnectedNodesSelected(item, true, tableName.TableName, 0, maxDepth);
                            }
                        }
                    }
                }
            }
            foreach (var receptor in _repetors.Values)
            {
                if (receptor.Nodes.Any(item => item.Value.IsSelected))
                {
                    receptor.IsSelected = true;
                }
            }
            resultGraph.AddRange(_allNodes.Where(item => item.IsSelected && item.Value is DbPrimaryKey));

            return resultGraph;
        }
        #region Old
        //public void FindDepth(Dictionary<string, string> receptorDictionary)
        //{
        //    foreach (Node node in _allNodes)
        //    {
        //        node.IsSelected = false;
        //    }
        //    foreach (Node receptor in _repetors.Values)
        //    {
        //        receptor.IsSelected = true;
        //        receptor.CurrentDepth = 0;
        //        if (receptorDictionary.ContainsKey(receptor.Value.ToString().ToLower()))
        //        {
        //            List<Node> items = receptor.Nodes.Values.ToList();
        //            string searchingValue = receptorDictionary[receptor.Value.ToString().ToLower()];
        //            foreach (Node item in items)
        //            {
        //                if (item.Value.ToString().ToLower() == searchingValue)
        //                {
        //                    item.IsSelected = true;
        //                    item.CurrentDepth = 0;
        //                    foreach (Node node in item.Nodes.Values)
        //                    {
        //                        node.IsSelected = true;
        //                        node.CurrentDepth = 0;
        //                        DbPrimaryKey tableName = node.Value as DbPrimaryKey;
        //                        if (tableName != null)
        //                        {
        //                            node.SetConnectedNodes(item, 1);
        //                        }
        //                    }
        //                }
        //            }
        //        }
        //    }
        //    MaxDepth = _allNodes.Select(item => item.CurrentDepth).Max();
        //}
        #endregion
        #endregion
        private static bool CompareValues(Expression query, Node itemNode)
        {
            try
            {
                bool result = false;
                string itemNodeString = itemNode.Value as string;
                if (itemNodeString != null)
                {
                    switch (query.CompareType)
                    {
                        case CompareOperator.Equals:
                            result = string.Compare(itemNodeString, query.Value.ToString()) == 0;
                            break;
                        case CompareOperator.GreaterThan:
                            result = string.Compare(itemNodeString, query.Value.ToString()) > 0;
                            break;
                        case CompareOperator.LessThan:
                            result = string.Compare(itemNodeString, query.Value.ToString()) < 0;
                            break;
                        case CompareOperator.GreaterEqualsThan:
                            result = string.Compare(itemNodeString, query.Value.ToString()) >= 0;
                            break;
                        case CompareOperator.LessEqualThan:
                            result = string.Compare(itemNodeString, query.Value.ToString()) <= 0;
                            break;
                    }
                    return result;
                }
                IComparable itemComparable = itemNode.Value as IComparable;
                IComparable valueComparable = query.Value as IComparable;
                if (itemComparable != null && valueComparable != null)
                {
                    switch (query.CompareType)
                    {
                        case CompareOperator.Equals:
                            result = itemComparable.CompareTo(valueComparable) == 0;
                            break;
                        case CompareOperator.GreaterThan:
                            result = itemComparable.CompareTo(valueComparable) > 0;
                            break;
                        case CompareOperator.LessThan:
                            result = itemComparable.CompareTo(valueComparable) < 0;
                            break;
                        case CompareOperator.GreaterEqualsThan:
                            result = itemComparable.CompareTo(valueComparable) >= 0;
                            break;
                        case CompareOperator.LessEqualThan:
                            result = itemComparable.CompareTo(valueComparable) <= 0;
                            break;
                    }
                }
                return result;
            }
            catch (Exception ex)
            {
                throw new ArgumentException("Query and item are not same types", ex);
            }

        }

        public void DeleteItem(Node node)
        {
            List<Node> connectedNodes = node.Nodes.Values.ToList();
            node.Clear();
            if (node.Value is DbPrimaryKey)
            {
                foreach (Node connectedNode in connectedNodes)
                {
                    if (!connectedNode.Nodes.Values.Any(item => item.Value is DbPrimaryKey))
                    {
                        DeleteItem(connectedNode);
                    }
                }
            }
            _allNodes.Remove(node);
            _repetors.Remove(node.Key);
        }

        public void Reset()
        {
            MaxDepth = 0;
            foreach (Node node in _allNodes)
            {
                node.IsSelected = true;
                node.CurrentDepth = -1;
            }
        }

        public void HideDepth(int depth)
        {
            foreach (Node node in _allNodes)
            {
                if (node.CurrentDepth > depth)
                {
                    node.IsSelected = false;
                }
                else
                {
                    node.IsSelected = true;
                }
            }
        }
    }

    #region Nodes
    public class Node : PropertyChanger, IComparable<Node>
    {
        #region Private
        private readonly SortedDictionary<string, Node> _nodes = new SortedDictionary<string, Node>(new AlphanumComparatorFast());
        private readonly object _value;
        private bool _isSelected = true;
        private int _weight;
        private bool _isMarked;
        private string _key;
        #endregion
        #region Properties
        public object Value
        {
            get { return _value; }
        }

        public SortedDictionary<string, Node> Nodes
        {
            get { return _nodes; }
        }



        public bool IsMarked
        {
            get { return _isMarked; }
            set { _isMarked = value; }
        }


        public bool IsSelected
        {
            get { return _isSelected; }
            set
            {
                _isSelected = value;
                OnPropertyChanged(nameof(IsSelected));
            }
        }

        public string Key
        {
            get { return _key; }
        }

        private int _currentDepth = -1;

        public int CurrentDepth
        {
            get { return _currentDepth; }
            set
            {
                _currentDepth = value;
                OnPropertyChanged(nameof(CurrentDepth));
            }
        }



        public int Weight
        {
            get { return _weight; }
            set
            {
                _weight = value;
                OnPropertyChanged(nameof(Weight));
            }
        }
        #endregion

        public Node(object value)
        {
            _value = value;
            _key = value.ToString();
        }

        public Node(Node node)
        {
            _value = node.Value;
            _key = node.Key;
        }


        public void AddItem(Node node)
        {
            if (!_nodes.ContainsKey(node.Key))
            {
                _nodes.Add(node.Key, node);
            }
        }

        public int CompareTo(Node other)
        {
            if (other.Value.GetType().IsInstanceOfType(_value))
            {
                IComparable comparableValue = Value as IComparable;
                IComparable otherComparable = other.Value as IComparable;
                if (comparableValue != null && otherComparable != null)
                {
                    return comparableValue.CompareTo(otherComparable);
                }
            }
            return 0;
        }

        public override string ToString()
        {
            return _value.ToString();
        }

        public void SetConnectedNodesSelected(Node prevNode, bool updatePk, string tableName, int depth, int maxDepth)
        {
            if (depth >= maxDepth)
            {
                return;
            }
            foreach (Node node in Nodes.Values)
            {
                if (node != prevNode && !node.IsSelected)
                {
                    DbPrimaryKey pk = node.Value as DbPrimaryKey;
                    if (pk == null)
                    {
                        node.IsSelected = true;
                        node.SetConnectedNodesSelected(this, false, tableName, depth + 1, maxDepth);
                    }
                    else if (updatePk)
                    {
                        node.IsSelected = pk.TableName != tableName;
                        DbPrimaryKey previousPk = prevNode.Value as DbPrimaryKey;
                        bool selectNextNode = true;
                        if (previousPk != null)
                        {
                            selectNextNode = previousPk.TableName != pk.TableName;
                        }
                        if (pk.TableName != tableName && Value is DbPrimaryKey && selectNextNode)
                        {
                            node.IsSelected = true;
                            node.SetConnectedNodesSelected(this, true, tableName, depth + 1, maxDepth);
                        }
                        else
                        {
                            node.IsSelected = false;
                            continue;
                        }
                    }
                }
            }
        }

        public void SetConnectedNodes(Node prevNode, int depth)
        {
            foreach (Node node in Nodes.Values)
            {
                if (node != prevNode)
                {
                    if (depth < node.CurrentDepth || node.CurrentDepth == -1)
                    {
                        node.CurrentDepth = depth;
                    }
                }
            }
            foreach (Node node in Nodes.Values)
            {
                if (node != prevNode && (depth <= node.CurrentDepth || node.CurrentDepth == -1))
                {
                    node.IsSelected = true;
                    int newDepth = depth;
                    if (node.Value is DbPrimaryKey)
                    {
                        newDepth = depth + 1;
                    }
                    node.SetConnectedNodes(this, newDepth);
                }
            }
        }

        public void DeleteRealation(Node node)
        {
            _nodes.Remove(node.Key);
            OnPropertyChanged("Nodes");
        }

        public virtual void Clear()
        {
            foreach (Node itemNode in Nodes.Values)
            {
                itemNode.DeleteRealation(this);
            }
            _nodes.Clear();
            OnPropertyChanged("Nodes");
        }

    }

    public class HierarchicalNode : Node
    {
        private Node _parent;

        public Node Parent
        {
            get { return _parent; }
        }

        public HierarchicalNode(object value, Node patentNode = null) : base(value)
        {
            _parent = patentNode;
        }
        public override void Clear()
        {
            if (_parent != null)
            {
                _parent.DeleteRealation(this);
            }
            base.Clear();
        }

    }

    #region QucikGraph

    public class GraphEdge : Edge<Node>
    {

        public GraphEdge(Node source, Node target) : base(source, target)
        {
        }
    }

    public class NodesGraph : BidirectionalGraph<Node, GraphEdge>
    {
        public NodesGraph()
        {

        }

        public NodesGraph(bool allowParallelEdges)
            : base(allowParallelEdges)
        { }

        public NodesGraph(bool allowParallelEdges, int vertexCapacity)
            : base(allowParallelEdges, vertexCapacity)
        { }

        public void BuildGraph(AGDSGraph graph)
        {
            foreach (var node in graph.AllNodes)
            {
                AddVertex(node);
            }
            foreach (var edge in from node in graph.AllNodes from childNode in node.Nodes.Values select new GraphEdge(node, childNode))
            {
                AddEdge(edge);
            }
        }

        public void BuildGraphFromSelected(List<Node> nodes)
        {
            foreach (Node node in nodes)
            {
                if (node.IsSelected)
                {
                    AddVertex(node);
                }
            }
            foreach (Node node in nodes)
            {
                foreach (Node childNode in node.Nodes.Values)
                {
                    if (node.IsSelected && childNode.IsSelected)
                    {
                        GraphEdge edge = new GraphEdge(node, childNode);
                        AddEdge(edge);
                    }
                }
            }
        }

    }

    public class IsPrimaryKeyNodeComparer : IComparer<Node>
    {
        public int Compare(Node x, Node y)
        {
            if (x.Value is DbPrimaryKey && !(y.Value is DbPrimaryKey))
            {
                return -1;
            }
            if (y.Value is DbPrimaryKey && !(x.Value is DbPrimaryKey))
            {
                return -1;
            }
            return 0;

        }
    }

    #endregion

    #endregion



}
