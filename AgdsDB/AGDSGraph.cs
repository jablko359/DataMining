using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QuickGraph;

namespace AgdsDB
{
    public class AGDSGraph
    {
        private List<Node> _repetors;
        private List<Node> _allNodes;

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


        //public List<Node> Find(string receptorName, object value)
        //{

        //} 
    }

    #region Nodes
    public class Node : IComparable<Node>
    {
        private readonly List<Node> _nodes = new List<Node>();
        private readonly object _value;

        public object Value
        {
            get { return _value; }
        }

        public List<Node> Nodes
        {
            get { return _nodes.ToList(); }
        }

        public Node(object value)
        {
            _value = value;
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
            _parent = null;
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


    #endregion

    #endregion



}
