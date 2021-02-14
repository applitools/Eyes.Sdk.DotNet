using Applitools.Selenium.Tests.Utils;
using Applitools.Tests.Utils;
using Applitools.VisualGrid;
using NUnit.Framework;
using OpenQA.Selenium;
using System;
using System.Drawing;
using System.Runtime.CompilerServices;

namespace Applitools.Selenium.Tests
{
    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    public class TestRendersMatch : ReportingTestSuite
    {
        [Test]
        public void TestSuccess()
        {
            ILogHandler logHandler = TestUtils.InitLogHandler();
            VisualGridRunner visualGridRunner = new VisualGridRunner(10, logHandler);

            Size[] ViewportList = {
                new Size(800, 600),
                new Size(700, 500),
                new Size(1200, 800),
                new Size(1600, 1200)
            };

            IWebDriver webDriver = SeleniumUtils.CreateChromeDriver();
            webDriver.Url = "https://applitools.com/helloworld";
            try
            {
                foreach (Size viewport in ViewportList)
                {
                    Eyes eyes = InitEyes_(null, webDriver, viewport);
                    eyes.Check(Target.Window().Fully());
                    eyes.CloseAsync();

                    try
                    {
                        eyes = InitEyes_(visualGridRunner, webDriver, viewport);
                        eyes.Check(Target.Window().Fully());
                        eyes.CloseAsync();
                    }
                    finally
                    {
                        eyes.AbortAsync();
                    }
                }
                TestResultsSummary results = visualGridRunner.GetAllTestResults();
                Assert.AreEqual(ViewportList.Length, results?.Count);
            }
            finally
            {
                webDriver?.Quit();
            }
        }

        [Test]
        public void TestFailure()
        {
            ILogHandler logHandler = TestUtils.InitLogHandler();
            VisualGridRunner visualGridRunner = new VisualGridRunner(10, logHandler);

            Size[] ViewportList = {
                new Size(800, 600),
                new Size(700, 500),
                new Size(1200, 800),
                new Size(1600, 1200)
            };

            IWebDriver webDriver = SeleniumUtils.CreateChromeDriver();
            webDriver.Url = "https://applitools.com/helloworld";
            Eyes eyes = null;
            try
            {
                int resultsTotal = 0;
                foreach (Size viewport in ViewportList)
                {
                    eyes = InitEyes_(null, webDriver, viewport);
                    eyes.Check(Target.Window().Fully());
                    eyes.Close();

                    eyes = InitEyes_(visualGridRunner, webDriver, viewport);
                    eyes.Check(Target.Window().Fully());
                    eyes.Close();
                    TestResultsSummary results = visualGridRunner.GetAllTestResults();
                    resultsTotal += results.Count;

                }
                Assert.AreEqual(4, resultsTotal);
            }
            catch (InvalidOperationException e)
            {
                if (e.Message.Equals("Runner already returned its results"))
                {
                    Assert.Pass();
                }
            }
            finally
            {
                webDriver?.Quit();
                eyes?.Abort();
            }
        }

        private Eyes InitEyes_(EyesRunner runner, IWebDriver driver, Size viewport, [CallerMemberName]string testName = null)
        {
            Eyes eyes = new Eyes(runner);

            Configuration sconf = new Configuration();
            sconf.SetBatch(TestDataProvider.BatchInfo);
            sconf.SetViewportSize(viewport);
            sconf.SetTestName(testName).SetAppName(nameof(TestRendersMatch));
            eyes.SetConfiguration(sconf);
            eyes.Open(driver);
            return eyes;
        }
    }
}
