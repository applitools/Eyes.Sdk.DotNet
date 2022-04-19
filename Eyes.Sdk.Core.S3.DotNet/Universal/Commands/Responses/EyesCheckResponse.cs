namespace Applitools.Selenium.Commands.Responses
{
    public class EyesCheckResponsePayload
    {
        public MatchResult Result { get; set; }
        public ResponsePayloadError Error { get; set; }
    }

    public class EyesCheckResponse : CommandBase
    {
        public EyesCheckResponsePayload Payload { get; set; }
    }
}