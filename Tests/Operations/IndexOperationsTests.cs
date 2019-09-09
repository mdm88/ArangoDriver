using System.Collections.Generic;
using System.Threading.Tasks;
using Arango.Tests;
using ArangoDriver.Client;
using ArangoDriver.Exceptions;
using ArangoDriver.External.dictator;
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
            Assert.AreEqual(AIndexType.Fulltext, createResult.Value.Enum<AIndexType>("type"));
            Assert.IsFalse(createResult.Value.Bool("unique"));
            Assert.IsTrue(createResult.Value.Bool("sparse"));
            
            Assert.AreEqual(1, createResult.Value.Size("fields"));
            new List<string> { "Foo" }.ForEach(field => Assert.IsTrue(createResult.Value.List<string>("fields").Contains(field)));
            
            Assert.IsTrue(createResult.Value.Bool("isNewlyCreated"));
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
            Assert.AreEqual("geo", createResult.Value.String("type"));
            Assert.IsFalse(createResult.Value.Bool("unique"));
            Assert.IsTrue(createResult.Value.Bool("sparse"));
            
            Assert.AreEqual(1, createResult.Value.Size("fields"));
            new List<string> { "Foo" }.ForEach(field => Assert.IsTrue(createResult.Value.List<string>("fields").Contains(field)));
            
            Assert.IsTrue(createResult.Value.Bool("isNewlyCreated"));
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
            Assert.AreEqual(AIndexType.Hash, createResult.Value.Enum<AIndexType>("type"));
            Assert.IsTrue(createResult.Value.Bool("unique"));
            Assert.IsFalse(createResult.Value.Bool("sparse"));
            Assert.AreEqual(1, createResult.Value.Int("selectivityEstimate"));
            
            Assert.AreEqual(2, createResult.Value.Size("fields"));
            new List<string> { "Foo", "Bar" }.ForEach(field => Assert.IsTrue(createResult.Value.List<string>("fields").Contains(field)));
            
            Assert.IsTrue(createResult.Value.Bool("isNewlyCreated"));
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
            Assert.AreEqual(AIndexType.Skiplist, createResult.Value.Enum<AIndexType>("type"));
            Assert.IsFalse(createResult.Value.Bool("unique"));
            Assert.IsFalse(createResult.Value.Bool("sparse"));
            
            Assert.AreEqual(2, createResult.Value.Size("fields"));
            new List<string> { "Foo", "Bar" }.ForEach(field => Assert.IsTrue(createResult.Value.List<string>("fields").Contains(field)));
            
            Assert.IsTrue(createResult.Value.Bool("isNewlyCreated"));
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
            Assert.AreEqual(AIndexType.Hash, createResult.Value.Enum<AIndexType>("type"));
            Assert.IsTrue(createResult.Value.Bool("unique"));
            Assert.IsFalse(createResult.Value.Bool("sparse"));
            Assert.AreEqual(1, createResult.Value.Int("selectivityEstimate"));
            
            Assert.AreEqual(2, createResult.Value.Size("fields"));
            new List<string> { "Foo", "Bar" }.ForEach(field => Assert.IsTrue(createResult.Value.List<string>("fields").Contains(field)));
            
            Assert.IsTrue(createResult.Value.Bool("isNewlyCreated"));
            
            var recreateResult = await _collection.Index
                .New(AIndexType.Hash)
                .Unique(true)
                .Fields("Foo", "Bar")
                .Create();
            
            Assert.AreEqual(200, recreateResult.StatusCode);
            Assert.IsTrue(recreateResult.Success);
            Assert.IsTrue(recreateResult.HasValue);
            Assert.IsTrue(recreateResult.Value.IsID("id"));
            Assert.AreEqual(AIndexType.Hash, recreateResult.Value.Enum<AIndexType>("type"));
            Assert.IsTrue(recreateResult.Value.Bool("unique"));
            Assert.IsFalse(recreateResult.Value.Bool("sparse"));
            Assert.AreEqual(1, recreateResult.Value.Int("selectivityEstimate"));
            
            Assert.AreEqual(2, recreateResult.Value.Size("fields"));
            new List<string> { "Foo", "Bar" }.ForEach(field => Assert.IsTrue(recreateResult.Value.List<string>("fields").Contains(field)));
            
            Assert.IsFalse(recreateResult.Value.Bool("isNewlyCreated"));
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
                .Get(createResult.Value.String("id"));
            
            Assert.AreEqual(200, getResult.StatusCode);
            Assert.IsTrue(getResult.Success);
            Assert.IsTrue(getResult.HasValue);
            Assert.IsTrue(getResult.Value.IsID("id"));
            Assert.AreEqual(createResult.Value.String("id"), getResult.Value.String("id"));
            Assert.AreEqual(AIndexType.Hash, getResult.Value.Enum<AIndexType>("type"));
            Assert.IsTrue(getResult.Value.Bool("unique"));
            Assert.IsFalse(getResult.Value.Bool("sparse"));
            
            Assert.AreEqual(2, getResult.Value.Size("fields"));
            new List<string> { "Foo", "Bar" }.ForEach(field => Assert.IsTrue(getResult.Value.List<string>("fields").Contains(field)));
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
                .Delete(createResult.Value.String("id"));
            
            Assert.AreEqual(200, deleteResult.StatusCode);
            Assert.IsTrue(deleteResult.Success);
            Assert.IsTrue(deleteResult.HasValue);
            Assert.IsTrue(deleteResult.Value.IsID("id"));
            Assert.AreEqual(createResult.Value.String("id"), deleteResult.Value.String("id"));
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
