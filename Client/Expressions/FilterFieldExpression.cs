using System;
using System.Linq;
using System.Linq.Expressions;
using ArangoDriver.Exceptions;

namespace ArangoDriver.Expressions
{
    internal sealed class FilterFieldExpression<T, TV> : FieldExpressionBase
    {
        public string Field
        {
            get
            {
                string field = _name + (!String.IsNullOrEmpty(_field) ? "." + _field : "");
                
                switch (Special)
                {
                    case Specials.Count:
                        return "COUNT(" + field + ")";
                    case Specials.Type:
                        return field + ".$type";
                    default:
                        return field;
                }
            }
        }

        public Specials Special { get; private set; }

        public FilterFieldExpression(Expression<Func<T, TV>> expression)
        {
            _name = expression.Parameters.First().Name;
            
            Parse(expression.Body);
        }

        protected override void Parse(Expression expression)
        {
            if (expression is MethodCallExpression methodCallExpression)
            {
                switch (methodCallExpression.Method.Name)
                {
                    case "Count":
                        Special = Specials.Count;
                        break;
                    case "GetType":
                        Special = Specials.Type;
                        break;
                    default:
                        throw new ExpressionInvalidException();
                }
                
                expression = methodCallExpression.Arguments.FirstOrDefault();

                if (expression == null)
                    return;
            }
            
            base.Parse(expression);
        }

        internal enum Specials
        {
            None,
            Count,
            Type
        } 
    }
}