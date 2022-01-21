namespace Applitools.Selenium.Commands.Requests
{
    public class EyesCloseRequestPayload
    {
        public EyesRef Eyes { get; set; }
    }

    public class EyesCloseRequest
    {
        public string Name => "Eyes.close";
        public EyesCloseRequestPayload Payload { get; set; }
    }
}