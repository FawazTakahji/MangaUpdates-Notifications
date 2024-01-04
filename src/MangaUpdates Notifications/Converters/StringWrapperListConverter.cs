using MangaUpdates_Notifications.Models;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace MangaUpdates_Notifications.Converters
{
    public class StringWrapperListConverter<T> : JsonConverter<T> where T : IList<StringWrapper>, new()
    {
        public override T ReadJson(JsonReader reader, Type objectType, T existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            var list = existingValue ?? new T();

            if (reader.TokenType == JsonToken.StartArray)
            {
                JArray array = JArray.Load(reader);
                foreach (var token in array)
                {
                    var stringValue = token.ToString();
                    list.Add(new StringWrapper { String = stringValue });
                }
            }

            return list;
        }

        public override void WriteJson(JsonWriter writer, T value, JsonSerializer serializer)
        {
            JArray array = new JArray();
            foreach (var item in value)
            {
                array.Add(item.String);
            }
            array.WriteTo(writer);
        }
    }

}
