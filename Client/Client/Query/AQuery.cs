using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using ArangoDriver.Client.Query.Filter;
using ArangoDriver.Client.Query.Query;
using ArangoDriver.Client.Query.Return;
using ArangoDriver.Client.Query.Update;
using ArangoDriver.Client.Query.Value;
using ArangoDriver.Exceptions;
using ArangoDriver.Expressions;
using ArangoDriver.Protocol;
using ArangoDriver.Protocol.Requests;
using ArangoDriver.Protocol.Responses;
using AqlFilter = ArangoDriver.Client.Query.Query.AqlFilter;

namespace ArangoDriver.Client.Query
{
    public class AQuery
    {
        private readonly RequestFactory _requestFactory;
        private readonly ADatabase _connection;
        private readonly StringBuilder _query = new StringBuilder();
        private readonly Dictionary<string, object> _bindVars = new Dictionary<string, object>();

        private bool? _count;
        private int? _ttl;
        private int? _batchSize;

        private string _cursorId;
        
        internal AQuery(RequestFactory requestFactory, ADatabase connection)
        {
            _requestFactory = requestFactory;
            _connection = connection;
        }
        
        #region Parameters
        
        /// <summary>
        /// Determines whether the number of retrieved documents should be returned in `Extra` property of `AResult` instance. Default value: false.
        /// </summary>
        public AQuery Count(bool value)
        {
            _count = value;
        	
        	return this;
        }
        
        /// <summary>
        /// Determines the time-to-live for the cursor in seconds. Default value: 30 seconds.
        /// </summary>
        public AQuery Ttl(int value)
        {
            _ttl = value;
        	
        	return this;
        }
        
        /// <summary>
        /// Determines maximum number of result documents to be transferred from the server to the client in one roundtrip. If not set this value is server-controlled.
        /// </summary>
        public AQuery BatchSize(int value)
        {
            _batchSize = value;
        	
        	return this;
        }
        
        #endregion
        
        #region QueryBuilder
        
        private readonly List<IAqlQuery> _queries = new List<IAqlQuery>();

        public AQuery For(string alias, IAqlValue collection)
        {
            _queries.Add(new AqlFor(alias, collection));

            return this;
        }
        
        public AQuery Filter(IAqlFilter filter)
        {
            _queries.Add(new AqlFilter(filter));

            return this;
        }

        public AQuery Insert(string collectionName, IAqlValue definition, AqlInsert.Options options = null)
        {
            _queries.Add(new AqlInsert(collectionName, definition, options));
                    
            return this;
        }

        public AQuery Update<T>(string alias, string collectionName, Action<UpdateBuilder<T>> build, bool mergeObjects = true)
        {
            var definition = new UpdateBuilder<T>(alias);
            
            build.Invoke(definition);
            
            _queries.Add(new AqlUpdate<T>(alias, collectionName, definition, mergeObjects));
            
            return this;
        }

        public AQuery Update<T>(string alias, string collectionName, UpdateBuilder<T> definition, bool mergeObjects = true)
        {
            _queries.Add(new AqlUpdate<T>(alias, collectionName, definition, mergeObjects));
            
            return this;
        }

        public AQuery Upsert<T>(string collectionName, IAqlValue search, IAqlValue insert, Action<UpdateBuilder<T>> build)
        {
            var update = new UpdateBuilder<T>();
            
            build.Invoke(update);
            
            _queries.Add(new AqlUpsert<T>(collectionName, search, insert, update));

            return this;
        }

        public AQuery Upsert<T>(string collectionName, IAqlValue search, IAqlValue insert, UpdateBuilder<T> update)
        {
            _queries.Add(new AqlUpsert<T>(collectionName, search, insert, update));

            return this;
        }

        public AQuery Remove(string alias, string collectionName)
        {
            _queries.Add(new AqlRemove(alias, collectionName));
            
            return this;
        }

        public AQuery Collect()
        {
            _queries.Add(new AqlCollect());

            return this;
        }

        public AQuery Collect(string alias, IAqlValue value)
        {
            _queries.Add(new AqlCollect(alias, value));
            
            return this;
        }

        public AQuery CollectCount(string alias)
        {
            _queries.Add(new AqlCollectCount(alias));
            
            return this;
        }

        public AQuery Aggregate(string alias, IAqlValue value)
        {
            _queries.Add(new AqlAggregate(alias, value));

            return this;
        }

        public AQuery Limit(int quantity)
        {
            _queries.Add(new AqlLimit(quantity));

            return this;
        }

        public AQuery Sort(IAqlValue field, AqlSort.Direction direction)
        {
            _queries.Add(new AqlSort(field, direction));

            return this;
        }

        public AQuery Let(string alias, IAqlValue value)
        {
            _queries.Add(new AqlLet(alias, value));

            return this;
        }
        
        public AQuery Return(IAqlReturn returned)
        {
            _queries.Add(new AqlReturn(returned.GetExpression()));
            
            return this;
        }

        public AQuery Raw(string expression)
        {
            _queries.Add(new AqlQueryRaw(expression));

            return this;
        }

        public string GetExpression()
        {
            int bindCount = 0;

            return GetExpression(ref bindCount);
        }
        public string GetExpression(ref int bindCount)
        {
            string expression = "";
            
            foreach (IAqlQuery query in _queries)
            {
                expression += query.GetExpression(ref bindCount) + " ";
            }

            return Minify(expression.Substring(0, expression.Length - 1));
        }
        public object[] GetBindedVars()
        {
            return _queries.SelectMany(x => x.GetBindedVars()).ToArray();
        }
        
        #endregion
        
        #region Operation
        
        /// <summary>
        /// Retrieves all results values as list of objects.
        /// </summary>
        public async Task<AResult<List<T>>> ToList<T>()
        {
            var resultInternal = await Post<T>();

            var result = new AResult<List<T>>()
            {
                StatusCode = resultInternal.StatusCode,
                Success = resultInternal.Success,
                Value = new List<T>(resultInternal.Value.Result)
            };

            while (resultInternal.Value.HasMore)
            {
                resultInternal = await Put<T>(_cursorId);
                
                result.StatusCode = resultInternal.StatusCode;
                result.Success = resultInternal.Success;

                if (resultInternal.Success)
                {
                    result.Value.AddRange(resultInternal.Value.Result);
                }
            }
            
            return result;
        }

        /// <summary>
        /// Retrieves a batch of results as list of objects
        /// </summary>
        /// <typeparam name="T">Object type</typeparam>
        /// <returns>AResult</returns>
        public Task<AResult<Body<List<T>>>> ToListBatch<T>()
        {
            if (string.IsNullOrEmpty(_cursorId))
            {
                return Post<T>();
            }
            else
            {
                return Put<T>(_cursorId);
            }
        }
        
        /// <summary>
        /// Retrieves first result value as single generic object.
        /// </summary>
        public async Task<AResult<T>> ToObject<T>()
        {
            var listResult = await Post<T>();
            var result = new AResult<T>()
            {
                StatusCode = listResult.StatusCode,
                Success = listResult.Success
            };
            
            if (listResult.Success)
            {
                if (listResult.Value.Result.Count > 0)
                {
                    result.Value = listResult.Value.Result[0];
                }
                else
                {
                    result.Value = default;
                }
            }
            
            return result;
        }
        
        /// <summary>
        /// Retrieves first result value as single object.
        /// </summary>
        public Task<AResult<object>> ToObject()
        {
            return ToObject<object>();
        }

        /// <summary>
        /// Retrieves result which does not contain value. This can be used to execute non-query operations where only success information is relevant.
        /// </summary>
        public async Task<AResult<object>> ExecuteNonQuery()
        {
            var request = _requestFactory.Create(HttpMethod.Post, ApiBaseUri.Cursor, "");
            var document = new QueryRequest()
            {
                Query = GetExpression(),
                Count = _count,
                TTL = _ttl,
                BatchSize = _batchSize
            };

            int i = 0;
            document.BindVars = GetBindedVars().ToDictionary(_ => "var" + i++);
            
            // TODO: options parameter
            
            request.SetBody(document);           
            
            var result = await _connection.RequestExecute(request);

            if (!result.Success)
            {
                switch (result.StatusCode)
                {
                    case 400:
                        throw new QueryInvalidException();
                    case 404:
                        throw new CollectionNotFoundException();
                    default:
                        throw new ArangoException();
                }
            }

            return result;
        }

        /// <summary>
        /// Create cursor and return first batch
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        /// <exception cref="QueryInvalidException"></exception>
        /// <exception cref="CollectionNotFoundException"></exception>
        /// <exception cref="ArangoException"></exception>
        private async Task<AResult<Body<List<T>>>> Post<T>()
        {
            var request = _requestFactory.Create(HttpMethod.Post, ApiBaseUri.Cursor, "");
            var document = new QueryRequest()
            {
                Query = GetExpression(),
                Count = _count,
                TTL = _ttl,
                BatchSize = _batchSize
            };

            int i = 0;
            document.BindVars = GetBindedVars().ToDictionary(_ => "var" + i++);
            
            // TODO: options parameter
            
            request.SetBody(document);           
            
            var result = await _connection.RequestQuery<Body<List<T>>>(request);

            if (!result.Success)
            {
                switch (result.StatusCode)
                {
                    case 400:
                        throw new QueryInvalidException();
                    case 404:
                        throw new CollectionNotFoundException();
                    default:
                        throw new ArangoException();
                }
            }
            
            _cursorId = result.Value.ID;

            return result;
        }
        
        /// <summary>
        /// Return next batch of results
        /// </summary>
        /// <param name="cursorId"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        /// <exception cref="QueryCursorNotFoundException"></exception>
        /// <exception cref="ArangoException"></exception>
        private async Task<AResult<Body<List<T>>>> Put<T>(string cursorId)
        {
            var request = _requestFactory.Create(HttpMethod.Put, ApiBaseUri.Cursor, "/" + cursorId);
            
            var result = await _connection.RequestQuery<Body<List<T>>>(request);

            if (!result.Success)
            {
                switch (result.StatusCode)
                {
                    case 404:
                        throw new QueryCursorNotFoundException();
                    default:
                        throw new ArangoException();
                }
            }
            
            return result;
        }
        
        /// <summary>
        /// Analyzes specified AQL query.
        /// </summary>
        public async Task<AResult<QueryParseResponse>> Parse(string query)
        {
            var request = _requestFactory.Create(HttpMethod.Post, ApiBaseUri.Query, "");
            var document = new Dictionary<string, object>();
            
            // required
            document[ParameterName.Query] = Minify(query);
            
            request.SetBody(document);
            
            var result = await _connection.RequestQuery<QueryParseResponse>(request);

            if (!result.Success)
            {
                switch (result.StatusCode)
                {
                    case 400:
                        throw new QueryInvalidException();
                    default:
                        throw new ArangoException();
                }
            }
            
            return result;
        }
        
        /// <summary>
        /// Deletes AQL query cursor.
        /// </summary>
        public async Task<AResult<bool>> DeleteCursor()
        {
            if (string.IsNullOrEmpty(_cursorId))
                throw new QueryCursorNotFoundException();
            
            var request = _requestFactory.Create(HttpMethod.Delete, ApiBaseUri.Cursor, "/" + _cursorId);
            
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
                case 404:
                    throw new QueryCursorNotFoundException();
                default:
                    throw new ArangoException(response.Body);
            }
            
            return result;
        }
        
        #endregion

        public AQuery SubQuery()
        {
            return _connection.Query;
        }
        
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
            destination.Add("id", source.ID);
            destination.Add("count", source.Count);
            destination.Add("cached", source.Cached);
            destination.Add("hasMore", source.HasMore);
            
            if (source.Extra != null)
            {
                destination.Add("extra", (Dictionary<string, object>)source.Extra);
            }
        }
    }
}
