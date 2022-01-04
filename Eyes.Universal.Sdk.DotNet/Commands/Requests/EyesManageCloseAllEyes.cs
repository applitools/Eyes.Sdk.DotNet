namespace Eyes.Universal.Sdk.DotNet.Commands.Requests
{
    public class EyesManageCloseAllEyesRequestPayload
    {
        public ManagerRef Manager { get; set; }
    }

    public class EyesManageCloseAllEyes
    {
        public EyesManageCloseAllEyesRequestPayload Payload { get; set; }
    }
}