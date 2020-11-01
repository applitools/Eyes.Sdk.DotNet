using Applitools.Selenium.Tests.Utils;
using Applitools.Tests.Utils;
using Applitools.VisualGrid;
using NUnit.Framework;
using OpenQA.Selenium;
using System;

namespace Applitools.Selenium.Tests.ApiTests
{
    [TestFixture, Parallelizable]
    [TestFixtureSource(typeof(TestAbort), nameof(TestAbort.UseVisualGridDataSource))]
    public class TestAbort2 : ReportingTestSuite
    {
        private readonly int concurrentSessions = 5;
        private readonly int viewPortWidth = 800;
        private readonly int viewPortHeight = 600;
        private readonly string appName = "My application";
        private readonly string batchName = "My batch";
        private readonly string testedUrl = "https://applitools.github.io/demo/TestPages/FramesTestPage/";//"https://applitools.com/docs/topics/overview.html";
        private readonly bool useVisualGrid_;

        private EyesRunner runner = null;
        private Configuration suiteConfig;
        private Eyes eyes;
        private IWebDriver webDriver;

        public TestAbort2(bool useVisualGrid)
        {
            useVisualGrid_ = useVisualGrid;
            suiteArgs_.Add(nameof(useVisualGrid), useVisualGrid);
        }

        [OneTimeSetUp]
        public void BeforeTestSuite()
        {
            // 1. Create the runner that manages multiple tests
            if (useVisualGrid_)
            {
                runner = new VisualGridRunner(concurrentSessions);
            }
            else
            {
                runner = new ClassicRunner();
            }
            // continued below.... 
            // 2. Create a configuration object, we will use this when setting up each test            
            suiteConfig = new Configuration();

            // 3. Set the various configuration values
            suiteConfig
               // 4. Add Visual Grid browser configurations
               .AddBrowser(900, 600, BrowserType.CHROME)
               .AddBrowser(1024, 786, BrowserType.CHROME)
               .AddBrowser(900, 600, BrowserType.FIREFOX)
               .AddBrowser(900, 600, BrowserType.IE_10)
               .AddBrowser(900, 600, BrowserType.IE_11)
               .AddBrowser(900, 600, BrowserType.EDGE_LEGACY)
               .AddDeviceEmulation(DeviceName.iPhone_4, Applitools.VisualGrid.ScreenOrientation.Portrait)
               .AddDeviceEmulation(DeviceName.Galaxy_S5, Applitools.VisualGrid.ScreenOrientation.Landscape)

               // 5. set up default Eyes configuration values
               .SetBatch(new BatchInfo(batchName))
               .SetAppName(appName)
               .SetViewportSize(new Applitools.Utils.Geometry.RectangleSize(viewPortWidth, viewPortHeight));

        }

        public void BeforeEachTest()
        {
            // 1. Create the Eyes instance for the test and associate it with the runner
            eyes = new Eyes(runner);

            // 2. Set the configuration values we set up in beforeTestSuite
            eyes.SetConfiguration(suiteConfig);

            // 3. Create a WebDriver for the test
            webDriver = SeleniumUtils.CreateChromeDriver();
        }

        [Test]
        public void Test_GetAllResults()
        {
            BeforeEachTest();
            Assert.That(Test_ThrowBeforeOpen, Throws.Exception, $"Before Open - (VG: {useVisualGrid_})");
            AfterEachTest();

            BeforeEachTest();
            Assert.That(Test_ThrowAfterOpen, Throws.Exception, $"After Open - (VG: {useVisualGrid_})");
            AfterEachTest();

            BeforeEachTest();
            if (!useVisualGrid_)
            {
                Assert.That(Test_ThrowDuringCheck, Throws.Exception, "During Check - (VG: false)");
            }
            else
            {
                Test_ThrowDuringCheck();
            }
            AfterEachTest();

            BeforeEachTest();
            Assert.That(Test_ThrowAfterCheck, Throws.Exception, $"After Check - (VG: {useVisualGrid_})");
            AfterEachTest();

            Assert.That(runner.GetAllTestResults, Throws.Exception, $"GetAllTestResults - (VG: {useVisualGrid_})");
        }

        public void Test_ThrowBeforeOpen()
        {
            // 1. Update the Eyes configuration with test specific values
            Configuration testConfig = eyes.GetConfiguration();
            testConfig.SetTestName("test URL : " + testedUrl);
            eyes.SetConfiguration(testConfig);
            throw new Exception("Before Open");
        }

        public void Test_ThrowAfterOpen()
        {
            // 1. Update the Eyes configuration with test specific values
            Configuration testConfig = eyes.GetConfiguration();
            testConfig.SetTestName("test URL : " + testedUrl);
            eyes.SetConfiguration(testConfig);

            //2. Open Eyes, the application,test name and viewport size are already configured
            IWebDriver driver = eyes.Open(webDriver);
            throw new Exception("After Open");
        }

        public void Test_ThrowDuringCheck()
        {
            // 1. Update the Eyes configuration with test specific values
            Configuration testConfig = eyes.GetConfiguration();
            testConfig.SetTestName("test URL : " + testedUrl);
            eyes.SetConfiguration(testConfig);

            //2. Open Eyes, the application,test name and viewport size are already configured
            IWebDriver driver = eyes.Open(webDriver);

            //3. Now run the test
            driver.Url = testedUrl;
            eyes.Check("Step 1 Content - " + testedUrl, Target.Frame("non-existing-frame"));
        }

        public void Test_ThrowAfterCheck()
        {
            // 1. Update the Eyes configuration with test specific values
            Configuration testConfig = eyes.GetConfiguration();
            testConfig.SetTestName("test URL : " + testedUrl);
            eyes.SetConfiguration(testConfig);

            //2. Open Eyes, the application,test name and viewport size are already configured
            IWebDriver driver = eyes.Open(webDriver);

            //3. Now run the test
            driver.Url = testedUrl;
            eyes.Check("Step 1 Content - " + testedUrl, Target.Window());
            throw new Exception("After Check");
        }

        public void AfterEachTest()
        {
            if (eyes.IsOpen)
            {
                eyes.Close(false);
            }
            else
            {
                eyes.Abort();
            }
            webDriver.Quit();
        }

        [OneTimeTearDown]
        public void AfterTestSuite()
        {
            // Wait until the test results are available and retrieve them
            TestResultsSummary allTestResults = runner.GetAllTestResults(false);
            foreach (var result in allTestResults)
            {
                HandleTestResults(result);
            }
        }

        void HandleTestResults(TestResultContainer summary)
        {
            Exception ex = summary.Exception;
            if (ex != null)
            {
                TestContext.WriteLine("System error occurred while checking target.");
            }
            TestResults result = summary.TestResults;
            if (result == null)
            {
                TestContext.WriteLine("No test results information available");
            }
            else
            {
                TestContext.WriteLine(
                    "AppName = {0}, testname = {1}, Browser = {2},OS = {3} viewport = {4}x{5}, matched = {6},mismatched = {7}, missing = {8}, aborted = {9}\n",
                    result.AppName,
                    result.Name,
                    result.HostApp,
                    result.HostOS,
                    result.HostDisplaySize.Width,
                    result.HostDisplaySize.Height,
                    result.Matches,
                    result.Mismatches,
                    result.Missing,
                    (result.IsAborted ? "aborted" : "no"));
            }
        }
    }
}
