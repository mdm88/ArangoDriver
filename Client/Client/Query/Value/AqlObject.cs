using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using ArangoDriver.Expressions;

namespace ArangoDriver.Client.Query.Value
{
    public class AqlObject<T> : IAqlValue<T>
    {
        private readonly Dictionary<string, IAqlValue> _dictionary;

        public AqlObject((Expression<Func<T, object>>, IAqlValue)[] d)
        {
            _dictionary = d.ToDictionary(
                x => new FieldExpression<T, object>(x.Item1).Field, 
                x => x.Item2
            );
        }
        public AqlObject((string, IAqlValue)[] d)
        {
            _dictionary = d.ToDictionary(
                x => x.Item1, 
                x => x.Item2
            );
        }

        public string GetExpression(ref int bindCount)
        {
            StringBuilder expression = new StringBuilder("{");
            
            foreach (var kvp in _dictionary)
            {
                expression.Append('"');
                expression.Append(kvp.Key);
                expression.Append("\":");
                expression.Append(kvp.Value.GetExpression(ref bindCount));
                expression.Append(',');
            }

            if (expression.Length > 1)
                expression.Remove(expression.Length - 1, 1);
            
            expression.Append('}');

            return expression.ToString();
        }

        public object[] GetBindedVars()
        {
            return _dictionary.SelectMany(x => x.Value.GetBindedVars()).ToArray();
        }
    }
}