using System.Collections.Generic;
using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace ArangoDriver.Protocol.Responses
{
    internal class CollectionsGetAllResponse
    {
        [JsonProperty(PropertyName = "error")]
        public string Error { get; set; }
        
        [JsonProperty(PropertyName = "code")]
        public string Code { get; set; }
        
        [JsonProperty(PropertyName = "result")]
        public List<Dictionary<string, object>> Result { get; set; }
    }
}