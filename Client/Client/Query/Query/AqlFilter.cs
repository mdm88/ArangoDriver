using ArangoDriver.Client.Query.Filter;

namespace ArangoDriver.Client.Query.Query
{
    public class AqlFilter : IAqlQuery
    {
        private readonly IAqlFilter _filter;

        internal AqlFilter(IAqlFilter filter)
        {
            _filter = filter;
        }
        
        public string GetExpression(ref int bindCount)
        {
            return "FILTER " + _filter.GetExpression(ref bindCount);
        }

        public object[] GetBindedVars()
        {
            return _filter.GetBindedVars();
        }
    }
}