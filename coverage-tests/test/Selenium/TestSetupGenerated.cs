using Applitools.Selenium;
using Applitools.Tests.Utils;
using Applitools.VisualGrid;
using Newtonsoft.Json.Linq;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Edge;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.IE;
using OpenQA.Selenium.Remote;
using OpenQA.Selenium.Safari;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Applitools.Generated.Selenium.Tests
{
    public abstract class TestSetupGenerated// : ReportingTestSuiteGenerrated
    {

        protected IWebDriver driver;
        protected IWebDriver webDriver;
        protected EyesRunner runner;
        protected Eyes eyes;
        protected string testedPageUrl = "https://applitools.github.io/demo/TestPages/FramesTestPage/";
        public static readonly BatchInfo BatchInfo = new BatchInfo("DotNet Generated Tests");
        public static readonly string DRIVER_PATH = Environment.GetEnvironmentVariable("DRIVER_PATH");
        public static readonly string SAUCE_USERNAME = Environment.GetEnvironmentVariable("SAUCE_USERNAME");
        public static readonly string SAUCE_ACCESS_KEY = Environment.GetEnvironmentVariable("SAUCE_ACCESS_KEY");
        public static readonly string SAUCE_SELENIUM_URL = "https://ondemand.saucelabs.com:443/wd/hub";
        protected enum browserType
        {
            Chrome,
            IE,
            Edge,
            Firefox,
            Safari11,
            Safari12
        }
        protected DriverOptions options_;
        private SafariOptions browserOptions;
        private DesiredCapabilities caps;

        protected void SetUpDriver(browserType browser = browserType.Chrome, bool legacy = false, bool headless = false)
        {
            switch (browser)
            {
                case browserType.Chrome:
                    driver = CreateChromeDriver(headless: headless);
                    break;
                case browserType.Firefox:
                    driver = CreateFirefoxDriver(headless: headless);
                    break;
                case browserType.IE:
                    var sauceOptions = new Dictionary<string, object>();
                    sauceOptions.Add("username", SAUCE_USERNAME);
                    sauceOptions.Add("accesskey", SAUCE_ACCESS_KEY);
                    var browserOptionsIE = new InternetExplorerOptions();
                    browserOptionsIE.PlatformName = "Windows 10";
                    browserOptionsIE.BrowserVersion = "11.285";
                    browserOptionsIE.AddAdditionalCapability("sauce:options", sauceOptions, true);
                    driver = new RemoteWebDriver(new Uri(SAUCE_SELENIUM_URL), browserOptionsIE.ToCapabilities(), TimeSpan.FromMinutes(4));
                    break;
                case browserType.Edge:
                    var sauceOptionsEdge = new Dictionary<string, object>();
                    sauceOptionsEdge.Add("username", SAUCE_USERNAME);
                    sauceOptionsEdge.Add("accesskey", SAUCE_ACCESS_KEY);
                    var browserOptionsEdge = new EdgeOptions();
                    browserOptionsEdge.PlatformName = "Windows 10";
                    browserOptionsEdge.BrowserVersion = "18.17763";
                    browserOptionsEdge.AddAdditionalCapability("sauce:options", sauceOptionsEdge);
                    driver = new RemoteWebDriver(new Uri(SAUCE_SELENIUM_URL), browserOptionsEdge.ToCapabilities(), TimeSpan.FromMinutes(4));
                    break;
                case browserType.Safari11:
                    if (legacy)
                    {
                        setDesiredCapabilities("macOS 10.12", "Safari", "11.0");
                        driver = new RemoteWebDriver(new Uri(SAUCE_SELENIUM_URL), caps, TimeSpan.FromMinutes(4));
                    }
                    else
                    {
                        browserOptions = new SafariOptions();
                        setDriverOptions(ref browserOptions, "macOS 10.12", "11.0");
                        driver = new RemoteWebDriver(new Uri(SAUCE_SELENIUM_URL), browserOptions.ToCapabilities(), TimeSpan.FromMinutes(4));
                    }
                    break;
                case browserType.Safari12:
                    if (legacy)
                    {
                        setDesiredCapabilities("macOS 10.13", "Safari", "12.1");
                        driver = new RemoteWebDriver(new Uri(SAUCE_SELENIUM_URL), caps, TimeSpan.FromMinutes(4));
                    }
                    else
                    {
                        browserOptions = new SafariOptions();
                        setDriverOptions(ref browserOptions, "macOS 10.13", "12.1");
                        driver = new RemoteWebDriver(new Uri(SAUCE_SELENIUM_URL), browserOptions.ToCapabilities(), TimeSpan.FromMinutes(4));
                    }
                    break;
                default:
                    throw new Exception("Unknown browser type");
            }
        }

        private void setDriverOptions(ref SafariOptions driverOptions, string PlatformName, string BrowserVersion)
        {
            var sauceOptions = new Dictionary<string, object>();
            sauceOptions.Add("username", SAUCE_USERNAME);
            sauceOptions.Add("accesskey", SAUCE_ACCESS_KEY);
            driverOptions.PlatformName = PlatformName;
            driverOptions.BrowserVersion = BrowserVersion;
            driverOptions.AddAdditionalCapability("username", SAUCE_USERNAME);
            driverOptions.AddAdditionalCapability("accesskey", SAUCE_ACCESS_KEY);
        }

        private void setDesiredCapabilities(string PlatformName, string BrowserName, string BrowserVersion)
        {
            caps = new DesiredCapabilities();
            caps.SetCapability("browserName", BrowserName);
            caps.SetCapability("platform", PlatformName);
            caps.SetCapability("version", BrowserVersion);
            caps.SetCapability("seleniumVersion", "3.4.0");
            caps.SetCapability("username", SAUCE_USERNAME);
            caps.SetCapability("accesskey", SAUCE_ACCESS_KEY);
        }

        protected static ChromeDriver CreateChromeDriver(ChromeOptions options = null, bool headless = false)
        {
            ChromeDriver webDriver = RetryCreateWebDriver(() =>
            {
                if (options == null)
                {
                    options = new ChromeOptions();
                }
                if (headless) options.AddArgument("--headless");

                ChromeDriver webDriverRet = DRIVER_PATH != null ? new ChromeDriver(DRIVER_PATH, options, TimeSpan.FromMinutes(4)) : new ChromeDriver(options);
                return webDriverRet;
            });
            return webDriver;
        }

        protected static FirefoxDriver CreateFirefoxDriver(bool headless = false)
        {
            FirefoxDriver webDriver = RetryCreateWebDriver(() =>
            {
                var options = new FirefoxOptions();
                if (headless) options.AddArgument("--headless");
                FirefoxDriver webDriverRet = new FirefoxDriver(DRIVER_PATH, options);
                return webDriverRet;
            });
            return webDriver;
        }

        protected void initEyes(bool isVisualGrid, bool isCSSMode)
        {
            runner = isVisualGrid ? (EyesRunner)(new VisualGridRunner(10)) : new ClassicRunner();
            eyes = new Eyes(runner);
            initEyesSettings(isVisualGrid, isCSSMode);
        }

        protected void initEyesSettings(bool isVisualGrid, bool isCSSMode)
        {
            eyes.Batch = BatchInfo;
            if (!isVisualGrid) eyes.StitchMode = isCSSMode ? StitchModes.CSS : StitchModes.Scroll;
            eyes.BranchName = "master";
            eyes.ParentBranchName = "master";
            eyes.SaveNewTests = false;
            //eyes.AddProperty("ForceFPS", eyes.ForceFullPageScreenshot ? "true" : "false");
            //eyes.AddProperty("Agent ID", eyes.FullAgentId);
            eyes.HideScrollbars = true;
            eyes.SetLogHandler(TestUtils.InitLogHandler());
        }


        protected bool isStaleElementError(Exception errorObj)
        {
            return (errorObj is StaleElementReferenceException);
        }

        protected string getDomString(TestResults results, string domId)
        {
            return TestUtils.GetDom(Environment.GetEnvironmentVariable("APPLITOOLS_API_KEY_READ"), results, domId);
        }

        protected JObject getDom(TestResults results, string domId)
        {
            string dom = getDomString(results, domId);
            return JObject.Parse(dom);
        }

        protected IList<JToken> getNodesByAttribute(JObject dom, string attributeName)
        {
            return dom.SelectTokens($"$..[?(@.attributes['{attributeName}'])]").ToList();
        }

        public static T RetryCreateWebDriver<T>(Func<T> func, int times = 3) where T : class, IWebDriver
        {
            int retriesLeft = times;
            int wait = 500;
            while (retriesLeft-- > 0)
            {
                try
                {
                    T result = func.Invoke();
                    if (result != null) return result;
                }
                catch (Exception)
                {
                    if (retriesLeft == 0) throw;
                }
                Thread.Sleep(wait);
                wait *= 2;
                wait = Math.Min(10000, wait);
            }
            return null;
        }
    }
}
