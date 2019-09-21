using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using ArangoDriver.Serialization;

namespace ArangoDriver.Protocol
{
    internal class RequestFactory
    {
        private readonly IJsonSerializer _jsonSerializer;

        internal RequestFactory(IJsonSerializer jsonSerializer)
        {
            _jsonSerializer = jsonSerializer;
        }

        internal Request Create(HttpMethod httpMethod, string apiUri)
        {
            return new Request(_jsonSerializer, httpMethod, apiUri);
        }

        internal Request Create(HttpMethod httpMethod, string apiUri, string operationUri)
        {
            return new Request(_jsonSerializer, httpMethod, apiUri, operationUri);
        }
    }
    
    internal class Request
    {
        private readonly IJsonSerializer _jsonSerializer;
        
        internal HttpMethod HttpMethod { get; set; }
        internal string OperationUri { get; set; }
        //internal WebHeaderCollection Headers = new WebHeaderCollection();
        internal Dictionary<string, string> Headers = new Dictionary<string, string>();
        internal Dictionary<string, string> QueryString = new Dictionary<string, string>();
        internal string Body { get; set; }

        internal Request(IJsonSerializer jsonSerializer, HttpMethod httpMethod, string apiUri) : this(jsonSerializer, httpMethod, apiUri, "")
        {
        }

        internal Request(IJsonSerializer jsonSerializer, HttpMethod httpMethod, string apiUri, string operationUri)
        {
            _jsonSerializer = jsonSerializer;
            HttpMethod = httpMethod;
            OperationUri = apiUri + operationUri;
        }
        
        internal string GetRelativeUri()
        {
            var uri = new StringBuilder(OperationUri);
            
            if (QueryString.Count > 0)
            {
                uri.Append("?");
                
                var index = 0;

                foreach (var item in QueryString)
                {
                    uri.Append(item.Key + "=" + item.Value);

                    index++;

                    if (index != QueryString.Count)
                    {
                        uri.Append("&");
                    }
                }
            }
            
            return uri.ToString();
        }
        
        internal void TrySetHeaderParameter(string parameterName, Dictionary<string, object> parameters)
        {
            if (parameters.ContainsKey(parameterName))
            {
                string value = (string)parameters[parameterName];
                if (parameterName == ParameterName.IfMatch)
                    value = "\"" + value + "\"";
                
                Headers.Add(parameterName, value);
            }
        }
        
        internal void TrySetQueryStringParameter(string parameterName, Dictionary<string, object> parameters)
        {
            if (parameters.ContainsKey(parameterName))
            {
                QueryString.Add(parameterName, (string)parameters[parameterName]);
            }
        }

        internal void SetBody<T>(T obj)
        {
            Body = _jsonSerializer.Serialize(obj);
        }
        
        internal static void TrySetBodyParameter(string parameterName, Dictionary<string, object> source, Dictionary<string, object> destination)
        {
            if (source.ContainsKey(parameterName))
            {
                destination[parameterName] = source[parameterName];
            }
        }
    }
}
