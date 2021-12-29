using Newtonsoft.Json;
using OpenQA.Selenium;

namespace Eyes.Universal.Sdk.DotNet.Commands.Client
{
    public class EyesManagerOpenEyesRequestManager
    {
        [JsonProperty("applitools-ref-id")]
        public string ApplitoolsRefId { get; set; }
    }

    public class EyesManagerOpenEyesRequestPayload
    {
        [JsonProperty("manager")]
        public EyesManagerOpenEyesRequestManager Manager { get; set; }

        [JsonProperty("driver")]
        public UniversalDriverConfig Driver { get; set; }

        [JsonProperty("config")]
        public EyesConfig Config { get; set; }
    }

    public class EyesManagerOpenEyes //: ClientCommandBase
    {
        [JsonProperty("name")]
        public string Name => "EyesManager.openEyes";

        [JsonProperty("key")]
        public string Key { get; set; }

        [JsonProperty("payload")]
        public EyesManagerOpenEyesRequestPayload Payload { get; set; }

        //[JsonProperty("manager")]
        //public object Manager { get; set; }

        //[JsonProperty("driver")]
        //public object Driver { get; set; }

        //[JsonProperty("config")]
        //public object Config { get; set; }
    }
}