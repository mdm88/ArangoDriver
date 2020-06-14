using System;
using System.Collections.Generic;
using System.Linq;

namespace ArangoDriver.Client.Query.Return
{
    public class AqlReturnManual : IAqlReturn
    {
        private readonly Dictionary<string, IAqlReturn> _returns;
        
        internal AqlReturnManual()
        {
            _returns = new Dictionary<string, IAqlReturn>();
        }

        internal AqlReturnManual(params (string, IAqlReturn)[] fields) : this()
        {
            foreach (var kvp in fields)
            {
                _returns.Add(kvp.Item1, kvp.Item2);
            }
        }
        
        public AqlReturnManual Add(string key, IAqlReturn returned)
        {
            _returns[key] = returned;
            
            return this;
        }
        
        public string GetExpression()
        {
            return "{" + String.Join(",", _returns.Select(x => x.Key + ":" + x.Value.GetExpression())) + "}";
        }
    }
}