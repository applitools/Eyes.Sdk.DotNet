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
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using Applitools.Metadata;
using Applitools.Utils;
using NUnit.Framework;

namespace Applitools.Generated.Selenium.Tests
{
    public abstract class TestSetupGenerated : FilteringTestSuite
    {
        protected IWebDriver driver;
        protected IWebDriver webDriver;
        protected SeleniumEyesRunner runner;
        protected Eyes eyes;
        protected string testedPageUrl = "https://applitools.github.io/demo/TestPages/FramesTestPage/";
        public static readonly BatchInfo BatchInfo = new BatchInfo("DotNet Generated Tests - Selenium");
        public static readonly string DRIVER_PATH = Environment.GetEnvironmentVariable("DRIVER_PATH");
        public static readonly string SAUCE_USERNAME = Environment.GetEnvironmentVariable("SAUCE_USERNAME");
        public static readonly string SAUCE_ACCESS_KEY = Environment.GetEnvironmentVariable("SAUCE_ACCESS_KEY");
        public static readonly string SAUCE_SELENIUM_URL = "https://ondemand.us-west-1.saucelabs.com:443/wd/hub";
        public static readonly string EG_SELENIUM_URL = Environment.GetEnvironmentVariable("EXECUTION_GRID_URL");
        public static readonly string LOCAL_SELENIUM_URL = "http://localhost:4444/wd/hub";
        public static readonly string LOCAL_FIREFOX_SELENIUM_URL = "http://localhost:4445/wd/hub";
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

        [SuppressMessage("ReSharper", "StringLiteralTypo")]
        protected void SetUpDriver(browserType browser = browserType.Chrome,
            bool legacy = false, bool headless = false, bool executionGrid = false)
        {
            switch (browser)
            {
                case browserType.Chrome:
                    if (executionGrid)
                    {
                        var chromeOptions = new ChromeOptions();
                        if (headless) chromeOptions.AddArgument("headless");
                        string egUrl = EG_SELENIUM_URL ?? Eyes.GetExecutionCloudUrl();
                        driver = new RemoteWebDriver(new Uri(egUrl), chromeOptions);
                    }
                    else
                    {
                        driver = CreateChromeDriver(headless: headless);
                    }
                    break;
                case browserType.Firefox:
                    //TestContext tc = TestContext.CurrentContext;
                    //logger_.Log(TraceLevel.Notice, Stage.TestFramework, StageType.Skipped,
                    //    new { message = "Firefox not supported for Selenium 4", testName = tc.Test.FullName });
                    //Assert.Inconclusive();
                    driver = CreateFirefoxDriver(headless: headless);
                    break;
                case browserType.IE:
                    var sauceOptions = new Dictionary<string, object>();
                    sauceOptions.Add("username", SAUCE_USERNAME);
                    sauceOptions.Add("accesskey", SAUCE_ACCESS_KEY);
                    var browserOptionsIe = new InternetExplorerOptions();
                    browserOptionsIe.PlatformName = "Windows 10";
                    browserOptionsIe.BrowserVersion = "11.285";
                    browserOptionsIe.AddAdditionalOption("sauce:options", sauceOptions);
                    driver = new RemoteWebDriver(new Uri(SAUCE_SELENIUM_URL), browserOptionsIe.ToCapabilities(), TimeSpan.FromMinutes(4));
                    break;
                case browserType.Edge:
                    var sauceOptionsEdge = new Dictionary<string, object>();
                    sauceOptionsEdge.Add("username", SAUCE_USERNAME);
                    sauceOptionsEdge.Add("accesskey", SAUCE_ACCESS_KEY);
                    var browserOptionsEdge = new EdgeOptions();
                    browserOptionsEdge.PlatformName = "Windows 10";
                    browserOptionsEdge.BrowserVersion = "18.17763";
                    browserOptionsEdge.AddAdditionalEdgeOption("sauce:options", sauceOptionsEdge);
                    driver = new RemoteWebDriver(new Uri(SAUCE_SELENIUM_URL), browserOptionsEdge.ToCapabilities(), TimeSpan.FromMinutes(4));
                    break;
                case browserType.Safari11:
                    var browserOptionsSafari11 = new SafariOptions();
                    SetDriverOptions_(ref browserOptionsSafari11, "macOS 10.12", "11.0");
                    driver = new RemoteWebDriver(new Uri(SAUCE_SELENIUM_URL), browserOptionsSafari11.ToCapabilities(), TimeSpan.FromMinutes(4));
                    break;
                case browserType.Safari12:
                    var browserOptionsSafari12 = new SafariOptions();
                    SetDriverOptions_(ref browserOptionsSafari12, "macOS 10.13", "12");
                    driver = new RemoteWebDriver(new Uri(SAUCE_SELENIUM_URL), browserOptionsSafari12.ToCapabilities(), TimeSpan.FromMinutes(4));
                    break;
                default:
                    throw new Exception("Unknown browser type");
            }
        }

        [SuppressMessage("ReSharper", "StringLiteralTypo")]
        private void SetDriverOptions_(ref SafariOptions driverOptions, string platformName, string browserVersion)
        {
            var sauceOptions = new Dictionary<string, object>
            {
                { "username", SAUCE_USERNAME },
                { "accesskey", SAUCE_ACCESS_KEY }
            };
            driverOptions.PlatformName = platformName;
            driverOptions.BrowserVersion = browserVersion;
            driverOptions.AddAdditionalOption("sauce:options", sauceOptions);
        }

        protected static ChromeDriver CreateChromeDriver(ChromeOptions options = null, bool headless = false)
        {
            ChromeDriver webDriver = RetryCreateWebDriver(() =>
            {
                options ??= new ChromeOptions();
                if (headless) options.AddArgument("--headless");

                ChromeDriver webDriverRet = DRIVER_PATH != null ? new ChromeDriver(DRIVER_PATH, options, TimeSpan.FromMinutes(4)) : new ChromeDriver(options);
                return webDriverRet;
            });
            return webDriver;
        }
        protected static RemoteWebDriver CreateChromeDriver2(ChromeOptions options = null, bool headless = false)
        {
            RemoteWebDriver webDriver = RetryCreateWebDriver(() =>
            {
                options ??= new ChromeOptions();
                if (headless) options.AddArgument("--headless");

                var driver1 = new RemoteWebDriver(new Uri(LOCAL_SELENIUM_URL), options);

                //ChromeDriver webDriverRet = DRIVER_PATH != null ? new ChromeDriver(DRIVER_PATH, options, TimeSpan.FromMinutes(4)) : new ChromeDriver(options);
                return driver1;
            });
            return webDriver;
        }

        protected static FirefoxDriver CreateFirefoxDriver(bool headless = false)
        {
            FirefoxDriver webDriver = RetryCreateWebDriver(() =>
            {
                var options = new FirefoxOptions();
                if (headless) options.AddArgument("--headless");
                FirefoxDriver webDriverRet = DRIVER_PATH != null ?  new FirefoxDriver(DRIVER_PATH, options) : new FirefoxDriver(options);
                return webDriverRet;
            });
            return webDriver;
        }

        protected static RemoteWebDriver CreateFirefoxDriver1(bool headless = false)
        {
            RemoteWebDriver webDriver = RetryCreateWebDriver(() =>
            {
                var options = new FirefoxOptions();
                if (headless) options.AddArgument("--headless");

                var driver1 = new RemoteWebDriver(new Uri(LOCAL_FIREFOX_SELENIUM_URL), options);

                //FirefoxDriver webDriverRet = new FirefoxDriver(DRIVER_PATH, options);
                return driver1;
            });
            return webDriver;
        }
        protected void InitEyes(bool isVisualGrid, bool isCSSMode)
        {
            string testName = NUnit.Framework.TestContext.CurrentContext.Test.MethodName;
            ILogHandler logHandler = TestUtils.InitLogHandler(testName);
            runner = isVisualGrid ? (SeleniumEyesRunner)(new VisualGridRunner(10, logHandler)) : new ClassicRunner(logHandler);
            eyes = new Applitools.Selenium.Eyes(runner);
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
        }
        
        protected void SetBrowsersInfo(params IRenderBrowserInfo[] browsersInfo)
        {
            var config = eyes.GetConfiguration();
            config.AddBrowsers(browsersInfo);
            eyes.SetConfiguration(config);
        }
        
        protected void SetAccessibilitySettings(AccessibilitySettings accessibilitySettings)
        {
            var config = eyes.GetConfiguration();
            config.SetAccessibilityValidation(accessibilitySettings);
            eyes.SetConfiguration(config);
        }

        protected void WaitBeforeCapture(int waitTimeMs)
        {
            var config = eyes.GetConfiguration();
            config.SetWaitBeforeCapture(waitTimeMs);
            eyes.SetConfiguration(config);
        }
        
        protected bool isStaleElementError(Exception errorObj)
        {
            return (errorObj is StaleElementReferenceException);
        }
        
        protected string getDomString(TestResults results, string domId)
        {
            return TestUtils.GetDom(Environment.GetEnvironmentVariable("APPLITOOLS_API_KEY_READ"), results, domId);
        }

        protected JObject GetDom(TestResults results, string domId)
        {
            string dom = getDomString(results, domId);
            return JObject.Parse(dom);
        }

        protected IList<JToken> GetNodesByAttributes(JToken dom, string attributeName)
        {
            return dom.SelectTokens($"$..[?(@.attributes['{attributeName}'])]").ToList();
        }
     
        protected void SetLayoutBreakpoints(LayoutBreakpointsOptions breakpointsOptions)
        {
            var config = eyes.GetConfiguration();
            config.SetLayoutBreakpoints(breakpointsOptions);
            eyes.SetConfiguration(config);   
        }

        protected void SetLayoutBreakpoints(params int[] breakpoints)
        {
            var config = eyes.GetConfiguration();
            config.SetLayoutBreakpoints(breakpoints);
            eyes.SetConfiguration(config);
        }
         
        protected void SetLayoutBreakpoints(bool enabled)
        {
            var config = eyes.GetConfiguration();
            config.SetLayoutBreakpoints(enabled);
            eyes.SetConfiguration(config);
        }

        protected void SetEnablePatterns(bool enable)
        {
            var config = eyes.GetConfiguration();
            config.SetEnablePatterns(enable);
            eyes.SetConfiguration(config);
        }

        protected SessionResults GetTestInfo(TestResults results)
        {
            SessionResults sessionResults = null;
            try
            {
                sessionResults = TestUtils.GetSessionResults(eyes.ApiKey, results);
            }
            catch (Exception e)
            {
                CommonUtils.LogExceptionStackTrace(logger_, Stage.TestFramework, StageType.TestResults, e);
                Assert.Fail("Exception appeared while getting session results");
            }

            ArgumentGuard.NotNull(sessionResults, nameof(sessionResults));
            return sessionResults;
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
