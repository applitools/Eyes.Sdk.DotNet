using Newtonsoft.Json;

namespace Eyes.Universal.Sdk.DotNet.Commands.Client
{
    [JsonObject(Title = "EyesManager.openEyes")]
    public class EyesManagerOpenEyes
    {
        [JsonProperty("manager")]
        public object Manager { get; set; }

        [JsonProperty("driver")]
        public object Driver { get; set; }

        [JsonProperty("config")]
        public object Config { get; set; }
    }
}