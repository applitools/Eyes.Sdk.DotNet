using Applitools.Utils.Geometry;

namespace Applitools.Selenium.Commands.Requests
{
    public class CoreSetViewportSizeRequestPayload : DriverRequestPayload
    {
        public RectangleSize Size { get; set; }
    }

    public class CoreSetViewportSizeRequest : RequestBase
    {
        public override string Name => "Core.setViewportSize";

        public DriverRequestPayload Payload { get; set; }
    }
}