using Applitools.VisualGrid;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;
using System.Collections;
using System.Diagnostics;
using System.IO;

namespace Applitools.Selenium
{
    [Parallelizable(ParallelScope.Children)]
    public class UFGConcurrencyBenchmarks
    {
        //private static readonly string LOG_PATH = Environment.GetEnvironmentVariable("APPLITOOLS_LOGS_PATH") ?? ".";
        //private static readonly string DATE_STRING = DateTime.Now.ToString("yyyy_MM_dd__HH_mm_ss");
        //private static readonly string LOG_FILE = Path.Combine(LOG_PATH, "DotNet", $"benchmarks_{DATE_STRING}", "benchmarks.log");
        //private static readonly ILogHandler logHandler = new FileLogHandler("./benchmarks.log", true, true);
        //private static readonly ILogHandler logHandler = new NunitLogHandler(false);
        private static readonly ILogHandler logHandler = NullLogHandler.Instance;
        private static readonly int browsers = int.Parse(Environment.GetEnvironmentVariable("BROWSERS") ?? "10");
        private static VisualGridRunner runner;
        private static readonly int concurrency = int.Parse(Environment.GetEnvironmentVariable("CONCURRENCY") ?? "20"); // 10, 20, 30
        private static readonly string url = Environment.GetEnvironmentVariable("URL") ?? "https://www.booking.com/index.uk.html?aid=376445"; //"https://edition.cnn.com/"; //"https://www.foxnews.com/", "https://www.booking.com/" 
        private static readonly int steps = int.Parse(Environment.GetEnvironmentVariable("STEPS") ?? "3"); //1, 3, 10
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
            for (int i = 0; i < browsers; i++)
            {
                configuration.AddBrowser(600 + 25 * i, 600 + 25 * i, BrowserType.CHROME);
            }
            eyes.SetConfiguration(configuration);

            ChromeOptions options = new ChromeOptions();
            options.AddArgument("--headless");
            IWebDriver driver = new ChromeDriver(options) { Url = url };
            try
            {
                eyes.Open(driver);
                for (int i = 1; i <= steps; i++)
                {
                    eyes.CheckWindow(url + " - step " + i, false);
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
            //IDictionary envVars = Environment.GetEnvironmentVariables();
            //foreach (object envKey in envVars.Keys)
            //    TestContext.Progress.WriteLine($"{envKey} = {envVars[envKey]}");
            TestContext.Progress.WriteLine($"URL: {url} ; browsers: {browsers} ; concurrency: {concurrency} ; steps: {steps}");
            runner = new VisualGridRunner(concurrency, logHandler);
            timer.Start();
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            runner.GetAllTestResults(false);
            timer.Stop();
            string result = string.Format("browsers: {0} ; concurrency: {1} ; steps: {2} ; total minutes: {3}",
                browsers, concurrency, steps, timer.Elapsed.TotalMinutes);
            runner.Logger.Log(result);
            TestContext.Progress.WriteLine(result);
        }
    }
}
