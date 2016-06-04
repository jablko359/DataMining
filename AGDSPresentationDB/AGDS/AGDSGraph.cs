using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Policy;
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
        public const int MinSearchDepth = 10;

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
        #region Find
        public void FindInGraph(IReadOnlyDictionary<string, Query> receptorDictionary)
        {
            foreach (var node in _allNodes)
            {
                node.Weight = 0;
            }
            foreach (var nodeQuery in receptorDictionary)
            {
                Node receptorNode;
                if (_repetors.TryGetValue(nodeQuery.Key, out receptorNode))
                {
                    receptorNode.Weight = int.MinValue;
                    foreach (var value in receptorNode.Nodes.Values)
                    {
                        if (CompareValues(nodeQuery.Value, value))
                        {
                            value.Weight = int.MinValue;
                            foreach (var node in value.Nodes.Values)
                            {
                                node.Weight++;
                                foreach (var subNode in node.Nodes.Values)
                                {
                                    subNode.Weight++;
                                    if (subNode.Value is DbPrimaryKey)
                                    {

                                        foreach (Node itemNode in subNode.Nodes.Values)
                                        {
                                            itemNode.Weight++;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }

            //foreach (Node receptor in _repetors.Values)
            //{
            //    if (receptorDictionary.ContainsKey(receptor.Value.ToString().ToLower()))
            //    {
            //        Query qr = receptorDictionary[receptor.Value.ToString().ToLower()];
            //        receptor.Weight = Int32.MinValue;
            //        List<Node> items = receptor.Nodes.Values.ToList();
            //        foreach (Node item in items)
            //        {
            //            if (CompareValues(qr, item))
            //            {
            //                item.Weight = Int32.MinValue;
            //                foreach (Node node in item.Nodes.Values)
            //                {
            //                    node.IsMarked = true;
            //                    node.Weight++;
            //                }
            //            }
            //        }
            //    }
            //}
            ////TodO ?
            //foreach (var node in _allNodes.Where(node => node.IsMarked))
            //{
            //    foreach (Node subNode in node.Nodes.Values)
            //    {
            //        subNode.Weight++;
            //        foreach (Node parameterNode in subNode.Nodes.Values.Where(parameterNode => !(parameterNode.Value is DbPrimaryKey)))
            //        {
            //            parameterNode.Weight++;
            //        }
            //    }
            //}
        }
       
        public List<Node> SearcNodes(IReadOnlyDictionary<string, Query> queries, int maxDepth)
        {
            //ToDo parse by tree
            List<Node> results = new List<Node>();
            foreach (var pair in queries)
            {
                results.AddRange(Find(pair.Value, maxDepth));
            }
            //ToDo set Is Selected based on query and quqer list


            return results;
        }

      
        public List<Node> Find(Query inputQuery, int maxDepth)
        {
            List<Node> resultGraph = new List<Node>();
            foreach (Node node in _allNodes)
            {
                node.IsSelected = false;
            }
            Dictionary<Query, List<Node>> fullfilledDictionary = new Dictionary<Query, List<Node>>();
            Node receptorNode;
            if (_repetors.TryGetValue(inputQuery.QueryName.Trim(), out receptorNode))
            {
                receptorNode.IsSelected = true;
                string receptorName = receptorNode.Value.ToString();
                List<Node> items = receptorNode.Nodes.Values.ToList();

                foreach (Node item in items)
                {
                    if (CompareValues(inputQuery, item))
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
            resultGraph.AddRange(_allNodes.Where(item => item.IsSelected));
            
            return resultGraph;
        }

        public void FindDepth(IReadOnlyDictionary<string, string> receptorDictionary)
        {
            foreach (Node node in _allNodes)
            {
                node.IsSelected = false;
            }
            foreach (Node receptor in _repetors.Values)
            {
                receptor.IsSelected = true;
                receptor.CurrentDepth = 0;
                if (receptorDictionary.ContainsKey(receptor.Value.ToString().ToLower()))
                {
                    List<Node> items = receptor.Nodes.Values.ToList();
                    string searchingValue = receptorDictionary[receptor.Value.ToString().ToLower()];
                    foreach (Node item in items)
                    {
                        if (item.Value.ToString().ToLower() == searchingValue)
                        {
                            item.IsSelected = true;
                            item.CurrentDepth = 0;
                            foreach (Node node in item.Nodes.Values)
                            {
                                node.IsSelected = true;
                                node.CurrentDepth = 0;
                                DbPrimaryKey tableName = node.Value as DbPrimaryKey;
                                if (tableName != null)
                                {
                                    node.SetConnectedNodes(item, 1);
                                }
                            }
                        }
                    }
                }
            }
            MaxDepth = _allNodes.Select(item => item.CurrentDepth).Max();
        }
        #endregion
        private static bool CompareValues(Query query, Node itemNode)
        {
            try
            {
                bool result = false;
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


            node.Clear();
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
