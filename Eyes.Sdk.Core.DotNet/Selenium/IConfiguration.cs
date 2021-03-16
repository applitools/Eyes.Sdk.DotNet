using Applitools.Utils.Geometry;
using Applitools.VisualGrid;
using System;
using System.Collections.Generic;

namespace Applitools.Selenium
{
    public interface IConfiguration : Applitools.IConfiguration
    {
        bool? IsForceFullPageScreenshot { get; set; }

        int WaitBeforeScreenshots { get; set; }

        StitchModes StitchMode { get; set; }

        bool HideScrollbars { get; set; }

        bool HideCaret { get; set; }

        bool DisableBrowserFetching { get; set; }

        List<RenderBrowserInfo> GetBrowsersInfo();

        VisualGridOption[] VisualGridOptions { get; set; }

        IConfiguration SetWaitBeforeScreenshots(int waitBeforeScreenshots);

        IConfiguration SetStitchMode(StitchModes stitchMode);

        IConfiguration SetHideScrollbars(bool hideScreenshot);

        IConfiguration SetHideCaret(bool hideCaret);

        IConfiguration SetDisableBrowserFetching(bool disableBrowserFetching);

        IConfiguration SetForceFullPageScreenshot(bool forceFullPageScreenshot);


        IConfiguration AddBrowsers(params IRenderBrowserInfo[] browsersInfo);
        IConfiguration AddBrowser(DesktopBrowserInfo browserInfo);
        IConfiguration AddBrowser(ChromeEmulationInfo emulationInfo);
        IConfiguration AddBrowser(IosDeviceInfo deviceInfo);
        IConfiguration AddBrowser(int width, int height, BrowserType browserType, string baselineEnvName = null);

        IConfiguration AddDeviceEmulation(DeviceName deviceName,
            ScreenOrientation screenOrientation = ScreenOrientation.Portrait,
            string baselineEnvName = null);

        IConfiguration SetVisualGridOptions(params VisualGridOption[] options);
        #region override setters

        new IConfiguration SetAgentId(string value);
        new IConfiguration SetAppName(string value);
        new IConfiguration SetBaselineBranchName(string value);
        new IConfiguration SetBaselineEnvName(string value);
        new IConfiguration SetBatch(BatchInfo value);
        new IConfiguration SetBranchName(string value);
        new IConfiguration SetDefaultMatchSettings(ImageMatchSettings value);
        new IConfiguration SetEnvironmentName(string value);
        new IConfiguration SetHostApp(string value);
        new IConfiguration SetHostOS(string value);
        new IConfiguration SetIgnoreCaret(bool value);
        new IConfiguration SetMatchTimeout(TimeSpan value);
        new IConfiguration SetParentBranchName(string value);
        new IConfiguration SetSaveDiffs(bool? value);
        new IConfiguration SetSaveNewTests(bool value);
        new IConfiguration SetSaveFailedTests(bool value);
        new IConfiguration SetSendDom(bool value);
        new IConfiguration SetStitchOverlap(int value);
        new IConfiguration SetTestName(string value);
        new IConfiguration SetViewportSize(RectangleSize value);
        new IConfiguration SetMatchLevel(MatchLevel value);
        new IConfiguration SetServerUrl(string value);
        new IConfiguration SetApiKey(string value);
        new IConfiguration SetEnablePatterns(bool value);
        new IConfiguration SetIgnoreDisplacements(bool value);
        new IConfiguration SetUseDom(bool value);
        new IConfiguration SetAccessibilityValidation(AccessibilitySettings value);
        new IConfiguration SetAbortIdleTestTimeout(int? value);
        new IConfiguration SetLayoutBreakpointsEnabled(bool value);
        new IConfiguration SetLayoutBreakpoints(params int[] value);
        #endregion
    }
}