using System.Net;

namespace Eyes.Universal.Sdk.DotNet.Commands.Requests
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

    public class CoreCloseBatchesRequest
    {
        public string Name => "Core.closeBatches";

        public string Key { get; set; }

        public CoreCloseBatchesRequestPayload Payload { get; set; }
    }
}