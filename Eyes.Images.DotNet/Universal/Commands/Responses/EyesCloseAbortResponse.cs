using System.Collections.Generic;

namespace Applitools.Commands.Responses
{
    public class EyesCloseAbortResponsePayload
    {
        public ResponsePayloadError Error { get; set; }
    }

    public class EyesCloseAbortResponse : CommandBase
    {
        public EyesCloseAbortResponsePayload Payload { get; set; }
    }
}