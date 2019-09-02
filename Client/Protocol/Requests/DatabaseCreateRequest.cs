using System.Collections.Generic;
using ArangoDriver.Client;
using Newtonsoft.Json;

namespace ArangoDriver.Protocol.Requests
{
    internal class DatabaseCreateRequest
    {
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }
        
        [JsonProperty(PropertyName = "users")]
        public List<AUser> Users { get; set; }
    }
}