﻿using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using ArangoDriver.External.dictator;
using ArangoDriver.Protocol;

namespace ArangoDriver.Client
{
    public class AFunction
    {
        private readonly RequestFactory _requestFactory;
        readonly Dictionary<string, object> _parameters = new Dictionary<string, object>();
        readonly ADatabase _connection;
        
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
            _parameters.Bool(ParameterName.IsDeterministic, value);
            
            return this;
        }
        
        /// <summary>
        /// Determines optional namespace from which to return all registered AQL user functions.
        /// </summary>
        public AFunction Namespace(string value)
        {
            _parameters.String(ParameterName.Namespace, value);
            
            return this;
        }
        
        /// <summary>
        /// Determines whether the function name is treated as a namespace prefix, and all functions in the specified namespace will be deleted. If set to false, the function name provided in name must be fully qualified, including any namespaces. Default value: false.
        /// </summary>
        public AFunction Group(bool value)
        {
            _parameters.String(ParameterName.Group, value.ToString().ToLower());
            
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
            document.String(ParameterName.Name, name);
            // required
            document.String(ParameterName.Code, code);
            // optional
            Request.TrySetBodyParameter(ParameterName.IsDeterministic, _parameters, document);
            
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
            
            _parameters.Clear();
            
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
            request.TrySetQueryStringParameter(ParameterName.Namespace, _parameters);
            
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
            
            _parameters.Clear();
            
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
            request.TrySetQueryStringParameter(ParameterName.Group, _parameters);
            
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
            
            _parameters.Clear();
            
            return result;
        }
        
        #endregion
    }
}