using System;
using Applitools;
using Applitools.Selenium;
using Applitools.Utils.Geometry;
using Applitools.VisualGrid;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace Eyes.Selenium.E2ETests
{
    public class TestResultContainerExceptionTests
    {
        private EyesRunner runner;
        private Applitools.Selenium.Eyes eyes;
        private IWebDriver driver;

        public static readonly string DRIVER_PATH = Environment.GetEnvironmentVariable("DRIVER_PATH");

        [SetUp]
        public void Setup()
        {
            var CI = Environment.GetEnvironmentVariable("CI");
            var options = new ChromeOptions();
          //  if (CI != null)
            {
                options.AddArguments("headless");
            }
            // Use Chrome browser
            driver = new ChromeDriver(DRIVER_PATH, options);

            //Initialize the Runner for your test with concurrency of 5.
            // Create Eyes object with the runner, meaning it'll be a Visual Grid eyes.
            runner = new VisualGridRunner(new RunnerOptions().TestConcurrency(5));
            eyes = new Applitools.Selenium.Eyes(runner);
            eyes.DontCloseBatches = false;

            var config = eyes.GetConfiguration();
            config.DontCloseBatches = false;

            runner.DontCloseBatches = true;

            config.AddBrowsers(new DesktopBrowserInfo(1000, 800, BrowserType.CHROME));
            config.AddBrowsers(new IosDeviceInfo(IosDeviceName.iPad_7));
            config.AddBrowsers(new ChromeEmulationInfo(DeviceName.Pixel_4_XL));
            config.SetLayoutBreakpoints(500, 1000);
            eyes.SetConfiguration(config);
        }
        
        [TearDown]
        public void TearDown()
        {
            // Close the browser.
            driver.Quit();

            // If the test was aborted before eyes.close was called, ends the test as aborted.
            eyes.AbortIfNotClosed();
        }

        [Test]
        public void ServerExceptionTest()
        {
            driver.Navigate().GoToUrl("https://applitools.github.io/demo/TestPages/AdoptedStyleSheets/index.html");
            driver = eyes.Open(driver, "Eyes Selenium SDK - Integration Tests", "ServerExceptionTest", new RectangleSize(width: 700, height: 460));
            eyes.Check(Target.Window().Fully(false));
            try
            {
                eyes.Close(true);
            }
            catch
            {
            }

            var allTestResults = runner.GetAllTestResults(false);
            Assert.AreEqual(3, allTestResults.Count);
            foreach (var testResult in allTestResults)
            {
                Assert.NotNull(testResult.Exception);
                Assert.NotNull(testResult.BrowserInfo);
            }
        }
    }
}