using Newtonsoft.Json;

namespace Eyes.Universal.Sdk.DotNet.Commands.Client
{
    [JsonObject(Title = "EyesManage.closeAllEyes")]
    public class EyesManageCloseAllEyes
    {
        [JsonProperty("manager")]
        public object Manager { get; set; }
    }
}