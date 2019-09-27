using System.Collections.Generic;
using System.Linq;

namespace ArangoDriver.Client.Query.Value
{
    public class AqlArrayValue<T> : IAqlValue<T>
    {
        protected IEnumerable<IAqlValue<T>> _values;
        
        internal AqlArrayValue(IEnumerable<IAqlValue<T>> values)
        {
            _values = values;
        }

        public string GetExpression(ref int bindCount)
        {
            string exp = "[";
            foreach (IAqlValue<T> aqlValue in _values)
            {
                exp += aqlValue.GetExpression(ref bindCount) + ",";
            }
            exp = exp.Substring(0, exp.Length - 1) + "]";
            
            return exp;
        }

        public virtual object[] GetBindedVars()
        {
            return _values.SelectMany(x => x.GetBindedVars()).ToArray();
        }
    }
}