using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using ArangoDriver.Protocol;
using ArangoDriver.Protocol.Responses;

namespace ArangoDriver.Client
{
    public class AIndex<T> where T : class
    {
        private readonly RequestFactory _requestFactory;
        readonly ACollection<T> _collection;
        
        internal AIndex(RequestFactory requestFactory, ACollection<T> collection)
        {
            _requestFactory = requestFactory;
            _collection = collection;
        }

        #region Actions
        
        public IndexBuilder<T> New(AIndexType type)
        {
            return new IndexBuilder<T>(_requestFactory, _collection, type);
        }
        
        /// <summary>
        /// Retrieves specified index.
        /// </summary>
        /// <exception cref="ArgumentException">Specified id value has invalid format.</exception>
        public async Task<AResult<Dictionary<string, object>>> Get(string id)
        {
            if (!Helpers.IsID(id))
            {
                throw new ArgumentException("Specified id value (" + id + ") has invalid format.");
            }
            
            var request = _requestFactory.Create(HttpMethod.Get, ApiBaseUri.Index, "/" + id);
            
            var response = await _collection.Send(request);
            var result = new AResult<Dictionary<string, object>>(response);
            
            switch (response.StatusCode)
            {
                case 200:
                    var body = response.ParseBody<IndexCreateResponse>();
                    
                    result.Success = (body != null);
                    result.Value = new Dictionary<string, object>()
                    {
                        {"id", body.Id},
                        {"name", body.Name},
                        {"type", body.Type},
                        {"fields", body.Fields},
                        {"minLength", body.MinLength},
                        {"geoJson", body.GeoJson},
                        {"sparse", body.Sparse},
                        {"unique", body.Unique},
                        {"isNewlyCreated", body.IsNewlyCreated},
                        {"selectivityEstimate", body.SelectivityEstimate}
                    };
                    break;
                case 404:
                default:
                    // Arango error
                    break;
            }
            
            return result;
        }
        
        /// <summary>
        /// Deletes specified index.
        /// </summary>
        /// <exception cref="ArgumentException">Specified id value has invalid format.</exception>
        public async Task<AResult<Dictionary<string, object>>> Delete(string id)
        {
            if (!Helpers.IsID(id))
            {
                throw new ArgumentException("Specified id value (" + id + ") has invalid format.");
            }
            
            var request = _requestFactory.Create(HttpMethod.Delete, ApiBaseUri.Index, "/" + id);
            
            var response = await _collection.Send(request);
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
    }
}
