using ArangoDriver.Client.Query.Value;

namespace ArangoDriver.Client.Query.Query
{
    public class AqlFor : IAqlQuery
    {
        private readonly string _alias;
        private readonly IAqlValue _collection;

        internal AqlFor(string alias, IAqlValue collection)
        {
            _alias = alias;
            _collection = collection;
        }
        
        public string GetExpression(ref int bindCount)
        {
            return "FOR " + _alias + " IN " + _collection.GetExpression(ref bindCount);
        }

        public object[] GetBindedVars()
        {
            return _collection.GetBindedVars();
        }
    }
}