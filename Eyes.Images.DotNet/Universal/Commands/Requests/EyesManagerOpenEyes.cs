using Newtonsoft.Json;

namespace Applitools.Commands.Requests
{
    public class ManagerRef
    {
        [JsonProperty("applitools-ref-id")]
        public string ApplitoolsRefId { get; set; }
    }

    public class OpenEyesRequestPayload
    {
        public ManagerRef Manager { get; set; }
        public ITarget Target { get; set; }
        public OpenSettings Settings { get; set; }
        public Config Config { get; set; }
    }

    public class EyesManagerOpenEyes : CommandBase
    {
        public OpenEyesRequestPayload Payload { get; set; }

        public EyesManagerOpenEyes()
        {
            Name = "EyesManager.openEyes";
        }
    }
}