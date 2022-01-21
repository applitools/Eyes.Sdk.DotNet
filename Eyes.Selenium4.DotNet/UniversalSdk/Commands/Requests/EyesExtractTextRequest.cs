namespace Applitools.Selenium.Commands.Requests
{
    public class OCRExtractSettings
    {
        public string Hint { get; set; }
        public int? number { get; set; }
        public string language { get; set; }
    }

    public class EyesExtractTextRequestPayload
    {
        public EyesRef Eyes { get; set; }
        public OCRExtractSettings[] Regions { get; set; }
        public EyesConfig Config { get; set; }
    }

    public class EyesExtractTextRequest
    {
        public string Name => "Eyes.extractText";

        public string Key { get; set; }

        public EyesExtractTextRequestPayload Payload { get; set; }
    }
}