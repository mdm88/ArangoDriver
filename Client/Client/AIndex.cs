using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using ArangoDriver.External.dictator;
using ArangoDriver.Protocol;
using ArangoDriver.Protocol.Requests;
using ArangoDriver.Protocol.Responses;

namespace ArangoDriver.Client
{
    public class AIndex
    {
        private readonly RequestFactory _requestFactory;
        readonly Dictionary<string, object> _parameters = new Dictionary<string, object>();
        readonly ADatabase _connection;
        
        internal AIndex(RequestFactory requestFactory, ADatabase connection)
        {
            _requestFactory = requestFactory;
            _connection = connection;
        }
        
        #region Parameters
        
        /// <summary>
        /// Determines an array of attribute paths in the collection with hash, fulltext, geo or skiplist indexes.
        /// </summary>
        public AIndex Fields(params string[] values)
        {
            _parameters.Array(ParameterName.Fields, values);
        	
        	return this;
        }
        
        /// <summary>
        /// Determines if the order within the array is longitude followed by latitude in geo index.
        /// </summary>
        public AIndex GeoJson(bool value)
        {
            _parameters.Bool(ParameterName.GeoJson, value);
        	
        	return this;
        }
        
        /// <summary>
        /// Determines minimum character length of words for fulltext index. Will default to a server-defined value if unspecified. It is thus recommended to set this value explicitly when creating the index.
        /// </summary>
        public AIndex MinLength(int value)
        {
            _parameters.Int(ParameterName.MinLength, value);
        	
        	return this;
        }
        
        /// <summary>
        /// Determines if the hash or skiplist index should be sparse.
        /// </summary>
        public AIndex Sparse(bool value)
        {
            _parameters.Bool(ParameterName.Sparse, value);
        	
        	return this;
        }
        
        /// <summary>
        /// Determines type of the index.
        /// </summary>
        public AIndex Type(AIndexType value)
        {
            // needs to be string value
            _parameters.String(ParameterName.Type, value.ToString().ToLower());
        	
        	return this;
        }
        
        /// <summary>
        /// Determines if the hash or skiplist index should be unique.
        /// </summary>
        public AIndex Unique(bool value)
        {
            _parameters.Bool(ParameterName.Unique, value);
        	
        	return this;
        }
        
        #endregion
        
        #region Create index (POST)
        
        /// <summary>
        /// Creates index within specified collection in current database context.
        /// </summary>
        public async Task<AResult<Dictionary<string, object>>> Create(string collectionName)
        {
            var request = _requestFactory.Create(HttpMethod.Post, ApiBaseUri.Index, "");
            var document = new IndexCreateRequest();
            
            // required
            request.QueryString.Add(ParameterName.Collection, collectionName);
            
            // required
            document.Type = _parameters.String(ParameterName.Type);
            document.Fields = _parameters.Array<string>(ParameterName.Fields).ToList();
            
            switch (_parameters.Enum<AIndexType>(ParameterName.Type))
            {
                case AIndexType.Fulltext:
                    if (_parameters.Has(ParameterName.MinLength))
                        document.MinLength = _parameters.Int(ParameterName.MinLength);
                    break;
                case AIndexType.Geo:
                    if (_parameters.Has(ParameterName.GeoJson))
                        document.GeoJson = _parameters.Bool(ParameterName.GeoJson);
                    break;
                case AIndexType.Hash:
                case AIndexType.Skiplist:
                    if (_parameters.Has(ParameterName.Sparse))
                        document.Sparse = _parameters.Bool(ParameterName.Sparse);
                    if (_parameters.Has(ParameterName.Unique))
                        document.Unique = _parameters.Bool(ParameterName.Unique);
                    break;
            }
            
            request.SetBody(document);
            
            var response = await _connection.Send(request);
            var result = new AResult<Dictionary<string, object>>(response);
            
            switch (response.StatusCode)
            {
                case 200:
                case 201:
                    var body = response.ParseBody<IndexCreateResponse>();
                    
                    result.Success = (body != null);
                    result.Value = new Dictionary<string, object>()
                    {
                        {"id", body.Id},
                        {"name", body.Name},
                        {"type", body.Type},
                        {"fields", body.Fields},
                        {"minLength", body.MinLength},
                        {"geoJson", body.GeoJson},
                        {"sparse", body.Sparse},
                        {"unique", body.Unique},
                        {"isNewlyCreated", body.IsNewlyCreated},
                        {"selectivityEstimate", body.SelectivityEstimate}
                    };
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
        
        #region Get index (GET)
        
        /// <summary>
        /// Retrieves specified index.
        /// </summary>
        /// <exception cref="ArgumentException">Specified id value has invalid format.</exception>
        public async Task<AResult<Dictionary<string, object>>> Get(string id)
        {
            if (!ADocument.IsID(id))
            {
                throw new ArgumentException("Specified id value (" + id + ") has invalid format.");
            }
            
            var request = _requestFactory.Create(HttpMethod.Get, ApiBaseUri.Index, "/" + id);
            
            var response = await _connection.Send(request);
            var result = new AResult<Dictionary<string, object>>(response);
            
            switch (response.StatusCode)
            {
                case 200:
                    var body = response.ParseBody<IndexCreateResponse>();
                    
                    result.Success = (body != null);
                    result.Value = new Dictionary<string, object>()
                    {
                        {"id", body.Id},
                        {"name", body.Name},
                        {"type", body.Type},
                        {"fields", body.Fields},
                        {"minLength", body.MinLength},
                        {"geoJson", body.GeoJson},
                        {"sparse", body.Sparse},
                        {"unique", body.Unique},
                        {"isNewlyCreated", body.IsNewlyCreated},
                        {"selectivityEstimate", body.SelectivityEstimate}
                    };
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
        
        #region Delete index (DELETE)
        
        /// <summary>
        /// Deletes specified index.
        /// </summary>
        /// <exception cref="ArgumentException">Specified id value has invalid format.</exception>
        public async Task<AResult<Dictionary<string, object>>> Delete(string id)
        {
            if (!ADocument.IsID(id))
            {
                throw new ArgumentException("Specified id value (" + id + ") has invalid format.");
            }
            
            var request = _requestFactory.Create(HttpMethod.Delete, ApiBaseUri.Index, "/" + id);
            
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
            
            _parameters.Clear();
            
            return result;
        }
        
        #endregion
    }
}
