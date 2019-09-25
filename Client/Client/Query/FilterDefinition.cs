using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using ArangoDriver.Expressions;

namespace ArangoDriver.Client
{
    public static class FilterBuilder
    {
        public static FilterDefinition Or(params FilterDefinition[] filters)
        {
            string expression = "FILTER ";
            List<object> values = new List<object>();

            int i = 0;
            foreach (FilterDefinition filter in filters)
            {
                string exp = filter.Expression.Replace("FILTER ", "");

                for (int j = 0; j < filter.Values.Count; j++)
                {
                    exp = exp.Replace("@{" + j + "}", "@{" + i++ + "}");
                }

                expression += exp + " OR ";
                values.AddRange(filter.Values);
            }

            expression = expression.Remove(expression.Length - 4);
            
            return new FilterDefinition(expression, values);
        }
        
        public static FilterDefinition Basic(string field, string operation, object value)
        {
            string expression = "FILTER " + field + " " + operation + " @{0}";

            return new FilterDefinition(expression, new List<object>() {value});
        }
        
        public static FilterDefinition Eq(string field, object value)
        {
            return Basic(field, "==", value);
        }
        public static FilterDefinition Neq(string field, object value)
        {
            return Basic(field, "!=", value);
        }
        public static FilterDefinition Gt(string field, object value)
        {
            return Basic(field, ">", value);
        }
        public static FilterDefinition Gte(string field, object value)
        {
            return Basic(field, ">=", value);
        }
        public static FilterDefinition Lt(string field, object value)
        {
            return Basic(field, "<", value);
        }
        public static FilterDefinition Lte(string field, object value)
        {
            return Basic(field, ">=", value);
        }
        
        public static FilterDefinition In<TV>(string field, IEnumerable<TV> values)
        {
            string expression = "FILTER " + field + " IN [";
            
            List<object> list = new List<object>();
            int i = 0;
            foreach (object value in values)
            {
                expression += "@{" + i++ + "},";
                
                list.Add(value);
            }
            
            expression = expression.Substring(0, expression.Length - 1) + "]";

            return new FilterDefinition(expression, list);
        }
    }
    
    public static class FilterBuilder<T>
    {
        public static FilterDefinition Or(params FilterDefinition[] filters)
        {
            return FilterBuilder.Or(filters);
        }
        
        public static FilterDefinition Basic<TV>(Expression<Func<T, TV>> field, string operation, TV value)
        {
            var expression = new FilterFieldExpression<T, TV>(field);

            object parsedValue = value;
            if (expression.Special == FilterFieldExpression<T, TV>.Specials.Type && value is Type valueType)
            {
                parsedValue = valueType.FullName + ", "  + valueType.Assembly.GetName().Name;
            }
            
            return FilterBuilder.Basic(expression.Field, operation, parsedValue);
        }
        
        public static FilterDefinition Eq<TV>(Expression<Func<T, TV>> field, TV value)
        {
            return Basic(field, "==", value);
        }
        public static FilterDefinition Neq<TV>(Expression<Func<T, TV>> field, TV value)
        {
            return Basic(field, "!=", value);
        }
        public static FilterDefinition Gt<TV>(Expression<Func<T, TV>> field, TV value)
        {
            return Basic(field, ">", value);
        }
        public static FilterDefinition Gte<TV>(Expression<Func<T, TV>> field, TV value)
        {
            return Basic(field, ">=", value);
        }
        public static FilterDefinition Lt<TV>(Expression<Func<T, TV>> field, TV value)
        {
            return Basic(field, "<", value);
        }
        public static FilterDefinition Lte<TV>(Expression<Func<T, TV>> field, TV value)
        {
            return Basic(field, ">=", value);
        }

        public static FilterDefinition In<TV>(Expression<Func<T, TV>> field, IEnumerable<TV> values)
        {
            var expression = new FilterFieldExpression<T, TV>(field);

            IEnumerable<object> parsedValues;
            if (expression.Special == FilterFieldExpression<T, TV>.Specials.Type)
            {
                parsedValues = values.Cast<Type>().Select(t => t.FullName + ", " + t.Assembly.GetName().Name);
            }
            else
            {
                parsedValues = values.Cast<object>();
            }
            
            return FilterBuilder.In(expression.Field, parsedValues);
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