using Applitools.Selenium.Tests.Utils;
using Applitools.Tests.Utils;
using Applitools.Ufg;
using Applitools.VisualGrid;
using NUnit.Framework;
using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Threading;

namespace Applitools.Selenium.Tests.VisualGridTests
{
    [TestFixture]
    public class TestMultiThreadWithServiceLockService
    {
        private VisualGridRunner renderingManager;
        private IWebDriver webDriver;

        private readonly AutoResetEvent threadALock = new AutoResetEvent(false);
        private readonly AutoResetEvent threadBLock = new AutoResetEvent(false);
        private int concurrentOpenSessions;

        [SetUp]
        public void Before()
        {
            concurrentOpenSessions = 3;
            renderingManager = new VisualGridRunner(concurrentOpenSessions);

            ILogHandler logHandler = TestUtils.InitLogHandler(nameof(TestMultiThreadWithServiceLockService));
            renderingManager.SetLogHandler(logHandler);

            webDriver = SeleniumUtils.CreateChromeDriver();
            webDriver.Url = "https://applitools.github.io/demo/TestPages/VisualGridTestPage";
            //webDriver.get("http://applitools-vg-test.surge.sh/test.html");

            //        System.setProperty("https.protocols", "TLSv1,TLSv1.1,TLSv1.2");
        }

        private int StartServiceAndCountCompletedTasks_(IEnumerable<ICompletableTask> allOpenTasks, AutoResetEvent threadDebugLock, AutoResetEvent testDebugLock)
        {
            //Start Opener and wait for 5 open tasks
            for (int i = 0; i < 5; ++i)
            {
                threadDebugLock.Set();
                testDebugLock.WaitOne(1000);
            }

            int completedTasks = 0;
            foreach (ICompletableTask openTask in allOpenTasks)
            {
                if (openTask.IsTaskComplete) completedTasks++;
            }

            return completedTasks;
        }

        private void CheckAllTaskAreNotComplete_(IEnumerable<ICompletableTask> allOpenTasks, string type)
        {
            foreach (ICompletableTask task in allOpenTasks)
            {
                Assert.IsFalse(task.IsTaskComplete, type + " Task is complete without notify openerService");
            }
        }

        private void TestThreadMethod_(string batchName, AutoResetEvent lockObject, params DesktopBrowserInfo[] browsersInfo)
        {
            try
            {
                Eyes eyes = new Eyes(renderingManager);
                eyes.Batch = new BatchInfo(batchName);
                Configuration renderingConfiguration = new Configuration();
                renderingConfiguration.SetTestName("Open Concurrency with Batch 3");
                renderingConfiguration.SetAppName("RenderingGridIntegration");
                renderingConfiguration.AddBrowsers(browsersInfo);
                //eyes.Proxy = new System.Net.WebProxy("http://127.0.0.1", 8888);
                eyes.SetConfiguration(renderingConfiguration);
                eyes.Open(webDriver);
                eyes.Check(Target.Window().WithName("test").SendDom(false));
                TestResults close = eyes.Close();
                lockObject.Set();
                Assert.NotNull(close);
            }
            catch (Exception e)
            {
                renderingManager.Logger.Log("Error: " + e);
            }
        }

        [TearDown]
        public void After()
        {
            TestResultsSummary resultSummary = renderingManager.GetAllTestResults();
            renderingManager.Logger.Log(resultSummary.ToString());
            webDriver.Quit();
        }
    }
}
