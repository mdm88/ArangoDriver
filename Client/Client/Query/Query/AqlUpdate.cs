using ArangoDriver.Client.Query.Value;

namespace ArangoDriver.Client.Query.Query
{
    public class AqlUpdate<T> : IAqlQuery
    {
        private readonly UpdateDefinition<T> _definition;

        internal AqlUpdate(UpdateDefinition<T> definition)
        {
            _definition = definition;
        }
        
        public string GetExpression(ref int bindCount)
        {
            string expression = _definition.Expression;
            int i = 0;
            foreach (object value in _definition.Values)
            {
                expression = expression.Replace("{" + i++ + "}", "@var" + bindCount++);
            }

            return expression;
        }

        public object[] GetBindedVars()
        {
            return _definition.Values.ToArray();
        }
    }
}