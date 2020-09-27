using Applitools.Utils;
using Applitools.VisualGrid;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Applitools.Selenium.Tests
{
    public class MultithreadedVisualGridTest1
    {
        private VisualGridRunner renderingManager_;
        private Logger logger_;
        private readonly BatchInfo batchInfo_;

        public static string[] MethodArgs = new string[] {
            "https://google.com",
            "https://facebook.com",
            "https://youtube.com",
            "https://amazon.com",
            "https://twitter.com",

            "https://ebay.com",
            "https://wikipedia.org",
            "https://instagram.com",
            "https://www.target.com/c/blankets-throws/-/N-d6wsb?lnk=ThrowsBlankets%E2%80%9C,tc",
            "https://www.usatoday.com",

            //"https://www.vans.com",
            "https://docs.microsoft.com/en-us/",
            "https://applitools.com/features/frontend-development",
            "https://applitools.com/docs/topics/overview.html",
            "https://www.qq.com/",
        };
        private int runningTasks_;

        public static void Main()
        {
            MultithreadedVisualGridTest1 program = new MultithreadedVisualGridTest1();
            program.Run();
        }

        public MultithreadedVisualGridTest1()
        {
            ILogHandler logHandler = TestUtils.InitLogHandler();
            renderingManager_ = new VisualGridRunner(40, logHandler);
            //renderingManager_.SetLogHandler(new StdoutLogHandler(true));
            //renderingManager_.DebugResourceWriter = new FileDebugResourceWriter(logPath);

            logger_ = renderingManager_.Logger;
            logger_.Log("enter");
            //renderingManager_.ServerUrl = SERVER_URL;
            //renderingManager_.ApiKey = API_KEY;
            batchInfo_ = new BatchInfo("Top Sites - Visual Grid");
        }

        private void Run()
        {
            Queue<Task> tasksQueue = new Queue<Task>();
            List<Task> tasks = new List<Task>();
            foreach (string state in MethodArgs)
            {
                Task t = new Task(() => Test(state));
                tasksQueue.Enqueue(t);
                tasks.Add(t);
            }

            StartTasks(tasksQueue, 5);

            Task.WhenAll(tasks).ContinueWith((T) => AfterClass()).Wait();
        }

        AutoResetEvent waithandle_ = new AutoResetEvent(false);
        private void StartTasks(Queue<Task> tasks, int maxTasks)
        {
            while (tasks.Count > 0)
            {
                Task task = tasks.Dequeue();
                if (Interlocked.Increment(ref runningTasks_) > maxTasks)
                {
                    logger_.Log("waiting for other tasks to finish. runningTasks: {0}", runningTasks_);
                    waithandle_.WaitOne();
                }
                task.Start();
            }
        }

        public void Test(string testedUrl)
        {
            ChromeOptions options = new ChromeOptions();
            IWebDriver webDriver = new ChromeDriver(options);
            Logger logger = null;
            Eyes eyes = null;
            try
            {
                webDriver.Url = testedUrl;
                eyes = InitEyes(webDriver, testedUrl);
                logger = eyes.Logger;
                logger.Log("navigated to " + testedUrl);

                logger.Log("running check for url " + testedUrl);
                ICheckSettings windowCheckSettings = Target.Window();
                eyes.Check(windowCheckSettings.WithName("Step1 - " + testedUrl));
                eyes.Check(windowCheckSettings.Fully(false).WithName("Step2 - " + testedUrl));
            }
            catch (Exception e)
            {
                logger?.Log(e.ToString());
            }
            finally
            {
                webDriver.Quit();
            }

            Interlocked.Decrement(ref runningTasks_);
            logger_.Log("closed a browser. releasing waithandle. runningTasks: {0}", runningTasks_);
            waithandle_.Set();

            logger_.Verbose("calling eyes_.Close() (test: {0})", testedUrl);
            TestResults results = eyes?.Close();
            logger_.Verbose("exit");
        }

        public void AfterClass()
        {
            logger_.Verbose("calling renderingManager_.GetAllTestResults()");
            TestResultsSummary allTestResults = renderingManager_.GetAllTestResults();
            logger_.Log(allTestResults.ToString());
        }

        protected Eyes InitEyes(IWebDriver webDriver, string testedUrl)
        {
            Eyes eyes = new Eyes(renderingManager_);
            eyes.Batch = batchInfo_;
            Logger logger = eyes.Logger;
            logger.Log("creating WebDriver: " + testedUrl);
            Configuration renderingConfiguration = new Configuration();
            renderingConfiguration.SetTestName("Top Sites - " + testedUrl);
            renderingConfiguration.SetAppName("Top Sites");
            string environment = "";
            renderingConfiguration.AddBrowser(800, 600, BrowserType.CHROME, environment);
            renderingConfiguration.AddBrowser(700, 500, BrowserType.FIREFOX, environment);
            renderingConfiguration.AddBrowser(1200, 800, BrowserType.IE_11, environment);
            renderingConfiguration.AddBrowser(1600, 1200, BrowserType.EDGE, environment);
            logger.Log("created configurations for url " + testedUrl);
            eyes.SetConfiguration(renderingConfiguration);
            eyes.Open(webDriver);
            return eyes;
        }
    }
}
