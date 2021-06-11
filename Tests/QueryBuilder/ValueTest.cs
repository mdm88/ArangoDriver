using System.Collections.Generic;
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
            
            Assert.AreEqual("LET x = x.Bar", query.GetExpression());
        }
        
        [Test]
        public void IfTest()
        {
            AQuery query = _db.Query
                .Let( "x", AValue.If(
                    AFilter.Eq(AValue<Dummy>.Field(x => x.Bar), AValue.Bind(2)), 
                    AValue<Dummy>.Field(x => x.Baz),
                    AValue.Bind(0)
                ));
            
            Assert.AreEqual("LET x = (x.Bar == @var0 ? x.Baz : @var1)", query.GetExpression());
            Assert.AreEqual(2, query.GetBindedVars()[0]);
            Assert.AreEqual(0, query.GetBindedVars()[1]);
        }
        
        [Test]
        public void FieldSuffixTest()
        {
            AQuery query = _db.Query
                .Let( "x", AValue<Dummy>.Field<int>(x => x.Foo, ".was"));
            
            Assert.AreEqual("LET x = x.Foo.was", query.GetExpression());
        }
        
        [Test]
        public void AttributesTest()
        {
            AQuery query = _db.Query
                .Let( "x", AValue.Attributes(AValue<Dummy>.Field(x => x.Foo)));
            
            Assert.AreEqual("LET x = ATTRIBUTES(x.Foo)", query.GetExpression());
        }
        
        [Test]
        public void OperationTest()
        {
            AQuery query = _db.Query
                .Let( "x", AValue.Op(AValue<Dummy>.Field(x => x.Baz), "/", AValue<Dummy>.Field(x => x.Bar)));
            
            Assert.AreEqual("LET x = (x.Baz / x.Bar)", query.GetExpression());
        }
        
        [Test]
        public void FunctionSingleTest()
        {
            AQuery query = _db.Query
                .Let( "x", ANumeric.Min(AValueArray.Bind(2, 5, 6)));
            
            Assert.AreEqual("LET x = MIN([@var0,@var1,@var2])", query.GetExpression());
            Assert.AreEqual(2, query.GetBindedVars()[0]);
            Assert.AreEqual(5, query.GetBindedVars()[1]);
            Assert.AreEqual(6, query.GetBindedVars()[2]);
        }
        
        [Test]
        public void FunctionMultipleTest()
        {
            AQuery query = _db.Query
                .Let( "x", ANumeric.Coalesce(AValue.Bind(2), AValue.Bind(5)));
            
            Assert.AreEqual("LET x = NOT_NULL(@var0,@var1)", query.GetExpression());
            Assert.AreEqual(2, query.GetBindedVars()[0]);
            Assert.AreEqual(5, query.GetBindedVars()[1]);
        }
        
        [Test]
        public void ArrayFunctionTest()
        {
            AQuery query = _db.Query
                .Let( "x", AArray.Minus(
                    AValue.Bind(new List<int>() {2, 5, 6}),
                    AValue.Bind(new List<int>() {4, 5, 6})
                ));
            
            Assert.AreEqual("LET x = MINUS(@var0,@var1)", query.GetExpression());
            Assert.AreEqual(typeof(List<int>), query.GetBindedVars()[0].GetType());
            Assert.AreEqual(typeof(List<int>), query.GetBindedVars()[1].GetType());
        }
        
        [Test]
        public void RangeTest()
        {
            AQuery query = _db.Query
                .For("x", AValue.Range(0, 200));
            
            Assert.AreEqual("FOR x IN 0..200", query.GetExpression());
        }
        
        [Test]
        public void ObjectTest()
        {
            AQuery query = _db.Query
                .Let( "x", AValue<Dummy>.Object(
                    (
                        x => x.Id,
                        AValue.Bind("asdf")
                    ),
                    (
                        x => x.Foo,
                        AValue<Dummy>.Field(z => z.Foo)
                    ),
                    (
                        x => x.Bar,
                        AValue.Bind(10)
                    ),
                    (
                        x => x.Baz,
                        AValue<Dummy>.Field(x => x.Bar)
                    )
                ));
            
            Assert.AreEqual("LET x = {\"_id\":@var0,\"Foo\":z.Foo,\"Bar\":@var1,\"Baz\":x.Bar}", query.GetExpression());
            Assert.AreEqual(typeof(string), query.GetBindedVars()[0].GetType());
            Assert.AreEqual(typeof(int), query.GetBindedVars()[1].GetType());
        }
        
        [Test]
        public void ObjectDynamicTest()
        {
            AQuery query = _db.Query
                .Let( "x", AValue.Object(
                    (
                        "_id",
                        AValue.Bind("asdf")
                    ),
                    (
                        "Foo",
                        AValue<Dummy>.Field(z => z.Foo)
                    ),
                    (
                        "lalalal",
                        AValue.Bind(10)
                    )
                ));
            
            Assert.AreEqual("LET x = {\"_id\":@var0,\"Foo\":z.Foo,\"lalalal\":@var1}", query.GetExpression());
            Assert.AreEqual(typeof(string), query.GetBindedVars()[0].GetType());
            Assert.AreEqual(typeof(int), query.GetBindedVars()[1].GetType());
        }
    }
}