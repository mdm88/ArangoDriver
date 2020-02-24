using ArangoDriver.Client.Query.Value;

namespace ArangoDriver.Client.Query.Query
{
    public class AqlCollect : IAqlQuery
    {
        private readonly string _alias;
        private readonly IAqlValue _value;

        internal AqlCollect()
        {
        }

        internal AqlCollect(string alias, IAqlValue value)
        {
            _alias = alias;
            _value = value;
        }
        
        public string GetExpression(ref int bindCount)
        {
            if (_alias != null && _value != null)
                return "COLLECT " + _alias + "=" + _value.GetExpression(ref bindCount);
            
            return "COLLECT";
        }

        public object[] GetBindedVars()
        {
            if (_alias != null && _value != null)
                return _value.GetBindedVars();
            
            return new object[] {};
        }
    }
}