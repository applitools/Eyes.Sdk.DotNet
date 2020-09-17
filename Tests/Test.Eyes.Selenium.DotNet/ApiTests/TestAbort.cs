using Applitools.Selenium.Tests.Utils;
using Applitools.Tests.Utils;
using Applitools.VisualGrid;
using NUnit.Framework;
using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading;

namespace Applitools.Selenium.Tests.ApiTests
{
    [TestFixture, Parallelizable(ParallelScope.All)]
    [TestFixtureSource(nameof(UseVisualGridDataSource))]
    public class TestAbort : ReportingTestSuite
    {
        IWebDriver driver_;
        Eyes eyes_;
        EyesRunner runner_;
        bool useVisualGrid_;

        public TestAbort(bool useVisualGrid)
        {
            useVisualGrid_ = useVisualGrid;
            suiteArgs_.Add(nameof(useVisualGrid), useVisualGrid);
        }

        public static IList<bool> UseVisualGridDataSource => new bool[] { true, false };

        [SetUp]
        public void LocalSetUp()
        {
            driver_ = SeleniumUtils.CreateChromeDriver();
            driver_.Url = "data:text/html,<p>Test</p>";
            runner_ = useVisualGrid_ ? (EyesRunner)new VisualGridRunner(10) : new ClassicRunner();
            eyes_ = new Eyes(runner_);
            eyes_.Batch = TestDataProvider.BatchInfo;
            string testName = useVisualGrid_ ? "Test Abort_VG" : "Test Abort";

            Configuration config = eyes_.GetConfiguration();
            config.AddBrowser(800, 600, BrowserType.CHROME);
            eyes_.SetConfiguration(config);

            eyes_.Open(driver_, testName, testName, new Size(1200, 800));
        }

        [TearDown]
        public void LocalTearDown()
        {
            driver_.Quit();
        }

        [Test]
        public void TestAbortIfNotClosed()
        {
            eyes_.Check(useVisualGrid_ ? "VG" : "SEL", Target.Window());
            Thread.Sleep(TimeSpan.FromSeconds(15));
            eyes_.Abort();
        }
    }
}