using System;
using System.Collections.Generic;
using Applitools.Commands;
using Applitools.Utils.Geometry;
using Applitools.VisualGrid;

namespace Applitools
{
    public interface IConfiguration
    {
        string AgentId { get; set; }

        string AppName { get; set; }

        string BaselineBranchName { get; set; }

        string BaselineEnvName { get; set; }

        BatchInfo Batch { get; set; }

        string BranchName { get; set; }

        ImageMatchSettings DefaultMatchSettings { get; set; }

        string EnvironmentName { get; set; }

        string HostApp { get; set; }

        string HostOS { get; set; }

        bool? IgnoreCaret { get; set; }

        TimeSpan? MatchTimeout { get; set; }

        string ParentBranchName { get; set; }

        bool? SaveDiffs { get; set; }

        bool? SaveNewTests { get; set; }
        bool? SaveFailedTests { get; set; }

        bool? SendDom { get; set; }

        int? StitchOverlap { get; set; }

        string TestName { get; set; }

        RectangleSize ViewportSize { get; set; }

        MatchLevel? MatchLevel { get; set; }

        string ServerUrl { get; set; }

        string ApiKey { get; set; }

        bool? IgnoreDisplacements { get; set; }

        bool? UseDom { get; set; }

        bool? EnablePatterns { get; set; }

        AccessibilitySettings AccessibilityValidation { get; set; }
        int? AbortIdleTestTimeout { get; set; }

        bool? LayoutBreakpointsEnabled { get; set; }
        
        LayoutBreakpointsOptions LayoutBreakpoints { get; set; }
        
        ProxySettings Proxy { get; set; }
        bool? IsForceFullPageScreenshot { get; set; }

        int? WaitBeforeScreenshots { get; set; }

        StitchModes? StitchMode { get; set; }

        bool? HideScrollbars { get; set; }

        bool? HideCaret { get; set; }

        bool? DisableBrowserFetching { get; set; }
        IImageCrop Cut { get; set; }


        VisualGridOption[] VisualGridOptions { get; set; }

        bool? UseCookies { get; set; }
        bool? DontCloseBatches { get; set; }
        DebugScreenshotHandler DebugScreenshotProvider { get; set; }


        List<RenderBrowserInfo> GetBrowsersInfo();


        IConfiguration AddBrowsers(params IRenderBrowserInfo[] browsersInfo);
        IConfiguration AddBrowser(DesktopBrowserInfo browserInfo);
        IConfiguration AddBrowser(ChromeEmulationInfo emulationInfo);
        IConfiguration AddBrowser(IosDeviceInfo deviceInfo);
        IConfiguration AddBrowser(AndroidDeviceInfo deviceInfo);
        IConfiguration AddBrowser(int width, int height, BrowserType browserType, string baselineEnvName = null);

        IConfiguration AddDeviceEmulation(DeviceName deviceName,
            ScreenOrientation screenOrientation = ScreenOrientation.Portrait,
            string baselineEnvName = null);

        IConfiguration SetWaitBeforeScreenshots(int? waitBeforeScreenshots);

        IConfiguration SetStitchMode(StitchModes? stitchMode);

        IConfiguration SetHideScrollbars(bool? hideScreenshot);

        IConfiguration SetHideCaret(bool? hideCaret);

        IConfiguration SetDisableBrowserFetching(bool? disableBrowserFetching);

        IConfiguration SetForceFullPageScreenshot(bool? forceFullPageScreenshot);

        IConfiguration SetAgentId(string value);
        IConfiguration SetAppName(string value);
        IConfiguration SetBaselineBranchName(string value);
        IConfiguration SetBaselineEnvName(string value);
        IConfiguration SetBatch(BatchInfo value);
        IConfiguration SetBranchName(string value);
        IConfiguration SetDefaultMatchSettings(ImageMatchSettings value);
        IConfiguration SetEnvironmentName(string value);
        IConfiguration SetHostApp(string value);
        IConfiguration SetHostOS(string value);
        IConfiguration SetIgnoreCaret(bool? value);
        IConfiguration SetMatchTimeout(TimeSpan? value);
        IConfiguration SetParentBranchName(string value);
        IConfiguration SetSaveDiffs(bool? value);
        IConfiguration SetSaveNewTests(bool? value);
        IConfiguration SetSaveFailedTests(bool? value);
        IConfiguration SetSendDom(bool? value);
        IConfiguration SetStitchOverlap(int? value);
        IConfiguration SetTestName(string value);
        IConfiguration SetViewportSize(RectangleSize value);
        IConfiguration SetMatchLevel(MatchLevel? value);
        IConfiguration SetServerUrl(string value);
        IConfiguration SetApiKey(string value);
        IConfiguration SetDontCloseBatches(bool? value);
        IConfiguration SetDebugScreenshotProvider(DebugScreenshotHandler value);
        IConfiguration SetCut(IImageCrop value);

        IConfiguration SetProxy(ProxySettings value);
        IConfiguration SetIgnoreDisplacements(bool? value);
        IConfiguration SetUseDom(bool? value);
        IConfiguration SetEnablePatterns(bool? value);
        IConfiguration SetAccessibilityValidation(AccessibilitySettings value);
        IConfiguration SetAbortIdleTestTimeout(int? value);
        IConfiguration SetLayoutBreakpointsEnabled(bool? shouldSet);
        IConfiguration SetLayoutBreakpoints(params int[] breakpoints);
        IConfiguration SetLayoutBreakpoints(bool layoutBreakpointsEnabled);
        IConfiguration SetLayoutBreakpoints(LayoutBreakpointsOptions options);
        IConfiguration SetVisualGridOptions(params VisualGridOption[] options);

        IConfiguration SetUseCookies(bool? useCookies);
    }
}