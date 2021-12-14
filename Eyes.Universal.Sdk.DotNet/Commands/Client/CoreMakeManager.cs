using Newtonsoft.Json;

namespace Eyes.Universal.Sdk.DotNet.Commands.Client
{
    public class CoreMakeManager : ClientCommandBase
    {
        [JsonProperty("key")]
        public string Key { get; set; }

        public override string Name => "Core.makeManager";
    }
}