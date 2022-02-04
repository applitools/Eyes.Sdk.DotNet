namespace Applitools.Selenium.Commands.Requests
{
    public class EyesAbortRequestPayload
    {
        public EyesRef Eyes { get; set; }
    }

    public class EyesAbortRequest : CommandBase
    {
        public EyesAbortRequestPayload Payload { get; set; }

        public EyesAbortRequest()
        {
            Name = "Eyes.abort";
        }
    }
}