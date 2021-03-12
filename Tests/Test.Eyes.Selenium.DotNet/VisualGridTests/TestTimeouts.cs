using Applitools.Selenium.Tests.Utils;
using Applitools.Selenium.VisualGrid;
using Applitools.Tests.Utils;
using Applitools.VisualGrid;
using NUnit.Framework;
using OpenQA.Selenium;
using System;
using System.Drawing;

namespace Applitools.Selenium.Tests.VisualGridTests
{
    [TestFixture]
    public class TestTimeouts : ReportingTestSuite
    {
        private TimeSpan originalTimeout;

        [OneTimeSetUp]
        public void Before()
        {
            originalTimeout = EyesSeleniumUtils.CAPTURE_TIMEOUT;
        }

        [OneTimeTearDown]
        public void After()
        {
            EyesSeleniumUtils.CAPTURE_TIMEOUT = originalTimeout;
        }

        [Test]
        public void TestTimeout()
        {
            //RenderingTask.pollTimeout_ = TimeSpan.FromSeconds(100);
            IWebDriver driver = SeleniumUtils.CreateChromeDriver();
            ILogHandler logHandler = TestUtils.InitLogHandler();
            VisualGridRunner runner = new VisualGridRunner(10, logHandler);
            try
            {
                Eyes eyes = new Eyes(runner);
                driver.Url = "https://applitools.com/helloworld";
                eyes.Batch = TestDataProvider.BatchInfo;
                eyes.Open(driver, "Timeout Test", "Visual Grid Timeout Test", new Size(1200, 800));
                eyes.Check(Target.Window().WithName("Test"));
                eyes.Close();
                runner.GetAllTestResults();
            }
            finally
            {
                driver.Quit();
                runner.StopServiceRunner();
            }
        }

        [Test]
        public void TestTimeout2()
        {
            EyesSeleniumUtils.CAPTURE_TIMEOUT = TimeSpan.FromTicks(1);
            IWebDriver driver = SeleniumUtils.CreateChromeDriver();
            ILogHandler logHandler = TestUtils.InitLogHandler();
            VisualGridRunner runner = new VisualGridRunner(10, logHandler);
            try
            {
                Eyes eyes = new Eyes(runner);
                driver.Url = "https://applitools.com/helloworld";
                eyes.Batch = TestDataProvider.BatchInfo;

                Configuration configuration = eyes.GetConfiguration();
                configuration.SetAppName("Test Timeouts").SetTestName("Test Timeouts").SetBatch(TestDataProvider.BatchInfo);
                configuration.AddBrowser(800, 600, BrowserType.CHROME);
                configuration.AddDeviceEmulation(DeviceName.Laptop_with_HiDPI_screen);
                eyes.SetConfiguration(configuration);
                eyes.Open(driver);
                Assert.That(() =>
                {
                    eyes.Check(Target.Window().WithName("Test"));
                    eyes.Close();
                    runner.GetAllTestResults();
                }, Throws.Exception.With.InstanceOf<EyesException>().With.InnerException.With.InstanceOf<EyesException>().With.Property("Message").EqualTo("DOM capture timeout."));
            }
            finally
            {
                driver.Quit();
                runner.StopServiceRunner();
            }
        }
    }
}
