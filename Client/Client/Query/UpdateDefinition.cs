using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using ArangoDriver.Expressions;

namespace ArangoDriver.Client
{
    public static class UpdateBuilder
    {
        public static UpdateDefinition<T> Update<T>(string alias, string collectionName)
        {
            return new UpdateDefinition<T>(alias, collectionName);
        }
    }
    public class UpdateDefinition<T>
    {
        private static int _vars = 0;

        private readonly string _alias;
        private readonly string _collectionName;
        private readonly List<string> _updates;

        internal Dictionary<string, object> Values { get; }

        public string Expression =>
            !IsEmpty() ? "UPDATE " + _alias + " WITH { " + string.Join(", ", _updates) + " } IN " + _collectionName : "";

        public UpdateDefinition(string alias, string collectionName)
        {
            _alias = alias;
            _collectionName = collectionName;
            _updates = new List<string>();
            Values = new Dictionary<string,object>();
        }

        public UpdateDefinition<T> Set(string field, object value)
        {
            string var = "set" + _vars++;
            _updates.Add(field + ": @" + var);

            Values.Add(var, value);

            return this;
        }
        public UpdateDefinition<T> Set(Expression<Func<T, object>> field, object value)
        {
            var expression = new FieldExpression<T>(field);
            
            return Set(expression.Field, value);
        }

        public bool IsEmpty()
        {
            return !_updates.Any();
        }
    }
}