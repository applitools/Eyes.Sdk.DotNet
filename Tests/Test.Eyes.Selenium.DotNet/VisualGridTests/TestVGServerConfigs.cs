using Applitools.Metadata;
using Applitools.Selenium.Tests.Utils;
using Applitools.Tests.Utils;
using Applitools.Ufg;
using Applitools.VisualGrid;
using NUnit.Framework;
using OpenQA.Selenium;
using System;
using EyesVG = Applitools.Selenium.VisualGrid.VisualGridEyes;

namespace Applitools.Selenium.Tests.VisualGridTests
{
    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    public class TestVGServerConfigs : ReportingTestSuite
    {
        public static object[] TestArgs()
        {
            string server1 = "https://eyesapi.applitools.com/";
            string server2 = "https://fabricdemoeyes.applitools.com/";
            string key1 = Environment.GetEnvironmentVariable("APPLITOOLS_API_KEY");
            string key2 = Environment.GetEnvironmentVariable("FABRICAM_DEMO_EYES_API_KEY");
            return new object[][]{
                new object[] {server1, server2, key1, key2, false, false, server2, key2, false},
                new object[] {null, server2, key1, key2, false, false, server2, key2, false},
                new object[] {null, null, key1, null, false, null, EyesBase.DefaultServerUrl, key1, false},
                new object[] {null, server2, key1, key2, false, null, server2, key2, false},
                new object[] {null, server2, null, key2, false, null, server2, key2, false},
                new object[] {null, server2, null, key2, false, true, server2, key2, true},
                new object[] {server1, null, key1, null, true, null, server1, key1, true},
                new object[] {server1, null, key1, null, true, false, server1, key1, false},
        };
        }

        //[TestCaseSource(nameof(TestArgs))]
        public void Test(string server1, string server2,
                         string key1, string key2,
                         bool isDisabled1,
                         bool? isDisabled2,
                         string expectedServer,
                         string expectedKey,
                         bool expectedDisabled)
        {
            VisualGridRunner renderingManager = CreateRenderingManager(server1, key1, isDisabled1);

            EyesVG eyes = new EyesVG(new ConfigurationProviderForTesting(), renderingManager);
            eyes.ServerUrl = server2;
            eyes.ApiKey = key2;
            eyes.IsDisabled = isDisabled2 ?? eyes.IsDisabled;
            DesktopBrowserInfo desktopBrowserInfo = new DesktopBrowserInfo(100, 100, BrowserType.FIREFOX, null);
            RenderBrowserInfo browserInfo = new RenderBrowserInfo(desktopBrowserInfo);
            IEyesConnector eyesConnector = eyes.CreateEyesConnector_(browserInfo, eyes.ApiKey);
            Assert.AreEqual(expectedServer, eyesConnector.ServerUrl);
            Assert.AreEqual(expectedKey, eyesConnector.ApiKey);
            Assert.AreEqual(expectedDisabled, eyesConnector.IsDisabled);
        }

        private VisualGridRunner CreateRenderingManager(string server1, string key1, bool isDisabled1)
        {
            VisualGridRunner renderingManager = new VisualGridRunner(3);
            renderingManager.SetLogHandler(new StdoutLogHandler(true));
            renderingManager.Logger.Log("enter");
            renderingManager.ServerUrl = server1;
            renderingManager.ApiKey = key1;
            renderingManager.IsDisabled = isDisabled1;
            return renderingManager;
        }

        [Test, Parallelizable]
        public void TestVGDoubleCloseNoCheck()
        {
            IWebDriver driver = SeleniumUtils.CreateChromeDriver();
            try
            {
                VisualGridRunner runner = new VisualGridRunner(10);
                Eyes eyes = new Eyes(runner);
                Configuration conf = new Configuration();
                conf.SetAppName("app").SetTestName("test");
                conf.SetBatch(TestDataProvider.BatchInfo);
                eyes.SetConfiguration(conf);

                eyes.Open(driver);
                Assert.That(() => { eyes.Close(); }, Throws.InvalidOperationException.With.Property("Message").EqualTo("Eyes not open"));
            }
            finally
            {
                driver.Quit();
            }
        }

        [TestCase(true)]
        [TestCase(false)]
        public void TestChangeConfigAfterOpen(bool useVisualGrid)
        {
            IWebDriver driver = SeleniumUtils.CreateChromeDriver();
            try
            {
                EyesRunner runner = useVisualGrid ? (EyesRunner)new VisualGridRunner(10) : new ClassicRunner();
                Eyes eyes = new Eyes(runner);

                Configuration conf = new Configuration();
                
                conf.SetAppName("app").SetTestName("test");
                conf.SetBatch(TestDataProvider.BatchInfo);

                conf.SetAccessibilityValidation(null).SetIgnoreDisplacements(false);
                eyes.SetConfiguration(conf);

                eyes.Open(driver);

                eyes.CheckWindow();

                conf.SetAccessibilityValidation(new AccessibilitySettings(AccessibilityLevel.AAA, AccessibilityGuidelinesVersion.WCAG_2_1)).SetIgnoreDisplacements(true);
                eyes.SetConfiguration(conf);

                eyes.CheckWindow();

                conf.SetAccessibilityValidation(new AccessibilitySettings(AccessibilityLevel.AA, AccessibilityGuidelinesVersion.WCAG_2_0)).SetMatchLevel(MatchLevel.Layout);
                eyes.SetConfiguration(conf);

                eyes.CheckWindow();

                eyes.Close(false);
                TestResultsSummary resultsSummary = runner.GetAllTestResults(false);

                Assert.AreEqual(1, resultsSummary.Count);

                TestResultContainer resultsContainer = resultsSummary[0];
                TestResults results = resultsContainer.TestResults;

                SessionResults sessionResults = TestUtils.GetSessionResults(eyes.ApiKey, results);

                string browserInfo = resultsContainer.BrowserInfo?.ToString();

                Assert.IsNull(sessionResults.StartInfo.DefaultMatchSettings.AccessibilitySettings, browserInfo);
                Assert.IsFalse(sessionResults.StartInfo.DefaultMatchSettings.IgnoreDisplacements, browserInfo);
                Assert.AreEqual(MatchLevel.Strict, sessionResults.StartInfo.DefaultMatchSettings.MatchLevel, browserInfo);

                Assert.AreEqual(3, sessionResults.ActualAppOutput.Length, browserInfo);

                Assert.IsNull(sessionResults.ActualAppOutput[0].ImageMatchSettings.AccessibilitySettings, browserInfo);

                Assert.AreEqual(AccessibilityLevel.AAA, sessionResults.ActualAppOutput[1].ImageMatchSettings.AccessibilitySettings.Level, browserInfo);
                Assert.AreEqual(AccessibilityGuidelinesVersion.WCAG_2_1, sessionResults.ActualAppOutput[1].ImageMatchSettings.AccessibilitySettings.GuidelinesVersion, browserInfo);
                Assert.IsTrue(sessionResults.ActualAppOutput[1].ImageMatchSettings.IgnoreDisplacements, browserInfo);
                Assert.AreEqual(MatchLevel.Strict, sessionResults.ActualAppOutput[1].ImageMatchSettings.MatchLevel, browserInfo);

                Assert.AreEqual(AccessibilityLevel.AA, sessionResults.ActualAppOutput[2].ImageMatchSettings.AccessibilitySettings.Level, browserInfo);
                Assert.AreEqual(AccessibilityGuidelinesVersion.WCAG_2_0, sessionResults.ActualAppOutput[2].ImageMatchSettings.AccessibilitySettings.GuidelinesVersion, browserInfo);
                Assert.IsTrue(sessionResults.ActualAppOutput[2].ImageMatchSettings.IgnoreDisplacements, browserInfo);
                Assert.AreEqual(MatchLevel.Layout2, sessionResults.ActualAppOutput[2].ImageMatchSettings.MatchLevel, browserInfo);
            }
            finally
            {
                driver.Quit();
            }
        }
    }
}