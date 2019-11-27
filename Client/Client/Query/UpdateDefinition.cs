using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using ArangoDriver.Client.Query.Value;
using ArangoDriver.Expressions;

namespace ArangoDriver.Client
{
    public static class UpdateBuilder<T>
    {
        public static UpdateDefinition<T> Update(string alias, string collectionName)
        {
            return new UpdateDefinition<T>(alias, collectionName);
        }
    }
    public class UpdateDefinition<T>
    {
        private int _vars = 0;

        private readonly string _alias;
        private readonly string _collectionName;
        private readonly List<string> _updates;

        internal List<object> Values { get; }
        internal Dictionary<string, object> Options { get; }

        public string Expression =>
            !IsEmpty()
                ? "UPDATE " + _alias + " WITH { " + string.Join(", ", _updates) + " } IN " + _collectionName +
                  (Options.Count > 0 ? " OPTIONS {" + string.Join(", ", Options.Select(kvp => kvp.Key + ":" + kvp.Value)) + "}" : "")
                : "";

        public UpdateDefinition(string alias, string collectionName)
        {
            _alias = alias;
            _collectionName = collectionName;
            _updates = new List<string>();
            Values = new List<object>();
            Options = new Dictionary<string, object>();
        }

        public UpdateDefinition<T> Set(string field, object value)
        {
            _updates.Add(field + ": @{" + _vars++ +"}");

            Values.Add(value);

            return this;
        }
        public UpdateDefinition<T> Set<TV>(Expression<Func<T, TV>> field, TV value)
        {
            var expression = new FieldExpression<T, TV>(field);
            
            return Set(expression.Field, value);
        }
        public UpdateDefinition<T> Set<TV>(Expression<Func<T, TV>> field, IAqlValue<TV> value)
        {
            var expression = new FieldExpression<T, TV>(field);

            _updates.Add(expression.Field + ": " + value.GetExpression(ref _vars));

            Values.Add(value.GetBindedVars());

            return this;
        }

        public UpdateDefinition<T> Inc(string alias, string field, double value)
        {
            _updates.Add(field + ": " + alias + "." + field + value.ToString("+0;-#"));

            return this;
        }
        public UpdateDefinition<T> Inc(Expression<Func<T, double>> field, double value)
        {
            var expression = new FieldExpression<T, double>(field);
            
            return Inc(expression.Name, expression.Field, value);
        }

        public UpdateDefinition<T> OptMergeObjects(bool mergeObjects)
        {
            Options["mergeObjects"] = mergeObjects.ToString().ToLower();

            return this;
        }

        public bool IsEmpty()
        {
            return !_updates.Any();
        }
    }
}