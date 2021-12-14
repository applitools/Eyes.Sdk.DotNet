using Newtonsoft.Json;

namespace Eyes.Universal.Sdk.DotNet.Commands.Client
{
    public class CoreMakeSdk : ClientCommandBase
    {
        [JsonProperty("name")]
        public override string Name => "Core.makeSDK";
    }
}