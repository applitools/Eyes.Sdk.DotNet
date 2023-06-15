using System;
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
        private SeleniumEyesRunner runner_;
        private Applitools.Selenium.Eyes eyes_;
        private IWebDriver driver_;

        public static readonly string DriverPath = Environment.GetEnvironmentVariable("DRIVER_PATH");

        [SetUp]
        public void Setup()
        {
            var ci = Environment.GetEnvironmentVariable("CI");
            var options = new ChromeOptions();
            //  if (CI != null)
            {
                options.AddArguments("headless");
                options.AddArguments("no-sandbox");
            }
            // Use Chrome browser
            driver_ = new ChromeDriver(DriverPath, options);

            //Initialize the Runner for your test with concurrency of 5.
            // Create Eyes object with the runner, meaning it'll be a Visual Grid eyes.
            runner_ = new VisualGridRunner(new RunnerOptions().TestConcurrency(5));

            // Initialize the eyes SDK (IMPORTANT: make sure your API key is set in the APPLITOOLS_API_KEY env variable).
            eyes_ = new Applitools.Selenium.Eyes(runner_);
            eyes_.SaveNewTests = false;
            eyes_.BranchName = "master";
            eyes_.ParentBranchName = "master";
            eyes_.HideScrollbars = true;
            var config = eyes_.GetConfiguration();
            config.AddBrowsers(new IosDeviceInfo(IosDeviceName.iPad_7));
            config.SetLayoutBreakpoints(500, 1000);
            eyes_.SetConfiguration(config);
        }

        [TearDown]
        public void TearDown()
        {
            // Close the browser.
            driver_.Quit();

            // If the test was aborted before eyes.close was called, ends the test as aborted.
            eyes_.AbortIfNotClosed();
        }

        [Test]
        public void ServerExceptionTest()
        {
            driver_.Navigate().GoToUrl("https://applitools.github.io/demo/TestPages/AdoptedStyleSheets/index.html");
            driver_ = eyes_.Open(driver_, "Eyes Selenium SDK - Integration Tests", "ServerExceptionTest", new RectangleSize(width: 700, height: 460));
            eyes_.Check(Target.Window().Fully(false));
            try
            {
                eyes_.Close(true);
            }
            catch
            {
            }

            var allTestResults = runner_.GetAllTestResults(false);
            Assert.AreEqual(1, allTestResults.Count);
            foreach (var testResult in allTestResults)
            {
                Assert.NotNull(testResult.Exception);
            }
        }
    }
}