﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace SOM
{
    public class InputVector
    {
        private Dictionary<string, double> _attributes = new Dictionary<string, double>();
        private List<double> _values = new List<double>();

        public IReadOnlyDictionary<string, double> Attributes
        {
            get { return _attributes; }
        }

        public IReadOnlyList<double> Values
        {
            get { return _values; }
        } 

        public InputVector(IList<string> atributeNames, IList<double> values)
        {
            if (atributeNames.Count != values.Count)
            {
                throw new Exception("Atribute names and values count must be the smae");
            }
            for (int i = 0; i < atributeNames.Count; i++)
            {
                _values.Add(values[i]);
                _attributes.Add(atributeNames[i],values[i]);
            }
        }
    }

    public class InputDataSetDeserializator
    {
        private string _filePath;
        public int AtributesCount { get; private set; }
        public IList<string> AttributesList { get; private set; }

        public InputDataSetDeserializator(string fielPath)
        {
            _filePath = fielPath;
            AtributesCount = 0;
        }

        public List<InputVector> Deserialize()
        {
            StreamReader reader;
            List<InputVector> vectors = new List<InputVector>();
            using (reader = new StreamReader(_filePath))
            {
                try
                {
                    string atr = reader.ReadLine();
                    string[] atributes = atr.Split();
                    AttributesList = atributes;
                    AtributesCount = atributes.Length;
                    string line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        string[] values = line.Split();
                        List<double> doubleValues = new List<double>();
                        foreach (string value in values)
                        {
                            double val;
                            if (Double.TryParse(value, out val))
                            {
                                doubleValues.Add(val);
                            }
                            else
                            {
                                throw new InvalidDataException("Only double attributes are supported");
                            }
                        }
                        if (atributes.Length != doubleValues.Count)
                        {
                            throw new ArgumentOutOfRangeException("Atribute not defined");
                        }
                        InputVector vector = new InputVector(atributes, doubleValues);
                        vectors.Add(vector);
                    }
                    return vectors;
                }
                catch (Exception ex)
                {
                    throw new InvalidDataException("Wrong file format", ex);
                }

            }
        }

        

    }
}
