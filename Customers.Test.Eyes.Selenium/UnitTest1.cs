using System;
using System.Drawing; //Required for eyes.Open API
using Applitools.Selenium; //Applitools Selenium SDK 👈🏼
using OpenQA.Selenium.Chrome; // Selenium's Chrome browser SDK
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using NUnit.Framework;

//using ExpectedConditions = SeleniumExtras.WaitHelpers.ExpectedConditions;

namespace Elad
{
    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    public class UnitTest1
    {
        [Test]
        public void ScrollRegion_1000_700()
        {
            TestScrollRegion_(new Size(1000, 700));
        }

        [Test]
        public void ScrollRegion_1000_2000()
        {
            TestScrollRegion_(new Size(1000, 2000));
        }

        [Test]
        public void ScrollRegion_No_Viewport()
        {
            TestScrollRegion_(null);
        }

        private static void TestScrollRegion_(Size? size)
        {
            // Open a Chrome browser.
            var driver = new ChromeDriver();

            // Initialize the eyes SDK and set your private API key.
            var eyes = new Eyes();
            //var eyes = new Eyes(new Uri("https://test2eyes.applitools.com"));
            //eyes.ApiKey = "XucYP13zAM2eWJE7EoyBiojebKA6D9xMucHuPHGMeQ4110";
            //scroll and take full page screenshot
            eyes.ForceFullPageScreenshot = true;

            // Hard code the Applitools API key or get it from the environment (see the Tutorial for details)
            //eyes.ApiKey = "0rQly9ew54FfJWfpSCG5wIWAj0LCtQ5HNPpatwRNFQc110";
            try
            {
                driver.Url = "https://qa.servicetitan.com/";

                driver.FindElement(By.CssSelector("#username")).SendKeys("applitools");
                driver.FindElement(By.CssSelector("#password")).SendKeys("1234");

                driver.FindElement(By.CssSelector("#loginButton")).Click();


                WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(30));
                wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementToBeClickable(By.CssSelector("#navbar > div.ui.inverted.menu.extra-navigation > a:nth-child(4) > i"))).Click();
                wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementToBeClickable(By.CssSelector("#settingsMenu > button"))).Click();

                if (size.HasValue)
                {
                    eyes.Open(driver, "test", "check region", size.Value);
                }
                else
                {
                    eyes.Open(driver, "test", "check region");
                }
                eyes.Check("region", Target.Region(By.CssSelector("#settingsMenu")).Fully());
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