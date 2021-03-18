using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Applitools.Utils
{
    public class PropertiesCollectionConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return typeof(PropertiesCollection).IsAssignableFrom(objectType);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var dataList = serializer.Deserialize<List<PropertyData>>(reader);
            var collection = new PropertiesCollection();
            foreach (var el in dataList)
                collection.Add(el.Name, el.Value);
            return collection;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            List<PropertyData> list = new List<PropertyData>((PropertiesCollection)value);
            serializer.Serialize(writer, list);
        }
    }
}
