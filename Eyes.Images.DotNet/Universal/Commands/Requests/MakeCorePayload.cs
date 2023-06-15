using System.Collections.Generic;
using Newtonsoft.Json;

namespace Applitools.Commands.Requests
{
    public class MakeCorePayload
    {
        [JsonProperty("agentId")]
        public string AgentId { get; set; }

        [JsonProperty("cwd")]
        public string Cwd { get; set; }
        
        [JsonIgnore]
        public string[] Commands { get; set; }

        [JsonIgnore]
        public string Protocol { get; set; }
        
        [JsonProperty("spec")]
        public object Spec => (object)Commands ?? Protocol;
    }
}
