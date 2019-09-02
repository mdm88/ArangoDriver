using Newtonsoft.Json;

namespace Tests
{
    public interface IFoo
    {
        string Id { get; set; }
        string Foo { get; set; }
    }
    
    public class Dummy : IFoo
    {
        [JsonProperty(PropertyName = "_id", NullValueHandling = NullValueHandling.Ignore)]
        public string Id { get; set; }
        
        [JsonProperty(PropertyName = "_key", NullValueHandling = NullValueHandling.Ignore)]
        public string Key { get; set; }
        
        [JsonProperty(PropertyName = "_rev", NullValueHandling = NullValueHandling.Ignore)]
        public string Revision { get; set; }
        
        public string Foo { get; set; }
        
        public int Bar { get; set; }
        
        public int Baz { get; set; }
    }
}
