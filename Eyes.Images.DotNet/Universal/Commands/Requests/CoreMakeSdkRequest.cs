using Newtonsoft.Json;

namespace Applitools.Commands.Requests
{
    public class CoreMakeSdkRequest : CommandBase
    {
        public CoreMakeSdkRequest()
        {
            Name = "Core.makeCore";
        }
        
        [JsonProperty]
        public MakeCorePayload Payload { get; set; }
    }
}