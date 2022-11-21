using System.Collections.Generic;

namespace Applitools.Commands.Responses
{
    public class EyesCheckResponsePayload
    {
        public ICollection<MatchResult> Result { get; set; }
        public ResponsePayloadError Error { get; set; }

        public EyesCheckResponsePayload()
        {
            Result = new List<MatchResult>();
        }
    }

    public class EyesCheckResponse : CommandBase
    {
        public EyesCheckResponsePayload Payload { get; set; }
    }
}