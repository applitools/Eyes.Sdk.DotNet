namespace Applitools.Selenium.Commands.Requests
{
    public class EyesCheckRequestPayload
    {
        public EyesRef Eyes { get; set; }
        public UniversalCheckSettings Settings { get; set; }
        public EyesConfig Config { get; set; }
    }

    public class EyesCheckRequest : RequestBase
    {
        public override string Name => "Eyes.check";
        
        public EyesCheckRequestPayload Payload { get; set; }
    }
}