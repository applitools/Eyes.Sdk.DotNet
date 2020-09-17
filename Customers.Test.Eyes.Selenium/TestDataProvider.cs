using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.IE;
using OpenQA.Selenium.Remote;
using OpenQA.Selenium.Safari;
using System;
using System.Collections;

namespace Applitools.Selenium.Tests
{
    public class TestDataProvider
    {
        public static readonly BatchInfo BatchInfo = new BatchInfo("DotNet Tests" + Environment.GetEnvironmentVariable("TEST_NAME_SUFFIX"));
        
        public static readonly string SAUCE_USERNAME = Environment.GetEnvironmentVariable("SAUCE_USERNAME");
        public static readonly string SAUCE_ACCESS_KEY = Environment.GetEnvironmentVariable("SAUCE_ACCESS_KEY");
        public static readonly string SAUCE_SELENIUM_URL = "https://ondemand.saucelabs.com:443/wd/hub";

        public static IEnumerable FixtureArgs
        {
            get
            {
                yield return new TestFixtureData(getChromeCapabilities_());
                //yield return new TestFixtureData(getFirefoxCapabilities_());
                //yield return new TestFixtureData(getSafariCapabilities_());
            }
        }

        private static object getIE11Capabilities_()
        {
            InternetExplorerOptions options = new InternetExplorerOptions();
            options.BrowserVersion = "11.0";
            ICapabilities capabilities = options.ToCapabilities();
            return capabilities;
        }

        private static object getFirefoxCapabilities_()
        {
            FirefoxOptions options = new FirefoxOptions();
            if (Utils.SeleniumUtils.RUN_HEADLESS)
            {
                options.AddArgument("--headless");
            }
            ICapabilities capabilities = options.ToCapabilities();
            return capabilities;
        }

        private static object getChromeCapabilities_()
        {
            ChromeOptions options = new ChromeOptions();
            options.AddArgument("disable-infobars");
            if (Utils.SeleniumUtils.RUN_HEADLESS)
            {
                options.AddArgument("headless");
            }
            //options.AddArgument("--force-device-scale-factor=2");
            ICapabilities cpabilities = options.ToCapabilities();
            return cpabilities;
        }

        private static object getSafariCapabilities_()
        {
            SafariOptions options = new SafariOptions();
            ICapabilities cpabilities = options.ToCapabilities();
            return cpabilities;
        }
    }
}