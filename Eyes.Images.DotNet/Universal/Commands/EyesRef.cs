using Newtonsoft.Json;

namespace Applitools.Commands
{
    public class EyesRef
    {
        [JsonProperty("applitools-ref-id")]
        public string ApplitoolsRefId { get; set; }
    }
}