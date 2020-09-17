using Applitools.Selenium.Tests.Utils;
using Applitools.Tests.Utils;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections;
using ExpectedConditions = SeleniumExtras.WaitHelpers.ExpectedConditions;

namespace Applitools.Selenium.Tests
{
    [Parallelizable(ParallelScope.All)]
    public class TestCustomTooltips
    {
        public static IEnumerable TestMethodDriver
        {
            get
            {
                yield return TestDataProvider.GetChromeOptions();
                if (!TestUtils.RUNS_ON_CI)
                {
                    //yield return TestDataProvider.GetFirefoxOptions();
                }
            }
        }

        //Skipped since the page is not available.
        //[TestCaseSource(nameof(TestMethodDriver))]
        //[Parallelizable]
        public void CustomTooltipsTest(DriverOptions driverOptions)
        {
            IWebDriver driver = SeleniumUtils.CreateWebDriver(driverOptions);

            driver.Url = "https://d1q3vzvnjy4w20.cloudfront.net/";

            Eyes eyes = new Eyes();
            TestUtils.SetupLogging(eyes);
            eyes.Batch = TestDataProvider.BatchInfo;
            eyes.HideScrollbars = false;
            eyes.Open(driver, nameof(TestCustomTooltips), nameof(CustomTooltipsTest), new System.Drawing.Size(800, 600));

            driver.FindElement(By.Id("coxkit-tooltip")).Click();
            Actions action = new Actions(driver);
            action.MoveToElement(driver.FindElement(By.Id("warningHover"))).Perform();

            By tooltipSelector = By.CssSelector("div[content ^= 'I want to tell you']");
            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(30));
            wait.Until(ExpectedConditions.ElementIsVisible(tooltipSelector));

            try
            {
                eyes.Check(Target.Region(tooltipSelector).WithName(nameof(CustomTooltipsTest)));
                eyes.Close();
            }
            finally
            {
                eyes.Abort();
                driver.Quit();
            }
        }
    }
}
