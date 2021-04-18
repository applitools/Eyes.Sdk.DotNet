using Applitools.Utils;
using Applitools.Utils.Geometry;
using System;
using System.Collections.Generic;
using System.Net;

namespace Applitools
{
    public class Configuration : IConfiguration
    {
        private static readonly TimeSpan DEFAULT_MATCH_TIMEOUT = TimeSpan.FromSeconds(2);

        public Configuration()
        {
            DefaultMatchSettings.IgnoreCaret = true;
            SetAgentId(null);
            SetSaveNewTests(true); // New tests are automatically saved by default.
        }

        public Configuration(IConfiguration configuration)
        {
            AgentId = configuration.AgentId;
            ApiKey = configuration.ApiKey;
            AppName = configuration.AppName;
            BaselineBranchName = configuration.BaselineBranchName;
            BaselineEnvName = configuration.BaselineEnvName;
            Batch = configuration.Batch;
            BranchName = configuration.BranchName;
            DefaultMatchSettings = configuration.DefaultMatchSettings?.Clone();
            EnvironmentName = configuration.EnvironmentName;
            HostApp = configuration.HostApp;
            HostOS = configuration.HostOS;
            IgnoreDisplacements = configuration.IgnoreDisplacements;
            MatchTimeout = configuration.MatchTimeout;
            ParentBranchName = configuration.ParentBranchName;
            SaveDiffs = configuration.SaveDiffs;
            SaveNewTests = configuration.SaveNewTests;
            SaveFailedTests = configuration.SaveFailedTests;
            SendDom = configuration.SendDom;
            ServerUrl = configuration.ServerUrl;
            Proxy = configuration.Proxy;
            StitchOverlap = configuration.StitchOverlap;
            TestName = configuration.TestName;
            ViewportSize = configuration.ViewportSize;
            AbortIdleTestTimeout = configuration.AbortIdleTestTimeout;
            LayoutBreakpointsEnabled = configuration.LayoutBreakpointsEnabled;
            layoutBreakpoints_ = new List<int>(configuration.LayoutBreakpoints);
        }


        /// <summary>
        /// Get the batch in which context future tests will run or <c>null</c>
        /// if tests are to run standalone.
        /// </summary>
        public BatchInfo Batch { get; set; }

        /// <summary>
        /// Sets the batch in which context future tests will run or <c>null</c>
        /// if tests are to run standalone.
        /// </summary>
        public IConfiguration SetBatch(BatchInfo value)
        {
            Batch = value;
            return this;
        }

        /// <summary>
        /// Gets the branch in which the baseline for subsequent test runs resides.
        /// If the branch does not already exist it will be created under the
        /// specified parent branch <see cref="ParentBranchName"/>.
        /// Changes made to the baseline or model of a branch do not propagate to other
        /// branches.
        /// Use <c>null</c> to try getting the branch name from the <code>APPLITOOLS_BRANCH</code> environment variable.
        /// If that variable doesn't exists, then the default branch will be used.
        /// </summary>
        public string BranchName { get; set; } = CommonUtils.GetEnvVar("APPLITOOLS_BRANCH");

        /// <summary>
        /// Sets the branch in which the baseline for subsequent test runs resides.
        /// If the branch does not already exist it will be created under the
        /// specified parent branch <see cref="ParentBranchName"/>.
        /// Changes made to the baseline or model of a branch do not propagate to other
        /// branches.
        /// Use <c>null</c> to try getting the branch name from the <code>APPLITOOLS_BRANCH</code> environment variable.
        /// If that variable doesn't exists, then the default branch will be used.
        /// </summary>
        public IConfiguration SetBranchName(string value)
        {
            BranchName = value;
            return this;
        }

        /// <summary>
        /// Gets the branch under which new branches are created.
        /// Use <c>null</c> to try getting the branch name from the <code>APPLITOOLS_PARENT_BRANCH</code> environment variable.
        /// If that variable doesn't exists, then the default branch will be used.
        /// </summary>
        public string ParentBranchName { get; set; } = CommonUtils.GetEnvVar("APPLITOOLS_PARENT_BRANCH");

        /// <summary>
        /// Sets the branch under which new branches are created.
        /// Use <c>null</c> to try getting the branch name from the <code>APPLITOOLS_PARENT_BRANCH</code> environment variable.
        /// If that variable doesn't exists, then the default branch will be used.
        /// </summary>
        public IConfiguration SetParentBranchName(string value)
        {
            ParentBranchName = value;
            return this;
        }

        /// <summary>
        /// Gets the baseline branch under which new branches are created.
        /// Use <c>null</c> to try getting the branch name from the <code>APPLITOOLS_BASELINE_BRANCH</code> environment variable.
        /// If that variable doesn't exists, then the default branch will be used.
        /// </summary>
        public string BaselineBranchName { get; set; } = CommonUtils.GetEnvVar("APPLITOOLS_BASELINE_BRANCH");

        /// <summary>
        /// Sets the baseline branch under which new branches are created.
        /// Use <c>null</c> to try getting the branch name from the <code>APPLITOOLS_BASELINE_BRANCH</code> environment variable.
        /// If that variable doesn't exists, then the default branch will be used.
        /// </summary>
        public IConfiguration SetBaselineBranchName(string value)
        {
            BaselineBranchName = value;
            return this;
        }

        /// <summary>
        /// Gets this agent's id.
        /// </summary>
        public string AgentId { get; set; }

        /// <summary>
        /// Sets this agent's id.
        /// </summary>
        public IConfiguration SetAgentId(string value)
        {
            AgentId = value;
            return this;
        }

        /// <summary>
        /// If not <c>null</c> determines the name of the environment of the baseline 
        /// to compare with.
        /// </summary>
        public string BaselineEnvName { get; set; }

        /// <summary>
        /// If not <c>null</c> determines the name of the environment of the baseline 
        /// to compare with.
        /// </summary>
        public IConfiguration SetBaselineEnvName(string value)
        {
            BaselineEnvName = value;
            return this;
        }

        /// <summary>
        /// If not <c>null</c> specifies a name for the environment in which the 
        /// application under test is running.
        /// </summary>
        public string EnvironmentName { get; set; }

        /// <summary>
        /// If not <c>null</c> specifies a name for the environment in which the 
        /// application under test is running.
        /// </summary>
        public IConfiguration SetEnvironmentName(string value)
        {
            EnvironmentName = value;
            return this;
        }


        /// <summary>
        /// Automatically save differences as a baseline.
        /// </summary>
        public bool? SaveDiffs { get; set; }

        /// <summary>
        /// Automatically save differences as a baseline.
        /// </summary>
        public IConfiguration SetSaveDiffs(bool? value)
        {
            SaveDiffs = value;
            return this;
        }

        public bool SaveNewTests { get; set; }

        public IConfiguration SetSaveNewTests(bool value)
        {
            SaveNewTests = value;
            return this;
        }

        public bool SaveFailedTests { get; set; }

        public IConfiguration SetSaveFailedTests(bool value)
        {
            SaveFailedTests = value;
            return this;
        }

        public string AppName { get; set; }

        public IConfiguration SetAppName(string value)
        {
            AppName = value;
            return this;
        }


        public string TestName { get; set; }

        public IConfiguration SetTestName(string value)
        {
            TestName = value;
            return this;
        }

        public RectangleSize ViewportSize { get; set; } = new RectangleSize();

        public IConfiguration SetViewportSize(int width, int height)
        {
            ViewportSize = new RectangleSize(width, height);
            return this;
        }

        public IConfiguration SetViewportSize(RectangleSize value)
        {
            ViewportSize = value;
            return this;
        }


        public ImageMatchSettings DefaultMatchSettings { get; set; } = new ImageMatchSettings(MatchLevel.Strict) { IgnoreCaret = true };

        public IConfiguration SetDefaultMatchSettings(ImageMatchSettings value)
        {
            DefaultMatchSettings = value;
            return this;
        }


        public int StitchOverlap { get; set; } = 10;

        public IConfiguration SetStitchOverlap(int value)
        {
            StitchOverlap = value;
            return this;
        }


        public bool SendDom { get; set; } = true;

        public IConfiguration SetSendDom(bool value)
        {
            SendDom = value;
            return this;
        }


        public TimeSpan MatchTimeout { get; set; } = DEFAULT_MATCH_TIMEOUT;

        public IConfiguration SetMatchTimeout(TimeSpan value)
        {
            MatchTimeout = value;
            return this;
        }


        public string HostApp { get; set; }

        public IConfiguration SetHostApp(string value)
        {
            HostApp = value;
            return this;
        }


        public string HostOS { get; set; }

        public IConfiguration SetHostOS(string value)
        {
            HostOS = value;
            return this;
        }


        public bool IgnoreCaret
        {
            get => DefaultMatchSettings.IgnoreCaret ?? true;
            set => SetIgnoreCaret(value);
        }

        public IConfiguration SetIgnoreCaret(bool value)
        {
            DefaultMatchSettings.IgnoreCaret = value;
            return this;
        }


        public MatchLevel MatchLevel
        {
            get => DefaultMatchSettings.MatchLevel ?? MatchLevel.Strict;
            set => SetMatchLevel(value);
        }

        public IConfiguration SetMatchLevel(MatchLevel value)
        {
            DefaultMatchSettings.MatchLevel = value;
            return this;
        }

        
        public string ServerUrl { get; set; }

        public IConfiguration SetServerUrl(string value)
        {
            ServerUrl = value;
            return this;
        }


        public string ApiKey { get; set; }

        public IConfiguration SetApiKey(string value)
        {
            ApiKey = value;
            return this;
        }


        public WebProxy Proxy { get; set; }

        public IConfiguration SetProxy(WebProxy proxy)
        {
            Proxy = proxy;
            return this;
        }

        public bool UseDom
        {
            get => DefaultMatchSettings.UseDom;
            set => SetUseDom(value);
        }

        /// <summary>
        /// Use the page DOM when computing the layout of the page.
        /// </summary>
        public IConfiguration SetUseDom(bool value)
        {
            DefaultMatchSettings.UseDom = value;
            return this;
        }


        public bool EnablePatterns
        {
            get => DefaultMatchSettings.EnablePatterns;
            set => SetEnablePatterns(value);
        }

        public IConfiguration SetEnablePatterns(bool value)
        {
            DefaultMatchSettings.EnablePatterns = value;
            return this;
        }

        public bool IgnoreDisplacements { get; set; }


        public IConfiguration SetIgnoreDisplacements(bool value)
        {
            IgnoreDisplacements = value;
            return this;
        }


        public AccessibilitySettings AccessibilityValidation
        {
            get => DefaultMatchSettings.AccessibilitySettings;
            set => SetAccessibilityValidation(value);
        }

        public IConfiguration SetAccessibilityValidation(AccessibilitySettings value)
        {
            DefaultMatchSettings.AccessibilitySettings = value;
            return this;
        }

        /// <summary>
        /// Sets and gets optional timeout in seconds. 
        /// </summary>
        public int? AbortIdleTestTimeout { get; set; } = null;

        /// <summary>
        /// Set optional server timeout in seconds for handling test.
        /// </summary>
        /// <param name="value">Timeout in seconds.</param>
        /// <returns>This configuration object.</returns>
        public IConfiguration SetAbortIdleTestTimeout(int? value)
        {
            AbortIdleTestTimeout = value;
            return this;
        }

        public IConfiguration SetLayoutBreakpointsEnabled(bool shouldSet)
        {
            LayoutBreakpointsEnabled = shouldSet;
            LayoutBreakpoints.Clear();
            return this;
        }

        public bool LayoutBreakpointsEnabled { get; set; }

        public IConfiguration SetLayoutBreakpoints(params int[] breakpoints)
        {
            return SetLayoutBreakpoints_(breakpoints);
        }

        private IConfiguration SetLayoutBreakpoints_(IList<int> breakpoints)
        {
            LayoutBreakpointsEnabled = false;
            LayoutBreakpoints.Clear();
            if (breakpoints == null || breakpoints.Count == 0)
            {
                return this;
            }

            foreach (int breakpoint in breakpoints)
            {
                ArgumentGuard.GreaterThan(breakpoint, 0, nameof(breakpoint));
                LayoutBreakpoints.Add(breakpoint);
            }

            ((List<int>)LayoutBreakpoints).Sort();
            return this;
        }

        private readonly IList<int> layoutBreakpoints_ = new List<int>();
        public IList<int> LayoutBreakpoints
        {
            get => layoutBreakpoints_;
            set => SetLayoutBreakpoints_(value);
        }

        public virtual Configuration Clone()
        {
            return new Configuration(this);
        }
    }
}
