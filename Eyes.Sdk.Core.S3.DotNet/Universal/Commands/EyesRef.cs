using Newtonsoft.Json;

namespace Applitools.Selenium.Commands
{
    public class EyesRef
    {
        [JsonProperty("applitools-ref-id")]
        public string ApplitoolsRefId { get; set; }
    }
}