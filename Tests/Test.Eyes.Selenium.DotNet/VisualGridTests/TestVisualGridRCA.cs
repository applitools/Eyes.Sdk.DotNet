using Applitools.Selenium.Tests.Utils;
using Applitools.Tests.Utils;
using Applitools.VisualGrid;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System;
using ExpectedConditions = SeleniumExtras.WaitHelpers.ExpectedConditions;

namespace Applitools.Selenium.Tests.VisualGridTests
{
    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    public class TestVisualGridRCA : ReportingTestSuite
    {
        private BatchInfo batch_ = new BatchInfo("Test Visual Grid RCA");

        [Test]
        public void Test_VG_RCA_Config()
        {
            EyesRunner runner = new VisualGridRunner(10);
            Eyes eyes = new Eyes(runner);
            eyes.Batch = batch_;
            IWebDriver driver = SeleniumUtils.CreateChromeDriver();
            try
            {
                driver.Url = "https://applitools.github.io/demo/TestPages/VisualGridTestPage";
                driver.SwitchTo().Frame("iframe");
                WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(30));
                wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("svg")));
                driver.SwitchTo().ParentFrame();
                eyes.Open(driver, "Test Visual Grid", "Test RCA Config");
                eyes.SendDom = true;
                eyes.Check(Target.Window());
                eyes.Close();
                runner.GetAllTestResults();
            }
            finally
            {
                driver.Quit();
            }
        }

        [Test]
        public void Test_VG_RCA_Fluent()
        {
            EyesRunner runner = new VisualGridRunner(10);
            Eyes eyes = new Eyes(runner);
            eyes.Batch = batch_;
            IWebDriver driver = SeleniumUtils.CreateChromeDriver();
            try
            {
                driver.Url = "https://applitools.github.io/demo/TestPages/VisualGridTestPage";
                
                driver.SwitchTo().Frame("iframe");
                WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(30));
                wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("#p2")));
                driver.SwitchTo().DefaultContent();

                eyes.Open(driver, "Test Visual Grid", "Test RCA Fluent");
                eyes.SendDom = false;
                eyes.Check(Target.Window().SendDom(true));
                eyes.Close();
                runner.GetAllTestResults();
            }
            finally
            {
                driver.Quit();
            }
        }
    }
}
