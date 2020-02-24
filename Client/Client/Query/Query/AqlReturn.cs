namespace ArangoDriver.Client.Query.Query
{
    public class AqlReturn : IAqlQuery
    {
        private readonly string _alias;

        internal AqlReturn(string alias)
        {
            _alias = alias;
        }
        
        public string GetExpression(ref int bindCount)
        {
            return "RETURN " + _alias;
        }

        public object[] GetBindedVars()
        {
            return new object[] {};
        }
    }
}