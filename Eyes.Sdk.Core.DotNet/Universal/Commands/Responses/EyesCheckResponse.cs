using Applitools;

namespace Applitools.Selenium.Commands.Responses
{
    public class EyesCheckResponsePayload
    {
        public MatchResult Result { get; set; }
        public ResponsePayloadError Error { get; set; }
    }

    public class EyesCheckResponse //: ClientCommandBase
    {
        public string Name { get; set; }
        public string Key { get; set; }

        public EyesCheckResponsePayload Payload { get; set; }
    }
}