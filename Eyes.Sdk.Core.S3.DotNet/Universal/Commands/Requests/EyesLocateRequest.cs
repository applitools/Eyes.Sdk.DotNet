namespace Applitools.Selenium.Commands.Requests
{
    public class EyesLocateRequestPayload
    {
        public ITarget Target { get; set; }
        public LocateSettings Settings { get; set; }
        public EyesConfig Config { get; set; }
    }
     
    public class EyesLocateRequest : CommandBase
    {
        public EyesLocateRequestPayload Payload { get; set; }

        public EyesLocateRequest()
        {
            Name = "Core.locate";
        }
    }
}