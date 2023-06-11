using Applitools.Tests.Utils;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using Applitools.Metadata;
using Applitools.VisualGrid;
using Applitools.Playwright;
using Applitools.Utils;
using Microsoft.Playwright;
using NUnit.Framework;

namespace Applitools.Generated.Playwright.Tests
{
    public abstract class TestSetupGenerated : FilteringTestSuite
    {
        protected PlaywrightEyesRunner Runner { get; set; }
        protected Eyes Eyes { get; set; }
        protected IPage Driver { get; set; }
        protected IPage EyesDriver { get; set; }

        protected PlaywrightDriverBuilder Builder { get; set; }

        protected string testedPageUrl_ = "https://applitools.github.io/demo/TestPages/FramesTestPage/";
        public static readonly BatchInfo BatchInfo = new BatchInfo("DotNet Generated Tests - Playwright");
        public static readonly string DRIVER_PATH = Environment.GetEnvironmentVariable("DRIVER_PATH");
        public static readonly string SAUCE_USERNAME = Environment.GetEnvironmentVariable("SAUCE_USERNAME");
        public static readonly string SAUCE_ACCESS_KEY = Environment.GetEnvironmentVariable("SAUCE_ACCESS_KEY");
        public static readonly string SAUCE_SELENIUM_URL = "https://ondemand.saucelabs.com:443/wd/hub";
        public static readonly string EG_SELENIUM_URL = Environment.GetEnvironmentVariable("EXECUTION_GRID_URL");
        public static readonly string LOCAL_SELENIUM_URL = "http://localhost:4444/wd/hub";
        public static readonly string LOCAL_FIREFOX_SELENIUM_URL = "http://localhost:4445/wd/hub";

        private enum BrowserType
        {
            Chrome,
            IE,
            Edge,
            Firefox,
            Safari11,
            Safari12
        }

        protected void InitEyes(bool isVisualGrid, StitchModes? stitching, string branchName)
        {
            string testName = NUnit.Framework.TestContext.CurrentContext.Test.MethodName;
            ILogHandler logHandler = TestUtils.InitLogHandler(testName);
            Runner = isVisualGrid ? new VisualGridRunner(10, logHandler) : new ClassicRunner(logHandler);
            Eyes = new Eyes(Runner)
            {
                MatchTimeout = TimeSpan.Zero,
                BranchName = branchName,
                ParentBranchName = "master",
                Batch = BatchInfo,
                SaveNewTests = false,
                HideScrollbars = true,
                HideCaret = true,
                StitchMode = stitching
            };
        }

        public PlaywrightDriverBuilder BuildDriver()
        {
            Builder = new PlaywrightDriverBuilder();
            return Builder;
        }

        public IPage GetPage()
        {
            return EyesDriver ?? Driver;
        }

        protected bool IsStaleElementError(Exception errorObj)
        {
            return false; //(errorObj is StaleElementReferenceException);
        }

        protected string GetDomString(TestResults results, string domId)
        {
            return TestUtils.GetDom(Environment.GetEnvironmentVariable("APPLITOOLS_API_KEY_READ"), results, domId);
        }

        protected JObject GetDom(TestResults results, string domId)
        {
            string dom = GetDomString(results, domId);
            return JObject.Parse(dom);
        }

        protected IList<JToken> GetNodesByAttribute(JObject dom, string attributeName)
        {
            return dom.SelectTokens($"$..[?(@.attributes['{attributeName}'])]").ToList();
        }

        protected void SetBrowsersInfo(params IRenderBrowserInfo[] browsersInfo)
        {
            var config = Eyes.GetConfiguration();
            config.AddBrowsers(browsersInfo);
            Eyes.SetConfiguration(config);
        }

        protected SessionResults GetTestInfo(TestResults results)
        {
            SessionResults sessionResults = null;
            try
            {
                sessionResults = TestUtils.GetSessionResults(Eyes.ApiKey, results);
            }
            catch (Exception e)
            {
                CommonUtils.LogExceptionStackTrace(logger_, Stage.TestFramework, StageType.TestResults, e);
                Assert.Fail("Exception appeared while getting session results");
            }

            ArgumentGuard.NotNull(sessionResults, nameof(sessionResults));
            return sessionResults;
        }

        protected void SetBatch(string name, Dictionary<string, string>[] properties)
        {
            BatchInfo batch = new BatchInfo(name);
            foreach (var item in properties)
            {
                batch.AddProperty(item["name"], item["value"]);
            }

            Eyes.Batch = batch;
        }

        protected void SetLayoutBreakpoints(LayoutBreakpointsOptions breakpointsOptions)
        {
            var config = Eyes.GetConfiguration();
            config.SetLayoutBreakpoints(breakpointsOptions);
            Eyes.SetConfiguration(config);   
        }
        
        protected void SetLayoutBreakpoints(params int[] breakpoints)
        {
            var config = Eyes.GetConfiguration();
            config.SetLayoutBreakpoints(breakpoints);
            Eyes.SetConfiguration(config);
        }
        
        protected void SetLayoutBreakpoints(bool enabled)
        {
            var config = Eyes.GetConfiguration();
            config.SetLayoutBreakpoints(enabled);
            Eyes.SetConfiguration(config);
        }

        protected void SetEnablePatterns(bool enable)
        {
            var config = Eyes.GetConfiguration();
            config.SetEnablePatterns(enable);
            Eyes.SetConfiguration(config);
        }

        protected void SetAccessibilitySettings(AccessibilitySettings accessibilitySettings)
        {
            var config = Eyes.GetConfiguration();
            config.SetAccessibilityValidation(accessibilitySettings);
            Eyes.SetConfiguration(config);
        }
        
        protected void WaitBeforeCapture(int waitTimeMs)
        {
            var config = Eyes.GetConfiguration();
            config.SetWaitBeforeCapture(waitTimeMs);
            Eyes.SetConfiguration(config);
        }
        
        protected List<JObject> GetNodesByAttributes(JObject dom, string attribute)
        {
            var nodes = new List<JObject>();
            if (dom.TryGetValue("attributes", out JToken attrs) && 
                attrs.Type == JTokenType.Object && 
                ((JObject)attrs).ContainsKey(attribute)) {
                nodes.Add(dom);
            }
            
            if (!dom.TryGetValue("childNodes", out JToken children)) {
                return nodes;
            }

            foreach (var child in children)
            {
                if (child.Type == JTokenType.Object)
                {
                    nodes.AddRange(GetNodesByAttributes((JObject)child, attribute));
                }
            }

            return nodes;
        }
    }
}