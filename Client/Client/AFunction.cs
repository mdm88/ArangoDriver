using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using ArangoDriver.Protocol;

namespace ArangoDriver.Client
{
    public class AFunction
    {
        private readonly RequestFactory _requestFactory;
        readonly ADatabase _connection;

        private bool? _isDeterministic;
        private string _namespace;
        private bool? _group;
        
        internal AFunction(RequestFactory requestFactory, ADatabase connection)
        {
            _requestFactory = requestFactory;
            _connection = connection;
        }
        
        #region Parameters
        
        /// <summary>
        /// Determines whether function return value solely depends on the input value and return value is the same for repeated calls with same input. This parameter is currently not applicable and may be used in the future for optimisation purpose.
        /// </summary>
        public AFunction IsDeterministic(bool value)
        {
            _isDeterministic = value;
            
            return this;
        }
        
        /// <summary>
        /// Determines optional namespace from which to return all registered AQL user functions.
        /// </summary>
        public AFunction Namespace(string value)
        {
            _namespace = value;
            
            return this;
        }
        
        /// <summary>
        /// Determines whether the function name is treated as a namespace prefix, and all functions in the specified namespace will be deleted. If set to false, the function name provided in name must be fully qualified, including any namespaces. Default value: false.
        /// </summary>
        public AFunction Group(bool value)
        {
            _group = value;
            
            return this;
        }
        
        #endregion
        
        #region Register (POST)

        /// <summary>
        /// Creates new or replaces existing AQL user function with specified name and code.
        /// </summary>
        public async Task<AResult<bool>> Register(string name, string code)
        {
            var request = _requestFactory.Create(HttpMethod.Post, ApiBaseUri.AqlFunction, "");
            var document = new Dictionary<string, object>();
            
            // required
            document.Add(ParameterName.Name, name);
            // required
            document.Add(ParameterName.Code, code);
            // optional
            if (_isDeterministic.HasValue)
                document.Add(ParameterName.IsDeterministic, _isDeterministic.Value.ToString().ToLower());
            
            request.SetBody(document);
            
            var response = await _connection.Send(request);
            var result = new AResult<bool>(response);
            
            switch (response.StatusCode)
            {
                case 200:
                case 201:
                    result.Success = true;
                    result.Value = true;
                    break;
                case 400:
                default:
                    // Arango error
                    break;
            }
            
            return result;
        }
        
        #endregion
        
        #region List (GET)

        /// <summary>
        /// Retrieves list of registered AQL user functions.
        /// </summary>
        public async Task<AResult<List<Dictionary<string, object>>>> List()
        {
            var request = _requestFactory.Create(HttpMethod.Get, ApiBaseUri.AqlFunction, "");
            
            // optional
            if (!string.IsNullOrEmpty(_namespace))
                request.QueryString.Add(ParameterName.Namespace, _namespace);
            
            var response = await _connection.Send(request);
            var result = new AResult<List<Dictionary<string, object>>>(response);
            
            switch (response.StatusCode)
            {
                case 200:
                    //var body = ((IEnumerable)response.ParseBody()).Cast<Dictionary<string, object>>().ToList();
                    var body = response.ParseBody<List<Dictionary<string, object>>>();
                    
                    result.Success = (body != null);
                    result.Value = body;
                    break;
                default:
                    // Arango error
                    break;
            }
            
            return result;
        }
        
        #endregion
        
        #region Unregister (DELETE)

        /// <summary>
        /// Unregisters specified AQL user function.
        /// </summary>
        public async Task<AResult<bool>> Unregister(string name)
        {
            var request = _requestFactory.Create(HttpMethod.Delete, ApiBaseUri.AqlFunction, "/" + name);
            
            // optional
            if (_group.HasValue)
                request.QueryString.Add(ParameterName.Group, _group.Value.ToString().ToLower());
            
            var response = await _connection.Send(request);
            var result = new AResult<bool>(response);
            
            switch (response.StatusCode)
            {
                case 200:
                    result.Success = true;
                    result.Value = true;
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
    }
}
