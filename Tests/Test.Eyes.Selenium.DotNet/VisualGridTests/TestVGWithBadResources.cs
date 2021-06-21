using Applitools.Selenium.Tests.Utils;
using Applitools.Tests.Utils;
using Applitools.VisualGrid;
using NUnit.Framework;
using OpenQA.Selenium;

namespace Applitools.Selenium.Tests.VisualGridTests
{
    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    public class TestVGWithBadResources : ReportingTestSuite
    {

        [Test]
        public void Test()
        {
            IWebDriver driver = SeleniumUtils.CreateChromeDriver();
            driver.Url = "https://applitools.github.io/demo/TestPages/VisualGridTestPage/bad_resources.html";

            BatchInfo batch = new BatchInfo("Visual Grid - Test bad resources");
            ILogHandler logHandler = TestUtils.InitLogHandler(nameof(TestVGWithBadWebhook));
            VisualGridRunner runner = new VisualGridRunner(10, logHandler);
            Eyes eyes = new Eyes(runner);

            Configuration config = new Configuration();
            config.SetBatch(batch);
            config.SetAppName("Visual Grid Tests");
            config.SetTestName("Bad Resources");
            config.SetViewportSize(new Applitools.Utils.Geometry.RectangleSize(800, 600));

            eyes.SetConfiguration(config);
            eyes.Open(driver);
            try
            {
                eyes.Check(Target.Window().Fully());
                eyes.Close();
                runner.GetAllTestResults();
            }
            finally
            {
                eyes.AbortIfNotClosed();
                driver.Quit();
            }
        }
    }
}
