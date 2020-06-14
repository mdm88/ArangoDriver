using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using ArangoDriver.Exceptions;
using ArangoDriver.Protocol;
using ArangoDriver.Protocol.Responses;

namespace ArangoDriver.Client
{
    public class DocumentReplace<T> where T : class
    {
        private readonly RequestFactory _requestFactory;
        private readonly ACollection<T> _collection;

        private bool? _waitForSync;
        private bool? _ignoreRevs;
        private bool? _returnNew;
        private bool? _returnOld;
        private string _ifMatch;
        
        #region Parameters
        
        /// <summary>
        /// Determines whether or not to wait until data are synchronised to disk. Default value: false.
        /// </summary>
        public DocumentReplace<T> WaitForSync(bool value)
        {
            _waitForSync = value;
        	
            return this;
        }

        /// <summary>
        /// Determines whether to '_rev' field in the given document is ignored. If this is set to false, then the '_rev' attribute given in the body document is taken as a precondition. The document is only replaced if the current revision is the one specified.
        /// </summary>
        public DocumentReplace<T> IgnoreRevs(bool value)
        {
            _ignoreRevs = value;

            return this;
        }

        /// <summary>
        /// Determines whether to return additionally the complete new document under the attribute 'new' in the result.
        /// </summary>
        public DocumentReplace<T> ReturnNew()
        {
            _returnNew = true;

            return this;
        }

        /// <summary>
        /// Determines whether to return additionally the complete previous revision of the changed document under the attribute 'old' in the result.
        /// </summary>
        public DocumentReplace<T> ReturnOld()
        {
            _returnOld = true;

            return this;
        }

        /// <summary>
        /// Conditionally operate on document with specified revision.
        /// </summary>
        public DocumentReplace<T> IfMatch(string revision)
        {
            _ifMatch = "\"" + revision + "\"";
        	
            return this;
        }

        #endregion

        internal DocumentReplace(RequestFactory requestFactory, ACollection<T> collection)
        {
            _requestFactory = requestFactory;
            _collection = collection;
        }
        
        #region Document
        
        /// <summary>
        /// Completely replaces existing document identified by its handle with new document data.
        /// </summary>
        /// <exception cref="ArgumentException">Specified id value has invalid format.</exception>
        public async Task<AResult<T>> DocumentById(string id, T document)
        {
            if (!Helpers.IsID(id))
            {
                throw new ArgumentException("Specified 'id' value (" + id + ") has invalid format.");
            }
            
            var request = _requestFactory.Create(HttpMethod.Put, ApiBaseUri.Document, "/" + id);
            
            if (_waitForSync.HasValue)
                request.QueryString.Add(ParameterName.WaitForSync, _waitForSync.Value.ToString().ToLower());
            if (_ignoreRevs.HasValue)
                request.QueryString.Add(ParameterName.IgnoreRevs, _ignoreRevs.Value.ToString().ToLower());
            if (_returnNew.HasValue)
                request.QueryString.Add(ParameterName.ReturnNew, _returnNew.Value.ToString().ToLower());
            if (_returnOld.HasValue)
                request.QueryString.Add(ParameterName.ReturnOld, _returnOld.Value.ToString().ToLower());
            if (!String.IsNullOrEmpty(_ifMatch))
                request.Headers.Add(ParameterName.IfMatch, _ifMatch);
            
            request.SetBody(document);
            
            var response = await _collection.Send(request);
            var result = new AResult<T>(response);
            
            switch (response.StatusCode)
            {
                case 201:
                case 202:
                    if (_returnNew.HasValue && _returnNew.Value || _returnOld.HasValue && _returnOld.Value)
                    {
                        T body;
                        if (_returnNew.HasValue && _returnNew.Value)
                            body = response.ParseBody<DocumentCreateResponse<T>>()?.New;
                        else
                            body = response.ParseBody<DocumentCreateResponse<T>>()?.Old;
                    
                        result.Success = body != null;
                        result.Value = body;
                    }
                    else
                    {
                        result.Success = true;
                        result.Value = null;
                    }
                    break;
                case 412:
                    var rev = (string)response.ParseBody<Dictionary<string, object>>()["_rev"];
                    
                    throw new VersionCheckViolationException(rev);
                case 404:
                    throw new CollectionNotFoundException();
                default:
                    throw new ArangoException(response.Body);
            }
            
            return result;
        }

        public Task<AResult<T>> DocumentByKey(string key, T document)
        {
            return DocumentById(_collection.Name + "/" + key, document);
        }
        
        /// <summary>
        /// Completely replaces existing document identified by its handle with new document data.
        /// </summary>
        /// <exception cref="ArgumentException">Specified id value has invalid format.</exception>
        public async Task<AResult<List<T>>> Documents(IEnumerable<T> document)
        {
            // TODO validate
            /*if (!ADocument.IsID(id))
            {
                throw new ArgumentException("Specified 'id' value (" + id + ") has invalid format.");
            }*/
            
            var request = _requestFactory.Create(HttpMethod.Put, ApiBaseUri.Document, "/" + _collection.Name);
            
            if (_waitForSync.HasValue)
                request.QueryString.Add(ParameterName.WaitForSync, _waitForSync.Value.ToString().ToLower());
            if (_ignoreRevs.HasValue)
                request.QueryString.Add(ParameterName.IgnoreRevs, _ignoreRevs.Value.ToString().ToLower());
            if (_returnNew.HasValue)
                request.QueryString.Add(ParameterName.ReturnNew, _returnNew.Value.ToString().ToLower());
            if (_returnOld.HasValue)
                request.QueryString.Add(ParameterName.ReturnOld, _returnOld.Value.ToString().ToLower());
            if (!String.IsNullOrEmpty(_ifMatch))
                request.Headers.Add(ParameterName.IfMatch, _ifMatch);
            
            request.SetBody(document);
            
            var response = await _collection.Send(request);
            var result = new AResult<List<T>>(response);
            
            switch (response.StatusCode)
            {
                case 201:
                case 202:
                    response.Headers.TryGetValues("X-Arango-Error-Codes", out var values);
                    if (values != null && values.Any())
                        throw new MultipleException();
                    
                    List<T> body;
                    if (_returnNew.HasValue && _returnNew.Value)
                        body = response.ParseBody<List<DocumentCreateResponse<T>>>().Select(e => e.New).ToList();
                    else if (_returnOld.HasValue && _returnOld.Value)
                        body = response.ParseBody<List<DocumentCreateResponse<T>>>().Select(e => e.Old).ToList();
                    else
                        body = response.ParseBody<List<T>>();
                    
                    result.Success = (body != null);
                    result.Value = body;
                    break;
                case 404:
                    throw new CollectionNotFoundException();
                default:
                    throw new ArangoException(response.Body);
            }
            
            return result;
        }

        #endregion

        #region Edge

        /// <summary>
        /// Completely replaces existing edge identified by its handle with new edge data.
        /// </summary>
        /// <exception cref="ArgumentException">Specified document does not contain '_from' and '_to' fields.</exception>
        public Task<AResult<T>> EdgeById(string id, T document)
        {
            // TODO validate
            /*if (!document.Has("_from") && !document.Has("_to"))
            {
                throw new ArgumentException("Specified document does not contain '_from' and '_to' fields.");
            }*/

            return DocumentById(id, document);
        }

        public Task<AResult<T>> EdgeByKey(string key, T document)
        {
            return EdgeById(_collection.Name + "/" + key, document);
        }

        #endregion

    }
}