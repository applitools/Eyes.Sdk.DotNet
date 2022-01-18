namespace Eyes.Universal.Sdk.DotNet.Commands
{
    public class EyesManagerOpenEyesResponsePayload
    {
        public EyesRef Result { get; set; }
        public ResponsePayloadError Error { get; set; }
    }

    public class EyesManagerOpenEyesResponse //: ClientCommandBase
    {
        public string Name { get; set; }

        public string Key { get; set; }

        public EyesManagerOpenEyesResponsePayload Payload { get; set; }
    }
}