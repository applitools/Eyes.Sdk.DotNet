using Applitools.Utils.Geometry;
using System;

namespace Applitools.Selenium
{
    public class Configuration : Applitools.Configuration, IConfiguration
    {

        public Configuration() { }

        public Configuration(IConfiguration configuration) : base(configuration)
        {
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
        public new IConfiguration SetProxy(ProxySettings value) => (IConfiguration)base.SetProxy(value);
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
