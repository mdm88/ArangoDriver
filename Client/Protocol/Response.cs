using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using ArangoDriver.Serialization;
using Newtonsoft.Json;

namespace ArangoDriver.Protocol
{
    internal class Response
    {
        private readonly IJsonSerializer _jsonSerializer;
        
        internal int StatusCode { get; set; }
        internal HttpResponseHeaders Headers { get; set; }
        internal string Body { get; set; }
        internal BodyType BodyType { get; set; }

        public Response(IJsonSerializer jsonSerializer)
        {
            _jsonSerializer = jsonSerializer;
        }

        internal T ParseBody<T>()
        {
            if (string.IsNullOrEmpty(Body))
            {
                return default(T);
            }

            return _jsonSerializer.Deserialize<T>(Body);
        }
    }
}
