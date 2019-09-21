using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using ArangoDriver.Exceptions;
using ArangoDriver.Protocol;

namespace ArangoDriver.Client
{
    public class ATransaction
    {
        private readonly RequestFactory _requestFactory;
        readonly ADatabase _connection;
        readonly List<string> _readCollections = new List<string>();
        readonly List<string> _writeCollections = new List<string>();
        readonly Dictionary<string, object> _transactionParams = new Dictionary<string, object>();

        private bool? _waitForSync;
        private int? _lockTimeout;
        
        internal ATransaction(RequestFactory requestFactory, ADatabase connection)
        {
            _requestFactory = requestFactory;
            _connection = connection;
        }
        
        #region Parameters
        
        /// <summary>
        /// Maps read collection to current transaction.
        /// </summary>
        public ATransaction ReadCollection(string collectionName)
        {
            _readCollections.Add(collectionName);
        	
        	return this;
        }
        
        /// <summary>
        /// Maps write collection to current transaction.
        /// </summary>
        public ATransaction WriteCollection(string collectionName)
        {
            _writeCollections.Add(collectionName);
        	
        	return this;
        }
        
        /// <summary>
        /// Determines whether or not to wait until data are synchronised to disk. Default value: false.
        /// </summary>
        public ATransaction WaitForSync(bool value)
        {
            _waitForSync = value;
        	
        	return this;
        }
        
        /// <summary>
        /// Determines a numeric value that can be used to set a timeout for waiting on collection locks. Setting value to 0 will make ArangoDB not time out waiting for a lock.
        /// </summary>
        public ATransaction LockTimeout(int value)
        {
            _lockTimeout = value;
        	
        	return this;
        }
        
        /// <summary>
        /// Maps key/value parameter to current transaction.
        /// </summary>
        public ATransaction Param(string key, object value)
        {
            _transactionParams.Add(key, value);
            
            return this;
        }
        
        #endregion
        
        /// <summary>
        /// Executes specified transaction.
        /// </summary>
        public async Task<AResult<T>> Execute<T>(string action)
        {
            var request = _requestFactory.Create(HttpMethod.Post, ApiBaseUri.Transaction, "");
            var document = new Dictionary<string, object>();
            
            // required
            document.Add(ParameterName.Action, action);
            // required
            if (_readCollections.Count > 0)
            {
                document.Add(ParameterName.Collections + ".read", _readCollections);
            }
            // required
            if (_writeCollections.Count > 0)
            {
                document.Add(ParameterName.Collections + ".write", _writeCollections);
            }
            // optional
            if (_waitForSync.HasValue)
                document.Add(ParameterName.WaitForSync, _waitForSync.Value);
            // optional
            if (_lockTimeout.HasValue)
                document.Add(ParameterName.LockTimeout, _lockTimeout.Value);
            // optional
            if (_transactionParams.Count > 0)
            {
                document.Add(ParameterName.Params, _transactionParams);
            }

            request.SetBody(document);
            
            var response = await _connection.Send(request);
            var result = new AResult<T>(response);
            
            switch (response.StatusCode)
            {
                case 200:
                    var body = response.ParseBody<Body<T>>();
                    
                    result.Success = (body != null);
                    result.Value = body.Result;
                    break;
                case 400:
                case 404:
                case 409:
                case 500:
                default:
                    throw new ArangoException();
            }
            
            return result;
        }
    }
}
