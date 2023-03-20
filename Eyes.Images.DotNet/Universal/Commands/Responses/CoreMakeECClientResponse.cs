namespace Applitools.Commands.Responses
{
    public class CoreMakeECClientResponsePayloadResult
    {
        public string Url { get; set; }
    }

    public class CoreMakeECClientResponsePayload
    {
        public CoreMakeECClientResponsePayloadResult Result { get; set; }
        public ResponsePayloadError Error { get; set; }
    }

    public class CoreMakeECClientResponse : CommandBase
    {
        public CoreMakeECClientResponsePayload Payload { get; set; }
    }
}