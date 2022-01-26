namespace Applitools.Selenium.Commands.Requests
{
    public class ManagerConfig
    {
        public string Type { get; set; }
        public int? Concurrency { get; set; }
        public bool? Legacy { get; set; }
    }

    public class CoreMakeManagerRequest : RequestBase
    {
        public override string Name => "Core.makeManager";

        public ManagerConfig Payload { get; set; }
    }
}