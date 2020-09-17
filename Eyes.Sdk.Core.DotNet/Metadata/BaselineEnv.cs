using Applitools.Utils.Geometry;
using Newtonsoft.Json;

namespace Applitools.Metadata
{
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
    public partial class BaselineEnv
    {
        [JsonProperty("inferred")]
        public string Inferred { get; set; }

        [JsonProperty("os")]
        public string Os { get; set; }

        [JsonProperty("hostingApp")]
        public string HostingApp { get; set; }

        [JsonProperty("hostingAppInfo")]
        public string HostingAppInfo { get; set; }

        [JsonProperty("deviceInfo")]
        public string DeviceInfo { get; set; }

        [JsonProperty("displaySize")]
        public RectangleSize DisplaySize { get; set; }
    }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
}