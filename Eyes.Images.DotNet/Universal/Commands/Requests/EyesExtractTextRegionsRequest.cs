namespace Applitools.Commands.Requests
{
    public class ExtractTextRegionsRequestPayload
    {
        public ITarget Target { get; set; }
        public TextRegionSettings Settings { get; set; }
        public Config Config { get; set; }
    }

    public class EyesExtractTextRegionsRequest : CommandBase
    {
        public ExtractTextRegionsRequestPayload Payload { get; set; }

        public EyesExtractTextRegionsRequest()
        {
            Name = "Core.locateText";
        }
    }
}