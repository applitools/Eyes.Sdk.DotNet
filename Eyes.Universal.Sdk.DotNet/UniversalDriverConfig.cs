using Newtonsoft.Json;
using OpenQA.Selenium;
using OpenQA.Selenium.Remote;

namespace Eyes.Universal.Sdk.DotNet
{
    public class UniversalDriverConfig
    {
        [JsonProperty("serverUrl")]
        public string ServerUrl { get; set; }

        [JsonProperty("sessionId")]
        public SessionId SessionId { get; set; }

        [JsonProperty("capabilities")]
        public ICapabilities Capabilities { get; set; }
    }
}