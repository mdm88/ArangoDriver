using System.Collections.Generic;
using Newtonsoft.Json;

namespace ArangoDriver.Protocol.Requests
{
    public class QueryRequest
    {
        [JsonProperty(PropertyName = "query")]
        public string Query { get; set; }
        
        [JsonProperty(PropertyName = "bindVars", NullValueHandling = NullValueHandling.Ignore)]
        public Dictionary<string, object> BindVars { get; set; }
        
        [JsonProperty(PropertyName = "count", NullValueHandling = NullValueHandling.Ignore)]
        public bool? Count { get; set; }
        
        [JsonProperty(PropertyName = "batchSize", NullValueHandling = NullValueHandling.Ignore)]
        public int? BatchSize { get; set; }
        
        [JsonProperty(PropertyName = "ttl", NullValueHandling = NullValueHandling.Ignore)]
        public int? TTL { get; set; }
    }
}