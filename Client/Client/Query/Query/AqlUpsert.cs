using System;
using System.Collections.Generic;
using System.Linq;
using ArangoDriver.Client.Query.Update;
using ArangoDriver.Client.Query.Value;

namespace ArangoDriver.Client.Query.Query
{
    public class AqlUpsert<T> : IAqlQuery
    {
        private readonly string _collectionName;
        private readonly IAqlValue _search;
        private readonly IAqlValue _insert;
        private readonly UpdateBuilder<T>  _update;
        private readonly Dictionary<string, object> _options;

        internal AqlUpsert(string collectionName, IAqlValue search, IAqlValue insert, UpdateBuilder<T> update)
        {
            _collectionName = collectionName;
            _search = search;
            _insert = insert;
            _update = update;
            _options = new Dictionary<string, object>();
        }
        
        public string GetExpression(ref int bindCount)
        {
            return "UPSERT " + _search.GetExpression(ref bindCount) + 
                   " INSERT " + _insert.GetExpression(ref bindCount) + 
                   " UPDATE " + _update.GetExpression(ref bindCount) + 
                   " IN " + _collectionName + 
                   (_options.Count > 0 ? " OPTIONS {" + string.Join(", ", _options.Select(kvp => kvp.Key + ":" + kvp.Value)) + "}" : "");
        }

        public object[] GetBindedVars()
        {
            return _search.GetBindedVars().Union(_insert.GetBindedVars()).Union(_update.GetBindedVars()).ToArray();
        }
    }
}