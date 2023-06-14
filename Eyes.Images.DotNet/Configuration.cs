using Applitools.Utils;
using Applitools.Utils.Geometry;
using System;
using System.Collections.Generic;
using Applitools.Commands;
using Applitools.VisualGrid;

namespace Applitools
{
    public class Configuration : IConfiguration
    {
        private readonly List<RenderBrowserInfo> browsersInfo_ = new List<RenderBrowserInfo>();

        public Configuration()
        {
            DefaultMatchSettings = new ImageMatchSettings();
        }

        public Configuration(IConfiguration configuration) : this()
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
            IsForceFullPageScreenshot = configuration.IsForceFullPageScreenshot;
            WaitBeforeCapture = configuration.WaitBeforeScreenshots;
            StitchMode = configuration.StitchMode;
            HideCaret = configuration.HideCaret;
            UseCookies = configuration.UseCookies;
            HideScrollbars = configuration.HideScrollbars;
            DisableBrowserFetching = configuration.DisableBrowserFetching;
            LayoutBreakpoints = configuration.LayoutBreakpoints;
            VisualGridOptions = (VisualGridOption[])configuration.VisualGridOptions?.Clone();


            if (configuration is Configuration config)
            {
                browsersInfo_.AddRange(config.browsersInfo_);
            }
        }

        public string Type { get; set; }
        public int? Concurrency { get; set; }
        public bool? Legacy { get; set; }

        public bool? IsForceFullPageScreenshot { get; set; }

        public int? WaitBeforeCapture { get; set; }

        [Obsolete("Use WaitBeforeCapture")]
        public int? WaitBeforeScreenshots
        {
            get => WaitBeforeCapture;
            set => WaitBeforeCapture = value;
        }

        public StitchModes? StitchMode { get; set; }

        public bool? HideCaret { get; set; }

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

        public VisualGridOption[] VisualGridOptions { get; set; }

        public IConfiguration SetVisualGridOptions(params VisualGridOption[] options)
        {
            VisualGridOptions = (VisualGridOption[])options?.Clone();
            return this;
        }

        public bool? UseCookies { get; set; }

        public IConfiguration SetUseCookies(bool? useCookies)
        {
            UseCookies = useCookies;
            return this;
        }

        public bool? SaveNewTests { get; set; }

        public IConfiguration SetSaveNewTests(bool? value)
        {
            SaveNewTests = value;
            return this;
        }

        public bool? SaveFailedTests { get; set; }

        public IConfiguration SetSaveFailedTests(bool? value)
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

        public RectangleSize ViewportSize { get; set; }

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

        public ImageMatchSettings DefaultMatchSettings { get; set; }

        public IConfiguration SetDefaultMatchSettings(ImageMatchSettings value)
        {
            DefaultMatchSettings = value;
            return this;
        }

        public int? StitchOverlap { get; set; }

        public IConfiguration SetStitchOverlap(int? value)
        {
            StitchOverlap = value;
            return this;
        }

        public bool? SendDom { get; set; }

        public IConfiguration SetSendDom(bool? value)
        {
            SendDom = value;
            return this;
        }

        public IImageCrop Cut { get; set; }

        public IConfiguration SetCut(IImageCrop value)
        {
            Cut = value;
            return this;
        }

        public bool? DontCloseBatches { get; set; }

        public IConfiguration SetDontCloseBatches(bool? value)
        {
            DontCloseBatches = value;
            return this;
        }

        public TimeSpan? MatchTimeout { get; set; }

        public IConfiguration SetMatchTimeout(TimeSpan? value)
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

        public bool? IgnoreCaret
        {
            get => DefaultMatchSettings.IgnoreCaret;
            set => SetIgnoreCaret(value);
        }

        public IConfiguration SetIgnoreCaret(bool? value)
        {
            DefaultMatchSettings.IgnoreCaret = value;
            return this;
        }

        public bool? IgnoreDisplacements
        {
            get => DefaultMatchSettings.IgnoreDisplacements;
            set => SetIgnoreDisplacements(value);
        }

        public IConfiguration SetIgnoreDisplacements(bool? value)
        {
            DefaultMatchSettings.IgnoreDisplacements = value;
            return this;
        }

        public MatchLevel? MatchLevel
        {
            get => DefaultMatchSettings.MatchLevel;
            set => SetMatchLevel(value);
        }

        public IConfiguration SetMatchLevel(MatchLevel? value)
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

        public bool? HideScrollbars { get; set; }

        public IConfiguration SetHideScrollbars(bool? value)
        {
            HideScrollbars = value;
            return this;
        }

        public bool? DisableBrowserFetching { get; set; }

        public IConfiguration SetDisableBrowserFetching(bool? value)
        {
            DisableBrowserFetching = value;
            return this;
        }

        public ProxySettings Proxy { get; set; }

        public IConfiguration SetProxy(ProxySettings proxy)
        {
            Proxy = proxy;
            return this;
        }

        public DebugScreenshotHandler DebugScreenshotProvider { get; set; }

        public IConfiguration SetDebugScreenshotProvider(DebugScreenshotHandler debugScreenshotProvider)
        {
            DebugScreenshotProvider = debugScreenshotProvider;
            return this;
        }

        public bool? UseDom
        {
            get => DefaultMatchSettings.UseDom;
            set => SetUseDom(value);
        }

        /// <summary>
        /// Use the page DOM when computing the layout of the page.
        /// </summary>
        public IConfiguration SetUseDom(bool? value)
        {
            DefaultMatchSettings.UseDom = value;
            return this;
        }

        public IConfiguration SetForceFullPageScreenshot(bool? value)
        {
            IsForceFullPageScreenshot = value;
            return this;
        }

        [Obsolete("Use SetWaitBeforeCapture(int? value)")]
        public IConfiguration SetWaitBeforeScreenshots(int? value)
        {
            WaitBeforeCapture = value;
            return this;
        }

        public IConfiguration SetWaitBeforeCapture(int? value)
        {
            WaitBeforeCapture = value;
            return this;
        }

        public IConfiguration SetStitchMode(StitchModes? value)
        {
            StitchMode = value;
            return this;
        }

        public IConfiguration SetHideCaret(bool? value)
        {
            HideCaret = value;
            return this;
        }

        public bool? EnablePatterns
        {
            get => DefaultMatchSettings.EnablePatterns;
            set => SetEnablePatterns(value);
        }

        public IConfiguration SetEnablePatterns(bool? value)
        {
            DefaultMatchSettings.EnablePatterns = value;
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
        public int? AbortIdleTestTimeout { get; set; }

        
        public LayoutBreakpointsOptions LayoutBreakpoints { get; set; } = new LayoutBreakpointsOptions();

        public bool? LayoutBreakpointsEnabled
        {
            get => LayoutBreakpoints.IsLayoutBreakpoints();
            set => LayoutBreakpoints.Breakpoints(value);
        }

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

        public IConfiguration SetLayoutBreakpoints(LayoutBreakpointsOptions options)
        {
            LayoutBreakpoints = options;
            return this;
        }

        public IConfiguration SetLayoutBreakpointsEnabled(bool? shouldSet)
        {
            LayoutBreakpointsEnabled = shouldSet;
            return this;
        }

        public IConfiguration SetLayoutBreakpoints(bool layoutBreakpointsEnabled)
        {
            return SetLayoutBreakpointsEnabled(layoutBreakpointsEnabled);
        }

        public IConfiguration SetLayoutBreakpoints(params int[] breakpoints)
        {
            LayoutBreakpoints = new LayoutBreakpointsOptions().Breakpoints(breakpoints);
            return this;
        }

        public List<RenderBrowserInfo> GetBrowsersInfo()
        {
            return browsersInfo_;
        }

        public IConfiguration AddBrowsers(params IRenderBrowserInfo[] browsersInfo)
        {
            foreach (IRenderBrowserInfo browserInfo in browsersInfo)
            {
                if (browserInfo is DesktopBrowserInfo desktopBrowserInfo)
                {
                    AddBrowser(desktopBrowserInfo);
                }
                else if (browserInfo is ChromeEmulationInfo chromeEmulationInfo)
                {
                    AddBrowser(chromeEmulationInfo);
                }
                else if (browserInfo is IosDeviceInfo iosDeviceInfo)
                {
                    AddBrowser(iosDeviceInfo);
                }
                else if (browserInfo is AndroidDeviceInfo androidDeviceInfo)
                {
                    AddBrowser(androidDeviceInfo);
                }
            }

            return this;
        }

        public IConfiguration AddMobileDevices(params IosDeviceInfo[] iosDeviceInfos)
        {
            foreach (var iosDeviceInfo in iosDeviceInfos)
            {
                AddMobileDevice(iosDeviceInfo);
            }

            return this;
        }

        public IConfiguration AddMobileDevice(IosDeviceInfo iosDeviceInfo)
        {
            return AddBrowser(iosDeviceInfo);
        }

        public Configuration AddMobileDevices(params AndroidDeviceInfo[] androidDeviceInfos)
        {
            foreach (var androidDeviceInfo in androidDeviceInfos)
            {
                AddMobileDevice(androidDeviceInfo);
            }

            return this;
        }

        public Configuration AddMobileDevice(AndroidDeviceInfo androidDeviceInfo)
        {
            RenderBrowserInfo renderBrowserInfo = new RenderBrowserInfo(androidDeviceInfo);
            browsersInfo_.Add(renderBrowserInfo);
            return this;
        }

        public IConfiguration AddBrowser(DesktopBrowserInfo desktopBrowserInfo)
        {
            RenderBrowserInfo browserInfo = new RenderBrowserInfo(desktopBrowserInfo);
            browsersInfo_.Add(browserInfo);
            return this;
        }

        public IConfiguration AddBrowser(ChromeEmulationInfo emulationInfo)
        {
            RenderBrowserInfo browserInfo = new RenderBrowserInfo(emulationInfo);
            browsersInfo_.Add(browserInfo);
            return this;
        }

        public IConfiguration AddBrowser(IosDeviceInfo deviceInfo)
        {
            RenderBrowserInfo browserInfo = new RenderBrowserInfo(deviceInfo);
            browsersInfo_.Add(browserInfo);
            return this;
        }

        public IConfiguration AddBrowser(AndroidDeviceInfo deviceInfo)
        {
            RenderBrowserInfo browserInfo = new RenderBrowserInfo(deviceInfo);
            browsersInfo_.Add(browserInfo);
            return this;
        }

        public IConfiguration AddBrowser(int width, int height, BrowserType browserType, string baselineEnvName = null)
        {
            DesktopBrowserInfo browserInfo = new DesktopBrowserInfo(width, height, browserType, baselineEnvName);
            return AddBrowser(browserInfo);
        }

        public IConfiguration AddDeviceEmulation(DeviceName deviceName,
            ScreenOrientation screenOrientation = ScreenOrientation.Portrait,
            string baselineEnvName = null)
        {
            ChromeEmulationInfo emulationInfo = new ChromeEmulationInfo(deviceName, screenOrientation);
            return AddBrowser(emulationInfo);
        }

        public virtual Configuration Clone()
        {
            return new Configuration(this);
        }
    }
}