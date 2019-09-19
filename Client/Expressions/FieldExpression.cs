using System;
using System.Linq;
using System.Linq.Expressions;
using Newtonsoft.Json;

namespace ArangoDriver.Expressions
{
    internal class FieldExpression<T, TV>
    {
        public string Field { get; }
        public string Name { get; }
        
        public FieldExpression(Expression<Func<T, TV>> expression)
        {
            Field = "";
            Name = expression.Parameters.First().Name;
            
            MemberExpression e = expression.Body as MemberExpression ?? (expression.Body as UnaryExpression)?.Operand as MemberExpression;
            while (e != null)
            {
                JsonPropertyAttribute attr = e.Member.GetCustomAttributes(typeof(JsonPropertyAttribute), true).FirstOrDefault() as JsonPropertyAttribute;
                
                Field = (attr?.PropertyName ?? e.Member.Name) + (!String.IsNullOrEmpty(Field) ? "." + Field : "");
                
                e = e.Expression as MemberExpression;
            }
        }
    }
}