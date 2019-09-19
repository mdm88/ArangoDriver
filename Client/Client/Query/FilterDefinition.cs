using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using ArangoDriver.Expressions;

namespace ArangoDriver.Client
{
    public static class FilterBuilder
    {
        private static int _vars = 0;
        
        public static FilterDefinition Equals(string field, object value)
        {
            string var = "var" + _vars++;
            string expression = "FILTER " + field + " = @" + var;

            return new FilterDefinition(expression, new Dictionary<string, object>() {{var, value}});
        }
        public static FilterDefinition Equals<T>(Expression<Func<T, object>> field, object value)
        {
            var expression = new FieldExpression<T>(field);
            
            return Equals(expression.Name + "." + expression.Field, value);
        }

        public static FilterDefinition In(string field, List<object> values)
        {
            string expression = "FILTER " + field + " IN [";
            
            Dictionary<string, object> vars = new Dictionary<string, object>();
            foreach (object value in values)
            {
                string var = "var" + _vars++;

                expression += "@" + var + ",";
                
                vars.Add(var, value);
            }
            
            expression = expression.Substring(0, expression.Length - 1) + "]";
            
            return new FilterDefinition(expression, vars);
        }
        public static FilterDefinition In<T>(string item, Expression<Func<T, object>> field, List<object> values)
        {
            var expression = new FieldExpression<T>(field);
            
            return In(expression.Name + "." + expression.Field, values);
        }
    }

    public class FilterDefinition
    {
        internal string Expression { get; }
        internal Dictionary<string, object> Values { get; }
        
        public FilterDefinition(string expression, Dictionary<string, object> values)
        {
            Expression = expression;
            Values = values;
        }
    }
}