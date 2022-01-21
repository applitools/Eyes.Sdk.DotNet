namespace Applitools.Selenium.Commands.Requests
{
    public class EyesCheckRequestPayload
    {
        public EyesRef Eyes { get; set; }
        public UniversalCheckSettings Settings { get; set; }
        public EyesConfig Config { get; set; }
    }

    public class EyesCheckRequest
    {
        public string Name => "Eyes.check";

        public string Key { get; set; }

        public EyesCheckRequestPayload Payload { get; set; }
    }
}