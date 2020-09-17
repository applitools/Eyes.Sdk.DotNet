using Applitools.Exceptions;
using Applitools.Selenium.Tests.Utils;
using Applitools.Tests.Utils;
using NUnit.Framework;
using OpenQA.Selenium;
using System;
using System.Drawing;

namespace Applitools.Selenium.Tests
{
    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    public class TestServerStatus : ReportingTestSuite
    {
        [Test]
        public void TestSessionSummary_Status_Failed()
        {
            Eyes eyes = new Eyes();
            eyes.Batch = TestDataProvider.BatchInfo;
            eyes.SaveNewTests = true;
            IWebDriver webDriver = SeleniumUtils.CreateChromeDriver();

            string guid = "_" + Guid.NewGuid();

            eyes.Open(webDriver, nameof(TestServerStatus) + guid, nameof(TestServerStatus) + guid, new Size(800, 600));
            webDriver.Url = "https://applitools.github.io/demo/TestPages/DynamicResolution/mobile.html";
            try
            {
                eyes.Check(nameof(TestSessionSummary_Status_Failed) + guid, Target.Window().Fully(false));
                eyes.Close(false);
            }
            finally
            {
                eyes.Abort();
            }

            eyes.Open(webDriver, nameof(TestServerStatus) + guid, nameof(TestServerStatus) + guid, new Size(800, 600));
            webDriver.Url = "https://applitools.github.io/demo/TestPages/DynamicResolution/desktop.html";
            try
            {
                eyes.Check(nameof(TestSessionSummary_Status_Failed) + guid, Target.Window().Fully(false));
                TestResults results = eyes.Close(false);
                Assert.IsTrue(results.IsDifferent);
            }
            finally
            {
                eyes.Abort();
                webDriver.Quit();
            }
        }

        [Test]
        public void TestSessionSummary_Status_New()
        {
            Eyes eyes = new Eyes();
            eyes.SaveNewTests = false;

            IWebDriver webDriver = SeleniumUtils.CreateChromeDriver();

            string guid = "_" + Guid.NewGuid();
            IWebDriver driver = eyes.Open(webDriver, nameof(TestServerStatus) + guid, nameof(TestServerStatus) + guid, new Size(800, 600));

            driver.Url = "https://applitools.github.io/demo/TestPages/FramesTestPage/";
            try
            {
                eyes.Check(nameof(TestSessionSummary_Status_New) + guid, Target.Window());
                TestResults results = eyes.Close(false);
                Assert.IsTrue(results.IsNew);
            }
            finally
            {
                eyes.Abort();
                driver.Quit();
            }
        }
    }
}
