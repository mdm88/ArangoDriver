namespace ArangoDriver.Client.Query.Value
{
    public class AqlCollectionFunctionValue<T> : IAqlValue<T>
    {
        private readonly string _collection;
        private readonly IAqlValue _value;
        
        internal AqlCollectionFunctionValue(string collection, IAqlValue value)
        {
            _collection = collection;
            _value = value;
        }

        public string GetExpression(ref int bindCount)
        {
            return "DOCUMENT('" + _collection + "'," + _value.GetExpression(ref bindCount) + ")";
        }

        public object[] GetBindedVars()
        {
            return _value.GetBindedVars();
        }
    }
}