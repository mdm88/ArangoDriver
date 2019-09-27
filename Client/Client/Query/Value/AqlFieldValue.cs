namespace ArangoDriver.Client.Query.Value
{
    public class AqlFieldValue<T> : IAqlValue<T>
    {
        private readonly string _expression;

        internal AqlFieldValue(string value)
        {
            _expression = value;
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