using System;
using System.Collections.Generic;
using System.Drawing;
using Applitools.Selenium;
using Applitools.Selenium.Tests;
using Applitools.Tests.Utils;
using Eyes.Universal.Sdk.DotNet;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.IE;
using OpenQA.Selenium.Remote;

namespace Test.Eyes.Selenium.DotNet.UniversalSdkTests
{
    [Parallelizable(ParallelScope.All)]
    public class TestUniversalSdkBrowserStack : ReportingTestSuite
    {
        [Test]
        public void TestIE11()
        {
            var usk = new UniversalSocketClient();
            //TestUtils.SetupLogging(eyes);
            
            InternetExplorerOptions options = new InternetExplorerOptions();
            options.BrowserVersion = "11.0";
            options.AddAdditionalCapability("resolution", "1024x768");

            Dictionary<string, object> browserstackOptions = new Dictionary<string, object>
            {
                {"os", "Windows"},
                {"osVersion", "10"},
                {"userName", TestDataProvider.BROWSERSTACK_USERNAME},
                {"accessKey", TestDataProvider.BROWSERSTACK_ACCESS_KEY}
            };

            options.AddAdditionalCapability("bstack:options", browserstackOptions, true);

            IWebDriver driver = null;
            try
            {
                driver = new RemoteWebDriver(new Uri(TestDataProvider.BROWSERSTACK_SELENIUM_URL), options);
                driver.Url = "https://applitools.github.io/demo/TestPages/FramesTestPage/";

                usk.Open(driver, "TestUniversalSdkBrowserStack", "TesIE11", new Size(800, 600));
                usk.Check("viewport", Target.Window().Fully(false).SendDom(false));
                usk.Check("full page", Target.Window().Fully(true).SendDom(false));
                usk.Close();
            }
            finally
            {
                driver?.Quit();
            }
        }
    }
}
