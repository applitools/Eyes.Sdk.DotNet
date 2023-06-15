using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Applitools;
using Applitools.Selenium;
using Applitools.Utils.Geometry;
using Applitools.VisualGrid;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace Eyes.Selenium.E2ETests
{
    public class MultithreadsTests
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
        public void MultiThreadSimpleTest()
        {
            int threadCount = 10;
            var tasks = new List<Task>();
            for (int i = 0; i < threadCount; i++)
            {
                var i1 = i;
                var task = Task.Factory.StartNew(() => SimpleTest(i1));
                tasks.Add(task);
            }

            Task.WaitAll(tasks.ToArray());
        }

        public void SimpleTest(int i)
        {
            var eyes = new Applitools.Selenium.Eyes(runner_);
            // Use Chrome browser
            var ci = Environment.GetEnvironmentVariable("CI");
            var options = new ChromeOptions();
            //  if (CI != null)
            {
                options.AddArguments("headless");
                options.AddArguments("no-sandbox");
            }
            //Initialize the Runner for your test with concurrency of 5.
            // Create Eyes object with the runner, meaning it'll be a Visual Grid eyes.
            IWebDriver driver = new ChromeDriver(DriverPath, options);
            try
            {
                eyes.Batch = BatchInfo;
                eyes.SaveNewTests = false;
                eyes.BranchName = "master";
                eyes.ParentBranchName = "master";
                eyes.HideScrollbars = true;
                var config = eyes.GetConfiguration();
                config.AddBrowsers(new IosDeviceInfo(IosDeviceName.iPad_7));
                config.SetLayoutBreakpoints(500, 1000);
                eyes.SetConfiguration(config);

                driver.Navigate().GoToUrl("http://applitools.github.io/demo/TestPages/fixed-position");
                driver = eyes.Open(driver, "Eyes Selenium SDK - Multithreading", $"SimpleTest - {i}", new RectangleSize(width: 700, height: 460));
                eyes.Check(Target.Region(By.CssSelector("#fixed")).Fully());
                eyes.Close(true);
                var a = eyes.GetResults(false);
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