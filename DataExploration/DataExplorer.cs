using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataExploration
{
    public class DataExplorer
    {
        private readonly List<IExplorable> _dataList;

        public DataExplorer(List<IExplorable> datalist)
        {
            _dataList = datalist;
        }

        private long GetElementCount(string elementName)
        {
            long count = 0;
            foreach (IExplorable explorable in _dataList)
            {
                count += explorable.Elemnets.Count(value => value == elementName);
            }
            return count;
        }

        public double GetSupport(string elementName)
        {
            return GetElementCount(elementName) / (double)_dataList.Count;
        }

        public Dictionary<string, double> GetFrequent()
        {
            Dictionary<string, double> frequentDictionary = new Dictionary<string, double>();
            foreach (IExplorable explorable in _dataList)
            {
                foreach (string dataName in explorable.Elemnets)
                {
                    if (!frequentDictionary.ContainsKey(dataName))
                    {
                        double frequency = GetSupport(dataName);
                        frequentDictionary.Add(dataName, frequency);
                    }
                }
            }
            return frequentDictionary;
        }
        public Dictionary<string, double> GetMostFrequent(double threshold)
        {
            return GetFrequent().Where(value => value.Value > threshold).ToDictionary(val => val.Key, value => value.Value);
        }

        public static string PrintDictionary<T1, T2>(Dictionary<T1, T2> dictToPrint)
        {
            StringBuilder sb = new StringBuilder();
            foreach (KeyValuePair<T1, T2> dictValue in dictToPrint)
            {
                sb.AppendLine(string.Format("{0} : {1}", dictValue.Key, dictValue.Value));
            }
            return sb.ToString();
        }
    }
}
