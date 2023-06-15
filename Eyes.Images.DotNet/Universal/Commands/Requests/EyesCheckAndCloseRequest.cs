namespace Applitools.Commands.Requests
{
    public class CheckAndCloseRequestPayload
    {
        public EyesRef Eyes { get; set; }
        public ITarget Target { get; set; }
        public CheckAndCloseSettings Settings { get; set; }
        public EyesConfig Config { get; set; }
    }

    public class EyesCheckAndCloseRequest : CommandBase
    {
        public CheckAndCloseRequestPayload Payload { get; set; }

        public EyesCheckAndCloseRequest()
        {
            Name = "Eyes.checkAndClose";
        }
    }
}