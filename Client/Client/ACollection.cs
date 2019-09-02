﻿using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using ArangoDriver.External.dictator;
using ArangoDriver.Protocol;

namespace ArangoDriver.Client
{
    public class ACollection
    {
        private readonly RequestFactory _requestFactory;
        private readonly ADatabase _connection;
        private readonly string _collectionName;

        public string Name => _collectionName;

        internal ACollection(RequestFactory requestFactory, ADatabase connection, string collectionName)
        {
            _requestFactory = requestFactory;
            _connection = connection;
            _collectionName = collectionName;
        }
        
        #region Collection
        
        /// <summary>
        /// Retrieves basic information about specified collection.
        /// </summary>
        public async Task<AResult<Dictionary<string, object>>> GetInformation()
        {
            var request = _requestFactory.Create(HttpMethod.Get, ApiBaseUri.Collection, "/" + _collectionName);

            var response = await _connection.Send(request);
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
        
        /// <summary>
        /// Retrieves basic information with additional properties about specified collection.
        /// </summary>
        public async Task<AResult<Dictionary<string, object>>> GetProperties()
        {
            var request = _requestFactory.Create(HttpMethod.Get, ApiBaseUri.Collection, "/" + _collectionName + "/properties");

            var response = await _connection.Send(request);
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
        
        /// <summary>
        /// Retrieves basic information with additional properties and document count in specified collection.
        /// </summary>
        public async Task<AResult<Dictionary<string, object>>> GetCount()
        {
            var request = _requestFactory.Create(HttpMethod.Get, ApiBaseUri.Collection, "/" + _collectionName + "/count");

            var response = await _connection.Send(request);
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
        
        /// <summary>
        /// Retrieves basic information with additional properties, document count and figures in specified collection.
        /// </summary>
        public async Task<AResult<Dictionary<string, object>>> GetFigures()
        {
            var request = _requestFactory.Create(HttpMethod.Get, ApiBaseUri.Collection, "/" + _collectionName + "/figures");

            var response = await _connection.Send(request);
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
        
        /// <summary>
        /// Retrieves basic information and revision ID of specified collection.
        /// </summary>
        public async Task<AResult<Dictionary<string, object>>> GetRevision()
        {
            var request = _requestFactory.Create(HttpMethod.Get, ApiBaseUri.Collection, "/" + _collectionName + "/revision");

            var response = await _connection.Send(request);
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
        
        /// <summary>
        /// Retrieves basic information, revision ID and checksum of specified collection.
        /// </summary>
        public async Task<AResult<Dictionary<string, object>>> GetChecksum()
        {
            var request = _requestFactory.Create(HttpMethod.Get, ApiBaseUri.Collection, "/" + _collectionName + "/checksum");

            // optional
            //request.TrySetQueryStringParameter(ParameterName.WithRevisions, _parameters);
            // optional
            //request.TrySetQueryStringParameter(ParameterName.WithData, _parameters);
            
            var response = await _connection.Send(request);
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
        
        /// <summary>
        /// Retrieves list of indexes in specified collection.
        /// </summary>
        public async Task<AResult<Dictionary<string, object>>> GetAllIndexes()
        {
            var request = _requestFactory.Create(HttpMethod.Get, ApiBaseUri.Index, "");

            // required
            request.QueryString.Add(ParameterName.Collection, _collectionName);
            
            var response = await _connection.Send(request);
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
        
        /// <summary>
        /// Removes all documents from specified collection.
        /// </summary>
        public async Task<AResult<Dictionary<string, object>>> Truncate()
        {
            var request = _requestFactory.Create(HttpMethod.Put, ApiBaseUri.Collection, "/" + _collectionName + "/truncate");
            
            var response = await _connection.Send(request);
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
            
            var response = await _connection.Send(request);
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
        
        /// <summary>
        /// Renames specified collection.
        /// </summary>
        public async Task<AResult<Dictionary<string, object>>> Rename(string newCollectionName)
        {
            var request = _requestFactory.Create(HttpMethod.Put, ApiBaseUri.Collection, "/" + _collectionName + "/rename");
            var document = new Dictionary<string, object>()
                .String(ParameterName.Name, newCollectionName);
            
            request.SetBody(document);
            
            var response = await Send(request);
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
        
        #region Documents and Edges

        /// <summary>
        /// Creates new document or edge within specified collection in current database context.
        /// Must call Document() or Edge() to confirm
        /// </summary>
        /// <returns>DocumentCreate</returns>
        public DocumentCreate Insert()
        {
            return new DocumentCreate(_requestFactory, this);
        }
        
        /// <summary>
        /// Checks for existence of specified document.
        /// </summary>
        /// <exception cref="ArgumentException">Specified 'id' value has invalid format.</exception>
        public async Task<AResult<string>> Check(string id)
        {
            if (!ADocument.IsID(id))
            {
                throw new ArgumentException("Specified 'id' value (" + id + ") has invalid format.");
            }
            
            var request = _requestFactory.Create(HttpMethod.Head, ApiBaseUri.Document, "/" + id);
            
            // optional
            //request.TrySetHeaderParameter(ParameterName.IfMatch, _parameters);
            // optional: If revision is different -> HTTP 200. If revision is identical -> HTTP 304.
            //request.TrySetHeaderParameter(ParameterName.IfNoneMatch, _parameters);
            
            var response = await _connection.Send(request);
            var result = new AResult<string>(response);
            
            switch (response.StatusCode)
            {
                case 200:
                    if ((response.Headers.ETag.Tag ?? "").Trim().Length > 0)
                    {
                        result.Value = response.Headers.ETag.Tag?.Replace("\"", "");
                        result.Success = (result.Value != null);
                    }
                    break;
                case 304:
                case 412:
                    if ((response.Headers.ETag.Tag ?? "").Trim().Length > 0)
                    {
                        result.Value = response.Headers.ETag.Tag?.Replace("\"", "");
                    }
                    break;
                case 404:
                default:
                    // Arango error
                    break;
            }
            
            return result;
        }

        /// <summary>
        /// Retrieves specified document.
        /// </summary>
        /// <exception cref="ArgumentException">Specified 'id' value has invalid format.</exception>
        public async Task<AResult<T>> Get<T>(string id)
        {
            if (!ADocument.IsID(id))
            {
                throw new ArgumentException("Specified 'id' value (" + id + ") has invalid format.");
            }
            
            var request = _requestFactory.Create(HttpMethod.Get, ApiBaseUri.Document, "/" + id);
            
            // optional
            //request.TrySetHeaderParameter(ParameterName.IfMatch, _parameters);
            // optional: If revision is different -> HTTP 200. If revision is identical -> HTTP 304.
            //request.TrySetHeaderParameter(ParameterName.IfNoneMatch, _parameters);
            
            var response = await _connection.Send(request);
            var result = new AResult<T>(response);
            
            switch (response.StatusCode)
            {
                case 200:
                    var body = response.ParseBody<T>();
                    
                    result.Success = (body != null);
                    result.Value = body;
                    break;
                case 412:
                    body = response.ParseBody<T>();
                    
                    result.Value = body;
                    break;
                case 304:
                case 404:
                default:
                    // Arango error
                    break;
            }
            
            return result;
        }

        /// <summary>
        /// Retrieves all documents.
        /// </summary>
        public async Task<AResult<List<T>>> GetAll<T>()
        {
            AQuery query = new AQuery(_requestFactory, _connection).Aql("FOR x IN " + _collectionName + " RETURN x");

            return await query.ToList<T>();
        }

        /// <summary>
        /// Retrieves list of edges from specified edge type collection to specified document vertex with given direction.
        /// </summary>
        /// <exception cref="ArgumentException">Specified 'startVertexID' value has invalid format.</exception>
        public async Task<AResult<List<Dictionary<string, object>>>> GetEdges(string startVertexID, ADirection direction)
        {
            if (!ADocument.IsID(startVertexID))
            {
                throw new ArgumentException("Specified 'startVertexID' value (" + startVertexID + ") has invalid format.");
            }

            var request = _requestFactory.Create(HttpMethod.Get, ApiBaseUri.Edges, "/" + Name);

            // required
            request.QueryString.Add(ParameterName.Vertex, startVertexID);
            // required
            request.QueryString.Add(ParameterName.Direction, direction.ToString().ToLower());

            var response = await _connection.Send(request);
            var result = new AResult<List<Dictionary<string, object>>>(response);

            switch (response.StatusCode)
            {
                case 200:
                    var body = response.ParseBody<Dictionary<string, object>>();

                    result.Success = (body != null);

                    if (result.Success)
                    {
                        result.Value = body.List<Dictionary<string, object>>("edges");
                    }
                    break;
                case 400:
                case 404:
                default:
                    // Arango error
                    break;
            }

            return result;
        }

        /// <summary>
        /// Updates existing document identified by its handle with new document data.
        /// Must call Update() method to confirm
        /// </summary>
        /// <returns>DocumentUpdate</returns>
        public DocumentUpdate Update()
        {
            return new DocumentUpdate(_requestFactory, this);
        }

        /// <summary>
        /// Completely replaces existing document identified by its handle with new document data.
        /// Must call Document() or Edge() to confirm
        /// </summary>
        /// <returns>DocumentReplace</returns>
        public DocumentReplace Replace()
        {
            return new DocumentReplace(_requestFactory, this);
        }
        
        /// <summary>
        /// Deletes specified document.
        /// Must call Delete() method to confirm
        /// </summary>
        /// <returns>DocumentDelete</returns>
        public DocumentDelete Delete()
        {
            return new DocumentDelete(_requestFactory, this);
        }
        
        #endregion
        
        internal Task<Response> Send(Request request)
        {
            return _connection.Send(request);
        }
    }
}
