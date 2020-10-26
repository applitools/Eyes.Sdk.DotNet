using Applitools.Selenium.Tests.Mock;
using Applitools.Selenium.Tests.Utils;
using Applitools.Selenium.VisualGrid;
using Applitools.Tests.Utils;
using Applitools.Ufg;
using Applitools.VisualGrid;
using NUnit.Framework;
using OpenQA.Selenium;
using System;
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
            public Action BeforeStartSession { get; internal set; }
            public Action AfterEndSession { get; internal set; }
        }

        private class MockServerConnector : ServerConnector
        {
            public MockServerConnector(Logger logger, Uri serverUrl = null, Events events = null) : base(logger, serverUrl)
            {
                BeforeStartSession = events?.BeforeStartSession;
                AfterEndSession = events?.AfterEndSession;
            }
            public Action BeforeStartSession { get; internal set; }
            public Action AfterEndSession { get; internal set; }

            protected override RunningSession StartSessionInternal(SessionStartInfo startInfo)
            {
                BeforeStartSession?.Invoke();
                return base.StartSessionInternal(startInfo);
            }

            protected override TestResults EndSessionInternal(RunningSession runningSession, bool isAborted, bool save)
            {
                TestResults result = base.EndSessionInternal(runningSession, isAborted, save);
                AfterEndSession?.Invoke();
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
            public MockEyesConnector(RenderBrowserInfo browserInfo, Applitools.Configuration configuration, Events events)
                : base(browserInfo, configuration, new MockServerConnectorFactory() { Events = events })
            {
            }

            public delegate void OnRenderEventHandler(RenderRequest[] requests);
            public event OnRenderEventHandler OnRender = null;

            public override Task<List<RunningRender>> RenderAsync(RenderRequest[] requests)
            {
                OnRender?.Invoke(requests);
                return base.RenderAsync(requests);
            }
        }

        private class MockEyesConnectorFactory : IEyesConnectorFactory
        {
            public Events Events { get; set; }

            public IUfgConnector CreateNewEyesConnector(RenderBrowserInfo browserInfo, Applitools.Configuration config)
            {
                return new MockEyesConnector(browserInfo, config, Events);
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

                mockEyesConnector.OnRender += (renderRequests) =>
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

        /*[Test]
        public void TestRetryWhenServerConcurrencyLimitReached()
        {
            VisualGridRunner runner = new VisualGridRunner(5);
            Eyes eyes = new Eyes(runner);
            TestUtils.SetupLogging(eyes);
            eyes.visualGridEyes_.EyesConnectorFactory = new MockEyesConnectorFactory();

            Configuration config = eyes.GetConfiguration();
            config.AddBrowser(new IosDeviceInfo(IosDeviceName.iPhone_7));
            eyes.SetConfiguration(config);

            MockEyesConnector mockEyesConnector;

            int counter = 0;
            bool wasConcurrencyFull = false;

            IWebDriver driver = SeleniumUtils.CreateChromeDriver();
            driver.Url = "http://applitools.github.io/demo";
            try
            {
                mockEyesConnector = OpenEyesAndGetConnector_(eyes, config, driver);
                EyesConnector eyesConnector = (EyesConnector)mockEyesConnector.WrappedConnector;
                MockServerConnector mockServerConnector = (MockServerConnector)eyesConnector.ServerConnector;

                mockServerConnector.OnStartSession += (sessionStartInfo) =>
                {
                    int counterBeforeInc = counter;
                    Interlocked.Increment(ref counter);
                    if (counterBeforeInc < 3)
                    {
                        RunningSession newSession = new RunningSession();
                        newSession.ConcurrencyFull = true;
                        return (false, null);
                    }
                    return (true, null);
                };

                eyes.visualGridEyes_.AfterServerConcurrencyLimitReachedQueried += (concurrecnyReached) =>
                {
                    if (concurrecnyReached)
                    {
                        wasConcurrencyFull = true;
                    }
                };

                eyes.Check(Target.Window().Fully());
                eyes.CloseAsync();
            }
            finally
            {
                eyes.AbortAsync();
                driver.Quit();
                runner.GetAllTestResults(false);
            }

            Assert.IsTrue(wasConcurrencyFull);
            Assert.IsFalse(eyes.visualGridEyes_.IsServerConcurrencyLimitReached());
            Assert.AreEqual(4, counter);
        }
        */
        private static MockEyesConnector OpenEyesAndGetConnector_(Eyes eyes, Configuration config, IWebDriver driver)
        {
            eyes.Open(driver, "Mock app", "Mock Test");

            MockEyesConnector mockEyesConnector = (MockEyesConnector)eyes.visualGridEyes_.eyesConnector_;
            //MockServerConnector mockServerConnector = new MockServerConnector(eyes.Logger, new Uri(eyes.ServerUrl));
            //EyesConnector eyesConnector = new EyesConnector(config.GetBrowsersInfo()[0], config)
            //{
            //    runningSession_ = new RunningSession(),
            //    ServerConnector = mockServerConnector
            //};
            //mockEyesConnector.WrappedConnector = eyesConnector;
            return mockEyesConnector;
        }
    }
}
