using Applitools.Selenium.Tests.Utils;
using Applitools.Tests.Utils;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.IE;
using OpenQA.Selenium.Remote;
using System;
using System.Collections.Generic;
using System.Drawing;
using OpenQA.Selenium.Safari;

namespace Applitools.Selenium.Tests
{
    [Parallelizable(ParallelScope.All)]
    public class TestBrowserStack : ReportingTestSuite
    {
        [TestCase(StitchModes.CSS)]
        [TestCase(StitchModes.Scroll)]
        public void TestIE11(StitchModes stitchMode)
        {
            Eyes eyes = new Eyes();
            TestUtils.SetupLogging(eyes);

            InternetExplorerOptions options = new InternetExplorerOptions();
            options.BrowserVersion = "11.0";
            options.AddAdditionalCapability("resolution", "1024x768");

            Dictionary<string, object> browserstackOptions = new Dictionary<string, object>();
            browserstackOptions.Add("os", "Windows");
            browserstackOptions.Add("osVersion", "10");
            browserstackOptions.Add("userName", TestDataProvider.BROWSERSTACK_USERNAME);
            browserstackOptions.Add("accessKey", TestDataProvider.BROWSERSTACK_ACCESS_KEY);

            options.AddAdditionalCapability("bstack:options", browserstackOptions, true);


            IWebDriver driver = null;
            try
            {
                driver = new RemoteWebDriver(new Uri(TestDataProvider.BROWSERSTACK_SELENIUM_URL), options);
                driver.Url = "https://applitools.github.io/demo/TestPages/FramesTestPage/";
                eyes.StitchMode = stitchMode;
                eyes.Batch = TestDataProvider.BatchInfo;
                eyes.Open(driver, "TestBrowserStack", "TesIE11_" + stitchMode.ToString().ToUpper(), new Size(800, 600));
                eyes.Check("viewport", Target.Window().Fully(false).SendDom(false));
                eyes.Check("full page", Target.Window().Fully(true).SendDom(false));
                eyes.Close();
            }
            finally
            {
                driver?.Quit();
                eyes.Abort();
            }
        }

        [TestCase(StitchModes.CSS)]
        [TestCase(StitchModes.Scroll)]
        public void SafariTest(StitchModes stitchMode)
        {
            var eyes = new Eyes
            {
                StitchMode = stitchMode
            };
            TestUtils.SetupLogging(eyes);

            SafariOptions capability = new SafariOptions();
            capability.AddAdditionalCapability("os_version", "14");
            capability.AddAdditionalCapability("device", "iPhone 12");
            capability.AddAdditionalCapability("real_mobile", "true");
            capability.AddAdditionalCapability("browserstack.local", "false");
            capability.AddAdditionalCapability("browserstack.user", TestDataProvider.BROWSERSTACK_USERNAME);
            capability.AddAdditionalCapability("browserstack.key", TestDataProvider.BROWSERSTACK_ACCESS_KEY);

            RemoteWebDriver driver = null;
            try
            {
                driver = new RemoteWebDriver(new Uri(TestDataProvider.BROWSERSTACK_SELENIUM_URL), capability);
                driver.Url = "https://applitools.github.io/demo/TestPages/FramesTestPage/";
                eyes.Open(driver, "TestBrowserStack", $"Test Safari {stitchMode}");
                eyes.Check("viewport", Target.Window().Fully());
                eyes.Check("full page", Target.Window().Fully());
                eyes.Close();
            }
            finally
            {
                eyes.AbortIfNotClosed();
                driver?.Quit();
            }
        }
    }
}