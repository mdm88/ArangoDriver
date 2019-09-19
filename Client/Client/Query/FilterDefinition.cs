using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using ArangoDriver.Expressions;

namespace ArangoDriver.Client
{
    public static class FilterBuilder<T>
    {   
        public static FilterDefinition Eq(string field, object value)
        {
            string expression = "FILTER " + field + " = @{0}";

            return new FilterDefinition(expression, new List<object>() {value});
        }
        public static FilterDefinition Eq<TV>(Expression<Func<T, TV>> field, TV value)
        {
            var expression = new FieldExpression<T, TV>(field);
            
            return Eq(expression.Name + "." + expression.Field, value);
        }
        
        public static FilterDefinition Gt(string field, object value)
        {
            string expression = "FILTER " + field + " > @{0}";

            return new FilterDefinition(expression, new List<object>() {value});
        }
        public static FilterDefinition Gt<TV>(Expression<Func<T, TV>> field, TV value)
        {
            var expression = new FieldExpression<T, TV>(field);
            
            return Gt(expression.Name + "." + expression.Field, value);
        }
        
        public static FilterDefinition Gte(string field, object value)
        {
            string expression = "FILTER " + field + " >= @{0}";

            return new FilterDefinition(expression, new List<object>() {value});
        }
        public static FilterDefinition Gte<TV>(Expression<Func<T, TV>> field, TV value)
        {
            var expression = new FieldExpression<T, TV>(field);
            
            return Gte(expression.Name + "." + expression.Field, value);
        }
        
        public static FilterDefinition Lt(string field, object value)
        {
            string expression = "FILTER " + field + " < @{0}";

            return new FilterDefinition(expression, new List<object>() {value});
        }
        public static FilterDefinition Lt<TV>(Expression<Func<T, TV>> field, TV value)
        {
            var expression = new FieldExpression<T, TV>(field);
            
            return Lt(expression.Name + "." + expression.Field, value);
        }
        
        public static FilterDefinition Lte(string field, object value)
        {
            string expression = "FILTER " + field + " <= @{0}";

            return new FilterDefinition(expression, new List<object>() {value});
        }
        public static FilterDefinition Lte<TV>(Expression<Func<T, TV>> field, TV value)
        {
            var expression = new FieldExpression<T, TV>(field);
            
            return Lte(expression.Name + "." + expression.Field, value);
        }

        public static FilterDefinition In<TV>(string field, IEnumerable<TV> values)
        {
            string expression = "FILTER " + field + " IN [";
            
            List<object> vars = new List<object>();
            int i = 0;
            foreach (object value in values)
            {
                expression += "@{" + i++ + "},";
                
                vars.Add(value);
            }
            
            expression = expression.Substring(0, expression.Length - 1) + "]";
            
            return new FilterDefinition(expression, vars);
        }
        public static FilterDefinition In<TV>(Expression<Func<T, TV>> field, IEnumerable<TV> values)
        {
            var expression = new FieldExpression<T, TV>(field);
            
            return In(expression.Name + "." + expression.Field, values);
        }
    }

    public class FilterDefinition
    {
        internal string Expression { get; }
        internal List<object> Values { get; }
        
        public FilterDefinition(string expression, List<object> values)
        {
            Expression = expression;
            Values = values;
        }
    }
}