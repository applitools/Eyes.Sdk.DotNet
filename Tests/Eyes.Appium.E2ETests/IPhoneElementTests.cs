using System;
using System.Collections.Generic;
using Applitools.Appium;
using NUnit.Framework;
using OpenQA.Selenium.Appium;
using OpenQA.Selenium.Appium.iOS;

namespace Eyes.Appium.E2ETests
{
    [TestFixture]
    public class IPhoneElementTests
    {
        private AppiumDriver<AppiumWebElement> driver;
        private Applitools.Appium.Eyes eyes;

        //[Test]
        public void IPhoneLayoutElementTest()
        {
            eyes = new Applitools.Appium.Eyes();
            AppiumOptions options = new AppiumOptions();
            options.AddAdditionalCapability("platformName", "iOS");
            options.AddAdditionalCapability("browserName", "Safari");
            options.AddAdditionalCapability("appium:deviceName", "iPhone 12 Pro Simulator");
            options.AddAdditionalCapability("appium:automationName", "XCUITest");
            options.AddAdditionalCapability("appium:platformVersion", "14.5");
            options.AddAdditionalCapability("username", "applitools-dev");
            options.AddAdditionalCapability("accessKey", Environment.GetEnvironmentVariable("SAUCE_ACCESS_KEY"));

            options.AddAdditionalCapability("idleTimeout", 300);

            var sauceOptions = new Dictionary<string, object>();
            //sauceOptions.Add("appiumVersion", "2.0.0-beta44");
            sauceOptions.Add("build", "<your build id>");
            sauceOptions.Add("name", "<your test name>");
            sauceOptions.Add("username", "applitools-dev");
            sauceOptions.Add("accessKey", Environment.GetEnvironmentVariable("SAUCE_ACCESS_KEY"));
            options.AddAdditionalCapability("sauce:options", sauceOptions);

            var url = new Uri("https://ondemand.us-west-1.saucelabs.com:443/wd/hub");
            var driver = new IOSDriver<AppiumWebElement>(url, options, TimeSpan.FromMinutes(5));

            driver.Url = "https://www.lampsplus.com/products/737T0";

            eyes.Open(driver, "Applitools Eyes SDK", "Appium C#");

            var shipsElement = driver.FindElementByCssSelector(".shipsInMessage");

            eyes.Check(Target.Window().Fully().Layout(shipsElement)); //Fails

            eyes.Close(false);
        }

        //[Test]
        public void IPhoneRegionElementTest()
        {
            eyes = new Applitools.Appium.Eyes();
            AppiumOptions options = new AppiumOptions();
            options.AddAdditionalCapability("platformName", "iOS");
            options.AddAdditionalCapability("browserName", "Safari");
            options.AddAdditionalCapability("appium:deviceName", "iPhone 12 Pro Simulator");
            options.AddAdditionalCapability("appium:automationName", "XCUITest");
            options.AddAdditionalCapability("appium:platformVersion", "14.5");
            options.AddAdditionalCapability("username", "applitools-dev");
            options.AddAdditionalCapability("accessKey", Environment.GetEnvironmentVariable("SAUCE_ACCESS_KEY"));

            options.AddAdditionalCapability("idleTimeout", 300);

            var sauceOptions = new Dictionary<string, object>();
            //sauceOptions.Add("appiumVersion", "2.0.0-beta44");
            sauceOptions.Add("build", "<your build id>");
            sauceOptions.Add("name", "<your test name>");
            sauceOptions.Add("username", "applitools-dev");
            sauceOptions.Add("accessKey", Environment.GetEnvironmentVariable("SAUCE_ACCESS_KEY"));
            options.AddAdditionalCapability("sauce:options", sauceOptions);

            var url = new Uri("https://ondemand.us-west-1.saucelabs.com:443/wd/hub");
            var driver = new IOSDriver<AppiumWebElement>(url, options, TimeSpan.FromMinutes(5));

            driver.Url = $"https://www.lampsplus.com/products/737T0";

            eyes.Open(driver, "Applitools Eyes SDK", "Appium C#");

            var shipsElement = driver.FindElementByCssSelector(".shipsInMessage");

            eyes.Check($"AndroidRegionElementTest", Target.Region(shipsElement).IgnoreDisplacements());

            eyes.Close(false);
        }
        
        [TearDown]
        public void AfterEach()
        {
            // Close the browser if driver isn't null.
            driver?.Quit();

            // If the test was aborted before eyes.close was called, ends the test as aborted.
            eyes.AbortIfNotClosed();
        }
    }
}