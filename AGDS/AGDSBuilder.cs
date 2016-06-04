using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using ConsoleApplication1;

namespace AGDS
{
    public class AGDSBuilder
    {
        private readonly Type _itemType;
        private readonly PropertyInfo[] _properties;

        public AGDSBuilder(Type itemType)
        {
            _itemType = itemType;
            _properties = itemType.GetProperties().Where(value => !Attribute.IsDefined(value, typeof(SkipPropertyAttribute))).ToArray();
        }

        public AGDSGraph BuildGraph(List<object> items)
        {
            GraphNode paramNode = new GraphNode("param", null);

            //lists and dict to connect both trees
            List<GraphNode> itemNodeList = new List<GraphNode>();
            Dictionary<string, List<GraphNode>> valuesDict = new Dictionary<string, List<GraphNode>>();

            //Create Nodes    
            foreach (PropertyInfo property in _properties)
            {
                if (property.GetMethod.ReturnType == typeof(double))
                {
                    valuesDict.Add(property.Name, new List<GraphNode>());
                    GraphNode propertyNode = new GraphNode(property.Name, paramNode);
                    paramNode.Children.Add(propertyNode);
                    foreach (object item in items)
                    {
                        double value = (double)property.GetValue(item);
                        if (!propertyNode.ContainsValue(value))
                        {
                            GraphNode valueNode = new GraphNode(value, propertyNode);
                            propertyNode.Children.Add(valueNode);
                            if (value > propertyNode.MaxParam)
                            {
                                propertyNode.MaxParam = value;
                            }
                            if (value < propertyNode.MinParam)
                            {
                                propertyNode.MinParam = value;
                            }
                            valuesDict[property.Name].Add(valueNode);
                        }

                    }
                    //ToDo: Sort
                    propertyNode.Children.Sort(new GraphNodeDoubleComparer());
                }
            }
            GraphNode classNode = new GraphNode(GraphNode.ClassNodeName, paramNode);
            paramNode.Children.Add(classNode);
            Dictionary<string, GraphNode> typeNodes = new Dictionary<string, GraphNode>();

            foreach (object item in items)
            {
                IClassificable classificable = item as IClassificable;
                if (classificable != null)
                {
                    GraphNode parentNode;
                    if (!typeNodes.ContainsKey(classificable.Class))
                    {
                        GraphNode typeNode = new GraphNode(classificable.Class, classNode);
                        classNode.Children.Add(typeNode);
                        parentNode = typeNode;
                        typeNodes.Add(classificable.Class, typeNode);
                    }
                    else
                    {
                        parentNode = typeNodes[classificable.Class];
                    }
                    GraphNode itemNode = new GraphNode(classificable, parentNode);
                    parentNode.Children.Add(itemNode);
                    itemNodeList.Add(itemNode);
                }
            }

            foreach (GraphNode itemNode in itemNodeList)
            {
                PropertyInfo[] itemProperties = itemNode.Value.GetType().GetProperties();
                foreach (PropertyInfo property in itemProperties)
                {
                    if (property.GetMethod.ReturnType == typeof(double))
                    {
                        double propertyValue = (double)property.GetValue(itemNode.Value);
                        List<GraphNode> values;
                        if (valuesDict.TryGetValue(property.Name, out values))
                        {
                            foreach (GraphNode val in values)
                            {
                                if (val.Value.Equals(propertyValue))
                                {
                                    val.Children.Add(itemNode);
                                    itemNode.Children.Add(val);
                                }
                            }
                        }
                    }

                }

            }
            return new AGDSGraph(paramNode, itemNodeList);
        }


    }


}
