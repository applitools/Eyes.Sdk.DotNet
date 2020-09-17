using System;
using Newtonsoft.Json;

namespace Applitools.Metadata
{
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
    public partial class SessionResults
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("revision")]
        public long Revision { get; set; }

        [JsonProperty("runningSessionId")]
        public string RunningSessionId { get; set; }

        [JsonProperty("isAborted")]
        public bool IsAborted { get; set; }

        [JsonProperty("isStarred")]
        public bool IsStarred { get; set; }

        [JsonProperty("startInfo")]
        public StartInfo StartInfo { get; set; }

        [JsonProperty("batchId")]
        public string BatchId { get; set; }

        [JsonProperty("secretToken")]
        public string SecretToken { get; set; }

        [JsonProperty("state")]
        public string State { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("isDefaultStatus")]
        public bool IsDefaultStatus { get; set; }

        [JsonProperty("startedAt")]
        public DateTime StartedAt { get; set; }

        [JsonProperty("duration")]
        public long Duration { get; set; }

        [JsonProperty("isDifferent")]
        public bool IsDifferent { get; set; }

        [JsonProperty("env")]
        public BaselineEnv Env { get; set; }

        [JsonProperty("branch")]
        public Branch Branch { get; set; }

        [JsonProperty("expectedAppOutput")]
        public ExpectedAppOutput[] ExpectedAppOutput { get; set; }

        [JsonProperty("actualAppOutput")]
        public ActualAppOutput[] ActualAppOutput { get; set; }

        [JsonProperty("baselineId")]
        public string BaselineId { get; set; }

        [JsonProperty("scenarioId")]
        public string ScenarioId { get; set; }

        [JsonProperty("scenarioName")]
        public string ScenarioName { get; set; }

        [JsonProperty("appId")]
        public string AppId { get; set; }

        [JsonProperty("baselineModelId")]
        public string BaselineModelId { get; set; }

        [JsonProperty("baselineEnvId")]
        public string BaselineEnvId { get; set; }

        [JsonProperty("baselineEnv")]
        public BaselineEnv BaselineEnv { get; set; }

        [JsonProperty("appName")]
        public string AppName { get; set; }

        [JsonProperty("baselineBranchName")]
        public string BaselineBranchName { get; set; }

        [JsonProperty("isNew")]
        public bool IsNew { get; set; }
    }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
}
