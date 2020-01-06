using System.Linq;

namespace ArangoDriver.Client.Query.Value
{
    public class AqlFunctionValue<T> : IAqlValue<T>
    {
        private readonly string _function;
        private readonly IAqlValue[] _values;
        
        internal AqlFunctionValue(string function, params IAqlValue[] values)
        {
            _function = function;
            _values = values;
        }

        public string GetExpression(ref int bindCount)
        {
            string values = "";
            foreach (IAqlValue value in _values)
            {
                values += value.GetExpression(ref bindCount) + ",";
            }
            values = values.Remove(values.Length - 1);
            
            return _function + "(" + values + ")";
        }

        public object[] GetBindedVars()
        {
            return _values.SelectMany(x => x.GetBindedVars()).ToArray();
        }
    }
}