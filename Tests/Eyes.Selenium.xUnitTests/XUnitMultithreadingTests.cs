using Xunit;
using Applitools;
using System;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using Applitools.Utils.Geometry;
using Applitools.VisualGrid;

[assembly: CollectionBehavior(MaxParallelThreads = 1)]

namespace Eyes.Selenium.xUnitTests
{
    public class XUnitMultithreadingTests : IDisposable
    {
        private EyesRunner runner;
        private Applitools.Selenium.Eyes eyes;
        private IWebDriver driver;

        public static readonly string DRIVER_PATH = Environment.GetEnvironmentVariable("DRIVER_PATH");

        public XUnitMultithreadingTests()
        {
            var options = new ChromeOptions();
            options.AddArguments("headless");
            options.AddArguments("no-sandbox");
            // Use Chrome browser
            driver = new ChromeDriver(DRIVER_PATH, options);

            //Initialize the Runner for your test with concurrency of 5.
            // Create Eyes object with the runner, meaning it'll be a Visual Grid eyes.
            runner = new VisualGridRunner(new RunnerOptions().TestConcurrency(5));

            // Initialize the eyes SDK (IMPORTANT: make sure your API key is set in the APPLITOOLS_API_KEY env variable).
            eyes = new Applitools.Selenium.Eyes(runner);
            eyes.SaveNewTests = false;
            eyes.BranchName = "master";
            eyes.ParentBranchName = "master";
            eyes.HideScrollbars = true;
            var config = eyes.GetConfiguration();
            config.AddBrowsers(new IosDeviceInfo(IosDeviceName.iPad_7));
            config.SetLayoutBreakpoints(500, 1000);
            eyes.SetConfiguration(config);
        }

        public void Dispose()
        {
            // Close the browser.
            driver.Quit();

            // If the test was aborted before eyes.close was called, ends the test as aborted.
            eyes.AbortIfNotClosed();
        }

        [SkippableTheory]
        [InlineData("A")]
        [InlineData("B")]
        public void XUnitMultithreadingTest(string name)
        {
            driver.Navigate().GoToUrl("https://applitools.github.io/demo/TestPages/AdoptedStyleSheets/index.html");
            driver = eyes.Open(driver, "Eyes Selenium SDK - xUnit Multithreading Tests", $"xUnit Multithreading Test {name}", new RectangleSize(width: 700, height: 460));
        }
    }
}
