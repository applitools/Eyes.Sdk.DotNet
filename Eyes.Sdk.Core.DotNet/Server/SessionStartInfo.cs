using Applitools.Utils;

namespace Applitools
{
    /// <summary>
    /// Encapsulates data required to start session using the Session API.
    /// </summary>
    public class SessionStartInfo
    {
        public SessionStartInfo(
            string agentId,
            string appIdOrName,
            string verId,
            string scenarioIdOrName,
            BatchInfo batchInfo,
            string baselineEnvName,
            object environment,
            string envName,
            ImageMatchSettings defaultMatchSettings,
            string branchName,
            string parentBranchName,
            string baselineBranchName,
            bool? saveDiffs,
            bool? render,
            PropertiesCollection properties)
        {
            ArgumentGuard.NotEmpty(agentId, nameof(agentId));
            ArgumentGuard.NotEmpty(appIdOrName, nameof(appIdOrName));
            ArgumentGuard.NotEmpty(scenarioIdOrName, nameof(scenarioIdOrName));
            ArgumentGuard.NotNull(batchInfo, nameof(batchInfo));
            ArgumentGuard.NotNull(environment, nameof(environment));
            ArgumentGuard.NotNull(defaultMatchSettings, nameof(defaultMatchSettings));

            AgentId = agentId;
            AppIdOrName = appIdOrName;
            VerId = verId;
            ScenarioIdOrName = scenarioIdOrName;
            BatchInfo = batchInfo;
            BaselineEnvName = baselineEnvName;
            Environment = environment;
            EnvironmentName = envName;
            DefaultMatchSettings = defaultMatchSettings;
            BranchName = branchName;
            ParentBranchName = parentBranchName;
            BaselineBranchName = baselineBranchName;
            SaveDiffs = saveDiffs;
            Render = render;
            Properties = properties;
        }

        public string AgentId { get; private set; }

        public string AppIdOrName { get; private set; }

        public string VerId { get; private set; }

        public string ScenarioIdOrName { get; private set; }

        public BatchInfo BatchInfo { get; private set; }

        public string BaselineEnvName { get; private set; }

        public string EnvironmentName { get; private set; }

        public object Environment { get; private set; }

        public ImageMatchSettings DefaultMatchSettings { get; private set; }

        public string BranchName { get; private set; }

        public string ParentBranchName { get; private set; }

        public string BaselineBranchName { get; private set; }

        public bool? SaveDiffs { get; private set; }

        public bool? Render { get; set; }

        public PropertiesCollection Properties { get; private set; }
    }
}
