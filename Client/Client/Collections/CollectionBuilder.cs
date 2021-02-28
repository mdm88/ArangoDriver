using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using ArangoDriver.Exceptions;
using ArangoDriver.Protocol;
using ArangoDriver.Protocol.Requests;

namespace ArangoDriver.Client
{
    public class CollectionBuilder
    {
	    private readonly RequestFactory _requestFactory;
        private readonly ADatabase _connection;
        private readonly CollectionCreateRequest _parameters;
        
        internal CollectionBuilder(RequestFactory requestFactory, ADatabase connection, string collectionName)
        {
	        _requestFactory = requestFactory;
            _connection = connection;
            _parameters = new CollectionCreateRequest()
            {
	            Name = collectionName
            };
        }
        
        #region Parameters
        
        /// <summary>
        /// Determines type of the collection. Default value: Document.
        /// </summary>
        public CollectionBuilder Type(ACollectionType value)
        {
            _parameters.Type = (int)value;
        	
        	return this;
        }
        
        /// <summary>
        /// Determines whether or not to wait until data are synchronised to disk. Default value: false.
        /// </summary>
        public CollectionBuilder WaitForSync(bool value)
        {
            _parameters.WaitForSync = value;
        	
        	return this;
        }
        
        /// <summary>
        /// Determines maximum size of a journal or datafile in bytes. Default value: server configured.
        /// </summary>
        public CollectionBuilder JournalSize(long value)
        {
            _parameters.JournalSize = value;
        	
        	return this;
        }
        
        /// <summary>
        /// Determines whether the collection will be compacted. Default value: true.
        /// </summary>
        public CollectionBuilder DoCompact(bool value)
        {
            _parameters.DoCompact = value;
        	
        	return this;
        }
        
        /// <summary>
        /// Determines whether the collection is a system collection. Default value: false.
        /// </summary>
        public CollectionBuilder IsSystem(bool value)
        {
            _parameters.IsSystem = value;
        	
        	return this;
        }
        
        /// <summary>
        /// Determines whether the collection data is kept in-memory only and not made persistent. Default value: false.
        /// </summary>
        public CollectionBuilder IsVolatile(bool value)
        {
            _parameters.IsVolatile = value;
        	
        	return this;
        }
        
        #region Key options
        
        /// <summary>
        /// Determines the type of the key generator. Default value: Traditional.
        /// </summary>
        public CollectionBuilder KeyGeneratorType(AKeyGeneratorType value)
        {
	        if (_parameters.KeyOptions == null)
		        _parameters.KeyOptions = new KeyOptions();

            // needs to be in string format - set enum format explicitely to override global setting
            _parameters.KeyOptions.Type = value.ToString().ToLower();
        	
        	return this;
        }
        
        /// <summary>
        /// Determines whether it is allowed to supply custom key values in the _key attribute of a document. Default value: true.
        /// </summary>
        public CollectionBuilder AllowUserKeys(bool value)
        {
	        if (_parameters.KeyOptions == null)
		        _parameters.KeyOptions = new KeyOptions();

            _parameters.KeyOptions.AllowUserKeys = value;
        	
        	return this;
        }
        
        /// <summary>
        /// Determines increment value for autoincrement key generator.
        /// </summary>
        public CollectionBuilder KeyIncrement(long value)
        {
	        if (_parameters.KeyOptions == null)
		        _parameters.KeyOptions = new KeyOptions();

            _parameters.KeyOptions.Increment = value;
        	
        	return this;
        }
        
        /// <summary>
        /// Determines initial offset value for autoincrement key generator.
        /// </summary>
        public CollectionBuilder KeyOffset(long value)
        {
	        if (_parameters.KeyOptions == null)
		        _parameters.KeyOptions = new KeyOptions();

            _parameters.KeyOptions.Offset = value;
        	
        	return this;
        }
        
        #endregion
        
        /// <summary>
        /// Determines the number of shards to create for the collection in cluster environment. Default value: 1.
        /// </summary>
        public CollectionBuilder NumberOfShards(int value)
        {
            _parameters.NumberOfShards = value;
        	
        	return this;
        }
        
        /// <summary>
        /// Determines which document attributes are used to specify the target shard for documents in cluster environment. Default value: ["_key"].
        /// </summary>
        public CollectionBuilder ShardKeys(List<string> value)
        {
            _parameters.ShardKeys = value;
        	
        	return this;
        }
        
        /// <summary>
        /// determines how many copies of each shard are kept on different DBServers. Default value: 1
        /// </summary>
        public CollectionBuilder ReplicationFactor(int value)
        {
            _parameters.ReplicationFactor = value;
        	
        	return this;
        }
        
        /// <summary>
        /// determines how many copies of each shard are kept on different DBServers. Default value: 1
        /// </summary>
        public CollectionBuilder ShardingStrategy(AShardingStrategy value)
        {
	        switch (value)
	        {
		        case AShardingStrategy.Hash:
			        _parameters.ShardingStrategy = "hash";
			        break;
		        case AShardingStrategy.CommunityCompat:
			        _parameters.ShardingStrategy = "community-compat";
			        break;
		        case AShardingStrategy.EnterpriseCompat:
			        _parameters.ShardingStrategy = "enterprise-compat";
			        break;
		        case AShardingStrategy.EnterpriseHashSmartEdge:
			        _parameters.ShardingStrategy = "enterprise-hash-smart-edge";
			        break;
		        case AShardingStrategy.EnterpriseSmartEdgeCompat:
			        _parameters.ShardingStrategy = "enterprise-smart-edge-compat";
			        break;
	        }
        	
	        return this;
        }
        
        #endregion
        
        /// <summary>
        /// Creates new collection in current database context.
        /// </summary>
        public async Task<AResult<Dictionary<string, object>>> Create()
        {
            var request = _requestFactory.Create(HttpMethod.Post, ApiBaseUri.Collection, "");
            
            request.SetBody(_parameters);

            var result = await _connection.RequestQuery<Dictionary<string, Object>>(request);
            
            if (!result.Success)
            {
			    throw new ArangoException();
            }
            
            return result;
        }
    }
}