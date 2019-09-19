using System.Threading.Tasks;
using Arango.Tests;
using ArangoDriver.Client;
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
                .For(TestDocumentCollectionName, "x");
            
            Assert.AreEqual("FOR x IN " + TestDocumentCollectionName, query.Query);
        }
        
        [Test]
        public void LimitTest()
        {
            AQuery query = _db.Query
                .Limit(5);
            
            Assert.AreEqual("LIMIT 5", query.Query);
        }
        
        [Test]
        public void ReturnTest()
        {
            AQuery query = _db.Query
                .Return("x");
            
            Assert.AreEqual("RETURN x", query.Query);
        }
    }
}