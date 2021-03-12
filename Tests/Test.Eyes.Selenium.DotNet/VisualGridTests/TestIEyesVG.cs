using Applitools.Metadata;
using Applitools.Tests.Utils;
using Applitools.Utils;
using Applitools.Utils.Geometry;
using Applitools.VisualGrid;
using NUnit.Framework;
using OpenQA.Selenium;
using System;
using System.Collections.Generic;

namespace Applitools.Selenium.Tests.VisualGridTests
{
    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    public class TestIEyesVG : TestIEyesBase
    {
        private VisualGridRunner runner_;
        private static Configuration renderingConfiguration_;
        private static readonly BatchInfo batchInfo_ = new BatchInfo("Top Sites - Visual Grid");
        public TestIEyesVG(string fixtureName) : base(fixtureName)
        {
            runner_ = new VisualGridRunner(40, LogHandler);
            logger_ = runner_.Logger;
            logger_.Log("enter");
            runner_.ServerUrl = SERVER_URL;
            runner_.ApiKey = API_KEY;
        }

        public TestIEyesVG() : this("visual_grid")
        {
        }

        protected virtual BatchInfo BatchInfo { get => batchInfo_; }

        protected override Eyes InitEyes(IWebDriver webDriver, string testedUrl)
        {
            Eyes eyes = new Eyes(runner_);
            Logger logger = eyes.Logger;
            logger.Log("creating WebDriver: " + testedUrl);
            Configuration renderingConfiguration = new Configuration(GetConfiguration());
            renderingConfiguration.SetTestName("Top Sites - " + testedUrl);

            logger.Log("created configurations for url " + testedUrl);
            eyes.SetConfiguration(renderingConfiguration);
            eyes.Open(webDriver);
            return eyes;
        }

        private Configuration GetConfiguration()
        {
            if (renderingConfiguration_ == null)
            {
                renderingConfiguration_ = new Configuration();
                renderingConfiguration_.SetAppName("Top Sites");
                renderingConfiguration_.SetBatch(BatchInfo);
                renderingConfiguration_.AddBrowser(800, 600, BrowserType.CHROME);
                renderingConfiguration_.AddBrowser(700, 500, BrowserType.FIREFOX);
                renderingConfiguration_.AddBrowser(1200, 800, BrowserType.IE_10);
                renderingConfiguration_.AddBrowser(1200, 800, BrowserType.IE_11);
            }
            return renderingConfiguration_;
        }

        internal override void ValidateResults(Eyes eyes, TestResults results)
        {
        }

        private void ValidateRunnerResults_()
        {
            Dictionary<BrowserType, string> browserTypes = new Dictionary<BrowserType, string>() {
                { BrowserType.CHROME, "CHROME" },
                { BrowserType.FIREFOX, "FIREFOX" },
                { BrowserType.EDGE, "EDGE" },
                { BrowserType.IE_10, "IE 10.0" },
                { BrowserType.IE_11, "IE 11.0" },
            };

            List<RenderBrowserInfo> browsers = renderingConfiguration_.GetBrowsersInfo();
            TestResultsSummary resultsSummary = runner_.GetAllTestResults(false);

            logger_.Log(resultsSummary.ToString());

            TestResultContainer[] testResultsContainer = resultsSummary.GetAllResults();

            foreach (TestResultContainer testResultContainer in testResultsContainer)
            {
                TestResults testResults = testResultContainer.TestResults;
                SessionResults sessionResults = TestUtils.GetSessionResults(runner_.ApiKey, testResults);

                ActualAppOutput[] actualAppOutputs = sessionResults.ActualAppOutput;
                Assert.AreEqual(2, actualAppOutputs.Length);

                ImageIdentifier image1 = actualAppOutputs[0].Image;
                Assert.IsTrue(image1.HasDom);
                RectangleSize hostDisplaySize = testResults.HostDisplaySize;
                Assert.AreEqual(hostDisplaySize.Width, image1.Size.Width);
                Assert.AreEqual(hostDisplaySize.Height, image1.Size.Height);

                ImageIdentifier image2 = actualAppOutputs[1].Image;
                Assert.IsTrue(image2.HasDom);

                BaselineEnv env = sessionResults.Env;
                RenderBrowserInfo browser = browsers.Find((item) =>
                {
                    return
                    (env.HostingAppInfo?.StartsWith(browserTypes[item.BrowserType], StringComparison.OrdinalIgnoreCase) ?? false) &&
                    (env.DisplaySize.Width == item.Width) && (env.DisplaySize.Height == item.Height);
                });
                Assert.NotNull(browser, $"browser {env.HostingAppInfo}, {env.DisplaySize} was not found in list:{Environment.NewLine}\t{browsers.Concat($",{Environment.NewLine}\t")}");
                browsers.Remove(browser);
            }
            CollectionAssert.IsEmpty(browsers);
        }

        [OneTimeTearDown]
        public void AfterClass()
        {
            logger_.Verbose("calling renderingManager_.GetAllTestResults()");
            ValidateRunnerResults_();
        }
    }
}
