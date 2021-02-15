using Applitools.Selenium.Tests.Utils;
using Applitools.Tests.Utils;
using NUnit.Framework;
using OpenQA.Selenium;
using System.Drawing;

namespace Applitools.Selenium.Tests
{
    [TestFixture]
    [Parallelizable]
    public class TestServerConnector : ReportingTestSuite
    {
        [Test]
        public void TestDelete()
        {
            IWebDriver driver = SeleniumUtils.CreateChromeDriver();
            ILogHandler logHandler = TestUtils.InitLogHandler();
            Eyes eyes = new Eyes(logHandler);
            eyes.Batch = TestDataProvider.BatchInfo;
            try
            {
                driver = eyes.Open(driver, "TestSessionConnector", "TestSessionConnector", new Size(800, 600));
                driver.Url = "https://applitools.com/helloworld";
                eyes.Check("Hello!", Target.Window());
                TestResults results = eyes.Close();
                results.Delete();
            }
            finally
            {
                driver.Quit();
                eyes.Abort();
            }
        }
    }
}
