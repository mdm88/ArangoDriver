using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Arango.Tests;
using ArangoDriver.Client;
using ArangoDriver.External.dictator;
using NUnit.Framework;

namespace Tests.DocumentOperations
{
    [TestFixture]
    public class DocumentOperationsTests : TestBase
    {
        private ADatabase _db;
        
        [SetUp]
        public async Task Setup()
        {
            await Connection.CreateDatabase(TestDatabaseGeneral);

            _db = Connection.GetDatabase(TestDatabaseGeneral);

            await _db.CreateCollection(TestDocumentCollectionName)
                .Type(ACollectionType.Document)
                .Create();
        }

        private async Task<List<Dictionary<string, object>>> InsertTestData()
        {
            var collection = _db.GetCollection<Dictionary<string, object>>(TestDocumentCollectionName);

            var documents = new List<Dictionary<string, object>>();
         	
            var document1 = new Dictionary<string, object>()
                .String("Foo", "string value one")
                .Int("Bar", 1);
        	
            var document2 = new Dictionary<string, object>()
                .String("Foo", "string value two")
                .Int("Bar", 2);
        	
            var createResult1 = await collection.Insert().Document(document1);
        	
            document1.Merge(createResult1.Value);
        	
            var createResult2 = await collection.Insert().Document(document2);
        	
            document2.Merge(createResult2.Value);
        	
            documents.Add(document1);
            documents.Add(document2);
        	
            return documents;
        }
        
        #region Create operations
        
        [Test]
        public async Task CreateDocument()
        {
            var collection = _db.GetCollection<Dictionary<string, object>>(TestDocumentCollectionName);
            
            var document = new Dictionary<string, object>()
        		.String("Foo", "Foo string value")
        		.Int("Bar", 12345);

            var createResult = await collection
                .Insert()
                .Document(document);
            
            Assert.AreEqual(202, createResult.StatusCode);
            Assert.IsTrue(createResult.Success);
            Assert.IsTrue(createResult.HasValue);
            Assert.IsTrue(createResult.Value.IsString("_id"));
            Assert.IsTrue(createResult.Value.IsString("_key"));
            Assert.IsTrue(createResult.Value.IsString("_rev"));
        }

        [Test]
        public async Task CreateDocumentWithReturnNew()
        {
            var collection = _db.GetCollection<Dictionary<string, object>>(TestDocumentCollectionName);

            var document = new Dictionary<string, object>()
                .String("Foo", "Foo string value")
                .Int("Bar", 12345);

            var createResult = await collection
                .Insert()
                .ReturnNew()
                .Document(document);

            Assert.AreEqual(202, createResult.StatusCode);
            Assert.IsTrue(createResult.Success);
            Assert.IsTrue(createResult.HasValue);
            Assert.IsTrue(createResult.Value.IsString("_id"));
            Assert.IsTrue(createResult.Value.IsString("_key"));
            Assert.IsTrue(createResult.Value.IsString("_rev"));
            Assert.AreEqual(document.String("Foo"), createResult.Value.String("Foo"));
            Assert.AreEqual(document.Int("Bar"), createResult.Value.Int("Bar"));
        }

        [Test]
        public async Task CreateDocumentWithWaitForSync()
        {
            var collection = _db.GetCollection<Dictionary<string, object>>(TestDocumentCollectionName);

            var document = new Dictionary<string, object>()
        		.String("Foo", "Foo string value")
        		.Int("Bar", 12345);

            var createResult = await collection
                .Insert()
                .WaitForSync(true)
                .Document(document);
            
            Assert.AreEqual(201, createResult.StatusCode);
            Assert.IsTrue(createResult.Success);
            Assert.IsTrue(createResult.HasValue);
            Assert.IsTrue(createResult.Value.IsString("_id"));
            Assert.IsTrue(createResult.Value.IsString("_key"));
            Assert.IsTrue(createResult.Value.IsString("_rev"));
        }

        [Test]
        public async Task CreateDocumentFromGenericObject()
        {
            var collection = _db.GetCollection<Dummy>(TestDocumentCollectionName);

            var dummy = new Dummy();
            dummy.Foo = "Foo string value";
            dummy.Bar = 12345;
         
            var createResult = await collection
                .Insert()
                .Document(dummy);
            
            Assert.AreEqual(202, createResult.StatusCode);
            Assert.IsTrue(createResult.Success);
            Assert.IsTrue(createResult.HasValue);
            Assert.IsNotNull(createResult.Value.Id);
            Assert.IsNotNull(createResult.Value.Key);
            Assert.IsNotNull(createResult.Value.Revision);
            
            var getResult = await collection
                .Get(createResult.Value.Id);
            
            Assert.AreEqual(getResult.Value.Id, createResult.Value.Id);
            Assert.AreEqual(getResult.Value.Key, createResult.Value.Key);
            Assert.AreEqual(getResult.Value.Revision, createResult.Value.Revision);
            Assert.AreEqual(getResult.Value.Foo, dummy.Foo);
            Assert.AreEqual(getResult.Value.Bar, dummy.Bar);
            Assert.AreEqual(getResult.Value.Baz, dummy.Baz);
        }
        
        [Test]
        public async Task CreateDocumentFromInterface()
        {
            var collection = _db.GetCollection<IFoo>(TestDocumentCollectionName);

            var dummy = new Dummy();
            dummy.Foo = "Foo string value";
            dummy.Bar = 12345;
         
            var createResult = await collection
                .Insert()
                .ReturnNew()
                .Document(dummy);
            
            Assert.AreEqual(202, createResult.StatusCode);
            Assert.IsTrue(createResult.Success);
            Assert.IsTrue(createResult.HasValue);
            Assert.IsNotNull(createResult.Value.Id);
            
            var getResult = await collection
                .Get(createResult.Value.Id);
            
            Assert.AreEqual(getResult.Value.Id, createResult.Value.Id);
            Assert.AreEqual(((Dummy)getResult.Value).Key, ((Dummy)createResult.Value).Key);
            Assert.AreEqual(((Dummy)getResult.Value).Revision, ((Dummy)createResult.Value).Revision);
            Assert.AreEqual(getResult.Value.Foo, dummy.Foo);
            Assert.AreEqual(((Dummy)getResult.Value).Bar, dummy.Bar);
            Assert.AreEqual(((Dummy)getResult.Value).Baz, dummy.Baz);
        }
        
        [Test]
        public async Task CreateDocumentWithCustomId()
        {
            var collection = _db.GetCollection<Dictionary<string, object>>(TestDocumentCollectionName);

            var document = new Dictionary<string, object>()
                .String("_key", "1234-5678")
        		.String("Foo", "Foo string value")
        		.Int("Bar", 12345);

            var createResult = await collection
                .Insert()
                .Document(document);
            
            Assert.AreEqual(202, createResult.StatusCode);
            Assert.IsTrue(createResult.Success);
            Assert.IsTrue(createResult.HasValue);
            Assert.AreEqual(TestDocumentCollectionName + "/" + document.Key(), createResult.Value.ID());
            Assert.AreEqual(document.Key(), createResult.Value.Key());
            Assert.IsTrue(createResult.Value.IsString("_rev"));
        }
        
        [Test]
        public async Task CreateDocumentMultiple()
        {
            var collection = _db.GetCollection<Dictionary<string, object>>(TestDocumentCollectionName);

            var document1 = new Dictionary<string, object>()
                .String("Foo", "Foo string value")
                .Int("Bar", 12345);
            
            var document2 = new Dictionary<string, object>()
                .String("Foo", "Foo string value")
                .Int("Bar", 12345);

            var createResult = await collection
                .Insert()
                .Documents(new List<Dictionary<string, object>>() {document1, document2});
            
            Assert.AreEqual(202, createResult.StatusCode);
            Assert.IsTrue(createResult.Success);
            Assert.IsTrue(createResult.HasValue);
            Assert.AreEqual(2, createResult.Value.Count);
            Assert.IsTrue(createResult.Value.First().IsString("_id"));
            Assert.IsTrue(createResult.Value.First().IsString("_key"));
            Assert.IsTrue(createResult.Value.First().IsString("_rev"));
        }
        
        #endregion
        
        #region Check operations
        
        [Test]
        public async Task CheckDocument()
        {
            var collection = _db.GetCollection<Dictionary<string, object>>(TestDocumentCollectionName);

            var documents = await InsertTestData();
            
            var checkResult = await collection
                .Check(documents[0].ID());
            
            Assert.AreEqual(200, checkResult.StatusCode);
            Assert.IsTrue(checkResult.Success);
            Assert.IsTrue(checkResult.HasValue);
            Assert.AreEqual(checkResult.Value, documents[0].Rev());
        }
        /*
        [Test]
        public void check_document_with_ifMatch()
        {
            var checkResult = await collection
                .IfMatch(documents[0].Rev())
                .Check(documents[0].ID());
            
            Assert.AreEqual(200, checkResult.StatusCode);
            Assert.IsTrue(checkResult.Success);
            Assert.IsTrue(checkResult.HasValue);
            Assert.AreEqual(checkResult.Value, documents[0].Rev());
        }
        
        [Test]
        public void check_document_with_ifMatch_and_return_412()
        {
        	var documents = ClearCollectionAndFetchTestDocumentData(TestDocumentCollectionName);
            var db = new ADatabase(Alias);
            
            var checkResult = db.Document
                .IfMatch("123456789")
                .Check(documents[0].ID());
            
            Assert.AreEqual(412, checkResult.StatusCode);
            Assert.IsFalse(checkResult.Success);
            Assert.IsTrue(checkResult.HasValue);
            Assert.AreEqual(checkResult.Value, documents[0].Rev());
        }
        
        [Test]
        public void check_document_with_ifNoneMatch()
        {
        	var documents = ClearCollectionAndFetchTestDocumentData(TestDocumentCollectionName);
            var db = new ADatabase(Alias);
            
            var checkResult = db.Document
                .IfNoneMatch("123456789")
                .Check(documents[0].ID());
            
            Assert.AreEqual(200, checkResult.StatusCode);
            Assert.IsTrue(checkResult.Success);
            Assert.IsTrue(checkResult.HasValue);
            Assert.AreEqual(checkResult.Value, documents[0].Rev());
        }
        
        [Test]
        public void check_document_with_ifNoneMatch_and_return_304()
        {
        	var documents = ClearCollectionAndFetchTestDocumentData(TestDocumentCollectionName);
            var db = new ADatabase(Alias);
            
            var checkResult = db.Document
                .IfNoneMatch(documents[0].Rev())
                .Check(documents[0].ID());
            
            Assert.AreEqual(304, checkResult.StatusCode);
            Assert.IsFalse(checkResult.Success);
            Assert.IsTrue(checkResult.HasValue);
            Assert.AreEqual(checkResult.Value, documents[0].Rev());
        }
        */
        #endregion
        
        #region Get operations
        
        [Test]
        public async Task GetDocument()
        {
            var collection = _db.GetCollection<Dictionary<string, object>>(TestDocumentCollectionName);

            var documents = await InsertTestData();

            var getResult = await collection
                .Get(documents[0].ID());
            
            Assert.AreEqual(200, getResult.StatusCode);
            Assert.IsTrue(getResult.Success);
            Assert.IsTrue(getResult.HasValue);
            Assert.AreEqual(getResult.Value.ID(), documents[0].ID());
            Assert.AreEqual(getResult.Value.Key(), documents[0].Key());
            Assert.AreEqual(getResult.Value.Rev(), documents[0].Rev());
            Assert.AreEqual(getResult.Value.String("Foo"), documents[0].String("Foo"));
            Assert.AreEqual(getResult.Value.String("Bar"), documents[0].String("Bar"));
        }
        /*
        [Test]
        public void get_document_with_ifMatch()
        {
        	var documents = ClearCollectionAndFetchTestDocumentData(TestDocumentCollectionName);
        	var db = new ADatabase(Alias);
        	
            var getResult = db.Document
                .IfMatch(documents[0].Rev())
                .Get(documents[0].ID());
            
            Assert.AreEqual(200, getResult.StatusCode);
            Assert.IsTrue(getResult.Success);
            Assert.IsTrue(getResult.HasValue);
            Assert.AreEqual(getResult.Value.ID(), documents[0].ID());
            Assert.AreEqual(getResult.Value.Key(), documents[0].Key());
            Assert.AreEqual(getResult.Value.Rev(), documents[0].Rev());
            Assert.AreEqual(getResult.Value.String("Foo"), documents[0].String("Foo"));
            Assert.AreEqual(getResult.Value.String("Bar"), documents[0].String("Bar"));
        }
        
        [Test]
        public void get_document_with_ifMatch_and_return_412()
        {
        	var documents = ClearCollectionAndFetchTestDocumentData(TestDocumentCollectionName);
        	var db = new ADatabase(Alias);
        	
            var getResult = db.Document
                .IfMatch("123456789")
                .Get(documents[0].ID());
            
            Assert.AreEqual(412, getResult.StatusCode);
            Assert.IsFalse(getResult.Success);
            Assert.IsTrue(getResult.HasValue);
            Assert.AreEqual(getResult.Value.ID(), documents[0].ID());
            Assert.AreEqual(getResult.Value.Key(), documents[0].Key());
            Assert.AreEqual(getResult.Value.Rev(), documents[0].Rev());
        }
        
        [Test]
        public void get_document_with_ifNoneMatch()
        {
        	var documents = ClearCollectionAndFetchTestDocumentData(TestDocumentCollectionName);
        	var db = new ADatabase(Alias);
        	
            var getResult = db.Document
                .IfNoneMatch("123456789")
                .Get(documents[0].ID());
            
            Assert.AreEqual(200, getResult.StatusCode);
            Assert.IsTrue(getResult.Success);
            Assert.IsTrue(getResult.HasValue);
            Assert.AreEqual(getResult.Value.ID(), documents[0].ID());
            Assert.AreEqual(getResult.Value.Key(), documents[0].Key());
            Assert.AreEqual(getResult.Value.Rev(), documents[0].Rev());
            Assert.AreEqual(getResult.Value.String("Foo"), documents[0].String("Foo"));
            Assert.AreEqual(getResult.Value.String("Bar"), documents[0].String("Bar"));
        }
        
        [Test]
        public void get_document_with_ifNoneMatch_and_return_304()
        {
        	var documents = ClearCollectionAndFetchTestDocumentData(TestDocumentCollectionName);
        	var db = new ADatabase(Alias);
        	
            var getResult = db.Document
                .IfNoneMatch(documents[0].Rev())
                .Get(documents[0].ID());
            
            Assert.AreEqual(304, getResult.StatusCode);
            Assert.IsFalse(getResult.Success);
            Assert.IsFalse(getResult.HasValue);
        }
        */
        [Test]
        public async Task GetDocumentAsGenericObject()
        {
            var collection = _db.GetCollection<Dummy>(TestDocumentCollectionName);

            var documents = await InsertTestData();

            var getResult = await collection
                .Get(documents[0].ID());
            
            Assert.AreEqual(200, getResult.StatusCode);
            Assert.IsTrue(getResult.Success);
            Assert.IsTrue(getResult.HasValue);
            Assert.AreEqual(documents[0].String("Foo"), getResult.Value.Foo);
            Assert.AreEqual(documents[0].Int("Bar"), getResult.Value.Bar);
            Assert.AreEqual(0, getResult.Value.Baz);
        }
        
        #endregion
        
        #region Update operations
        
        [Test]
        public async Task UpdateDocument()
        {
            var collection = _db.GetCollection<Dictionary<string, object>>(TestDocumentCollectionName);

            var documents = await InsertTestData();

            var document = new Dictionary<string, object>()
                .String("Foo", "some other new string")
                .Int("Bar", 54321)
                .Int("Baz", 12345);
            
            var updateResult = await collection
                .Update()
                .Document(documents[0].ID(), document);
            
            Assert.AreEqual(202, updateResult.StatusCode);
            Assert.IsTrue(updateResult.Success);
            Assert.IsTrue(updateResult.HasValue);
            Assert.AreEqual(updateResult.Value.ID(), documents[0].ID());
            Assert.AreEqual(updateResult.Value.Key(), documents[0].Key());
            Assert.AreNotEqual(updateResult.Value.Rev(), documents[0].Rev());

            var getResult = await collection
                .Get(updateResult.Value.ID());
            
            Assert.AreEqual(getResult.Value.ID(), updateResult.Value.ID());
            Assert.AreEqual(getResult.Value.Key(), updateResult.Value.Key());
            Assert.AreEqual(getResult.Value.Rev(), updateResult.Value.Rev());
            
            Assert.AreNotEqual(getResult.Value.String("Foo"), documents[0].String("Foo"));
            Assert.AreEqual(getResult.Value.String("Foo"), document.String("Foo"));
            
            Assert.AreNotEqual(getResult.Value.Int("Bar"), documents[0].Int("Bar"));
            Assert.AreEqual(getResult.Value.Int("Bar"), document.Int("Bar"));
            Assert.AreEqual(getResult.Value.Int("Baz"), document.Int("Baz"));
        }

        [Test]
        public async Task UpdateDocumentWithReturnOld()
        {
            var collection = _db.GetCollection<Dictionary<string, object>>(TestDocumentCollectionName);

            var documents = await InsertTestData();

            var document = new Dictionary<string, object>()
                .String("Foo", "some other new string")
                .Int("Bar", 54321)
                .Int("Baz", 12345);

            var updateResult = await collection
                .Update()
                .ReturnOld()
                .Document(documents[0].ID(), document);

            Assert.AreEqual(202, updateResult.StatusCode);
            Assert.IsTrue(updateResult.Success);
            Assert.IsTrue(updateResult.HasValue);
            Assert.AreEqual(updateResult.Value.ID(), documents[0].ID());
            Assert.AreEqual(updateResult.Value.Key(), documents[0].Key());
            Assert.AreEqual(updateResult.Value.Rev(), documents[0].Rev());
        }

        [Test]
        public async Task UpdateDocumentWithReturnNew()
        {
            var collection = _db.GetCollection<Dictionary<string, object>>(TestDocumentCollectionName);

            var documents = await InsertTestData();

            var document = new Dictionary<string, object>()
                .String("Foo", "some other new string")
                .Int("Bar", 54321)
                .Int("Baz", 12345);

            var updateResult = await collection
                .Update()
                .ReturnNew()
                .Document(documents[0].ID(), document);

            Assert.AreEqual(202, updateResult.StatusCode);
            Assert.IsTrue(updateResult.Success);
            Assert.IsTrue(updateResult.HasValue);
            Assert.AreEqual(updateResult.Value.ID(), documents[0].ID());
            Assert.AreEqual(updateResult.Value.Key(), documents[0].Key());
            Assert.AreNotEqual(updateResult.Value.Rev(), documents[0].Rev());
        }

        [Test]
        public async Task UpdateDocumentWithWaitForSync()
        {
            var collection = _db.GetCollection<Dictionary<string, object>>(TestDocumentCollectionName);

            var documents = await InsertTestData();

            var document = new Dictionary<string, object>()
                .String("Foo", "some other new string")
                .Int("Bar", 54321)
                .Int("Baz", 12345);
         
            var updateResult = await collection
                .Update()
                .WaitForSync(true)
                .Document(documents[0].ID(), document);
            
            Assert.AreEqual(201, updateResult.StatusCode);
            Assert.IsTrue(updateResult.Success);
            Assert.IsTrue(updateResult.HasValue);
            Assert.AreEqual(updateResult.Value.ID(), documents[0].ID());
            Assert.AreEqual(updateResult.Value.Key(), documents[0].Key());
            Assert.AreNotEqual(updateResult.Value.Rev(), documents[0].Rev());
            
            var getResult = await collection
                .Get(updateResult.Value.ID());
            
            Assert.AreEqual(getResult.Value.ID(), updateResult.Value.ID());
            Assert.AreEqual(getResult.Value.Key(), updateResult.Value.Key());
            Assert.AreEqual(getResult.Value.Rev(), updateResult.Value.Rev());
            
            Assert.AreNotEqual(getResult.Value.String("Foo"), documents[0].String("Foo"));
            Assert.AreEqual(getResult.Value.String("Foo"), document.String("Foo"));
            
            Assert.AreNotEqual(getResult.Value.Int("Bar"), documents[0].Int("Bar"));
            Assert.AreEqual(getResult.Value.Int("Bar"), document.Int("Bar"));
            Assert.AreEqual(getResult.Value.Int("Baz"), document.Int("Baz"));
        }

        [Test]
        public async Task UpdateDocumentWithoutIgnoreRevs()
        {
            var collection = _db.GetCollection<Dictionary<string, object>>(TestDocumentCollectionName);

            var documents = await InsertTestData();

            var document = new Dictionary<string, object>()
                .ID(documents[0].ID())
                .Key(documents[0].Key())
                .Rev(documents[0].Rev())
                .String("Foo", "some other new string")
                .Int("Bar", 54321)
                .Int("Baz", 12345);

            var updateResult = await collection
                .Update()
                .IgnoreRevs(false)
                .Document(documents[0].ID(), document);

            Assert.AreEqual(202, updateResult.StatusCode);
            Assert.IsTrue(updateResult.Success);
            Assert.IsTrue(updateResult.HasValue);
            Assert.AreEqual(updateResult.Value.ID(), documents[0].ID());
            Assert.AreEqual(updateResult.Value.Key(), documents[0].Key());
            Assert.AreNotEqual(updateResult.Value.Rev(), documents[0].Rev());
        }

        [Test]
        public async Task UpdateDocumentWithIfMatch()
        {
            var collection = _db.GetCollection<Dictionary<string, object>>(TestDocumentCollectionName);

            var documents = await InsertTestData();

            var document = new Dictionary<string, object>()
                .String("Foo", "some other new string")
                .Int("Bar", 54321)
                .Int("Baz", 12345);
         
            var updateResult = await collection
                .Update()
                .IfMatch(documents[0].Rev())
                .Document(documents[0].ID(), document);
            
            Assert.AreEqual(202, updateResult.StatusCode);
            Assert.IsTrue(updateResult.Success);
            Assert.IsTrue(updateResult.HasValue);
            Assert.AreEqual(updateResult.Value.ID(), documents[0].ID());
            Assert.AreEqual(updateResult.Value.Key(), documents[0].Key());
            Assert.AreNotEqual(updateResult.Value.Rev(), documents[0].Rev());
            
            var getResult = await collection
                .Get(updateResult.Value.ID());
            
            Assert.AreEqual(getResult.Value.ID(), updateResult.Value.ID());
            Assert.AreEqual(getResult.Value.Key(), updateResult.Value.Key());
            Assert.AreEqual(getResult.Value.Rev(), updateResult.Value.Rev());
            
            Assert.AreNotEqual(getResult.Value.String("Foo"), documents[0].String("Foo"));
            Assert.AreEqual(getResult.Value.String("Foo"), document.String("Foo"));
            
            Assert.AreNotEqual(getResult.Value.Int("Bar"), documents[0].Int("Bar"));
            Assert.AreEqual(getResult.Value.Int("Bar"), document.Int("Bar"));
            Assert.AreEqual(getResult.Value.Int("Baz"), document.Int("Baz"));
        }
        
        [Test]
        public async Task UpdateDocumentWithIfMatchAndReturn412()
        {
            var collection = _db.GetCollection<Dictionary<string, object>>(TestDocumentCollectionName);

            var documents = await InsertTestData();

            var document = new Dictionary<string, object>()
                .String("Foo", "some other new string")
                .Int("Bar", 54321)
                .Int("Baz", 12345);
         
            var updateResult = await collection
                .Update()
                .IfMatch("123456789")
                .Document(documents[0].ID(), document);
            
            Assert.AreEqual(412, updateResult.StatusCode);
            Assert.IsFalse(updateResult.Success);
            Assert.IsTrue(updateResult.HasValue);
            Assert.AreEqual(updateResult.Value.ID(), documents[0].ID());
            Assert.AreEqual(updateResult.Value.Key(), documents[0].Key());
            Assert.AreEqual(updateResult.Value.Rev(), documents[0].Rev());
        }
        
        [Test]
        public async Task UpdateDocumentWithKeepNull()
        {
            var collection = _db.GetCollection<Dictionary<string, object>>(TestDocumentCollectionName);

            var newDocument = new Dictionary<string, object>()
                .String("Foo", "some string")
                .Object("Bar", null);
            
            var createResult = await collection
                .Insert()
                .Document(newDocument);
            
            newDocument.Merge(createResult.Value);
            
            var document = new Dictionary<string, object>()
                .String("Foo", "some other new string")
                .Object("Baz", null);
            
            var updateResult = await collection
                .Update()
                .KeepNull(false)
                .Document(newDocument.ID(), document);
            
            Assert.AreEqual(202, updateResult.StatusCode);
            Assert.IsTrue(updateResult.Success);
            Assert.IsTrue(updateResult.HasValue);
            Assert.AreEqual(updateResult.Value.ID(), newDocument.ID());
            Assert.AreEqual(updateResult.Value.Key(), newDocument.Key());
            Assert.AreNotEqual(updateResult.Value.Rev(), newDocument.Rev());
            
            var getResult = await collection
                .Get(updateResult.Value.ID());
            
            Assert.AreEqual(getResult.Value.ID(), updateResult.Value.ID());
            Assert.AreEqual(getResult.Value.Key(), updateResult.Value.Key());
            Assert.AreEqual(getResult.Value.Rev(), updateResult.Value.Rev());
            
            Assert.AreNotEqual(getResult.Value.String("Foo"), newDocument.String("Foo"));
            Assert.AreEqual(getResult.Value.String("Foo"), document.String("Foo"));
            
            Assert.IsTrue(getResult.Value.Has("Bar"));
            
            Assert.IsFalse(getResult.Value.Has("Baz"));
        }
        
        [Test]
        public async Task UpdateDocumentWithMergeArrays()
        {
            var collection = _db.GetCollection<Dictionary<string, object>>(TestDocumentCollectionName);

            var newDocument = new Dictionary<string, object>()
                .String("Foo", "some string")
                .Document("Bar", new Dictionary<string, object>().String("Foo", "string value"));
            
            var createResult = await collection
                .Insert()
                .Document(newDocument);
            
            newDocument.Merge(createResult.Value);
            
            var document = new Dictionary<string, object>()
                .String("Foo", "some other new string")
                .Document("Bar", new Dictionary<string, object>().String("Bar", "other string value"));
            
            var updateResult = await collection
                .Update()
                .MergeObjects(true) // this is also default behavior
                .Document(newDocument.ID(), document);
            
            Assert.AreEqual(202, updateResult.StatusCode);
            Assert.IsTrue(updateResult.Success);
            Assert.IsTrue(updateResult.HasValue);
            Assert.AreEqual(updateResult.Value.ID(), newDocument.ID());
            Assert.AreEqual(updateResult.Value.Key(), newDocument.Key());
            Assert.AreNotEqual(updateResult.Value.Rev(), newDocument.Rev());
            
            var getResult = await collection
                .Get(updateResult.Value.ID());
            
            Assert.AreEqual(getResult.Value.ID(), updateResult.Value.ID());
            Assert.AreEqual(getResult.Value.Key(), updateResult.Value.Key());
            Assert.AreEqual(getResult.Value.Rev(), updateResult.Value.Rev());
            
            Assert.AreNotEqual(getResult.Value.String("Foo"), newDocument.String("Foo"));
            Assert.AreEqual(getResult.Value.String("Foo"), document.String("Foo"));
            
            Assert.IsTrue(getResult.Value.Has("Bar.Foo"));
            
            Assert.IsTrue(getResult.Value.Has("Bar.Bar"));
        }
        
        [Test]
        public async Task UpdateDocumentWithoutMergeArrays()
        {
            var collection = _db.GetCollection<Dictionary<string, object>>(TestDocumentCollectionName);

            var newDocument = new Dictionary<string, object>()
                .String("Foo", "some string")
                .Document("Bar", new Dictionary<string, object>().String("Foo", "string value"));
            
            var createResult = await collection
                .Insert()
                .Document(newDocument);
            
            newDocument.Merge(createResult.Value);
            
            var document = new Dictionary<string, object>()
                .String("Foo", "some other new string")
                .Document("Bar", new Dictionary<string, object>().String("Bar", "other string value"));
            
            var updateResult = await collection
                .Update()
                .MergeObjects(false)
                .Document(newDocument.ID(), document);
            
            Assert.AreEqual(202, updateResult.StatusCode);
            Assert.IsTrue(updateResult.Success);
            Assert.IsTrue(updateResult.HasValue);
            Assert.AreEqual(updateResult.Value.ID(), newDocument.ID());
            Assert.AreEqual(updateResult.Value.Key(), newDocument.Key());
            Assert.AreNotEqual(updateResult.Value.Rev(), newDocument.Rev());
            
            var getResult = await collection
                .Get(updateResult.Value.ID());
            
            Assert.AreEqual(getResult.Value.ID(), updateResult.Value.ID());
            Assert.AreEqual(getResult.Value.Key(), updateResult.Value.Key());
            Assert.AreEqual(getResult.Value.Rev(), updateResult.Value.Rev());
            
            Assert.AreNotEqual(getResult.Value.String("Foo"), newDocument.String("Foo"));
            Assert.AreEqual(getResult.Value.String("Foo"), document.String("Foo"));
            
            Assert.IsFalse(getResult.Value.Has("Bar.Foo"));
            
            Assert.IsTrue(getResult.Value.Has("Bar.Bar"));
        }
        
        [Test]
        public async Task UpdateDocumentWithGenericObject()
        {
            var collection = _db.GetCollection<Dummy>(TestDocumentCollectionName);

            var documents = await InsertTestData();

            var dummy = new Dummy();
            dummy.Foo = "some other new string";
            dummy.Bar = 54321;
            dummy.Baz = 12345;
         
            var updateResult = await collection
                .Update()
                .Document(documents[0].ID(), dummy);
            
            Assert.AreEqual(202, updateResult.StatusCode);
            Assert.IsTrue(updateResult.Success);
            Assert.IsTrue(updateResult.HasValue);
            Assert.AreEqual(updateResult.Value.Id, documents[0].ID());
            Assert.AreEqual(updateResult.Value.Key, documents[0].Key());
            Assert.AreNotEqual(updateResult.Value.Revision, documents[0].Rev());
            
            var getResult = await collection
                .Get(updateResult.Value.Id);
            
            Assert.AreEqual(getResult.Value.Id, updateResult.Value.Id);
            Assert.AreEqual(getResult.Value.Key, updateResult.Value.Key);
            Assert.AreEqual(getResult.Value.Revision, updateResult.Value.Revision);
            
            Assert.AreNotEqual(getResult.Value.Foo, documents[0].String("Foo"));
            Assert.AreEqual(getResult.Value.Foo, dummy.Foo);
            Assert.AreNotEqual(getResult.Value.Bar, documents[0].Int("Bar"));
            Assert.AreEqual(getResult.Value.Bar, dummy.Bar);
            Assert.AreEqual(getResult.Value.Baz, dummy.Baz);
        }
        
        #endregion
        
        #region Replace operations
        
        [Test]
        public async Task ReplaceDocument()
        {
            var collection = _db.GetCollection<Dictionary<string, object>>(TestDocumentCollectionName);

            var documents = await InsertTestData();

            var document = new Dictionary<string, object>()
                .String("Foo", "some other new string")
                .Int("Baz", 54321);
         
            var replaceResult = await collection
                .Replace()
                .Document(documents[0].ID(), document);
            
            Assert.AreEqual(202, replaceResult.StatusCode);
            Assert.IsTrue(replaceResult.Success);
            Assert.IsTrue(replaceResult.HasValue);
            Assert.AreEqual(replaceResult.Value.ID(), documents[0].ID());
            Assert.AreEqual(replaceResult.Value.Key(), documents[0].Key());
            Assert.AreNotEqual(replaceResult.Value.Rev(), documents[0].Rev());
            
            var getResult = await collection
                .Get(replaceResult.Value.ID());
            
            Assert.AreEqual(getResult.Value.ID(), replaceResult.Value.ID());
            Assert.AreEqual(getResult.Value.Key(), replaceResult.Value.Key());
            Assert.AreEqual(getResult.Value.Rev(), replaceResult.Value.Rev());
            
            Assert.AreNotEqual(getResult.Value.String("Foo"), documents[0].String("Foo"));
            Assert.AreEqual(getResult.Value.String("Foo"), document.String("Foo"));
            
            Assert.AreEqual(getResult.Value.Int("Baz"), document.Int("Baz"));
            
            Assert.IsFalse(getResult.Value.Has("Bar"));
        }

        [Test]
        public async Task ReplaceDocumentWithReturnOld()
        {
            var collection = _db.GetCollection<Dictionary<string, object>>(TestDocumentCollectionName);

            var documents = await InsertTestData();

            var document = new Dictionary<string, object>()
                .String("Foo", "some other new string")
                .Int("Baz", 54321);

            var replaceResult = await collection
                .Replace()
                .ReturnOld()
                .Document(documents[0].ID(), document);

            Assert.AreEqual(202, replaceResult.StatusCode);
            Assert.IsTrue(replaceResult.Success);
            Assert.IsTrue(replaceResult.HasValue);
            Assert.AreEqual(replaceResult.Value.ID(), documents[0].ID());
            Assert.AreEqual(replaceResult.Value.Key(), documents[0].Key());
            Assert.AreEqual(replaceResult.Value.Rev(), documents[0].Rev());
        }

        [Test]
        public async Task ReplaceDocumentWithReturnNew()
        {
            var collection = _db.GetCollection<Dictionary<string, object>>(TestDocumentCollectionName);

            var documents = await InsertTestData();

            var document = new Dictionary<string, object>()
                .String("Foo", "some other new string")
                .Int("Baz", 54321);

            var replaceResult = await collection
                .Replace()
                .ReturnNew()
                .Document(documents[0].ID(), document);

            Assert.AreEqual(202, replaceResult.StatusCode);
            Assert.IsTrue(replaceResult.Success);
            Assert.IsTrue(replaceResult.HasValue);
            Assert.AreEqual(replaceResult.Value.ID(), documents[0].ID());
            Assert.AreEqual(replaceResult.Value.Key(), documents[0].Key());
            Assert.AreNotEqual(replaceResult.Value.Rev(), documents[0].Rev());
        }

        [Test]
        public async Task ReplaceDocumentWithWaitForSync()
        {
            var collection = _db.GetCollection<Dictionary<string, object>>(TestDocumentCollectionName);

            var documents = await InsertTestData();

            var document = new Dictionary<string, object>()
                .String("Foo", "some other new string")
                .Int("Baz", 54321);
            
            var replaceResult = await collection
                .Replace()
                .WaitForSync(true)
                .Document(documents[0].ID(), document);
            
            Assert.AreEqual(201, replaceResult.StatusCode);
            Assert.IsTrue(replaceResult.Success);
            Assert.IsTrue(replaceResult.HasValue);
            Assert.AreEqual(replaceResult.Value.ID(), documents[0].ID());
            Assert.AreEqual(replaceResult.Value.Key(), documents[0].Key());
            Assert.AreNotEqual(replaceResult.Value.Rev(), documents[0].Rev());
            
            var getResult = await collection
                .Get(replaceResult.Value.ID());
            
            Assert.AreEqual(getResult.Value.ID(), replaceResult.Value.ID());
            Assert.AreEqual(getResult.Value.Key(), replaceResult.Value.Key());
            Assert.AreEqual(getResult.Value.Rev(), replaceResult.Value.Rev());
            
            Assert.AreNotEqual(getResult.Value.String("Foo"), documents[0].String("Foo"));
            Assert.AreEqual(getResult.Value.String("Foo"), document.String("Foo"));
            
            Assert.AreEqual(getResult.Value.Int("Baz"), document.Int("Baz"));
            
            Assert.IsFalse(getResult.Value.Has("Bar"));
        }

        [Test]
        public async Task ReplaceDocumentWithoutIgnoreRevs()
        {
            var collection = _db.GetCollection<Dictionary<string, object>>(TestDocumentCollectionName);

            var documents = await InsertTestData();

            var document = new Dictionary<string, object>()
                .ID(documents[0].ID())
                .Key(documents[0].Key())
                .Rev(documents[0].Rev())
                .String("Foo", "some other new string")
                .Int("Baz", 54321);

            var replaceResult = await collection
                .Replace()
                .IgnoreRevs(false)
                .Document(documents[0].ID(), document);

            Assert.AreEqual(202, replaceResult.StatusCode);
            Assert.IsTrue(replaceResult.Success);
            Assert.IsTrue(replaceResult.HasValue);
            Assert.AreEqual(replaceResult.Value.ID(), documents[0].ID());
            Assert.AreEqual(replaceResult.Value.Key(), documents[0].Key());
            Assert.AreNotEqual(replaceResult.Value.Rev(), documents[0].Rev());
        }

        [Test]
        public async Task ReplaceDocumentWithIfMatch()
        {
            var collection = _db.GetCollection<Dictionary<string, object>>(TestDocumentCollectionName);

            var documents = await InsertTestData();

            var document = new Dictionary<string, object>()
                .String("Foo", "some other new string")
                .Int("Baz", 54321);
         
            var replaceResult = await collection
                .Replace()
                .IfMatch(documents[0].Rev())
                .Document(documents[0].ID(), document);
            
            Assert.AreEqual(202, replaceResult.StatusCode);
            Assert.IsTrue(replaceResult.Success);
            Assert.IsTrue(replaceResult.HasValue);
            Assert.AreEqual(replaceResult.Value.ID(), documents[0].ID());
            Assert.AreEqual(replaceResult.Value.Key(), documents[0].Key());
            Assert.AreNotEqual(replaceResult.Value.Rev(), documents[0].Rev());
            
            var getResult = await collection
                .Get(replaceResult.Value.ID());
            
            Assert.AreEqual(getResult.Value.ID(), replaceResult.Value.ID());
            Assert.AreEqual(getResult.Value.Key(), replaceResult.Value.Key());
            Assert.AreEqual(getResult.Value.Rev(), replaceResult.Value.Rev());
            
            Assert.AreNotEqual(getResult.Value.String("Foo"), documents[0].String("Foo"));
            Assert.AreEqual(getResult.Value.String("Foo"), document.String("Foo"));
            
            Assert.AreEqual(getResult.Value.Int("Baz"), document.Int("Baz"));
            
            Assert.IsFalse(getResult.Value.Has("Bar"));
        }
        
        [Test]
        public async Task ReplaceDocumentWithIfMatchAndReturn412()
        {
            var collection = _db.GetCollection<Dictionary<string, object>>(TestDocumentCollectionName);

            var documents = await InsertTestData();

            var document = new Dictionary<string, object>()
                .String("Foo", "some other new string")
                .Int("Baz", 54321);
            
            var replaceResult = await collection
                .Replace()
                .IfMatch("123456789")
                .Document(documents[0].ID(), document);
            
            Assert.AreEqual(412, replaceResult.StatusCode);
            Assert.IsFalse(replaceResult.Success);
            Assert.IsTrue(replaceResult.HasValue);
            Assert.AreEqual(replaceResult.Value.ID(), documents[0].ID());
            Assert.AreEqual(replaceResult.Value.Key(), documents[0].Key());
            Assert.AreEqual(replaceResult.Value.Rev(), documents[0].Rev());
        }
        
        [Test]
        public async Task ReplaceDocumentWithGenericObject()
        {
            var collection = _db.GetCollection<Dummy>(TestDocumentCollectionName);

            var documents = await InsertTestData();

            var dummy = new Dummy();
            dummy.Foo = "some other new string";
            dummy.Baz = 54321;
         
            var replaceResult = await collection
                .Replace()
                .Document(documents[0].ID(), dummy);
            
            Assert.AreEqual(202, replaceResult.StatusCode);
            Assert.IsTrue(replaceResult.Success);
            Assert.IsTrue(replaceResult.HasValue);
            Assert.AreEqual(replaceResult.Value.Id, documents[0].ID());
            Assert.AreEqual(replaceResult.Value.Key, documents[0].Key());
            Assert.AreNotEqual(replaceResult.Value.Revision, documents[0].Rev());
            
            var getResult = await collection
                .Get(replaceResult.Value.Id);
            
            Assert.AreEqual(getResult.Value.Id, replaceResult.Value.Id);
            Assert.AreEqual(getResult.Value.Key, replaceResult.Value.Key);
            Assert.AreEqual(getResult.Value.Revision, replaceResult.Value.Revision);
            
            Assert.AreNotEqual(getResult.Value.Foo, documents[0].String("Foo"));
            Assert.AreEqual(getResult.Value.Foo, dummy.Foo);
            Assert.AreEqual(getResult.Value.Bar, dummy.Bar);
            Assert.AreEqual(getResult.Value.Baz, dummy.Baz);
        }
        
        #endregion
        
        #region Delete operations
        
        [Test]
        public async Task DeleteDocument()
        {   
            var collection = _db.GetCollection<Dictionary<string, object>>(TestDocumentCollectionName);

            var documents = await InsertTestData();

            var deleteResult = await collection
                .Delete()
                .ById(documents[0].ID());
            
            Assert.AreEqual(202, deleteResult.StatusCode);
            Assert.IsTrue(deleteResult.Success);
            Assert.IsTrue(deleteResult.HasValue);
            Assert.AreEqual(deleteResult.Value.ID(), documents[0].ID());
            Assert.AreEqual(deleteResult.Value.Key(), documents[0].Key());
            Assert.AreEqual(deleteResult.Value.Rev(), documents[0].Rev());
        }
        
        [Test]
        public async Task DeleteDocumentByKey()
        {   
            var collection = _db.GetCollection<Dictionary<string, object>>(TestDocumentCollectionName);

            var documents = await InsertTestData();

            var deleteResult = await collection
                .Delete()
                .ByKey(documents[0].Key());
            
            Assert.AreEqual(202, deleteResult.StatusCode);
            Assert.IsTrue(deleteResult.Success);
            Assert.IsTrue(deleteResult.HasValue);
            Assert.AreEqual(deleteResult.Value.ID(), documents[0].ID());
            Assert.AreEqual(deleteResult.Value.Key(), documents[0].Key());
            Assert.AreEqual(deleteResult.Value.Rev(), documents[0].Rev());
        }
        
        [Test]
        public async Task DeleteDocumentWithWaitForSync()
        {
            var collection = _db.GetCollection<Dictionary<string, object>>(TestDocumentCollectionName);

            var documents = await InsertTestData();

            var deleteResult = await collection
                .Delete()
                .WaitForSync(true)
                .ById(documents[0].ID());
            
            Assert.AreEqual(200, deleteResult.StatusCode);
            Assert.IsTrue(deleteResult.Success);
            Assert.IsTrue(deleteResult.HasValue);
            Assert.AreEqual(deleteResult.Value.ID(), documents[0].ID());
            Assert.AreEqual(deleteResult.Value.Key(), documents[0].Key());
            Assert.AreEqual(deleteResult.Value.Rev(), documents[0].Rev());
        }
        
        [Test]
        public async Task DeleteDocumentWithIfMatch()
        {
            var collection = _db.GetCollection<Dictionary<string, object>>(TestDocumentCollectionName);

            var documents = await InsertTestData();

            var deleteResult = await collection
                .Delete()
                .IfMatch(documents[0].Rev())
                .ById(documents[0].ID());
            
            Assert.AreEqual(202, deleteResult.StatusCode);
            Assert.IsTrue(deleteResult.Success);
            Assert.IsTrue(deleteResult.HasValue);
            Assert.AreEqual(deleteResult.Value.ID(), documents[0].ID());
            Assert.AreEqual(deleteResult.Value.Key(), documents[0].Key());
            Assert.AreEqual(deleteResult.Value.Rev(), documents[0].Rev());
        }
        
        [Test]
        public async Task DeleteDocumentWithIfMatchAndReturn412()
        {
            var collection = _db.GetCollection<Dictionary<string, object>>(TestDocumentCollectionName);

            var documents = await InsertTestData();

            var deleteResult = await collection
                .Delete()
                .IfMatch("123456789")
                .ById(documents[0].ID());
            
            Assert.AreEqual(412, deleteResult.StatusCode);
            Assert.IsFalse(deleteResult.Success);
            Assert.AreEqual(deleteResult.Value.ID(), documents[0].ID());
            Assert.AreEqual(deleteResult.Value.Key(), documents[0].Key());
            Assert.AreEqual(deleteResult.Value.Rev(), documents[0].Rev());
        }

        [Test]
        public async Task DeleteDocumentWithReturnOld()
        {
            var collection = _db.GetCollection<Dictionary<string, object>>(TestDocumentCollectionName);

            var documents = await InsertTestData();

            var deleteResult = await collection
                .Delete()
                .ReturnOld()
                .ById(documents[0].ID());

            Assert.AreEqual(202, deleteResult.StatusCode);
            Assert.IsTrue(deleteResult.Success);
            Assert.AreEqual(deleteResult.Value.ID(), documents[0].ID());
            Assert.AreEqual(deleteResult.Value.Key(), documents[0].Key());
            Assert.AreEqual(deleteResult.Value.Rev(), documents[0].Rev());
            Assert.IsTrue(deleteResult.Value.Has("old"));
        }

        #endregion
        
        #region Special cases
        
        #endregion
    }
}
