using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AGDSPresentationDB.Tools;
using Antlr4.Runtime;
using Antlr4.Runtime.Tree;

namespace AGDSPresentationDB.Parser
{
    public class InputQueryInterpreter : IQueryListener
    {
        private Query _topQuery;
        private readonly Dictionary<QueryParser.QueryContext, Query> _queries = new Dictionary<QueryParser.QueryContext, Query>();

        public Query TopQuery
        {
            get { return _topQuery; }
        }

        public void VisitTerminal(ITerminalNode node)
        {
            //   throw new NotImplementedException();
        }

        public void VisitErrorNode(IErrorNode node)
        {
                throw new NotImplementedException();
        }

        public void EnterEveryRule(ParserRuleContext ctx)
        {
            //     throw new NotImplementedException();
        }

        public void ExitEveryRule(ParserRuleContext ctx)
        {
            //    throw new NotImplementedException();
        }

        public void EnterExpression(QueryParser.ExpressionContext context)
        {
            //     throw new NotImplementedException();
        }

        public void ExitExpression(QueryParser.ExpressionContext context)
        {
            //    throw new NotImplementedException();
        }

        public void EnterQuery(QueryParser.QueryContext context)
        {
            try
            {
                Query createdQuery;
                if (context.expression() != null)
                {
                    QueryParser.ExpressionContext expressionContext = context.expression();
                    string columnName = expressionContext.STRING().ToString();
                    CompareOperator op = QueryHelper.GetCompareType(expressionContext.OPERATOR().GetText());
                    object value;
                    if (expressionContext.QUOTEDSTRING() != null)
                    {
                        value = expressionContext.QUOTEDSTRING().GetText().Trim(new char[] {'\''});
                    }
                    else
                    {
                        value = QueryHelper.GetValue(expressionContext.NUMBER().GetText());
                    }
                    Expression expression = new Expression(columnName, value, op);
                    createdQuery = new Query(expression);
                }
                else if (context.LOGIC() != null)
                {
                    QueryLogic logic = QueryHelper.GetLogic(context.LOGIC().GetText());
                    createdQuery = new Query(logic);
                }
                else
                {
                    return;
                }
                QueryParser.QueryContext parentContext = context.Parent as QueryParser.QueryContext;
                if (parentContext != null)
                {
                    Query parentQuery;
                    if (!_queries.TryGetValue(parentContext, out parentQuery))
                    {
                        parentContext = parentContext.Parent as QueryParser.QueryContext;
                        if (parentContext != null)
                        {
                            if (!_queries.TryGetValue(parentContext, out parentQuery))
                            {
                                return;
                            }
                        }
                    }
                    if (parentContext != null && parentContext.query(0) == context)
                    {
                        parentQuery.LeftQuery = createdQuery;
                    }
                    else if (parentContext != null && parentContext.query(1) == context)
                    {
                        parentQuery.RightQuery = createdQuery;
                    }
                    else if(parentContext != null)
                    {
                        if (parentContext.query(0).query(1) == context)
                        {
                            parentQuery.LeftQuery = createdQuery;
                        }
                        else if (parentContext.query(0).query(0) == context)
                        {
                            parentQuery.LeftQuery = createdQuery;
                        }
                        else if (parentContext.query(1).query(0) == context)
                        {
                            parentQuery.RightQuery = createdQuery;
                        }
                        else if (parentContext.query(1).query(1) == context)
                        {
                            parentQuery.RightQuery = createdQuery;
                        }
                    }
                }
                if (_topQuery == null)
                {
                    _topQuery = createdQuery;
                }
                _queries.Add(context, createdQuery);
            }
            catch (Exception ex)
            {
                throw new NotSupportedException(string.Format("Querry not supported: {0}", context.GetText()), ex);
            }
            
        }

        public void ExitQuery(QueryParser.QueryContext context)
        {
            //   throw new NotImplementedException();
        }
    }

    #region Containers and helper classes
    public class Expression
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

        public Expression(string columnName, object value, CompareOperator compare)
        {
            _value = value;
            _compare = compare;
            _queryName = columnName;
        }
    }

    public class Query
    {
        private Query _leftQuery;
        private QueryLogic _logic;
        private Query _rightQuery;
        private Expression _expression;

        public Query LeftQuery
        {
            get { return _leftQuery; }
            set { _leftQuery = value; }
        }

        public Query RightQuery
        {
            get { return _rightQuery; }
            set { _rightQuery = value; }
        }

        public Expression Expression
        {
            get { return _expression; }
        }

        public QueryLogic Logic
        {
            get { return _logic; }
        }

        public Query(QueryLogic logic)
        {
            _logic = logic;
        }

        public Query(Expression expression)
        {
            _expression = expression;
        }
    }

    public static class QueryHelper
    {
        public static CompareOperator GetCompareType(string stringOpertor)
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

        public static object GetValue(string value)
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
            if (double.TryParse(value, NumberStyles.Number, CultureInfo.CreateSpecificCulture("en-US"), out doubleValue))
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

        public static QueryLogic GetLogic(string logic)
        {
            StringBuilder sb = new StringBuilder(logic);
            sb[0] = Char.ToUpper(sb[0]);
            logic = sb.ToString();
            QueryLogic resultLogic;
            if (Enum.TryParse(logic, out resultLogic))
            {
                return resultLogic;
            }
            throw new ArgumentException(string.Format("Not known logic: {0}", logic));
        }
    }
    #region Enums

    public enum QueryLogic
    {
        And, Or
    }

    public enum CompareOperator
    {
        Equals, LessThan, GreaterThan, LessEqualThan, GreaterEqualsThan
    }
    #endregion
    #endregion

}
