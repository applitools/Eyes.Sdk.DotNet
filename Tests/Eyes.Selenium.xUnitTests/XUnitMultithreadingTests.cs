using Xunit;
using System;
using Applitools.Selenium;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using Applitools.Utils.Geometry;
using Applitools.VisualGrid;

[assembly: CollectionBehavior(MaxParallelThreads = 1)]

namespace Eyes.Selenium.xUnitTests
{
    public class XUnitMultithreadingTests : IDisposable
    {
        private readonly SeleniumEyesRunner runner_;
        private Applitools.Selenium.Eyes eyes_;
        private IWebDriver driver_;

        public static readonly string DriverPath = Environment.GetEnvironmentVariable("DRIVER_PATH");

        public XUnitMultithreadingTests()
        {
            var options = new ChromeOptions();
            options.AddArguments("headless");
            options.AddArguments("no-sandbox");
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

        public void Dispose()
        {
            // Close the browser.
            driver_.Quit();

            // If the test was aborted before eyes.close was called, ends the test as aborted.
            eyes_.Close();
        }

        [SkippableTheory]
        [InlineData("A")]
        [InlineData("B")]
        public void XUnitMultithreadingTest(string name)
        {
            driver_.Navigate().GoToUrl("https://applitools.github.io/demo/TestPages/AdoptedStyleSheets/index.html");
            driver_ = eyes_.Open(driver_, "Eyes Selenium SDK - xUnit Multithreading Tests", $"xUnit Multithreading Test {name}", new RectangleSize(width: 700, height: 460));
        }
    }
}
