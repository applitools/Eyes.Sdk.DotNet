using Applitools.Selenium.Tests.Utils;
using Applitools.Tests.Utils;
using Applitools.VisualGrid;
using NUnit.Framework;
using OpenQA.Selenium;
using System;
using System.Diagnostics;
using System.IO;

namespace Applitools.Selenium.Tests
{
    [Parallelizable(ParallelScope.Children)]
    public class UFGConcurrencyBenchmarks
    {
        private static readonly string LOG_PATH = Environment.GetEnvironmentVariable("APPLITOOLS_LOGS_PATH") ?? ".";
        private static readonly string DATE_STRING = DateTime.Now.ToString("yyyy_MM_dd__HH_mm_ss");
        private static readonly string LOG_FILE = Path.Combine(LOG_PATH, "DotNet", $"benchmarks_{DATE_STRING}", "benchmarks.log");
        //private static readonly ILogHandler logHandler = new FileLogHandler(LOG_FILE, true, false);
        private static readonly ILogHandler logHandler = new NunitLogHandler(false);
        private static readonly int numberOfBrowsers = 15;
        private static VisualGridRunner runner;
        private static readonly int concurrency = 20; // 10, 20, 30
        private static readonly string url = "https://edition.cnn.com/"; //"https://www.foxnews.com/", "https://www.booking.com/" 
        private static readonly int steps = 1;//1, 3, 10
        private static readonly Stopwatch timer = new Stopwatch();
        private static readonly BatchInfo batchInfo = new BatchInfo("UFG Benchmarks - New");

        [TestCase("1")]
        [TestCase("2")]
        [TestCase("3")]
        [TestCase("4")]
        public void ConcurrencyTest(string testName)
        {
            runner.Logger.Log("starting run {0} with concurrency {1} and {2} steps", testName, concurrency, steps);

            Eyes eyes = new Eyes(runner);

            IConfiguration configuration = eyes.GetConfiguration();
            configuration.SetBatch(batchInfo).SetAppName("Test").SetTestName($"Benchmarks - ({testName}, concurrency: {concurrency}, steps: {steps})");
            for (int i = 0; i < numberOfBrowsers; i++)
            {
                configuration.AddBrowser(600 + 25 * i, 600 + 25 * i, BrowserType.CHROME);
            }
            eyes.SetConfiguration(configuration);

            IWebDriver driver = SeleniumUtils.CreateChromeDriver();
            driver.Url = url;
            try
            {
                eyes.Open(driver);
                for (int i = 1; i <= steps; i++)
                {
                    eyes.CheckWindow(url + " - step " + i);
                }
                eyes.CloseAsync();
            }
            finally
            {
                eyes.AbortAsync();
                driver.Quit();
            }
        }

        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            runner = new VisualGridRunner(new RunnerOptions().TestConcurrency(concurrency), logHandler);
            timer.Start();
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            runner.GetAllTestResults(false);
            timer.Stop();
            string result = string.Format("concurrency: {0} ; steps: {1} ; total minutes: {2}",
                concurrency, steps, timer.Elapsed.TotalMinutes);
            runner.Logger.Log(result);
        }
    }
}
