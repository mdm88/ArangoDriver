using System.Linq;

namespace ArangoDriver.Client.Query.Value
{
    public class AqlOperationValue<T> : IAqlValue<T>
    {
        private readonly string _function;
        private readonly IAqlValue<T> _value1;
        private readonly IAqlValue<T> _value2;
        
        internal AqlOperationValue(string function, IAqlValue<T> value1, IAqlValue<T> value2)
        {
            _function = function;
            _value1 = value1;
            _value2 = value2;
        }

        public string GetExpression(ref int bindCount)
        {
            return _value1.GetExpression(ref bindCount) + " " + _function + " " + _value2.GetExpression(ref bindCount);
        }

        public object[] GetBindedVars()
        {
            return _value1.GetBindedVars().Union(_value2.GetBindedVars()).ToArray();
        }
    }
}