namespace Eyes.Universal.Sdk.DotNet.Commands.Requests
{
    public class CoreGetViewportSizeRequest
    {
        public string Name => "Core.getViewportSize";

        public DriverRequestPayload Payload { get; set; }
    }
}