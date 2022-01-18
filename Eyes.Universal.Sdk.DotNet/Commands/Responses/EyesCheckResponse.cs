using Applitools;

namespace Eyes.Universal.Sdk.DotNet.Commands.Responses
{
    public class EyesCheckResponsePayload
    {
        public MatchResult Result { get; set; }
    }

    public class EyesCheckResponse //: ClientCommandBase
    {
        public string Name { get; set; }
        public string Key { get; set; }

        public EyesCheckResponsePayload Payload { get; set; }
    }
}