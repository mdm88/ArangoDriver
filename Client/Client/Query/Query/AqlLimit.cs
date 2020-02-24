namespace ArangoDriver.Client.Query.Query
{
    public class AqlLimit : IAqlQuery
    {
        private readonly int _limit;

        internal AqlLimit(int limit)
        {
            _limit = limit;
        }
        
        public string GetExpression(ref int bindCount)
        {
            return "LIMIT " + _limit;
        }

        public object[] GetBindedVars()
        {
            return new object[] {};
        }
    }
}