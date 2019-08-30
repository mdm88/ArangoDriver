using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using ArangoDriver.External.dictator;
using ArangoDriver.Protocol;

namespace ArangoDriver.Client
{
    public class DocumentDelete
    {
        private readonly Dictionary<string, object> _parameters = new Dictionary<string, object>();
        private readonly ACollection _collection;

        #region Parameters
        
        /// <summary>
        /// Determines whether or not to wait until data are synchronised to disk. Default value: false.
        /// </summary>
        public DocumentDelete WaitForSync(bool value)
        {
            // needs to be string value
            _parameters.String(ParameterName.WaitForSync, value.ToString().ToLower());
        	
            return this;
        }

        /// <summary>
        /// Determines whether to return additionally the complete previous revision of the changed document under the attribute 'old' in the result.
        /// </summary>
        public DocumentDelete ReturnOld()
        {
            // needs to be string value
            _parameters.String(ParameterName.ReturnOld, true.ToString().ToLower());

            return this;
        }

        /// <summary>
        /// Conditionally operate on document with specified revision.
        /// </summary>
        public DocumentDelete IfMatch(string revision)
        {
            _parameters.String(ParameterName.IfMatch, revision);
        	
            return this;
        }

        #endregion

        public DocumentDelete(ACollection collection)
        {
            _collection = collection;
        }

        #region Delete
        
        /// <summary>
        /// Deletes specified document.
        /// </summary>
        /// <exception cref="ArgumentException">Specified 'id' value has invalid format.</exception>
        public async Task<AResult<Dictionary<string, object>>> Delete(string id)
        {
            if (!ADocument.IsID(id))
            {
                throw new ArgumentException("Specified 'id' value (" + id + ") has invalid format.");
            }
            
            var request = new Request(HttpMethod.Delete, ApiBaseUri.Document, "/" + id);
            
            // optional
            request.TrySetQueryStringParameter(ParameterName.WaitForSync, _parameters);
            // optional
            request.TrySetQueryStringParameter(ParameterName.ReturnOld, _parameters);
            // optional
            request.TrySetHeaderParameter(ParameterName.IfMatch, _parameters);
            
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
                    body = response.ParseBody<Dictionary<string, object>>();
                    
                    result.Value = body;
                    break;
                case 404:
                default:
                    // Arango error
                    break;
            }
            
            _parameters.Clear();
            
            return result;
        }

        #endregion
    }
}