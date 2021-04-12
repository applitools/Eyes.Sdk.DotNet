using Applitools.Utils.Geometry;
using Applitools.VisualGrid;
using System;
using System.Collections.Generic;
using System.Net;

namespace Applitools.Selenium
{
    public class Configuration : Applitools.Configuration, IConfiguration
    {
        private static readonly int DEFAULT_WAIT_BEFORE_SCREENSHOTS = 100;
        private readonly List<RenderBrowserInfo> browsersInfo_ = new List<RenderBrowserInfo>();

        public Configuration() { }

        public Configuration(IConfiguration configuration) : base(configuration)
        {
            IsForceFullPageScreenshot = configuration.IsForceFullPageScreenshot;
            WaitBeforeScreenshots = configuration.WaitBeforeScreenshots;
            StitchMode = configuration.StitchMode;
            HideScrollbars = configuration.HideScrollbars;
            HideCaret = configuration.HideCaret;
            DisableBrowserFetching = configuration.DisableBrowserFetching;
            VisualGridOptions = (VisualGridOption[])configuration.VisualGridOptions?.Clone();
            UseCookies = configuration.UseCookies;
            if (configuration is Configuration config)
            {
                browsersInfo_.AddRange(config.browsersInfo_);
            }
        }

        public bool? IsForceFullPageScreenshot { get; set; } = null;

        public IConfiguration SetForceFullPageScreenshot(bool value)
        {
            IsForceFullPageScreenshot = value;
            return this;
        }

        public int WaitBeforeScreenshots { get; set; } = DEFAULT_WAIT_BEFORE_SCREENSHOTS;

        public IConfiguration SetWaitBeforeScreenshots(int value)
        {
            WaitBeforeScreenshots = (value <= 0) ? DEFAULT_WAIT_BEFORE_SCREENSHOTS : value;
            return this;
        }

        public StitchModes StitchMode { get; set; } = StitchModes.Scroll;

        public IConfiguration SetStitchMode(StitchModes value)
        {
            StitchMode = value;
            return this;
        }


        public bool HideScrollbars { get; set; } = true;

        public IConfiguration SetHideScrollbars(bool value)
        {
            HideScrollbars = value;
            return this;
        }

        public bool HideCaret { get; set; } = true;

        public IConfiguration SetHideCaret(bool value)
        {
            HideCaret = value;
            return this;
        }


        public bool DisableBrowserFetching { get; set; } = false;

        public IConfiguration SetDisableBrowserFetching(bool value)
        {
            DisableBrowserFetching = value;
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
            }
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

        public VisualGridOption[] VisualGridOptions { get; set; }

        public IConfiguration SetVisualGridOptions(params VisualGridOption[] options)
        {
            VisualGridOptions = (VisualGridOption[])options?.Clone();
            return this;
        }


        public bool UseCookies { get; set; } = true;

        public IConfiguration SetUseCookies(bool useCookies)
        {
            UseCookies = useCookies;
            return this;
        }

        #region override setters

        public new IConfiguration SetAgentId(string value) => (IConfiguration)base.SetAgentId(value);
        public new IConfiguration SetAppName(string value) => (IConfiguration)base.SetAppName(value);
        public new IConfiguration SetBaselineBranchName(string value) => (IConfiguration)base.SetBaselineBranchName(value);
        public new IConfiguration SetBaselineEnvName(string value) => (IConfiguration)base.SetBaselineEnvName(value);
        public new IConfiguration SetBatch(BatchInfo value) => (IConfiguration)base.SetBatch(value);
        public new IConfiguration SetBranchName(string value) => (IConfiguration)base.SetBranchName(value);
        public new IConfiguration SetDefaultMatchSettings(ImageMatchSettings value) => (IConfiguration)base.SetDefaultMatchSettings(value);
        public new IConfiguration SetEnvironmentName(string value) => (IConfiguration)base.SetEnvironmentName(value);
        public new IConfiguration SetHostApp(string value) => (IConfiguration)base.SetHostApp(value);
        public new IConfiguration SetHostOS(string value) => (IConfiguration)base.SetHostOS(value);
        public new IConfiguration SetIgnoreCaret(bool value) => (IConfiguration)base.SetIgnoreCaret(value);
        public new IConfiguration SetMatchTimeout(TimeSpan value) => (IConfiguration)base.SetMatchTimeout(value);
        public new IConfiguration SetParentBranchName(string value) => (IConfiguration)base.SetParentBranchName(value);
        public new IConfiguration SetSaveDiffs(bool? value) => (IConfiguration)base.SetSaveDiffs(value);
        public new IConfiguration SetSaveNewTests(bool value) => (IConfiguration)base.SetSaveNewTests(value);
        public new IConfiguration SetSaveFailedTests(bool value) => (IConfiguration)base.SetSaveFailedTests(value);
        public new IConfiguration SetSendDom(bool value) => (IConfiguration)base.SetSendDom(value);
        public new IConfiguration SetStitchOverlap(int value) => (IConfiguration)base.SetStitchOverlap(value);
        public new IConfiguration SetTestName(string value) => (IConfiguration)base.SetTestName(value);
        public new IConfiguration SetViewportSize(int width, int height) => (IConfiguration)base.SetViewportSize(width, height);
        public new IConfiguration SetViewportSize(RectangleSize value) => (IConfiguration)base.SetViewportSize(value);
        public new IConfiguration SetMatchLevel(MatchLevel value) => (IConfiguration)base.SetMatchLevel(value);
        public new IConfiguration SetServerUrl(string value) => (IConfiguration)base.SetServerUrl(value);
        public new IConfiguration SetApiKey(string value) => (IConfiguration)base.SetApiKey(value);
        public new IConfiguration SetProxy(WebProxy value) => (IConfiguration)base.SetProxy(value);
        public new IConfiguration SetUseDom(bool value) => (IConfiguration)base.SetUseDom(value);
        public new IConfiguration SetEnablePatterns(bool value) => (IConfiguration)base.SetEnablePatterns(value);
        public new IConfiguration SetIgnoreDisplacements(bool value) => (IConfiguration)base.SetIgnoreDisplacements(value);
        public new IConfiguration SetAccessibilityValidation(AccessibilitySettings value) => (IConfiguration)base.SetAccessibilityValidation(value);
        public new IConfiguration SetAbortIdleTestTimeout(int? value) => (IConfiguration)base.SetAbortIdleTestTimeout(value);
        public new IConfiguration SetLayoutBreakpointsEnabled(bool value) => (IConfiguration)base.SetLayoutBreakpointsEnabled(value);
        public new IConfiguration SetLayoutBreakpoints(params int[] value) => (IConfiguration)base.SetLayoutBreakpoints(value);
        #endregion


        public override Applitools.Configuration Clone()
        {
            return new Configuration(this);
        }
    }
}
