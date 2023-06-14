using System;
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
        private Applitools.Selenium.Eyes eyes_;
        private SeleniumEyesRunner runner_;
        private IWebDriver driver_;
        private string egUrl_;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            egUrl_ = Applitools.Selenium.Eyes.GetExecutionCloudURL();
        }

        [SetUp]
        public void SetUp()
        {
            runner_ = new ClassicRunner();
            eyes_ = new Applitools.Selenium.Eyes(runner_);
            var chromeOptions = new ChromeOptions();
            driver_ = new RemoteWebDriver(new Uri(egUrl_), chromeOptions);
        }

        [TearDown]
        public void TearDown()
        {
            runner_.GetAllTestResults(false);
            if (driver_ != null) driver_.Quit();
            eyes_.Abort();
        }

        [Test]
        public void CheckWindowFullyWithCustomScrollRootWithScrollStitching()
        {
            driver_.Navigate().GoToUrl("https://demo.applitools.com");
            eyes_.Open(driver_, "Eyes Selenium SDK", "Test EC Url", new RectangleSize(700, 460));
            eyes_.Check(Target.Window().Fully(false));
            eyes_.Close();
        }
    }
}