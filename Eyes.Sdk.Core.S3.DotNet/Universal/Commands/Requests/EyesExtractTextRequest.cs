using Applitools.Fluent;

namespace Applitools.Selenium.Commands.Requests
{
    public class OCRExtractSettings
    {
        public TRegion Target { get; set; }
        public string Hint { get; set; }
        public int? minMatch { get; set; }
        public string Language { get; set; }
    }

    public class EyesExtractTextRequestPayload
    {
        public EyesRef Eyes { get; set; }
        public OCRExtractSettings[] Regions { get; set; }
        public EyesConfig Config { get; set; }
    }

    public class EyesExtractTextRequest : CommandBase
    {
        public EyesExtractTextRequestPayload Payload { get; set; }

        public EyesExtractTextRequest()
        {
            Name = "Eyes.extractText";
        }
    }
}