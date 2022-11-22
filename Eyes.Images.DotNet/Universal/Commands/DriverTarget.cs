using System.Collections.Generic;

namespace Applitools.Commands
{
    public class DriverTarget : ITarget
    {
        public string ServerUrl { get; set; }
        public ProxySettings Proxy { get; set; }
        public string SessionId { get; set; }
        public IDictionary<string, object> Capabilities { get; set; }
    }
}