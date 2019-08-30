using System.Collections.Generic;
using ArangoDriver.Client;
using Microsoft.Extensions.Configuration;
using NUnit.Framework;

namespace Arango.Tests
{
    public class TestBase
    {
	    public AConnection Connection;
	    
	    [SetUp]
	    public void PreSetup()
	    {
		    IConfigurationBuilder configurationBuilder = new ConfigurationBuilder();
		    configurationBuilder.AddJsonFile("appsettings.json");
		    IConfiguration configuration = configurationBuilder.Build();
		    
		    Connection = new AConnection(
			    configuration["Connection:Host"],
			    int.Parse(configuration["Connection:Port"]),
			    false,
			    configuration["Connection:User"],
			    configuration["Connection:Password"]
		    );
	    }
	    
	    //private static AConnection _connection;
	    /*public static AConnection Connection
	    {
		    get
		    {
			    if (_connection == null)
			    {
				    _connection = new AConnection(
					    "mmorts-arangodb",
					    8529,
					    false,
					    "root",
					    "MENDIpro88tren"
				    );
			    }

			    return _connection;
		    }
	    }*/

	    public static string TestDatabaseOneTime { get; } = "testOneTimeDatabase";
	    public static string TestDatabaseGeneral { get; } = "testDatabaseGeneral";

	    public static string TestDocumentCollectionName { get; } = "testDocumentCollection";
	    public static string TestEdgeCollectionName { get; } = "testEdgeCollection";

        public static string Alias { get; set; }
        public static string SystemAlias { get; set; }

        public static string Hostname { get; set; }
        public static int Port { get; set; }
        public static bool IsSecured { get; set; }
        public static string UserName { get; set; }
        public static string Password { get; set; }
        
        /*static Database()
        {
            TestDatabaseOneTime = "testOneTimeDatabase001xyzLatif";
            TestDatabaseGeneral = "testDatabaseGeneral001xyzLatif";

            TestDocumentCollectionName = "testDocumentCollection001xyzLatif";
            TestEdgeCollectionName = "testEdgeCollection001xyzLatif";
            
            Alias = "testAlias";
            SystemAlias = "systemAlias";
            Hostname = "localhost";
            Port = 8529;
            IsSecured = false;
            UserName = "";
            Password = "";

            ASettings.AddConnection(
                SystemAlias,
                Hostname,
                Port,
                IsSecured,
                "_system",
                UserName,
                Password
            );

            ASettings.AddConnection(
                Alias,
                Hostname,
                Port,
                IsSecured,
                TestDatabaseGeneral,
                UserName,
                Password
            );
        }
        
        public static void CreateTestCollection(string collectionName, ACollectionType collectionType)
        {
        	DeleteTestCollection(collectionName);
        	
            var db = new ADatabase(Database.Alias);

            var createResult = db.Collection
                .Type(collectionType)
                .Create(collectionName);
        }
        
        public static void ClearTestCollection(string collectionName)
        {
            var db = new ADatabase(Database.Alias);

            var createResult = db.Collection
                .Truncate(collectionName);
        }
        
        public static List<Dictionary<string, object>> ClearCollectionAndFetchTestDocumentData(string collectionName)
        {
            ClearTestCollection(collectionName);
            
            var documents = new List<Dictionary<string, object>>();
        	var db = new ADatabase(Alias);
        	
        	var document1 = new Dictionary<string, object>()
        		.String("foo", "string value one")
        		.Int("bar", 1);
        	
        	var document2 = new Dictionary<string, object>()
        		.String("foo", "string value two")
        		.Int("bar", 2);
        	
        	var createResult1 = db.Document.Create(TestDocumentCollectionName, document1);
        	
        	document1.Merge(createResult1.Value);
        	
        	var createResult2 = db.Document.Create(TestDocumentCollectionName, document2);
        	
        	document2.Merge(createResult2.Value);
        	
        	documents.Add(document1);
        	documents.Add(document2);
        	
        	return documents;
        }

        public static void DeleteTestCollection(string collectionName)
        {
            var db = new ADatabase(Database.Alias);

            var resultGet = db.Collection.Get(collectionName);
            
            if (resultGet.Success && (resultGet.Value.String("name") == collectionName))
            {
                db.Collection.Delete(collectionName);
            }
        }
        */
    }
}
