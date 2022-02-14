using Newtonsoft.Json;
using Applitools.Utils.Geometry;

namespace Applitools.Metadata
{
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
    public partial class Annotations
    {
        [JsonProperty("floating")]
        public FloatingMatchSettings[] Floating { get; set; }

        [JsonProperty("ignore")]
        public Region[] Ignore { get; set; }

        [JsonProperty("strict")]
        public Region[] Strict { get; set; }

        [JsonProperty("content")]
        public Region[] Content { get; set; }

        [JsonProperty("layout")]
        public Region[] Layout { get; set; }
    }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
}