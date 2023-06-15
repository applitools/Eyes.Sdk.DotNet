namespace Applitools.Commands.Requests
{
    public class AbortPayload
    {
        public EyesRef Eyes { get; set; }
        public CloseSettings Settings { get; set; }
    }

    public class EyesAbortRequest : CommandBase
    {
        public AbortPayload Payload { get; set; }

        public EyesAbortRequest()
        {
            Name = "Eyes.abort";
        }
    }
}