using Newtonsoft.Json;

namespace Eyes.Universal.Sdk.DotNet.Commands.Responses
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

    public class CoreMakeManagerResponse //: ClientCommandBase
    {
        public string Key { get; set; }

        public CoreMakeManagerResponsePayload Payload { get; set; }
    }
}