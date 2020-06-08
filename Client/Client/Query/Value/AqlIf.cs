using System.Collections.Generic;

namespace ArangoDriver.Client.Query.Value
{
    public class AqlIf<T> : IAqlValue<T>
    {
        private readonly IAqlExpression<bool> _condition;
        private readonly IAqlValue<T> _positive;
        private readonly IAqlValue<T> _negative;

        internal AqlIf(IAqlExpression<bool> condition, IAqlValue<T> positive, IAqlValue<T> negative)
        {
            _condition = condition;
            _positive = positive;
            _negative = negative;
        }

        public string GetExpression(ref int bindCount)
        {
            return 
                "(" +
                _condition.GetExpression(ref bindCount) + 
                " ? " +
                _positive.GetExpression(ref bindCount) + 
                " : " +
                _negative.GetExpression(ref bindCount) + 
                ")";
        }

        public object[] GetBindedVars()
        {
            var list = new List<object>();
            list.AddRange(_condition.GetBindedVars());
            list.AddRange(_positive.GetBindedVars());
            list.AddRange(_negative.GetBindedVars());

            return list.ToArray();
        }
    }
}