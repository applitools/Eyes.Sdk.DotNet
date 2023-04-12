using System.Collections.Generic;

namespace Applitools.Commands.Responses
{
    public class LocateResponseResult : Dictionary<string, IList<UniversalRegion>>
    {
    }

    public class LocateResponsePayload //: Dictionary<string, Region>
    {
        public LocateResponseResult Result { get; set; }
    }

    public class EyesLocateResponse : CommandBase
    {
        public LocateResponsePayload Payload { get; set; }
    }
}