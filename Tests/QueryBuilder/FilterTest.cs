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
            AQuery query = _db.Query
                .Aql("FOR x IN " + TestDocumentCollectionName)
                .Filter(FilterBuilder<Dummy>.Eq( x => x.Foo, "asd"));
            
            Assert.AreEqual("FOR x IN " + TestDocumentCollectionName + " FILTER x.Foo = @var0", query.Query);
            Assert.AreEqual("asd", query.BindVars["var0"]);
        }
    }
}