using Applitools.Selenium.Tests.Utils;
using Applitools.Tests.Utils;
using Applitools.Utils.Geometry;
using Applitools.VisualGrid;
using NUnit.Framework;
using OpenQA.Selenium;

namespace Applitools.Selenium.Tests.VisualGridTests
{
    [Parallelizable(ParallelScope.All)]
    public class TestDoubleOpenClose : ReportingTestSuite
    {
        [TestCase(false)]
        [TestCase(true)]

        public void TestDoubleOpenCheckClose(bool useVisualGrid)
        {
            IWebDriver driver;
            EyesRunner runner;
            Eyes eyes;
            SeleniumUtils.InitEyes(useVisualGrid, out driver, out runner, out eyes);

            driver.Url = "https://applitools.github.io/demo/TestPages/VisualGridTestPage/";

            string suffix = useVisualGrid ? "_VG" : "";
            try
            {
                eyes.Open(driver, "Applitools Eyes SDK", nameof(TestDoubleOpenCheckClose) + suffix, new RectangleSize(1200, 800));
                eyes.Check(Target.Window().Fully().IgnoreDisplacements(false).WithName("Step 1"));
                eyes.Close(false);

                eyes.Open(driver, "Applitools Eyes SDK", nameof(TestDoubleOpenCheckClose) + suffix, new RectangleSize(1200, 800));
                eyes.Check(Target.Window().Fully().IgnoreDisplacements(false).WithName("Step 2"));
                eyes.Close(false);
            }
            finally
            {
                driver.Quit();
            }

            TestResultsSummary allTestResults = runner.GetAllTestResults(false);
            Assert.AreEqual(2, allTestResults.Count);
        }

        [TestCase(false)]
        [TestCase(true)]
        public void TestDoubleOpenCheckCloseAsync(bool useVisualGrid)
        {
            IWebDriver driver;
            EyesRunner runner;
            Eyes eyes;
            SeleniumUtils.InitEyes(useVisualGrid, out driver, out runner, out eyes);
            driver.Url = "https://applitools.github.io/demo/TestPages/VisualGridTestPage/";

            string suffix = useVisualGrid ? "_VG" : "";

            try
            {
                eyes.Open(driver, "Applitools Eyes SDK", nameof(TestDoubleOpenCheckCloseAsync) + suffix, new RectangleSize(1200, 800));
                eyes.Check(Target.Window().Fully().IgnoreDisplacements(false).WithName("Step 1"));
                eyes.CloseAsync();

                eyes.Open(driver, "Applitools Eyes SDK", nameof(TestDoubleOpenCheckCloseAsync) + suffix, new RectangleSize(1200, 800));
                eyes.Check(Target.Window().Fully().IgnoreDisplacements(false).WithName("Step 2"));
                eyes.CloseAsync();
            }
            finally
            {
                driver.Quit();
            }
            TestResultsSummary allTestResults = runner.GetAllTestResults(false);
            Assert.AreEqual(2, allTestResults.Count);
        }

        [TestCase(false)]
        [TestCase(true)]
        public void TestDoubleOpenCheckCloseWithDifferentInstances(bool useVisualGrid)
        {
            IWebDriver driver = SeleniumUtils.CreateChromeDriver();
            driver.Url = "https://applitools.github.io/demo/TestPages/VisualGridTestPage/";

            EyesRunner runner = useVisualGrid ? (EyesRunner)new VisualGridRunner(10) : new ClassicRunner();
            runner.SetLogHandler(TestUtils.InitLogHandler());

            string suffix = useVisualGrid ? "_VG" : "";
            try
            {
                Eyes eyes1 = new Eyes(runner);
                eyes1.Batch = TestDataProvider.BatchInfo;
                eyes1.Open(driver, "Applitools Eyes SDK", nameof(TestDoubleOpenCheckCloseWithDifferentInstances) + suffix, new RectangleSize(1200, 800));
                eyes1.Check(Target.Window().Fully().IgnoreDisplacements(false).WithName("Step 1"));
                eyes1.Close(false);

                Eyes eyes2 = new Eyes(runner);
                eyes2.Batch = TestDataProvider.BatchInfo;
                eyes2.Open(driver, "Applitools Eyes SDK", nameof(TestDoubleOpenCheckCloseWithDifferentInstances) + suffix, new RectangleSize(1200, 800));
                eyes2.Check(Target.Window().Fully().IgnoreDisplacements(false).WithName("Step 2"));
                eyes2.Close(false);
            }
            finally
            {
                driver.Quit();
            }
            TestResultsSummary allTestResults = runner.GetAllTestResults(false);
            Assert.AreEqual(2, allTestResults.Count);
        }

        [TestCase(false)]
        [TestCase(true)]
        public void TestDoubleOpenCheckCloseAsyncWithDifferentInstances(bool useVisualGrid)
        {
            IWebDriver driver = SeleniumUtils.CreateChromeDriver();
            driver.Url = "https://applitools.github.io/demo/TestPages/VisualGridTestPage/";

            EyesRunner runner = useVisualGrid ? (EyesRunner)new VisualGridRunner(10) : new ClassicRunner();
            runner.SetLogHandler(TestUtils.InitLogHandler());

            string suffix = useVisualGrid ? "_VG" : "";
            try
            {
                Eyes eyes1 = new Eyes(runner);
                eyes1.Batch = TestDataProvider.BatchInfo;
                eyes1.Open(driver, "Applitools Eyes SDK", nameof(TestDoubleOpenCheckCloseAsyncWithDifferentInstances) + suffix, new RectangleSize(1200, 800));
                eyes1.Check(Target.Window().Fully().IgnoreDisplacements(false).WithName("Step 1"));
                eyes1.CloseAsync();

                Eyes eyes2 = new Eyes(runner);
                eyes2.Batch = TestDataProvider.BatchInfo;
                eyes2.Open(driver, "Applitools Eyes SDK", nameof(TestDoubleOpenCheckCloseAsyncWithDifferentInstances) + suffix, new RectangleSize(1200, 800));
                eyes2.Check(Target.Window().Fully().IgnoreDisplacements(false).WithName("Step 2"));
                eyes2.CloseAsync();
            }
            finally
            {
                driver.Quit();
            }

            TestResultsSummary allTestResults = runner.GetAllTestResults(false);
            Assert.AreEqual(2, allTestResults.Count);
        }

        [TestCase(true)]
        public void TestDoubleCheckDontGetAllResults(bool useVisualGrid)
        {
            IWebDriver driver = SeleniumUtils.CreateChromeDriver();
            driver.Url = "https://applitools.com/helloworld";

            EyesRunner runner = useVisualGrid ? (EyesRunner)new VisualGridRunner(10) : new ClassicRunner();
            runner.SetLogHandler(TestUtils.InitLogHandler());

            string suffix = useVisualGrid ? "_VG" : "";
            try
            {
                Eyes eyes1 = new Eyes(runner);
                eyes1.Batch = TestDataProvider.BatchInfo;
                eyes1.Open(driver, "Applitools Eyes SDK", nameof(TestDoubleCheckDontGetAllResults) + suffix, new RectangleSize(1200, 800));
                eyes1.Check(Target.Window().WithName("Step 1"));
                eyes1.Check(Target.Window().WithName("Step 2"));
                eyes1.Close(false);
            }
            finally
            {
                driver.Quit();
            }
        }
    }
}
