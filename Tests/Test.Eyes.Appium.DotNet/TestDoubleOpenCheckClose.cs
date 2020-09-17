using Applitools.Common;
using Applitools.Tests.Utils;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Appium;
using OpenQA.Selenium.Remote;
using System;

namespace Applitools.Appium.Tests
{
    [Parallelizable]
    public class TestDoubleOpenCheckClose : ReportingTestSuite
    {
        //[Test]
        public void TestDoubleOpenCheckCloseOnSauceLabs()
        {
            Eyes eyes = new Eyes();
            TestUtils.SetupLogging(eyes);
            eyes.SendDom = false;
            eyes.Batch = TestDataProvider.BatchInfo;

            RunOnAndroid_(eyes);
            RunOnIos_(eyes);
        }

        private static void RunOnAndroid_(Eyes eyes)
        {
            AppiumOptions options = new AppiumOptions();

            options.AddAdditionalCapability("name", "Open Check Close X2 (A)");

            options.AddAdditionalCapability("username", TestDataProvider.SAUCE_USERNAME);
            options.AddAdditionalCapability("accesskey", TestDataProvider.SAUCE_ACCESS_KEY);

            options.AddAdditionalCapability("deviceName", "Samsung Galaxy Tab S3 GoogleAPI Emulator");
            options.AddAdditionalCapability("deviceOrientation", "landscape");
            options.AddAdditionalCapability("platformName", "Android");
            options.AddAdditionalCapability("platformVersion", "8.1");
            options.AddAdditionalCapability("browserName", "Chrome");

            RemoteWebDriver driver = new RemoteWebDriver(new Uri(TestDataProvider.SAUCE_SELENIUM_URL), options.ToCapabilities(), TimeSpan.FromMinutes(3));
            //*************

            try
            {
                IWebDriver eyesDriver = eyes.Open(driver, "Applitools", "Open Check Close X2 SauceLabs (A)");
                eyesDriver.Url = "https://www.applitools.com";
                eyes.Check("Test Android", Target.Window().SendDom(false));
                eyes.Close(false);
            }
            finally
            {
                // Close the browser.
                driver.Quit();

                // If the test was aborted before eyes.Close was called, ends the test as aborted.
                eyes.Abort();
            }
        }

        private static void RunOnIos_(Eyes eyes)
        {
            AppiumOptions options = new AppiumOptions();

            options.AddAdditionalCapability("name", "Open Check Close X2 (B)");

            options.AddAdditionalCapability("username", TestDataProvider.SAUCE_USERNAME);
            options.AddAdditionalCapability("accessKey", TestDataProvider.SAUCE_ACCESS_KEY);

            options.AddAdditionalCapability("deviceName", "iPad Simulator");
            options.AddAdditionalCapability("deviceOrientation", "portrait");
            options.AddAdditionalCapability("platformName", "iOS");
            options.AddAdditionalCapability("platformVersion", "12.2");
            options.AddAdditionalCapability("browserName", "Safari");

            RemoteWebDriver driver = new RemoteWebDriver(new Uri(TestDataProvider.SAUCE_SELENIUM_URL), options.ToCapabilities(), TimeSpan.FromMinutes(3));

            try
            {
                IWebDriver eyesDriver = eyes.Open(driver, "Applitools", "Open Check Close X2 SauceLabs (B)");
                driver.Url = "https://www.google.com";
                eyes.Check("Test iOS", Target.Window().SendDom(false));
                eyes.Close(false);
            }
            finally
            {
                // Close the browser.
                driver.Quit();

                // If the test was aborted before eyes.Close was called, ends the test as aborted.
                eyes.Abort();
            }
        }

    }
}
