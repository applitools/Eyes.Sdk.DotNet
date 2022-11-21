using Newtonsoft.Json;

namespace Applitools.Commands.Responses
{
    public class CoreMakeManagerResponseResult
    {
        [JsonProperty("applitools-ref-id")]
        public string ApplitoolsRefId { get; set; }
    }

    public class CoreMakeManagerResponsePayload
    {
        public CoreMakeManagerResponseResult Result { get; set; }
    }

    public class CoreMakeManagerResponse : CommandBase
    {
        public CoreMakeManagerResponsePayload Payload { get; set; }
    }
}