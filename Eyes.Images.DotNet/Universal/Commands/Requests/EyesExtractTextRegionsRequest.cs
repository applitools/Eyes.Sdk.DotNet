namespace Applitools.Commands.Requests
{
    public class ExtractTextRegionsRequestPayload
    {
        public EyesRef Eyes { get; set; }
        public ITarget Target { get; set; }
        public TextRegionSettings Settings { get; set; }
        public Config Config { get; set; }
    }

    public class EyesExtractTextRegionsRequest : CommandBase
    {
        public ExtractTextRegionsRequestPayload Payload { get; set; }

        public EyesExtractTextRegionsRequest()
        {
            Name = "Eyes.locateText";
        }
    }
}