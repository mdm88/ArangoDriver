using System;
using System.Threading.Tasks;
using Arango.Tests;
using ArangoDriver.Client;
using NUnit.Framework;

namespace Tests.QueryBuilder
{
    [TestFixture]
    public class FilterTest : TestBase
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
        public void SimpleTest()
        {
            AQuery query = _db.Query.Filter(FilterBuilder<Dummy>.Eq( x => x.Foo, "asd"));
            
            Assert.AreEqual("FILTER x.Foo == @var0", query.Query);
            Assert.AreEqual("asd", query.BindVars["var0"]);
        }
        
        [Test]
        public void MultipleTest()
        {
            AQuery query = _db.Query
                .Filter(FilterBuilder<Dummy>.Lt(x => x.Baz, 50))
                .Filter(FilterBuilder<Dummy>.Gte(x => x.Bar, 1));
            
            Assert.AreEqual("FILTER x.Baz < @var0 FILTER x.Bar >= @var1", query.Query);
            Assert.AreEqual(50, query.BindVars["var0"]);
            Assert.AreEqual(1, query.BindVars["var1"]);
        }
        
        [Test]
        public void TypeTest()
        {
            Type dummyType = typeof(Dummy);
            
            AQuery query = _db.Query.Filter(FilterBuilder<Dummy>.Eq(x => x.GetType(), dummyType));
            
            Assert.AreEqual("FILTER x.$type == @var0", query.Query);
            Assert.AreEqual(dummyType.FullName + ", "  + dummyType.Assembly.GetName().Name, query.BindVars["var0"]);
        }
        
        [Test]
        public void OrTest()
        {
            var f1 = FilterBuilder<Dummy>.Eq(x => x.Foo, "asd");
            var f2 = FilterBuilder<Dummy>.Gt(x => x.Bar, 1);

            AQuery query = _db.Query.Filter(FilterBuilder<Dummy>.Or(new[] {f1, f2}));
            
            Assert.AreEqual("FILTER x.Foo == @var0 OR x.Bar > @var1", query.Query);
            Assert.AreEqual("asd", query.BindVars["var0"]);
            Assert.AreEqual(1, query.BindVars["var1"]);
        }
    }
}