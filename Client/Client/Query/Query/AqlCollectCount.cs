namespace ArangoDriver.Client.Query.Query
{
    public class AqlCollectCount : IAqlQuery
    {
        private readonly string _alias;

        internal AqlCollectCount(string alias)
        {
            _alias = alias;
        }
        
        public string GetExpression(ref int bindCount)
        {
            return "COLLECT WITH COUNT INTO " + _alias;
        }

        public object[] GetBindedVars()
        {   
            return new object[] {};
        }
    }
}