using System.Threading.Tasks;
using Arango.Tests;
using ArangoDriver.Client;
using ArangoDriver.Client.Query;
using ArangoDriver.Client.Query.Update;
using ArangoDriver.Client.Query.Value;
using NUnit.Framework;

namespace Tests.QueryBuilder
{
    [TestFixture]
    public class UpdateTest : TestBase
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
            AQuery query = _db.Query
                .Raw("FOR x IN " + TestDocumentCollectionName)
                .Update<Dummy>(
                    "x", TestDocumentCollectionName,
                    builder => builder
                        .Set(x => x.Foo, AValue.Bind("asdf"))
                );

            Assert.AreEqual("FOR x IN " + TestDocumentCollectionName + " UPDATE x WITH { Foo:@var0 } IN " + TestDocumentCollectionName, query.GetExpression());
            Assert.AreEqual("asdf", query.GetBindedVars()[0]);
        }
        
        [Test]
        public void MultipleTest()
        {
            AQuery query = _db.Query
                .Raw("FOR x IN " + TestDocumentCollectionName)
                .Update<Dummy>(
                    "x", TestDocumentCollectionName, 
                    builder => builder
                        .Set(x => x.Foo, AValue.Bind("asdf"))
                        .Set(x => x.Id, AValue.Bind("1235"))
                );

            Assert.AreEqual("FOR x IN " + TestDocumentCollectionName + " UPDATE x WITH { Foo:@var0, _id:@var1 } IN " + TestDocumentCollectionName, query.GetExpression());
            Assert.AreEqual("asdf", query.GetBindedVars()[0]);
            Assert.AreEqual("1235", query.GetBindedVars()[1]);
        }
        
        [Test]
        public void IncTest()
        {
            AQuery query = _db.Query
                .Raw("FOR x IN " + TestDocumentCollectionName)
                .Update<Dummy>(
                    "x", TestDocumentCollectionName, 
                    builder => builder
                        .Inc(x => x.Bar, AValue.Bind(38))
                );

            Assert.AreEqual("FOR x IN " + TestDocumentCollectionName + " UPDATE x WITH { Bar:x.Bar+@var0 } IN " + TestDocumentCollectionName, query.GetExpression());
            Assert.AreEqual(38, query.GetBindedVars()[0]);
        }
        
        [Test]
        public void SubElementTest()
        {
            AQuery query = _db.Query
                .Raw("FOR x IN " + TestDocumentCollectionName)
                .Update<Complex>(
                    "x", TestDocumentCollectionName,
                    builderComplex => builderComplex
                        .SetPartial(
                            x => x.dum,
                            builderDummy => builderDummy
                                .Set(x => x.Bar, AValue.Bind(38))
                        )
                );

            Assert.AreEqual("FOR x IN " + TestDocumentCollectionName + " UPDATE x WITH { dum:MERGE(x.dum,{ Bar:@var0 }) } IN " + TestDocumentCollectionName, query.GetExpression());
            Assert.AreEqual(38, query.GetBindedVars()[0]);
        }
        
        [Test]
        public void SubElementIncTest()
        {
            AQuery query = _db.Query
                .Raw("FOR x IN " + TestDocumentCollectionName)
                .Update<Complex>(
                    "x", TestDocumentCollectionName,
                    builderComplex => builderComplex
                        .SetPartial(
                            x => x.dum,
                            builderDummy => builderDummy
                                .Inc(x => x.Bar, AValue.Bind(38))
                        )
                );

            Assert.AreEqual("FOR x IN " + TestDocumentCollectionName + " UPDATE x WITH { dum:MERGE(x.dum,{ Bar:x.dum.Bar+@var0 }) } IN " + TestDocumentCollectionName, query.GetExpression());
            Assert.AreEqual(38, query.GetBindedVars()[0]);
        }
        
        [Test]
        public void PredefinedTest()
        {
            UpdateBuilder<Dummy> definition = new UpdateBuilder<Dummy>()
                .Set(x => x.Foo, AValue.Bind("asdf"));
            
            AQuery query = _db.Query
                .Raw("FOR x IN " + TestDocumentCollectionName)
                .Update<Dummy>(
                    "x", TestDocumentCollectionName,
                    definition
                );

            Assert.AreEqual("FOR x IN " + TestDocumentCollectionName + " UPDATE x WITH { Foo:@var0 } IN " + TestDocumentCollectionName, query.GetExpression());
            Assert.AreEqual("asdf", query.GetBindedVars()[0]);
        }
        
        [Test]
        public void OptionsTest()
        {
            AQuery query = _db.Query
                .Raw("FOR x IN " + TestDocumentCollectionName)
                .Update<Dummy>(
                    "x", TestDocumentCollectionName,
                    builder => builder
                        .Set(x => x.Foo, AValue.Bind("asdf")),
                    false
                );

            Assert.AreEqual("FOR x IN " + TestDocumentCollectionName + " UPDATE x WITH { Foo:@var0 } IN " + TestDocumentCollectionName + " OPTIONS {mergeObjects:false}", query.GetExpression());
            Assert.AreEqual("asdf", query.GetBindedVars()[0]);
        }
    }
}