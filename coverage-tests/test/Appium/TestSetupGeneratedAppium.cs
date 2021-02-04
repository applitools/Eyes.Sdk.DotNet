using System;
using System.Collections.Generic;
using OpenQA.Selenium.Appium;
using OpenQA.Selenium.Appium.Android;
using OpenQA.Selenium.Appium.iOS;
using OpenQA.Selenium.Appium.Enums;
using OpenQA.Selenium.Remote;
using OpenQA.Selenium;
using Applitools.Appium;
using Applitools.Generated.Selenium.Tests;
using Applitools.Tests.Utils;

namespace Applitools.Generated.Appium.Tests
{
	public abstract class TestSetupGeneratedAppium
	{

		protected RemoteWebDriver driver;
		protected IWebDriver webDriver;
		protected EyesRunner runner;
		protected Eyes eyes;
		protected string testedPageUrl = "https://applitools.github.io/demo/TestPages/FramesTestPage/";
		public static readonly BatchInfo BatchInfo = new BatchInfo("DotNet Generated Tests");
		public static readonly string DRIVER_PATH = Environment.GetEnvironmentVariable("DRIVER_PATH");


		protected void initEyes(bool isVisualGrid, bool isCSSMode)
		{
			eyes = new Applitools.Appium.Eyes();
			eyes.MatchTimeout = TimeSpan.FromSeconds(10);
			eyes.Batch = BatchInfo;
			eyes.BranchName = "master";
			eyes.SaveNewTests = false;
            string testName = NUnit.Framework.TestContext.CurrentContext.Test.MethodName;
            ILogHandler logHandler = TestUtils.InitLogHandler(testName);
			eyes.SetLogHandler(logHandler);
		}

		protected void initDriver(string device, string app)
		{
			AppiumOptions options = new AppiumOptions();
			options.AddAdditionalCapability(MobileCapabilityType.AppiumVersion, "1.17.1");
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
			options.AddAdditionalCapability("name", "Android Demo");

			options.AddAdditionalCapability("idleTimeout", 300);

			switch (DEVICES[device]["platformName"])
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
			}
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
	}
}
