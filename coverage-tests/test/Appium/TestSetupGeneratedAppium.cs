using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using Applitools.Metadata;
using Applitools.Selenium;
using OpenQA.Selenium.Appium;
using OpenQA.Selenium.Appium.Android;
using OpenQA.Selenium.Appium.iOS;
using OpenQA.Selenium.Appium.Enums;
using OpenQA.Selenium.Remote;
using OpenQA.Selenium;
using Applitools.Tests.Utils;
using Applitools.Utils;
using NUnit.Framework;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Edge;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.IE;
using OpenQA.Selenium.Safari;

namespace Applitools.Generated.Appium.Tests
{
	public abstract class TestSetupGeneratedAppium : FilteringTestSuite
	{
		protected enum browserType
		{
			Chrome,
			IE,
			Edge,
			Firefox,
			Safari11,
			Safari12
		}
		
		protected RemoteWebDriver driver;
		protected IWebDriver webDriver;
		protected EyesRunner runner;
		protected Applitools.Appium.Eyes eyes;
		protected string testedPageUrl = "https://applitools.github.io/demo/TestPages/FramesTestPage/";
		public static readonly BatchInfo BatchInfo = new BatchInfo("DotNet Generated Tests - Appium");
		public static readonly string DRIVER_PATH = Environment.GetEnvironmentVariable("DRIVER_PATH");
		public static readonly string SAUCE_USERNAME = Environment.GetEnvironmentVariable("SAUCE_USERNAME");
		public static readonly string SAUCE_ACCESS_KEY = Environment.GetEnvironmentVariable("SAUCE_ACCESS_KEY");
		public static readonly string SAUCE_SELENIUM_URL = "https://ondemand.us-west-1.saucelabs.com:443/wd/hub";
		public static readonly string EG_SELENIUM_URL = Environment.GetEnvironmentVariable("EXECUTION_GRID_URL");
		public static readonly string LOCAL_SELENIUM_URL = "http://localhost:4444/wd/hub";
		public static readonly string LOCAL_FIREFOX_SELENIUM_URL = "http://localhost:4445/wd/hub";
		
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
                    browserOptionsIe.AddAdditionalCapability("sauce:options", sauceOptions, true);
                    driver = new RemoteWebDriver(new Uri(SAUCE_SELENIUM_URL), browserOptionsIe.ToCapabilities(), TimeSpan.FromMinutes(4));
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

		protected void InitEyes(bool isVisualGrid, bool isCSSMode)
		{
			eyes = new Applitools.Appium.Eyes();
			eyes.MatchTimeout = TimeSpan.FromSeconds(10);
			eyes.Batch = BatchInfo;
			eyes.BranchName = "master";
			eyes.SaveNewTests = false;
            string testName = NUnit.Framework.TestContext.CurrentContext.Test.MethodName;
            ILogHandler logHandler = TestUtils.InitLogHandler(testName);
			//eyes.SetLogHandler(logHandler);
		}

		protected void InitDriver(string device, string app)
		{
			AppiumOptions options = new AppiumOptions();
			//options.AddAdditionalCapability(MobileCapabilityType.AppiumVersion, "1.17.1");
			options.AddAdditionalCapability(MobileCapabilityType.PlatformName, DEVICES[device]["platformName"]);
			options.AddAdditionalCapability(MobileCapabilityType.PlatformVersion, DEVICES[device]["platformVersion"]);
			options.AddAdditionalCapability(MobileCapabilityType.DeviceName, DEVICES[device]["deviceName"]);
			if (DEVICES[device].ContainsKey("deviceOrientation")) options.AddAdditionalCapability("deviceOrientation", DEVICES[device]["deviceOrientation"]);
			else options.AddAdditionalCapability("deviceOrientation", "portrait");
			options.AddAdditionalCapability("browserName", "");

			options.AddAdditionalCapability("phoneOnly", false);
			options.AddAdditionalCapability("tabletOnly", false);
			options.AddAdditionalCapability("privateDevicesOnly", false);

			options.AddAdditionalCapability(MobileCapabilityType.App, app);

			string url = null;
			if (DEVICES[device].ContainsKey("sauce"))
			{ 
				options.AddAdditionalCapability("username", CREDENTIALS["sauce"]["username"]);
				options.AddAdditionalCapability("accesskey", CREDENTIALS["sauce"]["access_key"]);
				url = SAUCE_SERVER_URL;
			}

			string platformName = (string)DEVICES[device]["platformName"];
			options.AddAdditionalCapability("name", $"{platformName} Demo");

			options.AddAdditionalCapability("idleTimeout", 300);

			switch (platformName)
			{
                case "Android":
                    driver = new AndroidDriver<AppiumWebElement>(
                    new Uri(url), options, TimeSpan.FromMinutes(5));
                    break;
                case "iOS":
					driver = new IOSDriver<AppiumWebElement>(
					new Uri(url), options, TimeSpan.FromMinutes(5));
					break;
			}
		}

		private Dictionary<string, Dictionary<string, object>> DEVICES = new Dictionary<string, Dictionary<string, object>>
		{
			{ "Android Emulator", new Dictionary<string, object>
				{
					{ "deviceName", "Android Emulator" },
					{ "platformName", "Android" },
					{ "platformVersion", "6.0" },
					{ "deviceOrientation", "landscape" },
					{ "clearSystemFiles", true },
					{ "noReset", true },
					{ "url", SAUCE_SERVER_URL },
					{ "sauce", true },
					{ "name", "Android Demo" }

				}
			},
			{ "Samsung Galaxy S8", new Dictionary<string, object>
				{
					{ "browserName", ""},
					{ "deviceName", "Samsung Galaxy S8 FHD GoogleAPI Emulator" },
					{ "platformName", "Android" },
					{ "platformVersion", "8.1" },
					{ "username", Environment.GetEnvironmentVariable("SAUCE_USERNAME")},
					{ "access_key", Environment.GetEnvironmentVariable("SAUCE_ACCESS_KEY")},
					{ "url", SAUCE_SERVER_URL },
					{ "sauce", true },
					{ "name", "Android Demo" }
				}
			},
			{ "iPhone XS", new Dictionary<string, object>
				{
					{ "browserName", ""},
					{ "deviceName", "iPhone XS Simulator" },
					{ "platformName", "iOS" },
					{ "platformVersion", "13.2" },
					{ "username", Environment.GetEnvironmentVariable("SAUCE_USERNAME")},
					{ "access_key", Environment.GetEnvironmentVariable("SAUCE_ACCESS_KEY")},
					{ "url", SAUCE_SERVER_URL },
					{ "sauce", true },
					{ "name", "iOS Native Demo" }
				}
			},
			{"Pixel 3 XL", new Dictionary<string, object>
				{
					{ "browserName", ""},
					{ "deviceName", "Google Pixel 3 XL GoogleAPI Emulator" },
					{ "platformName", "Android" },
					{ "platformVersion", "11" },
					{ "automationName", "UiAutomator2" },
					{ "username", Environment.GetEnvironmentVariable("SAUCE_USERNAME")},
					{ "access_key", Environment.GetEnvironmentVariable("SAUCE_ACCESS_KEY")},
					{ "url", SAUCE_SERVER_URL },
					{ "sauce", true },
					{ "name", "Android Native Demo" }
				}
			},
			{"Pixel 3a XL", new Dictionary<string, object>
				{
					{ "deviceName", "Google Pixel 3a XL GoogleAPI Emulator" },
					{ "platformName", "Android" },
					{ "platformVersion", "11" },
					{ "automationName", "UiAutomator2" },
					{ "username", Environment.GetEnvironmentVariable("SAUCE_USERNAME")},
					{ "access_key", Environment.GetEnvironmentVariable("SAUCE_ACCESS_KEY")},
					{ "url", SAUCE_SERVER_URL },
					{ "sauce", true },
					{ "name", "Android Native Demo" }
					
				}
			},
			{ "iPhone 12", new Dictionary<string, object>
				{
					{ "deviceName", "iPhone 12 Simulator" },
					{ "platformName", "iOS" },
					{ "platformVersion", "15" },
					{ "automationName", "XCUITest" },
					{ "username", Environment.GetEnvironmentVariable("SAUCE_USERNAME")},
					{ "access_key", Environment.GetEnvironmentVariable("SAUCE_ACCESS_KEY")},
					{ "url", SAUCE_SERVER_URL },
					{ "sauce", true },
					{ "name", "iOS Native Demo" }
				}
			},
		};

		const string SAUCE_SERVER_URL = "https://ondemand.saucelabs.com:443/wd/hub";

		private static readonly Dictionary<string, Dictionary<string, string>> CREDENTIALS = new Dictionary<string, Dictionary<string, string>>
		{
			{ "sauce", new Dictionary<string, string>
				{
					{"username", Environment.GetEnvironmentVariable("SAUCE_USERNAME") },
					{"access_key", Environment.GetEnvironmentVariable("SAUCE_ACCESS_KEY") }
				}
			}
		};
		
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
			driverOptions.AddAdditionalCapability("sauce:options", sauceOptions);
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
