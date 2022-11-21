namespace Applitools.Commands.Requests
{
    public class LocateTextRequestPayload
    {
        public EyesRef Eyes { get; set; }
        public ITarget Target { get; set; }
        public LocateTextSettings Settings { get; set; }
        public EyesConfig Config { get; set; }
    }

    public class EyesExtractTextRegionsRequest : CommandBase
    {
        public LocateTextRequestPayload Payload { get; set; }

        public EyesExtractTextRegionsRequest()
        {
            Name = "Eyes.locateText";
        }
    }
}