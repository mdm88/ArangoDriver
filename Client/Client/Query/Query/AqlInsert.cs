using System;
using System.Collections.Generic;
using System.Linq;
using ArangoDriver.Client.Query.Update;
using ArangoDriver.Client.Query.Value;

namespace ArangoDriver.Client.Query.Query
{
    public class AqlInsert : IAqlQuery
    {
        private readonly string _collectionName;
        private readonly IAqlValue _value;
        private readonly Dictionary<string, object> _options;

        internal AqlInsert(string collectionName, IAqlValue value)
        {
            _collectionName = collectionName;
            _value = value;
            _options = new Dictionary<string, object>();
        }
        
        public string GetExpression(ref int bindCount)
        {
            return "INSERT " + _value.GetExpression(ref bindCount) + " INTO " + _collectionName +
                (_options.Count > 0 ? " OPTIONS {" + string.Join(", ", _options.Select(kvp => kvp.Key + ":" + kvp.Value)) + "}" : "");
        }

        public object[] GetBindedVars()
        {
            return _value.GetBindedVars();
        }
    }
}