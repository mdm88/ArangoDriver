using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Arango.Tests;
using ArangoDriver.Client;
using ArangoDriver.External.dictator;
using NUnit.Framework;

namespace Tests.QueryOperations
{
    [TestFixture]
    public class QueryOperationsTests : TestBase
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
        
        #region ToDocument(s)
        
        [Test]
        public async Task Should_execute_AQL_query_with_document_result()
        {
            await InsertTestData(_db);
            
            var queryResult = await _db.Query
                .Aql($@"
                FOR item IN {TestDocumentCollectionName}
                    LIMIT 1
                    RETURN item
                ")
                .ToObject<Dictionary<string, object>>();

            Assert.AreEqual(201, queryResult.StatusCode);
            Assert.IsTrue(queryResult.Success);
            Assert.IsTrue(queryResult.HasValue);
            Assert.IsTrue(queryResult.Value.IsString("Foo"));
            Assert.IsTrue(queryResult.Value.IsLong("Bar"));
        }
        
        [Test]
        public async Task Should_execute_AQL_query_with_document_list_result()
        {
            await InsertTestData(_db);

            var queryResult = await _db.Query
                .Aql($@"
                FOR item IN {TestDocumentCollectionName}
                    RETURN item
                ")
                .ToList<Dictionary<string, object>>();

            Assert.AreEqual(201, queryResult.StatusCode);
            Assert.IsTrue(queryResult.Success);
            Assert.IsTrue(queryResult.HasValue);
            Assert.AreEqual(2, queryResult.Value.Count);
            Assert.IsTrue(queryResult.Value[0].IsString("Foo"));
            Assert.IsTrue(queryResult.Value[0].IsLong("Bar"));
            Assert.IsTrue(queryResult.Value[1].IsString("Foo"));
            Assert.IsTrue(queryResult.Value[1].IsLong("Bar"));
        }
        
        #endregion
        
        #region ToObject
        
        [Test]
        public async Task Should_execute_AQL_query_with_single_object_result()
        {
            await InsertTestData(_db);

            var queryResult = await _db.Query
                .Aql($@"
                FOR item IN {TestDocumentCollectionName}
                    LIMIT 1
                    RETURN item
                ")
                .ToObject();

            Assert.AreEqual(201, queryResult.StatusCode);
            Assert.IsTrue(queryResult.Success);
            Assert.IsTrue(queryResult.HasValue);
            Assert.IsTrue(queryResult.Value != null);
        }
        
        [Test]
        public async Task Should_execute_AQL_query_with_single_primitive_object_result()
        {
            await InsertTestData(_db);

            var queryResult = await _db.Query
                .Aql($@"
                FOR item IN {TestDocumentCollectionName}
                    SORT item.Bar
                    LIMIT 1
                    RETURN item.Bar
                ")
                .ToObject<int>();

            Assert.AreEqual(201, queryResult.StatusCode);
            Assert.IsTrue(queryResult.Success);
            Assert.IsTrue(queryResult.HasValue);
            Assert.AreEqual(1, queryResult.Value);
        }
        
        [Test]
        public async Task Should_execute_AQL_query_with_single_dictionary_object_result()
        {
            await InsertTestData(_db);

            var queryResult = await _db.Query
                .Aql($@"
                FOR item IN {TestDocumentCollectionName}
                    SORT item.Bar
                    LIMIT 1
                    RETURN item
                ")
                .ToObject<Dictionary<string, object>>();

            Assert.AreEqual(201, queryResult.StatusCode);
            Assert.IsTrue(queryResult.Success);
            Assert.IsTrue(queryResult.HasValue);
            Assert.IsTrue(queryResult.Value.IsString("Foo"));
            Assert.IsTrue(queryResult.Value.IsLong("Bar"));
        }
        
        [Test]
        public async Task Should_execute_AQL_query_with_single_strongly_typed_object_result()
        {
            var documents = await InsertTestData(_db);

            var queryResult = await _db.Query
                .Aql($@"
                FOR item IN {TestDocumentCollectionName}
                    SORT item.Bar
                    LIMIT 1
                    RETURN item
                ")
                .ToObject<Dummy>();

            Assert.AreEqual(201, queryResult.StatusCode);
            Assert.IsTrue(queryResult.Success);
            Assert.IsTrue(queryResult.HasValue);
            Assert.IsTrue(documents.First(q => q.String("Foo") == queryResult.Value.Foo) != null);
            Assert.IsTrue(documents.First(q => q.Int("Bar") == queryResult.Value.Bar) != null);
        }
        
        [Test]
        public async Task Should_execute_AQL_query_with_no_result_object()
        {
            var queryResult = await _db.Query
                .Aql(@"
                LET items = []
                FOR item IN items
                    RETURN item
                ")
                .ToObject();

            Assert.AreEqual(201, queryResult.StatusCode);
            Assert.IsTrue(queryResult.Success);
            Assert.IsFalse(queryResult.HasValue);
            Assert.IsNull(queryResult.Value);
        }
        
        [Test]
        public async Task Should_execute_AQL_query_with_no_result_strongly_typed()
        {
            var queryResult = await _db.Query
                .Aql(@"
                LET items = []
                FOR item IN items
                    RETURN item
                ")
                .ToObject<Dummy>();

            Assert.AreEqual(201, queryResult.StatusCode);
            Assert.IsTrue(queryResult.Success);
            Assert.IsFalse(queryResult.HasValue);
            Assert.IsNull(queryResult.Value);
        }
        
        #endregion
        
        #region ToList
        
        [Test]
        public async Task Should_execute_AQL_query_with_list_result()
        {
            await InsertTestData(_db);

            var queryResult = await _db.Query
                .Aql($@"
                FOR item IN {TestDocumentCollectionName}
                    RETURN item
                ")
                .ToList<object>();

            Assert.AreEqual(201, queryResult.StatusCode);
            Assert.IsTrue(queryResult.Success);
            Assert.IsTrue(queryResult.HasValue);
            Assert.AreEqual(2, queryResult.Value.Count);
        }
        
        [Test]
        public async Task Should_execute_AQL_query_with_primitive_list_result()
        {
            await InsertTestData(_db);

            var queryResult = await _db.Query
                .Aql($@"
                FOR item IN {TestDocumentCollectionName}
                    SORT item.Bar
                    RETURN item.Bar
                ")
                .ToList<int>();

            Assert.AreEqual(201, queryResult.StatusCode);
            Assert.IsTrue(queryResult.Success);
            Assert.IsTrue(queryResult.HasValue);
            Assert.AreEqual(2, queryResult.Value.Count);
            Assert.AreEqual(1, queryResult.Value[0]);
            Assert.AreEqual(2, queryResult.Value[1]);
        }
        
        [Test]
        public async Task Should_execute_AQL_query_with_dictionary_list_result()
        {
            await InsertTestData(_db);

            var queryResult = await _db.Query
                .Aql($@"
                FOR item IN {TestDocumentCollectionName}
                    RETURN item
                ")
                .ToList<Dictionary<string, object>>();

            Assert.AreEqual(201, queryResult.StatusCode);
            Assert.IsTrue(queryResult.Success);
            Assert.IsTrue(queryResult.HasValue);
            Assert.AreEqual(2, queryResult.Value.Count);
            Assert.IsTrue(queryResult.Value[0].IsString("Foo"));
            Assert.IsTrue(queryResult.Value[0].IsLong("Bar"));
            Assert.IsTrue(queryResult.Value[1].IsString("Foo"));
            Assert.IsTrue(queryResult.Value[1].IsLong("Bar"));
        }
        
        [Test]
        public async Task Should_execute_AQL_query_with_strongly_typed_list_result()
        {
            var documents = await InsertTestData(_db);

            var queryResult = await _db.Query
                .Aql($@"
                FOR item IN {TestDocumentCollectionName}
                    SORT item.Bar
                    RETURN item
                ")
                .ToList<Dummy>();

            Assert.AreEqual(201, queryResult.StatusCode);
            Assert.IsTrue(queryResult.Success);
            Assert.IsTrue(queryResult.HasValue);
            Assert.AreEqual(2, queryResult.Value.Count);
            Assert.AreEqual(documents[0].String("Foo"), queryResult.Value[0].Foo);
            Assert.AreEqual(documents[0].Int("Bar"), queryResult.Value[0].Bar);
            Assert.AreEqual(documents[1].String("Foo"), queryResult.Value[1].Foo);
            Assert.AreEqual(documents[1].Int("Bar"), queryResult.Value[1].Bar);
        }

        #endregion

        #region ExecuteNonQuery

        [Test]
        public async Task Should_execute_non_query_result()
        {
            await InsertTestData(_db);

            var queryResult = await _db.Query
                .Aql($@"
                UPSERT {{ Bar: 1 }}
                INSERT {{ Foo: 'some string value', Bar: 1 }} 
                UPDATE {{ Foo: 'some string value updated', Bar: 2 }}
                IN {TestDocumentCollectionName}
                ")
                .ExecuteNonQuery();

            Assert.AreEqual(201, queryResult.StatusCode);
            Assert.IsTrue(queryResult.Success);
            Assert.IsFalse(queryResult.HasValue);
            Assert.IsNull(queryResult.Value);
        }

        #endregion

        [Test]
        public async Task Should_execute_AQL_query_with_count()
        {
            await InsertTestData(_db);

            var queryResult = await _db.Query
                .Count(true)
                .Aql($@"
                FOR item IN {TestDocumentCollectionName}
                    RETURN item
                ")
                .ToList<object>();

            Assert.AreEqual(201, queryResult.StatusCode);
            Assert.IsTrue(queryResult.Success);
            Assert.IsTrue(queryResult.HasValue);
            Assert.AreEqual(queryResult.Value.Count, 2);
            Assert.AreEqual(queryResult.Extra.Long("count"), 2);
        }
        
        [Test]
        public async Task Should_execute_AQL_query_with_batchSize()
        {
            await InsertTestData(_db);

            var collection = _db.GetCollection<Dictionary<string, object>>(TestDocumentCollectionName);
            
            var doc3 = new Dictionary<string, object>()
                .String("Foo", "Foo string 3");
            await collection.Insert().Document(doc3);
            
            var doc4 = new Dictionary<string, object>()
                .String("Foo", "Foo string 4");
            await collection.Insert().Document(doc4);
            
            var queryResult = await _db.Query
                .BatchSize(1)
                .Aql($@"
                FOR item IN {TestDocumentCollectionName}
                    RETURN item
                ")
                .ToList<object>();

            Assert.AreEqual(200, queryResult.StatusCode);
            Assert.IsTrue(queryResult.Success);
            Assert.IsTrue(queryResult.HasValue);
            Assert.AreEqual(queryResult.Value.Count, 4);
        }
        
        [Test]
        public async Task Should_execute_AQL_query_with_bindVar()
        {
            await InsertTestData(_db);

            var queryResult = await _db.Query
                .BindVar("barNumber", 1)
                .Aql($@"
                FOR item IN {TestDocumentCollectionName}
                    FILTER item.Bar == @barNumber
                    RETURN item
                ")
                .ToList<object>();

            Assert.AreEqual(201, queryResult.StatusCode);
            Assert.IsTrue(queryResult.Success);
            Assert.IsTrue(queryResult.HasValue);
            Assert.AreEqual(queryResult.Value.Count, 1);
        }
        
        [Test]
        public async Task Should_execute_AQL_query_fluent()
        {
            await InsertTestData(_db);
            
            var queryOperation = _db.Query
                .Aql($@"
                FOR item IN {TestDocumentCollectionName}
                    RETURN item
                ");

            queryOperation.Count(true);
            queryOperation.BatchSize(1);

            var queryResult = await queryOperation.ToList<object>();

            Assert.AreEqual(200, queryResult.StatusCode);
            Assert.IsTrue(queryResult.Success);
            Assert.IsTrue(queryResult.HasValue);
            Assert.AreEqual(queryResult.Value.Count, 2);
            Assert.AreEqual(queryResult.Extra.Long("count"), 2);
        }
        
        [Test]
        public async Task Should_parse_query()
        {
            await InsertTestData(_db);

            var parseResult = await _db.Query
                .Parse($@"
                FOR item IN {TestDocumentCollectionName}
                    RETURN item
                ");
            
            Assert.AreEqual(200, parseResult.StatusCode);
            Assert.IsTrue(parseResult.Success);
            Assert.IsTrue(parseResult.HasValue);
            Assert.IsTrue(parseResult.Value.IsList("bindVars"));
            Assert.IsTrue(parseResult.Value.IsList("collections"));
            Assert.IsTrue(parseResult.Value.IsList("ast"));
        }
        
        [Test]
        public async Task Should_minify_query()
        {
            var singleLineQuery = AQuery.Minify(@"
            FOR item IN MyDocumentCollection
                RETURN item
            ");
            
            Assert.AreEqual("FOR item IN MyDocumentCollection\nRETURN item\n", singleLineQuery);
        }
        
        [Test]
        public async Task Should_return_404_with_deleteCursor_operation()
        {
            await InsertTestData(_db);

            var queryResult = await _db.Query
                .BatchSize(1)
                .Aql($@"
                FOR item IN {TestDocumentCollectionName}
                    RETURN item
                ")
                .ToList<object>();

            Assert.IsTrue(queryResult.Extra.IsString("id"));
            
            var deleteCursorResult = await _db.Query
                .DeleteCursor(queryResult.Extra.String("id"));
            
            Assert.AreEqual(404, deleteCursorResult.StatusCode);
            Assert.IsFalse(deleteCursorResult.Success);
            Assert.IsTrue(deleteCursorResult.HasValue);
            Assert.IsFalse(deleteCursorResult.Value);
        }
    }
}
