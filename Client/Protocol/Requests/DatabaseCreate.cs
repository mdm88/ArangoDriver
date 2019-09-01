using System.Collections.Generic;
using ArangoDriver.Client;
using Newtonsoft.Json;

namespace ArangoDriver.Protocol.Requests
{
    internal class DatabaseCreate
    {
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }
        
        [JsonProperty(PropertyName = "users")]
        public List<AUser> Users { get; set; }
    }
}