using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using ArangoDriver.External.dictator;
using ArangoDriver.Protocol;

namespace ArangoDriver.Client
{
    public class DocumentCreate
    {
        private readonly RequestFactory _requestFactory;
        private readonly Dictionary<string, object> _parameters = new Dictionary<string, object>();
        private readonly ACollection _collection;
        
        #region Parameters
        
        /// <summary>
        /// Determines whether or not to wait until data are synchronised to disk. Default value: false.
        /// </summary>
        public DocumentCreate WaitForSync(bool value)
        {
            // needs to be string value
            _parameters.String(ParameterName.WaitForSync, value.ToString().ToLower());
        	
            return this;
        }

        /// <summary>
        /// Determines whether to return additionally the complete new document under the attribute 'new' in the result.
        /// </summary>
        public DocumentCreate ReturnNew()
        {
            // needs to be string value
            _parameters.String(ParameterName.ReturnNew, true.ToString().ToLower());

            return this;
        }
        
        #endregion

        internal DocumentCreate(RequestFactory requestFactory, ACollection collection)
        {
            _requestFactory = requestFactory;
            _collection = collection;
        }

        /// <summary>
        /// Creates new document within specified collection in current database context.
        /// </summary>
        private async Task<AResult<Dictionary<string, object>>> Insert<T>(T document)
        {
            var request = _requestFactory.Create(HttpMethod.Post, ApiBaseUri.Document, "/" + _collection.Name);
            
            // optional
            request.TrySetQueryStringParameter(ParameterName.WaitForSync, _parameters);
            // optional
            request.TrySetQueryStringParameter(ParameterName.ReturnNew, _parameters);

            request.SetBody(document);
            
            var response = await _collection.Send(request);
            var result = new AResult<Dictionary<string, object>>(response);
            
            switch (response.StatusCode)
            {
                case 201:
                case 202:
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
            
            _parameters.Clear();
            
            return result;
        }
        
        /// <summary>
        /// Creates new document within specified collection in current database context.
        /// </summary>
        private async Task<AResult<List<Dictionary<string, object>>>> InsertMany<T>(IEnumerable<T> document)
        {
            var request = _requestFactory.Create(HttpMethod.Post, ApiBaseUri.Document, "/" + _collection.Name);
            
            // optional
            request.TrySetQueryStringParameter(ParameterName.WaitForSync, _parameters);
            // optional
            request.TrySetQueryStringParameter(ParameterName.ReturnNew, _parameters);

            request.SetBody(document);
            
            var response = await _collection.Send(request);
            var result = new AResult<List<Dictionary<string, object>>>(response);
            
            switch (response.StatusCode)
            {
                case 201:
                case 202:
                    
                    var body = response.ParseBody<List<Dictionary<string, object>>>();
                    
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
        public Task<AResult<Dictionary<string, object>>> Document<T>(T document)
        {
            return Insert(document);
        }
        
        /// <summary>
        /// Creates multiple new document within specified collection in current database context.
        /// </summary>
        public Task<AResult<List<Dictionary<string, object>>>> Documents<T>(IEnumerable<T> document)
        {
            return InsertMany(document);
        }
        
        #endregion

        #region Edge

        /// <summary>
        /// Creates new edge document with document data in current database context.
        /// </summary>
        /// <exception cref="ArgumentException">Specified document does not contain '_from' and '_to' fields.</exception>
        public Task<AResult<Dictionary<string, object>>> Edge(Dictionary<string, object> document)
        {
            if (!document.Has("_from") && !document.Has("_to"))
            {
                throw new ArgumentException("Specified document does not contain '_from' and '_to' fields.");
            }

            return Insert(document);
        }

        /// <summary>
        /// Creates new edge document within specified collection between two document vertices in current database context.
        /// </summary>
        /// <exception cref="ArgumentException">Specified 'from' and 'to' ID values have invalid format.</exception>
        public Task<AResult<Dictionary<string, object>>> Edge(string fromId, string toId)
        {
            if (!ADocument.IsID(fromId))
            {
                throw new ArgumentException("Specified 'from' value (" + fromId + ") has invalid format.");
            }

            if (!ADocument.IsID(toId))
            {
                throw new ArgumentException("Specified 'to' value (" + toId + ") has invalid format.");
            }

            var document = new Dictionary<string, object>
            {
                { "_from", fromId  },
                { "_to", toId  },
            };

            return Edge(document);
        }

        /// <summary>
        /// Creates new edge with document data within specified collection between two document vertices in current database context.
        /// </summary>
        /// <exception cref="ArgumentException">Specified 'from' and 'to' ID values have invalid format.</exception>
        public Task<AResult<Dictionary<string, object>>> Edge(string fromId, string toId, Dictionary<string, object> document)
        {
            if (!ADocument.IsID(fromId))
            {
                throw new ArgumentException("Specified 'from' value (" + fromId + ") has invalid format.");
            }

            if (!ADocument.IsID(toId))
            {
                throw new ArgumentException("Specified 'to' value (" + toId + ") has invalid format.");
            }

            document.From(fromId);
            document.To(toId);

            return Edge(document);
        }

        /// <summary>
        /// Creates new edge with document data within specified collection between two document vertices in current database context.
        /// </summary>
        /// <exception cref="ArgumentException">Specified 'from' and 'to' ID values have invalid format.</exception>
        public Task<AResult<Dictionary<string, object>>> Edge<T>(string fromId, string toId, T obj)
        {
            if (!ADocument.IsID(fromId))
            {
                throw new ArgumentException("Specified 'from' value (" + fromId + ") has invalid format.");
            }

            if (!ADocument.IsID(toId))
            {
                throw new ArgumentException("Specified 'to' value (" + toId + ") has invalid format.");
            }

            return Edge(fromId, toId, Dictator.ToDocument(obj));
        }

        #endregion
    }
}