using System.IO;

namespace ArangoDriver.Serialization
{
    public interface IJsonSerializer
    {
        string Serialize<T>(T obj);
        T Deserialize<T>(string json);
        T Deserialize<T>(Stream json);
    }
}