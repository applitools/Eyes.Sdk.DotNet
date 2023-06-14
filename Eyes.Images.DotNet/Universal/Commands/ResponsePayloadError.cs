using System;
using Newtonsoft.Json;

namespace Applitools.Commands
{
    public class ResponsePayloadError
    {
        [JsonProperty("message")]
        public string Message { get; set; }
        
        [JsonProperty("stack")]
        public string Stack { get; set; }
        
        [JsonProperty("reason")]
        public string Reason { get; set; }

        public override string ToString()
        {
            return $"{Message}{Environment.NewLine}{Stack}";
        }
    }
}