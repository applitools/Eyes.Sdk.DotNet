using Applitools.Selenium.Tests.Utils;
using Applitools.Tests.Utils;
using Applitools.VisualGrid;
using NUnit.Framework;
using OpenQA.Selenium;
using System.Drawing;

namespace Applitools.Selenium.Tests.ApiTests
{
    [Parallelizable(ParallelScope.All)]
    public class TestApiMethods : ReportingTestSuite
    {
        [TestCase(true), TestCase(false)]
        public void TestCloseAsync(bool useVisualGrid)
        {
            IWebDriver driver = SeleniumUtils.CreateChromeDriver();
            EyesRunner runner = useVisualGrid ? (EyesRunner)new VisualGridRunner(10) : new ClassicRunner();
            string logPath = TestUtils.InitLogPath();
            runner.SetLogHandler(TestUtils.InitLogHandler(logPath: logPath));
            if (useVisualGrid && !TestUtils.RUNS_ON_CI)
            {
                ((VisualGridRunner)runner).DebugResourceWriter = new FileDebugResourceWriter(logPath);
            }
            Eyes eyes = new Eyes(runner);
            eyes.Batch = TestDataProvider.BatchInfo;
            try
            {
                driver.Url = "https://applitools.com/helloworld";
                eyes.Open(driver, nameof(TestApiMethods), nameof(TestCloseAsync) + "_1", new Size(800, 600));
                eyes.Check(Target.Window().WithName("step 1"));
                eyes.CloseAsync();
                driver.FindElement(By.TagName("button")).Click();
                eyes.Open(driver, nameof(TestApiMethods), nameof(TestCloseAsync) + "_2", new Size(800, 600));
                eyes.Check(Target.Window().WithName("step 2"));
                eyes.CloseAsync();
                runner.GetAllTestResults();
            }
            finally
            {
                driver.Quit();
            }
        }
    }
}
