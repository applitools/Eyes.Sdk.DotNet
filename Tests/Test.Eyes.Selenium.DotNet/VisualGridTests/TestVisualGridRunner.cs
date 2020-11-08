using Applitools.Selenium.Tests.Utils;
using Applitools.Selenium.VisualGrid;
using Applitools.Tests.Utils;
using Applitools.Ufg;
using Applitools.Utils.Geometry;
using Applitools.VisualGrid;
using NUnit.Framework;
using OpenQA.Selenium;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ScreenOrientation = Applitools.VisualGrid.ScreenOrientation;

namespace Applitools.Selenium.Tests.VisualGridTests
{
    [Parallelizable(ParallelScope.All)]
    public class TestVisualGridRunner
    {
        private class ConfigurationProvider : ISeleniumConfigurationProvider
        {
            public ConfigurationProvider(Configuration config)
            {
                configuration_ = config;
            }
            private readonly Configuration configuration_;
            public Configuration GetConfiguration()
            {
                return configuration_;
            }
        }

        private class Events
        {
            public Func<RunningSession> BeforeStartSession { get; internal set; }
            public Action AfterEndSession { get; internal set; }
        }

        private class MockServerConnector : ServerConnector
        {
            public MockServerConnector(Logger logger, Uri serverUrl = null, Events events = null) : base(logger, serverUrl)
            {
                events_ = events;
            }

            private readonly Events events_;

            protected override RunningSession StartSessionInternal(SessionStartInfo startInfo)
            {
                RunningSession result = events_?.BeforeStartSession?.Invoke();
                if (result == null)
                {
                    result = base.StartSessionInternal(startInfo);
                }
                return result;
            }

            protected override TestResults EndSessionInternal(RunningSession runningSession, bool isAborted, bool save)
            {
                TestResults result = base.EndSessionInternal(runningSession, isAborted, save);
                events_?.AfterEndSession?.Invoke();
                return result;
            }
        }

        private class MockServerConnectorFactory : IServerConnectorFactory
        {
            public Events Events { get; set; }
            public IServerConnector CreateNewServerConnector(Logger logger, Uri serverUrl = null)
            {
                return new MockServerConnector(logger, serverUrl, Events);
            }
        }

        private class MockEyesConnector : EyesConnector
        {
            public MockEyesConnector(Logger logger, RenderBrowserInfo browserInfo, Applitools.Configuration configuration, Events events)
                : base(logger, browserInfo, configuration, new MockServerConnectorFactory() { Events = events })
            {
            }

            public delegate void BeforeRenderEventHandler(RenderRequest[] requests);
            public event BeforeRenderEventHandler BeforeRender = null;

            public delegate void AfterRenderEventHandler(Task<List<RunningRender>> results, RenderRequest[] requests);
            public event AfterRenderEventHandler AfterRender = null;

            public override Task<List<RunningRender>> RenderAsync(RenderRequest[] requests)
            {
                BeforeRender?.Invoke(requests);
                Task<List<RunningRender>> result = base.RenderAsync(requests);
                AfterRender(result, requests);
                return result;
            }
        }

        private class MockEyesConnectorFactory : IEyesConnectorFactory
        {
            public Events Events { get; set; }

            public IUfgConnector CreateNewEyesConnector(Logger logger, RenderBrowserInfo browserInfo, Applitools.Configuration config)
            {
                return new MockEyesConnector(logger, browserInfo, config, Events);
            }
        }

        [Test]
        public void TestOpenBeforeRender()
        {
            VisualGridRunner runner = new VisualGridRunner(10);
            Eyes eyes = new Eyes(runner);
            eyes.SetLogHandler(TestUtils.InitLogHandler());
            eyes.visualGridEyes_.EyesConnectorFactory = new MockEyesConnectorFactory();

            Configuration config = eyes.GetConfiguration();
            config.AddBrowser(new IosDeviceInfo(IosDeviceName.iPhone_7));
            eyes.SetConfiguration(config);

            object errorMessageLocker = new object();
            string errorMessage = null;

            IWebDriver driver = SeleniumUtils.CreateChromeDriver();
            driver.Url = "http://applitools.github.io/demo";
            try
            {
                eyes.Open(driver, "Mock app", "Mock Test");
                MockEyesConnector mockEyesConnector = (MockEyesConnector)eyes.visualGridEyes_.eyesConnector_;

                mockEyesConnector.BeforeRender += (renderRequests) =>
                {
                    IVisualGridEyes eyes = runner.allEyes_.First();
                    if (!eyes.GetAllRunningTests().First().IsTestOpen)
                    {
                        lock (errorMessageLocker)
                        {
                            errorMessage = "Render called before open";
                        }
                    }
                };
                eyes.CheckWindow();
                eyes.CloseAsync();
            }
            finally
            {
                eyes.AbortAsync();
                driver.Quit();
                runner.GetAllTestResults(false);
            }

            Assert.IsNull(errorMessage);
        }

        [Test]
        public void TestRunnerConcurrency()
        {
            Configuration config = new Configuration();

            config.AddBrowser(new IosDeviceInfo(IosDeviceName.iPhone_7));
            config.AddDeviceEmulation(DeviceName.Galaxy_S5, ScreenOrientation.Landscape);
            config.AddBrowser(new DesktopBrowserInfo(800, 800, BrowserType.CHROME));
            config.AddBrowser(new DesktopBrowserInfo(800, 800, BrowserType.FIREFOX));
            config.AddBrowser(new DesktopBrowserInfo(800, 800, BrowserType.SAFARI));

            ConfigurationProvider configurationProvider = new ConfigurationProvider(config);
            VisualGridRunner runner = new VisualGridRunner(new RunnerOptions().TestConcurrency(3));
            VisualGridEyes eyes = new VisualGridEyes(configurationProvider, runner);
            eyes.SetLogHandler(TestUtils.InitLogHandler());

            int currentlyOpenTests = 0;
            bool isFail = false;

            eyes.EyesConnectorFactory = new MockEyesConnectorFactory()
            {
                Events = new Events()
                {
                    BeforeStartSession = () =>
                    {
                        if (Interlocked.Increment(ref currentlyOpenTests) > 3)
                        {
                            isFail = true;
                        }
                        Thread.Sleep(3000);
                        if (currentlyOpenTests > 3)
                        {
                            isFail = true;
                        }
                        return null;
                    },
                    AfterEndSession = () =>
                    {
                        Interlocked.Decrement(ref currentlyOpenTests);
                    }
                }
            };

            IWebDriver driver = SeleniumUtils.CreateChromeDriver();
            driver.Url = "http://applitools.github.io/demo";
            try
            {
                eyes.Open(driver, "Eyes SDK", "UFG Runner Concurrency", new Size(800, 800));
                eyes.Check(Target.Window().Fully());
                eyes.CloseAsync(true);
            }
            finally
            {
                eyes.AbortAsync();
                driver.Quit();
                runner.GetAllTestResults(false);
            }

            Assert.IsFalse(isFail, "Number of open tests was higher than the concurrency limit");
        }

        [Test]
        public void TestRetryWhenServerConcurrencyLimitReached()
        {
            Configuration config = new Configuration();

            config.AddBrowser(new IosDeviceInfo(IosDeviceName.iPhone_7));

            ConfigurationProvider configurationProvider = new ConfigurationProvider(config);
            VisualGridRunner runner = new VisualGridRunner(new RunnerOptions().TestConcurrency(3));
            VisualGridEyes eyes = new VisualGridEyes(configurationProvider, runner);
            eyes.SetLogHandler(TestUtils.InitLogHandler());

            int counter = 0;
            bool wasConcurrencyFull = false;

            eyes.EyesConnectorFactory = new MockEyesConnectorFactory()
            {
                Events = new Events()
                {
                    BeforeStartSession = () =>
                    {
                        if (Interlocked.Increment(ref counter) < 4)
                        {
                            RunningSession newSession = new RunningSession();
                            newSession.ConcurrencyFull = true;
                            return newSession;
                        }
                        return null;
                    }
                }
            };

            eyes.AfterServerConcurrencyLimitReachedQueried += (concurrecnyReached) =>
            {
                if (concurrecnyReached)
                {
                    wasConcurrencyFull = true;
                }
            };

            IWebDriver driver = SeleniumUtils.CreateChromeDriver();
            driver.Url = "http://applitools.github.io/demo";
            try
            {
                eyes.Open(driver, "Eyes SDK", "UFG Runner Concurrency", new Size(800, 800));
                eyes.Check(Target.Window().Fully());
                eyes.CloseAsync(true);
            }
            finally
            {
                eyes.AbortAsync();
                driver.Quit();
                runner.GetAllTestResults(false);
            }

            Assert.IsTrue(wasConcurrencyFull);
            Assert.IsFalse(((IVisualGridEyes)eyes).IsServerConcurrencyLimitReached());
            Assert.AreEqual(4, counter);
        }

        [Test]
        public void TestResetContent()
        {
            Configuration config = new Configuration();

            config.AddBrowser(new IosDeviceInfo(IosDeviceName.iPhone_7));
            ConfigurationProvider configurationProvider = new ConfigurationProvider(config);
            VisualGridRunner runner = new VisualGridRunner(new RunnerOptions().TestConcurrency(3));
            VisualGridEyes eyes = new VisualGridEyes(configurationProvider, runner);
            eyes.SetLogHandler(TestUtils.InitLogHandler());

            IWebDriver driver = SeleniumUtils.CreateChromeDriver();
            driver.Url = "http://applitools.github.io/demo";
            try
            {
                eyes.Open(driver, "Eyes SDK", "UFG Runner Concurrency", new Size(800, 800));
                eyes.Check(Target.Window().Fully());
                eyes.CloseAsync(true);
            }
            finally
            {
                eyes.AbortAsync();
                driver.Quit();
                runner.GetAllTestResults(false);
            }

            foreach (ResourceFuture resourceFuture in ((IVisualGridRunner)runner).CachedResources.Values)
            {
                RGridResource resource = resourceFuture.GetResource();
                Assert.IsNotNull(resource);
                Assert.IsNull(resource.Content);
            }
        }

        [Test]
        public void TestConcurrencyAmount()
        {
            VisualGridRunner runner = new VisualGridRunner();
            Assert.AreEqual(VisualGridRunner.DEFAULT_CONCURRENCY, runner.testConcurrency_.ActualConcurrency);
            Assert.AreEqual(VisualGridRunner.DEFAULT_CONCURRENCY, runner.testConcurrency_.UserConcurrency);
            Assert.IsTrue(runner.testConcurrency_.IsDefault);
            Assert.IsFalse(runner.testConcurrency_.IsLegacy);

            runner = new VisualGridRunner(10);
            Assert.AreEqual(50, runner.testConcurrency_.ActualConcurrency);
            Assert.AreEqual(10, runner.testConcurrency_.UserConcurrency);
            Assert.IsFalse(runner.testConcurrency_.IsDefault);
            Assert.IsTrue(runner.testConcurrency_.IsLegacy);


            runner = new VisualGridRunner(new RunnerOptions().TestConcurrency(5));
            Assert.AreEqual(5, runner.testConcurrency_.ActualConcurrency);
            Assert.AreEqual(5, runner.testConcurrency_.UserConcurrency);
            Assert.IsFalse(runner.testConcurrency_.IsDefault);
            Assert.IsFalse(runner.testConcurrency_.IsLegacy);

            runner = new VisualGridRunner(new RunnerOptions().TestConcurrency(5).TestConcurrency(7));
            Assert.AreEqual(7, runner.testConcurrency_.ActualConcurrency);
            Assert.AreEqual(7, runner.testConcurrency_.UserConcurrency);
            Assert.IsFalse(runner.testConcurrency_.IsDefault);
            Assert.IsFalse(runner.testConcurrency_.IsLegacy);
        }

        [Test]
        public void TestConcurrencyLogMessage()
        {
            VisualGridRunner runner = new VisualGridRunner();
            CollectionAssert.AreEquivalent(
                new Dictionary<string, object>
                {
                    { "type", "runnerStarted" },
                    { "defaultConcurrency", VisualGridRunner.DEFAULT_CONCURRENCY }
                },
                (IDictionary)runner.GetConcurrencyLog());

            runner = new VisualGridRunner(10);
            CollectionAssert.AreEquivalent(
                new Dictionary<string, object>
                {
                    { "type", "runnerStarted" },
                    { "concurrency", 10 }
                },
                (IDictionary)runner.GetConcurrencyLog());

            runner = new VisualGridRunner(new RunnerOptions().TestConcurrency(10));
            CollectionAssert.AreEquivalent(
              new Dictionary<string, object>
              {
                    { "type", "runnerStarted" },
                    { "testConcurrency", 10 }
              },
              (IDictionary)runner.GetConcurrencyLog());

            Assert.IsNull(runner.GetConcurrencyLog());
        }

        [Test]
        public void TestParallelStepsLimitOfTest()
        {
            bool isOnlyOneRender = true;
            int runningRendersCount = 0;

            VisualGridRunner runner = new VisualGridRunner();
            Configuration config = new Configuration();
            config.AddBrowser(new IosDeviceInfo(IosDeviceName.iPhone_7));
            ConfigurationProvider configurationProvider = new ConfigurationProvider(config);

            VisualGridEyes eyes = new VisualGridEyes(configurationProvider, runner);
            eyes.SetLogHandler(TestUtils.InitLogHandler());
            eyes.EyesConnectorFactory = new MockEyesConnectorFactory();

            IWebDriver driver = SeleniumUtils.CreateChromeDriver();
            try
            {
                driver.Url = "http://applitools.github.io/demo";
                eyes.Open(driver, "Eyes SDK", "UFG Server Concurrency", new RectangleSize(800, 800));
                MockEyesConnector mockEyesConnector = (MockEyesConnector)eyes.eyesConnector_;

                mockEyesConnector.BeforeRender += (renderRequests) =>
                {
                    int runningRendersCountBeforeInc = runningRendersCount;
                    Interlocked.Increment(ref runningRendersCount);
                    if (runningRendersCountBeforeInc >= RunningTest.PARALLEL_STEPS_LIMIT)
                    {
                        isOnlyOneRender = false;
                    }

                    Thread.Sleep(1000);
                    if (renderRequests.Length != 1)
                    {
                        isOnlyOneRender = false;
                    }
                };

                mockEyesConnector.AfterRender += (results, renderRequests) =>
                {
                    Interlocked.Decrement(ref runningRendersCount);
                };

                for (int i = 0; i < 10; i++)
                {
                    eyes.Check(Target.Window().Fully());
                }
                eyes.CloseAsync(false);
            }
            finally
            {
                eyes.AbortAsync();
                driver.Quit();
                runner.GetAllTestResults(false);
            }

            Assert.IsTrue(isOnlyOneRender);
        }
    }
}
