using System;

namespace Applitools.Appium.Tests
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
    }
}