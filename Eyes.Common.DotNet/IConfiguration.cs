using System;
using System.Collections.Generic;
using System.Net;
using Applitools.Utils.Geometry;

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

        bool IgnoreCaret { get; set; }

        TimeSpan MatchTimeout { get; set; }

        string ParentBranchName { get; set; }

        bool? SaveDiffs { get; set; }

        bool SaveNewTests { get; set; }
        bool SaveFailedTests { get; set; }

        bool SendDom { get; set; }

        int StitchOverlap { get; set; }

        string TestName { get; set; }

        RectangleSize ViewportSize { get; set; }

        MatchLevel MatchLevel { get; set; }

        string ServerUrl { get; set; }

        string ApiKey { get; set; }

        bool IgnoreDisplacements { get; set; }

        bool UseDom { get; set; }

        bool EnablePatterns { get; set; }

        AccessibilitySettings AccessibilityValidation { get; set; }
        int? AbortIdleTestTimeout { get; set; }

        bool LayoutBreakpointsEnabled { get; set; }

        IList<int> LayoutBreakpoints { get; set; }

        WebProxy Proxy { get; set; }

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
        IConfiguration SetIgnoreCaret(bool value);
        IConfiguration SetMatchTimeout(TimeSpan value);
        IConfiguration SetParentBranchName(string value);
        IConfiguration SetSaveDiffs(bool? value);
        IConfiguration SetSaveNewTests(bool value);
        IConfiguration SetSaveFailedTests(bool value);
        IConfiguration SetSendDom(bool value);
        IConfiguration SetStitchOverlap(int value);
        IConfiguration SetTestName(string value);
        IConfiguration SetViewportSize(RectangleSize value);
        IConfiguration SetMatchLevel(MatchLevel value);
        IConfiguration SetServerUrl(string value);
        IConfiguration SetApiKey(string value);

        IConfiguration SetProxy(ProxySettings value);
        IConfiguration SetProxy(WebProxy value);
        IConfiguration SetIgnoreDisplacements(bool value);
        IConfiguration SetUseDom(bool value);
        IConfiguration SetEnablePatterns(bool value);
        IConfiguration SetAccessibilityValidation(AccessibilitySettings value);
        IConfiguration SetAbortIdleTestTimeout(int? value);
        IConfiguration SetLayoutBreakpointsEnabled(bool shouldSet);
        IConfiguration SetLayoutBreakpoints(params int[] breakpoints);
    }
}