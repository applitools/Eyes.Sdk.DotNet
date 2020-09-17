namespace Applitools.Appium.Tests
{
    using System;
    using OpenQA.Selenium;
    using OpenQA.Selenium.Appium.Enums;
    using OpenQA.Selenium.Chrome;
    using OpenQA.Selenium.Remote;

    class AppiumTest
    {
        public static TestResults CheckWebsite(RemoteWebDriver driver, Eyes eyes)
        {
            driver = eyes.Open(driver, ".NET SDK", "Mobile Chrome");

            driver.Navigate().GoToUrl("https://applitools.com");
            eyes.CheckWindow("Home");
            driver.FindElement(By.ClassName("automated")).Click();
            eyes.CheckWindow("Pricing");
            return eyes.Close(false);
        }

        public static void Main(string[] argv)
        {
            ChromeOptions options = new ChromeOptions();
            options.AddAdditionalCapability(MobileCapabilityType.DeviceName, "Nexus S5");
            options.AddAdditionalCapability(MobileCapabilityType.PlatformName, "Android");
            options.AddAdditionalCapability(MobileCapabilityType.PlatformVersion, "5.1.0");
            options.AddAdditionalCapability(MobileCapabilityType.NewCommandTimeout, 600);
            options.AddAdditionalCapability("idleTimeout", 900);
            RemoteWebDriver driver = new RemoteWebDriver(new Uri("http://192.168.57.1:4723/wd/hub/"), options);

            var eyes = new Eyes();
            try
            {
                CheckWebsite(driver, eyes);
                // Set wrong scale ratio and run test again.
                eyes.ScaleRatio = 2;
                CheckWebsite(driver, eyes);
                // Go back to automatic scale ratio and run the test again.
                eyes.ScaleRatio = 0;
                CheckWebsite(driver, eyes);
            }
            finally
            {
                eyes.AbortIfNotClosed();
                driver.Quit();
            }
        }
    }
}
