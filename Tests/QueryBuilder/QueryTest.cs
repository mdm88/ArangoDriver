using System.Threading.Tasks;
using Arango.Tests;
using ArangoDriver.Client;
using ArangoDriver.Client.Query;
using ArangoDriver.Client.Query.Query;
using ArangoDriver.Client.Query.Value;
using NUnit.Framework;

namespace Tests.QueryBuilder
{
    [TestFixture]
    public class QueryTest : TestBase
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
        
        [Test]
        public void ForTest()
        {
            AQuery query = _db.Query
                .For("x", AValue.Field(TestDocumentCollectionName));
            
            Assert.AreEqual("FOR x IN " + TestDocumentCollectionName, query.GetExpression());
        }
        
        [Test]
        public void LimitTest()
        {
            AQuery query = _db.Query
                .Limit(5);
            
            Assert.AreEqual("LIMIT 5", query.GetExpression());
        }
        
        [Test]
        public void SortTest()
        {
            AQuery query = _db.Query
                .Sort(AValue<Dummy>.Field(x => x.Foo), AqlSort.Direction.Desc);
            
            Assert.AreEqual("SORT x.Foo DESC", query.GetExpression());
        }
        
        [Test]
        public void LetTest()
        {
            AQuery query = _db.Query
                .Let("x", AValue.Document<Dummy>(TestDocumentCollectionName, AValue.Field<string>("asd")));

            Assert.AreEqual("LET x = DOCUMENT('" + TestDocumentCollectionName + "',asd)", query.GetExpression());
        }
        
        [Test]
        public void LetSubQueryTest()
        {

            AQuery subquery = _db.Query
                .For("y", AValue.Field(TestDocumentCollectionName))
                .Collect()
                .Aggregate("max", ANumeric.Max(AValue.Field("y.value")))
                .Return("max");
                
            AQuery query = _db.Query
                .Let("x", AValue.Subquery<int>(subquery));

            Assert.AreEqual("LET x = (FOR y IN " + TestDocumentCollectionName + " COLLECT AGGREGATE max = MAX(y.value) RETURN max)", query.GetExpression());
        }
        
        [Test]
        public void ReturnTest()
        {
            AQuery query = _db.Query
                .Return("x");
            
            Assert.AreEqual("RETURN x", query.GetExpression());
        }
        
        [Test]
        public void ReturnPartialTest()
        {
            AQuery query = _db.Query
                .Return<Dummy>(x => x.Key, x => x.Foo);
            
            Assert.AreEqual("RETURN {_key:x._key, Foo:x.Foo}", query.GetExpression());
        }
    }
}