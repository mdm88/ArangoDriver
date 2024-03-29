﻿using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using ArangoDriver.Client.Query;
using ArangoDriver.Exceptions;
using ArangoDriver.Protocol;
using ArangoDriver.Protocol.Responses;
using ArangoDriver.Serialization;

namespace ArangoDriver.Client
{
    public class ADatabase
    {
        private readonly RequestFactory _requestFactory;
        private readonly AConnection _connection;
        private readonly string _databaseName;
        private readonly IJsonSerializer _jsonSerializer;

        /// <summary>
        /// Provides access to AQL user function management operations in current database context.
        /// </summary>
        public AFunction Function => new AFunction(_requestFactory, this);

        /// <summary>
        /// Provides access to query operations in current database context.
        /// </summary>
        public AQuery Query => new AQuery(_requestFactory, this);

        /// <summary>
        /// Provides access to transaction operations in current database context.
        /// </summary>
        public ATransaction Transaction => new ATransaction(_requestFactory, this);

        /// <summary>
        /// Provides access to foxx services in current database context.
        /// </summary>
        public AFoxx Foxx => new AFoxx(_requestFactory, this);
        
        /// <summary>
        /// Provides access to foxx administration in current database context.
        /// </summary>
        public AFoxxAdmin FoxxAdmin => new AFoxxAdmin(_requestFactory, this);

        /// <summary>
        /// Initializes new database context to perform operations on remote database identified by specified alias.
        /// </summary>
        internal ADatabase(RequestFactory requestFactory, AConnection connection, string database, IJsonSerializer jsonSerializer)
        {
            _requestFactory = requestFactory;
            _connection = connection;
            _databaseName = database;
            _jsonSerializer = jsonSerializer;
        }
        
        #region Database
        
        /// <summary>
        /// Retrieves information about currently connected database.
        /// </summary>
        public async Task<AResult<Dictionary<string, object>>> GetCurrent()
        {
            var request = _requestFactory.Create(HttpMethod.Get, ApiBaseUri.Database, "/current");
            
            var response = await Send(request);
            var result = new AResult<Dictionary<string, object>>(response);
            
            switch (response.StatusCode)
            {
                case 200:
                    var body = response.ParseBody<Body<Dictionary<string, object>>>();
                    
                    result.Success = (body != null);
                    result.Value = body?.Result;
                    break;
                case 404:
                    throw new DatabaseNotFoundException();
                default:
                    throw new ArangoException(response.Body);
            }
            
            return result;
        }
        
        #endregion
        
        #region Collections

        public CollectionBuilder CreateCollection(string name)
        {
            return new CollectionBuilder(_requestFactory, this, name);
        }

        public ACollection<T> GetCollection<T>(string name) where T : class
        {
            return new ACollection<T>(_requestFactory, this, name, _jsonSerializer);
        }
        
        /// <summary>
        /// Retrieves information about collections in current database connection.
        /// </summary>
        public async Task<AResult<List<Dictionary<string, object>>>> GetAllCollections()
        {
            var request = _requestFactory.Create(HttpMethod.Get, ApiBaseUri.Collection, "");
            
            var response = await Send(request);
            var result = new AResult<List<Dictionary<string, object>>>(response);
            
            switch (response.StatusCode)
            {
                case 200:
                    var body = response.ParseBody<CollectionsGetAllResponse>();
                    
                    result.Success = (body != null);
                    result.Value = body?.Result;
                    break;
                default:
                    throw new ArangoException(response.Body);
            }
            
            return result;
        }
        
        /// <summary>
        /// Deletes specified collection.
        /// </summary>
        public async Task<AResult<Dictionary<string, object>>> DropCollection(string collectionName)
        {
            var request = _requestFactory.Create(HttpMethod.Delete, ApiBaseUri.Collection, "/" + collectionName);

            // optional
            //request.TrySetQueryStringParameter(ParameterName.IsSystem, _parameters);

            var response = await Send(request);
            var result = new AResult<Dictionary<string, object>>(response);
            
            switch (response.StatusCode)
            {
                case 200:
                    var body = response.ParseBody<Dictionary<string, object>>();
                    
                    result.Success = (body != null);
                    result.Value = body;
                    break;
                case 404:
                    throw new CollectionNotFoundException();
                default:
                    throw new ArangoException(response.Body);
            }
            
            return result;
        }

        #endregion
        
        internal Task<Response> Send(Request request)
        {
            return _connection.Send(_databaseName, request);
        }
        
        internal Task<HttpResponseMessage> Request(Request request)
        {
            return _connection.Request(request, "_db/" + _databaseName + "/");
        }
        internal Task<AResult<T>> RequestQuery<T>(Request request)
        {
            return _connection.RequestQuery<T>(request, "_db/" + _databaseName + "/");
        }
        internal Task<AResult<object>> RequestExecute(Request request)
        {
            return _connection.RequestExecute(request, "_db/" + _databaseName + "/");
        }
    }
}
