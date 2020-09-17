using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
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
    }
}
