using System;
using Applitools.Commands;
using Applitools.Playwright.Universal.Dto;
using Applitools.Universal;
using Applitools.Utils.Geometry;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Applitools.Playwright.Universal.Driver.Dto
{
    public class DriverCommandDto
    {
        [JsonProperty("driver")] public Reference Driver { get; set; }

        [JsonProperty("context")] public Reference Context { get; set; }

        [JsonProperty("element")] public Reference Element { get; set; }

        [JsonProperty("parent")] public Reference Parent { get; set; }

        [JsonProperty("selector")] 
        [JsonConverter(typeof(SelectorDeserializer))]
        public Selector Selector { get; set; }

        [JsonProperty("script")] public string Script { get; set; }

        [JsonProperty("arg")]
        [JsonConverter(typeof(ExecuteScriptDeserializer))]
        public object Arg { get; set; }

        [JsonProperty("size")] public RectangleSize Size { get; set; }

        [JsonProperty("url")] public string Url { get; set; }

        [JsonProperty("message")] public string Message { get; set; }

        [JsonProperty("level")] public string Level { get; set; }

        [JsonProperty("error")] public ResponsePayloadError Error { get; set; }

        public override string ToString()
        {
            return "DriverCommandDto{" +
                   "driver=" + Driver +
                   ", context=" + Context +
                   ", element=" + Element +
                   ", parent=" + Parent +
                   ", selector=" + Selector +
                   ", script='" + Script + '\'' +
                   ", arg=" + Arg +
                   ", size=" + Size +
                   ", url='" + Url + '\'' +
                   '}';
        }
    }

    public class SelectorDeserializer : JsonConverter
    {
        public override bool CanWrite => false;

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
        
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var jsonNode = JToken.Load(reader);

            if (jsonNode.Type == JTokenType.String)
            {
                return new Selector(jsonNode.ToString());
            }

            if (jsonNode.Type == JTokenType.Object)
            {
                return jsonNode.ToObject<Selector>();
            }
            
            throw new JsonSerializationException($"failed to deserialize {jsonNode} to type Selector");
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(Selector) || objectType == typeof(string);
        }
    }
}