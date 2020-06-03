using ArangoDriver.Client.Query.Value;

namespace ArangoDriver.Client.Query.Update
{
    public class AqlUpdateInc : IAqlUpdate
    {
        private readonly string _field;
        private readonly string _fieldname;
        private readonly IAqlValue _value;

        public AqlUpdateInc(string field, string fieldname, IAqlValue value)
        {
            _field = field;
            _fieldname = fieldname;
            _value = value;
        }
        
        public string GetExpression(ref int bindCount)
        {
            return _field + ":" + _fieldname + "." + _field + "+" + _value.GetExpression(ref bindCount);
        }

        public object[] GetBindedVars()
        {
            return _value.GetBindedVars();
        }
    }
}