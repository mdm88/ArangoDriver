using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using ArangoDriver.External.dictator;
using ArangoDriver.Protocol;
using ArangoDriver.Protocol.Responses;

namespace ArangoDriver.Client
{
    public class DocumentCreate<T> where T : class
    {
        private readonly RequestFactory _requestFactory;
        private readonly Dictionary<string, object> _parameters = new Dictionary<string, object>();
        private readonly ACollection<T> _collection;
        
        #region Parameters
        
        /// <summary>
        /// Determines whether or not to wait until data are synchronised to disk. Default value: false.
        /// </summary>
        public DocumentCreate<T> WaitForSync(bool value)
        {
            // needs to be string value
            _parameters.String(ParameterName.WaitForSync, value.ToString().ToLower());
        	
            return this;
        }

        /// <summary>
        /// Determines whether to return additionally the complete new document under the attribute 'new' in the result.
        /// </summary>
        public DocumentCreate<T> ReturnNew()
        {
            // needs to be string value
            _parameters.String(ParameterName.ReturnNew, true.ToString().ToLower());

            return this;
        }
        
        #endregion

        internal DocumentCreate(RequestFactory requestFactory, ACollection<T> collection)
        {
            _requestFactory = requestFactory;
            _collection = collection;
        }

        /// <summary>
        /// Creates new document within specified collection in current database context.
        /// </summary>
        private async Task<AResult<T>> Insert(T document)
        {
            var request = _requestFactory.Create(HttpMethod.Post, ApiBaseUri.Document, "/" + _collection.Name);
            
            // optional
            request.TrySetQueryStringParameter(ParameterName.WaitForSync, _parameters);
            // optional
            request.TrySetQueryStringParameter(ParameterName.ReturnNew, _parameters);

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
                    else
                        body = response.ParseBody<T>();

                    result.Success = (body != null);
                    result.Value = body;
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
        
        /// <summary>
        /// Creates new document within specified collection in current database context.
        /// </summary>
        private async Task<AResult<List<T>>> InsertMany(IEnumerable<T> document)
        {
            var request = _requestFactory.Create(HttpMethod.Post, ApiBaseUri.Document, "/" + _collection.Name);
            
            // optional
            request.TrySetQueryStringParameter(ParameterName.WaitForSync, _parameters);
            // optional
            request.TrySetQueryStringParameter(ParameterName.ReturnNew, _parameters);

            request.SetBody(document);
            
            var response = await _collection.Send(request);
            var result = new AResult<List<T>>(response);
            
            switch (response.StatusCode)
            {
                case 201:
                case 202:
                    
                    var body = response.ParseBody<List<T>>();
                    
                    result.Success = (body != null);
                    result.Value = body;
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
        
        #region Document
        
        /// <summary>
        /// Creates new document within specified collection in current database context.
        /// </summary>
        public Task<AResult<T>> Document(T document)
        {
            return Insert(document);
        }
        
        /// <summary>
        /// Creates multiple new document within specified collection in current database context.
        /// </summary>
        public Task<AResult<List<T>>> Documents(IEnumerable<T> document)
        {
            return InsertMany(document);
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

            return Insert(document);
        }

        #endregion
    }
}