using System;
using System.Diagnostics;
using System.Net;
using System.Reflection;
using Applitools.Utils.Geometry;

namespace Applitools
{
    public abstract class EyesBaseConfig
    {
        internal protected abstract Configuration Config { get; }
        private IServerConnector serverConnector_;
        protected Assembly actualAssembly_;

        #region configuration properties

        /// <summary>
        /// Sets the API key of your applitools Eyes account.
        /// If <c>null</c>, then the API key will be read from the <code>APPLITOOLS_API_KEY</code> environment variable.
        /// </summary>
        /// <param name="value">The API key.</param>
        public virtual string ApiKey
        {
            get => ServerConnector?.ApiKey ?? Config.ApiKey;
            set
            {
                Config.ApiKey = value;
                if (serverConnector_ != null)
                {
                    serverConnector_.ApiKey = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the Eyes server URL.
        /// </summary>
        public virtual string ServerUrl
        {
            get => ServerConnector?.ServerUrl.AbsoluteUri ?? Config.ServerUrl;
            set
            {
                Config.ServerUrl = value;
                if (serverConnector_ != null)
                {
                    serverConnector_.ServerUrl = new Uri(value);
                }
            }
        }

        /// <summary>
        /// Gets or sets the proxy used to access the Eyes server or <c>null</c> to use the system 
        /// proxy.
        /// </summary>
        public WebProxy Proxy
        {
            get => ServerConnector?.Proxy ?? Config.Proxy;
            set
            {
                Config.Proxy = value;
                if (serverConnector_ != null)
                {
                    serverConnector_.Proxy = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets this agent's id.
        /// </summary>
        public string AgentId
        {
            get => Config.AgentId;
            set => Config.SetAgentId(value);
        }

        public virtual string AppName
        {
            get => Config.AppName;
            set => Config.SetAppName(value);
        }

        public virtual string TestName
        {
            get => Config.TestName;
            set => Config.SetTestName(value);
        }

        /// <summary>
        /// Gets or sets the branch in which the baseline for subsequent test runs resides.
        /// If the branch does not already exist it will be created under the
        /// specified parent branch <see cref="ParentBranchName"/>.
        /// Changes made to the baseline or model of a branch do not propagate to other
        /// branches.
        /// Use <c>null</c> to try getting the branch name from the <code>APPLITOOLS_BRANCH</code> environment variable.
        /// If that variable doesn't exists, then the default branch will be used.
        /// </summary>
        public string BranchName
        {
            get => Config.BranchName;
            set => Config.SetBranchName(value);
        }

        /// <summary>
        /// Gets or sets the baseline branch under which new branches are created.
        /// Use <c>null</c> to try getting the branch name from the <code>APPLITOOLS_BASELINE_BRANCH</code> environment variable.
        /// If that variable doesn't exists, then the default branch will be used.
        /// </summary>
        public string BaselineBranchName
        {
            get => Config.BaselineBranchName;
            set => Config.SetBaselineBranchName(value);
        }

        /// <summary>
        /// Gets or sets the branch under which new branches are created.
        /// Use <c>null</c> to try getting the branch name from the <code>APPLITOOLS_PARENT_BRANCH</code> environment variable.
        /// If that variable doesn't exists, then the default branch will be used.
        /// </summary>
        public string ParentBranchName
        {
            get => Config.ParentBranchName;
            set => Config.SetParentBranchName(value);
        }

        /// <summary>
        /// If not <c>null</c> specifies a name for the environment in which the 
        /// application under test is running.
        /// </summary>
        public string EnvironmentName
        {
            get => Config.EnvironmentName;
            set => Config.SetEnvironmentName(value);
        }

        /// <summary>
        /// If not <c>null</c> specifies a name for the environment in which the 
        /// application under test is running.
        /// </summary>
        public string EnvName
        {
            get => Config.EnvironmentName;
            set => Config.SetEnvironmentName(value);
        }

        /// <summary>
        /// If not <c>null</c> determines the name of the environment of the baseline 
        /// to compare with.
        /// </summary>
        public string BaselineEnvName
        {
            get => Config.BaselineEnvName;
            set => Config.SetBaselineEnvName(value);
        }

        public bool SendDom
        {
            get => Config.SendDom;
            set => Config.SetSendDom(value);
        }

        public TimeSpan MatchTimeout
        {
            get => Config.MatchTimeout;
            set => Config.SetMatchTimeout(value);
        }

        /// <summary>
        /// Get or sets the batch in which context future tests will run or <c>null</c>
        /// if tests are to run standalone.
        /// </summary>
        public BatchInfo Batch
        {
            get => Config.Batch ?? new BatchInfo();
            set => Config.SetBatch(value);
        }

        /// <summary>
        /// Automatically save differences as a baseline.
        /// </summary>
        public bool? SaveDiffs
        {
            get => Config.SaveDiffs;
            set => Config.SetSaveDiffs(value);
        }

        [Obsolete("Use " + nameof(SaveDiffs) + " instead.")]
        public bool SaveFailedTests
        {
            get => Config.SaveDiffs ?? false;
            set => Config.SetSaveDiffs(value);
        }

        /// <summary>
        /// Specifies how detected mismatches are reported.
        /// </summary>
        [Obsolete]
        public FailureReports FailureReports { get; set; } = FailureReports.OnClose;

        public bool SaveNewTests
        {
            get => Config.SaveNewTests;
            set => Config.SetSaveNewTests(value);
        }

        /// <summary>
        /// Gets or sets whether or not to ignore a blinking caret.
        /// </summary>
        public bool IgnoreCaret
        {
            get => Config.IgnoreCaret;
            set => Config.SetIgnoreCaret(value);
        }

        /// <summary>
        /// Match settings to be used for the session.
        /// </summary>
        public ImageMatchSettings DefaultMatchSettings
        {
            get => Config.DefaultMatchSettings;
            set => Config.SetDefaultMatchSettings(value);
        }

        /// <summary>
        /// The test-wide match level to use when application screenshots with expected output.
        /// </summary>
        public MatchLevel MatchLevel
        {
            get => Config.MatchLevel;
            set => Config.SetMatchLevel(value);
        }

        public string HostApp
        {
            get => Config.HostApp;
            set => Config.SetHostApp(value);
        }
        public string HostOS
        {
            get => Config.HostOS;
            set => Config.SetHostOS(value);
        }

        public int StitchOverlap
        {
            get => Config.StitchOverlap;
            set => Config.SetStitchOverlap(value);
        }

        public RectangleSize ViewportSize
        {
            get => Config.ViewportSize;
            set => Config.SetViewportSize(value);
        }
        public bool UseDom
        {
            get => Config.UseDom;
            set => Config.SetUseDom(value);
        }

        public bool EnablePatterns
        {
            get => Config.EnablePatterns;
            set => Config.SetEnablePatterns(value);
        }
        protected virtual string BaseAgentId => GetBaseAgentId();

        protected FileVersionInfo GetActualAssemblyVersionInfo()
        {
            FileVersionInfo versionInfo = FileVersionInfo.GetVersionInfo(actualAssembly_.Location);
            return versionInfo;
        }

        internal string GetBaseAgentId()
        {
            FileVersionInfo versionInfo = GetActualAssemblyVersionInfo();
            return $"{versionInfo.FileDescription}/{versionInfo.ProductVersion}";
        }

        public virtual string FullAgentId
        {
            get
            {
                string agentId = Config.AgentId;
                return (agentId == null) ? BaseAgentId : $"{agentId} [{BaseAgentId}]";
            }
        }

        public IServerConnector ServerConnector
        {
            get
            {
                if (serverConnector_ != null && serverConnector_.AgentId == null)
                {
                    serverConnector_.AgentId = FullAgentId;
                }
                return serverConnector_;
            }

            set => serverConnector_ = value;
        }

        #endregion

    }
}
