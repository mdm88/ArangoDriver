using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using ArangoDriver.dictator;
using ArangoDriver.Protocol;
using fastJSON;

namespace ArangoDriver.Client
{
    /// <summary>
    /// Stores data about single endpoint and processes communication between client and server.
    /// </summary>
    public class AConnection
    {
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
        }
        
        #region Databases

        /// <summary>
        /// Get specific database connection object
        /// </summary>
        /// <param name="databaseName">Database name</param>
        /// <returns>ADatabase</returns>
        public ADatabase GetDatabase(string databaseName)
        {
            return new ADatabase(this, databaseName);
        }
        
        /// <summary>
        /// Creates new database with given name.
        /// </summary>
        public AResult<bool> Create(string databaseName)
        {
            return Create(databaseName, null);
        }
        
        /// <summary>
        /// Creates new database with given name and user list.
        /// </summary>
        public AResult<bool> Create(string databaseName, List<AUser> users)
        {
            var request = new Request(HttpMethod.POST, ApiBaseUri.Database, "");
            var bodyDocument = new Dictionary<string, object>();
            
            // required: database name
            bodyDocument.String("name", databaseName);
            
            // optional: list of users
            if ((users != null) && (users.Count > 0))
            {
                var userList = new List<Dictionary<string, object>>();
                
                foreach (var user in users)
                {
                    var userItem = new Dictionary<string, object>();
                    
                    if (!string.IsNullOrEmpty(user.Username))
                    {
                        userItem.String("username", user.Username);
                    }
                    
                    if (!string.IsNullOrEmpty(user.Password))
                    {
                        userItem.String("passwd", user.Password);
                    }
                    
                    userItem.Bool("active", user.Active);
                    
                    if (user.Extra != null)
                    {
                        userItem.Document("extra", user.Extra);
                    }
                    
                    userList.Add(userItem);
                }
                
                bodyDocument.List("users", userList);
            }
            
            request.Body = JSON.ToJSON(bodyDocument, ASettings.JsonParameters);
            
            var response = Send(request);
            var result = new AResult<bool>(response);
            
            switch (response.StatusCode)
            {
                case 201:
                    var body = response.ParseBody<Body<bool>>();
                    
                    result.Success = (body != null);
                    result.Value = body?.Result ?? false;
                    break;
                case 400:
                case 403:
                case 404:
                default:
                    // Arango error
                    break;
            }
            
            return result;
        }
        
        /// <summary>
        /// Retrieves list of accessible databases which current user can access without specifying a different username or password.
        /// </summary>
        public AResult<List<string>> GetAccessibleDatabases()
        {
            var request = new Request(HttpMethod.GET, ApiBaseUri.Database, "/user");
            
            var response = Send(request);
            var result = new AResult<List<string>>(response);
            
            switch (response.StatusCode)
            {
                case 200:
                    var body = response.ParseBody<Body<List<string>>>();
                    
                    result.Success = (body != null);
                    result.Value = body?.Result;
                    break;
                case 400:
                default:
                    // Arango error
                    break;
            }
            
            return result;
        }

        /// <summary>
        /// Retrieves the list of all existing databases.
        /// </summary>
        public AResult<List<string>> GetAllDatabases()
        {
            var request = new Request(HttpMethod.GET, ApiBaseUri.Database, "");
            
            var response = Send(request);
            var result = new AResult<List<string>>(response);
            
            switch (response.StatusCode)
            {
                case 200:
                    var body = response.ParseBody<Body<List<string>>>();
                    
                    result.Success = (body != null);
                    result.Value = body?.Result;
                    break;
                case 400:
                case 403:
                default:
                    // Arango error
                    break;
            }
            
            return result;
        }

        /// <summary>
        /// Deletes specified database.
        /// </summary>
        public AResult<bool> Drop(string databaseName)
        {
            var request = new Request(HttpMethod.DELETE, ApiBaseUri.Database, "/" + databaseName);
            
            var response = Send(request);
            var result = new AResult<bool>(response);
            
            switch (response.StatusCode)
            {
                case 200:
                    var body = response.ParseBody<Body<bool>>();
                    
                    result.Success = (body != null);
                    result.Value = body?.Result ?? false;
                    break;
                case 400:
                case 403:
                case 404:
                default:
                    // Arango error
                    break;
            }
            
            return result;
        }

        #endregion
        
        internal Response Send(Request request)
        {
            return Send(_baseUri, request);
        }
        
        internal Response Send(string databaseName, Request request)
        {
            return Send(new Uri(_baseUri + "_db/" + databaseName + "/"), request);
        }
        
        private Response Send(Uri uri, Request request)
        {
            var httpRequest = WebRequest.CreateHttp(uri + request.GetRelativeUri());

            if (request.Headers.Count > 0)
            {
                httpRequest.Headers = request.Headers;
            }

            httpRequest.KeepAlive = true;
            if (!_useWebProxy)
            {
                httpRequest.Proxy = null;
            }
            httpRequest.SendChunked = false;
            httpRequest.Method = request.HttpMethod.ToString();
            httpRequest.UserAgent = ASettings.DriverName + "/" + ASettings.DriverVersion;

            if (!string.IsNullOrEmpty(_username) && !string.IsNullOrEmpty(_password))
            {
                httpRequest.Headers.Add(
                    "Authorization", 
                    "Basic " + Convert.ToBase64String(Encoding.ASCII.GetBytes(_username + ":" + _password))
                );
            }

            if (!string.IsNullOrEmpty(request.Body))
            {
                httpRequest.ContentType = "application/json; charset=utf-8";

                var data = Encoding.UTF8.GetBytes(request.Body);

                using (var stream = httpRequest.GetRequestStream())
                {
                    stream.Write(data, 0, data.Length);
                    stream.Flush();
                    stream.Close();
                }
            }
            else
            {
                httpRequest.ContentLength = 0;
            }

            var response = new Response();

            try
            {
                using (var httpResponse = (HttpWebResponse)httpRequest.GetResponse())
                using (var responseStream = httpResponse.GetResponseStream())
                using (var reader = new StreamReader(responseStream))
                {
                    response.StatusCode = (int)httpResponse.StatusCode;
                    response.Headers = httpResponse.Headers;
                    response.Body = reader.ReadToEnd();

                    reader.Close();
                    responseStream.Close();
                }

                response.GetBodyDataType();
            }
            catch (WebException webException)
            {
                if ((webException.Status == WebExceptionStatus.ProtocolError) && 
                    (webException.Response != null))
                {
                    using (var exceptionHttpResponse = (HttpWebResponse)webException.Response)
                    {
                        response.StatusCode = (int)exceptionHttpResponse.StatusCode;

                        if (exceptionHttpResponse.Headers.Count > 0)
                        {
                            response.Headers = exceptionHttpResponse.Headers;
                        }

                        if (exceptionHttpResponse.ContentLength > 0)
                        {
                            using (var exceptionResponseStream = exceptionHttpResponse.GetResponseStream())
                            using (var exceptionReader = new StreamReader(exceptionResponseStream))
                            {
                                response.Body = exceptionReader.ReadToEnd();

                                exceptionReader.Close();
                                exceptionResponseStream.Close();
                            }
                            
                            response.GetBodyDataType();
                        }
                    }

                    response.Error = new AEerror();
                    response.Error.Exception = webException;

                    if (response.BodyType == BodyType.Document)
                    {
                        var body = response.ParseBody<Body<object>>();
                        
                        if ((body != null) && body.Error)
                        {
                            response.Error.StatusCode = body.Code;
                            response.Error.Number = body.ErrorNum;
                            response.Error.Message = "ArangoDB error: " + body.ErrorMessage;
                        }
                    }
                    
                    if (string.IsNullOrEmpty(response.Error.Message))
                    {
                        response.Error.StatusCode = response.StatusCode;
                        response.Error.Number = 0;
                        response.Error.Message = "Protocol error: " + webException.Message;
                    }
                }
                else
                {
                    throw;
                }
            }

            return response;
        }
    }
}
