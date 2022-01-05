using System.Collections.Generic;

namespace Eyes.Universal.Sdk.DotNet.Commands
{
    public class UniversalDriverConfig
    {
        public string ServerUrl { get; set; }
        public string SessionId { get; set; }
        public IDictionary<string, object> Capabilities { get; set; }
    }
}