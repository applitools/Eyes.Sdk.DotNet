using System.Collections.Generic;
using System.Net;

namespace Applitools.Commands.Requests
{
    public class CloseBatchSettings
    {
        public string BatchId { get; set; }
        public string ServerUrl { get; set; }
        public WebProxy Proxy { get; set; }
        public string ApiKey { get; set; }
    }

    public class CloseBatchRequestPayload
    {
        public ICollection<CloseBatchSettings> Settings { get; set; }

        public CloseBatchRequestPayload()
        {
            Settings = new List<CloseBatchSettings>();
        }
    }

    public class CoreCloseBatchesRequest : CommandBase
    {
        public CloseBatchRequestPayload Payload { get; set; }

        public CoreCloseBatchesRequest()
        {
            Name = "Core.closeBatch";
        }
    }
}