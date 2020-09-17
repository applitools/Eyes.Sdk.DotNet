using Applitools.Tests.Utils;
using Applitools.Utils.Cropping;
using Applitools.Utils.Geometry;
using Newtonsoft.Json;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Appium;
using OpenQA.Selenium.Appium.Enums;
using OpenQA.Selenium.Appium.iOS;
using System;
using System.Collections.Generic;

namespace Applitools.Appium.Tests
{
    [TestFixture]
    [Parallelizable(ParallelScope.Fixtures)]
    public class TestAppium_RealDevice_iOS : ReportingTestSuite
    {
        private Eyes eyes_;
        private IOSDriver<AppiumWebElement> driver_;
        private ICutProvider cutProvider_ = null;

        private By firstElementSelector_ = By.XPath("//XCUIElementTypeWindow[1]");
        private By containerSelector_ = MobileBy.AccessibilityId("BottomContainer");
        private By labelInContainerSelector_ = MobileBy.AccessibilityId("BottomLabel");
        private By imageInContainerSelector_ = MobileBy.AccessibilityId("BottomImage");
        private By buttonSelector_ = MobileBy.IosNSPredicate("type == 'XCUIElementTypeButton'");

        private HashSet<Region> expectedIgnoreRegions_ = new HashSet<Region>();

        [OneTimeSetUp]
        public new void OneTimeSetup()
        {
            eyes_ = new Eyes();

            eyes_.MatchTimeout = TimeSpan.FromSeconds(10);
            eyes_.Batch = TestDataProvider.BatchInfo;
            eyes_.SaveNewTests = false;

            driver_ = InitMobileDriverOnSauceLabs();
         
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
            eyes_.Logger.Log("iOS Driver Session Details: " + JsonConvert.SerializeObject(driver_.SessionDetails, Formatting.Indented));
        }

        [Test]
        public void Appium_iOS_CheckWindow()
        {
            eyes_.Check("Window", Target.Window().Ignore(buttonSelector_));
            expectedIgnoreRegions_ = new HashSet<Region>() { new Region(155, 258, 65, 30) };
        }

        [Test]
        public void Appium_iOS_CheckRegionWithIgnoreRegion()
        {
            var labelInContainer = driver_.FindElement(labelInContainerSelector_);
            eyes_.Check("Image Container", Target.Region(containerSelector_).Ignore(labelInContainer).Ignore(imageInContainerSelector_));
            expectedIgnoreRegions_ = new HashSet<Region>() { new Region(115, 35, 113, 65), new Region(0, 0, 343, 21) };
        }

        [Test]
        public void Appium_iOS_CheckRegion()
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

        private IOSDriver<AppiumWebElement> InitMobileDriverOnSauceLabs()
        {
            AppiumOptions options = new AppiumOptions();

            options.AddAdditionalCapability(MobileCapabilityType.AppiumVersion, "1.17.1");
            options.AddAdditionalCapability(MobileCapabilityType.PlatformName, "iOS");
            options.AddAdditionalCapability(MobileCapabilityType.PlatformVersion, "13.2");
            options.AddAdditionalCapability(MobileCapabilityType.DeviceName, "iPhone XS Simulator");

            options.AddAdditionalCapability("phoneOnly", false);
            options.AddAdditionalCapability("tabletOnly", false);
            options.AddAdditionalCapability("privateDevicesOnly", false);

            options.AddAdditionalCapability(MobileCapabilityType.App, "https://applitools.bintray.com/Examples/eyes-ios-hello-world/1.2/eyes-ios-hello-world.zip");

            options.AddAdditionalCapability("username", TestDataProvider.SAUCE_USERNAME);
            options.AddAdditionalCapability("accesskey", TestDataProvider.SAUCE_ACCESS_KEY);
            options.AddAdditionalCapability("name", "iOS Native Demo");

            options.AddAdditionalCapability("idleTimeout", 300);

            IOSDriver<AppiumWebElement> driver = new IOSDriver<AppiumWebElement>(
                new Uri(TestDataProvider.SAUCE_SELENIUM_URL), options, TimeSpan.FromMinutes(5));

            return driver;
        }

    }
}
