using Newtonsoft.Json;

namespace Applitools.Metadata
{
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
    public partial class StartInfo
    {
        [JsonProperty("sessionType")]
        public string SessionType { get; set; }

        [JsonProperty("saveDiffs")]
        public bool SaveDiffs { get; set; }

        [JsonProperty("isTransient")]
        public bool IsTransient { get; set; }

        [JsonProperty("ignoreBaseline")]
        public bool IgnoreBaseline { get; set; }

        [JsonProperty("batchInfo")]
        public BatchInfo BatchInfo { get; set; }

        [JsonProperty("environment")]
        public BaselineEnv Environment { get; set; }

        [JsonProperty("matchLevel")]
        public string MatchLevel { get; set; }

        [JsonProperty("defaultMatchSettings")]
        public ImageMatchSettings DefaultMatchSettings { get; set; }

        [JsonProperty("agentId")]
        public string AgentId { get; set; }

        [JsonProperty("properties")]
        public object[] Properties { get; set; }

        [JsonProperty("render")]
        public bool Render { get; set; }

        [JsonProperty("agentRunId")]
        public string AgentRunId { get; set; }
    }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
}
