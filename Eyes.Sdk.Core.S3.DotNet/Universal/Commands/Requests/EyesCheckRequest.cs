namespace Applitools.Commands.Requests
{
    public class CheckRequestPayload
    {
        public EyesRef Eyes { get; set; }
        public ITarget Target { get; set; }
        public UniversalCheckSettings Settings { get; set; }
        public Config Config { get; set; }
    }

    public class EyesCheckRequest : CommandBase
    {
        public CheckRequestPayload Payload { get; set; }

        public EyesCheckRequest()
        {
            Name = "Eyes.check";
        }
    }
}