using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Applitools.VisualGrid.Model
{
    public class FrameData
    {
        [JsonProperty("cdt")]
        public List<CdtData> Cdt { get; set; }

        [JsonProperty("url")]
        public Uri Url { get; set; }

        [JsonProperty("resourceUrls")]
        public List<Uri> ResourceUrls { get; set; }

        [JsonProperty("blobs")]
        public List<BlobData> Blobs { get; set; }

        [JsonProperty("frames")]
        public List<FrameData> Frames { get; set; }

        [JsonProperty("srcAttr")]
        public Uri SrcAttr { get; set; }

        public override string ToString()
        {
            return $"src='{SrcAttr}' ; url='{Url}' ; sub frames: {Frames?.Count ?? 0} ; resource urls: {ResourceUrls?.Count ?? 0} ; blobs: {Blobs?.Count ?? 0}";
        }
    }
}
