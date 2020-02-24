namespace ArangoDriver.Client.Query.Value
{
    public class AqlSubqueryValue<T> : IAqlValue<T>
    {
        private readonly AQuery _query;

        public AqlSubqueryValue(AQuery query)
        {
            _query = query;
        }
        
        public string GetExpression(ref int bindCount)
        {
            return "(" + _query.GetExpression(ref bindCount) + ")";
        }

        public object[] GetBindedVars()
        {
            return _query.GetBindedVars();
        }
    }
}