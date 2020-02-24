using System.Threading.Tasks;
using Arango.Tests;
using ArangoDriver.Client;
using ArangoDriver.Client.Query;
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
            UpdateDefinition<Dummy> upd = UpdateBuilder<Dummy>.Update("x", TestDocumentCollectionName)
                .Set(x => x.Foo, "asdf");
            
            AQuery query = _db.Query
                .Raw("FOR x IN " + TestDocumentCollectionName)
                .Update(upd);

            Assert.AreEqual("FOR x IN " + TestDocumentCollectionName + " UPDATE x WITH { Foo: @var0 } IN " + TestDocumentCollectionName, query.GetExpression());
            Assert.AreEqual("asdf", query.GetBindedVars()[0]);
        }
        
        [Test]
        public void MultipleTest()
        {
            UpdateDefinition<Dummy> upd = UpdateBuilder<Dummy>.Update("x", TestDocumentCollectionName)
                .Set(x => x.Foo, "asdf")
                .Set(x => x.Id, "1235");
            
            AQuery query = _db.Query
                .Raw("FOR x IN " + TestDocumentCollectionName)
                .Update(upd);

            Assert.AreEqual("FOR x IN " + TestDocumentCollectionName + " UPDATE x WITH { Foo: @var0, _id: @var1 } IN " + TestDocumentCollectionName, query.GetExpression());
            Assert.AreEqual("asdf", query.GetBindedVars()[0]);
            Assert.AreEqual("1235", query.GetBindedVars()[1]);
        }
        
        [Test]
        public void IncTest()
        {
            UpdateDefinition<Dummy> upd = UpdateBuilder<Dummy>.Update("x", TestDocumentCollectionName)
                .Inc(x => x.Bar, 38);
            
            AQuery query = _db.Query
                .Raw("FOR x IN " + TestDocumentCollectionName)
                .Update(upd);

            Assert.AreEqual("FOR x IN " + TestDocumentCollectionName + " UPDATE x WITH { Bar: x.Bar+38 } IN " + TestDocumentCollectionName, query.GetExpression());
            Assert.IsEmpty(query.GetBindedVars());
        }
        
        [Test]
        public void IncNegativeTest()
        {
            UpdateDefinition<Dummy> upd = UpdateBuilder<Dummy>.Update("x", TestDocumentCollectionName)
                .Inc(x => x.Bar, -5);
            
            AQuery query = _db.Query
                .Raw("FOR x IN " + TestDocumentCollectionName)
                .Update(upd);

            Assert.AreEqual("FOR x IN " + TestDocumentCollectionName + " UPDATE x WITH { Bar: x.Bar-5 } IN " + TestDocumentCollectionName, query.GetExpression());
            Assert.IsEmpty(query.GetBindedVars());
        }
        
        [Test]
        public void OptionsTest()
        {
            UpdateDefinition<Dummy> upd = UpdateBuilder<Dummy>.Update("x", TestDocumentCollectionName)
                .Set(x => x.Foo, "asdf")
                .OptMergeObjects(false);
            
            AQuery query = _db.Query
                .Raw("FOR x IN " + TestDocumentCollectionName)
                .Update(upd);

            Assert.AreEqual("FOR x IN " + TestDocumentCollectionName + " UPDATE x WITH { Foo: @var0 } IN " + TestDocumentCollectionName + " OPTIONS {mergeObjects:false}", query.GetExpression());
            Assert.AreEqual("asdf", query.GetBindedVars()[0]);
        }
    }
}