namespace Applitools.Selenium.Commands.Requests
{
    public class EyesAbortRequestPayload
    {
        public EyesRef Eyes { get; set; }
    }

    public class EyesAbortRequest : RequestBase
    {
        public override string Name => "Eyes.abort";

        public EyesAbortRequestPayload Payload { get; set; }
    }
}