using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace AGDSPresentationDB.AGDS
{
    public class DbDecompositor
    {
        #region Private members
        private const string BaseTableString = "BASE TABLE";
        private const string GetRelationsQuerry =
            @"SELECT fk.name 'FK Name', tp.name 'Parent table', cp.name, cp.column_id, tr.name 'Refrenced table', cr.name, cr.column_id FROM sys.foreign_keys fk INNER JOIN sys.tables tp ON fk.parent_object_id = tp.object_id INNER JOIN sys.tables tr ON fk.referenced_object_id = tr.object_id INNER JOIN sys.foreign_key_columns fkc ON fkc.constraint_object_id = fk.object_id INNER JOIN sys.columns cp ON fkc.parent_column_id = cp.column_id AND fkc.parent_object_id = cp.object_id INNER JOIN sys.columns cr ON fkc.referenced_column_id = cr.column_id AND fkc.referenced_object_id = cr.object_id ORDER BY tp.name, cp.column_id";

        private readonly Dictionary<string, DbTable> _tables = new Dictionary<string, DbTable>();
        private List<DbRelation> _relations;
        private readonly SqlConnection _connection;

        #endregion
        #region Properties

        public List<DbTable> Tables
        {
            get { return _tables.Values.ToList(); }
        }

        public IReadOnlyList<DbRelation> Relations
        {
            get { return _relations; }
        }
        #endregion
        #region Constructor
        public DbDecompositor(SqlConnection connection)
        {
            _connection = connection;
        }
        #endregion
        #region Private methods
        
        /// <summary>
        /// Get Table from db and create DbTable
        /// </summary>
        /// <param name="tableName"></param>
        /// <returns></returns>
        private DbTable GetTable(string tableName)
        {
            try
            {
                string querry = string.Format("SELECT * FROM {0}", tableName);
                DbTable table = new DbTable(tableName);
                SqlCommand command = new SqlCommand(querry, _connection);
                using (command)
                {
                    SqlDataReader reader = command.ExecuteReader();
                    using (reader)
                    {

                        List<string> columns = new List<string>();
                        DataTable datatable = reader.GetSchemaTable();
                        foreach (DataRow row in datatable.Rows)
                        {
                            string columnName = (string)row[0];
                            columns.Add(columnName);
                        }
                        //Assume that first column is primary Key
                        table.PrimaryKey = columns[0];
                        while (reader.Read())
                        {
                            foreach (string column in columns)
                            {
                                table.AddValue(column, reader[column]);
                            }
                        }
                    }
                }
                return table;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }

        }
        /// <summary>
        /// Get all declared relations in database
        /// </summary>
        /// <param name="connection"></param>
        /// <returns></returns>
        private List<DbRelation> GetRelations(SqlConnection connection)
        {
            List<DbRelation> relations = new List<DbRelation>();
            SqlCommand command = connection.CreateCommand();
            command.CommandText = GetRelationsQuerry;
            using (SqlDataReader reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    string parent = reader.GetString(1);
                    string refered = reader.GetString(4);
                    if (_tables.ContainsKey(parent) && _tables.ContainsKey(refered))
                    {
                        DbRelation relation = new DbRelation
                        {
                            PrimaryKey = reader.GetString(5),
                            ForeignKey = reader.GetString(2),
                            RelationTo = _tables[refered],
                            RelationFrom = _tables[parent]
                        };
                        relations.Add(relation);
                    }
                }
            }
            return relations;
        }

        private void FillKeys()
        {
            foreach (DbRelation relation in _relations)
            {
                if (_tables.ContainsKey(relation.RelationFrom.Name))
                {
                    DbTable relationFrom = _tables[relation.RelationFrom.Name];
                    relationFrom.AddRelation(relation.ForeignKey,relation.RelationTo);
                }
            }
        }
       
        #endregion
        #region Public methods
        /// <summary>
        /// Decomposes database and saves its basic tables
        /// </summary>
        public void Decompose()
        {
            using (_connection)
            {
                try
                {
                    _connection.Open();
                    var tables = _connection.GetSchema("Tables");
                    foreach (DataRow row in tables.Rows)
                    {
                        string tablename = (string)row[2];
                        string tabletype = (string)row[3];
                        if (tabletype == BaseTableString)
                        {
                            DbTable table = GetTable(tablename);
                            _tables.Add(table.Name, table);
                        }

                    }
                    _relations = GetRelations(_connection);
                    FillKeys();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(string.Format("Error while decomposing database:\n{0}", ex.Message));
                }

            }
        }
        public bool HasForgeinKey(string tableName)
        {
            return _relations.Any(item => item.RelationFrom.Name == tableName);
        }
        #endregion
    }

    public class DbRelation
    {
        private DbTable _relationFrom;
        private string _foreignKey;
        private DbTable _relationTo;
        private string _primaryKey;

        public DbTable RelationFrom
        {
            get { return _relationFrom; }
            set { _relationFrom = value; }
        }

        public DbTable RelationTo
        {
            get { return _relationTo; }
            set { _relationTo = value; }
        }

        public string PrimaryKey
        {
            get { return _primaryKey; }
            set { _primaryKey = value; }
        }

        public string ForeignKey
        {
            get { return _foreignKey; }
            set { _foreignKey = value; }
        }
    }

    public class DbTable
    {
        private readonly Dictionary<string, List<object>> _tableDictionary = new Dictionary<string, List<object>>();
        //Keeps related tables by foreign key
        private readonly Dictionary<object, DbTable> _relatedDbTables = new Dictionary<object, DbTable>();

        private string _name;
        private string _primaryKey;

        public string PrimaryKey
        {
            get { return _primaryKey; }
            set { _primaryKey = value; }
        }

        public IReadOnlyDictionary<object, DbTable> RealtedTables
        {
            get { return _relatedDbTables; }
        }

        public IReadOnlyDictionary<string, List<object>> TableDictionary
        {
            get { return _tableDictionary; }
        } 

        public string Name
        {
            get { return _name; }
        }

        public DbTable(string name)
        {
            _name = name;
        }

        public void AddValue(string columnName, object value)
        {
            Type type = value.GetType();
            if (!_tableDictionary.ContainsKey(columnName))
            {
                _tableDictionary.Add(columnName, new List<object>());
            }
            _tableDictionary[columnName].Add(value);
        }

        public List<object> GetObjects(string columnName)
        {
            if (_tableDictionary.ContainsKey(columnName))
            {
                return _tableDictionary[columnName];
            }
            return new List<object>();
        }

        public void AddRelation(object forgeinKey, DbTable table)
        {
            if (!_relatedDbTables.ContainsKey(forgeinKey))
            {
                _relatedDbTables.Add(forgeinKey, table);
            }
        }

        public Dictionary<string, object> GetRow(object key)
        {
            Dictionary<string,object> row = new Dictionary<string, object>();
            int index = 0;
            foreach (object pk in _tableDictionary[PrimaryKey])
            {
                if (pk.Equals(key))
                {
                    break;
                }
                index++;
            }
            if (index != int.MinValue)
            {
                foreach (KeyValuePair<string, List<object>> pair in _tableDictionary)
                {
                    row.Add(pair.Key, pair.Value[index]);
                }
            }
            return row;
        }

        
    }
}
