using Applitools.Utils.Geometry;
using System;

namespace Applitools.Selenium
{
    public interface IConfiguration : Applitools.IConfiguration
    {

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
        new IConfiguration SetProxy(ProxySettings value);
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