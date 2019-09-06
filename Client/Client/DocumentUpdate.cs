using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using ArangoDriver.Exceptions;
using ArangoDriver.External.dictator;
using ArangoDriver.Protocol;
using ArangoDriver.Protocol.Responses;

namespace ArangoDriver.Client
{
    public class DocumentUpdate<T> where T : class
    {   
        private readonly RequestFactory _requestFactory;
        private readonly Dictionary<string, object> _parameters = new Dictionary<string, object>();
        private readonly ACollection<T> _collection;

        #region Parameters
        
        /// <summary>
        /// Determines whether or not to wait until data are synchronised to disk. Default value: false.
        /// </summary>
        public DocumentUpdate<T> WaitForSync(bool value)
        {
            // needs to be string value
            _parameters.String(ParameterName.WaitForSync, value.ToString().ToLower());
        	
            return this;
        }

        /// <summary>
        /// Determines whether to keep any attributes from existing document that are contained in the patch document which contains null value. Default value: true.
        /// </summary>
        public DocumentUpdate<T> KeepNull(bool value)
        {
            // needs to be string value
            _parameters.String(ParameterName.KeepNull, value.ToString().ToLower());
        	
            return this;
        }
        
        /// <summary>
        /// Determines whether the value in the patch document will overwrite the existing document's value. Default value: true.
        /// </summary>
        public DocumentUpdate<T> MergeObjects(bool value)
        {
            // needs to be string value
            _parameters.String(ParameterName.MergeObjects, value.ToString().ToLower());
        	
            return this;
        }

        /// <summary>
        /// Determines whether to '_rev' field in the given document is ignored. If this is set to false, then the '_rev' attribute given in the body document is taken as a precondition. The document is only replaced if the current revision is the one specified.
        /// </summary>
        public DocumentUpdate<T> IgnoreRevs(bool value)
        {
            // needs to be string value
            _parameters.String(ParameterName.IgnoreRevs, value.ToString().ToLower());

            return this;
        }

        /// <summary>
        /// Determines whether to return additionally the complete new document under the attribute 'new' in the result.
        /// </summary>
        public DocumentUpdate<T> ReturnNew()
        {
            // needs to be string value
            _parameters.String(ParameterName.ReturnNew, true.ToString().ToLower());

            return this;
        }

        /// <summary>
        /// Determines whether to return additionally the complete previous revision of the changed document under the attribute 'old' in the result.
        /// </summary>
        public DocumentUpdate<T> ReturnOld()
        {
            // needs to be string value
            _parameters.String(ParameterName.ReturnOld, true.ToString().ToLower());

            return this;
        }

        /// <summary>
        /// Conditionally operate on document with specified revision.
        /// </summary>
        public DocumentUpdate<T> IfMatch(string revision)
        {
            _parameters.String(ParameterName.IfMatch, revision);
        	
            return this;
        }

        #endregion

        internal DocumentUpdate(RequestFactory requestFactory, ACollection<T> collection)
        {
            _requestFactory = requestFactory;
            _collection = collection;
        }
        
        #region Update

        /// <summary>
        /// Updates existing document identified by its handle with new document data.
        /// </summary>
        public async Task<AResult<T>> DocumentById(string id, T document)
        {
            if (!ADocument.IsID(id))
            {
                throw new ArgumentException("Specified 'id' value (" + id + ") has invalid format.");
            }
            
            var request = _requestFactory.Create(HttpMethod.Patch, ApiBaseUri.Document, "/" + id);
            
            // optional
            request.TrySetQueryStringParameter(ParameterName.WaitForSync, _parameters);
            // optional
            request.TrySetQueryStringParameter(ParameterName.KeepNull, _parameters);
            // optional
            request.TrySetQueryStringParameter(ParameterName.MergeObjects, _parameters);
            // optional
            request.TrySetQueryStringParameter(ParameterName.IgnoreRevs, _parameters);
            // optional
            request.TrySetQueryStringParameter(ParameterName.ReturnNew, _parameters);
            // optional
            request.TrySetQueryStringParameter(ParameterName.ReturnOld, _parameters);
            // optional
            request.TrySetHeaderParameter(ParameterName.IfMatch, _parameters);

            request.SetBody(document);
            
            var response = await _collection.Send(request);
            var result = new AResult<T>(response);

            switch (response.StatusCode)
            {
                case 201:
                case 202:
                    T body;
                    if (_parameters.ContainsKey(ParameterName.ReturnNew) && (string)_parameters[ParameterName.ReturnNew] == "true")
                        body = response.ParseBody<DocumentCreateResponse<T>>()?.New;
                    else if (_parameters.ContainsKey(ParameterName.ReturnOld) && (string)_parameters[ParameterName.ReturnOld] == "true")
                        body = response.ParseBody<DocumentCreateResponse<T>>()?.Old;
                    else
                        body = response.ParseBody<T>();
                    
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
            
            _parameters.Clear();
            
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
            
            var request = _requestFactory.Create(HttpMethod.Patch, ApiBaseUri.Document, "/" + _collection.Name);
            
            // optional
            request.TrySetQueryStringParameter(ParameterName.WaitForSync, _parameters);
            // optional
            request.TrySetQueryStringParameter(ParameterName.KeepNull, _parameters);
            // optional
            request.TrySetQueryStringParameter(ParameterName.MergeObjects, _parameters);
            // optional
            request.TrySetQueryStringParameter(ParameterName.IgnoreRevs, _parameters);
            // optional
            request.TrySetQueryStringParameter(ParameterName.ReturnNew, _parameters);
            // optional
            request.TrySetQueryStringParameter(ParameterName.ReturnOld, _parameters);
            // optional
            request.TrySetHeaderParameter(ParameterName.IfMatch, _parameters);
            
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
                    if (_parameters.ContainsKey(ParameterName.ReturnNew) && (string)_parameters[ParameterName.ReturnNew] == "true")
                        body = response.ParseBody<List<DocumentCreateResponse<T>>>().Select(e => e.New).ToList();
                    else if (_parameters.ContainsKey(ParameterName.ReturnOld) && (string)_parameters[ParameterName.ReturnOld] == "true")
                        body = response.ParseBody<List<DocumentCreateResponse<T>>>().Select(e => e.Old).ToList();
                    else
                        body = response.ParseBody<List<T>>();
                    
                    result.Success = (body != null);
                    result.Value = body;
                    break;
                case 404:
                    throw new CollectionNotFoundException();
                default:
                    throw new ArangoException();
            }
            
            _parameters.Clear();
            
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