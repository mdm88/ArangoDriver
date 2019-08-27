# Basic usage

- [Connection management](#connection-management)
- [Checking and removing existing connections](#checking-and-removing-existing-connections)
- [Database context](#database-context)
- [AResult object](#aresult-object)
- [AError object](#aerror-object)
- [JSON representation](#json-representation)
- [Serialization options](#serialization-options)
- [Document ID, key and revision](#document-id-key-and-revision)
- [Fluent API](#fluent-api)

Driver's public API with its core functionality is exposed through `Arango.Client` namespace. Most of the classes which perform operations on ArangoDB database starts with `A` prefix (e.g. `ADatabase`, `ACollection`, `AQuery`, ...) to reduce the visual noise of superfluous naming.

## Connection management

Before the connection is initiated, driver needs to know server host, port and credentials (if needed). 

```csharp
// adds new connection data to database manager
var connection = new AConnection(
    "127.0.0.1",
    8529,
    false,
    "usr",
    "pwd"
);
```

## Database context

With previously set connection you can get access to a specific database and perform desired operations. 
There is no need to dispose `ADatabase` instance in order to free database connection because operations are performed through HTTP calls.

```csharp
// initialize new database context
var db = connection.GetDatabase("myDatabase");

// retrieve specified document
var getResult = db.Document.Get("myCollection/123");
```

## AResult object

Once the operation is executed, returned data are contained within `AResult` object which consists of following properties:

- `Success` - Determines whether the operation ended with success and returned result value is other than null.
- `StatusCode` - Integer value of the operation response HTTP status code.
- `HasValue` - Determines if the operation contains value other than null.
- `Value` - Generic object which type and value depends on performed operation.
- `Error` - If operation ended with failure, this property would contain instance of `AError` object which contains further information about the error.
- `Extra` - Document which might contain additional information on performed operation.

## AError object

In case of operation failure driver doesn't throw exceptions explicitely, but `AResult` object `Error` property would contain instance of `AError` object with following properties:

- `StatusCode` - Integer value of the operation response HTTP status code.
- `Number` - Integer value indicating [ArangoDB internal error code](https://docs.arangodb.com/ErrorCodes/index.html).
- `Message` - String value containing error description.
- `Exception` - Exception object with further information about failure.

## JSON representation

JSON objects are by default represented as `Dictionary<string, object>`. In order to simplify the usage of dictionaries (aka documents), driver comes equipped with embedded [dictator library](https://github.com/yojimbo87/dictator) which provide helpful set of methods to provide easier way to handle data stored in these objects. Dictator also provides methods for [conversion](https://github.com/yojimbo87/dictator#convert-document-to-strongly-typed-object) of documents to generic objects and [vice versa](https://github.com/yojimbo87/dictator#convert-strongly-typed-object-to-document). Custom classes can also take advantage of several [property attributes](https://github.com/yojimbo87/dictator#property-attributes).

Internal serialization and deserialization of JSON documents is done by embedded [fastJSON library](https://github.com/mgholam/fastJSON) which functionality is accessible through `Arango.fastJSON` namespace.

## Serialization options

`ASettings.JsonParameters` static property can be used to set custom serialization options which are provided by [fastJSON](https://github.com/mgholam/fastJSON) library. By default all options are set to their default values except `UseEscapedUnicode` and `UseFastGuid` which are both set to false value. Advantage of this approach is the ability to, for example, explicitely set `UseValuesOfEnums` to true value which will result in Enum type fields being stored as integers instead of strings.

## Document ID, key and revision

Apart from standard dictionary extensions provided by [dictator](https://github.com/yojimbo87/dictator), there are also following ArangoDB specific extension methods which can be used by `Dictionary<string, object>` instances to work with ArangoDB standard ID, key and revision document fields:

- `HasID()` - Checks if `_id` field is present and has valid format.
- `ID()` - Retrieves value of `_id` field. If the field is missing or has invalid format null value is returned.
- `ID(string id)` - Stores `_id` field value.
- `HasKey()` - Checks if `_key` field is present and has valid format.
- `Key()` - Retrieves value of `_key` field. If the field is missing or has invalid format null value is returned.
- `Key(string key)` - Stores `_key` field value.
- `HasRev()` - Checks if `_rev` field is present and has valid format.
- `Rev()` - Retrieves value of `_rev` field. If the field is missing or has invalid format null value is returned.
- `Rev(string rev)` - Stores `_rev` field value.
- `IsID(string fieldPath)` - Checks if specified field path has valid document ID value in the format of `collection/key`.
- `IsKey(string fieldPath)` - Checks if specified field path has valid document key value.

`ADocument` class provides several static methods for format validation of [ID](https://docs.arangodb.com/Glossary/index.html#document_handle), [key](https://docs.arangodb.com/NamingConventions/DocumentKeys.html) and [revision](https://docs.arangodb.com/Glossary/index.html#document_revision) values: 

- `ADocument.IsID(string id)` - Determines if specified value has valid document `_id` format. 
- `ADocument.IsKey(string key)` - Determines if specified value has valid document `_key` format.
- `ADocument.IsRev(string id)` - Determines if specified value has valid document `_rev` format.
- `ADocument.Identify(string collection, long key)` - Constructs document ID from specified collection and key values.
- `ADocument.Identify(string collection, string key)` - Constructs document ID from specified collection and key values. If key format is invalid null value is returned.
- `ADocument.ParseKey(string id)` - Parses key value out of specified document ID. If ID has invalid value null is returned. 

## Fluent API

Driver is heavily using [fluent API](http://en.wikipedia.org/wiki/Fluent_interface) which provides extensive flexiblity to the way how various operations can be executed. Instead of having multiple overloaded methods with bloated set of parameters which can be assigned to given operation, fluent API gives developers ability to apply specific parameter only when needed without the need to obey method signature.

```csharp
// initialize new database context
var db = connection.GetDatabase("myDatabase");
// operation core
var queryOperation = db.Query
    .Aql("FOR item IN myCollection RETURN item");

// add optional parameter
if (... condition whether to use count query parameter ...)
{
    queryOperation.Count(true);
}

// add another optional parameter
if (... condition whether to use batch size query parameter ...)
{
    queryOperation.BatchSize(1);
}

// execute query operation
var queryResult1 = queryOperation.ToList();

// more concise example of query operation
var queryResult2 = db.Query
    .Count(true)
    .BatchSize(1)
    .Aql("FOR item IN myCollection RETURN item")
    .ToList();
```