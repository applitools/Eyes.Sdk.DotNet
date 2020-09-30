using Applitools.Metadata;
using Applitools.Selenium.Tests.Utils;
using Applitools.Tests.Utils;
using Applitools.Ufg;
using Applitools.VisualGrid;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading;
using ScreenOrientation = Applitools.VisualGrid.ScreenOrientation;

namespace Applitools.Selenium.Tests.VisualGridTests
{
    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    public class TestRenderings : ReportingTestSuite
    {
        [Test]
        public void TestMobileOnly()
        {
            VisualGridRunner runner = new VisualGridRunner(30);
            Eyes eyes = new Eyes(runner);

            eyes.SetLogHandler(TestUtils.InitLogHandler());

            Configuration sconf = new Configuration();
            sconf.SetTestName("Mobile Render Test");
            sconf.SetAppName("Visual Grid Render Test");
            sconf.SetBatch(TestDataProvider.BatchInfo);

            sconf.AddDeviceEmulation(DeviceName.Galaxy_S5);

            eyes.SetConfiguration(sconf);
            ChromeDriver driver = SeleniumUtils.CreateChromeDriver();
            eyes.Open(driver);
            driver.Url = "https://applitools.github.io/demo/TestPages/DynamicResolution/mobile.html";
            eyes.Check("Test Mobile Only", Target.Window().Fully());
            driver.Quit();
            eyes.Close();
            TestResultsSummary allResults = runner.GetAllTestResults();
        }

        [Test]
        public void ViewportsTest()
        {
            VisualGridRunner runner = new VisualGridRunner(30);
            Eyes eyes = new Eyes(runner);

            eyes.SetLogHandler(TestUtils.InitLogHandler());

            Configuration sconf = new Configuration();
            sconf.SetBatch(TestDataProvider.BatchInfo);
            sconf.SetTestName("Viewport Size Test");
            sconf.SetAppName("Visual Grid Viewports Test");
            sconf.SetHideScrollbars(true);
            sconf.SetStitchMode(StitchModes.CSS);
            sconf.SetForceFullPageScreenshot(true);
            sconf.SetMatchLevel(MatchLevel.Strict);
            int numOfBrowsers = 0;
            foreach (BrowserType b in Enum.GetValues(typeof(BrowserType)))
            {
#pragma warning disable CS0618 // Type or member is obsolete
                if (b == BrowserType.EDGE) continue;
#pragma warning restore CS0618 // Type or member is obsolete
                sconf.AddBrowser(700, 500, b);
                sconf.AddBrowser(800, 600, b);
                numOfBrowsers++;
            }
            eyes.SetConfiguration(sconf);

            ChromeDriver driver = SeleniumUtils.CreateChromeDriver();
            eyes.Open(driver);
            driver.Url = "https://www.applitools.com";
            eyes.Check("Test Viewport", Target.Window().Fully());
            driver.Quit();

            TestResultsSummary allResults = runner.GetAllTestResults(false);
            Assert.Greater(sconf.GetBrowsersInfo().Count, numOfBrowsers);

            Dictionary<string, HashSet<Size>> results = new Dictionary<string, HashSet<Size>>();
            foreach (TestResultContainer testResultContainer in allResults)
            {
                Assert.NotNull(testResultContainer, nameof(testResultContainer));
                SessionResults sessionResults = TestUtils.GetSessionResults(eyes.ApiKey, testResultContainer.TestResults);
                if (sessionResults == null)
                {
                    eyes.Logger.Log("Error: sessionResults is null for item {0}", testResultContainer);
                    continue;
                }
                BaselineEnv env = sessionResults.Env;
                string browser = env.HostingAppInfo;
                if (browser == null)
                {
                    eyes.Log("Error: HostingAppInfo (browser) is null. {0}", testResultContainer);
                    continue;
                }
                if (!results.TryGetValue(browser, out HashSet<Size> sizesList))
                {
                    sizesList = new HashSet<Size>();
                    results.Add(browser, sizesList);
                }
                Size displaySize = env.DisplaySize;
                if (sizesList.Contains(displaySize))
                {
                    Assert.Fail($"Browser {browser} viewport size {displaySize} already exist in results.");
                }
                sizesList.Add(displaySize);
            }
            Assert.AreEqual(numOfBrowsers, results.Count, "unique browsers in results");
            Assert.AreEqual(sconf.GetBrowsersInfo().Count, allResults.Count, "all results");
        }

        [Parallelizable(ParallelScope.None)]
        [TestCase("https://applitools.github.io/demo/TestPages/DomTest/shadow_dom.html", "Shadow DOM Test")]
        [TestCase("https://applitools.github.io/demo/TestPages/VisualGridTestPage/canvastest.html", "Canvas Test")]
        public void TestSpecialRendering(string url, string testName)
        {
            VisualGridRunner runner = new VisualGridRunner(30);

            string logsPath = TestUtils.InitLogPath();
            runner.DebugResourceWriter = new FileDebugResourceWriter(logsPath);

            Eyes eyes = new Eyes(runner);
            eyes.SetLogHandler(TestUtils.InitLogHandler(logPath: logsPath));

            Configuration sconf = new Configuration();
            sconf.SetTestName(testName);
            sconf.SetAppName("Visual Grid Render Test");
            sconf.SetBatch(TestDataProvider.BatchInfo);

            sconf.AddDeviceEmulation(DeviceName.Galaxy_S5);
            sconf.AddBrowser(1200, 800, BrowserType.CHROME);
            sconf.AddBrowser(1200, 800, BrowserType.FIREFOX);

            // Edge doesn't support Shadow-DOM - returns an empty image.
            //sconf.AddBrowser(1200, 800, BrowserType.EDGE);

            // Internet Explorer doesn't support Shadow-DOM - fails to render and throws an error.
            //sconf.AddBrowser(1200, 800, BrowserType.IE_11);
            //sconf.AddBrowser(1200, 800, BrowserType.IE_10);

            eyes.SetConfiguration(sconf);
            ChromeDriver driver = SeleniumUtils.CreateChromeDriver();
            eyes.Open(driver);
            driver.Url = url;
            Thread.Sleep(500);
            eyes.Check(testName, Target.Window().Fully());
            driver.Quit();
            eyes.Close(false);
            TestResultsSummary allResults = runner.GetAllTestResults(false);//TODO - this never ends!
        }

        [Test]
        public void IOSSimulatorUfgTest()
        {
            VisualGridRunner runner = new VisualGridRunner(10);
            Eyes eyes = new Eyes(runner);
            TestUtils.SetupLogging(eyes);
            IWebDriver driver = SeleniumUtils.CreateChromeDriver();
            try
            {
                IConfiguration config = eyes.GetConfiguration();
                config.SaveDiffs = false;
                config.Batch = TestDataProvider.BatchInfo;
                config.AddBrowser(new IosDeviceInfo(IosDeviceName.iPhone_XR, ScreenOrientation.Landscape));
                eyes.SetConfiguration(config);

                driver.Url = "http://applitools.github.io/demo";
                eyes.Open(driver, "Eyes SDK", "UFG Mobile Happy Flow", new Size(800,600));
                eyes.CheckWindow();
                eyes.Close();
            }
            finally
            {
                eyes.AbortIfNotClosed();
                driver.Quit();
                runner.GetAllTestResults();
            }
        }
    }
}
