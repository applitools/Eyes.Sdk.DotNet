using Applitools.Utils.Geometry;

namespace Applitools.Commands.Requests
{
    public class SetViewportSizeRequestPayload : DriverRequestPayload
    {
        public RectangleSize Size { get; set; }
    }

    public class CoreSetViewportSizeRequest : CommandBase
    {
        public SetViewportSizeRequestPayload Payload { get; set; }

        public CoreSetViewportSizeRequest()
        {
            Name = "Core.setViewportSize";
        }
    }
}