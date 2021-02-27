using Applitools.Selenium.Tests.Utils;
using Applitools.Tests.Utils;
using NUnit.Framework;
using OpenQA.Selenium;
using System.Drawing;

namespace Applitools.Selenium.Tests
{
    [TestFixture]
    [Parallelizable]
    public class TestClassicRunnerExceptions : ReportingTestSuite
    {
        [Test]
        public void TestExceptionInGetAllTestResults()
        {
            ILogHandler logHandler = TestUtils.InitLogHandler();
            EyesRunner runner = new ClassicRunner(logHandler);
            Eyes eyes = new Eyes(runner);
            eyes.SaveNewTests = false;
            IWebDriver driver = SeleniumUtils.CreateChromeDriver();
            driver.Url = "https://applitools.com/helloworld?diff1";

            eyes.Open(driver, 
                nameof(TestClassicRunnerExceptions), 
                nameof(TestExceptionInGetAllTestResults), 
                new Size(800, 600));
            eyes.CheckWindow();
            eyes.CloseAsync();

            driver.Quit();
            Assert.That(() => { TestResultsSummary results = runner.GetAllTestResults(); }, Throws.Exception);
        }
    }
}
