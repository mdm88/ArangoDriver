namespace ArangoDriver.Client.Query.Query
{
    public class AqlRemove : IAqlQuery
    {
        private readonly string _alias;
        private readonly string _collectionName;

        internal AqlRemove(string alias, string collectionName)
        {
            _alias = alias;
            _collectionName = collectionName;
        }
        
        public string GetExpression(ref int bindCount)
        {
            return "REMOVE " + _alias + " IN " + _collectionName;
        }

        public object[] GetBindedVars()
        {
            return new object[] {};
        }
    }
}