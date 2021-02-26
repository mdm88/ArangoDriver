using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net.Http;
using System.Threading.Tasks;
using ArangoDriver.Expressions;
using ArangoDriver.Protocol;
using ArangoDriver.Protocol.Requests;
using ArangoDriver.Protocol.Responses;

namespace ArangoDriver.Client
{
    public class IndexBuilder<T> where T : class
    {
        private readonly RequestFactory _requestFactory;
        private readonly ACollection<T> _collection;
        private readonly IndexCreateRequest _create;
        
        internal IndexBuilder(RequestFactory requestFactory, ACollection<T> collection, AIndexType type)
        {
            _requestFactory = requestFactory;
            _collection = collection;
            
            _create = new IndexCreateRequest()
            {
	            Type = type.ToString().ToLower()
            };
        }
        
        #region Parameters
        
        /// <summary>
        /// Determines an array of attribute paths in the collection with hash, fulltext, geo or skiplist indexes.
        /// </summary>
        public IndexBuilder<T> Fields(params string[] values)
        {
	        _create.Fields = values.ToList();
        	
        	return this;
        }

        /// <summary>
        /// Determines an array of attribute paths in the collection with hash, fulltext, geo or skiplist indexes.
        /// </summary>
        public IndexBuilder<T> Fields(params Expression<Func<T, object>>[] values)
        {
	        _create.Fields = values.Select(x => new FieldExpression<T, object>(x).Field).ToList();

	        return this;
        }
        
        /// <summary>
        /// Determines if the order within the array is longitude followed by latitude in geo index.
        /// </summary>
        public IndexBuilder<T> GeoJson(bool value)
        {
	        _create.GeoJson = value;
        	
        	return this;
        }
        
        /// <summary>
        /// Determines minimum character length of words for fulltext index. Will default to a server-defined value if unspecified. It is thus recommended to set this value explicitly when creating the index.
        /// </summary>
        public IndexBuilder<T> MinLength(int value)
        {
	        _create.MinLength = value;
        	
        	return this;
        }
        
        /// <summary>
        /// Determines if the hash or skiplist index should be sparse.
        /// </summary>
        public IndexBuilder<T> Sparse(bool value)
        {
	        _create.Sparse = value;
        	
        	return this;
        }
        
        /// <summary>
        /// Determines if the hash or skiplist index should be unique.
        /// </summary>
        public IndexBuilder<T> Unique(bool value)
        {
	        _create.Unique = value;
        	
        	return this;
        }
        
        #endregion
        
        /// <summary>
        /// Creates index within specified collection in current database context.
        /// </summary>
        public async Task<AResult<IndexCreateResponse>> Create()
        {
            var request = _requestFactory.Create(HttpMethod.Post, ApiBaseUri.Index, "");
            
            // required
            request.QueryString.Add(ParameterName.Collection, _collection.Name);

            request.SetBody(_create);
            
            var response = await _collection.Send(request);
            var result = new AResult<IndexCreateResponse>(response);
            
            switch (response.StatusCode)
            {
                case 200:
                case 201:
                    var body = response.ParseBody<IndexCreateResponse>();
                    
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
    }
}