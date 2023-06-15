using System;
using Newtonsoft.Json;

namespace Applitools.Metadata
{

    public partial class ExpectedAppOutput
    {
        [JsonProperty("tag")]
        public string Tag { get; set; }

        [JsonProperty("image")]
        public ImageIdentifier Image { get; set; }

        [JsonProperty("thumbprint")]
        public ImageIdentifier Thumbprint { get; set; }

        [JsonProperty("occurredAt")]
        public DateTime OccurredAt { get; set; }

        [JsonProperty("annotations")]
        public Annotations Annotations { get; set; }
    }
}