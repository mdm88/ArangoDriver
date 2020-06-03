using System;
using System.Collections.Generic;
using System.Linq;
using ArangoDriver.Client.Query.Update;

namespace ArangoDriver.Client.Query.Query
{
    public class AqlUpdate<T> : IAqlQuery
    {
        private readonly string _alias;
        private readonly string _collectionName;
        private readonly UpdateBuilder<T>  _definition;
        private readonly Dictionary<string, object> _options;

        internal AqlUpdate(string alias, string collectionName, UpdateBuilder<T> definition, bool mergeObjects = true)
        {
            _alias = alias;
            _collectionName = collectionName;
            _definition = definition;
            _options = new Dictionary<string, object>();

            if (!mergeObjects)
                _options["mergeObjects"] = "false";
        }
        
        public string GetExpression(ref int bindCount)
        {
            return "UPDATE " + _alias + " WITH " + _definition.GetExpression(ref bindCount) + " IN " + _collectionName +
                (_options.Count > 0 ? " OPTIONS {" + string.Join(", ", _options.Select(kvp => kvp.Key + ":" + kvp.Value)) + "}" : "");
        }

        public object[] GetBindedVars()
        {
            return _definition.GetBindedVars();
        }
    }
}