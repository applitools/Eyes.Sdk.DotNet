using Applitools.Selenium.Tests.Utils;
using Applitools.Tests.Utils;
using NUnit.Framework;
using OpenQA.Selenium;
using System;
using System.Drawing;

namespace Applitools.Selenium.Tests.ApiTests
{
    public class TestBatchApi : ReportingTestSuite
    {
        [TestCase(false)]
        [TestCase(true)]
        public void TestCloseBatch(bool dontCloseBatch)
        {
            Environment.SetEnvironmentVariable("APPLITOOLS_DONT_CLOSE_BATCHES", dontCloseBatch.ToString());
            IWebDriver driver = SeleniumUtils.CreateChromeDriver();
            // Initialize the VisualGridEyes SDK and set your private API key.
            EyesRunner runner = new ClassicRunner();
            Eyes eyes = new Eyes(runner);
            TestUtils.SetupLogging(eyes);

            eyes.SendDom = false;
            eyes.StitchMode = StitchModes.CSS;

            BatchInfo batchInfo = new BatchInfo("Runner Testing");
            eyes.Batch = batchInfo;

            // Navigate the browser to the "hello world!" web-site.
            driver.Url = "https://applitools.com/helloworld";
            BatchInfo batch;
            try
            {
                eyes.Open(driver, "Applitools Eyes SDK", "Classic Runner Test", new Size(1200, 800));
                eyes.Check(Target.Window().WithName("Step 1"));
                eyes.Close();
            }
            finally
            {
                driver.Quit();
            }

            batch = TestUtils.GetBatchInfo(eyes);
            Assert.IsFalse(batch.IsCompleted);

            runner.GetAllTestResults(false);

            batch = TestUtils.GetBatchInfo(eyes);
            Assert.AreEqual(!dontCloseBatch, batch.IsCompleted);
        }
    }
}
