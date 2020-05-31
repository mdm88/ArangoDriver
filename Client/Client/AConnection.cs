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
        private readonly JsonNetSerializer _jsonSerializer;
        private readonly RequestFactory _requestFactory;
        
        private readonly string _username;
        private readonly string _password;
        private readonly Uri _baseUri;
        private readonly bool _useWebProxy;
        
        public AConnection(string hostname, int port, bool isSecured, string userName, string password, bool useWebProxy = false)
        {
            _username = userName;
            _password = password;

            _useWebProxy = useWebProxy;

            _baseUri = new Uri((isSecured ? "https" : "http") + "://" + hostname + ":" + port + "/");
            
            _httpClient = new HttpClient();
            _jsonSerializer = new JsonNetSerializer();
            _requestFactory = new RequestFactory(_jsonSerializer);
        }

        public void RegisterSerializer(JsonConverter converter)
        {
            _jsonSerializer.RegisterSerializer(converter);
        }
        
        #region Databases

        /// <summary>
        /// Get specific database connection object
        /// </summary>
        /// <param name="databaseName">Database name</param>
        /// <returns>ADatabase</returns>
        public ADatabase GetDatabase(string databaseName)
        {
            return new ADatabase(_requestFactory, this, databaseName);
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


            HttpResponseMessage httpResponseMessage = await _httpClient.SendAsync(httpRequestMessage);

            var response = new Response(_jsonSerializer)
            {
                StatusCode = (int) httpResponseMessage.StatusCode,
                Headers = httpResponseMessage.Headers,
                Body = await httpResponseMessage.Content.ReadAsStringAsync()
            };

            return response;
        }
    }
}
