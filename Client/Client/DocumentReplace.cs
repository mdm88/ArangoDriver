using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using ArangoDriver.External.dictator;
using ArangoDriver.Protocol;
using fastJSON;

namespace ArangoDriver.Client
{
    public class DocumentReplace
    {
        private readonly Dictionary<string, object> _parameters = new Dictionary<string, object>();
        private readonly ACollection _collection;

        #region Parameters
        
        /// <summary>
        /// Determines whether or not to wait until data are synchronised to disk. Default value: false.
        /// </summary>
        public DocumentReplace WaitForSync(bool value)
        {
            // needs to be string value
            _parameters.String(ParameterName.WaitForSync, value.ToString().ToLower());
        	
            return this;
        }

        /// <summary>
        /// Determines whether to '_rev' field in the given document is ignored. If this is set to false, then the '_rev' attribute given in the body document is taken as a precondition. The document is only replaced if the current revision is the one specified.
        /// </summary>
        public DocumentReplace IgnoreRevs(bool value)
        {
            // needs to be string value
            _parameters.String(ParameterName.IgnoreRevs, value.ToString().ToLower());

            return this;
        }

        /// <summary>
        /// Determines whether to return additionally the complete new document under the attribute 'new' in the result.
        /// </summary>
        public DocumentReplace ReturnNew()
        {
            // needs to be string value
            _parameters.String(ParameterName.ReturnNew, true.ToString().ToLower());

            return this;
        }

        /// <summary>
        /// Determines whether to return additionally the complete previous revision of the changed document under the attribute 'old' in the result.
        /// </summary>
        public DocumentReplace ReturnOld()
        {
            // needs to be string value
            _parameters.String(ParameterName.ReturnOld, true.ToString().ToLower());

            return this;
        }

        /// <summary>
        /// Conditionally operate on document with specified revision.
        /// </summary>
        public DocumentReplace IfMatch(string revision)
        {
            _parameters.String(ParameterName.IfMatch, revision);
        	
            return this;
        }

        #endregion

        public DocumentReplace(ACollection collection)
        {
            _collection = collection;
        }
        
        #region Document
        
        /// <summary>
        /// Completely replaces existing document identified by its handle with new document data.
        /// </summary>
        /// <exception cref="ArgumentException">Specified 'id' value has invalid format.</exception>
        public async Task<AResult<Dictionary<string, object>>> Document(string id, string json)
        {
            if (!ADocument.IsID(id))
            {
                throw new ArgumentException("Specified 'id' value (" + id + ") has invalid format.");
            }
            
            var request = new Request(HttpMethod.Put, ApiBaseUri.Document, "/" + id);
            
            // optional
            request.TrySetQueryStringParameter(ParameterName.WaitForSync, _parameters);
            // optional
            request.TrySetQueryStringParameter(ParameterName.IgnoreRevs, _parameters);
            // optional
            request.TrySetQueryStringParameter(ParameterName.ReturnNew, _parameters);
            // optional
            request.TrySetQueryStringParameter(ParameterName.ReturnOld, _parameters);
            // optional
            request.TrySetHeaderParameter(ParameterName.IfMatch, _parameters);
            
            request.Body = json;
            
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
                case 412:
                    body = response.ParseBody<Dictionary<string, object>>();
                    
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
        /// Completely replaces existing document identified by its handle with new document data.
        /// </summary>
        /// <exception cref="ArgumentException">Specified id value has invalid format.</exception>
        public Task<AResult<Dictionary<string, object>>> Document(string id, Dictionary<string, object> document)
        {
            return Document(id, JSON.ToJSON(document, ASettings.JsonParameters));
        }
        
        /// <summary>
        /// Completely replaces existing document identified by its handle with new document data.
        /// </summary>
        /// <exception cref="ArgumentException">Specified id value has invalid format.</exception>
        public Task<AResult<Dictionary<string, object>>> Document<T>(string id, T obj)
        {
            return Document(id, Dictator.ToDocument(obj));
        }

        #endregion

        #region Edge

        /// <summary>
        /// Completely replaces existing edge identified by its handle with new edge data.
        /// </summary>
        /// <exception cref="ArgumentException">Specified document does not contain '_from' and '_to' fields.</exception>
        public Task<AResult<Dictionary<string, object>>> Edge(string id, Dictionary<string, object> document)
        {
            if (!document.Has("_from") && !document.Has("_to"))
            {
                throw new ArgumentException("Specified document does not contain '_from' and '_to' fields.");
            }

            return Document(id, JSON.ToJSON(document, ASettings.JsonParameters));
        }

        /// <summary>
        /// Completely replaces existing edge identified by its handle with new edge data. This helper method injects 'fromID' and 'toID' fields into given document to construct valid edge document.
        /// </summary>
        /// <exception cref="ArgumentException">Specified 'from' or 'to' ID values have invalid format.</exception>
        public Task<AResult<Dictionary<string, object>>> Edge(string id, string fromId, string toId, Dictionary<string, object> document)
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

            return Document(id, JSON.ToJSON(document, ASettings.JsonParameters));
        }

        /// <summary>
        /// Completely replaces existing edge identified by its handle with new edge data. This helper method injects 'fromID' and 'toID' fields into given document to construct valid edge document.
        /// </summary>
        public Task<AResult<Dictionary<string, object>>> Edge<T>(string id, string fromId, string toId, T obj)
        {
            return Edge(id, fromId, toId, Dictator.ToDocument(obj));
        }

        #endregion

    }
}