# Database operations

- [Create database](#create-database)
- [Retrieve current database](#retrieve-current-database)
- [Retrieve accessible databases](#retrieve-accessible-databases)
- [Retrieve all databases](#retrieve-all-databases)
- [Retrieve database collections](#retrieve-database-collections)
- [Delete database](#delete-database)
- [More examples](#more-examples)

Most operations which are focused on management of database instances can only be performed through connection set to default `_system` database.

## Create database

Creates new database with given name and optional user list.

```csharp
// creates new database
var createDatabaseResult1 = await connection.CreateDatabase("myDatabase1");

// creates another new database with specified users
var users = new List<AUser>()
{
    new AUser { Username = "admin", Password = "secret", Active = true },
    new AUser { Username = "tester001", Password = "test001", Active = false } 
};

var createDatabaseResult2 = connection.CreateDatabase("myDatabase2", users), 
```

## Retrieve current database

Retrieves information about currently connected database.

```csharp
var db = connection.GetDatabase("myDatabase");

var currentDatabaseResult = await db.GetCurrent();

if (currentDatabaseResult.Success)
{
    var name = currentDatabaseResult.Value.String("name");
    var id = currentDatabaseResult.Value.String("id");
    var path = currentDatabaseResult.Value.String("path");
    var isSystem = currentDatabaseResult.Value.Bool("isSystem");
}
```

## Retrieve accessible databases

Retrieves list of accessible databases which current user can access without specifying a different username or password.

```csharp
var accessibleDatabasesResult = await connection.GetAccessibleDatabases();

if (accessibleDatabasesResult.Success)
{
    foreach (var database in accessibleDatabasesResult.Value)
    {
        var name = database;
    }
}
```

## Retrieve all databases

Retrieves the list of all existing databases.

```csharp
var allDatabasesResult = await connection.GetAllDatabases();

if (allDatabasesResult.Success)
{
    foreach (var database in allDatabasesResult.Value)
    {
        var name = database;
    }
}
```

## Retrieve database collections

Retrieves information about collections in current database connection.

```csharp
var db = connection.GetDatabase("myDatabase");

var databaseCollectionsResult = await db.GetAllCollections();
    
if (databaseCollectionsResult.Success)
{
    foreach (var collection in databaseCollectionsResult.Value)
    {
        var name = collection.String("name");
    }
}
```

## Delete database

Deletes specified database.

```csharp
var deleteDatabaseResult = await connection.DropDatabase("myDatabase");

if (deleteDatabaseResult.Success)
{
    var isDeleted = deleteDatabaseResult.Value;
}
```