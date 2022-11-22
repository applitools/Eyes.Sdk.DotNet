namespace Applitools.Commands.Requests
{
    public class MakeManagerRequestPayload
    {
        public string Type { get; set; }
        public int? Concurrency { get; set; }
        public bool? Legacy { get; set; }
        public string AgentId { get; set; }
    }

    public class CoreMakeManagerRequest : CommandBase
    {
        public MakeManagerRequestPayload Payload { get; set; }

        public CoreMakeManagerRequest()
        {
            Name = "Core.makeManager";
        }
    }
}