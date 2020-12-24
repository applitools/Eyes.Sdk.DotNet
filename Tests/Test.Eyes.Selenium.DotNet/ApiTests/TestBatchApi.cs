using Applitools.Selenium.Tests.Utils;
using Applitools.Tests.Utils;
using NUnit.Framework;
using OpenQA.Selenium;
using System.Drawing;
using System.Threading;

namespace Applitools.Selenium.Tests.ApiTests
{
    [Parallelizable(ParallelScope.All)]
    public class TestBatchApi : ReportingTestSuite
    {
        [TestCase(false)]
        [TestCase(true)]
        public void TestCloseBatch(bool dontCloseBatch)
        {
            IWebDriver driver = SeleniumUtils.CreateChromeDriver();
            // Initialize the VisualGridEyes SDK and set your private API key.
            EyesRunner runner = new ClassicRunner();
            runner.DontCloseBatches = dontCloseBatch;
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

            Thread.Sleep(10000);

            batch = TestUtils.GetBatchInfo(eyes);
            Assert.AreEqual(!dontCloseBatch, batch.IsCompleted);
        }
    }
}
