using System.Collections.Generic;

namespace Applitools.Commands.Responses
{
    public class EyesExtractTextResponsePayload //: Dictionary<string, Region>
    {
        public ICollection<string> Result { get; set; }
    }

    public class EyesExtractTextResponse : CommandBase
    {
        public EyesExtractTextResponsePayload Payload { get; set; }
    }
}