using System.Collections.Generic;
using Newtonsoft.Json;

namespace ArangoDriver.Protocol.Responses
{
    public class IndexCreateResponse
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }
        
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }
        
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
        
        [JsonProperty(PropertyName = "isNewlyCreated", NullValueHandling = NullValueHandling.Ignore)]
        public bool? IsNewlyCreated { get; set; }
        
        [JsonProperty(PropertyName = "selectivityEstimate", NullValueHandling = NullValueHandling.Ignore)]
        public int? SelectivityEstimate { get; set; }
    }
}