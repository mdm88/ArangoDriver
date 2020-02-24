namespace ArangoDriver.Client.Query.Query
{
    public class AqlQueryRaw : IAqlQuery
    {
        private readonly string _expression;

        public AqlQueryRaw(string expression)
        {
            _expression = expression;
        }
        
        public string GetExpression(ref int bindCount)
        {
            return _expression;
        }

        public object[] GetBindedVars()
        {
            return new object[] {};
        }
    }
}