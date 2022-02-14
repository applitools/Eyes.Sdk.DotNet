namespace Applitools.Selenium.Commands.Requests
{
    public class EyesManageCloseAllEyesRequestPayload
    {
        public ManagerRef Manager { get; set; }
    }

    public class EyesManageCloseAllEyesRequest : CommandBase
    {
        public EyesManageCloseAllEyesRequestPayload Payload { get; set; }

        public EyesManageCloseAllEyesRequest()
        {
            Name = "EyesManager.closeAllEyes";
        }
    }
}