using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Appium;
using OpenQA.Selenium.Appium.iOS;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Remote;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.Drawing;
using ExpectedConditions = SeleniumExtras.WaitHelpers.ExpectedConditions;

namespace Applitools.Selenium.Tests
{
    public class CustomerMobileTests
    {
        [Test]
        public void TestAndroidStitching()
        {
            ChromeOptions options = new ChromeOptions();
            ChromeMobileEmulationDeviceSettings mobileSettings = new ChromeMobileEmulationDeviceSettings();
            mobileSettings.PixelRatio = 4;
            mobileSettings.Width = 360;
            mobileSettings.Height = 740;
            mobileSettings.UserAgent = "Mozilla/5.0 (Linux; Android 8.0.0; SM-G960F Build/R16NW) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/64.0.3282.137 Mobile Safari/537.36";
            options.EnableMobileEmulation(mobileSettings);
            IWebDriver driver = new ChromeDriver(options);
            driver.Url = "https://silko11dev.outsystems.net/TestApp_AUTO_OutSystemsUIMobile/Adaptive";
            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(30));
            wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("#Columns2")));
            Eyes eyes = new Eyes();
            try
            {
                eyes.Open(driver, nameof(CustomerMobileTests), nameof(TestAndroidStitching));

                IWebElement element = driver.FindElement(By.ClassName("content"));
                eyes.Check(Target.Region(element).Fully());


                eyes.Close();
            }
            finally
            {
                eyes.AbortIfNotClosed();
                driver.Quit();
            }
        }

        [Test]
        public void TestIOSFeaturesUsingRemoteWebDriver()
        {
            // This is your api key, make sure you use it in all your tests.
            Eyes eyes = new Eyes();

            // Setup appium - Ensure the capabilities meets your environment.
            // Refer to http://appium.io documentation if required.
            DesiredCapabilities dc = new DesiredCapabilities();
            dc.SetCapability("platformName", "iOS");
            dc.SetCapability("browserName", "Safari");
            dc.SetCapability("platformVersion", "14");
            dc.SetCapability("deviceName", "iPad Pro 12.9 2020");

            dc.SetCapability("browserstack.user", TestDataProvider.BROWSERSTACK_USERNAME);
            dc.SetCapability("browserstack.key", TestDataProvider.BROWSERSTACK_ACCESS_KEY);

            IWebDriver driver = new RemoteWebDriver(new Uri(TestDataProvider.BROWSERSTACK_SELENIUM_URL), dc);
            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(60);


            try
            {
                driver.Url = "https://www.lampsplus.com/products/wall-lights";

                // Start the test.
                eyes.Open(driver, "Lampsplus Ticket", "37479_2");
                eyes.CheckWindow("ScreenShot", true);
                eyes.Close(false);
            }
            finally
            {
                eyes.AbortIfNotClosed();
                driver.Quit();
            }
        }

        [Test]
        public void TestIOSFeaturesUsingIOSDriver()
        {
            // This is your api key, make sure you use it in all your tests.
            Eyes eyes = new Eyes();

            // Setup appium - Ensure the capabilities meets your environment.
            // Refer to http://appium.io documentation if required.
            AppiumOptions options = new AppiumOptions();
            options.AddAdditionalCapability("platformName", "iOS");
            options.AddAdditionalCapability("browserName", "Safari");
            options.AddAdditionalCapability("platformVersion", "14");
            options.AddAdditionalCapability("deviceName", "iPad Pro 12.9 2020");
            options.AddAdditionalCapability("browserstack.user", TestDataProvider.BROWSERSTACK_USERNAME);
            options.AddAdditionalCapability("browserstack.key", TestDataProvider.BROWSERSTACK_ACCESS_KEY);

            IWebDriver driver = new IOSDriver<AppiumWebElement>(new Uri(TestDataProvider.BROWSERSTACK_SELENIUM_URL), options);
            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(60);


            try
            {
                driver.Url = "https://www.lampsplus.com/products/wall-lights";

                // Start the test.
                eyes.Open(driver, "Lampsplus Ticket", "37479_2");
                eyes.CheckWindow("ScreenShot", true);
                eyes.Close(false);
            }
            finally
            {
                eyes.AbortIfNotClosed();
                driver.Quit();
            }
        }
    }
}
