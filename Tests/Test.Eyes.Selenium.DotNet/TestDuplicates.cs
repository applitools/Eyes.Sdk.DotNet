using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System;
using ExpectedConditions = SeleniumExtras.WaitHelpers.ExpectedConditions;

namespace Applitools.Selenium.Tests
{
    [TestFixture]
    [TestFixtureSource(typeof(TestDataProvider), nameof(TestDataProvider.FixtureArgs))]
    [Parallelizable]
    public class TestDuplicates : TestSetup
    {
        public TestDuplicates(DriverOptions options) : this(options, false) { }

        public TestDuplicates(DriverOptions options, bool useVisualGrid)
          : base("Eyes Selenium SDK - Duplicates", options, useVisualGrid)
        {
            testedPageUrl = "https://applitools.github.io/demo/TestPages/VisualGridTestPage/duplicates.html";
        }
        public TestDuplicates(DriverOptions options, StitchModes stitchMode)
        : base("Eyes Selenium SDK - Duplicates", options, stitchMode)
        {
            testedPageUrl = "https://applitools.github.io/demo/TestPages/VisualGridTestPage/duplicates.html";
        }

        [Test]
        public void TestDuplicatedIFrames()
        {
            IWebDriver driver = GetDriver();
            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(30));

            driver.SwitchTo().Frame(2);
            wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("#p2")));
            driver.SwitchTo().DefaultContent();

            driver.SwitchTo().Frame(3);
            wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("#p2")));
            driver.SwitchTo().DefaultContent();

            GetEyes().CheckWindow("Duplicated Iframes");
        }

    }
}
