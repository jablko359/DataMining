using System;
using System.Collections.Generic;

namespace AGDSPresentationDB.AGDS
{
    public class AgdsBuilder
    {
        private DbDecompositor _decompositor;
        //private Dictionary<string, List<Node>> _neuronsDictionary = new Dictionary<string, List<Node>>();
        private Dictionary<string, Dictionary<object, Node>> _neuronsDictionary = new Dictionary<string, Dictionary<object, Node>>();
        private List<Node> _allNodes = new List<Node>();

        public AgdsBuilder(DbDecompositor decompositor)
        {
            _decompositor = decompositor;
        }

        public AGDSGraph BuildGraph()
        {
            _decompositor.Decompose();
            List<Node> tableNodes = new List<Node>();
            foreach (DbTable table in _decompositor.Tables)
            {
                if(table.Name != "sysdiagrams")
                    tableNodes.AddRange(CreateGraphForTable(table));
            }
            SetRelations();
            foreach (Node allNode in _allNodes)
            {
                
            }
            return new AGDSGraph(tableNodes, _allNodes);
        }

        private List<Node> CreateGraphForTable(DbTable table)
        {
            List<Node> tableNodes = new List<Node>();
            Dictionary<object, Node> primaryNodes = new Dictionary<object, Node>();
            List<object> primaryKeys = table.TableDictionary[table.PrimaryKey];
            foreach (KeyValuePair<string, List<object>> column in table.TableDictionary)
            {
                if (column.Key != table.PrimaryKey && !table.RealtedTables.ContainsKey(column.Key))
                {
                    Node columnNode = new Node(column.Key);
                    _allNodes.Add(columnNode);
                    Dictionary<object, Node> valueNodes = new Dictionary<object, Node>();
                    for (int i = 0; i < column.Value.Count; i++)
                    {
                        Node valueNode;
                        if (!valueNodes.ContainsKey(column.Value[i]))
                        {
                            valueNode = new HierarchicalNode(column.Value[i], columnNode);
                            _allNodes.Add(valueNode);
                            valueNodes.Add(column.Value[i], valueNode);
                            columnNode.AddItem(valueNode);
                        }
                        else
                        {
                            valueNode = valueNodes[column.Value[i]];
                        }
                        object pkValue = primaryKeys[i];

                        Node itemNode;
                        if (!primaryNodes.ContainsKey(pkValue))
                        {
                            DbPrimaryKey primaryKey = new DbPrimaryKey(pkValue, table.Name);
                            itemNode = new Node(primaryKey);
                            _allNodes.Add(itemNode);
                            primaryNodes.Add(pkValue, itemNode);
                        }
                        else
                        {
                            itemNode = primaryNodes[pkValue];
                        }
                        itemNode.AddItem(valueNode);
                        valueNode.AddItem(itemNode);

                    }
                    tableNodes.Add(columnNode);
                }
            }
            _neuronsDictionary.Add(table.Name, primaryNodes);
            return tableNodes;
        }

        private void SetRelations()
        {
            foreach (DbRelation relation in _decompositor.Relations)
            {
                if (_neuronsDictionary.ContainsKey(relation.RelationFrom.Name) &&
                    _neuronsDictionary.ContainsKey(relation.RelationTo.Name))
                {
                    var relationFrom = _neuronsDictionary[relation.RelationFrom.Name];
                    var relationTo = _neuronsDictionary[relation.RelationTo.Name];
                    foreach (KeyValuePair<object, Node> pair in relationFrom)
                    {
                        var row = relation.RelationFrom.GetRow(pair.Key);
                        object fk = row[relation.ForeignKey];
                        if (fk.GetType() != typeof(DBNull))
                        {
                            Node fNode = relationTo[fk];
                            fNode.AddItem(pair.Value);
                            pair.Value.AddItem(fNode);
                        }
                    }
                }
            }
        }
    }

    public class DbPrimaryKey
    {
        private readonly string _tableName;
        private readonly object _value;

        public string TableName
        {
            get { return _tableName; }
        }

        public object Key
        {
            get { return _tableName; }
        }

        public DbPrimaryKey(object value, string tableName)
        {
            _value = value;
            _tableName = tableName;
        }

        public override string ToString()
        {
            return string.Format("PK - {0} : {1}", _tableName, _value);
        }

        public override bool Equals(object obj)
        {
            return _value.Equals(obj);
        }

        public override int GetHashCode()
        {
            return _value.GetHashCode();
        }
    }
}
