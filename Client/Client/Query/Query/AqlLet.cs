using ArangoDriver.Client.Query.Value;

namespace ArangoDriver.Client.Query.Query
{
    public class AqlLet : IAqlQuery
    {
        private readonly string _alias;
        private readonly IAqlValue _value;

        internal AqlLet(string alias, IAqlValue value)
        {
            _alias = alias;
            _value = value;
        }
        
        public string GetExpression(ref int bindCount)
        {
            return "LET " + _alias + " = " + _value.GetExpression(ref bindCount);
        }

        public object[] GetBindedVars()
        {
            return _value.GetBindedVars();
        }
    }
}