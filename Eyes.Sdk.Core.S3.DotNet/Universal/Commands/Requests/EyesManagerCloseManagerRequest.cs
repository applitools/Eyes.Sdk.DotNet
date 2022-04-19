namespace Applitools.Selenium.Commands.Requests
{
    public class EyesManageCloseManagerRequestPayload
    {
        public ManagerRef Manager { get; set; }
        public bool ThrowErr { get; set; }
    }

    public class EyesManagerCloseManagerRequest : CommandBase
    {
        public EyesManageCloseManagerRequestPayload Payload { get; set; }

        public EyesManagerCloseManagerRequest()
        {
            Name = "EyesManager.closeManager";
        }
    }
}