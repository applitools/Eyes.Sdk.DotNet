using Applitools.Utils.Geometry;
using Newtonsoft.Json;

namespace Applitools.Metadata
{
    public class PageCoverageInfo
    {
        [JsonProperty("pageId")] 
        public string PageId { get; set; }

        [JsonProperty("width")] 
        public long Width { get; set; }

        [JsonProperty("height")] 
        public long Height { get; set; }

        [JsonProperty("imagePositionInPage")] 
        public Location Location { get; set; }
    }
}