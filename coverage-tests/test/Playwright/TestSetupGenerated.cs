using Applitools.Tests.Utils;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using Applitools.VisualGrid;
using Applitools.Playwright;
using Microsoft.Playwright;

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
        public static readonly BatchInfo BatchInfo = new BatchInfo("DotNet Generated Tests");
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

        protected void InitEyes(bool isVisualGrid, StitchModes stitching, string branchName)
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
    }
}