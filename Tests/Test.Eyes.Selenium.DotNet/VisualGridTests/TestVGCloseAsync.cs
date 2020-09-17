using Applitools.Selenium.Tests.Utils;
using Applitools.Tests.Utils;
using Applitools.VisualGrid;
using NUnit.Framework;
using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Applitools.Selenium.Tests.VisualGridTests
{
    [TestFixture]
    [Parallelizable]
    public class TestVGCloseAsync : ReportingTestSuite
    {
        [Test]
        public void TestCloseAsync()
        {
            EyesRunner runner = new VisualGridRunner(10);
            Eyes eyes = new Eyes(runner);
            IWebDriver driver = SeleniumUtils.CreateChromeDriver();
            TestUtils.SetupLogging(eyes);
            driver.Url = "https://applitools.com/helloworld";
            try
            {
                Configuration config = new Configuration();
                config.SetAppName("Visual Grid Tests").SetTestName("Test CloseAsync").SetBatch(TestDataProvider.BatchInfo);
                foreach (BrowserType b in Enum.GetValues(typeof(BrowserType)))
                {
                    config.AddBrowser(800, 600, b);
                }
                eyes.SetConfiguration(config);
                var combinations = config.GetBrowsersInfo();
                Assert.Greater(combinations.Count, 1);
                eyes.Open(driver);
                eyes.CheckWindow();
                driver.Quit();
                driver = null;
                ICollection<Task<TestResultContainer>> closeTasks = eyes.CloseAsync_();
                Assert.AreEqual(combinations.Count, closeTasks.Count);
                runner.GetAllTestResults();
            }
            finally
            {
                if (driver != null)
                {
                    driver.Quit();
                }
                eyes.Abort();
            }
        }
    }
}
