using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using AGDSPresentationDB.Annotations;

namespace AGDSPresentationDB.Tools
{
    public class QueryParser
    {
        private readonly Dictionary<string, Query> _querries = new Dictionary<string, Query>();
        HashSet<string> _columns = new HashSet<string>();
        private readonly List<string> _selectors = new List<string>();
        private readonly string _querry;
        private readonly Regex _splitPattern = new Regex("=|<=|>=|<|>");


        public IReadOnlyDictionary<string, Query> Querries
        {
            get { return _querries; }
        }

        public IReadOnlyList<string> Selectors
        {
            get { return _selectors; }
        }


        public QueryParser([NotNull]string querry)
        {
            _querry = querry;
        }

        public bool ParseQuerry()
        {
            string[] args = _querry.Split(';');
            foreach (string arg in args)
            {
                string[] splitedArg = _splitPattern.Split(arg);
                string operatorType = _splitPattern.Match(arg).Value;
                if (splitedArg.Length == 2 && !string.IsNullOrEmpty(operatorType))
                {
                    CompareOperator compareType = GetCompareType(operatorType);
                    _columns.Add(splitedArg[0]);
                    string argument = splitedArg[1].Trim();
                    object objectArg;
                    if (argument[0] == '\'' && argument[argument.Length - 1] == '\'')
                    {
                        objectArg = argument.Substring(1, argument.Length - 2);
                    }
                    else
                    {
                        objectArg = GetValue(argument);
                    }
                    Query query = new Query(splitedArg[0], objectArg, compareType);
                    _querries.Add(splitedArg[0].Trim(), query);
                }
                else
                {
                    return false;
                }
            }
            return true;
        }

        private static CompareOperator GetCompareType(string stringOpertor)
        {
            stringOpertor = stringOpertor.Trim();
            CompareOperator compareType;
            if (stringOpertor == "=")
            {
                compareType = CompareOperator.Equals;
            }
            else if (stringOpertor == "<")
            {
                compareType = CompareOperator.LessThan;
            }
            else if (stringOpertor == ">")
            {
                compareType = CompareOperator.GreaterThan;
            }
            else if (stringOpertor == "<=")
            {
                compareType = CompareOperator.LessEqualThan;
            }
            else if (stringOpertor == ">=")
            {
                compareType = CompareOperator.GreaterEqualsThan;
            }
            else
            {
                throw new ArgumentException("Not supported operator");
            }
            return compareType;
        }

        private static object GetValue(string value)
        {
            int intValue;
            long longValue;
            double doubleValue;
            DateTime dateTimeValue;
            if (Int32.TryParse(value, out intValue))
            {
                return intValue;
            }
            if (Int64.TryParse(value, out longValue))
            {
                return longValue;
            }
            if (Double.TryParse(value, out doubleValue))
            {
                return doubleValue;
            }
            if (DateTime.TryParse(value, out dateTimeValue))
            {
                return dateTimeValue;
            }
            else
            {
                throw new ArgumentException("Not known type");
            }

        }
    }

    public class Query
    {
        private readonly CompareOperator _compare;
        private readonly object _value;
        private readonly string _queryName;

        public CompareOperator CompareType
        {
            get { return _compare; }
        }

        public string QueryName
        {
            get { return _queryName; }
        }

        public object Value
        {
            get { return _value; }
        }

        public Query(string columnName, object value, CompareOperator compare)
        {
            _value = value;
            _compare = compare;
            _queryName = columnName;
        }

    }

    public enum CompareOperator
    {
        Equals, LessThan, GreaterThan, LessEqualThan, GreaterEqualsThan
    }
}
