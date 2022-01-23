namespace Applitools.Selenium.Commands.Requests
{
    public class LocateSettings
    {
        public object[] LocatorNames { get; set; }
        public bool? FirstOnly { get; set; }
    }

    public class EyesLocateRequestPayload
    {
        public EyesRef Eyes { get; set; }
        public LocateSettings Settings { get; set; }
        public EyesConfig Config { get; set; }
    }

    public class EyesLocateRequest : RequestBase
    {
        public override string Name => "Eyes.locate";
        
        public EyesLocateRequestPayload Payload { get; set; }
    }
}