namespace Applitools.Commands.Requests
{
    public class CloseResponsePayload
    {
        public EyesRef Eyes { get; set; }
        public CloseSettings Settings { get; set; }
        public EyesConfig Config { get; set; }
    }

    public class EyesCloseRequest : CommandBase
    {
        public CloseResponsePayload Payload { get; set; }

        public EyesCloseRequest()
        {
            Name = "Eyes.close";
        }
    }
}