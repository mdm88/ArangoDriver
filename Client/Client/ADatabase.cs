using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using ArangoDriver.External.dictator;
using ArangoDriver.Protocol;

namespace ArangoDriver.Client
{
    public class ADatabase
    {
        private readonly RequestFactory _requestFactory;
        private readonly AConnection _connection;
        private readonly string _databaseName;

        /// <summary>
        /// Provides access to AQL user function management operations in current database context.
        /// </summary>
        public AFunction Function
        {
            get
            {
                return new AFunction(_requestFactory, this);
            }
        }
        
        /// <summary>
        /// Provides access to index operations in current database context.
        /// </summary>
        public AIndex Index
        {
            get
            {
                return new AIndex(_requestFactory, this);
            }
        }
        
        /// <summary>
        /// Provides access to query operations in current database context.
        /// </summary>
        public AQuery Query
        {
            get
            {
                return new AQuery(_requestFactory, this);
            }
        }
        
        /// <summary>
        /// Provides access to transaction operations in current database context.
        /// </summary>
        public ATransaction Transaction
        {
            get
            {
                return new ATransaction(_requestFactory, this);
            }
        }

        /// <summary>
        /// Provides access to foxx services in current database context.
        /// </summary>
        public AFoxx Foxx
        {
            get
            {
                return new AFoxx(_requestFactory, this);
            }
        }

        /// <summary>
        /// Initializes new database context to perform operations on remote database identified by specified alias.
        /// </summary>
        internal ADatabase(RequestFactory requestFactory, AConnection connection, string database)
        {
            _requestFactory = requestFactory;
            _connection = connection;
            _databaseName = database;
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
                case 400:
                case 404:
                default:
                    // Arango error
                    break;
            }
            
            return result;
        }
        
        #endregion
        
        #region Collections

        public CollectionBuilder CreateCollection(string name)
        {
            return new CollectionBuilder(_requestFactory, this, name);
        }

        public ACollection GetCollection(string name)
        {
            return new ACollection(_requestFactory, this, name);
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
                    var body = response.ParseBody<Dictionary<string, object>>();
                    
                    result.Success = (body != null);
                    result.Value = body.List<Dictionary<string, object>>("result");
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
                case 400:
                case 404:
                default:
                    // Arango error
                    break;
            }
            
            return result;
        }

        #endregion
        
        internal Task<Response> Send(Request request)
        {
            return _connection.Send(_databaseName, request);
        }
    }
}
