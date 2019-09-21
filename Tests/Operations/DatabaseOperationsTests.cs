using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Arango.Tests;
using ArangoDriver.Client;
using ArangoDriver.Exceptions;
using NUnit.Framework;

namespace Tests.Operations
{
    public class DatabaseOperationsTests : TestBase
    {   
        [Test]
        public async Task Should_get_list_of_accessible_databases()
        {
            await Connection.CreateDatabase(TestDatabaseOneTime);

            var resultList = await Connection.GetAccessibleDatabases();

            Assert.AreEqual(200, resultList.StatusCode);
            Assert.IsTrue(resultList.Success);
            Assert.IsTrue(resultList.HasValue);
            Assert.IsTrue(resultList.Value.Contains(TestDatabaseOneTime));
        }
        
        [Test]
        public async Task Should_get_list_of_all_databases()
        {
            await Connection.CreateDatabase(TestDatabaseOneTime);

            var resultList = await Connection.GetAllDatabases();

            Assert.AreEqual(200, resultList.StatusCode);
            Assert.IsTrue(resultList.Success);
            Assert.IsTrue(resultList.HasValue);
            Assert.IsTrue(resultList.Value.Contains(TestDatabaseOneTime));
            Assert.IsTrue(resultList.Value.Contains("_system"));
        }
        
        [Test]
        public async Task Should_get_current_database()
        {
            await Connection.CreateDatabase(TestDatabaseOneTime);

            var resultCurrent = await Connection.GetDatabase(TestDatabaseOneTime).GetCurrent();

            Assert.AreEqual(200, resultCurrent.StatusCode);
            Assert.IsTrue(resultCurrent.Success);
            Assert.IsTrue(resultCurrent.HasValue);
            Assert.AreEqual(TestDatabaseOneTime, resultCurrent.Value["name"]);
            Assert.AreEqual(false, string.IsNullOrEmpty((string) resultCurrent.Value["id"]));
            Assert.AreEqual(false, string.IsNullOrEmpty((string) resultCurrent.Value["path"]));
            Assert.AreEqual(false, resultCurrent.Value["isSystem"]);
        }
        
        [Test]
        public async Task Should_get_database_collections()
        {
            await Connection.CreateDatabase(TestDatabaseOneTime);

            var db = Connection.GetDatabase(TestDatabaseOneTime);

            var createResult = await db.CreateCollection(TestDocumentCollectionName).Create();

            var getResult = await db
                //.ExcludeSystem(true)
                .GetAllCollections();
            
            Assert.AreEqual(200, getResult.StatusCode);
            Assert.IsTrue(getResult.Success);
            Assert.IsTrue(getResult.HasValue);
            
            var foundCreatedCollection = getResult.Value.FirstOrDefault(col => (string) col["name"] == (string) createResult.Value["name"]);
            
            Assert.IsNotNull(foundCreatedCollection);
            
            var foundSystemCollection = getResult.Value.FirstOrDefault(col => (string) col["name"] == "_system");
            
            Assert.IsNull(foundSystemCollection);
        }
        
        [Test]
        public async Task Should_create_database()
        {
            var createResult = await Connection.CreateDatabase(TestDatabaseOneTime);

            Assert.AreEqual(201, createResult.StatusCode);
            Assert.IsTrue(createResult.Success);
            Assert.IsTrue(createResult.HasValue);
            Assert.IsTrue(createResult.Value);
        }
        
        [Test]
        public async Task Should_create_database_with_users()
        {
            var users = new List<AUser>()
            {
                new AUser { Username = "admin", Password = "secret", Active = true },
                new AUser { Username = "tester001", Password = "test001", Active = false } 
            };
            
            var createResult = await Connection.CreateDatabase(TestDatabaseOneTime, users);

            Assert.AreEqual(201, createResult.StatusCode);
            Assert.IsTrue(createResult.Success);
            Assert.IsTrue(createResult.HasValue);
            Assert.IsTrue(createResult.Value);
        }
        
        [Test]
        public async Task Should_fail_create_already_existing_database()
        {
            await Connection.CreateDatabase(TestDatabaseGeneral);

            Assert.ThrowsAsync<DatabaseAlreadyExistsException>(() =>
            {
                return Connection.CreateDatabase(TestDatabaseGeneral);
            });
        }
        
        [Test]
        public async Task Should_delete_database()
        {
            await Connection.CreateDatabase(TestDatabaseOneTime);
            
            var deleteResult = await Connection.DropDatabase(TestDatabaseOneTime);

            Assert.AreEqual(200, deleteResult.StatusCode);
            Assert.IsTrue(deleteResult.Success);
            Assert.IsTrue(deleteResult.HasValue);
            Assert.IsTrue(deleteResult.Value);
        }
    }
}
