namespace Eyes.Universal.Sdk.DotNet.Commands
{
    public class EyesManagerOpenEyesResponsePayloadError
    {
        public string Message { get; set; }
        public string Stack { get; set; }
    }

    public class EyesManagerOpenEyesResponsePayload
    {
        public EyesRef Eyes { get; set; }
        public EyesManagerOpenEyesResponsePayloadError Error { get; set; }
    }

    public class EyesManagerOpenEyesResponse //: ClientCommandBase
    {
        public string Name { get; set; }

        public string Key { get; set; }

        public EyesManagerOpenEyesResponsePayload Payload { get; set; }
    }
}