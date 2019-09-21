using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Arango.Tests;
using ArangoDriver.Client;
using Newtonsoft.Json.Linq;
using NUnit.Framework;

namespace Tests.Operations
{
    [TestFixture()]
    public class CollectionOperationsTests : TestBase
    {
        private ADatabase _db;
        
        [SetUp]
        public async Task Setup()
        {
            await Connection.CreateDatabase(TestDatabaseGeneral);

            _db = Connection.GetDatabase(TestDatabaseGeneral);
        }
        
        #region Create operations
    	
        [Test]
        public async Task Should_create_document_collection()
        {
            var createResult = await _db.CreateCollection(TestDocumentCollectionName).Create();

            Assert.AreEqual(200, createResult.StatusCode);
            Assert.IsTrue(createResult.Success);
            Assert.IsTrue(createResult.HasValue);
            Assert.IsNotEmpty((string) createResult.Value["id"]);
            Assert.AreEqual(TestDocumentCollectionName, createResult.Value["name"]);
            Assert.AreEqual(false, createResult.Value["waitForSync"]);
            //Assert.AreEqual(false, createResult.Value["isVolatile"));
            Assert.AreEqual(false, createResult.Value["isSystem"]);
            Assert.AreEqual((int)ACollectionStatus.Loaded, createResult.Value["status"]);
            Assert.AreEqual((int)ACollectionType.Document, createResult.Value["type"]);
        }
        
        [Test]
        public async Task Should_create_edge_collection()
        {
            var createResult = await _db.CreateCollection(TestEdgeCollectionName)
                .Type(ACollectionType.Edge)
                .Create();

            Assert.AreEqual(200, createResult.StatusCode);
            Assert.IsTrue(createResult.Success);
            Assert.IsTrue(createResult.HasValue);
            Assert.IsNotEmpty((string) createResult.Value["id"]);
            Assert.AreEqual(TestEdgeCollectionName, createResult.Value["name"]);
            Assert.AreEqual(false, createResult.Value["waitForSync"]);
            //Assert.AreEqual(false, createResult.Value["isVolatile"));
            Assert.AreEqual(false, createResult.Value["isSystem"]);
            Assert.AreEqual((int)ACollectionStatus.Loaded, createResult.Value["status"]);
            Assert.AreEqual((int)ACollectionType.Edge, createResult.Value["type"]);
        }
        
        [Test]
        public async Task Should_create_autoincrement_collection()
        {
            var createResult = await _db.CreateCollection(TestDocumentCollectionName)
                .KeyGeneratorType(AKeyGeneratorType.Autoincrement)
                .Create();
            
            Assert.AreEqual(200, createResult.StatusCode);
            Assert.IsTrue(createResult.Success);
            Assert.IsTrue(createResult.HasValue);
            Assert.IsNotEmpty((string) createResult.Value["id"]);
            Assert.AreEqual(TestDocumentCollectionName, createResult.Value["name"]);
            Assert.AreEqual(false, createResult.Value["waitForSync"]);
            //Assert.AreEqual(false, createResult.Value["isVolatile"));
            Assert.AreEqual(false, createResult.Value["isSystem"]);
            Assert.AreEqual((int)ACollectionStatus.Loaded, createResult.Value["status"]);
            Assert.AreEqual((int)ACollectionType.Document, createResult.Value["type"]);


			// create documents and test if their key are incremented accordingly

            var collection = _db.GetCollection<Dictionary<string, object>>(TestDocumentCollectionName);

            var newDocument = new Dictionary<string, object>
            {
                {"Foo", "some string"}, 
                {"Bar", new Dictionary<string, object>() {{"Foo", "string value"}}}
            };

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
            var createResult = await _db.CreateCollection(TestEdgeCollectionName)
                .Create();

            var getResult = await _db.GetCollection<Dictionary<string, object>>((string)createResult.Value["name"])
                .GetInformation();
            
            Assert.AreEqual(200, getResult.StatusCode);
            Assert.IsTrue(getResult.Success);
            Assert.IsTrue(getResult.HasValue);
            Assert.AreEqual(createResult.Value["id"], getResult.Value["id"]);
            Assert.AreEqual(createResult.Value["name"], getResult.Value["name"]);
            Assert.AreEqual(createResult.Value["isSystem"], getResult.Value["isSystem"]);
            Assert.AreEqual(createResult.Value["status"], getResult.Value["status"]);
            Assert.AreEqual(createResult.Value["type"], getResult.Value["type"]);
        }
        
        [Test]
        public async Task Should_get_collection_properties()
        {
            var createResult = await _db.CreateCollection(TestEdgeCollectionName)
                .Create();

            var getResult = await _db.GetCollection<Dictionary<string, object>>((string)createResult.Value["name"])
                .GetProperties();
            
            Assert.AreEqual(200, getResult.StatusCode);
            Assert.IsTrue(getResult.Success);
            Assert.IsTrue(getResult.HasValue);
            Assert.AreEqual(createResult.Value["id"], getResult.Value["id"]);
            Assert.AreEqual(createResult.Value["name"], getResult.Value["name"]);
            Assert.AreEqual(createResult.Value["isSystem"], getResult.Value["isSystem"]);
            //Assert.AreEqual(createResult.Value["isVolatile"), getResult.Value["isVolatile"));
            Assert.AreEqual(createResult.Value["status"], getResult.Value["status"]);
            Assert.AreEqual(createResult.Value["type"], getResult.Value["type"]);
            Assert.AreEqual(createResult.Value["waitForSync"], getResult.Value["waitForSync"]);
            //Assert.IsTrue(getResult.Value["doCompact"));
            //Assert.IsTrue(getResult.Value.Long("journalSize") > 1);
            Assert.AreEqual(AKeyGeneratorType.Traditional.ToString().ToLower(), ((Dictionary<string,object>)getResult.Value["keyOptions"])["type"]);
            Assert.AreEqual(true, ((Dictionary<string,object>)getResult.Value["keyOptions"])["allowUserKeys"]);
        }
        
        [Test]
        public async Task CountDocuments()
        {
            var createResult = await _db.CreateCollection(TestEdgeCollectionName)
                .Create();

            var getResult = await _db.GetCollection<Dictionary<string, object>>((string)createResult.Value["name"])
                .Count();
            
            Assert.AreEqual(200, getResult.StatusCode);
            Assert.IsTrue(getResult.Success);
            Assert.IsTrue(getResult.HasValue);
            Assert.AreEqual(0, getResult.Value);
        }
        
        [Test]
        public async Task Should_get_collection_figures()
        {
            var createResult = await _db.CreateCollection(TestEdgeCollectionName)
                .Create();

            var getResult = await _db.GetCollection<Dictionary<string, object>>((string)createResult.Value["name"])
                .GetFigures();
            
            Assert.AreEqual(200, getResult.StatusCode);
            Assert.IsTrue(getResult.Success);
            Assert.IsTrue(getResult.HasValue);
            Assert.AreEqual(createResult.Value["id"], getResult.Value["id"]);
            Assert.AreEqual(createResult.Value["name"], getResult.Value["name"]);
            Assert.AreEqual(createResult.Value["isSystem"], getResult.Value["isSystem"]);
            //Assert.AreEqual(createResult.Value["isVolatile"), getResult.Value["isVolatile"));
            Assert.AreEqual(createResult.Value["status"], getResult.Value["status"]);
            Assert.AreEqual(createResult.Value["type"], getResult.Value["type"]);
            Assert.AreEqual(createResult.Value["waitForSync"], getResult.Value["waitForSync"]);
            //Assert.IsTrue(getResult.Value["doCompact"));
            //Assert.IsTrue(getResult.Value.Long("journalSize") > 0);
            Assert.AreEqual(AKeyGeneratorType.Traditional.ToString().ToLower(), ((Dictionary<string,object>)getResult.Value["keyOptions"])["type"]);
            Assert.AreEqual(true, ((Dictionary<string,object>)getResult.Value["keyOptions"])["allowUserKeys"]);
            Assert.AreEqual(0, getResult.Value["count"]);
            Assert.IsNotEmpty((IEnumerable) getResult.Value["figures"]);
        }
        
        [Test]
        public async Task Should_get_collection_revision()
        {
            var createResult = await _db.CreateCollection(TestEdgeCollectionName)
                .Create();

            var getResult = await _db.GetCollection<Dictionary<string, object>>((string)createResult.Value["name"])
                .GetRevision();
            
            Assert.AreEqual(200, getResult.StatusCode);
            Assert.IsTrue(getResult.Success);
            Assert.IsTrue(getResult.HasValue);
            Assert.AreEqual(createResult.Value["id"], getResult.Value["id"]);
            Assert.AreEqual(createResult.Value["name"], getResult.Value["name"]);
            Assert.AreEqual(createResult.Value["isSystem"], getResult.Value["isSystem"]);
            Assert.AreEqual(createResult.Value["status"], getResult.Value["status"]);
            Assert.AreEqual(createResult.Value["type"], getResult.Value["type"]);
            Assert.IsNotEmpty((string) getResult.Value["revision"]);
        }
        
        [Test]
        public async Task Should_get_collection_cehcksum()
        {
            var createResult = await _db.CreateCollection(TestEdgeCollectionName)
                .Create();

            var getResult = await _db.GetCollection<Dictionary<string, object>>((string)createResult.Value["name"])
                //.WithData(true)
                //.WithRevisions(true)
                .GetChecksum();
            
            Assert.AreEqual(200, getResult.StatusCode);
            Assert.IsTrue(getResult.Success);
            Assert.IsTrue(getResult.HasValue);
            Assert.AreEqual(createResult.Value["id"], getResult.Value["id"]);
            Assert.AreEqual(createResult.Value["name"], getResult.Value["name"]);
            Assert.AreEqual(createResult.Value["isSystem"], getResult.Value["isSystem"]);
            Assert.AreEqual(createResult.Value["status"], getResult.Value["status"]);
            Assert.AreEqual(createResult.Value["type"], getResult.Value["type"]);
            Assert.IsNotEmpty((string) getResult.Value["revision"]);
            Assert.IsNotEmpty((string) getResult.Value["checksum"]);
        }
        
        [Test]
        public async Task Should_get_all_indexes_in_collection()
        {
            await _db
                .CreateCollection(TestDocumentCollectionName)
                .Type(ACollectionType.Document)
                .Create();
            
            var operationResult = await _db.GetCollection<Dictionary<string, object>>(TestDocumentCollectionName)
                .GetAllIndexes();
            
            Assert.AreEqual(200, operationResult.StatusCode);
            Assert.IsTrue(operationResult.Success);
            Assert.IsTrue(operationResult.HasValue);
            Assert.IsNotEmpty((IEnumerable) (operationResult.Value["indexes"]));
            Assert.IsNotNull(operationResult.Value["identifiers"]);
        }
        
        #endregion
        
        #region Update/change operations

        [Test]
        public async Task Should_truncate_collection()
        {
            var createResult = await _db.CreateCollection(TestEdgeCollectionName)
                .Create();

            var clearResult = await _db.GetCollection<Dictionary<string, object>>((string) createResult.Value["name"])
                .Truncate();

            Assert.AreEqual(200, clearResult.StatusCode);
            Assert.IsTrue(clearResult.Success);
            Assert.IsTrue(clearResult.HasValue);
            Assert.AreEqual(createResult.Value["id"], clearResult.Value["id"]);
            Assert.AreEqual(createResult.Value["name"], clearResult.Value["name"]);
            Assert.AreEqual(createResult.Value["isSystem"], clearResult.Value["isSystem"]);
            Assert.AreEqual(createResult.Value["status"], clearResult.Value["status"]);
            Assert.AreEqual(createResult.Value["type"], clearResult.Value["type"]);
        }

        /*
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
                .ChangeProperties(createResult.Value["name"));
            
            Assert.AreEqual(200, operationResult.StatusCode);
            Assert.IsTrue(operationResult.Success);
            Assert.IsTrue(operationResult.HasValue);
            Assert.AreEqual(createResult.Value["id"), operationResult.Value["id"));
            Assert.AreEqual(createResult.Value["name"), operationResult.Value["name"));
            Assert.AreEqual(createResult.Value["isSystem"), operationResult.Value["isSystem"));
            Assert.AreEqual(createResult.Value["status"), operationResult.Value["status"));
            Assert.AreEqual(createResult.Value["type"), operationResult.Value["type"));
            Assert.IsFalse(operationResult.Value["isVolatile"));
            Assert.IsTrue(operationResult.Value["doCompact"));
            Assert.AreEqual((int)AKeyGeneratorType.Traditional, operationResult.Value.Enum<AKeyGeneratorType>("keyOptions.type"));
            Assert.IsTrue(operationResult.Value["keyOptions.allowUserKeys"));
            Assert.IsTrue(operationResult.Value["waitForSync"));
            Assert.IsTrue(operationResult.Value.Long("journalSize") == journalSize);
        }*/
        
        [Test]
        public async Task Should_rename_collection()
        {
            var createResult = await _db.CreateCollection(TestEdgeCollectionName)
                .Create();

            var operationResult = await _db.GetCollection<Dictionary<string, object>>((string)createResult.Value["name"])
                .Rename(TestEdgeCollectionName);
            
            Assert.AreEqual(200, operationResult.StatusCode);
            Assert.IsTrue(operationResult.Success);
            Assert.IsTrue(operationResult.HasValue);
            Assert.AreEqual(createResult.Value["id"], operationResult.Value["id"]);
            Assert.AreEqual(TestEdgeCollectionName, operationResult.Value["name"]);
            Assert.AreEqual(createResult.Value["isSystem"], operationResult.Value["isSystem"]);
            Assert.AreEqual(createResult.Value["status"], operationResult.Value["status"]);
            Assert.AreEqual(createResult.Value["type"], operationResult.Value["type"]);
        }
        
        #endregion
        
        #region Delete operations
        
        [Test]
        public async Task Should_delete_collection()
        {
            var createResult = await _db.CreateCollection(TestEdgeCollectionName)
                .Create();

            var deleteResult = await _db.DropCollection((string)createResult.Value["name"]);
            
            Assert.AreEqual(200, deleteResult.StatusCode);
            Assert.IsTrue(deleteResult.Success);
            Assert.IsTrue(deleteResult.HasValue);
            Assert.AreEqual(createResult.Value["id"], deleteResult.Value["id"]);
        }
        
        #endregion
    }
}
