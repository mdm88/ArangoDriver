using System.Linq.Expressions;
using System.Threading.Tasks;
using Arango.Tests;
using ArangoDriver.Client;
using ArangoDriver.Client.Query;
using ArangoDriver.Client.Query.Query;
using ArangoDriver.Client.Query.Return;
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
                .Return(AReturn.Variable("max"));
                
            AQuery query = _db.Query
                .Let("x", AValue.Subquery<int>(subquery));

            Assert.AreEqual("LET x = (FOR y IN " + TestDocumentCollectionName + " COLLECT AGGREGATE max = MAX(y.value) RETURN max)", query.GetExpression());
        }

        [Test]
        public void InsertTest()
        {
            var insert = new Dummy()
            {
                Foo = "asd",
                Bar = 10
            };

            AQuery query = _db.Query
                .Insert("collection", AValue.Bind(insert));

            Assert.AreEqual("INSERT @var0 INTO collection", query.GetExpression());
            Assert.AreEqual(insert, query.GetBindedVars()[0]);
        }

        [Test]
        public void InsertWithOptionsTest()
        {
            var insert = new Dummy()
            {
                Foo = "asd",
                Bar = 10
            };

            var options = new AqlInsert.Options()
            {
                IgnoreErrors = true,
                Overwrite = OverwriteMode.Replace
            };

            AQuery query = _db.Query
                .Insert("collection", AValue.Bind(insert), options);

            Assert.AreEqual("INSERT @var0 INTO collection OPTIONS {ignoreErrors:true, overwrite:true, overwriteMode:\"replace\"}", query.GetExpression());
            Assert.AreEqual(insert, query.GetBindedVars()[0]);
        }

        [Test]
        public void UpsertTest()
        {
            var search = new Dummy()
            {
                Foo = "asd"
            };
            var insert = new Dummy()
            {
                Foo = "asd",
                Bar = 10
            };

            AQuery query = _db.Query
                .Upsert<Dummy>("collection", AValue.Bind(search), AValue.Bind(insert), builder => builder.Inc(x => x.Bar, AValue.Bind(1)));

            Assert.AreEqual("UPSERT @var0 INSERT @var1 UPDATE { Bar:x.Bar+@var2 } IN collection", query.GetExpression());
            Assert.AreEqual(search, query.GetBindedVars()[0]);
            Assert.AreEqual(insert, query.GetBindedVars()[1]);
            Assert.AreEqual(1, query.GetBindedVars()[2]);
        }

        [Test]
        public void ReturnTest()
        {
            AQuery query = _db.Query
                .Return(AReturn.Variable("x"));
            
            Assert.AreEqual("RETURN x", query.GetExpression());
        }
        
        [Test]
        public void ReturnPartialTest()
        {
            AQuery query = _db.Query
                .Return(AReturn.Partial<Dummy>(x => x.Key, x => x.Foo));
            
            Assert.AreEqual("RETURN {_key:x._key, Foo:x.Foo}", query.GetExpression());
        }
        
        
        [Test]
        public void ReturnPartial2Test()
        {
            AQuery query = _db.Query
                .Return(
                    AReturn.Partial<Dummy>(
                        Expression.Property(Expression.Parameter(typeof(Dummy), "x"), "Key"),
                        Expression.Property(Expression.Parameter(typeof(Dummy), "x"), "Foo")
                    )
                );
            
            Assert.AreEqual("RETURN {_key:x._key, Foo:x.Foo}", query.GetExpression());
        }
    }
}