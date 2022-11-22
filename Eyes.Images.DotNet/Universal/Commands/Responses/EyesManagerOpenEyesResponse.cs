namespace Applitools.Commands.Responses
{
    public class EyesManagerOpenEyesResponsePayload
    {
        public EyesRef Result { get; set; }
        public ResponsePayloadError Error { get; set; }
    }

    public class EyesManagerOpenEyesResponse : CommandBase
    {
        public EyesManagerOpenEyesResponsePayload Payload { get; set; }
    }
}