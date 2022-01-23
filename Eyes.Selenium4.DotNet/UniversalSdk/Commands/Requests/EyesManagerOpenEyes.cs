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

    public class EyesManagerOpenEyes : RequestBase
    {
        public override string Name => "EyesManager.openEyes";
        
        public EyesManagerOpenEyesRequestPayload Payload { get; set; }
    }
}