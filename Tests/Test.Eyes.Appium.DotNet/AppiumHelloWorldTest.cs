using System;
using Applitools.Appium;
using OpenQA.Selenium;
using OpenQA.Selenium.Appium;
using OpenQA.Selenium.Appium.Enums;
using OpenQA.Selenium.Remote;

namespace Applitools.Selenium.Tests
{
    public class HelloWorld
    {
        public static void Main(string[] args)
        {

            // This is your api key, make sure you use it in all your tests.
            Eyes eyes = new Eyes();

            // Set the desired capabilities.
            AppiumOptions dc = new AppiumOptions();
            dc.AddAdditionalCapability(MobileCapabilityType.PlatformName, "Android");
            dc.AddAdditionalCapability(MobileCapabilityType.PlatformVersion, "6.0.1");
            dc.AddAdditionalCapability(MobileCapabilityType.DeviceName, "LGE Nexus 5");
            dc.AddAdditionalCapability(MobileCapabilityType.BrowserName, "Chrome");
            //dc.AddAdditionalCapability("chromedriverExecutable", @"C:\Users\USER\devel\Eyes.Sdk.DotNet\bin\deps\WebDriver\chromedriver.exe");

            RemoteWebDriver driver = new RemoteWebDriver(new Uri("http://127.0.0.1:4723/wd/hub"), dc);
            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(120);

            try
            {

                // Start visual testing.
                eyes.Open(driver, "Hello World!", "My first Appium C# test!");

                // Navigate the browser to the "hello world!" web-site.
                driver.Url = "https://applitools.com/helloworld";

                Console.WriteLine("11111111111111111111111");

                // Visual checkpoint #1.
                eyes.CheckWindow("Hello!");

                Console.WriteLine("22222222222222222222222");

                // Click the "Click me!" button.
                driver.FindElement(By.TagName("button")).Click();

                Console.WriteLine("333333333333333333333333333");

                // Visual checkpoint #2.
                eyes.CheckWindow("Click!");

                // End the test.
                eyes.Close();

            }
            catch (Exception ex)
            {

                Console.WriteLine(ex);

            }
            finally
            {

                // Close the browser.
                driver.Quit();

                // If the test was aborted before eyes.Close was called, ends the test as aborted.
                eyes.AbortIfNotClosed();

            }
        }
    }
}