using Applitools.Tests.Utils;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.IE;
using OpenQA.Selenium.Safari;
using System;
using System.Collections;

namespace Applitools.Selenium.Tests
{
    public static class TestDataProvider
    {
        public static readonly BatchInfo BatchInfo = new BatchInfo("DotNet Tests" + Environment.GetEnvironmentVariable("TEST_NAME_SUFFIX"));

        public static readonly string SAUCE_USERNAME = Environment.GetEnvironmentVariable("SAUCE_USERNAME");
        public static readonly string SAUCE_ACCESS_KEY = Environment.GetEnvironmentVariable("SAUCE_ACCESS_KEY");
        public static readonly string SAUCE_SELENIUM_URL = "https://ondemand.saucelabs.com:443/wd/hub";

        public static readonly string BROWSERSTACK_USERNAME = Environment.GetEnvironmentVariable("BROWSERSTACK_USERNAME");
        public static readonly string BROWSERSTACK_ACCESS_KEY = Environment.GetEnvironmentVariable("BROWSERSTACK_ACCESS_KEY");
        public static readonly string BROWSERSTACK_SELENIUM_URL = "http://hub-cloud.browserstack.com/wd/hub/";

        public static IEnumerable FixtureArgs
        {
            get
            {
                yield return new TestFixtureData(GetChromeOptions(), StitchModes.CSS).SetArgDisplayNames("Chrome", "CSS");
                yield return new TestFixtureData(GetChromeOptions(), StitchModes.Scroll).SetArgDisplayNames("Chrome", "Scroll");
                yield return new TestFixtureData(GetChromeOptions(), true).SetArgDisplayNames("Chrome", "VG");

                //if (TestUtils.RUNS_ON_CI)
                //{
                //    //yield return new TestFixtureData(GetFirefoxOptions(), StitchModes.CSS).SetArgDisplayNames("Firefox", "CSS");
                //    //yield return new TestFixtureData(GetFirefoxOptions(), StitchModes.Scroll).SetArgDisplayNames("Firefox", "CSS");
                //    yield return new TestFixtureData(GetIE11Options(), StitchModes.CSS).SetArgDisplayNames("IE", "CSS");
                //    yield return new TestFixtureData(GetIE11Options(), StitchModes.Scroll).SetArgDisplayNames("IE", "Scroll");
                //    //yield return new TestFixtureData(GetSafariOptions(), StitchModes.CSS).SetArgDisplayNames("Safari", "CSS");
                //    //yield return new TestFixtureData(GetSafariOptions(), StitchModes.Scroll).SetArgDisplayNames("Safari", "CSS");
                //}
            }
        }

        public static IEnumerable MobileDeviceFixtureArgs
        {
            get
            {
                yield return new TestFixtureData(GetGalaxyS8Options()).SetArgDisplayNames("Android - Galaxy S8");
                yield return new TestFixtureData(GetiOSSafariOptions()).SetArgDisplayNames("iOS - Safari");
            }
        }

        private static DriverOptions GetGalaxyS8Options()
        {
            ChromeOptions options = new ChromeOptions();
            options.AddAdditionalCapability("deviceName", "Samsung Galaxy S8 WQHD GoogleAPI Emulator");
            options.AddAdditionalCapability("platformVersion", "7.1");
            options.PlatformName = "Android";
            return options;
        }

        private static DriverOptions GetiOSSafariOptions()
        {
            SafariOptions options = new SafariOptions();
            options.AddAdditionalCapability("deviceName", "iPhone X Simulator");
            options.AddAdditionalCapability("platformVersion", "11.2");
            options.PlatformName = "iOS";
            return options;
        }

        private static DriverOptions GetIE11Options()
        {
            InternetExplorerOptions options = new InternetExplorerOptions();
            options.BrowserVersion = "11.0";
            return options;
        }

        public static DriverOptions GetFirefoxOptions()
        {
            FirefoxOptions options = new FirefoxOptions();
            if (TestUtils.RUN_HEADLESS)
            {
                options.AddArgument("--headless");
            }
            return options;
        }

        public static DriverOptions GetChromeOptions()
        {
            ChromeOptions options = new ChromeOptions();
            if (TestUtils.RUN_HEADLESS)
            {
                options.AddArgument("--headless");
            }
            options.AddArguments("--no-sandbox", "--verbose", "--disable-gpu");
            //options.AddArgument("--force-device-scale-factor=1.5");
            return options;
        }

        private static DriverOptions GetSafariOptions()
        {
            SafariOptions options = new SafariOptions();
            return options;
        }
    }
}