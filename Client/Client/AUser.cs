using System.Collections.Generic;
using Newtonsoft.Json;

namespace ArangoDriver.Client
{
    public class AUser
    {
        [JsonProperty(PropertyName = "username")]
        public string Username { get; set; }
        
        [JsonProperty(PropertyName = "passwd")]
        public string Password { get; set; }
        
        [JsonProperty(PropertyName = "active")]
        public bool Active { get; set; }
        
        [JsonProperty(PropertyName = "extra")]
        public Dictionary<string, object> Extra { get; set; }
        
        public AUser()
        {
            Active = true;
        }
    }
}
