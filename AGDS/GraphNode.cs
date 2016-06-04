using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ConsoleApplication1;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace AGDS
{
    public class GraphNode : INotifyPropertyChanged
    {
        public const string ClassNodeName = "class";

        #region Private
        private List<GraphNode> _children = new List<GraphNode>();
        private GraphNode _parent;
        private readonly object _value;
        private double _currentWeight;
        private double _maxParam = Double.MinValue;
        private double _minParam = Double.MaxValue;

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(String propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        #endregion

        #region Properties        
        public double CurrentWeight
        {
            get { return _currentWeight; }
            set
            {
                _currentWeight = value;
                NotifyPropertyChanged("CurrentWeight");
            }
        }
        public double MaxParam
        {
            get { return _maxParam; }
            set { _maxParam = value; }
        }

        public double MinParam
        {
            get { return _minParam; }
            set { _minParam = value; }
        }
        public GraphNode Parent
        {
            get { return _parent; }
        }
        public List<GraphNode> Children
        {
            get { return _children; }
        }
        public object Value
        {
            get { return _value; }
        }
        #endregion

        #region Constructor

        public GraphNode(object value,GraphNode parent)
        {
            _parent = parent;
            _value = value;
        }

        #endregion

        public bool ChildrenContainsValue(object value)
        {
            foreach (GraphNode node in Children)
            {
                if(node == value)
                {
                    return true;
                }
            }
            return false;
        }

        

        public bool ContainsValue(object obj)
        {
            foreach(GraphNode node in Children)
            {
                if (node.Value.Equals(obj))
                {
                    return true;
                }
            }
            return false;
        }
    }

    public class GraphNodeDoubleComparer : IComparer<GraphNode>
    {
        public int Compare(GraphNode x, GraphNode y)
        {
            if(x.Value is double && y.Value is double)
            {
                double xDouble = (double)x.Value;
                double yDouble = (double)y.Value;
                return xDouble.CompareTo(yDouble);
            }
            return 0;
        }
    }

    //public class StringNode : GraphNode
    //{
    //    #region Private
    //    private readonly string _name;


    //    #endregion

    //    #region Properties 
    //    public string Value
    //    {
    //        get { return _name; }
    //    }




    //    #endregion

    //    public StringNode(string name, GraphNode parent) : base(parent)
    //    {
    //        _name = name;
    //    }




    //}

    //public class ItemNode : GraphNode
    //{
    //    private readonly IClassificable _item;

    //    public IClassificable Item
    //    {
    //        get { return _item; }
    //    }

    //    public ItemNode(IClassificable item, GraphNode parent) : base(parent)
    //    {
    //        _item = item;
    //    }
    //}

    //public class ValueNode : GraphNode, IComparable
    //{
    //    private readonly double _value;

    //    public double Value
    //    {
    //        get { return _value; }
    //    }

    //    public ValueNode(double value, GraphNode parent)
    //        : base(parent)
    //    {
    //        _value = value;
    //    }


    //}    
}
