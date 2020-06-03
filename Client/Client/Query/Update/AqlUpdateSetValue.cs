using ArangoDriver.Client.Query.Value;

namespace ArangoDriver.Client.Query.Update
{
    public class AqlUpdateSetValue : IAqlUpdate
    {
        private readonly string _field;
        private readonly IAqlValue _value;

        public AqlUpdateSetValue(string field, IAqlValue value)
        {
            _field = field;
            _value = value;
        }
        
        public string GetExpression(ref int bindCount)
        {
            return _field + ":" + _value.GetExpression(ref bindCount);
        }

        public object[] GetBindedVars()
        {
            return _value.GetBindedVars();
        }
    }
}