namespace Eyes.Universal.Sdk.DotNet.Commands.Requests
{
    public class EyesAbortRequestPayload
    {
        public EyesRef Eyes { get; set; }
    }

    public class EyesAbortRequest
    {
        public string Name => "Eyes.abort";

        public EyesAbortRequestPayload Payload { get; set; }
    }
}