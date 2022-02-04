namespace Applitools.Selenium.Commands.Requests
{
    public class EyesCloseRequestPayload
    {
        public EyesRef Eyes { get; set; }
    }

    public class EyesCloseRequest : CommandBase
    {
        public EyesCloseRequestPayload Payload { get; set; }

        public EyesCloseRequest()
        {
            Name = "Eyes.close";
        }
    }
}