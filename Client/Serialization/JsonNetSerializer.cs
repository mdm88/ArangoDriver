using System;
using System.Collections.Generic;
using System.IO;
using ArangoDriver.Exceptions;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;

namespace ArangoDriver.Serialization
{
    public class JsonNetSerializer : IJsonSerializer
    {
        protected readonly JsonSerializer _serializer;

        public JsonNetSerializer()
        {
            _serializer = new JsonSerializer()
            {
                TypeNameHandling = TypeNameHandling.Auto,
                MetadataPropertyHandling = MetadataPropertyHandling.ReadAhead,
                ContractResolver = JsonNetContractResolver.Instance
            };
        }

        public string Serialize<T>(T obj)
        {
            using StringWriter writer = new StringWriter();
            using JsonTextWriter jsonWriter = new JsonTextWriter(writer);

            _serializer.Serialize(jsonWriter, obj, typeof(T));
            jsonWriter.Flush();

            return writer.ToString();
        }

        public T Deserialize<T>(string json)
        {
            try
            {
                using StringReader sr = new StringReader(json);
                using JsonReader reader = new JsonTextReader(sr);

                return _serializer.Deserialize<T>(reader);
            }
            catch (Exception e)
            {
                throw new ArangoException(e.Message);
            }
        }

        public T Deserialize<T>(Stream stream)
        {
            try
            {
                using StreamReader sr = new StreamReader(stream);
                using JsonReader reader = new JsonTextReader(sr);

                return _serializer.Deserialize<T>(reader);
            }
            catch (Exception e)
            {
                throw new ArangoException(e.Message);
            }
        }
    }

    public class JsonNetContractResolver : DefaultContractResolver
    {
        public static readonly JsonNetContractResolver Instance = new JsonNetContractResolver();

        protected override JsonContract CreateContract(Type objectType)
        {
            JsonContract contract = base.CreateContract(objectType);

            
            Type t = objectType.IsGenericType && objectType.GetGenericTypeDefinition() == typeof(Nullable<>)
                ? Nullable.GetUnderlyingType(objectType)
                : objectType;

            if (t?.IsEnum ?? false)
            {
                contract.Converter = new StringEnumConverter();
            }
            
            if (objectType == typeof(object) || objectType == typeof(IDictionary<string, object>))
            {
                contract.Converter = new DictionaryConverter();
            }

            return contract;
        }

        public class DictionaryConverter : CustomCreationConverter<IDictionary<string, object>>
        {
            public override IDictionary<string, object> Create(Type objectType)
            {
                return new Dictionary<string, object>();
            }

            public override bool CanConvert(Type objectType)
            {
                // in addition to handling IDictionary<string, object>
                // we want to handle the deserialization of dict value
                // which is of type object
                return objectType == typeof(object) || base.CanConvert(objectType);
            }

            public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
            {
                if (reader.TokenType == JsonToken.StartObject
                    || reader.TokenType == JsonToken.Null)
                    return base.ReadJson(reader, objectType, existingValue, serializer);

                // if the next token is not an object
                // then fall back on standard deserializer (strings, numbers etc.)
                return serializer.Deserialize(reader);
            }
        }
    }
}