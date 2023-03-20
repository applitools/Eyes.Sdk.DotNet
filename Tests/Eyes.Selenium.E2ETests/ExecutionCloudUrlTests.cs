using System;
using Applitools;
using Applitools.Selenium;
using Applitools.Utils.Geometry;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Remote;

namespace Eyes.Selenium.E2ETests
{
    public class ExecutionCloudUrlTests
    {
        private Applitools.Selenium.Eyes eyes;
        private EyesRunner runner;
        private IWebDriver driver;
        private string EG_URL;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            EG_URL = Applitools.Selenium.Eyes.GetExecutionCloudURL();
        }

        [SetUp]
        public void SetUp()
        {
            runner = new ClassicRunner();
            eyes = new Applitools.Selenium.Eyes(runner);
            var chromeOptions = new ChromeOptions();
            driver = new RemoteWebDriver(new Uri(EG_URL), chromeOptions);
        }

        [TearDown]
        public void TearDown()
        {
            runner.GetAllTestResults(false);
            if (driver != null) driver.Quit();
            eyes.Abort();
        }

        [Test]
        public void CheckWindowFullyWithCustomScrollRootWithScrollStitching()
        {
            driver.Navigate().GoToUrl("https://demo.applitools.com");
            eyes.Open(driver, "Eyes Selenium SDK", "Test EC Url", new RectangleSize(700, 460));
            eyes.Check(Target.Window().Fully(false));
            eyes.CloseAsync();
        }
    }
}