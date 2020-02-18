using System.Linq;

namespace ArangoDriver.Client.Query.Filter
{
    public class AqlFilterAnd : IAqlFilter
    {
        private readonly IAqlFilter[] _filters;
        
        internal AqlFilterAnd(params IAqlFilter[] filters)
        {
            _filters = filters;
        }

        public string GetExpression(ref int bindCount)
        {
            string exp = "(";
            foreach (IAqlFilter filter in _filters)
            {
                exp += filter.GetExpression(ref bindCount) + " AND ";
            }
            exp = exp.Substring(0, exp.Length - 5) + ")";

            return exp;
        }

        public object[] GetBindedVars()
        {
            return _filters.SelectMany(f => f.GetBindedVars()).ToArray();
        }
    }
}