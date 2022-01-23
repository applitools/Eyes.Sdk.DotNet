namespace Applitools.Selenium.Commands.Requests
{
    public class CoreGetViewportSizeRequest : RequestBase
    {
        public override string Name => "Core.getViewportSize";

        public DriverRequestPayload Payload { get; set; }
    }
}