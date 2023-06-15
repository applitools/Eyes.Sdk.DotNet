using System.Collections.Generic;
using Applitools.Utils.Geometry;

namespace Applitools.Commands.Responses
{
    public class LocateResponseResult : Dictionary<string, IList<Region>>
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