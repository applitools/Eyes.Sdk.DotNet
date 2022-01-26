namespace Applitools.Selenium.Commands.Requests
{
    public class EyesManageCloseAllEyesRequestPayload
    {
        public ManagerRef Manager { get; set; }
    }

    public class EyesManageCloseAllEyesRequest : RequestBase
    {
        public override string Name => "EyesManager.closeAllEyes";

        public EyesManageCloseAllEyesRequestPayload Payload { get; set; }
    }
}