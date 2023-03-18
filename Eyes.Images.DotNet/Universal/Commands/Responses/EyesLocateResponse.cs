using System.Collections.Generic;

namespace Applitools.Commands.Responses
{
    public class LocateResponseResult : Dictionary<string, IList<Locate>>
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