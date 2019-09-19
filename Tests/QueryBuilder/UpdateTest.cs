using System.Threading.Tasks;
using Arango.Tests;
using ArangoDriver.Client;
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
            UpdateDefinition<Dummy> upd = UpdateBuilder.Update<Dummy>("x", TestDocumentCollectionName)
                .Set(x => x.Foo, "asdf")
                .Set(x => x.Id, "1235");
            
            AQuery query = _db.Query
                .Aql("FOR x IN " + TestDocumentCollectionName)
                .Update(upd);

            Assert.AreEqual("FOR x IN " + TestDocumentCollectionName + " UPDATE x WITH { Foo: @set0, _id: @set1 } IN " + TestDocumentCollectionName, query.Query);
            Assert.AreEqual("asdf", query.BindVars["set0"]);
            Assert.AreEqual("1235", query.BindVars["set1"]);
        }
    }
}