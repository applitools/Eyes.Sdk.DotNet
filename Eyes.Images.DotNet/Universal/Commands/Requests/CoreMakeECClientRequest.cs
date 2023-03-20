namespace Applitools.Commands.Requests
{
    public class CoreMakeECClientRequestPayload
    {
        public EGClientSettings Settings { get; set; }
    }

    public class CoreMakeECClientRequest : CommandBase
    {
        public CoreMakeECClientRequestPayload Payload { get; set; }

        public CoreMakeECClientRequest()
        {
            Name = "Core.makeECClient";
        }
    }
}