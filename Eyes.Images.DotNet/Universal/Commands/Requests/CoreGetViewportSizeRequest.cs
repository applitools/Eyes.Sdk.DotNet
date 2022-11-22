namespace Applitools.Commands.Requests
{
    public class CoreGetViewportSizeRequest : CommandBase
    {
        public DriverRequestPayload Payload { get; set; }

        public CoreGetViewportSizeRequest()
        {
            Name = "Core.getViewportSize";
        }
    }
}