using System.Collections.Generic;

namespace Applitools.Selenium.Commands.Requests
{
    public class CoreDeleteTestSettings
    {
        public string TestId { get; set; }
        public string BatchId { get; set; }
        public string ServerUrl { get; set; }
        public string SecretToken { get; set; }
        public string ApiKey { get; set; }
        public ProxySettings Proxy { get; set; }
    }

    public class CoreDeleteTestRequestPayload
    {
        public ICollection<CoreDeleteTestSettings> Settings { get; set; }

        public CoreDeleteTestRequestPayload()
        {
            Settings = new List<CoreDeleteTestSettings>();
        }
    }

    public class CoreDeleteTestRequest : CommandBase
    {
        public CoreDeleteTestRequestPayload Payload { get; set; }

        public CoreDeleteTestRequest()
        {
            Name = "Core.deleteTest";
        }
    }
}