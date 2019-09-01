using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using ArangoDriver.External.dictator;
using ArangoDriver.Protocol;

namespace ArangoDriver.Client
{
    public class CollectionBuilder
    {
	    private readonly RequestFactory _requestFactory;
        private readonly Dictionary<string, object> _parameters = new Dictionary<string, object>();
        private readonly ADatabase _connection;
        private readonly string _collectionName;
        
        internal CollectionBuilder(RequestFactory requestFactory, ADatabase connection, string collectionName)
        {
	        _requestFactory = requestFactory;
            _connection = connection;
            _collectionName = collectionName;
        }
        
        #region Parameters
        
        /// <summary>
        /// Determines type of the collection. Default value: Document.
        /// </summary>
        public CollectionBuilder Type(ACollectionType value)
        {
            // set enum format explicitely to override global setting
            _parameters.Enum(ParameterName.Type, value, EnumFormat.Integer);
        	
        	return this;
        }
        
        /// <summary>
        /// Determines whether or not to wait until data are synchronised to disk. Default value: false.
        /// </summary>
        public CollectionBuilder WaitForSync(bool value)
        {
            _parameters.Bool(ParameterName.WaitForSync, value);
        	
        	return this;
        }
        
        /// <summary>
        /// Determines maximum size of a journal or datafile in bytes. Default value: server configured.
        /// </summary>
        public CollectionBuilder JournalSize(long value)
        {
            _parameters.Long(ParameterName.JournalSize, value);
        	
        	return this;
        }
        
        /// <summary>
        /// Determines whether the collection will be compacted. Default value: true.
        /// </summary>
        public CollectionBuilder DoCompact(bool value)
        {
            _parameters.Bool(ParameterName.DoCompact, value);
        	
        	return this;
        }
        
        /// <summary>
        /// Determines whether the collection is a system collection. Default value: false.
        /// </summary>
        public CollectionBuilder IsSystem(bool value)
        {
            _parameters.Bool(ParameterName.IsSystem, value);
        	
        	return this;
        }
        
        /// <summary>
        /// Determines whether the collection data is kept in-memory only and not made persistent. Default value: false.
        /// </summary>
        public CollectionBuilder IsVolatile(bool value)
        {
            _parameters.Bool(ParameterName.IsVolatile, value);
        	
        	return this;
        }
        
        #region Key options
        
        /// <summary>
        /// Determines the type of the key generator. Default value: Traditional.
        /// </summary>
        public CollectionBuilder KeyGeneratorType(AKeyGeneratorType value)
        {
            // needs to be in string format - set enum format explicitely to override global setting
            _parameters.Enum(ParameterName.KeyOptionsType, value.ToString().ToLower(), EnumFormat.String);
        	
        	return this;
        }
        
        /// <summary>
        /// Determines whether it is allowed to supply custom key values in the _key attribute of a document. Default value: true.
        /// </summary>
        public CollectionBuilder AllowUserKeys(bool value)
        {
            _parameters.Bool(ParameterName.KeyOptionsAllowUserKeys, value);
        	
        	return this;
        }
        
        /// <summary>
        /// Determines increment value for autoincrement key generator.
        /// </summary>
        public CollectionBuilder KeyIncrement(long value)
        {
            _parameters.Long(ParameterName.KeyOptionsIncrement, value);
        	
        	return this;
        }
        
        /// <summary>
        /// Determines initial offset value for autoincrement key generator.
        /// </summary>
        public CollectionBuilder KeyOffset(long value)
        {
            _parameters.Long(ParameterName.KeyOptionsOffset, value);
        	
        	return this;
        }
        
        #endregion
        
        /// <summary>
        /// Determines the number of shards to create for the collection in cluster environment. Default value: 1.
        /// </summary>
        public CollectionBuilder NumberOfShards(int value)
        {
            _parameters.Int(ParameterName.NumberOfShards, value);
        	
        	return this;
        }
        
        /// <summary>
        /// Determines which document attributes are used to specify the target shard for documents in cluster environment. Default value: ["_key"].
        /// </summary>
        public CollectionBuilder ShardKeys(List<string> value)
        {
            _parameters.List(ParameterName.ShardKeys, value);
        	
        	return this;
        }
        
        /// <summary>
        /// Determines whether the return value should include the number of documents in collection. Default value: true.
        /// </summary>
        public CollectionBuilder Count(bool value)
        {
            _parameters.Bool(ParameterName.Count, value);
        	
        	return this;
        }
        
        #region Checksum options
        
        /// <summary>
        /// Determines whether to include document revision ids in the checksum calculation. Default value: false.
        /// </summary>
        public CollectionBuilder WithRevisions(bool value)
        {
            // needs to be in string format
            _parameters.String(ParameterName.WithRevisions, value.ToString().ToLower());
        	
        	return this;
        }
        
        /// <summary>
        /// Determines whether to include document body data in the checksum calculation. Default value: false.
        /// </summary>
        public CollectionBuilder WithData(bool value)
        {
            // needs to be in string format
            _parameters.String(ParameterName.WithData, value.ToString().ToLower());
        	
        	return this;
        }
        
        #endregion
        
        /// <summary>
        /// Determines which attribute will be retuned in the list. Default value: Path.
        /// </summary>
        public CollectionBuilder ReturnListType(AReturnListType value)
        {
            // needs to be string value
            _parameters.String(ParameterName.Type, value.ToString().ToLower());
        	
        	return this;
        }
        
        #endregion
        
        /// <summary>
        /// Creates new collection in current database context.
        /// </summary>
        public async Task<AResult<Dictionary<string, object>>> Create()
        {
            var request = _requestFactory.Create(HttpMethod.Post, ApiBaseUri.Collection, "");
            var document = new Dictionary<string, object>();
            
            // required
            document.String(ParameterName.Name, _collectionName);
            // optional
            Request.TrySetBodyParameter(ParameterName.Type, _parameters, document);
            // optional
            Request.TrySetBodyParameter(ParameterName.WaitForSync, _parameters, document);
            // optional
            Request.TrySetBodyParameter(ParameterName.JournalSize, _parameters, document);
            // optional
            Request.TrySetBodyParameter(ParameterName.DoCompact, _parameters, document);
            // optional
            Request.TrySetBodyParameter(ParameterName.IsSystem, _parameters, document);
            // optional
            Request.TrySetBodyParameter(ParameterName.IsVolatile, _parameters, document);
            // optional
            Request.TrySetBodyParameter(ParameterName.KeyOptionsType, _parameters, document);
            // optional
            Request.TrySetBodyParameter(ParameterName.KeyOptionsAllowUserKeys, _parameters, document);
            // optional
            Request.TrySetBodyParameter(ParameterName.KeyOptionsIncrement, _parameters, document);
            // optional
            Request.TrySetBodyParameter(ParameterName.KeyOptionsOffset, _parameters, document);
            // optional
            Request.TrySetBodyParameter(ParameterName.NumberOfShards, _parameters, document);
            // optional
            Request.TrySetBodyParameter(ParameterName.ShardKeys, _parameters, document);
            
            request.SetBody(document);
            
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
            
            _parameters.Clear();
            
            return result;
        }
    }
}