using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using ArangoDriver.Exceptions;
using ArangoDriver.Protocol;
using ArangoDriver.Serialization;

namespace ArangoDriver.Client
{
    public class DocumentGet<T> where T : class
    {
        private readonly RequestFactory _requestFactory;
        private readonly ACollection<T> _collection;
        private readonly IJsonSerializer _jsonSerializer;

        internal DocumentGet(RequestFactory requestFactory, ACollection<T> collection, IJsonSerializer jsonSerializer)
        {
            _requestFactory = requestFactory;
            _collection = collection;
            _jsonSerializer = jsonSerializer;
        }

        /// <summary>
        /// Checks for existence of specified document.
        /// </summary>
        /// <exception cref="ArgumentException">Specified 'id' value has invalid format.</exception>
        public async Task<AResult<T>> ById(string id)
        {
            if (!Helpers.IsID(id))
            {
                throw new ArgumentException("Specified 'id' value (" + id + ") has invalid format.");
            }
            
            var request = _requestFactory.Create(HttpMethod.Get, ApiBaseUri.Document, "/" + id);
            
            // optional
            //request.TrySetHeaderParameter(ParameterName.IfMatch, _parameters);
            // optional: If revision is different -> HTTP 200. If revision is identical -> HTTP 304.
            //request.TrySetHeaderParameter(ParameterName.IfNoneMatch, _parameters);
            
            using var response = await _collection.Request(request);
            
            var result = new AResult<T>()
            {
                StatusCode = (int) response.StatusCode,
                Success = response.IsSuccessStatusCode
            };

            switch (result.StatusCode)
            {
                case 200:
                case 304:
                    var body = _jsonSerializer.Deserialize<T>(await response.Content.ReadAsStreamAsync());
                    
                    result.Success = (body != null);
                    result.Value = body;
                    break;
                case 412:
                    var rev = (string) _jsonSerializer.Deserialize<Dictionary<string, object>>(await response.Content.ReadAsStreamAsync())["_rev"];
                    
                    throw new VersionCheckViolationException(rev);
                case 404:
                    throw new CollectionNotFoundException();
                default:
                    throw new ArangoException();
            }
            
            return result;
        }

        /// <summary>
        /// Checks for existence of specified document by key.
        /// </summary>
        public Task<AResult<T>> ByKey(string key)
        {
            return ById(_collection.Name + "/" + key);
        }

        /// <summary>
        /// Checks for existence of specified document by key.
        /// </summary>
        public Task<AResult<T>> ByKey(long key)
        {
            return ById(_collection.Name + "/" + key);
        }
    }
}