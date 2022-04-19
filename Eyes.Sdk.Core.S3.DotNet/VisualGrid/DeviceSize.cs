using Applitools.Utils.Geometry;
using Newtonsoft.Json;

namespace Applitools.VisualGrid.Model
{
    public class DeviceSize
    {
        public DeviceSize() { }
        public DeviceSize (int width, int height)
        {
            Portrait = new RectangleSize(height, width);
            Landscape = LandscapeLeft = LandscapeRight = new RectangleSize(width, height);
        }

        [JsonProperty("portrait")]
        public RectangleSize Portrait { get; set; }

        [JsonProperty("landscapeLeft")]
        public RectangleSize LandscapeLeft { get; set; }

        [JsonProperty("landscapeRight")]
        public RectangleSize LandscapeRight { get; set; }

        [JsonProperty("landscape")]
        public RectangleSize Landscape { get; set; }
    }
}