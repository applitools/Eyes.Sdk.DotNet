using Newtonsoft.Json;

namespace Eyes.Universal.Sdk.DotNet.Commands
{
    public class EyesRef
    {
        [JsonProperty("applitools-ref-id")]
        public string ApplitoolsRefId { get; set; }
    }
}