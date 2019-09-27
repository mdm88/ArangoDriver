using System;
using System.Threading.Tasks;
using Arango.Tests;
using ArangoDriver.Client;
using ArangoDriver.Client.Query;
using ArangoDriver.Client.Query.Filter;
using ArangoDriver.Client.Query.Value;
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
            //AQuery query = _db.Query.Filter(FilterBuilder<Dummy>.Eq( x => x.Foo, "asd"));
            AQuery query = _db.Query.Filter(AFilter.Eq(AValue<Dummy>.Field(x => x.Foo), AValue.Bind("asd")));
            
            Assert.AreEqual("FILTER x.Foo == @var0", query.Query);
            Assert.AreEqual("asd", query.BindVars["var0"]);
        }
        
        [Test]
        public void MultipleTest()
        {
            AQuery query = _db.Query
                //.Filter(FilterBuilder<Dummy>.Lt(x => x.Baz, 50))
                //.Filter(FilterBuilder<Dummy>.Gte(x => x.Bar, 1));
                .Filter(AFilter.Lt(AValue<Dummy>.Field(x => x.Baz), AValue.Bind(50)))
                .Filter(AFilter.Gte(AValue<Dummy>.Field(x => x.Bar), AValue.Bind(1)));
            
            Assert.AreEqual("FILTER x.Baz < @var0 FILTER x.Bar >= @var1", query.Query);
            Assert.AreEqual(50, query.BindVars["var0"]);
            Assert.AreEqual(1, query.BindVars["var1"]);
        }
        
        [Test]
        public void TypeTest()
        {
            Type dummyType = typeof(Dummy);
            
            //AQuery query = _db.Query.Filter(FilterBuilder<Dummy>.Eq(x => x.GetType(), dummyType));
            AQuery query = _db.Query.Filter(AFilter.Eq(AValue<Dummy>.Field(x => x.GetType()), AValue.Bind(dummyType)));
            
            Assert.AreEqual("FILTER x.$type == @var0", query.Query);
            Assert.AreEqual(dummyType.FullName + ", "  + dummyType.Assembly.GetName().Name, query.BindVars["var0"]);
        }
        
        [Test]
        public void InTest()
        {
            //AQuery query = _db.Query.Filter(FilterBuilder<Dummy>.In(x => x.Foo, new[] {"asd", "qwe"}));
            AQuery query = _db.Query.Filter(AFilter.In(AValue<Dummy>.Field(x => x.Foo), AValueArray.Bind("asd", "qwe")));
            
            Assert.AreEqual("FILTER x.Foo IN [@var0,@var1]", query.Query);
            Assert.AreEqual("asd", query.BindVars["var0"]);
            Assert.AreEqual("qwe", query.BindVars["var1"]);
        }
        
        [Test]
        public void OrTest()
        {
            /*AQuery query = _db.Query.Filter(FilterBuilder<Dummy>.Or(
                FilterBuilder<Dummy>.Eq(x => x.Foo, "asd"), 
                FilterBuilder<Dummy>.Gt(x => x.Bar, 1))
            );*/
            AQuery query = _db.Query.Filter(
                AFilter.Or(
                    AFilter.Eq(AValue<Dummy>.Field(x => x.Foo), AValue.Bind("asd")), 
                    AFilter.Gt(AValue<Dummy>.Field(x => x.Bar), AValue.Bind(1))
                )
            );
            
            Assert.AreEqual("FILTER (x.Foo == @var0 OR x.Bar > @var1)", query.Query);
            Assert.AreEqual("asd", query.BindVars["var0"]);
            Assert.AreEqual(1, query.BindVars["var1"]);
        }
    }
}