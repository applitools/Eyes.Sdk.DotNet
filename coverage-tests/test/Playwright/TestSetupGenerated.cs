using Applitools.Tests.Utils;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using Applitools.VisualGrid;
using Applitools.Playwright;

namespace Applitools.Generated.Playwright.Tests
{
    public abstract class TestSetupGenerated : FilteringTestSuite
    {
        protected PlaywrightEyesRunner runner;
        protected Eyes eyes;
        protected string testedPageUrl = "https://applitools.github.io/demo/TestPages/FramesTestPage/";
        public static readonly BatchInfo BatchInfo = new BatchInfo("DotNet Generated Tests");
        public static readonly string DRIVER_PATH = Environment.GetEnvironmentVariable("DRIVER_PATH");
        public static readonly string SAUCE_USERNAME = Environment.GetEnvironmentVariable("SAUCE_USERNAME");
        public static readonly string SAUCE_ACCESS_KEY = Environment.GetEnvironmentVariable("SAUCE_ACCESS_KEY");
        public static readonly string SAUCE_SELENIUM_URL = "https://ondemand.saucelabs.com:443/wd/hub";
        public static readonly string EG_SELENIUM_URL = Environment.GetEnvironmentVariable("EXECUTION_GRID_URL");
        public static readonly string LOCAL_SELENIUM_URL = "http://localhost:4444/wd/hub";
        public static readonly string LOCAL_FIREFOX_SELENIUM_URL = "http://localhost:4445/wd/hub";
        protected enum browserType
        {
            Chrome,
            IE,
            Edge,
            Firefox,
            Safari11,
            Safari12
        }
  
        protected void initEyes(bool isVisualGrid, bool isCSSMode)
        {
            string testName = NUnit.Framework.TestContext.CurrentContext.Test.MethodName;
            ILogHandler logHandler = TestUtils.InitLogHandler(testName);
            runner = isVisualGrid ? new VisualGridRunner(10, logHandler) : new ClassicRunner(logHandler);
            eyes = new Eyes(runner);
            initEyesSettings(isVisualGrid, isCSSMode);
        }

        protected void initEyesSettings(bool isVisualGrid, bool isCSSMode)
        {
            eyes.Batch = BatchInfo;
            if (!isVisualGrid) eyes.StitchMode = isCSSMode ? StitchModes.CSS : StitchModes.Scroll;
            eyes.BranchName = "master";
            eyes.ParentBranchName = "master";
            eyes.SaveNewTests = false;
            //eyes.AddProperty("ForceFPS", eyes.ForceFullPageScreenshot ? "true" : "false");
            //eyes.AddProperty("Agent ID", eyes.FullAgentId);
            eyes.HideScrollbars = true;
        }

        protected bool isStaleElementError(Exception errorObj)
        {
            return false;//(errorObj is StaleElementReferenceException);
        }

        protected string getDomString(TestResults results, string domId)
        {
            return TestUtils.GetDom(Environment.GetEnvironmentVariable("APPLITOOLS_API_KEY_READ"), results, domId);
        }

        protected JObject getDom(TestResults results, string domId)
        {
            string dom = getDomString(results, domId);
            return JObject.Parse(dom);
        }

        protected IList<JToken> getNodesByAttribute(JObject dom, string attributeName)
        {
            return dom.SelectTokens($"$..[?(@.attributes['{attributeName}'])]").ToList();
        }
    }
}
