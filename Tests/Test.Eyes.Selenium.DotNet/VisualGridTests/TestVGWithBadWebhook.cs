using Applitools.Selenium.Tests.Utils;
using Applitools.Tests.Utils;
using Applitools.VisualGrid;
using NUnit.Framework;
using OpenQA.Selenium;

namespace Applitools.Selenium.Tests.VisualGridTests
{
    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    public class TestVGWithBadWebhook : ReportingTestSuite
    {

        [Test]
        public void Test()
        {
            IWebDriver driver = SeleniumUtils.CreateChromeDriver();
            driver.Url = "https://applitools.com/helloworld";

            BatchInfo batch = new BatchInfo("Visual Grid - Test bad webhook");
            VisualGridRunner runner = new VisualGridRunner(10);
            Eyes eyes = new Eyes(runner);

            Configuration config = new Configuration();
            config.SetBatch(batch);
            config.SetAppName("Visual Grid Tests");
            config.SetTestName("Bad Webhook");
            config.SetViewportSize(new Applitools.Utils.Geometry.RectangleSize(800, 600));

            eyes.SetConfiguration(config);
            eyes.Open(driver);
            eyes.Check(Target.Window().Fully().BeforeRenderScreenshotHook("gibberish uncompilable java script"));
            driver.Quit();
            Assert.That(() =>
            {
                eyes.Close();
                runner.GetAllTestResults();
            }, Throws.Exception.With.Property("Message").StartsWith("Render Failed for DesktopBrowserInfo {ViewportSize={Width=800, Height=600}, BrowserType=CHROME} "));

        }
    }
}
