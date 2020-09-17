using Applitools.Tests.Utils;
using Applitools.Utils.Images;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Remote;
using System;
using System.Drawing;

namespace Applitools.Selenium.Tests
{
    [TestFixture]
    [Parallelizable]
    public class TestHelperMethods
    {
        private readonly string logsPath_ = Environment.GetEnvironmentVariable("APPLITOOLS_LOGS_PATH") ?? ".";

        //[Test]
        //[Ignore("fails if not set up correctly")]
        public void TestSetViewportSize()
        {
            ChromeOptions options = new ChromeOptions();
            options.AddArgument("disable-infobars");
            if (TestUtils.RUN_HEADLESS)
            {
                options.AddArgument("headless");
            }
            IWebDriver driver = new RemoteWebDriver(options);

            Size expectedSize = new Size(700, 499);

            try
            {
                driver.Url = "http://viewportsizes.com/mine/";

                SeleniumEyes.SetViewportSize(driver, expectedSize);

                Bitmap screenshot = BasicImageUtils.CreateBitmap(((ITakesScreenshot)driver).GetScreenshot().AsByteArray);
                screenshot.Save($@"{logsPath_}\TestSetViewportSize.png");

                Size actualSize = SeleniumEyes.GetViewportSize(driver);

                Assert.AreEqual(expectedSize, actualSize, "Sizes differ");
            }
            finally
            {
                driver.Quit();
            }
        }
    }
}
