namespace Eyes.Universal.Sdk.DotNet.Commands.Requests
{
    public class CoreMakeManager : ClientCommandBase
    {
        public string Key { get; set; }

        public override string Name => "Core.makeManager";
    }
}