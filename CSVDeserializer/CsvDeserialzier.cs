﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace CSVDeserializer
{

    /// <summary>
    /// Deserializes and serializes List of objects to CSV file. 
    /// Serializable object needs to have default constructor
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class CSVSerializer<T> where T : class, new()
    {
        #region Private members
        private char _separator;
        private List<PropertyInfo> _properties;
        private string _replacement;
        #endregion

        #region Properties

        public char Separator
        {
            set { _separator = value; }
        }
        public string Replacement
        {
            set { _replacement = value; }
        }

        #endregion

        public void Serialize(Stream stream, IList<T> data)
        {
            StringBuilder sb = new StringBuilder();
            List<string> values = new List<string>();

            sb.AppendLine(GetHeader());        
            foreach (var item in data)
            {
                values.Clear();

                foreach (var p in _properties)
                {
                    var raw = p.GetValue(item);
                    var value = raw == null ?
                                "" :
                                raw.ToString().Replace(_separator.ToString(), _replacement);
                    values.Add(value);
                }
                sb.AppendLine(string.Join(_separator.ToString(), values.ToArray()));
            }

            using (var sw = new StreamWriter(stream))
            {
                sw.Write(sb.ToString().Trim());
            }
        }
        public IList<T> Deserialize(Stream stream)
        {
            string[] columns;
            string[] rows;
            try
            {
                using (var sr = new StreamReader(stream))
                {
                    columns = sr.ReadLine().Split(_separator);
                    rows = sr.ReadToEnd().Split(new string[] { Environment.NewLine }, StringSplitOptions.None);
                }
            }
            catch (Exception ex)
            {
                throw new InvalidDataException(
                        "The CSV File is Invalid. See Inner Exception for more inoformation.", ex);
            }
            var data = new List<T>();
            for (int row = 0; row < rows.Length; row++)
            {
                var line = rows[row];
                if (string.IsNullOrWhiteSpace(line))
                {                    
                    break;
                }
                var parts = line.Split(_separator);

                var datum = new T();
                for (int i = 0; i < parts.Length; i++)
                {
                    var value = parts[i];
                    var column = columns[i];

                    value = value.Replace(_replacement, _separator.ToString());

                    var p = _properties.First(a => a.Name == column);

                    var converter = TypeDescriptor.GetConverter(p.PropertyType);
                    var convertedvalue = converter.ConvertFrom(value);

                    p.SetValue(datum, convertedvalue);
                }
                data.Add(datum);
            }
            return data;
        }
        public CSVSerializer()
        {
            _separator = ',';
            _replacement = " ";

            Type type = typeof(T);

            PropertyInfo[] properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance
                 | BindingFlags.GetProperty | BindingFlags.SetProperty);

            _properties = (from a in properties
                           where a.GetCustomAttribute<CsvIgnoreAttribute>() == null
                           orderby a.Name
                           select a).ToList();
        }
        private string GetHeader()
        {
            var columns = _properties.Select(a => a.Name).ToArray();
            var header = string.Join(_separator.ToString(), columns);
            return header;
        }

    }
    public class CsvIgnoreAttribute : Attribute { }
}
