using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using ArangoDriver.Exceptions;
using ArangoDriver.Protocol;
using ArangoDriver.Protocol.Requests;
using ArangoDriver.Serialization;
using Newtonsoft.Json;

namespace ArangoDriver.Client
{
    /// <summary>
    /// Stores data about single endpoint and processes communication between client and server.
    /// </summary>
    public class AConnection
    {
        private readonly HttpClient _httpClient;
        private readonly IJsonSerializer _jsonSerializer;
        private readonly RequestFactory _requestFactory;
        
        private readonly string _username;
        private readonly string _password;
        private readonly Uri _baseUri;
        
        public AConnection(string hostname, int port, bool isSecured, string userName, string password, IJsonSerializer jsonSerializer)
        {
            _username = userName;
            _password = password;

            _baseUri = new Uri((isSecured ? "https" : "http") + "://" + hostname + ":" + port + "/");
            
            _httpClient = new HttpClient();
            _jsonSerializer = jsonSerializer;
            _requestFactory = new RequestFactory(_jsonSerializer);
        }
        
        #region Databases

        /// <summary>
        /// Get specific database connection object
        /// </summary>
        /// <param name="databaseName">Database name</param>
        /// <returns>ADatabase</returns>
        public ADatabase GetDatabase(string databaseName)
        {
            return new ADatabase(_requestFactory, this, databaseName, _jsonSerializer);
        }
        
        /// <summary>
        /// Creates new database with given name.
        /// </summary>
        public Task<AResult<bool>> CreateDatabase(string databaseName)
        {
            return CreateDatabase(databaseName, null);
        }
        
        /// <summary>
        /// Creates new database with given name and user list.
        /// </summary>
        public async Task<AResult<bool>> CreateDatabase(string databaseName, List<AUser> users)
        {
            var request = _requestFactory.Create(HttpMethod.Post, ApiBaseUri.Database, "");
            var document = new DatabaseCreateRequest()
            {
                Name = databaseName,
                Users = users
            };
            
            request.SetBody(document);
            
            var response = await Send(request);
            var result = new AResult<bool>(response);
            
            switch (response.StatusCode)
            {
                case 201:
                    var body = response.ParseBody<Body<bool>>();
                    
                    result.Success = (body != null);
                    result.Value = body?.Result ?? false;
                    break;
                case 409:
                    throw new DatabaseAlreadyExistsException();
                default:
                    throw new ArangoException(response.Body);
            }
            
            return result;
        }
        
        /// <summary>
        /// Retrieves list of accessible databases which current user can access without specifying a different username or password.
        /// </summary>
        public async Task<AResult<List<string>>> GetAccessibleDatabases()
        {
            var request = _requestFactory.Create(HttpMethod.Get, ApiBaseUri.Database, "/user");
            
            var response = await Send(request);
            var result = new AResult<List<string>>(response);
            
            switch (response.StatusCode)
            {
                case 200:
                    var body = response.ParseBody<Body<List<string>>>();
                    
                    result.Success = (body != null);
                    result.Value = body?.Result;
                    break;
                default:
                    throw new ArangoException(response.Body);
            }
            
            return result;
        }

        /// <summary>
        /// Retrieves the list of all existing databases.
        /// </summary>
        public async Task<AResult<List<string>>> GetAllDatabases()
        {
            var request = _requestFactory.Create(HttpMethod.Get, ApiBaseUri.Database, "");
            
            var response = await Send(request);
            var result = new AResult<List<string>>(response);
            
            switch (response.StatusCode)
            {
                case 200:
                    var body = response.ParseBody<Body<List<string>>>();
                    
                    result.Success = (body != null);
                    result.Value = body?.Result;
                    break;
                default:
                    throw new ArangoException(response.Body);
            }
            
            return result;
        }

        /// <summary>
        /// Deletes specified database.
        /// </summary>
        public async Task<AResult<bool>> DropDatabase(string databaseName)
        {
            var request = _requestFactory.Create(HttpMethod.Delete, ApiBaseUri.Database, "/" + databaseName);
            
            var response = await Send(request);
            var result = new AResult<bool>(response);
            
            switch (response.StatusCode)
            {
                case 200:
                    var body = response.ParseBody<Body<bool>>();
                    
                    result.Success = (body != null);
                    result.Value = body?.Result ?? false;
                    break;
                case 404:
                    throw new DatabaseNotFoundException();
                default:
                    throw new ArangoException(response.Body);
            }
            
            return result;
        }

        #endregion
        
        internal Task<Response> Send(Request request)
        {
            return Send(_baseUri, request);
        }
        
        internal Task<Response> Send(string databaseName, Request request)
        {
            return Send(new Uri(_baseUri + "_db/" + databaseName + "/"), request);
        }

        private async Task<Response> Send(Uri uri, Request request)
        {
            HttpRequestMessage httpRequestMessage = new HttpRequestMessage(request.HttpMethod, uri + request.GetRelativeUri());
            httpRequestMessage.Version = HttpVersion.Version11;
            
            foreach (KeyValuePair<string, string> header in request.Headers)
            {
                httpRequestMessage.Headers.Add(header.Key, header.Value);
            }

            httpRequestMessage.Headers.Add(HttpRequestHeader.KeepAlive.ToString(), "true");
            httpRequestMessage.Headers.Add(HttpRequestHeader.UserAgent.ToString(),
                ASettings.DriverName + "/" + ASettings.DriverVersion);

            if (!string.IsNullOrEmpty(_username) && !string.IsNullOrEmpty(_password))
            {
                httpRequestMessage.Headers.Add(
                    HttpRequestHeader.Authorization.ToString(),
                    "Basic " + Convert.ToBase64String(Encoding.ASCII.GetBytes(_username + ":" + _password))
                );
            }

            if (request.Body != null)
            {
                httpRequestMessage.Content = new StringContent(request.Body, Encoding.UTF8, "application/json");
                //httpRequestMessage.Headers.Add(HttpRequestHeader.ContentLength.ToString(), "0");
            }
            else
            {
                httpRequestMessage.Headers.Add(HttpRequestHeader.ContentLength.ToString(), "0");
            }
            
            
            /*using var response = await _httpClient.GetAsync("http://localhost:58815/books", HttpCompletionOption.ResponseHeadersRead);

            response.EnsureSuccessStatusCode();

            if (response.Content is object)
            {
                var stream = await response.Content.ReadAsStreamAsync();
                var data = await JsonSerializer.DeserializeAsync<List<Book>>(stream);
                // do something with the data or return it
            }*/

            HttpResponseMessage httpResponseMessage = await _httpClient.SendAsync(httpRequestMessage);

            var response = new Response(_jsonSerializer)
            {
                StatusCode = (int) httpResponseMessage.StatusCode,
                Headers = httpResponseMessage.Headers,
                Body = await httpResponseMessage.Content.ReadAsStringAsync()
            };

            return response;
        }
        
        
        
        internal Task<HttpResponseMessage> Request(Request request, string uri = "")
        {
            HttpRequestMessage httpRequestMessage = new HttpRequestMessage(request.HttpMethod, _baseUri + uri + request.GetRelativeUri());
            httpRequestMessage.Version = HttpVersion.Version11;
            
            foreach (KeyValuePair<string, string> header in request.Headers)
            {
                httpRequestMessage.Headers.Add(header.Key, header.Value);
            }

            httpRequestMessage.Headers.Add(HttpRequestHeader.KeepAlive.ToString(), "true");
            httpRequestMessage.Headers.Add(HttpRequestHeader.UserAgent.ToString(),
                ASettings.DriverName + "/" + ASettings.DriverVersion);

            if (!string.IsNullOrEmpty(_username) && !string.IsNullOrEmpty(_password))
            {
                httpRequestMessage.Headers.Add(
                    HttpRequestHeader.Authorization.ToString(),
                    "Basic " + Convert.ToBase64String(Encoding.ASCII.GetBytes(_username + ":" + _password))
                );
            }

            if (request.Body != null)
            {
                httpRequestMessage.Content = new StringContent(request.Body, Encoding.UTF8, "application/json");
                //httpRequestMessage.Headers.Add(HttpRequestHeader.ContentLength.ToString(), "0");
            }
            else
            {
                httpRequestMessage.Headers.Add(HttpRequestHeader.ContentLength.ToString(), "0");
            }

            return _httpClient.SendAsync(httpRequestMessage, HttpCompletionOption.ResponseHeadersRead);
        }
        
        internal async Task<AResult<T>> RequestQuery<T>(Request request, string uri = "")
        {
            using var response = await Request(request, uri);
            
            var result = new AResult<T>()
            {
                StatusCode = (int) response.StatusCode,
                Success = response.IsSuccessStatusCode
            };
            
            if (response.IsSuccessStatusCode) {
                result.Value = _jsonSerializer.Deserialize<T>(await response.Content.ReadAsStreamAsync());
            }

            return result;
        }
        
        internal async Task<AResult<object>> RequestExecute(Request request, string uri = "")
        {
            using var response = await Request(request, uri);
            
            return new AResult<object>()
            {
                StatusCode = (int) response.StatusCode,
                Success = response.IsSuccessStatusCode
            };
        }
    }
}
