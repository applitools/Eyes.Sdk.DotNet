using OpenQA.Selenium;

namespace Eyes.Universal.Sdk.DotNet.Commands
{
    public class UniversalDriverConfig
    {
        public string ServerUrl { get; set; }
        public string SessionId { get; set; }
        public ICapabilities Capabilities { get; set; }
    }
}