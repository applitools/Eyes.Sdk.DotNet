using System;
using Applitools.Tests.Utils;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Appium;
using OpenQA.Selenium.Appium.Android;
using OpenQA.Selenium.Appium.Enums;

namespace Applitools.Appium.Tests
{
    public class TestWebview
    {
        [Test]
        public void TestShouldSendWebview()
        {
            var eyes = new Eyes();
            AppiumDriver<AppiumWebElement> driver = new AndroidDriver<AppiumWebElement>
                (new Uri(TestDataProvider.SAUCE_SELENIUM_URL), GetPixel3aXL_(), TimeSpan.FromMinutes(5));
            driver.FindElement(By.Id("com.applitools.eyes.android:id/btn_web_view")).Click();
            eyes.Open(driver, "Applitools Eyes SDK", "TestWebview");
            eyes.Check(Target.Window().Webview());
            eyes.Check(Target.Window().Webview("WEBVIEW_com.applitools.eyes.android"));
            eyes.Close();
        }

        private static AppiumOptions GetPixel3aXL_()
        {
            AppiumOptions options = new AppiumOptions();
            options.AddAdditionalCapability(MobileCapabilityType.BrowserName, "");
            options.AddAdditionalCapability(MobileCapabilityType.PlatformName, "Android");
            options.AddAdditionalCapability(MobileCapabilityType.AppiumVersion, "1.20.2");
            options.AddAdditionalCapability(MobileCapabilityType.PlatformVersion, "10.0");
            options.AddAdditionalCapability(MobileCapabilityType.DeviceName, "Google Pixel 3a XL GoogleAPI Emulator");
            options.AddAdditionalCapability("username", TestDataProvider.SAUCE_USERNAME);
            options.AddAdditionalCapability("accesskey", TestDataProvider.SAUCE_ACCESS_KEY);
            options.AddAdditionalCapability("name", "Pixel 3a xl (c#)");
            options.AddAdditionalCapability(MobileCapabilityType.App, "https://applitools.jfrog.io/artifactory/Examples/android/1.3/app-debug.apk");
            return options;
        }
    }
}