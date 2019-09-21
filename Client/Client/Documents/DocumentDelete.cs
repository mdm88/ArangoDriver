using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using ArangoDriver.Exceptions;
using ArangoDriver.Protocol;

namespace ArangoDriver.Client
{
    public class DocumentDelete<T> where T : class
    {
        private readonly RequestFactory _requestFactory;
        private readonly ACollection<T> _collection;

        private bool? _waitForSync;
        private bool? _returnOld;
        private string _ifMatch;
        
        #region Parameters
        
        /// <summary>
        /// Determines whether or not to wait until data are synchronised to disk. Default value: false.
        /// </summary>
        public DocumentDelete<T> WaitForSync(bool value)
        {
            _waitForSync = value;
        	
            return this;
        }

        /// <summary>
        /// Determines whether to return additionally the complete previous revision of the changed document under the attribute 'old' in the result.
        /// </summary>
        public DocumentDelete<T> ReturnOld()
        {
            _returnOld = true;

            return this;
        }

        /// <summary>
        /// Conditionally operate on document with specified revision.
        /// </summary>
        public DocumentDelete<T> IfMatch(string revision)
        {
            _ifMatch = "\"" + revision + "\"";
        	
            return this;
        }

        #endregion

        internal DocumentDelete(RequestFactory requestFactory, ACollection<T> collection)
        {
            _requestFactory = requestFactory;
            _collection = collection;
        }

        #region Delete
        
        /// <summary>
        /// Deletes specified document.
        /// </summary>
        /// <exception cref="ArgumentException">Specified 'id' value has invalid format.</exception>
        public async Task<AResult<Dictionary<string, object>>> ById(string id)
        {
            if (!Helpers.IsID(id))
            {
                throw new ArgumentException("Specified 'id' value (" + id + ") has invalid format.");
            }
            
            var request = _requestFactory.Create(HttpMethod.Delete, ApiBaseUri.Document, "/" + id);
            
            if (_waitForSync.HasValue)
                request.QueryString.Add(ParameterName.WaitForSync, _waitForSync.Value.ToString().ToLower());
            if (_returnOld.HasValue)
                request.QueryString.Add(ParameterName.ReturnOld, _returnOld.Value.ToString().ToLower());
            if (!String.IsNullOrEmpty(_ifMatch))
                request.Headers.Add(ParameterName.IfMatch, _ifMatch);
            
            var response = await _collection.Send(request);
            var result = new AResult<Dictionary<string, object>>(response);
            
            switch (response.StatusCode)
            {
                case 200:
                case 202:
                    var body = response.ParseBody<Dictionary<string, object>>();
                    
                    result.Success = (body != null);
                    result.Value = body;
                    break;
                case 412:
                    var rev = (string)response.ParseBody<Dictionary<string, object>>()["_rev"];
                    
                    throw new VersionCheckViolationException(rev);
                case 404:
                    throw new CollectionNotFoundException();
                default:
                    throw new ArangoException();
            }
            
            return result;
        }

        public Task<AResult<Dictionary<string, object>>> ByKey(string key)
        {
            return ById(_collection.Name + "/" + key);
        }

        #endregion
        
        // TODO falta la version multiple
    }
}