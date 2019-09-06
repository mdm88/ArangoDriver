using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using ArangoDriver.External.dictator;
using ArangoDriver.Protocol;
using ArangoDriver.Protocol.Requests;
using ArangoDriver.Protocol.Responses;

namespace ArangoDriver.Client
{
    public class AQuery
    {
        private readonly RequestFactory _requestFactory;
        readonly Dictionary<string, object> _parameters = new Dictionary<string, object>();
        readonly ADatabase _connection;
        readonly StringBuilder _query = new StringBuilder();
        readonly Dictionary<string, object> _bindVars = new Dictionary<string, object>();
        
        internal AQuery(RequestFactory requestFactory, ADatabase connection)
        {
            _requestFactory = requestFactory;
            _connection = connection;
        }
        
        #region Parameters
        
        /// <summary>
        /// Sets AQL query code.
        /// </summary>
        public AQuery Aql(string query)
        {
        	var cleanQuery = Minify(query);
        	
        	if (_query.Length > 0)
        	{
        		_query.Append(" ");
        	}
        	
        	_query.Append(cleanQuery);
        	
            return this;
        }
        
        /// <summary>
        /// Maps key/value bind parameter to the AQL query.
        /// </summary>
        public AQuery BindVar(string key, object value)
        {
            _bindVars.Object(key, value);
            
            return this;
        }
        
        /// <summary>
        /// Determines whether the number of retrieved documents should be returned in `Extra` property of `AResult` instance. Default value: false.
        /// </summary>
        public AQuery Count(bool value)
        {
        	_parameters.Bool(ParameterName.Count, value);
        	
        	return this;
        }
        
        /// <summary>
        /// Determines whether the number of documents in the result set should be returned. Default value: false.
        /// </summary>
        public AQuery Ttl(int value)
        {
        	_parameters.Int(ParameterName.TTL, value);
        	
        	return this;
        }
        
        /// <summary>
        /// Determines maximum number of result documents to be transferred from the server to the client in one roundtrip. If not set this value is server-controlled.
        /// </summary>
        public AQuery BatchSize(int value)
        {
        	_parameters.Int(ParameterName.BatchSize, value);
        	
        	return this;
        }
        
        #endregion
        
        #region Retrieve list result (POST)
        
        /// <summary>
        /// Retrieves result value as list of objects.
        /// </summary>
        public async Task<AResult<List<T>>> ToList<T>()
        {
            var request = _requestFactory.Create(HttpMethod.Post, ApiBaseUri.Cursor, "");
            var document = new QueryRequest()
            {
                Query = _query.ToString()
            };
            
            // optional
            if (_parameters.Has(ParameterName.Count))
                document.Count = _parameters.Bool(ParameterName.Count);
            // optional
            if (_parameters.Has(ParameterName.BatchSize))
                document.BatchSize = _parameters.Int(ParameterName.BatchSize);
            // optional
            if (_parameters.Has(ParameterName.TTL))
                document.TTL = _parameters.Int(ParameterName.TTL);
            // optional
            if (_bindVars.Count > 0)
                document.BindVars = _bindVars;
            
            // TODO: options parameter
            
            request.SetBody(document);
            //this.LastRequest = request.Body;            
            var response = await _connection.Send(request);
            //this.LastResponse = response.Body;
            var result = new AResult<List<T>>(response);
            
            switch (response.StatusCode)
            {
                case 201:
                    var body = response.ParseBody<Body<List<T>>>();
                    
                    result.Success = (body != null);
                    
                    if (result.Success)
                    {
                        result.Value = new List<T>();
                        result.Value.AddRange(body.Result);
                        result.Extra = new Dictionary<string, object>();
                        
                        CopyExtraBodyFields<List<T>>(body, result.Extra);
                        
                        if (body.HasMore)
                        {
                            var putResult = await Put<T>(body.ID);
                            
                            result.Success = putResult.Success;
                            result.StatusCode = putResult.StatusCode;
                            
                            if (putResult.Success)
                            {
                                result.Value.AddRange(putResult.Value);
                            }
                            else
                            {
                                result.Error = putResult.Error;
                            }
                        }
                    }
                    break;
                case 400:
                case 404:
                case 405:
                default:
                    // Arango error
                    break;
            }
            
            _parameters.Clear();
            _bindVars.Clear();
            _query.Clear();
            
            return result;
        }
        
        #endregion
        
        #region Retrieve single result (POST)
        
        /// <summary>
        /// Retrieves result value as single generic object.
        /// </summary>
        public async Task<AResult<T>> ToObject<T>()
        {
            var listResult = await ToList<T>();
            var result = new AResult<T>();
            
            result.StatusCode = listResult.StatusCode;
            result.Success = listResult.Success;
            result.Extra = listResult.Extra;
            result.Error = listResult.Error;
            
            if (listResult.Success)
            {
                if (listResult.Value.Count > 0)
                {
                    result.Value = listResult.Value[0];
                }
                else
                {
                    result.Value = default;
                }
            }
            
            return result;
        }
        
        /// <summary>
        /// Retrieves result value as single object.
        /// </summary>
        public Task<AResult<object>> ToObject()
        {
            return ToObject<object>();
        }

        #endregion

        #region Retrieve non-query result (POST)

        /// <summary>
        /// Retrieves result which does not contain value. This can be used to execute non-query operations where only success information is relevant.
        /// </summary>
        public async Task<AResult<object>> ExecuteNonQuery()
        {
            var listResult = await ToList<Dictionary<string, object>>();
            var result = new AResult<object>();

            result.StatusCode = listResult.StatusCode;
            result.Success = listResult.Success;
            result.Extra = listResult.Extra;
            result.Error = listResult.Error;
            result.Value = null;

            return result;
        }

        #endregion

        #region More results in cursor (PUT)

        internal async Task<AResult<List<T>>> Put<T>(string cursorID)
        {
            var request = _requestFactory.Create(HttpMethod.Put, ApiBaseUri.Cursor, "/" + cursorID);
            
            var response = await _connection.Send(request);
            var result = new AResult<List<T>>(response);
            
            switch (response.StatusCode)
            {
                case 200:                    
                    var body = response.ParseBody<Body<List<T>>>();
                    
                    result.Success = (body.Result != null);
                    
                    if (result.Success)
                    {
                        result.Value = new List<T>();
                        result.Value.AddRange(body.Result);
                        
                        if (body.HasMore)
                        {
                            var putResult = await Put<T>(body.ID);
                            
                            result.Success = putResult.Success;
                            result.StatusCode = putResult.StatusCode;
                            
                            if (putResult.Success)
                            {
                                result.Value.AddRange(putResult.Value);
                            }
                            else
                            {
                                result.Error = putResult.Error;
                            }
                        }
                    }
                    break;
                case 400:
                case 404:
                default:
                    break;
            }
            
            return result;
        }
        
        #endregion
        
        #region Parse (POST)

        /// <summary>
        /// Analyzes specified AQL query.
        /// </summary>
        public async Task<AResult<Dictionary<string, object>>> Parse(string query)
        {
            var request = _requestFactory.Create(HttpMethod.Post, ApiBaseUri.Query, "");
            var document = new Dictionary<string, object>();
            
            // required
            document.String(ParameterName.Query, Minify(query));
            
            request.SetBody(document);
            
            var response = await _connection.Send(request);
            var result = new AResult<Dictionary<string, object>>(response);
            
            switch (response.StatusCode)
            {
                case 200:
                    var body = response.ParseBody<QueryParseResponse>();
                    
                    result.Success = (body != null);
                    
                    if (result.Success)
                    {
                        result.Value = new Dictionary<string, object>()
                        {
                            {"collections", body.Collections},
                            {"bindVars", body.BindVars},
                            {"ast", body.Ast}
                        };
                    }
                    break;
                case 400:
                default:
                    // Arango error
                    break;
            }
            
            _parameters.Clear();
            _bindVars.Clear();
            _query.Clear();
            
            return result;
        }
        
        #endregion
        
        #region Delete cursor (DELETE)

        // TODO: check docs - https://docs.arangodb.com/HttpAqlQuery/index.html
        // in docs status code is 200 and it isn't clear what is returned in data
        /// <summary>
        /// Deletes specified AQL query cursor.
        /// </summary>
        public async Task<AResult<bool>> DeleteCursor(string cursorID)
        {
            var request = _requestFactory.Create(HttpMethod.Delete, ApiBaseUri.Cursor, "/" + cursorID);
            
            var response = await _connection.Send(request);
            var result = new AResult<bool>(response);
            
            switch (response.StatusCode)
            {
                case 202:
                    if (response.BodyType == BodyType.Document)
                    {
                        result.Success = true;
                        result.Value = true;
                    }
                    break;
                case 400:
                case 404:
                default:
                    // Arango error
                    break;
            }
            
            _parameters.Clear();
            _bindVars.Clear();
            _query.Clear();
            
            return result;
        }
        
        #endregion
        
        /// <summary>
        /// Transforms specified query into minified version with removed leading and trailing whitespaces except new line characters.
        /// </summary>
        public static string Minify(string inputQuery)
        {
        	var query = inputQuery.Replace("\r", "");
        	
        	var cleanQuery = new StringBuilder();

        	var lastAcceptedIndex = 0;
        	var startRejecting = true;
        	var acceptedLength = 0;
        	
        	for (int i = 0; i < query.Length; i++)
        	{
        		if (startRejecting)
        		{
        			if ((query[i] != '\n') && (query[i] != '\t') && (query[i] != ' '))
	        		{
        				
	    				lastAcceptedIndex = i;
	    				startRejecting = false;
	        		}
        		}
    			
        		if (!startRejecting)
    			{
        			if (query[i] == '\n')
	    			{
	    				cleanQuery.Append(query.Substring(lastAcceptedIndex, acceptedLength + 1));
	    				
	    				acceptedLength = 0;
	    				lastAcceptedIndex = i;
	    				startRejecting = true;
	    			}
        			else if (i == (query.Length - 1))
        			{
        				cleanQuery.Append(query.Substring(lastAcceptedIndex, acceptedLength + 1));
        			}
        			else
        			{
        				acceptedLength++;
        			}
    			}
        	}
        	
        	return cleanQuery.ToString();
        }
        
        private void CopyExtraBodyFields<T>(Body<T> source, Dictionary<string, object> destination)
        {
            destination.String("id", source.ID);
            destination.Long("count", source.Count);
            destination.Bool("cached", source.Cached);
            
            if (source.Extra != null)
            {
                destination.Document("extra", (Dictionary<string, object>)source.Extra);
            }
        }
    }
}
