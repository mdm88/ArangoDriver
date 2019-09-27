namespace ArangoDriver.Client.Query.Value
{
    public class AqlFunctionSingleValue<T> : IAqlValue<T>
    {
        private readonly string _function;
        private readonly IAqlValue _value;
        
        internal AqlFunctionSingleValue(string function, IAqlValue value)
        {
            _function = function;
            _value = value;
        }

        public string GetExpression(ref int bindCount)
        {
            return _function + "(" + _value.GetExpression(ref bindCount) + ")";
        }

        public object[] GetBindedVars()
        {
            return _value.GetBindedVars();
        }
    }
}