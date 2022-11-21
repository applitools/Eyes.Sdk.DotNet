using System.Collections.Generic;

namespace Applitools.Commands.Requests
{
    public class DeleteTestRequestPayload
    {
        public ICollection<DeleteTestSettings> Settings { get; set; }

        public DeleteTestRequestPayload()
        {
            Settings = new List<DeleteTestSettings>();
        }
    }

    public class CoreDeleteTestRequest : CommandBase
    {
        public DeleteTestRequestPayload Payload { get; set; }

        public CoreDeleteTestRequest()
        {
            Name = "Core.deleteTest";
        }
    }
}