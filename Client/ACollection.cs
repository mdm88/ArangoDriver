using System.Collections.Generic;
using ArangoDriver.External.dictator;
using ArangoDriver.Protocol;
using fastJSON;

namespace ArangoDriver.Client
{
    public class ACollection
    {
        private readonly ADatabase _connection;
        private readonly string _collectionName;
        
        internal ACollection(ADatabase connection, string collectionName)
        {
            _connection = connection;
            _collectionName = collectionName;
        }
        
        #region Get collection (GET)
        
        /// <summary>
        /// Retrieves basic information about specified collection.
        /// </summary>
        public AResult<Dictionary<string, object>> Get()
        {
            var request = new Request(HttpMethod.GET, ApiBaseUri.Collection, "/" + _collectionName);

            var response = _connection.Send(request);
            var result = new AResult<Dictionary<string, object>>(response);
            
            switch (response.StatusCode)
            {
                case 200:
                    var body = response.ParseBody<Dictionary<string, object>>();
                    
                    result.Success = (body != null);
                    result.Value = body;
                    break;
                case 404:
                default:
                    // Arango error
                    break;
            }
            
            return result;
        }
        
        #endregion
        
        #region Get collection properties (GET)
        
        /// <summary>
        /// Retrieves basic information with additional properties about specified collection.
        /// </summary>
        public AResult<Dictionary<string, object>> GetProperties()
        {
            var request = new Request(HttpMethod.GET, ApiBaseUri.Collection, "/" + _collectionName + "/properties");

            var response = _connection.Send(request);
            var result = new AResult<Dictionary<string, object>>(response);
            
            switch (response.StatusCode)
            {
                case 200:
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
            
            return result;
        }
        
        #endregion
        
        #region Get collection documents count (GET)
        
        /// <summary>
        /// Retrieves basic information with additional properties and document count in specified collection.
        /// </summary>
        public AResult<Dictionary<string, object>> GetCount()
        {
            var request = new Request(HttpMethod.GET, ApiBaseUri.Collection, "/" + _collectionName + "/count");

            var response = _connection.Send(request);
            var result = new AResult<Dictionary<string, object>>(response);
            
            switch (response.StatusCode)
            {
                case 200:
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
            
            return result;
        }
        
        #endregion
        
        #region Get collection figures (GET)
        
        /// <summary>
        /// Retrieves basic information with additional properties, document count and figures in specified collection.
        /// </summary>
        public AResult<Dictionary<string, object>> GetFigures()
        {
            var request = new Request(HttpMethod.GET, ApiBaseUri.Collection, "/" + _collectionName + "/figures");

            var response = _connection.Send(request);
            var result = new AResult<Dictionary<string, object>>(response);
            
            switch (response.StatusCode)
            {
                case 200:
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
            
            return result;
        }
        
        #endregion
        
        #region Get collection revision (GET)
        
        /// <summary>
        /// Retrieves basic information and revision ID of specified collection.
        /// </summary>
        public AResult<Dictionary<string, object>> GetRevision()
        {
            var request = new Request(HttpMethod.GET, ApiBaseUri.Collection, "/" + _collectionName + "/revision");

            var response = _connection.Send(request);
            var result = new AResult<Dictionary<string, object>>(response);
            
            switch (response.StatusCode)
            {
                case 200:
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
            
            return result;
        }
        
        #endregion
        
        #region Get collection checksum (GET)
        
        /// <summary>
        /// Retrieves basic information, revision ID and checksum of specified collection.
        /// </summary>
        public AResult<Dictionary<string, object>> GetChecksum()
        {
            var request = new Request(HttpMethod.GET, ApiBaseUri.Collection, "/" + _collectionName + "/checksum");

            // optional
            //request.TrySetQueryStringParameter(ParameterName.WithRevisions, _parameters);
            // optional
            //request.TrySetQueryStringParameter(ParameterName.WithData, _parameters);
            
            var response = _connection.Send(request);
            var result = new AResult<Dictionary<string, object>>(response);
            
            switch (response.StatusCode)
            {
                case 200:
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
            
            return result;
        }
        
        #endregion
        
        #region Get all indexes (GET)
        
        /// <summary>
        /// Retrieves list of indexes in specified collection.
        /// </summary>
        public AResult<Dictionary<string, object>> GetAllIndexes()
        {
            var request = new Request(HttpMethod.GET, ApiBaseUri.Index, "");

            // required
            request.QueryString.Add(ParameterName.Collection, _collectionName);
            
            var response = _connection.Send(request);
            var result = new AResult<Dictionary<string, object>>(response);
            
            switch (response.StatusCode)
            {
                case 200:
                    var body = response.ParseBody<Dictionary<string, object>>();
                    
                    result.Success = (body != null);
                    result.Value = body;
                    break;
                case 404:
                default:
                    // Arango error
                    break;
            }
            
            return result;
        }
        
        #endregion
        
        #region Truncate collection (PUT)
        
        /// <summary>
        /// Removes all documents from specified collection.
        /// </summary>
        public AResult<Dictionary<string, object>> Truncate()
        {
            var request = new Request(HttpMethod.PUT, ApiBaseUri.Collection, "/" + _collectionName + "/truncate");
            
            var response = _connection.Send(request);
            var result = new AResult<Dictionary<string, object>>(response);
            
            switch (response.StatusCode)
            {
                case 200:
                    var body = response.ParseBody<Dictionary<string, object>>();
                    
                    result.Success = (body != null);
                    result.Value = body;
                    break;
                default:
                    // Arango error
                    break;
            }
            
            return result;
        }
        
        #endregion
        
        #region Change collection properties (PUT)
        
        /*/// <summary>
        /// Changes properties of specified collection.
        /// </summary>
        public AResult<Dictionary<string, object>> ChangeProperties()
        {
            var request = new Request(HttpMethod.PUT, ApiBaseUri.Collection, "/" + _collectionName + "/properties");
            var bodyDocument = new Dictionary<string, object>();
            
            // optional
            Request.TrySetBodyParameter(ParameterName.WaitForSync, _parameters, bodyDocument);
            // optional
            Request.TrySetBodyParameter(ParameterName.JournalSize, _parameters, bodyDocument);
            
            request.Body = JSON.ToJSON(bodyDocument, ASettings.JsonParameters);
            
            var response = _connection.Send(request);
            var result = new AResult<Dictionary<string, object>>(response);
            
            switch (response.StatusCode)
            {
                case 200:
                    var body = response.ParseBody<Dictionary<string, object>>();
                    
                    result.Success = (body != null);
                    result.Value = body;
                    break;
                default:
                    // Arango error
                    break;
            }
            
            return result;
        }*/
        
        #endregion
        
        #region Rename collection (PUT)
        
        /// <summary>
        /// Renames specified collection.
        /// </summary>
        public AResult<Dictionary<string, object>> Rename(string newCollectionName)
        {
            var request = new Request(HttpMethod.PUT, ApiBaseUri.Collection, "/" + _collectionName + "/rename");
            var bodyDocument = new Dictionary<string, object>()
                .String(ParameterName.Name, newCollectionName);
            
            request.Body = JSON.ToJSON(bodyDocument, ASettings.JsonParameters);
            
            var response = _connection.Send(request);
            var result = new AResult<Dictionary<string, object>>(response);
            
            switch (response.StatusCode)
            {
                case 200:
                    var body = response.ParseBody<Dictionary<string, object>>();
                    
                    result.Success = (body != null);
                    result.Value = body;
                    break;
                default:
                    // Arango error
                    break;
            }
            
            return result;
        }
        
        #endregion
        
        #region Rotate journal of a collection (PUT)
        
        /// <summary>
        /// Rotates the journal of specified collection to make the data in the file available for compaction. Current journal of the collection will be closed and turned into read-only datafile. This operation is not available in cluster environment.
        /// </summary>
        public AResult<bool> RotateJournal()
        {
            var request = new Request(HttpMethod.PUT, ApiBaseUri.Collection, "/" + _collectionName + "/rotate");
            
            var response = _connection.Send(request);
            var result = new AResult<bool>(response);
            
            switch (response.StatusCode)
            {
                case 200:
                    var body = response.ParseBody<Body<bool>>();
                    
                    result.Success = (body != null);
                    result.Value = body.Result;
                    break;
                case 400:
                case 404:
                default:
                    // Arango error
                    break;
            }
            
            return result;
        }
        
        #endregion
        
        #region Delete collection (DELETE)
        
        /// <summary>
        /// Deletes specified collection.
        /// </summary>
        public AResult<Dictionary<string, object>> Delete()
        {
            var request = new Request(HttpMethod.DELETE, ApiBaseUri.Collection, "/" + _collectionName);

            // optional
            //request.TrySetQueryStringParameter(ParameterName.IsSystem, _parameters);

            var response = _connection.Send(request);
            var result = new AResult<Dictionary<string, object>>(response);
            
            switch (response.StatusCode)
            {
                case 200:
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
            
            return result;
        }
        
        #endregion
    }
}
