namespace Applitools.Selenium.Commands.Requests
{
    public class AbortPayload
    {
        public EyesRef Eyes { get; set; }
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