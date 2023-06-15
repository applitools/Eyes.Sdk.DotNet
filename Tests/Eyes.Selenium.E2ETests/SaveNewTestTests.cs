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
    public class SaveNewTestTests
    {
        private SeleniumEyesRunner runner_;

        public static readonly string DriverPath = Environment.GetEnvironmentVariable("DRIVER_PATH");
        public static readonly BatchInfo BatchInfo = new BatchInfo("DotNet Multithreads Test");

        [SetUp]
        public void Setup()
        {
            //  Trace.Listeners.Add(new ConsoleTraceListener());
            runner_ = new ClassicRunner();
        }

        [Test]
        public void SaveNewToDefaultTest()
        {
            var eyes = new Applitools.Selenium.Eyes(runner_);
            // Use Chrome browser
            var options = new ChromeOptions();
            options.AddArguments("headless");
            options.AddArguments("no-sandbox");
            //Initialize the Runner for your test with concurrency of 5.
            // Create Eyes object with the runner, meaning it'll be a Visual Grid eyes.
            IWebDriver driver = new ChromeDriver(DriverPath, options);
            try
            {
                eyes.Batch = BatchInfo;
                eyes.BranchName = "master";
                eyes.ParentBranchName = "master";
                eyes.HideScrollbars = true;
                var config = eyes.GetConfiguration();
                config.AddBrowsers(new IosDeviceInfo(IosDeviceName.iPad_7));
                config.SetLayoutBreakpoints(500, 1000);
                eyes.SetConfiguration(config);

                driver.Navigate().GoToUrl("http://applitools.github.io/demo/TestPages/fixed-position");
                driver = eyes.Open(driver, "Eyes Selenium SDK - Save New Test", $"SaveNewTestTest - {Guid.NewGuid()}", new RectangleSize(width: 700, height: 460));
                eyes.Check(Target.Region(By.CssSelector("#fixed")).Fully());
                eyes.Close(true);
            }
            finally
            {
                // If the test was aborted before eyes.close was called, ends the test as aborted.
                eyes.AbortIfNotClosed();
                driver.Quit();
            }
        }
    }
}