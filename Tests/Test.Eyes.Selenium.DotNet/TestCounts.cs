using Applitools.Selenium.Tests.Utils;
using Applitools.Tests.Utils;
using Applitools.VisualGrid;
using NUnit.Framework;
using OpenQA.Selenium;
using System.Drawing;
using System.Runtime.CompilerServices;

namespace Applitools.Selenium.Tests
{
    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    public class TestCounts : ReportingTestSuite
    {
        private static TestObjects InitEyes_([CallerMemberName] string testName = null)
        {
            IWebDriver webDriver = SeleniumUtils.CreateChromeDriver();
            webDriver.Url = "https://applitools.com/helloworld";
            ILogHandler logHandler = TestUtils.InitLogHandler(testName);
            VisualGridRunner runner = new VisualGridRunner(10, logHandler);
            Eyes eyes = new Eyes(runner);
            eyes.SendDom = false;
            TestObjects testObjects = new TestObjects(webDriver, runner, eyes);
            return testObjects;
        }

        [Test]
        public void Test_VGTestsCount_1()
        {
            TestObjects testObjects = InitEyes_();
            testObjects.Eyes.Batch = TestDataProvider.BatchInfo;
            try
            {
                testObjects.Eyes.Open(testObjects.WebDriver, "Test Count", "Test_VGTestsCount_1", new Size(640, 480));
                testObjects.Eyes.Check("Test", Target.Window());
                testObjects.Eyes.Close();
                TestResultsSummary resultsSummary = testObjects.Runner.GetAllTestResults();
                Assert.AreEqual(1, resultsSummary?.Count);
            }
            finally
            {
                testObjects.WebDriver.Quit();
                testObjects.Eyes.Abort();
            }
        }

        [Test]
        public void Test_VGTestsCount_2()
        {
            TestObjects testObjects = InitEyes_();
            try
            {
                Configuration conf = new Configuration();
                conf.SetBatch(TestDataProvider.BatchInfo);
                conf.AddBrowser(new DesktopBrowserInfo(900, 600));
                conf.AddBrowser(new DesktopBrowserInfo(1024, 768));
                testObjects.Eyes.SetConfiguration(conf);
                testObjects.Eyes.Open(testObjects.WebDriver, "Test Count", "Test_VGTestsCount_2");
                testObjects.Eyes.Check("Test", Target.Window());
                testObjects.Eyes.Close();
                TestResultsSummary resultsSummary = testObjects.Runner.GetAllTestResults();
                Assert.AreEqual(2, resultsSummary?.Count);
            }
            finally
            {
                testObjects.WebDriver.Quit();
                testObjects.Eyes.Abort();
            }
        }

        [Test]
        public void Test_VGTestsCount_3()
        {
            TestObjects testObjects = InitEyes_();
            try
            {
                Configuration conf = new Configuration();
                conf.SetBatch(TestDataProvider.BatchInfo);
                conf.AddBrowser(new DesktopBrowserInfo(900, 600));
                conf.AddBrowser(new DesktopBrowserInfo(1024, 768));
                conf.SetAppName("Test Count").SetTestName("Test_VGTestsCount_3");
                testObjects.Eyes.SetConfiguration(conf);
                testObjects.Eyes.Open(testObjects.WebDriver);
                testObjects.Eyes.Check("Test", Target.Window());
                testObjects.Eyes.Close();
                TestResultsSummary resultsSummary = testObjects.Runner.GetAllTestResults();
                Assert.AreEqual(2, resultsSummary?.Count);
            }
            finally
            {
                testObjects.WebDriver.Quit();
                testObjects.Eyes.Abort();
            }
        }

        [Test]
        public void Test_VGTestsCount_4()
        {
            TestObjects testObjects = InitEyes_();
            try
            {
                Configuration conf = new Configuration();
                conf.SetBatch(TestDataProvider.BatchInfo);
                conf.SetAppName("Test Count").SetTestName("Test_VGTestsCount_4");
                testObjects.Eyes.SetConfiguration(conf);
                testObjects.Eyes.Open(testObjects.WebDriver);
                testObjects.Eyes.Check("Test", Target.Window());
                testObjects.Eyes.Close();
                TestResultsSummary resultsSummary = testObjects.Runner.GetAllTestResults();
                Assert.AreEqual(1, resultsSummary?.Count);
            }
            finally
            {
                testObjects.WebDriver.Quit();
                testObjects.Eyes.Abort();
            }
        }

        [Test]
        public void Test_VGTestsCount_5()
        {
            TestObjects testObjects = InitEyes_();
            try
            {
                Configuration conf = new Configuration();
                conf.SetBatch(TestDataProvider.BatchInfo);
                conf.AddBrowser(new DesktopBrowserInfo(900, 600));
                conf.AddBrowser(new DesktopBrowserInfo(1024, 768));
                testObjects.Eyes.SetConfiguration(conf);
                testObjects.Eyes.Open(testObjects.WebDriver, "Test Count", "Test_VGTestsCount_5", new Size(640, 480));
                testObjects.Eyes.Check("Test", Target.Window());
                testObjects.Eyes.Close();
                TestResultsSummary resultsSummary = testObjects.Runner.GetAllTestResults();
                Assert.AreEqual(2, resultsSummary?.Count);
            }
            finally
            {
                testObjects.WebDriver.Quit();
                testObjects.Eyes.Abort();
            }
        }

        private class TestObjects
        {
            public TestObjects(IWebDriver webDriver, VisualGridRunner runner, Eyes eyes)
            {
                WebDriver = webDriver;
                Runner = runner;
                Eyes = eyes;
            }

            public Eyes Eyes { get; }

            public VisualGridRunner Runner { get; }

            public IWebDriver WebDriver { get; }
        }
    }
}
