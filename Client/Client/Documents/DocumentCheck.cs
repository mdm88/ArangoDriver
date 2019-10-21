using System;
using System.Net.Http;
using System.Threading.Tasks;
using ArangoDriver.Exceptions;
using ArangoDriver.Protocol;

namespace ArangoDriver.Client
{
    public class DocumentCheck<T> where T : class
    {
        private readonly RequestFactory _requestFactory;
        private readonly ACollection<T> _collection;

        internal DocumentCheck(RequestFactory requestFactory, ACollection<T> collection)
        {
            _requestFactory = requestFactory;
            _collection = collection;
        }

        /// <summary>
        /// Checks for existence of specified document.
        /// </summary>
        /// <exception cref="ArgumentException">Specified 'id' value has invalid format.</exception>
        public async Task<AResult<string>> ById(string id)
        {
            if (!Helpers.IsID(id))
            {
                throw new ArgumentException("Specified 'id' value (" + id + ") has invalid format.");
            }
            
            var request = _requestFactory.Create(HttpMethod.Head, ApiBaseUri.Document, "/" + id);
            
            // optional
            //request.TrySetHeaderParameter(ParameterName.IfMatch, _parameters);
            // optional: If revision is different -> HTTP 200. If revision is identical -> HTTP 304.
            //request.TrySetHeaderParameter(ParameterName.IfNoneMatch, _parameters);
            
            var response = await _collection.Send(request);
            var result = new AResult<string>(response);
            
            switch (response.StatusCode)
            {
                case 200:
                case 304:
                case 404:
                    if ((response.Headers?.ETag?.Tag ?? "").Trim().Length > 0)
                    {
                        result.Value = response.Headers?.ETag?.Tag?.Replace("\"", "");
                        result.Success = (result.Value != null);
                    }
                    break;
                case 412:
                    throw new VersionCheckViolationException(response.Headers.ETag.Tag?.Replace("\"", ""));
                default:
                    throw new ArangoException();
            }
            
            return result;
        }

        /// <summary>
        /// Checks for existence of specified document by key.
        /// </summary>
        public Task<AResult<string>> ByKey(string key)
        {
            return ById(_collection.Name + "/" + key);
        }
    }
}