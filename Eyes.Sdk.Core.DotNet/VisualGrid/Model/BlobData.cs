using Newtonsoft.Json;
using System;

namespace Applitools.VisualGrid.Model
{
    public class BlobData
    {
        [JsonProperty("url")]
        public Uri Url { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("value")]
        public string Value { get; set; }

        [JsonProperty("errorStatusCode")]
        public int? ErrorStatusCode { get; set; }

        public override string ToString()
        {
            return Url.ToString();
        }
    }

}