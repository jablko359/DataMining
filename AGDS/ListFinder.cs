using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace AGDS
{
    public class ListFinder<T>
    {
        private readonly List<T> _objects;
        private readonly List<ListNode<T>> _nodes = new List<ListNode<T>>();

        public ListFinder(List<T> objects)
        {
            _objects = objects;
            CreateList();
        }
        
        private void CreateList()
        {
            foreach(T obj in _objects)
            {
                _nodes.Add(new ListNode<T>(obj));
            }
        }        

        public List<ListNode<T>> FindLike(T searchItem, double probability)
        {            
            foreach(var lNode in _nodes)
            {
                foreach(PropertyInfo property in searchItem.GetType().GetProperties())
                {
                    if(property.GetMethod.ReturnType == typeof(double))
                    {
                        double propValue = (double)property.GetValue(searchItem);
                        lNode.CountPropertyWeight(property.Name, propValue);
                    }
                    
                    
                }
                lNode.CountWeight();
            }
            return _nodes.Where(item => item.Weight > probability).ToList();
        }
    }

    public class ListNode<T>
    {
        private T _item;
        private readonly Dictionary<string,double> _currentWeights = new Dictionary<string, double>();
        private readonly Dictionary<string, double> _properties = new Dictionary<string, double>();
        private double _weight;
        private readonly static Dictionary<string, double> _maxValues = new Dictionary<string, double>();
        private readonly static Dictionary<string, double> _minValues = new Dictionary<string, double>();

        public T Item
        {
            get { return _item; }            
        }
        public double Weight
        {
            get { return _weight; }
        }

        public ListNode(T item)
        {
            _item = item;
            foreach(PropertyInfo property in item.GetType().GetProperties())
            {
                if(property.GetMethod.ReturnType == typeof(double))
                {
                    _properties.Add(property.Name, (double)property.GetValue(item));
                    if (!_maxValues.ContainsKey(property.Name))
                    {
                        _maxValues.Add(property.Name, (double)property.GetValue(item));
                    }
                    else if(_maxValues[property.Name] < (double)property.GetValue(item))
                    {
                        _maxValues[property.Name] = (double)property.GetValue(item);
                    }
                    if (!_minValues.ContainsKey(property.Name))
                    {
                        _minValues.Add(property.Name, (double)property.GetValue(item));
                    }
                    else if (_minValues[property.Name] > (double)property.GetValue(item))
                    {
                        _minValues[property.Name] = (double)property.GetValue(item);
                    }
                }                             
            }
        }

        public double GetPropertyWeight(string propertyName)
        {
            if (_currentWeights.ContainsKey(propertyName))
            {
                return _currentWeights[propertyName];
            }
            else
            {
                return 0;
            }
        }

        public void SetPropertyWeight(string propertyName, double weight)
        {
            if (_currentWeights.ContainsKey(propertyName))
            {
                _currentWeights[propertyName] = weight;
            }
            else
            {
                _currentWeights.Add(propertyName, weight);
            }
        }

        public List<string> GetProperties()
        {
            return _properties.Keys.ToList();
        }

        public static double GetMaxValue(string propertyName)
        {
            if (_maxValues.ContainsKey(propertyName))
            {
                return _maxValues[propertyName];
            }
            else
            {
                return 0;
            }
        }

        public static double GetMinValue(string propertyName)
        {
            if (_minValues.ContainsKey(propertyName))
            {
                return _minValues[propertyName];
            }
            else
            {
                return 0;
            }
        }

        public void CountPropertyWeight(string propertyName,double value)
        {
            double weight = 1 - (Math.Abs(value - _properties[propertyName]) / (_maxValues[propertyName] - _minValues[propertyName]));
            SetPropertyWeight(propertyName,weight);
        }

        public void CountWeight()
        {
            double weight = 0;
            foreach(double value in _currentWeights.Values)
            {
                weight += value;
            }
            weight /= (double)_currentWeights.Count;
            _weight = weight;
        }
        
    }
}
