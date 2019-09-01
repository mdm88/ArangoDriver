namespace ArangoDriver.Serialization
{
    internal interface IJsonSerializer
    {
        string Serialize<T>(T obj);
        T Deserialize<T>(string json);
    }
}