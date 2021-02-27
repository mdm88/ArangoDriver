using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using ArangoDriver.Exceptions;
using ArangoDriver.Protocol;
using ArangoDriver.Protocol.Requests;
using ArangoDriver.Protocol.Responses;
using ArangoDriver.Serialization;

namespace ArangoDriver.Client
{
    public class DocumentCreate<T> where T : class
    {
        private readonly RequestFactory _requestFactory;
        private readonly ACollection<T> _collection;
        private readonly IJsonSerializer _jsonSerializer;

        private bool? _waitForSync;
        private bool? _returnNew;
        private bool? _returnOld;
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
        /// Determines whether to return additionally the complete new document in the result.
        /// </summary>
        public DocumentCreate<T> ReturnNew()
        {
            _returnNew = true;

            return this;
        }

        /// <summary>
        /// Determines whether to return additionally the complete old document in the result.
        /// Only available if the overwrite option is used.
        /// </summary>
        public DocumentCreate<T> ReturnOld()
        {
            _returnOld = true;

            return this;
        }

        public DocumentCreate<T> OverwriteMode(OverwriteMode mode)
        {
            _overwriteMode = mode;

            return this;
        }
        
        #endregion

        internal DocumentCreate(RequestFactory requestFactory, ACollection<T> collection, IJsonSerializer jsonSerializer)
        {
            _requestFactory = requestFactory;
            _collection = collection;
            _jsonSerializer = jsonSerializer;
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
            if (_returnOld.HasValue)
                request.QueryString.Add(ParameterName.ReturnOld, _returnOld.Value.ToString().ToLower());
            if (_overwriteMode.HasValue)
            {
                request.QueryString.Add(ParameterName.Overwrite, "true");
                request.QueryString.Add(ParameterName.OverwriteMode, _overwriteMode.Value.ToString().ToLower());
            }

            request.SetBody(document);
            
            var result = await _collection.RequestQuery<DocumentCreateResponse<T>>(request);

            if (!result.Success)
            {
                switch (result.StatusCode)
                {
                    case 409:
                        throw new UniqueConstraintViolationException();
                    case 404:
                        throw new CollectionNotFoundException();
                    default:
                        throw new ArangoException();
                }
            }
            
            T body = default;
            if (_returnNew.HasValue && _returnNew.Value)
                body = result.Value.New;
            else if (_returnOld.HasValue && _returnOld.Value)
                body = result.Value.Old;

            return new AResult<T>()
            {
                StatusCode = result.StatusCode,
                Success = result.Success,
                Value = body
            };
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
            if (_returnOld.HasValue)
                request.QueryString.Add(ParameterName.ReturnOld, _returnOld.Value.ToString().ToLower());
            if (_overwriteMode.HasValue)
            {
                request.QueryString.Add(ParameterName.Overwrite, "true");
                request.QueryString.Add(ParameterName.OverwriteMode, _overwriteMode.Value.ToString().ToLower());
            }

            request.SetBody(documents);
            
            using var response = await _collection.Request(request);
            
            var result = new AResult<List<T>>()
            {
                StatusCode = (int) response.StatusCode,
                Success = response.IsSuccessStatusCode
            };

            switch (result.StatusCode)
            {
                case 201:
                case 202:
                    response.Headers.TryGetValues("X-Arango-Error-Codes", out var errors);
                    if (errors != null && errors.Any())
                        throw new MultipleException();

                    var values = _jsonSerializer.Deserialize<List<DocumentCreateResponse<T>>>(await response.Content.ReadAsStreamAsync());

                    if (_returnNew.HasValue && _returnNew.Value)
                        result.Value = values.Select(e => e.New).ToList();
                    else if (_returnOld.HasValue && _returnOld.Value)
                        result.Value = values.Select(e => e.Old).ToList();
                    break;
                case 404:
                    throw new CollectionNotFoundException();
                default:
                    throw new ArangoException();
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