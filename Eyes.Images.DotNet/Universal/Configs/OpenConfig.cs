using Applitools.Metadata;
using Applitools.Utils;

namespace Applitools
{
    public interface IOpenConfig
    {
        string ServerUrl { get; }
        string ApiKey { get; }
        ProxySettings Proxy { get; }
        int? ConnectionTimeout { get; }
        bool? RemoveSession { get; set; }
        string AgentId { get; }
        string AppName { get; }
        string TestName { get; }
        string DisplayName { get; }
        string UserTestId { get; }
        string SessionType { get; }
        PropertiesCollection Properties { get; set; }
        BatchInfo Batch { get; }
        bool? KeepBatchOpen { get; set; }
        string EnvironmentName { get; }
        BaselineEnv Environment { get; }
        string BranchName { get; }
        string ParentBranchName { get; }
        string BaselineBranchName { get; }
        string BaselineEnvName { get; }
        bool? CompareWithParentBranch { get; }
        bool? IgnoreBaseline { get; }
        bool? IgnoreGitBranching { get; }
        bool? SaveDiffs { get; }
        int? AbortIdleTestTimeout { get; }
    }

    public class OpenConfig : IOpenConfig
    {
        public string ServerUrl { get; set; }
        public string ApiKey { get; set; }
        public ProxySettings Proxy { get; set; }
        public int? ConnectionTimeout { get; set; }
        public bool? RemoveSession { get; set; }
        public string AgentId { get; set; }
        public string AppName { get; set; }
        public string TestName { get; set; }
        public string DisplayName { get; set; }
        public string UserTestId { get; set; }
        public string SessionType { get; set; }
        public PropertiesCollection Properties { get; set; } = new PropertiesCollection();
        public BatchInfo Batch { get; set; }
        public bool? KeepBatchOpen { get; set; }
        public string EnvironmentName { get; set; }
        public BaselineEnv Environment { get; set; }
        public string BranchName { get; set; } = CommonUtils.GetEnvVar("APPLITOOLS_BRANCH");
        public string ParentBranchName { get; set; } = CommonUtils.GetEnvVar("APPLITOOLS_PARENT_BRANCH");
        public string BaselineBranchName { get; set; } = CommonUtils.GetEnvVar("APPLITOOLS_BASELINE_BRANCH");
        public string BaselineEnvName { get; set; }
        public bool? CompareWithParentBranch { get; set; }
        public bool? IgnoreBaseline { get; set; }
        public bool? IgnoreGitBranching { get; set; }
        public bool? SaveDiffs { get; set; }
        public int? AbortIdleTestTimeout { get; set; }
    }
}