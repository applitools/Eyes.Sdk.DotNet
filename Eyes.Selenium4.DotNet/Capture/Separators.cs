using Newtonsoft.Json;

namespace Applitools.Selenium.Capture
{
    public class Separators
    {
        [JsonProperty("separator")]
        public string Separator { get; set; }
        [JsonProperty("cssStartToken")]
        public string CssStartToken{ get; set; }
        [JsonProperty("cssEndToken")]
        public string CssEndToken { get; set; }
        [JsonProperty("iframeStartToken")]
        public string IFrameStartToken { get; set; }
        [JsonProperty("iframeEndToken")]
        public string IFrameEndToken { get; set; }

    }
}
