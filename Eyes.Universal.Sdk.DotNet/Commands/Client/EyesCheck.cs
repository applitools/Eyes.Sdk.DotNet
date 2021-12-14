using Newtonsoft.Json;

namespace Eyes.Universal.Sdk.DotNet.Commands.Client
{
    [JsonObject(Title = "Eyes.check")]
    public class EyesCheck
    {
        [JsonProperty("eyes")]
        public object Eyes { get; set; }

        [JsonProperty("settings")]
        public object Settings { get; set; }

        [JsonProperty("config")]
        public object Config { get; set; }
    }
}