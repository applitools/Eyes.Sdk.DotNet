using Applitools.Selenium.Tests.Mock;
using Applitools.Selenium.Tests.Utils;
using Applitools.Selenium.VisualGrid;
using Applitools.Tests.Utils;
using Applitools.Ufg;
using Applitools.Ufg.Model;
using Applitools.Utils;
using Applitools.Utils.Geometry;
using Applitools.VisualGrid;
using NUnit.Framework;
using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Applitools.Selenium.Tests.VisualGridTests
{
    [Parallelizable(ParallelScope.All)]
    public class TestVisualGridApi : ReportingTestSuite
    {
        private class MockEyesConnector : EyesConnector
        {
            public MockEyesConnector(Logger logger, RenderBrowserInfo browserInfo, Applitools.Configuration configuration)
                : base(logger, browserInfo, configuration, new MockServerConnectorFactory())
            {
                Logger.Verbose("created");
            }
            public RenderRequest[] LastRenderRequests { get; set; }
            public string RenderId { get; set; } = "47A4C2BD-0349-4232-B588-C9B9DA77498B";
            public string JobId { get; set; } = "A72E234C-58AA-4406-B8FD-8899FACEA147";

            public override async Task<List<RunningRender>> RenderAsync(RenderRequest[] renderRequests)
            {
                LastRenderRequests = renderRequests;
                Logger.Verbose("mock-rendering {0} render requests...", LastRenderRequests.Length);
                List<RunningRender> runningRenders = new List<RunningRender>();
                foreach (RenderRequest request in renderRequests)
                {
                    RunningRender render = new RunningRender(RenderId, JobId, RenderStatus.Rendered, null, false);
                    runningRenders.Add(render);
                }
                await Task.Delay(10);
                Logger.Verbose("mock-rendered {0} requests", runningRenders.Count);
                return runningRenders;
            }

            public override List<RenderStatusResults> RenderStatusById(string[] renderIds)
            {
                List<RenderStatusResults> results = new List<RenderStatusResults>();
                foreach (string renderId in renderIds)
                {
                    RenderStatusResults result = new RenderStatusResults()
                    {
                        RenderId = renderId,
                        Status = RenderStatus.Rendered
                    };
                    results.Add(result);
                }
                return results;
            }

            public override RenderingInfo GetRenderingInfo()
            {
                return renderInfo_ ?? new RenderingInfo();
            }

            public override void SetRenderInfo(RenderingInfo renderingInfo)
            {
                renderInfo_ = renderingInfo;
            }

            public override MatchResult MatchWindow(Applitools.IConfiguration config, string resultImageURL,
                string domLocation, ICheckSettings checkSettings, IList<IRegion> regions,
                IList<VisualGridSelector[]> regionSelectors, Location location, RenderStatusResults results, string source)
            {
                return new MatchResult() { AsExpected = true };
            }

            public override bool?[] CheckResourceStatus(string renderId, HashObject[] hashes)
            {
                bool?[] arr = new bool?[hashes.Length];
                Array.Fill(arr, true);
                return arr;
            }
        }

        private class MockEyesConnectorFactory : IEyesConnectorFactory
        {
            public IUfgConnector CreateNewEyesConnector(Logger logger, RenderBrowserInfo browserInfo, Applitools.Configuration config)
            {
                logger.Verbose($"creating {nameof(MockEyesConnector)}");
                return new MockEyesConnector(logger, browserInfo, config);
            }
        }

        [Test]
        public void TestVisualGridOptions()
        {
            // We want VG mode
            EyesRunner runner = new VisualGridRunner(10);
            ILogHandler logHandler = TestUtils.InitLogHandler();
            runner.SetLogHandler(logHandler);
            Eyes eyes = new Eyes(runner);
            eyes.visualGridEyes_.EyesConnectorFactory = new MockEyesConnectorFactory();
            Configuration config = eyes.GetConfiguration();
            config.AddBrowser(800, 600, BrowserType.CHROME);
            config.SetVisualGridOptions(new VisualGridOption("option1", "value1"), new VisualGridOption("option2", false));
            config.SetBatch(TestDataProvider.BatchInfo);
            eyes.SetConfiguration(config);

            IWebDriver driver = SeleniumUtils.CreateChromeDriver();
            driver.Url = "https://applitools.github.io/demo/TestPages/DynamicResolution/desktop.html";
            try
            {
                IWebDriver eyesDriver;
                // First check - global + fluent config
                eyes.Logger.Verbose("starting first check");
                MockEyesConnector mockEyesConnector = OpenEyesAndGetConnector_(eyes, config, driver, out eyesDriver);
                eyes.Logger.Verbose("calling eyes.Check");
                eyes.Check(Target.Window().VisualGridOptions(new VisualGridOption("option3", "value3"), new VisualGridOption("option4", 5)));
                eyes.Logger.Verbose("calling eyes.Close");
                eyes.Close();
                var expected1 = new Dictionary<string, object>()
                {
                    {"option1", "value1"}, {"option2", false}, {"option3", "value3"}, {"option4", 5}
                };
                eyes.Logger.Verbose("getting results...");
                var actual1 = mockEyesConnector.LastRenderRequests[0].Options;
                eyes.Logger.Verbose("comparing results...");
                CollectionAssert.AreEquivalent(expected1, actual1);
                eyes.Logger.Verbose("done first check");


                // Second check - only global
                mockEyesConnector = OpenEyesAndGetConnector_(eyes, config, driver, out eyesDriver);
                eyes.CheckWindow();
                eyes.Close();
                var expected2 = new Dictionary<string, object>()
                {
                    {"option1", "value1"}, {"option2", false}
                };
                var actual2 = mockEyesConnector.LastRenderRequests[0].Options;
                CollectionAssert.AreEquivalent(expected2, actual2);

                // Third check - resetting
                mockEyesConnector = OpenEyesAndGetConnector_(eyes, config, driver, out eyesDriver);
                config = eyes.GetConfiguration();
                config.SetVisualGridOptions(null);
                eyes.SetConfiguration(config);
                eyes.CheckWindow();
                eyes.Close();
                var actual3 = mockEyesConnector.LastRenderRequests[0].Options;
                Assert.IsNull(actual3);
                runner.GetAllTestResults();
            }
            finally
            {
                eyes.AbortIfNotClosed();
                driver.Close();
            }
        }

        [Test]
        public void TestVisualGridSkipList()
        {
            VisualGridRunner runner = new VisualGridRunner(10);
            Eyes eyes = new Eyes(runner);
            TestUtils.SetupLogging(eyes);
            eyes.visualGridEyes_.EyesConnectorFactory = new MockEyesConnectorFactory();

            Configuration config = eyes.GetConfiguration();
            config.AddBrowser(1050, 600, BrowserType.CHROME);
            config.SetBatch(TestDataProvider.BatchInfo);
            eyes.SetConfiguration(config);

            AutoResetEvent waitHandle = new AutoResetEvent(false);
            runner.debugLock_ = waitHandle;

            IWebDriver driver = SeleniumUtils.CreateChromeDriver();
            //driver.Url = "https://applitools.github.io/demo/DomSnapshot/test-iframe.html";
            driver.Url = "https://applitools.github.io/demo/TestPages/VisualGridTestPage/";
            try
            {
                MockEyesConnector mockEyesConnector = OpenEyesAndGetConnector_(eyes, config, driver, out IWebDriver eyesDriver);

                eyes.Check(Target.Window());
                string[] expectedUrls = new string[] {
                    "https://applitools.github.io/demo/TestPages/VisualGridTestPage/AbrilFatface-Regular.woff2",
                    "https://applitools.github.io/demo/TestPages/VisualGridTestPage/applitools_logo_combined.svg",
                    "https://applitools.github.io/demo/TestPages/VisualGridTestPage/company_name.png",
                    "https://applitools.github.io/demo/TestPages/VisualGridTestPage/frame.html",
                    "https://applitools.github.io/demo/TestPages/VisualGridTestPage/innerstyle0.css",
                    "https://applitools.github.io/demo/TestPages/VisualGridTestPage/innerstyle1.css",
                    "https://applitools.github.io/demo/TestPages/VisualGridTestPage/innerstyle2.css",
                    "https://applitools.github.io/demo/TestPages/VisualGridTestPage/logo.svg",
                    "https://applitools.github.io/demo/TestPages/VisualGridTestPage/minions-800x500_green_sideways.png",
                    "https://applitools.github.io/demo/TestPages/VisualGridTestPage/minions-800x500.jpg",
                    "https://applitools.github.io/demo/TestPages/VisualGridTestPage/slogan.svg",
                    "https://applitools.github.io/demo/TestPages/VisualGridTestPage/style0.css",
                    "https://applitools.github.io/demo/TestPages/VisualGridTestPage/style1.css",
                    "https://fonts.googleapis.com/css?family=Raleway",
                    "https://fonts.googleapis.com/css?family=Unlock",
                    "https://fonts.gstatic.com/s/raleway/v18/1Ptxg8zYS_SKggPN4iEgvnHyvveLxVvaorCFPrEHJA.woff2",
                    "https://fonts.gstatic.com/s/raleway/v18/1Ptxg8zYS_SKggPN4iEgvnHyvveLxVvaorCGPrEHJA.woff2",
                    "https://fonts.gstatic.com/s/raleway/v18/1Ptxg8zYS_SKggPN4iEgvnHyvveLxVvaorCHPrEHJA.woff2",
                    "https://fonts.gstatic.com/s/raleway/v18/1Ptxg8zYS_SKggPN4iEgvnHyvveLxVvaorCIPrE.woff2",
                    "https://fonts.gstatic.com/s/raleway/v18/1Ptxg8zYS_SKggPN4iEgvnHyvveLxVvaorCMPrEHJA.woff2",
                    "https://fonts.gstatic.com/s/unlock/v10/7Au-p_8ykD-cDl72LwLT.woff2",
                    "https://use.fontawesome.com/releases/v5.8.2/css/all.css",
                    "https://use.fontawesome.com/releases/v5.8.2/webfonts/fa-brands-400.eot",
                    "https://use.fontawesome.com/releases/v5.8.2/webfonts/fa-brands-400.svg",
                    "https://use.fontawesome.com/releases/v5.8.2/webfonts/fa-brands-400.ttf",
                    "https://use.fontawesome.com/releases/v5.8.2/webfonts/fa-brands-400.woff",
                    "https://use.fontawesome.com/releases/v5.8.2/webfonts/fa-brands-400.woff2",
                    "https://use.fontawesome.com/releases/v5.8.2/webfonts/fa-regular-400.eot",
                    "https://use.fontawesome.com/releases/v5.8.2/webfonts/fa-regular-400.svg",
                    "https://use.fontawesome.com/releases/v5.8.2/webfonts/fa-regular-400.ttf",
                    "https://use.fontawesome.com/releases/v5.8.2/webfonts/fa-regular-400.woff",
                    "https://use.fontawesome.com/releases/v5.8.2/webfonts/fa-regular-400.woff2",
                    "https://use.fontawesome.com/releases/v5.8.2/webfonts/fa-solid-900.eot",
                    "https://use.fontawesome.com/releases/v5.8.2/webfonts/fa-solid-900.svg",
                    "https://use.fontawesome.com/releases/v5.8.2/webfonts/fa-solid-900.ttf",
                    "https://use.fontawesome.com/releases/v5.8.2/webfonts/fa-solid-900.woff",
                    "https://use.fontawesome.com/releases/v5.8.2/webfonts/fa-solid-900.woff2"};

                waitHandle.WaitOne(TimeSpan.FromSeconds(5));

                CollectionAssert.AreEquivalent(expectedUrls, ((IVisualGridRunner)runner).CachedBlobsURLs.Keys);

                UserAgent userAgent = eyes.visualGridEyes_.userAgent_;
                EyesWebDriverTargetLocator switchTo = (EyesWebDriverTargetLocator)driver.SwitchTo();
                FrameData domData = VisualGridEyes.CaptureDomSnapshot_(switchTo, userAgent, runner, (EyesWebDriver)eyesDriver, eyes.Logger);
                DomAnalyzer domAnalyzer = new DomAnalyzer(runner,
                    domData,
                    eyes.visualGridEyes_.eyesConnector_,
                    userAgent,
                    eyes.visualGridEyes_.debugResourceWriter_);
                IDictionary<string, RGridResource> resourceMap = domAnalyzer.Analyze();
                CollectionAssert.AreEquivalent(expectedUrls, resourceMap.Keys);
                //eyes.Check(Target.Window());
                eyes.Close();

                runner.GetAllTestResults();
            }
            finally
            {
                eyes.AbortIfNotClosed();
                driver.Close();
            }
        }

        [Test]
        public void TestRunnerOptions()
        {
            RunnerOptions runnerOptions = new RunnerOptions().TestConcurrency(5);

            VisualGridRunner runner1 = new VisualGridRunner();
            Assert.AreEqual(VisualGridRunner.CONCURRENCY_FACTOR, ((IRunnerOptionsInternal)runner1.runnerOptions_).GetConcurrency());
            runner1.GetAllTestResults();

            VisualGridRunner runner2 = new VisualGridRunner(runnerOptions);
            Assert.AreEqual(5, ((IRunnerOptionsInternal)runner2.runnerOptions_).GetConcurrency());
            runner2.GetAllTestResults();

            VisualGridRunner runner3 = new VisualGridRunner(5);
            Assert.AreEqual(VisualGridRunner.CONCURRENCY_FACTOR * 5, ((IRunnerOptionsInternal)runner3.runnerOptions_).GetConcurrency());
            runner3.GetAllTestResults();
        }

        private static MockEyesConnector OpenEyesAndGetConnector_(Eyes eyes, Configuration config, IWebDriver webDriver, out IWebDriver eyesDriver)
        {
            eyesDriver = eyes.Open(webDriver, "Mock app", "Mock Test");

            MockEyesConnector mockEyesConnector = (MockEyesConnector)eyes.visualGridEyes_.eyesConnector_;
            //MockServerConnector mockServerConnector = new MockServerConnector(eyes.Logger, new Uri(eyes.ServerUrl));
            //EyesConnector eyesConnector = new EyesConnector(config.GetBrowsersInfo()[0], config)
            //    runningSession_ = new RunningSession(),
            //    ServerConnector = mockServerConnector
            //};
            //mockEyesConnector.WrappedConnector = eyesConnector;
            return mockEyesConnector;
        }
    }
}
