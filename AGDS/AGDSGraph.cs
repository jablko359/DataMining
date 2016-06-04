using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ConsoleApplication1;
using System.Reflection;

namespace AGDS
{
    public class AGDSGraph
    {
        private GraphNode _paramNode;
        private readonly List<GraphNode> _items;

        public GraphNode ParamNode
        {
            get { return _paramNode; }
        }

        

        public AGDSGraph(GraphNode paramNode, List<GraphNode> items)
        {          
            _paramNode = paramNode;
            _items = items;
        }

        public List<GraphNode> FindLike(object item, double probability)
        {
            Dictionary<string, double> propertyInfoDict = new Dictionary<string, double>();
            PropertyInfo[] properties = item.GetType().GetProperties();
            foreach (PropertyInfo property in properties)
            {
                if (property.GetMethod.ReturnType == typeof(double))
                {
                    propertyInfoDict.Add(property.Name, (double)property.GetValue(item));
                }
            }
            foreach (GraphNode node in ParamNode.Children)
            {
                string currentPropertyName = (string)node.Value;
                foreach (GraphNode valueNode in node.Children)
                {
                    if(valueNode.Value is double)
                    {
                        double currentValue = (double)valueNode.Value;
                        double min = node.MinParam < propertyInfoDict[currentPropertyName] ? node.MinParam : propertyInfoDict[currentPropertyName];
                        double max = node.MaxParam > propertyInfoDict[currentPropertyName] ? node.MaxParam : propertyInfoDict[currentPropertyName];
                        valueNode.CurrentWeight = 1 - ((Math.Abs(propertyInfoDict[currentPropertyName] - currentValue)) / (max - min));
                        if (valueNode.CurrentWeight < 0)
                        {
                            valueNode.CurrentWeight = 0;
                        }
                    }                    
                }
            }
            foreach(GraphNode itemNode in _items)
            {
                double mean = 0;
                foreach(GraphNode valueNode in itemNode.Children)
                {
                    mean += valueNode.CurrentWeight;
                }
                mean /= itemNode.Children.Count;
                itemNode.CurrentWeight = mean;
            }
            return _items.Where(i => i.CurrentWeight >= probability).ToList();
        }
    }
}
