using System;
using System.Collections.Generic;
using Applitools.Playwright.Universal.Dto;
using Applitools.Universal;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Applitools.Playwright.Universal.Driver.Dto
{
    public class ExecuteScriptDeserializer : JsonConverter
    {
        public override bool CanWrite => false;

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue,
            JsonSerializer serializer)
        {
            var jsonNode = JToken.Load(reader);

            switch (jsonNode.Type)
            {
                case JTokenType.Object:
                    return HandleObject_((JObject)jsonNode);
                case JTokenType.Array:
                    return HandleArray_((JArray)jsonNode);
                case JTokenType.Boolean:
                    return jsonNode.Value<bool>();
                case JTokenType.Null:
                    return null;
            }

            throw new EyesException("Unsupported type to deserialize!");
        }

        private object HandleObject_(JObject jsonObject)
        {
            if (jsonObject.TryGetValue(Applitools.Universal.Refer.APPLITOOLS_REF_ID, out JToken refId) && refId.Type == JTokenType.String)
            {
                if (jsonObject.TryGetValue("type", out JToken typeNode) && typeNode.Type == JTokenType.String)
                {
                    string type = typeNode.Value<string>();

                    switch (type)
                    {
                        case "element":
                            return jsonObject.ToObject<Element>();
                        case "selector":
                            return jsonObject.ToObject<Selector>();
                        default:
                            return jsonObject.ToObject<Reference>();
                    }
                }
            }
            else
            {
                Dictionary<object, object> obj = new Dictionary<object, object>();
                foreach (var subNode in jsonObject)
                {
                    switch (subNode.Value?.Type)
                    {
                        case JTokenType.Object:
                            obj.Add(subNode.Key, HandleObject_((JObject)subNode.Value));
                            break;
                        case JTokenType.Array:
                            obj.Add(subNode.Key, HandleArray_((JArray)subNode.Value));
                            break;
                        case JTokenType.Boolean:
                            obj.Add(subNode.Key, subNode.Value.Value<bool>());
                            break;
                        case JTokenType.Integer:
                            obj.Add(subNode.Key, subNode.Value.Value<int>());
                            break;
                        case JTokenType.Float:
                            obj.Add(subNode.Key, subNode.Value.Value<float>());
                            break;
                        case JTokenType.Null:
                            obj.Add(subNode.Key, null);
                            break;
                        default:
                            obj.Add(subNode.Key,subNode.Value?.ToString());
                            break;
                    }
                }

                return obj;
            }

            throw new JsonSerializationException($"failed to deserialize {jsonObject}");
        }

        private object HandleArray_(JArray jsonArray)
        {
            List<object> array = new List<object>();
            foreach (var subNode in jsonArray)
            {
                switch (subNode.Type)
                {
                    case JTokenType.Object:
                        array.Add(HandleObject_((JObject)subNode));
                        break;
                    case JTokenType.Array:
                        array.Add( HandleArray_((JArray)subNode));
                        break;
                    case JTokenType.Boolean:
                        array.Add( subNode.Value<bool>());
                        break;
                    case JTokenType.Integer:
                        array.Add( subNode.Value<int>());
                        break;
                    case JTokenType.Float:
                        array.Add( subNode.Value<float>());
                        break;
                    case JTokenType.Null:
                        array.Add( null);
                        break;
                    default:
                        array.Add(subNode.ToString());
                        break;
                }
            }
            return array;
        }

        public override bool CanConvert(Type objectType)
        {
            return true;
        }
    }
}