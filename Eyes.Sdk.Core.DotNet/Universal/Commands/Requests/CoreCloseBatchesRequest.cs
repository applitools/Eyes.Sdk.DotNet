using System.Net;

namespace Applitools.Selenium.Commands.Requests
{
    public class CloseBatchesSettings
    {
        public string BatchIds { get; set; }
        public string ServerUrl { get; set; }
        public WebProxy Proxy { get; set; }
    }

    public class CoreCloseBatchesRequestPayload
    {
        public EyesRef Eyes { get; set; }
        public CloseBatchesSettings Settings { get; set; }
        public EyesConfig Config { get; set; }
    }

    public class CoreCloseBatchesRequest : RequestBase
    {
        public override string Name => "Core.closeBatches";

        public CoreCloseBatchesRequestPayload Payload { get; set; }
    }
}