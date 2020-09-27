using System;
using System.Collections.Generic;
using System.Threading;
using Applitools.Selenium.Tests.Utils;
using Applitools.Tests.Utils;
using Applitools.Ufg;
using Applitools.VisualGrid;
using NUnit.Framework;
using OpenQA.Selenium;

namespace Applitools.Selenium.Tests.VisualGridTests
{
    [TestFixture]
    public class TestMultiThreadWithServiceLockService
    {
        private VisualGridRunner renderingManager;
        private IWebDriver webDriver;
        private readonly AutoResetEvent  openerThreadLock = new AutoResetEvent(false);
        private readonly AutoResetEvent checkerThreadLock = new AutoResetEvent(false);
        private readonly AutoResetEvent  closerThreadLock = new AutoResetEvent(false);
        private readonly AutoResetEvent  renderThreadLock = new AutoResetEvent(false);

        private readonly AutoResetEvent  openerTestLock = new AutoResetEvent(false);
        private readonly AutoResetEvent checkerTestLock = new AutoResetEvent(false);
        private readonly AutoResetEvent  closerTestLock = new AutoResetEvent(false);
        private readonly AutoResetEvent  renderTestLock = new AutoResetEvent(false);

        private readonly AutoResetEvent threadALock = new AutoResetEvent(false);
        private readonly AutoResetEvent threadBLock = new AutoResetEvent(false);
        private int concurrentOpenSessions;

        [SetUp]
        public void Before()
        {
            concurrentOpenSessions = 3;
            renderingManager = new VisualGridRunner(concurrentOpenSessions,
                openerThreadLock, checkerThreadLock, closerThreadLock, renderThreadLock,
                openerTestLock, checkerTestLock, closerTestLock, renderTestLock);

            ILogHandler logHandler = TestUtils.InitLogHandler(nameof(TestMultiThreadWithServiceLockService));
            renderingManager.SetLogHandler(logHandler);

            webDriver = SeleniumUtils.CreateChromeDriver();
            webDriver.Url = "https://applitools.github.io/demo/TestPages/VisualGridTestPage";
            //webDriver.get("http://applitools-vg-test.surge.sh/test.html");

            //        System.setProperty("https.protocols", "TLSv1,TLSv1.1,TLSv1.2");
        }

        //[Test]
        public void Test()
        {

            Thread threadA = new Thread(() =>
            {
                string baselineEnvName = "";
                TestThreadMethod_("VG-MultiThreadBatchC11", threadALock,
                        new DesktopBrowserInfo(800, 600, BrowserType.CHROME, baselineEnvName),
                        new DesktopBrowserInfo(700, 500, BrowserType.CHROME, baselineEnvName),
                        new DesktopBrowserInfo(400, 300, BrowserType.CHROME, baselineEnvName));
            });
            threadA.Name = "ThreadA";

            Thread threadB = new Thread(() =>
            {
                string baselineEnvName = "";
                TestThreadMethod_("VG-MultiThreadBatchC22", threadBLock,
                        new DesktopBrowserInfo(840, 680, BrowserType.CHROME, baselineEnvName),
                        new DesktopBrowserInfo(750, 530, BrowserType.CHROME, baselineEnvName));
            });
            threadB.Name = "ThreadB";

            //Start the thread and wait for all tasks to be created
            threadA.Start();
            threadALock.WaitOne();

            //Start the thread and wait for all tasks to be created
            threadB.Start();
            threadBLock.WaitOne();

            //Get all tasks
            IEnumerable<ICompletableTask> allOpenTasks = renderingManager.GetAllTasksByType(TaskType.Open);
            IEnumerable<ICompletableTask> allCheckTasks = renderingManager.GetAllTasksByType(TaskType.Check);
            IEnumerable<ICompletableTask> allCloseTasks = renderingManager.GetAllTasksByType(TaskType.Close);
            IEnumerable<ICompletableTask> allRenderingTasks = renderingManager.GetAllRenderingTasks();

            //Test that all tests are not opened yet.
            CheckAllTaskAreNotComplete_(allOpenTasks, "OPEN");

            //Test that all tests are not checked yet.
            CheckAllTaskAreNotComplete_(allCheckTasks, "CHECK");

            //Test that all tests are not closed yet.
            CheckAllTaskAreNotComplete_(allCloseTasks, "CLOSE");

            //Test that all tests are not rendered yet.
            CheckAllTaskAreNotComplete_(allRenderingTasks, "RENDER");

            //Start Rendering
            renderThreadLock.Set();
            renderTestLock.WaitOne(); // Wait for first render.

            renderThreadLock.Set();
            renderTestLock.WaitOne();  // Wait for second render.

            int openedTaskCount = StartServiceAndCountCompletedTasks_(allOpenTasks, openerThreadLock, openerTestLock);

            Assert.AreEqual(concurrentOpenSessions, openedTaskCount, "Completed opened tasks are not equal to concurrency");

            int closeTasksCount = StartServiceAndCountCompletedTasks_(allCloseTasks, closerThreadLock, closerTestLock);

            Assert.AreEqual(0, closeTasksCount, "Close tasks are completed before check tasks");

            int checkedTasksCount = StartServiceAndCountCompletedTasks_(allCheckTasks, checkerThreadLock, checkerTestLock);

            Assert.AreEqual(concurrentOpenSessions, checkedTasksCount, "Completed checked tasks are not equal to concurrency");

            PauseAllServices_();

            closeTasksCount = StartServiceAndCountCompletedTasks_(allCloseTasks, closerThreadLock, closerTestLock);

            Assert.AreEqual(concurrentOpenSessions, closeTasksCount);

            openedTaskCount = StartServiceAndCountCompletedTasks_(allOpenTasks, openerThreadLock, openerTestLock);

            Assert.AreEqual(5, openedTaskCount, "Completed opened tasks are not equal to concurrency");

            checkedTasksCount = StartServiceAndCountCompletedTasks_(allCheckTasks, checkerThreadLock, checkerTestLock);

            Assert.AreEqual(5, checkedTasksCount, "Completed checked tasks are not equal to concurrency");

            closeTasksCount = StartServiceAndCountCompletedTasks_(allCloseTasks, closerThreadLock, closerTestLock);

            Assert.AreEqual(5, closeTasksCount, "Close tasks are completed before check tasks");

        }

        private void PauseAllServices_()
        {
            renderingManager.PauseServices();
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
