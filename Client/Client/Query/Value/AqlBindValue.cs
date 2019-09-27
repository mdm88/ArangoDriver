namespace ArangoDriver.Client.Query.Value
{
    public class AqlBindValue<T> : IAqlValue<T>
    {
        protected T _value;
        
        internal AqlBindValue(T value)
        {
            _value = value;
        }

        public string GetExpression(ref int bindCount)
        {
            return "@var" + bindCount++;
        }

        public virtual object[] GetBindedVars()
        {
            return new object[] {_value};
        }
    }
}