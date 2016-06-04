using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using AGDSPresentationDB.Tools;
using AGDSPresentationDB.ViewModels;
using QuickGraph;

namespace AGDSPresentationDB.AGDS
{
    public class AGDSGraph : PropertyChanger
    {
        private List<Node> _repetors;
        private List<Node> _allNodes;
        private int _maxDepth = 0;
        


        public int MaxDepth
        {
            get { return _maxDepth; }
            set
            {
                _maxDepth = value;
                OnPropertyChanged(nameof(MaxDepth));
            }
        }



        public List<Node> Receptors
        {
            get { return _repetors; }
            set { _repetors = value; }
        }

        public List<Node> AllNodes
        {
            get { return _allNodes; }
            private set { _allNodes = value; }
        }

        public AGDSGraph(List<Node> receptors, List<Node> allNodes)
        {
            _repetors = receptors;
            _allNodes = allNodes;
        }

        public void FindInGraph(IReadOnlyDictionary<string, Query> receptorDictionary)
        {
            foreach (Node Node in _allNodes)
            {
                Node.Weight = 0;
            }
            foreach (Node receptor in _repetors)
            {
                if (receptorDictionary.ContainsKey(receptor.Value.ToString().ToLower()))
                {
                    Query qr = receptorDictionary[receptor.Value.ToString().ToLower()];
                    receptor.Weight = Int32.MinValue;
                    List<Node> items = receptor.Nodes;
                    foreach (Node item in items)
                    {
                        if (CompareValues(qr, item))
                        {
                            item.Weight = Int32.MinValue;
                            foreach (Node node in item.Nodes)
                            {
                                node.IsMarked = true;
                                node.Weight++;
                            }
                        }
                    }
                }
            }
            //TodO ?
            foreach (var node in _allNodes.Where(node => node.IsMarked))
            {
                foreach (Node subNode in node.Nodes)
                {
                    subNode.Weight++;
                    foreach (Node parameterNode in subNode.Nodes.Where(parameterNode => !(parameterNode.Value is DbPrimaryKey)))
                    {
                        parameterNode.Weight++;
                    }
                }
            }
        }

        public void Find(IReadOnlyDictionary<string, Query> queries)
        {
            foreach (Node node in _allNodes)
            {
                node.IsSelected = false;
            }
            foreach (Node receptor in _repetors)
            {
                receptor.IsSelected = true;
                string receptorName = receptor.Value.ToString().ToLower();
                if (queries.ContainsKey(receptorName.ToLower()))
                {
                    Query qr = queries[receptorName];
                    List<Node> items = receptor.Nodes;

                    foreach (Node item in items)
                    {
                        if (CompareValues(qr, item))
                        {
                            item.IsSelected = true;
                            foreach (Node node in item.Nodes)
                            {
                                node.IsSelected = true;
                                DbPrimaryKey tableName = node.Value as DbPrimaryKey;
                                if (tableName != null)
                                {
                                    node.SetConnectedNodesSelected(item, true, tableName.TableName);
                                }
                            }
                        }
                    }
                }
            }

        }

        public void FindDepth(IReadOnlyDictionary<string, string> receptorDictionary)
        {
            foreach (Node node in _allNodes)
            {
                node.IsSelected = false;
            }
            foreach (Node receptor in _repetors)
            {
                receptor.IsSelected = true;
                receptor.CurrentDepth = 0;
                if (receptorDictionary.ContainsKey(receptor.Value.ToString().ToLower()))
                {
                    List<Node> items = receptor.Nodes;
                    string searchingValue = receptorDictionary[receptor.Value.ToString().ToLower()];
                    foreach (Node item in items)
                    {
                        if (item.Value.ToString().ToLower() == searchingValue)
                        {
                            item.IsSelected = true;
                            item.CurrentDepth = 0;
                            foreach (Node node in item.Nodes)
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
            _repetors.Remove(node);
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
        private readonly List<Node> _nodes = new List<Node>();
        private readonly object _value;
        private bool _isSelected = true;
        private int _weight;
        private bool _isMarked;

        public object Value
        {
            get { return _value; }
        }

        public List<Node> Nodes
        {
            get { return _nodes; }
        }

        public Node(object value)
        {
            _value = value;
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


        public void AddItem(Node node)
        {
            if (!_nodes.Contains(node))
            {
                _nodes.Add(node);
                _nodes.Sort();
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

        public void SetConnectedNodesSelected(Node prevNode, bool updatePk, string tableName)
        {
            foreach (Node node in Nodes)
            {
                if (node != prevNode && !node.IsSelected)
                {
                    DbPrimaryKey pk = node.Value as DbPrimaryKey;
                    if (pk == null)
                    {
                        node.IsSelected = true;
                        node.SetConnectedNodesSelected(this, false, tableName);
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
                            node.SetConnectedNodesSelected(this, true, tableName);
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
            foreach (Node node in Nodes)
            {
                if (node != prevNode)
                {
                    if (depth < node.CurrentDepth || node.CurrentDepth == -1)
                    {
                        node.CurrentDepth = depth;
                    }
                }
            }
            foreach (Node node in Nodes)
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
            _nodes.Remove(node);
            OnPropertyChanged("Nodes");
        }

        public virtual void Clear()
        {
            foreach (Node itemNode in Nodes)
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
            foreach (Node node in graph.AllNodes)
            {
                AddVertex(node);
            }
            foreach (Node node in graph.AllNodes)
            {
                foreach (Node childNode in node.Nodes)
                {
                    GraphEdge edge = new GraphEdge(node, childNode);
                    AddEdge(edge);
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
