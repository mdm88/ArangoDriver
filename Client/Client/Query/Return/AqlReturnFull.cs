namespace ArangoDriver.Client.Query.Return
{
    public class AqlReturnFull : IAqlReturn
    {
        public string _returns;

        internal AqlReturnFull(string returns)
        {
            _returns = returns;
        }
        
        public string GetExpression()
        {
            return _returns;
        }
    }
}