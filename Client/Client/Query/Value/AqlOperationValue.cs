using System.Linq;

namespace ArangoDriver.Client.Query.Value
{
    public class AqlOperationValue<T> : IAqlValue<T>
    {
        private readonly string _function;
        private readonly IAqlValue<T>[] _values;
        
        internal AqlOperationValue(string function, params IAqlValue<T>[] values)
        {
            _function = function;
            _values = values;
        }

        public string GetExpression(ref int bindCount)
        {
            string[] exp = new string[_values.Length];
            for (int i = 0; i < _values.Length; i++)
            {
                exp[i] = _values[i].GetExpression(ref bindCount);
            }

            return "(" + string.Join(" " + _function + " ", exp) + ")";
        }

        public object[] GetBindedVars()
        {
            return _values.SelectMany(x => x.GetBindedVars()).ToArray();
        }
    }
}