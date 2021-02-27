using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Arango.Tests;
using ArangoDriver.Client;
using ArangoDriver.Exceptions;
using Newtonsoft.Json.Linq;
using NUnit.Framework;

namespace Tests.Operations
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
        
        #region Create operations
        
        [Test]
        public async Task CreateDocument()
        {
            var collection = _db.GetCollection<Dictionary<string, object>>(TestDocumentCollectionName);
            
            var document = new Dictionary<string, object>();
        	document.Add("Foo", "Foo string value");
            document.Add("Bar", 12345);

            var createResult = await collection
                .Insert()
                .ReturnNew()
                .Document(document);
            
            Assert.AreEqual(202, createResult.StatusCode);
            Assert.IsTrue(createResult.Success);
            Assert.IsTrue(createResult.HasValue);
            Assert.IsNotEmpty((string) createResult.Value["_id"]);
            Assert.IsNotEmpty((string) createResult.Value["_key"]);
            Assert.IsNotEmpty((string) createResult.Value["_rev"]);
        }

        [Test]
        public async Task CreateDocumentWithReturnNew()
        {
            var collection = _db.GetCollection<Dictionary<string, object>>(TestDocumentCollectionName);

            var document = new Dictionary<string, object>();
            document.Add("Foo", "Foo string value");
            document.Add("Bar", 12345);

            var createResult = await collection
                .Insert()
                .ReturnNew()
                .Document(document);

            Assert.AreEqual(202, createResult.StatusCode);
            Assert.IsTrue(createResult.Success);
            Assert.IsTrue(createResult.HasValue);
            Assert.IsNotEmpty((string) createResult.Value["_id"]);
            Assert.IsNotEmpty((string) createResult.Value["_key"]);
            Assert.IsNotEmpty((string) createResult.Value["_rev"]);
            Assert.AreEqual(document["Foo"], createResult.Value["Foo"]);
            Assert.AreEqual(document["Bar"], createResult.Value["Bar"]);
        }

        [Test]
        public async Task CreateDocumentWithWaitForSync()
        {
            var collection = _db.GetCollection<Dictionary<string, object>>(TestDocumentCollectionName);

            var document = new Dictionary<string, object>();
            document.Add("Foo", "Foo string value");
            document.Add("Bar", 12345);

            var createResult = await collection
                .Insert()
                .ReturnNew()
                .WaitForSync(true)
                .Document(document);
            
            Assert.AreEqual(201, createResult.StatusCode);
            Assert.IsTrue(createResult.Success);
            Assert.IsTrue(createResult.HasValue);
            Assert.IsNotEmpty((string) createResult.Value["_id"]);
            Assert.IsNotEmpty((string) createResult.Value["_key"]);
            Assert.IsNotEmpty((string) createResult.Value["_rev"]);
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
                .ReturnNew()
                .Document(dummy);
            
            Assert.AreEqual(202, createResult.StatusCode);
            Assert.IsTrue(createResult.Success);
            Assert.IsTrue(createResult.HasValue);
            Assert.IsNotNull(createResult.Value.Id);
            Assert.IsNotNull(createResult.Value.Key);
            Assert.IsNotNull(createResult.Value.Revision);
            
            var getResult = await collection
                .Get()
                .ById(createResult.Value.Id);
            
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
                .Get()
                .ById(createResult.Value.Id);
            
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

            var document = new Dictionary<string, object>();
            document.Add("_key", "1234-5678");
            document.Add("Foo", "Foo string value");
            document.Add("Bar", 12345);

            var createResult = await collection
                .Insert()
                .ReturnNew()
                .Document(document);
            
            Assert.AreEqual(202, createResult.StatusCode);
            Assert.IsTrue(createResult.Success);
            Assert.IsTrue(createResult.HasValue);
            Assert.AreEqual(TestDocumentCollectionName + "/" + document.Key(), createResult.Value.ID());
            Assert.AreEqual(document.Key(), createResult.Value.Key());
            Assert.IsNotEmpty((string) createResult.Value["_rev"]);
        }
        
        [Test]
        public async Task CreateDocumentMultiple()
        {
            var collection = _db.GetCollection<Dictionary<string, object>>(TestDocumentCollectionName);

            var document1 = new Dictionary<string, object>();
            document1.Add("Foo", "Foo string value");
            document1.Add("Bar", 12345);
            
            var document2 = new Dictionary<string, object>();
            document2.Add("Foo", "Foo string value");
            document2.Add("Bar", 12345);

            var createResult = await collection
                .Insert()
                .ReturnNew()
                .Documents(new List<Dictionary<string, object>>() {document1, document2});
            
            Assert.AreEqual(202, createResult.StatusCode);
            Assert.IsTrue(createResult.Success);
            Assert.IsTrue(createResult.HasValue);
            Assert.AreEqual(2, createResult.Value.Count);
            Assert.IsNotEmpty((string) createResult.Value.First()["_id"]);
            Assert.IsNotEmpty((string) createResult.Value.First()["_key"]);
            Assert.IsNotEmpty((string) createResult.Value.First()["_rev"]);
        }
        
        #endregion
        
        #region Check operations
        
        [Test]
        public async Task CheckDocument()
        {
            var collection = _db.GetCollection<Dictionary<string, object>>(TestDocumentCollectionName);

            var documents = await InsertTestData(_db);
            
            var checkResult = await collection
                .Check()
                .ById(documents[0].ID());
            
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

            var documents = await InsertTestData(_db);

            var getResult = await collection
                .Get()
                .ById(documents[0].ID());
            
            Assert.AreEqual(200, getResult.StatusCode);
            Assert.IsTrue(getResult.Success);
            Assert.IsTrue(getResult.HasValue);
            Assert.AreEqual(getResult.Value.ID(), documents[0].ID());
            Assert.AreEqual(getResult.Value.Key(), documents[0].Key());
            Assert.AreEqual(getResult.Value.Rev(), documents[0].Rev());
            Assert.AreEqual(getResult.Value["Foo"], documents[0]["Foo"]);
            Assert.AreEqual(getResult.Value["Bar"], documents[0]["Bar"]);
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
            Assert.AreEqual(getResult.Value["Foo"), documents[0]["Foo"));
            Assert.AreEqual(getResult.Value["Bar"), documents[0]["Bar"));
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
            Assert.AreEqual(getResult.Value["Foo"), documents[0]["Foo"));
            Assert.AreEqual(getResult.Value["Bar"), documents[0]["Bar"));
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

            var documents = await InsertTestData(_db);

            var getResult = await collection
                .Get()
                .ById(documents[0].ID());
            
            Assert.AreEqual(200, getResult.StatusCode);
            Assert.IsTrue(getResult.Success);
            Assert.IsTrue(getResult.HasValue);
            Assert.AreEqual(documents[0]["Foo"], getResult.Value.Foo);
            Assert.AreEqual(documents[0]["Bar"], getResult.Value.Bar);
            Assert.AreEqual(0, getResult.Value.Baz);
        }
        
        #endregion
        
        #region Update operations
        
        [Test]
        public async Task UpdateDocument()
        {
            var collection = _db.GetCollection<Dictionary<string, object>>(TestDocumentCollectionName);

            var documents = await InsertTestData(_db);

            var document = new Dictionary<string, object>();
            document.Add("Foo", "some other new string");
            document.Add("Bar", 54321);
            document.Add("Baz", 12345);
            
            var updateResult = await collection
                .Update()
                .DocumentById(documents[0].ID(), document);
            
            Assert.AreEqual(202, updateResult.StatusCode);
            Assert.IsTrue(updateResult.Success);
            Assert.IsTrue(updateResult.HasValue);
            Assert.AreEqual(updateResult.Value.ID(), documents[0].ID());
            Assert.AreEqual(updateResult.Value.Key(), documents[0].Key());
            Assert.AreNotEqual(updateResult.Value.Rev(), documents[0].Rev());

            var getResult = await collection
                .Get()
                .ById(updateResult.Value.ID());
            
            Assert.AreEqual(getResult.Value.ID(), updateResult.Value.ID());
            Assert.AreEqual(getResult.Value.Key(), updateResult.Value.Key());
            Assert.AreEqual(getResult.Value.Rev(), updateResult.Value.Rev());
            
            Assert.AreNotEqual(getResult.Value["Foo"], documents[0]["Foo"]);
            Assert.AreEqual(getResult.Value["Foo"], document["Foo"]);
            
            Assert.AreNotEqual(getResult.Value["Bar"], documents[0]["Bar"]);
            Assert.AreEqual(getResult.Value["Bar"], document["Bar"]);
            Assert.AreEqual(getResult.Value["Baz"], document["Baz"]);
        }

        [Test]
        public async Task UpdateDocumentWithReturnOld()
        {
            var collection = _db.GetCollection<Dictionary<string, object>>(TestDocumentCollectionName);

            var documents = await InsertTestData(_db);

            var document = new Dictionary<string, object>();
            document.Add("Foo", "some other new string");
            document.Add("Bar", 54321);
            document.Add("Baz", 12345);

            var updateResult = await collection
                .Update()
                .ReturnOld()
                .DocumentById(documents[0].ID(), document);

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

            var documents = await InsertTestData(_db);

            var document = new Dictionary<string, object>();
            document.Add("Foo", "some other new string");
            document.Add("Bar", 54321);
            document.Add("Baz", 12345);

            var updateResult = await collection
                .Update()
                .ReturnNew()
                .DocumentById(documents[0].ID(), document);

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

            var documents = await InsertTestData(_db);

            var document = new Dictionary<string, object>();
            document.Add("Foo", "some other new string");
            document.Add("Bar", 54321);
            document.Add("Baz", 12345);
         
            var updateResult = await collection
                .Update()
                .WaitForSync(true)
                .DocumentById(documents[0].ID(), document);
            
            Assert.AreEqual(201, updateResult.StatusCode);
            Assert.IsTrue(updateResult.Success);
            Assert.IsTrue(updateResult.HasValue);
            Assert.AreEqual(updateResult.Value.ID(), documents[0].ID());
            Assert.AreEqual(updateResult.Value.Key(), documents[0].Key());
            Assert.AreNotEqual(updateResult.Value.Rev(), documents[0].Rev());
            
            var getResult = await collection
                .Get()
                .ById(updateResult.Value.ID());
            
            Assert.AreEqual(getResult.Value.ID(), updateResult.Value.ID());
            Assert.AreEqual(getResult.Value.Key(), updateResult.Value.Key());
            Assert.AreEqual(getResult.Value.Rev(), updateResult.Value.Rev());
            
            Assert.AreNotEqual(getResult.Value["Foo"], documents[0]["Foo"]);
            Assert.AreEqual(getResult.Value["Foo"], document["Foo"]);
            
            Assert.AreNotEqual(getResult.Value["Bar"], documents[0]["Bar"]);
            Assert.AreEqual(getResult.Value["Bar"], document["Bar"]);
            Assert.AreEqual(getResult.Value["Baz"], document["Baz"]);
        }

        [Test]
        public async Task UpdateDocumentWithoutIgnoreRevs()
        {
            var collection = _db.GetCollection<Dictionary<string, object>>(TestDocumentCollectionName);

            var documents = await InsertTestData(_db);

            var document = new Dictionary<string, object>()
                .ID(documents[0].ID())
                .Key(documents[0].Key())
                .Rev(documents[0].Rev());
            document.Add("Foo", "some other new string");
            document.Add("Bar", 54321);
            document.Add("Baz", 12345);

            var updateResult = await collection
                .Update()
                .IgnoreRevs(false)
                .DocumentById(documents[0].ID(), document);

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

            var documents = await InsertTestData(_db);

            var document = new Dictionary<string, object>();
            document.Add("Foo", "some other new string");
            document.Add("Bar", 54321);
            document.Add("Baz", 12345);
         
            var updateResult = await collection
                .Update()
                .IfMatch(documents[0].Rev())
                .DocumentById(documents[0].ID(), document);
            
            Assert.AreEqual(202, updateResult.StatusCode);
            Assert.IsTrue(updateResult.Success);
            Assert.IsTrue(updateResult.HasValue);
            Assert.AreEqual(updateResult.Value.ID(), documents[0].ID());
            Assert.AreEqual(updateResult.Value.Key(), documents[0].Key());
            Assert.AreNotEqual(updateResult.Value.Rev(), documents[0].Rev());
            
            var getResult = await collection
                .Get()
                .ById(updateResult.Value.ID());
            
            Assert.AreEqual(getResult.Value.ID(), updateResult.Value.ID());
            Assert.AreEqual(getResult.Value.Key(), updateResult.Value.Key());
            Assert.AreEqual(getResult.Value.Rev(), updateResult.Value.Rev());
            
            Assert.AreNotEqual(getResult.Value["Foo"], documents[0]["Foo"]);
            Assert.AreEqual(getResult.Value["Foo"], document["Foo"]);
            
            Assert.AreNotEqual(getResult.Value["Bar"], documents[0]["Bar"]);
            Assert.AreEqual(getResult.Value["Bar"], document["Bar"]);
            Assert.AreEqual(getResult.Value["Baz"], document["Baz"]);
        }
        
        [Test]
        public async Task UpdateDocumentWithIfMatchAndReturn412()
        {
            var collection = _db.GetCollection<Dictionary<string, object>>(TestDocumentCollectionName);

            var documents = await InsertTestData(_db);

            var document = new Dictionary<string, object>();
            document.Add("Foo", "some other new string");
            document.Add("Bar", 54321);
            document.Add("Baz", 12345);
         
            var exception = Assert.ThrowsAsync<VersionCheckViolationException>(() =>
            {
                return collection
                    .Update()
                    .IfMatch("123456789")
                    .DocumentById(documents[0].ID(), document);
            });
            
            Assert.AreEqual(exception.Version, documents[0].Rev());
        }
        
        [Test]
        public async Task UpdateDocumentWithKeepNull()
        {
            var collection = _db.GetCollection<Dictionary<string, object>>(TestDocumentCollectionName);

            var newDocument = new Dictionary<string, object>();
            newDocument.Add("Foo", "some string");
            newDocument.Add("Bar", null);
            
            var createResult = await collection
                .Insert()
                .ReturnNew()
                .Document(newDocument);
            
            newDocument = createResult.Value;

            var document = new Dictionary<string, object>();
            document.Add("Foo", "some other new string");
            document.Add("Baz", null);
            
            var updateResult = await collection
                .Update()
                .KeepNull(false)
                .DocumentById(newDocument.ID(), document);
            
            Assert.AreEqual(202, updateResult.StatusCode);
            Assert.IsTrue(updateResult.Success);
            Assert.IsTrue(updateResult.HasValue);
            Assert.AreEqual(updateResult.Value.ID(), newDocument.ID());
            Assert.AreEqual(updateResult.Value.Key(), newDocument.Key());
            Assert.AreNotEqual(updateResult.Value.Rev(), newDocument.Rev());
            
            var getResult = await collection
                .Get()
                .ById(updateResult.Value.ID());
            
            Assert.AreEqual(getResult.Value.ID(), updateResult.Value.ID());
            Assert.AreEqual(getResult.Value.Key(), updateResult.Value.Key());
            Assert.AreEqual(getResult.Value.Rev(), updateResult.Value.Rev());
            
            Assert.AreNotEqual(getResult.Value["Foo"], newDocument["Foo"]);
            Assert.AreEqual(getResult.Value["Foo"], document["Foo"]);
            
            Assert.IsTrue(getResult.Value.ContainsKey("Bar"));
            
            Assert.IsFalse(getResult.Value.ContainsKey("Baz"));
        }
        
        [Test]
        public async Task UpdateDocumentWithMergeArrays()
        {
            var collection = _db.GetCollection<Dictionary<string, object>>(TestDocumentCollectionName);

            var newDocument = new Dictionary<string, object>();
            newDocument.Add("Foo", "some string");
            newDocument.Add("Bar", new Dictionary<string, object>(){{"Foo", "string value"}});
            
            var createResult = await collection
                .Insert()
                .ReturnNew()
                .Document(newDocument);
            
            newDocument = createResult.Value;

            var document = new Dictionary<string, object>();
            document.Add("Foo", "some other new string");
            document.Add("Bar", new Dictionary<string, object>(){{"Bar", "other string value"}});
            
            var updateResult = await collection
                .Update()
                .MergeObjects(true) // this is also default behavior
                .DocumentById(newDocument.ID(), document);
            
            Assert.AreEqual(202, updateResult.StatusCode);
            Assert.IsTrue(updateResult.Success);
            Assert.IsTrue(updateResult.HasValue);
            Assert.AreEqual(updateResult.Value.ID(), newDocument.ID());
            Assert.AreEqual(updateResult.Value.Key(), newDocument.Key());
            Assert.AreNotEqual(updateResult.Value.Rev(), newDocument.Rev());
            
            var getResult = await collection
                .Get()
                .ById(updateResult.Value.ID());
            
            Assert.AreEqual(getResult.Value.ID(), updateResult.Value.ID());
            Assert.AreEqual(getResult.Value.Key(), updateResult.Value.Key());
            Assert.AreEqual(getResult.Value.Rev(), updateResult.Value.Rev());
            
            Assert.AreNotEqual(getResult.Value["Foo"], newDocument["Foo"]);
            Assert.AreEqual(getResult.Value["Foo"], document["Foo"]);
            
            Assert.IsTrue(((Dictionary<string, object>)getResult.Value["Bar"]).ContainsKey("Foo"));
            Assert.IsTrue(((Dictionary<string, object>)getResult.Value["Bar"]).ContainsKey("Bar"));
        }
        
        [Test]
        public async Task UpdateDocumentWithoutMergeArrays()
        {
            var collection = _db.GetCollection<Dictionary<string, object>>(TestDocumentCollectionName);

            var newDocument = new Dictionary<string, object>();
            newDocument.Add("Foo", "some string");
            newDocument.Add("Bar", new Dictionary<string, object>(){{"Foo", "string value"}});
            
            var createResult = await collection
                .Insert()
                .ReturnNew()
                .Document(newDocument);
            
            newDocument = createResult.Value;
            
            var document = new Dictionary<string, object>();
            document.Add("Foo", "some other new string");
            document.Add("Bar", new Dictionary<string, object>(){{"Bar", "other string value"}});
            
            var updateResult = await collection
                .Update()
                .MergeObjects(false)
                .DocumentById(newDocument.ID(), document);
            
            Assert.AreEqual(202, updateResult.StatusCode);
            Assert.IsTrue(updateResult.Success);
            Assert.IsTrue(updateResult.HasValue);
            Assert.AreEqual(updateResult.Value.ID(), newDocument.ID());
            Assert.AreEqual(updateResult.Value.Key(), newDocument.Key());
            Assert.AreNotEqual(updateResult.Value.Rev(), newDocument.Rev());
            
            var getResult = await collection
                .Get()
                .ById(updateResult.Value.ID());
            
            Assert.AreEqual(getResult.Value.ID(), updateResult.Value.ID());
            Assert.AreEqual(getResult.Value.Key(), updateResult.Value.Key());
            Assert.AreEqual(getResult.Value.Rev(), updateResult.Value.Rev());
            
            Assert.AreNotEqual(getResult.Value["Foo"], newDocument["Foo"]);
            Assert.AreEqual(getResult.Value["Foo"], document["Foo"]);
            
            Assert.IsFalse(((Dictionary<string, object>)getResult.Value["Bar"]).ContainsKey("Foo"));
            
            Assert.IsTrue(((Dictionary<string, object>)getResult.Value["Bar"]).ContainsKey("Bar"));
        }
        
        [Test]
        public async Task UpdateDocumentWithGenericObject()
        {
            var collection = _db.GetCollection<Dummy>(TestDocumentCollectionName);

            var documents = await InsertTestData(_db);

            var dummy = new Dummy();
            dummy.Foo = "some other new string";
            dummy.Bar = 54321;
            dummy.Baz = 12345;
         
            var updateResult = await collection
                .Update()
                .DocumentById(documents[0].ID(), dummy);
            
            Assert.AreEqual(202, updateResult.StatusCode);
            Assert.IsTrue(updateResult.Success);
            Assert.IsTrue(updateResult.HasValue);
            Assert.AreEqual(updateResult.Value.Id, documents[0].ID());
            Assert.AreEqual(updateResult.Value.Key, documents[0].Key());
            Assert.AreNotEqual(updateResult.Value.Revision, documents[0].Rev());
            
            var getResult = await collection
                .Get()
                .ById(updateResult.Value.Id);
            
            Assert.AreEqual(getResult.Value.Id, updateResult.Value.Id);
            Assert.AreEqual(getResult.Value.Key, updateResult.Value.Key);
            Assert.AreEqual(getResult.Value.Revision, updateResult.Value.Revision);
            
            Assert.AreNotEqual(getResult.Value.Foo, documents[0]["Foo"]);
            Assert.AreEqual(getResult.Value.Foo, dummy.Foo);
            Assert.AreNotEqual(getResult.Value.Bar, documents[0]["Bar"]);
            Assert.AreEqual(getResult.Value.Bar, dummy.Bar);
            Assert.AreEqual(getResult.Value.Baz, dummy.Baz);
        }
        
        #endregion
        
        #region Replace operations
        
        [Test]
        public async Task ReplaceDocument()
        {
            var collection = _db.GetCollection<Dictionary<string, object>>(TestDocumentCollectionName);

            var documents = await InsertTestData(_db);

            var document = new Dictionary<string, object>();
            document.Add("Foo", "some other new string");
            document.Add("Baz", 54321);
         
            var replaceResult = await collection
                .Replace()
                .DocumentById(documents[0].ID(), document);
            
            Assert.AreEqual(202, replaceResult.StatusCode);
            Assert.IsTrue(replaceResult.Success);
            Assert.IsFalse(replaceResult.HasValue);
            
            var getResult = await collection
                .Get()
                .ById(documents[0].ID());
            
            Assert.AreEqual(getResult.Value.ID(), documents[0].ID());
            Assert.AreEqual(getResult.Value.Key(), documents[0].Key());
            Assert.AreNotEqual(getResult.Value.Rev(), documents[0].Rev());
            
            Assert.AreNotEqual(getResult.Value["Foo"], documents[0]["Foo"]);
            Assert.AreEqual(getResult.Value["Foo"], document["Foo"]);
            
            Assert.AreEqual(getResult.Value["Baz"], document["Baz"]);
            
            Assert.IsFalse(getResult.Value.ContainsKey("Bar"));
        }

        [Test]
        public async Task ReplaceDocumentMultiple()
        {
            var collection = _db.GetCollection<Dictionary<string, object>>(TestDocumentCollectionName);

            var documents = await InsertTestData(_db);

            var document = new Dictionary<string, object>();
            document.Add("Foo", "some other new string");
            document.Add("Baz", 54321);
         
            var replaceResult = await collection
                .Replace()
                .Documents(documents);
            
            Assert.AreEqual(202, replaceResult.StatusCode);
            Assert.IsTrue(replaceResult.Success);
            Assert.IsTrue(replaceResult.HasValue);
            Assert.AreEqual(replaceResult.Value[0].ID(), documents[0].ID());
            Assert.AreEqual(replaceResult.Value[0].Key(), documents[0].Key());
            Assert.AreNotEqual(replaceResult.Value[0].Rev(), documents[0].Rev());
            Assert.AreEqual(replaceResult.Value[1].ID(), documents[1].ID());
            Assert.AreEqual(replaceResult.Value[1].Key(), documents[1].Key());
            Assert.AreNotEqual(replaceResult.Value[1].Rev(), documents[1].Rev());
            
            var getResult = await collection
                .Get()
                .ById(replaceResult.Value[0].ID());
            
            Assert.AreEqual(getResult.Value.ID(), replaceResult.Value[0].ID());
            Assert.AreEqual(getResult.Value.Key(), replaceResult.Value[0].Key());
            Assert.AreEqual(getResult.Value.Rev(), replaceResult.Value[0].Rev());
            Assert.AreNotEqual(getResult.Value.Rev(), documents[1].Rev());
        }
        
        [Test]
        public async Task ReplaceDocumentWithReturnOld()
        {
            var collection = _db.GetCollection<Dictionary<string, object>>(TestDocumentCollectionName);

            var documents = await InsertTestData(_db);

            var document = new Dictionary<string, object>();
            document.Add("Foo", "some other new string");
            document.Add("Baz", 54321);

            var replaceResult = await collection
                .Replace()
                .ReturnOld()
                .DocumentById(documents[0].ID(), document);

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

            var documents = await InsertTestData(_db);

            var document = new Dictionary<string, object>();
            document.Add("Foo", "some other new string");
            document.Add("Baz", 54321);

            var replaceResult = await collection
                .Replace()
                .ReturnNew()
                .DocumentById(documents[0].ID(), document);

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

            var documents = await InsertTestData(_db);

            var document = new Dictionary<string, object>();
            document.Add("Foo", "some other new string");
            document.Add("Baz", 54321);
            
            var replaceResult = await collection
                .Replace()
                .WaitForSync(true)
                .DocumentById(documents[0].ID(), document);
            
            Assert.AreEqual(201, replaceResult.StatusCode);
            Assert.IsTrue(replaceResult.Success);
            Assert.IsFalse(replaceResult.HasValue);
            
            var getResult = await collection
                .Get()
                .ById(documents[0].ID());
            
            Assert.AreEqual(getResult.Value.ID(), documents[0].ID());
            Assert.AreEqual(getResult.Value.Key(), documents[0].Key());
            Assert.AreNotEqual(getResult.Value.Rev(), documents[0].Rev());
            
            Assert.AreNotEqual(getResult.Value["Foo"], documents[0]["Foo"]);
            Assert.AreEqual(getResult.Value["Foo"], document["Foo"]);
            Assert.AreEqual(getResult.Value["Baz"], document["Baz"]);
            
            Assert.IsFalse(getResult.Value.ContainsKey("Bar"));
        }

        [Test]
        public async Task ReplaceDocumentWithoutIgnoreRevs()
        {
            var collection = _db.GetCollection<Dictionary<string, object>>(TestDocumentCollectionName);

            var documents = await InsertTestData(_db);

            var document = new Dictionary<string, object>()
                .ID(documents[0].ID())
                .Key(documents[0].Key())
                .Rev(documents[0].Rev());
            document.Add("Foo", "some other new string");
            document.Add("Baz", 54321);

            var replaceResult = await collection
                .Replace()
                .IgnoreRevs(false)
                .DocumentById(documents[0].ID(), document);

            Assert.AreEqual(202, replaceResult.StatusCode);
            Assert.IsTrue(replaceResult.Success);
            Assert.IsFalse(replaceResult.HasValue);
        }

        [Test]
        public async Task ReplaceDocumentWithIfMatch()
        {
            var collection = _db.GetCollection<Dictionary<string, object>>(TestDocumentCollectionName);

            var documents = await InsertTestData(_db);

            var document = new Dictionary<string, object>();
            document.Add("Foo", "some other new string");
            document.Add("Baz", 54321);
         
            var replaceResult = await collection
                .Replace()
                .IfMatch(documents[0].Rev())
                .DocumentById(documents[0].ID(), document);
            
            Assert.AreEqual(202, replaceResult.StatusCode);
            Assert.IsTrue(replaceResult.Success);
            Assert.IsFalse(replaceResult.HasValue);
            
            var getResult = await collection
                .Get()
                .ById(documents[0].ID());
            
            Assert.AreEqual(getResult.Value.ID(), documents[0].ID());
            Assert.AreEqual(getResult.Value.Key(), documents[0].Key());
            Assert.AreNotEqual(getResult.Value.Rev(), documents[0].Rev());
            
            Assert.AreNotEqual(getResult.Value["Foo"], documents[0]["Foo"]);
            Assert.AreEqual(getResult.Value["Foo"], document["Foo"]);
            Assert.AreEqual(getResult.Value["Baz"], document["Baz"]);
            
            Assert.IsFalse(getResult.Value.ContainsKey("Bar"));
        }
        
        [Test]
        public async Task ReplaceDocumentWithIfMatchAndReturn412()
        {
            var collection = _db.GetCollection<Dictionary<string, object>>(TestDocumentCollectionName);

            var documents = await InsertTestData(_db);

            var document = new Dictionary<string, object>();
            document.Add("Foo", "some other new string");
            document.Add("Baz", 54321);
            
            var exception = Assert.ThrowsAsync<VersionCheckViolationException>(() =>
            {
                return collection
                    .Replace()
                    .IfMatch("123456789")
                    .DocumentById(documents[0].ID(), document);
            });
            
            Assert.AreEqual(exception.Version, documents[0].Rev());
        }
        
        [Test]
        public async Task ReplaceDocumentWithGenericObject()
        {
            var collection = _db.GetCollection<Dummy>(TestDocumentCollectionName);

            var documents = await InsertTestData(_db);

            var dummy = new Dummy();
            dummy.Foo = "some other new string";
            dummy.Baz = 54321;
         
            var replaceResult = await collection
                .Replace()
                .DocumentById(documents[0].ID(), dummy);
            
            Assert.AreEqual(202, replaceResult.StatusCode);
            Assert.IsTrue(replaceResult.Success);
            Assert.IsFalse(replaceResult.HasValue);
            
            var getResult = await collection
                .Get()
                .ById(documents[0].ID());
            
            Assert.AreEqual(getResult.Value.Id, documents[0].ID());
            Assert.AreEqual(getResult.Value.Key, documents[0].Key());
            Assert.AreNotEqual(getResult.Value.Revision, documents[0].Rev());
            
            Assert.AreNotEqual(getResult.Value.Foo, documents[0]["Foo"]);
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

            var documents = await InsertTestData(_db);

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

            var documents = await InsertTestData(_db);

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

            var documents = await InsertTestData(_db);

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

            var documents = await InsertTestData(_db);

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

            var documents = await InsertTestData(_db);

            var exception = Assert.ThrowsAsync<VersionCheckViolationException>(() =>
            {
                return collection
                    .Delete()
                    .IfMatch("123456789")
                    .ById(documents[0].ID());
            });
            
            Assert.AreEqual(exception.Version, documents[0].Rev());
        }

        [Test]
        public async Task DeleteDocumentWithReturnOld()
        {
            var collection = _db.GetCollection<Dictionary<string, object>>(TestDocumentCollectionName);

            var documents = await InsertTestData(_db);

            var deleteResult = await collection
                .Delete()
                .ReturnOld()
                .ById(documents[0].ID());

            Assert.AreEqual(202, deleteResult.StatusCode);
            Assert.IsTrue(deleteResult.Success);
            Assert.AreEqual(deleteResult.Value.ID(), documents[0].ID());
            Assert.AreEqual(deleteResult.Value.Key(), documents[0].Key());
            Assert.AreEqual(deleteResult.Value.Rev(), documents[0].Rev());
            Assert.IsTrue(deleteResult.Value.ContainsKey("old"));
        }

        #endregion
        
        #region Special cases
        
        #endregion
    }
}
