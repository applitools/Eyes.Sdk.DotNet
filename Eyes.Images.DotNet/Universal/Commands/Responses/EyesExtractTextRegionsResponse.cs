using System.Collections.Generic;

namespace Applitools.Commands.Responses
{
    public class ExtractTextResponseResult : Dictionary<string, TextRegion>
    {
    }

    public class ExtractTextRegionsResponsePayload //: Dictionary<string, Region>
    {
        public IDictionary<string, IList<TextRegion>> Result { get; set; }
    }

    public class EyesExtractTextRegionsResponse : CommandBase
    {
        public ExtractTextRegionsResponsePayload Payload { get; set; }
    }
}