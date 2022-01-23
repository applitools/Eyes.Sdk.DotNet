namespace Applitools.Selenium.Commands.Requests
{
    public class OCRSearchSettings
    {
        public object[] Patterns { get; set; }
        public bool? IgnoreCase { get; set; }
        public bool? FirstOnly { get; set; }
        public string Language { get; set; }
    }

    public class EyesExtractTextRegionsRequestPayload
    {
        public EyesRef Eyes { get; set; }
        public OCRSearchSettings Settings { get; set; }
        public EyesConfig Config { get; set; }
    }

    public class EyesExtractTextRegionsRequest : RequestBase
    {
        public override string Name => "Eyes.extractTextRegions";
        
        public EyesExtractTextRegionsRequestPayload Payload { get; set; }
    }
}