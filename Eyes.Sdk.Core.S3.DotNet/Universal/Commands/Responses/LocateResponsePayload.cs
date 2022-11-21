using System.Collections.Generic;
using Applitools.Utils.Geometry;

namespace Applitools.Commands.Requests
{
    public class LocateResponsePayload : Dictionary<string, Region>
    {
    }

    public class EyesLocateResponse : CommandBase
    {
        public LocateResponsePayload Payload { get; set; }
    }
}