using System;
using Applitools.Utils;
using Applitools.Utils.Geometry;

namespace Applitools
{
    public abstract class EyesBaseConfig
    {
        protected Configuration Config { get; set; }

        public string ApiKey
        {
            get => Config.ApiKey;
            set => Config.SetApiKey(value);
        }

        public string TestName
        {
            get => Config.TestName;
            set => Config.SetTestName(value);
        }

        public string AppName
        {
            get => Config.AppName;
            set => Config.SetAppName(value);
        }

        public RectangleSize ViewportSize
        {
            get => Config.ViewportSize;
            set => Config.SetViewportSize(value);
        }

        public string ServerUrl
        {
            get => Config.ServerUrl;
            set => Config.SetServerUrl(value);
        }

        public ProxySettings Proxy
        {
            get => Config.Proxy;
            set => Config.SetProxy(value);
        }

        #region configuration properties

        public StitchModes? StitchMode
        {
            get => Config.StitchMode;
            set => Config.SetStitchMode(value);
        }

        public bool? DontCloseBatches
        {
            get => Config.DontCloseBatches;
            set => Config.SetDontCloseBatches(value);
        }

        public int? WaitBeforeScreenshots
        {
            get => Config.WaitBeforeScreenshots;
            set => Config.SetWaitBeforeScreenshots(value);
        }

        public bool? ForceFullPageScreenshot
        {
            get => Config.IsForceFullPageScreenshot;
            set => Config.SetForceFullPageScreenshot(value);
        }

        public bool? HideScrollbars
        {
            get => Config.HideScrollbars;
            set => Config.SetHideScrollbars(value);
        }

        public bool? HideCaret
        {
            get => Config.HideCaret;
            set => Config.SetHideCaret(value);
        }

        public bool? SaveNewTests
        {
            get => Config.SaveNewTests;
            set => Config.SetSaveNewTests(value);
        }

        public string HostOS
        {
            get => Config.HostOS;
            set => Config.SetHostOS(value);
        }

        public string BranchName
        {
            get => Config.BranchName;
            set => Config.SetBranchName(value);
        }

        public string ParentBranchName
        {
            get => Config.ParentBranchName;
            set => Config.SetParentBranchName(value);
        }

        public BatchInfo Batch
        {
            get => Config.Batch;
            set => Config.SetBatch(value);
        }
        
        public string BaselineBranchName
        {
            get => Config.BaselineBranchName;
            set => Config.SetBaselineBranchName(value);
        }
        
        public MatchLevel? MatchLevel
        {
            get => Config.MatchLevel;
            set => Config.SetMatchLevel(value);
        }
        
        public bool? IgnoreCaret
        {
            get => Config.IgnoreCaret;
            set => Config.SetIgnoreCaret(value);
        }
        
        public bool? SaveFailedTests
        {
            get => Config.SaveFailedTests;
            set => Config.SetSaveFailedTests(value);
        }
        
        public bool? SendDom
        {
            get => Config.SendDom;
            set => Config.SetSendDom(value);
        }

        public bool? SaveDiffs
        {
            get => Config.SaveDiffs;
            set => Config.SetSaveDiffs(value);
        }

        public int? StitchOverlap
        {
            get => Config.StitchOverlap;
            set => Config.SetStitchOverlap(value);
        }

        public TimeSpan? MatchTimeout
        {
            get => Config.MatchTimeout;
            set => Config.SetMatchTimeout(value);
        }
        
        public string AgentId
        {
            get => Config.AgentId;
            set => Config.SetAgentId(value);
        }

        public string FullAgentId
        {
            get => AgentId;
            set => AgentId = value;
        }
        
        public string BaselineEnvName
        {
            get => Config.BaselineEnvName;
            set => Config.SetBaselineEnvName(value);
        }

        public string EnvironmentName
        {
            get => Config.EnvironmentName;
            set => Config.SetEnvironmentName(value);
        }

        public string HostApp
        {
            get => Config.HostApp;
            set => Config.SetHostApp(value);
        }

        public Logger Logger { get; }

        #endregion

        public virtual bool IsDisabled { get; set; }

        public bool IsOpen { get; protected set; }

        protected EyesBaseConfig(Configuration configuration)
        {
            Config = configuration ?? new Configuration();
            Logger = new Logger();
        }

        public void SetConfiguration(IConfiguration configuration)
        {
            ArgumentGuard.NotNull(configuration, nameof(configuration));

            string serverUrl = configuration.ServerUrl;
            if (serverUrl != null)
            {
                ServerUrl = serverUrl;
            }

            string apiKey = configuration.ApiKey;
            if (apiKey != null)
            {
                ApiKey = apiKey;
            }

            ProxySettings proxy = configuration.Proxy;
            if (proxy != null)
            {
                Proxy = proxy;
            }
        }
    }
}
