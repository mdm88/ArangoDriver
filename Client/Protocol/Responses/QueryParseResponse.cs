using System.Collections.Generic;
using Newtonsoft.Json;

namespace ArangoDriver.Protocol.Responses
{
    public class QueryParseResponse
    {
        [JsonProperty(PropertyName = "collections")]
        public List<object> Collections { get; set; }
        
        [JsonProperty(PropertyName = "bindVars")]
        public List<object> BindVars { get; set; }
        
        [JsonProperty(PropertyName = "ast")]
        public List<object> Ast { get; set; }
    }
}