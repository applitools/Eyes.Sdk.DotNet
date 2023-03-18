namespace Applitools.Commands.Requests
{
    public class CloseManagerRequestPayload
    {
        public ManagerRef Manager { get; set; }
        public CloseSettings Settings { get; set; }
    }

    public class EyesManagerCloseManagerRequest : CommandBase
    {
        public CloseManagerRequestPayload Payload { get; set; }

        public EyesManagerCloseManagerRequest()
        {
            Name = "EyesManager.getResults";
        }
    }
}