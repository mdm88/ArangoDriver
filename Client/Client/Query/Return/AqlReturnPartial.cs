using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using ArangoDriver.Expressions;

namespace ArangoDriver.Client.Query.Return
{
    public class AqlReturnPartial<T> : IAqlReturn
    {
        private readonly List<FieldExpression<T, object>> _returns;
        
        internal AqlReturnPartial()
        {
            _returns = new List<FieldExpression<T, object>>();
        }

        internal AqlReturnPartial(params Expression<Func<T, object>>[] fields) : this()
        {
            Add(fields);
        }

        internal AqlReturnPartial(params MemberExpression[] fields) : this()
        {
            Add(fields);
        }
        
        public AqlReturnPartial<T> Add(params Expression<Func<T, object>>[] fields)
        {
            _returns.AddRange(fields.Select(e => new FieldExpression<T, object>(e)));
            
            return this;
        }
        
        public AqlReturnPartial<T> Add(params MemberExpression[] fields)
        {
            _returns.AddRange(fields.Select(e => new FieldExpression<T, object>(e)));
            
            return this;
        }

        public string GetExpression()
        {
            return "{" + String.Join(", ", _returns.Select(e => e.Field + ":" + e.Name + "." + e.Field)) + "}";
        }
    }
}