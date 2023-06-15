using System;

namespace Applitools
{
    public class ECClientCapabilities
    {
        public Uri ServerUrl { get; set; }
        public string ApiKey { get; set; }
    }

    public class EGClientSettings
    {
        public ECClientCapabilities Capabilities { get; set; }
        public ProxySettings Proxy { get; set; }
    }
}