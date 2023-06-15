using System;
using System.Net;
using Applitools.Metadata;

namespace Applitools
{
    public class OpenSettings
    {
        public Uri ServerUrl { get; set; }
        public string ApiKey { get; set; }
        public WebProxy Proxy { get; set; }
        public int? ConnectionTimeout { get; set; }
        public bool? RemoveSession { get; set; }
        public string AgentId { get; set; }
        public string AppName { get; set; }
        public string TestName { get; set; }
        public string DisplayName { get; set; }
        public string UserTestId { get; set; }
        public string SessionType { get; set; }
        public PropertiesCollection Properties { get; set; }
        public BatchInfo Batch { get; set; }
        public bool? KeepBatchOpen { get; set; }
        public string EnvironmentName { get; set; }
        public BaselineEnv Environment { get; set; }
        public string BranchName { get; set; }
        public string ParentBranchName { get; set; }
        public string BaselineEnvName { get; set; }
        public string BaselineBranchName { get; set; }
        public bool? CompareWithParentBranch { get; set; }
        public bool? IgnoreGitBranching { get; set; }
        public bool? SaveDiffs { get; set; }
        public int? AbortIdleTestTimeout { get; set; }

        public OpenSettings()
        {
            Properties = new PropertiesCollection();
        }
    }
}