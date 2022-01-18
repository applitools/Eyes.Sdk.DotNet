using Applitools.Utils.Geometry;

namespace Eyes.Universal.Sdk.DotNet.Commands.Requests
{
    public class CoreSetViewportSizeRequestPayload : DriverRequestPayload
    {
        public RectangleSize Size { get; set; }
    }

    public class CoreSetViewportSizeRequest
    {
        public string Name => "Core.setViewportSize";

        public DriverRequestPayload Payload { get; set; }
    }
}