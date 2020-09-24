using Newtonsoft.Json;
using System.Drawing;

namespace Applitools.VisualGrid.Model
{
    public class DeviceSize
    {
        [JsonProperty("portrait")]
        public Size? Portrait { get; set; }

        [JsonProperty("landscapeLeft")]
        public Size? LandscapeLeft { get; set; }
        
        [JsonProperty("landscapeRight")]
        public Size? LandscapeRight { get; set; }

        [JsonProperty("landscape")]
        public Size? Landscape { get; set; }
    }
}