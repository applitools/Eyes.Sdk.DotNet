using Newtonsoft.Json;

namespace Applitools.Selenium.Commands.Requests
{
    public class ManagerRef
    {
        [JsonProperty("applitools-ref-id")]
        public string ApplitoolsRefId { get; set; }
    }

    public class EyesManagerOpenEyesRequestPayload
    {
        public ManagerRef Manager { get; set; }

        public UniversalDriverConfig Driver { get; set; }

        public EyesConfig Config { get; set; }
    }

    public class EyesManagerOpenEyes : CommandBase
    {
        public EyesManagerOpenEyesRequestPayload Payload { get; set; }

        public EyesManagerOpenEyes()
        {
            Name = "EyesManager.openEyes";
        }
    }
}