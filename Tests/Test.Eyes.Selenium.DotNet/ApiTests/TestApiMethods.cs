using Applitools.Fluent;
using Applitools.Selenium.Fluent;
using Applitools.Selenium.Tests.Utils;
using Applitools.Selenium.VisualGrid;
using Applitools.Tests.Utils;
using Applitools.VisualGrid;
using NUnit.Framework;
using OpenQA.Selenium;
using System;
using System.Drawing;

namespace Applitools.Selenium.Tests.ApiTests
{
    [Parallelizable(ParallelScope.All)]
    public class TestApiMethods : ReportingTestSuite
    {
        [TestCase(true), TestCase(false)]
        public void TestCloseAsync(bool useVisualGrid)
        {
            IWebDriver driver = SeleniumUtils.CreateChromeDriver();
            string logPath = TestUtils.InitLogPath();
            ILogHandler logHandler = TestUtils.InitLogHandler(logPath: logPath);
            EyesRunner runner = useVisualGrid ? (EyesRunner)new VisualGridRunner(10, logHandler) : new ClassicRunner(logHandler);
            if (useVisualGrid && !TestUtils.RUNS_ON_CI)
            {
                ((VisualGridRunner)runner).DebugResourceWriter = new FileDebugResourceWriter(logPath);
            }
            Eyes eyes = new Eyes(runner);
            eyes.Batch = TestDataProvider.BatchInfo;
            try
            {
                driver.Url = "https://applitools.com/helloworld";
                eyes.Open(driver, nameof(TestApiMethods), nameof(TestCloseAsync) + "_1", new Size(800, 600));
                eyes.Check(Target.Window().WithName("step 1"));
                eyes.CloseAsync();
                driver.FindElement(By.TagName("button")).Click();
                eyes.Open(driver, nameof(TestApiMethods), nameof(TestCloseAsync) + "_2", new Size(800, 600));
                eyes.Check(Target.Window().WithName("step 2"));
                eyes.CloseAsync();
                runner.GetAllTestResults();
            }
            finally
            {
                driver.Quit();
            }
        }

        [TestCase(false, null, null, false)]
        [TestCase(false, null, false, false)]
        [TestCase(false, null, true, true)]
        [TestCase(false, false, null, false)]
        [TestCase(false, false, false, false)]
        [TestCase(false, false, true, false)]
        [TestCase(false, true, null, true)]
        [TestCase(false, true, false, true)]
        [TestCase(false, true, true, true)]
        [TestCase(true, null, null, true)]
        [TestCase(true, null, false, false)]
        [TestCase(true, null, true, true)]
        [TestCase(true, false, null, false)]
        [TestCase(true, false, false, false)]
        [TestCase(true, false, true, false)]
        [TestCase(true, true, null, true)]
        [TestCase(true, true, false, true)]
        [TestCase(true, true, true, true)]
        public void TestFullyInConfiguration(bool useVisualGrid, bool? checkSettingsValue, bool? forceFullPageScreenshot, bool expected)
        {

            ILogHandler logHandler = TestUtils.InitLogHandler();
            EyesRunner runner = useVisualGrid ? (EyesRunner)new VisualGridRunner(10, logHandler) : new ClassicRunner(logHandler);
            Eyes eyes = new MockEyes(runner);
         
            if (forceFullPageScreenshot.HasValue)
            {
                var config = eyes.GetConfiguration();
                config.SetForceFullPageScreenshot(forceFullPageScreenshot.Value);
                eyes.SetConfiguration(config);
            }

            RunIsFullyTest_(checkSettingsValue, expected, eyes);
        }

        [TestCase(false, null, null, false)]
        [TestCase(false, null, false, false)]
        [TestCase(false, null, true, true)]
        [TestCase(false, false, null, false)]
        [TestCase(false, false, false, false)]
        [TestCase(false, false, true, false)]
        [TestCase(false, true, null, true)]
        [TestCase(false, true, false, true)]
        [TestCase(false, true, true, true)]
        [TestCase(true, null, null, true)]
        [TestCase(true, null, false, false)]
        [TestCase(true, null, true, true)]
        [TestCase(true, false, null, false)]
        [TestCase(true, false, false, false)]
        [TestCase(true, false, true, false)]
        [TestCase(true, true, null, true)]
        [TestCase(true, true, false, true)]
        [TestCase(true, true, true, true)]
        public void TestFullyDirectlyOnEyes(bool useVisualGrid, bool? checkSettingsValue, bool? forceFullPageScreenshot, bool expected)
        {
            ILogHandler logHandler = TestUtils.InitLogHandler();
            EyesRunner runner = useVisualGrid ? (EyesRunner)new VisualGridRunner(10, logHandler) : new ClassicRunner(logHandler);
            Eyes eyes = new MockEyes(runner);

            if (forceFullPageScreenshot.HasValue)
            {
                eyes.ForceFullPageScreenshot = forceFullPageScreenshot.Value;
            }

            RunIsFullyTest_(checkSettingsValue, expected, eyes);
        }

        private static void RunIsFullyTest_(bool? checkSettingsValue, bool expected, Eyes eyes)
        {
            // Test Classic API
            eyes.CheckWindow(fully: checkSettingsValue);
            bool actual = ((IMockEyes)eyes.activeEyes_).ActualFullyValue;
            Assert.AreEqual(expected, actual);

            // Test Fluent API
            var target = Target.Window();
            if (checkSettingsValue.HasValue)
            {
                target.Fully(checkSettingsValue.Value);
            }
            eyes.Check(target);
            actual = ((IMockEyes)eyes.activeEyes_).ActualFullyValue;
            Assert.AreEqual(expected, actual);
        }

        interface IMockEyes
        {
            bool ActualFullyValue { get; set; }
        }

        class MockEyes : Eyes
        {
            public MockEyes(EyesRunner runner) : base(runner)
            {
            }

            internal override void CheckImpl_(ICheckSettings checkSettings)
            {
                activeEyes_.Check(checkSettings);
            }

            internal override VisualGridEyes CreateVisualGridEyes_(VisualGridRunner visualGridRunner)
            {
                return new MockVisualGridEyes(this, visualGridRunner);
            }

            internal override SeleniumEyes CreateSeleniumEyes_(EyesRunner runner)
            {
                return new MockSeleniumEyes(this, (ClassicRunner)runner);
            }
        }

        class MockVisualGridEyes : VisualGridEyes, IMockEyes
        {
            public bool ActualFullyValue { get; set; }
            internal MockVisualGridEyes(ISeleniumConfigurationProvider configurationProvider, VisualGridRunner visualGridRunner)
                : base(configurationProvider, visualGridRunner)
            {
                IsDisabled = true; // so we won't make all the actual Open, Close, networking, etc. calls.
            }

            internal override void CheckImpl_(ICheckSettings checkSettings)
            {
                checkSettings = UpdateCheckSettings_(checkSettings);
                bool? stitch = ((ICheckSettingsInternal)checkSettings).GetStitchContent();
                Assert.NotNull(stitch);
                ActualFullyValue = stitch.Value;
            }
        }

        class MockSeleniumEyes : SeleniumEyes, IMockEyes
        {
            public bool ActualFullyValue { get; set; }
            public MockSeleniumEyes(ISeleniumConfigurationProvider configurationProvider, ClassicRunner runner)
                : base(configurationProvider, runner)
            {
                IsDisabled = true;
            }

            public MockSeleniumEyes(ISeleniumConfigurationProvider configurationProvider, Uri serverUrl, ClassicRunner runner)
                : base(configurationProvider, serverUrl, runner)
            {
                IsDisabled = true;
            }

            internal override void CheckImpl_(ICheckSettings checkSettings)
            {
                ICheckSettingsInternal checkSettingsInternal = (ICheckSettingsInternal)checkSettings;
                ISeleniumCheckTarget seleniumCheckTarget = (ISeleniumCheckTarget)checkSettings;
                CheckState state = InitCheckState_(checkSettings, checkSettingsInternal, seleniumCheckTarget);
                ActualFullyValue = state.StitchContent;
            }
        }
    }
}
