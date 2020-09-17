using Applitools.Selenium.Tests.Utils;
using Applitools.Tests.Utils;
using Applitools.VisualGrid;
using NUnit.Framework;
using OpenQA.Selenium.Chrome;

namespace Applitools.Selenium.Tests.VisualGridTests
{
    [TestFixture]
    [Parallelizable]
    public class TestSpecialCharacters : ReportingTestSuite
    {
        [Test]
        public void TestRenderSpecialCharacters()
        {
            VisualGridRunner runner = new VisualGridRunner(30);
            Eyes eyes = new Eyes(runner);
            eyes.SetLogHandler(TestUtils.InitLogHandler());

            Configuration sconf = new Configuration();
            sconf.SetTestName("Special Characters");
            sconf.SetAppName("Special Characters Test");

            sconf.AddBrowser(800, 600, BrowserType.CHROME);
            sconf.SetBatch(TestDataProvider.BatchInfo);

            eyes.SetConfiguration(sconf);
            ChromeDriver driver = SeleniumUtils.CreateChromeDriver();
            eyes.Open(driver);
            driver.Url = "https://applitools.github.io/demo/TestPages/SpecialCharacters/index.html";
            eyes.Check("Test Special Characters", Target.Window().Fully());
            driver.Quit();
            eyes.Close();
            TestResultsSummary allResults = runner.GetAllTestResults();
        }
    }
}
