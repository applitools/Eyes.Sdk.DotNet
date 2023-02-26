using System.Collections.Generic;

namespace Applitools.Commands.Requests
{
    public class ExtractTextRequestPayload
    {
        public EyesRef Eyes { get; set; }
        public ITarget Target { get; set; }
        public ICollection<ExtractTextSettings> Settings { get; set; }
        public Config Config { get; set; }

        public ExtractTextRequestPayload()
        {
            Settings = new List<ExtractTextSettings>();
        }
    }

    public class EyesExtractTextRequest : CommandBase
    {
        public ExtractTextRequestPayload Payload { get; set; }

        public EyesExtractTextRequest()
        {
            Name = "Eyes.extractText";
        }
    }
}