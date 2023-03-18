using System.Collections.Generic;

namespace Applitools.Commands.Responses
{
    public class EyesGetResultsResponsePayload
    {
        public ICollection<TestResults> Result { get; set; }
        public ResponsePayloadError Error { get; set; }

        public EyesGetResultsResponsePayload()
        {
            Result = new List<TestResults>();
        }
    }

    public class EyesGetResultsResponse : CommandBase
    {
        public EyesGetResultsResponsePayload Payload { get; set; }
    }
}