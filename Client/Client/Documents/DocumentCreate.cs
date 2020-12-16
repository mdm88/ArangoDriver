using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using ArangoDriver.Exceptions;
using ArangoDriver.Protocol;
using ArangoDriver.Protocol.Requests;
using ArangoDriver.Protocol.Responses;

namespace ArangoDriver.Client
{
    public class DocumentCreate<T> where T : class
    {
        private readonly RequestFactory _requestFactory;
        private readonly ACollection<T> _collection;

        private bool? _waitForSync;
        private bool? _returnNew;
        private OverwriteMode? _overwriteMode;
        
        #region Parameters
        
        /// <summary>
        /// Determines whether or not to wait until data are synchronised to disk. Default value: false.
        /// </summary>
        public DocumentCreate<T> WaitForSync(bool value)
        {
            _waitForSync = value;
        	
            return this;
        }

        /// <summary>
        /// Determines whether to return additionally the complete new document under the attribute 'new' in the result.
        /// </summary>
        public DocumentCreate<T> ReturnNew()
        {
            _returnNew = true;

            return this;
        }

        public DocumentCreate<T> OverwriteMode(OverwriteMode mode)
        {
            _overwriteMode = mode;

            return this;
        }
        
        #endregion

        internal DocumentCreate(RequestFactory requestFactory, ACollection<T> collection)
        {
            _requestFactory = requestFactory;
            _collection = collection;
        }

        #region Document
        
        /// <summary>
        /// Creates new document within specified collection in current database context.
        /// </summary>
        public async Task<AResult<T>> Document(T document)
        {
            var request = _requestFactory.Create(HttpMethod.Post, ApiBaseUri.Document, "/" + _collection.Name);
            
            if (_waitForSync.HasValue)
                request.QueryString.Add(ParameterName.WaitForSync, _waitForSync.Value.ToString().ToLower());
            if (_returnNew.HasValue)
                request.QueryString.Add(ParameterName.ReturnNew, _returnNew.Value.ToString().ToLower());
            if (_overwriteMode.HasValue)
                request.QueryString.Add(ParameterName.OverwriteMode, _overwriteMode.Value.ToString().ToLower());

            request.SetBody(document);
            
            var response = await _collection.Send(request);
            var result = new AResult<T>(response);
            
            switch (response.StatusCode)
            {
                case 201:
                case 202:
                    T body;
                    if (_returnNew.HasValue && _returnNew.Value)
                        body = response.ParseBody<DocumentCreateResponse<T>>()?.New;
                    else
                        body = response.ParseBody<T>();

                    result.Success = (body != null);
                    result.Value = body;
                    break;
                case 409:
                    throw new UniqueConstraintViolationException();
                case 404:
                    throw new CollectionNotFoundException();
                default:
                    throw new ArangoException(response.Body);
            }
            
            return result;
        }
        
        /// <summary>
        /// Creates multiple new document within specified collection in current database context.
        /// </summary>
        public async Task<AResult<List<T>>> Documents(IEnumerable<T> document)
        {
            List<T> documents = document.ToList();
            if (documents.Count == 1)
            {
                AResult<T> resultSingle;
                try
                {
                    resultSingle = await Document(documents.First());
                }
                catch (UniqueConstraintViolationException)
                {
                    throw new MultipleException();
                }

                return new AResult<List<T>>()
                {
                    StatusCode = resultSingle.StatusCode,
                    Success = resultSingle.Success,
                    Extra = resultSingle.Extra,
                    Value = new List<T>()
                    {
                        resultSingle.Value
                    }
                };
            }
            
            var request = _requestFactory.Create(HttpMethod.Post, ApiBaseUri.Document, "/" + _collection.Name);
            
            if (_waitForSync.HasValue)
                request.QueryString.Add(ParameterName.WaitForSync, _waitForSync.Value.ToString().ToLower());
            if (_returnNew.HasValue)
                request.QueryString.Add(ParameterName.ReturnNew, _returnNew.Value.ToString().ToLower());

            request.SetBody(documents);
            
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
        /// Creates new edge document with document data in current database context.
        /// </summary>
        /// <exception cref="ArgumentException">Specified document does not contain '_from' and '_to' fields.</exception>
        public Task<AResult<T>> Edge(T document)
        {
            // TODO validate
            /*if (!document.Has("_from") && !document.Has("_to"))
            {
                throw new ArgumentException("Specified document does not contain '_from' and '_to' fields.");
            }*/

            return Document(document);
        }

        #endregion
    }
}