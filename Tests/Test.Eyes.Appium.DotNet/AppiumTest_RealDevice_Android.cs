using Applitools.Tests.Utils;
using Applitools.Utils.Cropping;
using Applitools.Utils.Geometry;
using Newtonsoft.Json;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Appium;
using OpenQA.Selenium.Appium.Android;
using OpenQA.Selenium.Appium.Enums;
using System;
using System.Collections.Generic;

namespace Applitools.Appium.Tests
{
    [TestFixture]
    [Parallelizable(ParallelScope.Fixtures)]
    public class TestAppium_RealDevice_Android : ReportingTestSuite
    {
        private Eyes eyes_;
        private AndroidDriver<AppiumWebElement> driver_;
        private ICutProvider cutProvider_ = null;

        private By firstElementSelector_ = MobileBy.AndroidUIAutomator("new UiSelector().index(0)");
        private By containerSelector_ = MobileBy.AndroidUIAutomator("new UiSelector().resourceId(\"com.applitools.helloworld.android:id/image_container\")");
        private By labelInContainerSelector_ = MobileBy.AndroidUIAutomator("new UiSelector().textContains(\"You successfully clicked the button!\")");
        private By imageInContainerSelector_ = MobileBy.AndroidUIAutomator("new UiSelector().resourceId(\"com.applitools.helloworld.android:id/image\")");
        private By buttonSelector_ = MobileBy.AndroidUIAutomator("new UiSelector().className(\"android.widget.Button\")");

        private HashSet<Region> expectedIgnoreRegions_ = new HashSet<Region>();

        [OneTimeSetUp]
        public new void OneTimeSetup()
        {
            eyes_ = new Eyes();

            eyes_.MatchTimeout = TimeSpan.FromSeconds(10);
            eyes_.Batch = TestDataProvider.BatchInfo;
            eyes_.SaveNewTests = false;
            driver_ = InitMobileDriverOnSauceLabs();
            //driver_ = InitMobileDriverOnBrowserStack();
            //driver_ = InitMobileDriverOnLocalDevice();
            
            IWebElement button = driver_.FindElement(buttonSelector_);
            button.Click();

            eyes_.CutProvider = cutProvider_;
        }

        [SetUp]
        public void TestSetup()
        {
            TestContext.TestAdapter testData = TestContext.CurrentContext.Test;
            TestUtils.SetupLogging(eyes_, testData.Name);
            eyes_.Open(driver_, "DotNet Tests", testData.Name);
            eyes_.Logger.Log("Android Driver Session Details: " + JsonConvert.SerializeObject(driver_.SessionDetails, Formatting.Indented));
        }

        [Test]
        public void Appium_Android_CheckWindow()
        {
            eyes_.Check("Window", Target.Window().Ignore(buttonSelector_));
            expectedIgnoreRegions_ = new HashSet<Region>() { new Region(136, 237, 90, 48) };
        }

        [Test]
        public void Appium_Android_CheckRegionWithIgnoreRegion()
        {
            var labelInContainer = driver_.FindElement(labelInContainerSelector_);
            eyes_.Check("Image Container", Target.Region(containerSelector_).Ignore(labelInContainer).Ignore(imageInContainerSelector_));
            expectedIgnoreRegions_ = new HashSet<Region>() { new Region(53, 0, 254, 22), new Region(0, 21, 360, 234) };
        }

        [Test]
        public void Appium_Android_CheckRegion()
        {
            eyes_.Check("Button", Target.Region(buttonSelector_));
            expectedIgnoreRegions_ = new HashSet<Region>();
        }

        [TearDown]
        public new void TearDown()
        {
            try
            {
                TestResults results = eyes_.Close();
                eyes_.Logger.Log("Mismatches: " + results.Mismatches);
                if (expectedIgnoreRegions_.Count > 0)
                {
                    var sessionResults = TestUtils.GetSessionResults(eyes_.ApiKey, results);
                    var ignoreRegions = sessionResults.ActualAppOutput[0].ImageMatchSettings.Ignore;
                    TestUtils.CompareSimpleRegionsList_(ignoreRegions, expectedIgnoreRegions_, "Ignore");
                }
            }
            finally
            {
                eyes_.AbortIfNotClosed();
            }
        }

        [OneTimeTearDown]
        public new void OneTimeTearDown()
        {
            driver_.Quit();
        }

        private AndroidDriver<AppiumWebElement> InitMobileDriverOnSauceLabs()
        {
            AppiumOptions options = new AppiumOptions();

            options.AddAdditionalCapability(MobileCapabilityType.AppiumVersion, "1.17.1");
            options.AddAdditionalCapability(MobileCapabilityType.PlatformName, "Android");
            options.AddAdditionalCapability(MobileCapabilityType.PlatformVersion, "7.0");
            options.AddAdditionalCapability(MobileCapabilityType.DeviceName, "Samsung Galaxy S8 FHD GoogleAPI Emulator");

            options.AddAdditionalCapability("phoneOnly", false);
            options.AddAdditionalCapability("tabletOnly", false);
            options.AddAdditionalCapability("privateDevicesOnly", false);

            options.AddAdditionalCapability(MobileCapabilityType.App, "https://applitools.bintray.com/Examples/eyes-android-hello-world.apk");

            options.AddAdditionalCapability("username", TestDataProvider.SAUCE_USERNAME);
            options.AddAdditionalCapability("accesskey", TestDataProvider.SAUCE_ACCESS_KEY);
            options.AddAdditionalCapability("name", "Android Demo");

            options.AddAdditionalCapability("idleTimeout", 300);

            AndroidDriver<AppiumWebElement> driver = new AndroidDriver<AppiumWebElement>(
                new Uri(TestDataProvider.SAUCE_SELENIUM_URL), options, TimeSpan.FromMinutes(5));

            //cutProvider_ = new FixedCutProvider(96, 192, 0, 0);

            return driver;
        }

        private AndroidDriver<AppiumWebElement> InitMobileDriverOnBrowserStack()
        {
            AppiumOptions options = new AppiumOptions();
            options.AddAdditionalCapability("browserstack.appium_version", "1.17.0");
            options.AddAdditionalCapability(MobileCapabilityType.PlatformName, "Android");
            options.AddAdditionalCapability(MobileCapabilityType.PlatformVersion, "8.0");
            options.AddAdditionalCapability(MobileCapabilityType.DeviceName, "Samsung Galaxy S8");

            options.AddAdditionalCapability("phoneOnly", false);
            options.AddAdditionalCapability("tabletOnly", false);
            options.AddAdditionalCapability("privateDevicesOnly", false);

            options.AddAdditionalCapability(MobileCapabilityType.App, "bs://c688cc8a02bce1bd23737537e546a1590e2cc274");

            options.AddAdditionalCapability("browserstack.user", TestDataProvider.BROWSERSTACK_USERNAME);
            options.AddAdditionalCapability("browserstack.key", TestDataProvider.BROWSERSTACK_ACCESS_KEY);

            options.AddAdditionalCapability("name", "Android Demo");

            options.AddAdditionalCapability("idleTimeout", 300);

            AndroidDriver<AppiumWebElement> driver = new AndroidDriver<AppiumWebElement>(
                new Uri(TestDataProvider.BROWSERSTACK_SELENIUM_URL), options, TimeSpan.FromMinutes(5));

            //cutProvider_ = new FixedCutProvider(96, 192, 0, 0);

            return driver;
        }

        private AndroidDriver<AppiumWebElement> InitMobileDriverOnLocalDevice()
        {
            AppiumOptions options = new AppiumOptions();
            options.AddAdditionalCapability(MobileCapabilityType.PlatformName, "Android");
            options.AddAdditionalCapability(MobileCapabilityType.PlatformVersion, "9.0");

            //options.AddAdditionalCapability(MobileCapabilityType.DeviceName, "Samsung Galaxy S8");
            options.AddAdditionalCapability(MobileCapabilityType.DeviceName, "emulator-5554");

            options.AddAdditionalCapability("phoneOnly", false);
            options.AddAdditionalCapability("tabletOnly", false);
            options.AddAdditionalCapability("privateDevicesOnly", false);

            options.AddAdditionalCapability(MobileCapabilityType.App, @"c:\Users\USER\Downloads\eyes-android-hello-world.apk");

            options.AddAdditionalCapability("idleTimeout", 300);

            AndroidDriver<AppiumWebElement> driver = new AndroidDriver<AppiumWebElement>(
                new Uri("http://127.0.0.1:4723/wd/hub"), options, TimeSpan.FromMinutes(5));

            return driver;
        }
    }
}
