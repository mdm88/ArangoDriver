using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using ArangoDriver.External.dictator;
using ArangoDriver.Protocol;

namespace ArangoDriver.Client
{
    public class DocumentUpdate
    {   
        private readonly RequestFactory _requestFactory;
        private readonly Dictionary<string, object> _parameters = new Dictionary<string, object>();
        private readonly ACollection _collection;

        #region Parameters
        
        /// <summary>
        /// Determines whether or not to wait until data are synchronised to disk. Default value: false.
        /// </summary>
        public DocumentUpdate WaitForSync(bool value)
        {
            // needs to be string value
            _parameters.String(ParameterName.WaitForSync, value.ToString().ToLower());
        	
            return this;
        }

        /// <summary>
        /// Determines whether to keep any attributes from existing document that are contained in the patch document which contains null value. Default value: true.
        /// </summary>
        public DocumentUpdate KeepNull(bool value)
        {
            // needs to be string value
            _parameters.String(ParameterName.KeepNull, value.ToString().ToLower());
        	
            return this;
        }
        
        /// <summary>
        /// Determines whether the value in the patch document will overwrite the existing document's value. Default value: true.
        /// </summary>
        public DocumentUpdate MergeObjects(bool value)
        {
            // needs to be string value
            _parameters.String(ParameterName.MergeObjects, value.ToString().ToLower());
        	
            return this;
        }

        /// <summary>
        /// Determines whether to '_rev' field in the given document is ignored. If this is set to false, then the '_rev' attribute given in the body document is taken as a precondition. The document is only replaced if the current revision is the one specified.
        /// </summary>
        public DocumentUpdate IgnoreRevs(bool value)
        {
            // needs to be string value
            _parameters.String(ParameterName.IgnoreRevs, value.ToString().ToLower());

            return this;
        }

        /// <summary>
        /// Determines whether to return additionally the complete new document under the attribute 'new' in the result.
        /// </summary>
        public DocumentUpdate ReturnNew()
        {
            // needs to be string value
            _parameters.String(ParameterName.ReturnNew, true.ToString().ToLower());

            return this;
        }

        /// <summary>
        /// Determines whether to return additionally the complete previous revision of the changed document under the attribute 'old' in the result.
        /// </summary>
        public DocumentUpdate ReturnOld()
        {
            // needs to be string value
            _parameters.String(ParameterName.ReturnOld, true.ToString().ToLower());

            return this;
        }

        /// <summary>
        /// Conditionally operate on document with specified revision.
        /// </summary>
        public DocumentUpdate IfMatch(string revision)
        {
            _parameters.String(ParameterName.IfMatch, revision);
        	
            return this;
        }

        #endregion

        internal DocumentUpdate(RequestFactory requestFactory, ACollection collection)
        {
            _requestFactory = requestFactory;
            _collection = collection;
        }
        
        #region Update

        /// <summary>
        /// Updates existing document identified by its handle with new document data.
        /// </summary>
        public async Task<AResult<Dictionary<string, object>>> Update<T>(string id, T document)
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
        
        #endregion
    }
}