using System.Collections.Generic;
using System.Threading.Tasks;
using Arango.Tests;
using ArangoDriver.Client;
using ArangoDriver.External.dictator;
using NUnit.Framework;

namespace Tests.CollectionOperations
{
    [TestFixture()]
    public class CollectionOperationsTests : TestBase
    {
        private ADatabase db;
        
        [SetUp]
        public async Task Setup()
        {
            await Connection.CreateDatabase(TestDatabaseGeneral);

            db = Connection.GetDatabase(TestDatabaseGeneral);
        }
        
        #region Create operations
    	
        [Test]
        public async Task Should_create_document_collection()
        {
            var createResult = await db.CreateCollection(TestDocumentCollectionName).Create();

            Assert.AreEqual(200, createResult.StatusCode);
            Assert.IsTrue(createResult.Success);
            Assert.IsTrue(createResult.HasValue);
            Assert.AreEqual(true, createResult.Value.IsString("id"));
            Assert.AreEqual(TestDocumentCollectionName, createResult.Value.String("name"));
            Assert.AreEqual(false, createResult.Value.Bool("waitForSync"));
            //Assert.AreEqual(false, createResult.Value.Bool("isVolatile"));
            Assert.AreEqual(false, createResult.Value.Bool("isSystem"));
            Assert.AreEqual(ACollectionStatus.Loaded, createResult.Value.Enum<ACollectionStatus>("status"));
            Assert.AreEqual(ACollectionType.Document, createResult.Value.Enum<ACollectionType>("type"));
        }
        
        [Test]
        public async Task Should_create_edge_collection()
        {
            var createResult = await db.CreateCollection(TestEdgeCollectionName)
                .Type(ACollectionType.Edge)
                .Create();

            Assert.AreEqual(200, createResult.StatusCode);
            Assert.IsTrue(createResult.Success);
            Assert.IsTrue(createResult.HasValue);
            Assert.AreEqual(true, createResult.Value.IsString("id"));
            Assert.AreEqual(TestEdgeCollectionName, createResult.Value.String("name"));
            Assert.AreEqual(false, createResult.Value.Bool("waitForSync"));
            //Assert.AreEqual(false, createResult.Value.Bool("isVolatile"));
            Assert.AreEqual(false, createResult.Value.Bool("isSystem"));
            Assert.AreEqual(ACollectionStatus.Loaded, createResult.Value.Enum<ACollectionStatus>("status"));
            Assert.AreEqual(ACollectionType.Edge, createResult.Value.Enum<ACollectionType>("type"));
        }
        
        [Test]
        public async Task Should_create_autoincrement_collection()
        {
            var createResult = await db.CreateCollection(TestDocumentCollectionName)
                .KeyGeneratorType(AKeyGeneratorType.Autoincrement)
                .Create();
            
            Assert.AreEqual(200, createResult.StatusCode);
            Assert.IsTrue(createResult.Success);
            Assert.IsTrue(createResult.HasValue);
            Assert.AreEqual(true, createResult.Value.IsString("id"));
            Assert.AreEqual(TestDocumentCollectionName, createResult.Value.String("name"));
            Assert.AreEqual(false, createResult.Value.Bool("waitForSync"));
            //Assert.AreEqual(false, createResult.Value.Bool("isVolatile"));
            Assert.AreEqual(false, createResult.Value.Bool("isSystem"));
            Assert.AreEqual(ACollectionStatus.Loaded, createResult.Value.Enum<ACollectionStatus>("status"));
            Assert.AreEqual(ACollectionType.Document, createResult.Value.Enum<ACollectionType>("type"));


			// create documents and test if their key are incremented accordingly

            var collection = db.GetCollection(TestDocumentCollectionName);
			
            var newDocument = new Dictionary<string, object>()
                .String("foo", "some string")
                .Document("bar", new Dictionary<string, object>().String("foo", "string value"));
            
            var doc1Result = await collection
                .Insert()
                .Document(newDocument);
            
            Assert.AreEqual(202, doc1Result.StatusCode);
            Assert.IsTrue(doc1Result.Success);
            Assert.IsTrue(doc1Result.HasValue);
            Assert.AreEqual(TestDocumentCollectionName + "/" + 1, doc1Result.Value.ID());
            Assert.AreEqual("1", doc1Result.Value.Key());
            Assert.IsFalse(string.IsNullOrEmpty(doc1Result.Value.Rev()));
            
            var doc2Result = await collection
                .Insert()
                .Document(newDocument);
            
            Assert.AreEqual(202, doc2Result.StatusCode);
            Assert.IsTrue(doc2Result.Success);
            Assert.IsTrue(doc2Result.HasValue);
            Assert.AreEqual(TestDocumentCollectionName + "/" + 2, doc2Result.Value.ID());
            Assert.AreEqual("2", doc2Result.Value.Key());
            Assert.IsFalse(string.IsNullOrEmpty(doc2Result.Value.Rev()));
        }
        
        #endregion
        
        #region Get operations
        
        [Test]
        public async Task Should_get_collection()
        {
            var createResult = await db.CreateCollection(TestEdgeCollectionName)
                .Create();

            var getResult = await db.GetCollection(createResult.Value.String("name"))
                .Get();
            
            Assert.AreEqual(200, getResult.StatusCode);
            Assert.IsTrue(getResult.Success);
            Assert.IsTrue(getResult.HasValue);
            Assert.AreEqual(createResult.Value.String("id"), getResult.Value.String("id"));
            Assert.AreEqual(createResult.Value.String("name"), getResult.Value.String("name"));
            Assert.AreEqual(createResult.Value.Bool("isSystem"), getResult.Value.Bool("isSystem"));
            Assert.AreEqual(createResult.Value.Int("status"), getResult.Value.Int("status"));
            Assert.AreEqual(createResult.Value.Int("type"), getResult.Value.Int("type"));
        }
        
        [Test]
        public async Task Should_get_collection_properties()
        {
            var createResult = await db.CreateCollection(TestEdgeCollectionName)
                .Create();

            var getResult = await db.GetCollection(createResult.Value.String("name"))
                .GetProperties();
            
            Assert.AreEqual(200, getResult.StatusCode);
            Assert.IsTrue(getResult.Success);
            Assert.IsTrue(getResult.HasValue);
            Assert.AreEqual(createResult.Value.String("id"), getResult.Value.String("id"));
            Assert.AreEqual(createResult.Value.String("name"), getResult.Value.String("name"));
            Assert.AreEqual(createResult.Value.Bool("isSystem"), getResult.Value.Bool("isSystem"));
            //Assert.AreEqual(createResult.Value.Bool("isVolatile"), getResult.Value.Bool("isVolatile"));
            Assert.AreEqual(createResult.Value.Int("status"), getResult.Value.Int("status"));
            Assert.AreEqual(createResult.Value.Int("type"), getResult.Value.Int("type"));
            Assert.AreEqual(createResult.Value.Bool("waitForSync"), getResult.Value.Bool("waitForSync"));
            //Assert.IsTrue(getResult.Value.Bool("doCompact"));
            //Assert.IsTrue(getResult.Value.Long("journalSize") > 1);
            Assert.AreEqual(AKeyGeneratorType.Traditional, getResult.Value.Enum<AKeyGeneratorType>("keyOptions.type"));
            Assert.AreEqual(true, getResult.Value.Bool("keyOptions.allowUserKeys"));
        }
        
        [Test]
        public async Task Should_get_collection_count()
        {
            var createResult = await db.CreateCollection(TestEdgeCollectionName)
                .Create();

            var getResult = await db.GetCollection(createResult.Value.String("name"))
                .GetCount();
            
            Assert.AreEqual(200, getResult.StatusCode);
            Assert.IsTrue(getResult.Success);
            Assert.IsTrue(getResult.HasValue);
            Assert.AreEqual(createResult.Value.String("id"), getResult.Value.String("id"));
            Assert.AreEqual(createResult.Value.String("name"), getResult.Value.String("name"));
            Assert.AreEqual(createResult.Value.Bool("isSystem"), getResult.Value.Bool("isSystem"));
            //Assert.AreEqual(createResult.Value.Bool("isVolatile"), getResult.Value.Bool("isVolatile"));
            Assert.AreEqual(createResult.Value.Int("status"), getResult.Value.Int("status"));
            Assert.AreEqual(createResult.Value.Int("type"), getResult.Value.Int("type"));
            Assert.AreEqual(createResult.Value.Bool("waitForSync"), getResult.Value.Bool("waitForSync"));
            //Assert.IsTrue(getResult.Value.Bool("doCompact"));
            //Assert.IsTrue(getResult.Value.Long("journalSize") > 1);
            Assert.AreEqual(AKeyGeneratorType.Traditional, getResult.Value.Enum<AKeyGeneratorType>("keyOptions.type"));
            Assert.AreEqual(true, getResult.Value.Bool("keyOptions.allowUserKeys"));
            Assert.AreEqual(0, getResult.Value.Long("count"));
        }
        
        [Test]
        public async Task Should_get_collection_figures()
        {
            var createResult = await db.CreateCollection(TestEdgeCollectionName)
                .Create();

            var getResult = await db.GetCollection(createResult.Value.String("name"))
                .GetFigures();
            
            Assert.AreEqual(200, getResult.StatusCode);
            Assert.IsTrue(getResult.Success);
            Assert.IsTrue(getResult.HasValue);
            Assert.AreEqual(createResult.Value.String("id"), getResult.Value.String("id"));
            Assert.AreEqual(createResult.Value.String("name"), getResult.Value.String("name"));
            Assert.AreEqual(createResult.Value.Bool("isSystem"), getResult.Value.Bool("isSystem"));
            //Assert.AreEqual(createResult.Value.Bool("isVolatile"), getResult.Value.Bool("isVolatile"));
            Assert.AreEqual(createResult.Value.Int("status"), getResult.Value.Int("status"));
            Assert.AreEqual(createResult.Value.Int("type"), getResult.Value.Int("type"));
            Assert.AreEqual(createResult.Value.Bool("waitForSync"), getResult.Value.Bool("waitForSync"));
            //Assert.IsTrue(getResult.Value.Bool("doCompact"));
            //Assert.IsTrue(getResult.Value.Long("journalSize") > 0);
            Assert.AreEqual(AKeyGeneratorType.Traditional, getResult.Value.Enum<AKeyGeneratorType>("keyOptions.type"));
            Assert.AreEqual(true, getResult.Value.Bool("keyOptions.allowUserKeys"));
            Assert.AreEqual(0, getResult.Value.Long("count"));
            Assert.IsTrue(getResult.Value.Document("figures").Count > 0);
        }
        
        [Test]
        public async Task Should_get_collection_revision()
        {
            var createResult = await db.CreateCollection(TestEdgeCollectionName)
                .Create();

            var getResult = await db.GetCollection(createResult.Value.String("name"))
                .GetRevision();
            
            Assert.AreEqual(200, getResult.StatusCode);
            Assert.IsTrue(getResult.Success);
            Assert.IsTrue(getResult.HasValue);
            Assert.AreEqual(createResult.Value.String("id"), getResult.Value.String("id"));
            Assert.AreEqual(createResult.Value.String("name"), getResult.Value.String("name"));
            Assert.AreEqual(createResult.Value.Bool("isSystem"), getResult.Value.Bool("isSystem"));
            Assert.AreEqual(createResult.Value.Int("status"), getResult.Value.Int("status"));
            Assert.AreEqual(createResult.Value.Int("type"), getResult.Value.Int("type"));
            Assert.IsTrue(getResult.Value.IsString("revision"));
        }
        
        [Test]
        public async Task Should_get_collection_cehcksum()
        {
            var createResult = await db.CreateCollection(TestEdgeCollectionName)
                .Create();

            var getResult = await db.GetCollection(createResult.Value.String("name"))
                //.WithData(true)
                //.WithRevisions(true)
                .GetChecksum();
            
            Assert.AreEqual(200, getResult.StatusCode);
            Assert.IsTrue(getResult.Success);
            Assert.IsTrue(getResult.HasValue);
            Assert.AreEqual(createResult.Value.String("id"), getResult.Value.String("id"));
            Assert.AreEqual(createResult.Value.String("name"), getResult.Value.String("name"));
            Assert.AreEqual(createResult.Value.Bool("isSystem"), getResult.Value.Bool("isSystem"));
            Assert.AreEqual(createResult.Value.Int("status"), getResult.Value.Int("status"));
            Assert.AreEqual(createResult.Value.Int("type"), getResult.Value.Int("type"));
            Assert.IsTrue(getResult.Value.IsString("revision"));
            Assert.IsTrue(getResult.Value.IsString("checksum"));
        }
        
        [Test]
        public async Task Should_get_all_indexes_in_collection()
        {
            var createResult = await db
                .CreateCollection(TestDocumentCollectionName)
                .Type(ACollectionType.Document)
                .Create();
            
            var operationResult = await db.GetCollection(TestDocumentCollectionName)
                .GetAllIndexes();
            
            Assert.AreEqual(200, operationResult.StatusCode);
            Assert.IsTrue(operationResult.Success);
            Assert.IsTrue(operationResult.HasValue);
            Assert.IsTrue(operationResult.Value.Size("indexes") > 0);
            Assert.IsTrue(operationResult.Value.IsDocument("identifiers"));
        }
        
        #endregion
        
        #region Update/change operations
        
        [Test]
        public async Task Should_truncate_collection()
        {
            var createResult = await db.CreateCollection(TestEdgeCollectionName)
                .Create();

            var clearResult = await db.GetCollection(createResult.Value.String("name"))
                .Truncate();
            
            Assert.AreEqual(200, clearResult.StatusCode);
            Assert.IsTrue(clearResult.Success);
            Assert.IsTrue(clearResult.HasValue);
            Assert.AreEqual(createResult.Value.String("id"), clearResult.Value.String("id"));
            Assert.AreEqual(createResult.Value.String("name"), clearResult.Value.String("name"));
            Assert.AreEqual(createResult.Value.Bool("isSystem"), clearResult.Value.Bool("isSystem"));
            Assert.AreEqual(createResult.Value.Int("status"), clearResult.Value.Int("status"));
            Assert.AreEqual(createResult.Value.Int("type"), clearResult.Value.Int("type"));
        }
        
        /*[Test]
        public void Should_load_collection()
        {
            CreateTestDatabase(TestDatabaseGeneral);

            var db = new ADatabase(Alias);

            var createResult = db.Collection
                .Create(TestDocumentCollectionName);

            var operationResult = db.Collection
                .Load(createResult.Value.String("name"));
            
            Assert.AreEqual(200, operationResult.StatusCode);
            Assert.IsTrue(operationResult.Success);
            Assert.IsTrue(operationResult.HasValue);
            Assert.AreEqual(createResult.Value.String("id"), operationResult.Value.String("id"));
            Assert.AreEqual(createResult.Value.String("name"), operationResult.Value.String("name"));
            Assert.AreEqual(createResult.Value.Bool("isSystem"), operationResult.Value.Bool("isSystem"));
            Assert.AreEqual(createResult.Value.Int("status"), operationResult.Value.Int("status"));
            Assert.AreEqual(createResult.Value.Int("type"), operationResult.Value.Int("type"));
            Assert.IsTrue(operationResult.Value.Long("count") == 0);
        }
        
        [Test]
        public void Should_load_collection_without_count()
        {
            CreateTestDatabase(TestDatabaseGeneral);

            var db = new ADatabase(Alias);

            var createResult = db.Collection
                .Create(TestDocumentCollectionName);

            var operationResult = db.Collection
                .Count(false)
                .Load(createResult.Value.String("name"));
            
            Assert.AreEqual(200, operationResult.StatusCode);
            Assert.IsTrue(operationResult.Success);
            Assert.IsTrue(operationResult.HasValue);
            Assert.AreEqual(createResult.Value.String("id"), operationResult.Value.String("id"));
            Assert.AreEqual(createResult.Value.String("name"), operationResult.Value.String("name"));
            Assert.AreEqual(createResult.Value.Bool("isSystem"), operationResult.Value.Bool("isSystem"));
            Assert.AreEqual(ACollectionStatus.Loaded, operationResult.Value.Enum<ACollectionStatus>("status"));
            Assert.AreEqual(createResult.Value.Int("type"), operationResult.Value.Int("type"));
            Assert.IsFalse(operationResult.Value.Has("count"));
        }
        
        [Test]
        public void Should_unload_collection()
        {
            CreateTestDatabase(TestDatabaseGeneral);

            var db = new ADatabase(Alias);

            var createResult = db.Collection
                .Create(TestDocumentCollectionName);

            var operationResult = db.Collection
                .Unload(createResult.Value.String("name"));
            
            Assert.AreEqual(200, operationResult.StatusCode);
            Assert.IsTrue(operationResult.Success);
            Assert.IsTrue(operationResult.HasValue);
            Assert.AreEqual(createResult.Value.String("id"), operationResult.Value.String("id"));
            Assert.AreEqual(createResult.Value.String("name"), operationResult.Value.String("name"));
            Assert.AreEqual(createResult.Value.Bool("isSystem"), operationResult.Value.Bool("isSystem"));
            Assert.IsTrue(operationResult.Value.Enum<ACollectionStatus>("status") == ACollectionStatus.Unloaded || operationResult.Value.Enum<ACollectionStatus>("status") == ACollectionStatus.Unloading);
            Assert.AreEqual(createResult.Value.Int("type"), operationResult.Value.Int("type"));
        }
        
        [Test]
        public void Should_change_collection_properties()
        {
            CreateTestDatabase(TestDatabaseGeneral);

            var db = new ADatabase(Alias);

            var createResult = db.Collection
                .Create(TestDocumentCollectionName);

            const long journalSize = 199999999;
            
            var operationResult = db.Collection
                .WaitForSync(true)
                .JournalSize(journalSize)
                .ChangeProperties(createResult.Value.String("name"));
            
            Assert.AreEqual(200, operationResult.StatusCode);
            Assert.IsTrue(operationResult.Success);
            Assert.IsTrue(operationResult.HasValue);
            Assert.AreEqual(createResult.Value.String("id"), operationResult.Value.String("id"));
            Assert.AreEqual(createResult.Value.String("name"), operationResult.Value.String("name"));
            Assert.AreEqual(createResult.Value.Bool("isSystem"), operationResult.Value.Bool("isSystem"));
            Assert.AreEqual(createResult.Value.Int("status"), operationResult.Value.Int("status"));
            Assert.AreEqual(createResult.Value.Int("type"), operationResult.Value.Int("type"));
            Assert.IsFalse(operationResult.Value.Bool("isVolatile"));
            Assert.IsTrue(operationResult.Value.Bool("doCompact"));
            Assert.AreEqual(AKeyGeneratorType.Traditional, operationResult.Value.Enum<AKeyGeneratorType>("keyOptions.type"));
            Assert.IsTrue(operationResult.Value.Bool("keyOptions.allowUserKeys"));
            Assert.IsTrue(operationResult.Value.Bool("waitForSync"));
            Assert.IsTrue(operationResult.Value.Long("journalSize") == journalSize);
        }*/
        
        [Test]
        public async Task Should_rename_collection()
        {
            var createResult = await db.CreateCollection(TestEdgeCollectionName)
                .Create();

            var operationResult = await db.GetCollection(createResult.Value.String("name"))
                .Rename(TestEdgeCollectionName);
            
            Assert.AreEqual(200, operationResult.StatusCode);
            Assert.IsTrue(operationResult.Success);
            Assert.IsTrue(operationResult.HasValue);
            Assert.AreEqual(createResult.Value.String("id"), operationResult.Value.String("id"));
            Assert.AreEqual(TestEdgeCollectionName, operationResult.Value.String("name"));
            Assert.AreEqual(createResult.Value.Bool("isSystem"), operationResult.Value.Bool("isSystem"));
            Assert.AreEqual(createResult.Value.Int("status"), operationResult.Value.Int("status"));
            Assert.AreEqual(createResult.Value.Int("type"), operationResult.Value.Int("type"));
        }
        
        #endregion
        
        #region Delete operations
        
        [Test]
        public async Task Should_delete_collection()
        {
            var createResult = await db.CreateCollection(TestEdgeCollectionName)
                .Create();

            var deleteResult = await db.DropCollection(createResult.Value.String("name"));
            
            Assert.AreEqual(200, deleteResult.StatusCode);
            Assert.IsTrue(deleteResult.Success);
            Assert.IsTrue(deleteResult.HasValue);
            Assert.AreEqual(createResult.Value.String("id"), deleteResult.Value.String("id"));
        }
        
        #endregion
    }
}
