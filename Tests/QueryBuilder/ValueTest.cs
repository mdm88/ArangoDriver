using System.Linq.Expressions;
using System.Threading.Tasks;
using Arango.Tests;
using ArangoDriver.Client;
using ArangoDriver.Client.Query;
using ArangoDriver.Client.Query.Value;
using NUnit.Framework;

namespace Tests.QueryBuilder
{
    [TestFixture]
    public class ValueTest : TestBase
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
        public void FieldTest()
        {
            AQuery query = _db.Query
                .Let( "x", AValue<Dummy>.Field(x => x.Bar));
            
            Assert.AreEqual("LET x = x.Bar", query.Query);
        }
        
        [Test]
        public void FieldSuffixTest()
        {
            AQuery query = _db.Query
                .Let( "x", AValue<Dummy>.Field<int>(x => x.Foo, ".was"));
            
            Assert.AreEqual("LET x = x.Foo.was", query.Query);
        }
        
        [Test]
        public void AttributesTest()
        {
            AQuery query = _db.Query
                .Let( "x", AValue.Attributes(AValue<Dummy>.Field(x => x.Foo)));
            
            Assert.AreEqual("LET x = ATTRIBUTES(x.Foo)", query.Query);
        }
        
        [Test]
        public void OperationTest()
        {
            AQuery query = _db.Query
                .Let( "x", AValue.Op(AValue<Dummy>.Field(x => x.Baz), "/", AValue<Dummy>.Field(x => x.Bar)));
            
            Assert.AreEqual("LET x = (x.Baz / x.Bar)", query.Query);
        }
        
        [Test]
        public void FunctionSingleTest()
        {
            AQuery query = _db.Query
                .Let( "x", ANumeric.Min(AValueArray.Bind(2, 5, 6)));
            
            Assert.AreEqual("LET x = MIN([@var0,@var1,@var2])", query.Query);
            Assert.AreEqual(2, query.BindVars["var0"]);
            Assert.AreEqual(5, query.BindVars["var1"]);
            Assert.AreEqual(6, query.BindVars["var2"]);
        }
        
        [Test]
        public void FunctionMultipleTest()
        {
            AQuery query = _db.Query
                .Let( "x", ANumeric.Coalesce(AValue.Bind(2), AValue.Bind(5)));
            
            Assert.AreEqual("LET x = NOT_NULL(@var0,@var1)", query.Query);
            Assert.AreEqual(2, query.BindVars["var0"]);
            Assert.AreEqual(5, query.BindVars["var1"]);
        }
    }
}