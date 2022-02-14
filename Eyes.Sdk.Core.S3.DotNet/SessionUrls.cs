using Newtonsoft.Json;
using System;

namespace Applitools
{
    public class SessionUrls
    {
        [JsonProperty("batch")]
        public string Batch { get; set; }

        [JsonProperty("session")]
        public string Session { get; set; }
    }
}
