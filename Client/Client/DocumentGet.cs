using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using ArangoDriver.Exceptions;
using ArangoDriver.External.dictator;
using ArangoDriver.Protocol;

namespace ArangoDriver.Client
{
    public class DocumentGet<T> where T : class
    {
        private readonly RequestFactory _requestFactory;
        private readonly ACollection<T> _collection;

        internal DocumentGet(RequestFactory requestFactory, ACollection<T> collection)
        {
            _requestFactory = requestFactory;
            _collection = collection;
        }

        /// <summary>
        /// Checks for existence of specified document.
        /// </summary>
        /// <exception cref="ArgumentException">Specified 'id' value has invalid format.</exception>
        public async Task<AResult<T>> ById(string id)
        {
            if (!ADocument.IsID(id))
            {
                throw new ArgumentException("Specified 'id' value (" + id + ") has invalid format.");
            }
            
            var request = _requestFactory.Create(HttpMethod.Get, ApiBaseUri.Document, "/" + id);
            
            // optional
            //request.TrySetHeaderParameter(ParameterName.IfMatch, _parameters);
            // optional: If revision is different -> HTTP 200. If revision is identical -> HTTP 304.
            //request.TrySetHeaderParameter(ParameterName.IfNoneMatch, _parameters);
            
            var response = await _collection.Send(request);
            var result = new AResult<T>(response);
            
            switch (response.StatusCode)
            {
                case 200:
                case 304:
                    var body = response.ParseBody<T>();
                    
                    result.Success = (body != null);
                    result.Value = body;
                    break;
                case 412:
                    var rev = response.ParseBody<Dictionary<string, object>>().String("_rev");
                    
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
    }
}