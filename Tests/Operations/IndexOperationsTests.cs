using System.Collections.Generic;
using System.Threading.Tasks;
using Arango.Tests;
using ArangoDriver.Client;
using ArangoDriver.Exceptions;
using NUnit.Framework;

namespace Tests.Operations
{
    [TestFixture]
    public class IndexOperationsTests : TestBase
    {
        private ADatabase _db;
        private ACollection<object> _collection;

        [SetUp]
        public async Task Setup()
        {
            await Connection.CreateDatabase(TestDatabaseGeneral);

            _db = Connection.GetDatabase(TestDatabaseGeneral);
            
            await _db.CreateCollection(TestDocumentCollectionName)
                .Type(ACollectionType.Document)
                .Create();

            _collection = _db.GetCollection<object>(TestDocumentCollectionName);
        }
        
        [Test]
        public async Task Should_create_fulltext_index()
        {
            var createResult = await _collection.Index
                .New(AIndexType.Fulltext)
                .Fields("Foo")
                .Create();
            
            Assert.AreEqual(201, createResult.StatusCode);
            Assert.IsTrue(createResult.Success);
            Assert.IsTrue(createResult.HasValue);
            Assert.IsTrue(createResult.Value.IsID("id"));
            Assert.AreEqual(AIndexType.Fulltext.ToString().ToLower(), createResult.Value["type"]);
            Assert.IsFalse((bool) createResult.Value["unique"]);
            Assert.IsTrue((bool) createResult.Value["sparse"]);
            
            Assert.AreEqual(1, ((List<string>)createResult.Value["fields"]).Count);
            new List<string> { "Foo" }.ForEach(field => Assert.IsTrue(((List<string>)createResult.Value["fields"]).Contains(field)));
            
            Assert.IsTrue((bool) createResult.Value["isNewlyCreated"]);
        }
        
        [Test]
        public async Task Should_create_geo_index()
        {
            var createResult = await _collection.Index
                .New(AIndexType.Geo)
                .Fields("Foo")
                .Create();
            
            Assert.AreEqual(201, createResult.StatusCode);
            Assert.IsTrue(createResult.Success);
            Assert.IsTrue(createResult.HasValue);
            Assert.IsTrue(createResult.Value.IsID("id"));
            Assert.AreEqual("geo", createResult.Value["type"]);
            Assert.IsFalse((bool) createResult.Value["unique"]);
            Assert.IsTrue((bool) createResult.Value["sparse"]);
            
            Assert.AreEqual(1, ((List<string>)createResult.Value["fields"]).Count);
            new List<string> { "Foo" }.ForEach(field => Assert.IsTrue(((List<string>)createResult.Value["fields"]).Contains(field)));
            
            Assert.IsTrue((bool) createResult.Value["isNewlyCreated"]);
        }
        
        [Test]
        public async Task Should_create_hash_index()
        {
            var createResult = await _collection.Index
                .New(AIndexType.Hash)
                .Unique(true)
                .Fields("Foo", "Bar")
                .Create();
            
            Assert.AreEqual(201, createResult.StatusCode);
            Assert.IsTrue(createResult.Success);
            Assert.IsTrue(createResult.HasValue);
            Assert.IsTrue(createResult.Value.IsID("id"));
            Assert.AreEqual(AIndexType.Hash.ToString().ToLower(), createResult.Value["type"]);
            Assert.IsTrue((bool) createResult.Value["unique"]);
            Assert.IsFalse((bool) createResult.Value["sparse"]);
            Assert.AreEqual(1, createResult.Value["selectivityEstimate"]);
            
            Assert.AreEqual(2, ((List<string>)createResult.Value["fields"]).Count);
            new List<string> { "Foo", "Bar" }.ForEach(field => Assert.IsTrue(((List<string>)createResult.Value["fields"]).Contains(field)));
            
            Assert.IsTrue((bool) createResult.Value["isNewlyCreated"]);
        }
        
        [Test]
        public async Task Should_create_skiplist_index()
        {
            var createResult = await _collection.Index
                .New(AIndexType.Skiplist)
                .Unique(false)
                .Fields("Foo", "Bar")
                .Create();
            
            Assert.AreEqual(201, createResult.StatusCode);
            Assert.IsTrue(createResult.Success);
            Assert.IsTrue(createResult.HasValue);
            Assert.IsTrue(createResult.Value.IsID("id"));
            Assert.AreEqual(AIndexType.Skiplist.ToString().ToLower(), createResult.Value["type"]);
            Assert.IsFalse((bool) createResult.Value["unique"]);
            Assert.IsFalse((bool) createResult.Value["sparse"]);
            
            Assert.AreEqual(2, ((List<string>)createResult.Value["fields"]).Count);
            new List<string> { "Foo", "Bar" }.ForEach(field => Assert.IsTrue(((List<string>)createResult.Value["fields"]).Contains(field)));
            
            Assert.IsTrue((bool) createResult.Value["isNewlyCreated"]);
        }
        
        [Test]
        public async Task Should_recreate_hash_index()
        {
            var createResult = await _collection.Index
                .New(AIndexType.Hash)
                .Unique(true)
                .Fields("Foo", "Bar")
                .Create();
            
            Assert.AreEqual(201, createResult.StatusCode);
            Assert.IsTrue(createResult.Success);
            Assert.IsTrue(createResult.HasValue);
            Assert.IsTrue(createResult.Value.IsID("id"));
            Assert.AreEqual(AIndexType.Hash.ToString().ToLower(), createResult.Value["type"]);
            Assert.IsTrue((bool) createResult.Value["unique"]);
            Assert.IsFalse((bool) createResult.Value["sparse"]);
            Assert.AreEqual(1, createResult.Value["selectivityEstimate"]);
            
            Assert.AreEqual(2, ((List<string>)createResult.Value["fields"]).Count);
            new List<string> { "Foo", "Bar" }.ForEach(field => Assert.IsTrue(((List<string>)createResult.Value["fields"]).Contains(field)));
            
            Assert.IsTrue((bool) createResult.Value["isNewlyCreated"]);
            
            var recreateResult = await _collection.Index
                .New(AIndexType.Hash)
                .Unique(true)
                .Fields("Foo", "Bar")
                .Create();
            
            Assert.AreEqual(200, recreateResult.StatusCode);
            Assert.IsTrue(recreateResult.Success);
            Assert.IsTrue(recreateResult.HasValue);
            Assert.IsTrue(recreateResult.Value.IsID("id"));
            Assert.AreEqual(AIndexType.Hash.ToString().ToLower(), recreateResult.Value["type"]);
            Assert.IsTrue((bool) recreateResult.Value["unique"]);
            Assert.IsFalse((bool) recreateResult.Value["sparse"]);
            Assert.AreEqual(1, recreateResult.Value["selectivityEstimate"]);
            
            Assert.AreEqual(2, ((List<string>)createResult.Value["fields"]).Count);
            new List<string> { "Foo", "Bar" }.ForEach(field => Assert.IsTrue(((List<string>)createResult.Value["fields"]).Contains(field)));
            
            Assert.IsFalse((bool) recreateResult.Value["isNewlyCreated"]);
        }
        
        [Test]
        public async Task Should_get_index()
        {
            var createResult = await _collection.Index
                .New(AIndexType.Hash)
                .Unique(true)
                .Fields("Foo", "Bar")
                .Create();
            
            Assert.AreEqual(201, createResult.StatusCode);
            Assert.IsTrue(createResult.Success);
            
            var getResult = await _collection.Index
                .Get((string)createResult.Value["id"]);
            
            Assert.AreEqual(200, getResult.StatusCode);
            Assert.IsTrue(getResult.Success);
            Assert.IsTrue(getResult.HasValue);
            Assert.IsTrue(getResult.Value.IsID("id"));
            Assert.AreEqual(createResult.Value["id"], getResult.Value["id"]);
            Assert.AreEqual(AIndexType.Hash.ToString().ToLower(), getResult.Value["type"]);
            Assert.IsTrue((bool) getResult.Value["unique"]);
            Assert.IsFalse((bool) getResult.Value["sparse"]);
            
            Assert.AreEqual(2, ((List<string>)createResult.Value["fields"]).Count);
            new List<string> { "Foo", "Bar" }.ForEach(field => Assert.IsTrue(((List<string>)createResult.Value["fields"]).Contains(field)));
        }
        
        [Test]
        public async Task Should_delete_index()
        {
            var createResult = await _collection.Index
                .New(AIndexType.Hash)
                .Unique(true)
                .Fields("Foo", "Bar")
                .Create();
            
            Assert.AreEqual(201, createResult.StatusCode);
            Assert.IsTrue(createResult.Success);
            
            var deleteResult = await _collection.Index
                .Delete((string) createResult.Value["id"]);
            
            Assert.AreEqual(200, deleteResult.StatusCode);
            Assert.IsTrue(deleteResult.Success);
            Assert.IsTrue(deleteResult.HasValue);
            Assert.IsTrue(deleteResult.Value.IsID("id"));
            Assert.AreEqual(createResult.Value["id"], deleteResult.Value["id"]);
        }
        

        [Test]
        public async Task UniqueConstraintOk()
        {
            var collection = _db.GetCollection<Dummy>(TestDocumentCollectionName);
            
            var indexResult = await collection.Index
                .New(AIndexType.Hash)
                .Unique(true)
                .Fields("Foo")
                .Create();
            
            Assert.AreEqual(201, indexResult.StatusCode);
            Assert.IsTrue(indexResult.Success);
            
            // First Doc
            var doc1 = new Dummy()
            {
                Foo = "asd",
                Bar = 1
            };
            var createResult = await collection.Insert().Document(doc1);
            
            Assert.AreEqual(202, createResult.StatusCode);
            Assert.IsTrue(createResult.Success);
            
            // Second Doc
            var doc2 = new Dummy()
            {
                Foo = "gfd",
                Bar = 1
            };
            createResult = await collection.Insert().Document(doc2);
            
            Assert.AreEqual(202, createResult.StatusCode);
            Assert.IsTrue(createResult.Success);
        }

        [Test]
        public async Task UniqueConstraintViolation()
        {
            var collection = _db.GetCollection<Dummy>(TestDocumentCollectionName);
            
            var indexResult = await collection.Index
                .New(AIndexType.Hash)
                .Unique(true)
                .Fields("Foo")
                .Create();
            
            Assert.AreEqual(201, indexResult.StatusCode);
            Assert.IsTrue(indexResult.Success);
            
            // First Doc
            var doc1 = new Dummy()
            {
                Foo = "asd",
                Bar = 1
            };
            var createResult = await collection.Insert().Document(doc1);
            
            Assert.AreEqual(202, createResult.StatusCode);
            Assert.IsTrue(createResult.Success);
            
            // Duplicated
            Assert.ThrowsAsync<UniqueConstraintViolationException>(() =>
            {
                var doc3 = new Dummy()
                {
                    Foo = "asd",
                    Bar = 2
                };
                return collection.Insert().Document(doc3);
            });
        }

        [Test]
        public async Task UniqueConstraintViolationMulti()
        {
            var collection = _db.GetCollection<Dummy>(TestDocumentCollectionName);
            
            var indexResult = await collection.Index
                .New(AIndexType.Hash)
                .Unique(true)
                .Fields("Foo")
                .Create();
            
            Assert.AreEqual(201, indexResult.StatusCode);
            Assert.IsTrue(indexResult.Success);
            
            // First Doc
            var doc1 = new Dummy()
            {
                Foo = "asd",
                Bar = 1
            };
            var createResult = await collection.Insert().Document(doc1);
            
            Assert.AreEqual(202, createResult.StatusCode);
            Assert.IsTrue(createResult.Success);
            
            // Duplicated
            Assert.ThrowsAsync<MultipleException>(() =>
            {
                var dup = new List<Dummy>()
                {
                    new Dummy()
                    {
                        Foo = "asd",
                        Bar = 2
                    },
                    new Dummy()
                    {
                        Foo = "wer",
                        Bar = 2
                    }
                };
                return collection.Insert().Documents(dup);
            });
        }
    }
}
