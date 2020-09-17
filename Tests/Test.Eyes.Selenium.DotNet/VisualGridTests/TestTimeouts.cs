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
            originalTimeout = VisualGridEyes.CAPTURE_TIMEOUT;
        }

        [OneTimeTearDown]
        public void After()
        {
            VisualGridEyes.CAPTURE_TIMEOUT = originalTimeout;
        }

        [Test]
        public void TestTimeout()
        {
            //RenderingTask.pollTimeout_ = TimeSpan.FromSeconds(100);
            IWebDriver driver = SeleniumUtils.CreateChromeDriver();
            try
            {
                EyesRunner runner = new VisualGridRunner(10);
                Eyes eyes = new Eyes(runner);
                eyes.SetLogHandler(TestUtils.InitLogHandler());
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
            }
        }

        [Test]
        public void TestTimeout2()
        {
            IWebDriver driver = SeleniumUtils.CreateChromeDriver();
            VisualGridEyes.CAPTURE_TIMEOUT = TimeSpan.FromMilliseconds(1);
            try
            {
                EyesRunner runner = new VisualGridRunner(10);
                Eyes eyes = new Eyes(runner);
                eyes.SetLogHandler(TestUtils.InitLogHandler());
                driver.Url = "https://applitools.com/helloworld";
                eyes.Batch = TestDataProvider.BatchInfo;

                Configuration configuration = eyes.GetConfiguration();
                configuration.SetAppName("Test Timeouts").SetTestName("Test Timeouts").SetBatch(TestDataProvider.BatchInfo);
                configuration.AddBrowser(800, 600, BrowserType.CHROME);
                configuration.AddBrowser(700, 500, BrowserType.FIREFOX);
                configuration.AddBrowser(600, 400, BrowserType.EDGE);
                configuration.AddBrowser(900, 700, BrowserType.IE_10);
                configuration.AddBrowser(1000, 800, BrowserType.IE_11);
                configuration.AddDeviceEmulation(DeviceName.Galaxy_S5);
                configuration.AddDeviceEmulation(DeviceName.iPhone_6_7_8_Plus);
                configuration.AddDeviceEmulation(DeviceName.Laptop_with_HiDPI_screen);
                eyes.SetConfiguration(configuration);
                eyes.Open(driver);
                Assert.That(() =>
                {
                    eyes.Check(Target.Window().WithName("Test"));
                    eyes.Close();
                    runner.GetAllTestResults();
                }, Throws.Exception.With.InstanceOf<EyesException>().With.Property("Message").EqualTo("DOM capture timeout."));
            }
            finally
            {
                driver.Quit();
            }
        }
    }
}
