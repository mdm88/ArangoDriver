using System;
using System.Linq;
using System.Linq.Expressions;
using ArangoDriver.Exceptions;
using Newtonsoft.Json;

namespace ArangoDriver.Expressions
{
    internal abstract class FieldExpressionBase
    {
        protected string _field;
        protected string _name;
        
        protected virtual void Parse(Expression expression)
        {
            _field = "";
            
            MemberExpression e = expression as MemberExpression ?? (expression as UnaryExpression)?.Operand as MemberExpression;
            if (e == null)
                throw new ExpressionInvalidException();
            while (e != null)
            {
                JsonPropertyAttribute attr = e.Member.GetCustomAttributes(typeof(JsonPropertyAttribute), true).FirstOrDefault() as JsonPropertyAttribute;
                
                _field = (attr?.PropertyName ?? e.Member.Name) + (!String.IsNullOrEmpty(_field) ? "." + _field : "");
                
                e = e.Expression as MemberExpression;
            }
        }
    }
}