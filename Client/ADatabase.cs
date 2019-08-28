using System.Collections.Generic;
using ArangoDriver.External.dictator;
using ArangoDriver.Protocol;

namespace ArangoDriver.Client
{
    public class ADatabase
    {
        private readonly AConnection _connection;
        private readonly string _databaseName;
        
        /// <summary>
        /// Provides access to document operations in current database context.
        /// </summary>
        public ADocument Document
        {
            get
            {
                return new ADocument(this);
            }
        }

        /// <summary>
        /// Provides access to AQL user function management operations in current database context.
        /// </summary>
        public AFunction Function
        {
            get
            {
                return new AFunction(this);
            }
        }
        
        /// <summary>
        /// Provides access to index operations in current database context.
        /// </summary>
        public AIndex Index
        {
            get
            {
                return new AIndex(this);
            }
        }
        
        /// <summary>
        /// Provides access to query operations in current database context.
        /// </summary>
        public AQuery Query
        {
            get
            {
                return new AQuery(this);
            }
        }
        
        /// <summary>
        /// Provides access to transaction operations in current database context.
        /// </summary>
        public ATransaction Transaction
        {
            get
            {
                return new ATransaction(this);
            }
        }

        /// <summary>
        /// Provides access to foxx services in current database context.
        /// </summary>
        public AFoxx Foxx
        {
            get
            {
                return new AFoxx(this);
            }
        }

        /// <summary>
        /// Initializes new database context to perform operations on remote database identified by specified alias.
        /// </summary>
        public ADatabase(AConnection connection, string database)
        {
            _connection = connection;
            _databaseName = database;
        }
        
        #region Database
        
        /// <summary>
        /// Retrieves information about currently connected database.
        /// </summary>
        public AResult<Dictionary<string, object>> GetCurrent()
        {
            var request = new Request(HttpMethod.GET, ApiBaseUri.Database, "/current");
            
            var response = Send(request);
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
            return new CollectionBuilder(this, name);
        }

        public ACollection GetCollection(string name)
        {
            return new ACollection(this, name);
        }
        
        /// <summary>
        /// Retrieves information about collections in current database connection.
        /// </summary>
        public AResult<List<Dictionary<string, object>>> GetAllCollections()
        {
            var request = new Request(HttpMethod.GET, ApiBaseUri.Collection, "");
            
            var response = Send(request);
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
        
        #endregion
        
        internal Response Send(Request request)
        {
            return _connection.Send(_databaseName, request);
        }
    }
}
