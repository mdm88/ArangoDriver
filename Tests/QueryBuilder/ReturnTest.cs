using System.Threading.Tasks;
using Arango.Tests;
using ArangoDriver.Client;
using ArangoDriver.Client.Query;
using ArangoDriver.Client.Query.Return;
using ArangoDriver.Client.Query.Value;
using NUnit.Framework;

namespace Tests.QueryBuilder
{
    [TestFixture]
    public class ReturnTest : TestBase
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
            var returns = AReturn.Variable("Foo");
            
            AQuery query = _db.Query
                .Return(returns);

            Assert.AreEqual("RETURN Foo", query.GetExpression());
        }
        
        [Test]
        public void PartialTest()
        {
            var returns = AReturn.Partial<Dummy>(x => x.Bar, x => x.Foo);
            
            AQuery query = _db.Query
                .Return(returns);

            Assert.AreEqual("RETURN {Bar:x.Bar, Foo:x.Foo}", query.GetExpression());
        }
        
        [Test]
        public void ManualTest()
        {
            var returns = AReturn.Manual(("lala", new AqlReturnFull("Foo")));
            
            AQuery query = _db.Query
                .Return(returns);

            Assert.AreEqual("RETURN {lala:Foo}", query.GetExpression());
        }
    }
}