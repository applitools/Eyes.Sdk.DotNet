using Applitools.Selenium.Tests.Mock;
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
            ILogHandler logHandler = TestUtils.InitLogHandler();
            VisualGridRunner runner = new VisualGridRunner(30, logHandler);
            Eyes eyes = new Eyes(runner);

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
            WebDriverProvider webdriverProvider = new WebDriverProvider();
            IServerConnectorFactory serverConnectorFactory = new MockServerConnectorFactory(webdriverProvider);
            ILogHandler logHandler = TestUtils.InitLogHandler();
            VisualGridRunner runner = new VisualGridRunner(30, nameof(ViewportsTest), serverConnectorFactory, logHandler);
            Eyes eyes = new Eyes(runner);

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
            webdriverProvider.SetDriver(driver);
            try
            {
                eyes.Open(driver);
                driver.Url = "data:text/html,<html><body><h1>Hello, world!</h1></body></html>";
                eyes.Check("Test Viewport", Target.Window().Fully());
            }
            finally
            {
                driver.Quit();
            }
            TestResultsSummary allResults = runner.GetAllTestResults(false);
            MockServerConnector serverConnector = (MockServerConnector)runner.ServerConnector;
            HashSet<RenderRequest> requests = new HashSet<RenderRequest>();
            foreach (string requestJson in serverConnector.RenderRequests)
            {
                RenderRequest[] reqs = Newtonsoft.Json.JsonConvert.DeserializeObject<RenderRequest[]>(requestJson);
                foreach (RenderRequest req in reqs)
                {
                    requests.Add(req);
                }
            }
            int browserCount = sconf.GetBrowsersInfo().Count;
            Assert.AreEqual(browserCount, numOfBrowsers * 2);
            Assert.AreEqual(browserCount, requests.Count);
        }

        [Parallelizable(ParallelScope.None)]
        [TestCase("https://applitools.github.io/demo/TestPages/DomTest/shadow_dom.html", "Shadow DOM Test")]
        [TestCase("https://applitools.github.io/demo/TestPages/VisualGridTestPage/canvastest.html", "Canvas Test")]
        public void TestSpecialRendering(string url, string testName)
        {
            string logsPath = TestUtils.InitLogPath();
            ILogHandler logHandler = TestUtils.InitLogHandler(logPath: logsPath);
            VisualGridRunner runner = new VisualGridRunner(30, logHandler);

            runner.DebugResourceWriter = new FileDebugResourceWriter(logsPath);

            Eyes eyes = new Eyes(runner);

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
            try
            {
                eyes.Open(driver);
                driver.Url = url;
                Thread.Sleep(500);
                eyes.Check(testName, Target.Window().Fully());
                eyes.Close(false);
                TestResultsSummary allResults = runner.GetAllTestResults(false);
            }
            finally
            {
                driver.Quit();
                eyes.AbortIfNotClosed();
                runner.StopServiceRunner();
            }
        }

        [Test]
        public void IOSSimulatorUfgTest()
        {
            ILogHandler logHandler = TestUtils.InitLogHandler();
            VisualGridRunner runner = new VisualGridRunner(10, logHandler);
            Eyes eyes = new Eyes(runner);
            IWebDriver driver = SeleniumUtils.CreateChromeDriver();
            try
            {
                IConfiguration config = eyes.GetConfiguration();
                config.SaveDiffs = false;
                config.Batch = TestDataProvider.BatchInfo;
                config.AddBrowser(new IosDeviceInfo(IosDeviceName.iPhone_XR, ScreenOrientation.Landscape));
                eyes.SetConfiguration(config);

                driver.Url = "http://applitools.github.io/demo";
                eyes.Open(driver, "Eyes SDK", "UFG Mobile Happy Flow", new Size(800, 600));
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
