using Applitools.Selenium.Tests.Utils;
using Applitools.Tests.Utils;
using Applitools.VisualGrid;
using NUnit.Framework;
using OpenQA.Selenium;

namespace Applitools.Selenium.Tests.VisualGridTests
{
    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    public class TestDefaultRendering : ReportingTestSuite
    {
        [Test]
        public void TestDefaultRenderingOfMultipleTargets()
        {
            IWebDriver driver = SeleniumUtils.CreateChromeDriver();
            driver.Url = "https://applitools.com/helloworld";
            VisualGridRunner runner = new VisualGridRunner(10);
            Eyes eyes = new Eyes(runner);
            Configuration config = eyes.GetConfiguration();
            config.AddBrowser(800, 600, BrowserType.CHROME);
            config.AddBrowser(800, 600, BrowserType.FIREFOX);
            config.AddBrowser(1200, 800, BrowserType.CHROME);
            config.AddBrowser(1200, 800, BrowserType.FIREFOX);
            config.SetAppName(nameof(TestDefaultRendering)).SetTestName(nameof(TestDefaultRenderingOfMultipleTargets));
            eyes.SetConfiguration(config);
            try
            {
                eyes.Open(driver);
                eyes.Check(Target.Window());
                eyes.Close();
            }
            finally
            {
                eyes.Abort();
                driver.Quit();
            }
            TestResultsSummary allTestResults = runner.GetAllTestResults();
            string batchId = null;
            string batchName = null;
            foreach (TestResultContainer trc in allTestResults)
            {
                if (batchId == null) batchId = trc.TestResults.BatchId;
                if (batchName == null) batchName = trc.TestResults.BatchName;
                Assert.AreEqual(batchId, trc.TestResults.BatchId);
                Assert.AreEqual(batchName, trc.TestResults.BatchName);
            }
        }
    }
}
