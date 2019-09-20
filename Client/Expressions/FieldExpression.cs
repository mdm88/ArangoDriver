using System;
using System.Linq;
using System.Linq.Expressions;

namespace ArangoDriver.Expressions
{
    internal sealed class FieldExpression<T, TV> : FieldExpressionBase
    {
        public string Field => _field;

        public string Name => _name;
        
        public FieldExpression(Expression<Func<T, TV>> expression)
        {
            _name = expression.Parameters.First().Name;
            
            Parse(expression.Body);
        }
    }
}