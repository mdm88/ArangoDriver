using System.Collections.Generic;
using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace ArangoDriver.Protocol.Requests
{
    public class IndexCreateRequest
    {
        [JsonProperty(PropertyName = "type")]
        public string Type { get; set; }
        
        [JsonProperty(PropertyName = "fields", NullValueHandling = NullValueHandling.Ignore)]
        public List<string> Fields { get; set; }
        
        [JsonProperty(PropertyName = "minLength", NullValueHandling = NullValueHandling.Ignore)]
        public int? MinLength { get; set; }
        
        [JsonProperty(PropertyName = "geoJson", NullValueHandling = NullValueHandling.Ignore)]
        public bool? GeoJson { get; set; }
        
        [JsonProperty(PropertyName = "sparse", NullValueHandling = NullValueHandling.Ignore)]
        public bool? Sparse { get; set; }
        
        [JsonProperty(PropertyName = "unique", NullValueHandling = NullValueHandling.Ignore)]
        public bool? Unique { get; set; }
    }
}