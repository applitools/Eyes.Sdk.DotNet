using Newtonsoft.Json;

namespace Applitools.Commands.Responses
{
    public class ResponseDto
    {
        public ResponseDto()
        {
        }

        public ResponseDto(string name, string key, ResponsePayload payload)
        {
            Name = name;
            Key = key;
            Payload = payload;
        }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("key")]
        public string Key { get; set; }

        [JsonProperty("payload")]
        public ResponsePayload Payload { get; set; } = new ResponsePayload();
    }

    public class ResponsePayload
    {
        public ResponsePayload()
        {
        }

        public ResponsePayload(object result, ResponsePayloadError error = null)
        {
            Result = result;
            Error = error;
        }

        [JsonProperty("error")]
        public ResponsePayloadError Error { get; set; }

        [JsonProperty("result")]
        public object Result { get; set; }
    }
}