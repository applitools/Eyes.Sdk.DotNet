namespace Applitools.Selenium.Commands.Requests
{
    public class CheckRequestPayload
    {
        public EyesRef Eyes { get; set; }
        public ITarget Target { get; set; }
        public Applitools.Commands.CheckSettings Settings { get; set; }
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