using System.Linq;
using ArangoDriver.Client.Query.Value;

namespace ArangoDriver.Client.Query.Filter
{
    public class AqlFilter : IAqlFilter
    {
        private readonly string _operator;
        private readonly IAqlValue _value1;
        private readonly IAqlValue _value2;

        internal AqlFilter(string op, IAqlValue value1, IAqlValue value2)
        {
            _operator = op;
            _value1 = value1;
            _value2 = value2;
        }

        public string GetExpression(ref int bindCount)
        {
            return _value1.GetExpression(ref bindCount) + " " + _operator + " " + _value2.GetExpression(ref bindCount);
        }

        public object[] GetBindedVars()
        {
            object[] bind1 = _value1.GetBindedVars();
            object[] bind2 = _value2.GetBindedVars();
            
            if (bind1 != null && bind2 != null)
                return bind1.Union(bind2).ToArray();
            if (bind1 != null)
                return bind1;
            if (bind2 != null)
                return bind2;
            
            return null;
        }
    }
}