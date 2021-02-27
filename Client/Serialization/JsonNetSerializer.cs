using System;
using System.Collections.Generic;
using System.IO;
using ArangoDriver.Exceptions;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

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
            };
            _serializer.Converters.Add(new DictionaryConverter());
            _serializer.Converters.Add(new StringEnumConverter());
        }

        public void RegisterSerializer(JsonConverter converter)
        {
            _serializer.Converters.Add(converter);
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
        
        private class DictionaryConverter : CustomCreationConverter<IDictionary<string, object>>
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