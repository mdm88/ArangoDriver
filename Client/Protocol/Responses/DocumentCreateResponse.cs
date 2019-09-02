using Newtonsoft.Json;

namespace ArangoDriver.Protocol.Responses
{
    internal class DocumentCreateResponse<T>
    {
        [JsonProperty(PropertyName = "_id")]
        public string Id { get; set; }
        
        [JsonProperty(PropertyName = "_key")]
        public string Key { get; set; }
        
        [JsonProperty(PropertyName = "_rev")]
        public string Revision { get; set; }
        
        [JsonProperty(PropertyName = "new")]
        public T New { get; set; }
        
        [JsonProperty(PropertyName = "old")]
        public T Old { get; set; }
    }
}