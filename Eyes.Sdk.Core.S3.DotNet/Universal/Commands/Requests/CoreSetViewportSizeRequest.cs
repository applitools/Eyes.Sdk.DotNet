using Applitools.Utils.Geometry;

namespace Applitools.Selenium.Commands.Requests
{
    public class CoreSetViewportSizeRequestPayload : DriverRequestPayload
    {
        public RectangleSize Size { get; set; }
    }

    public class CoreSetViewportSizeRequest : CommandBase
    {
        public DriverRequestPayload Payload { get; set; }

        public CoreSetViewportSizeRequest()
        {
            Name = "Core.setViewportSize";
        }
    }
}