using Applitools.Ufg.Model;
using Applitools.Utils;
using Applitools.Ufg;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using System.Diagnostics;
using Applitools.Selenium;
using Newtonsoft.Json;

namespace Applitools.VisualGrid
{
    public class VisualGridRunner : EyesRunner, IVisualGridRunner
    {
        internal const int CONCURRENCY_FACTOR = 5;
        internal const int DEFAULT_CONCURRENCY = 5;
        internal readonly RunnerOptions runnerOptions_;

        private object lockObject_ = new object();
        public object LockObject
        {
            get
            {
                StackFrame frame = new StackFrame(1);
                Logger.Verbose("Lock #{0} acquired by: {1}", lockObject_.GetHashCode(), frame.GetMethod().Name);
                return lockObject_;
            }
        }

        internal class TestConcurrency
        {
            public int UserConcurrency { get; }
            public int ActualConcurrency { get; }
            public bool IsLegacy { get; }
            public bool IsDefault { get; }

            public TestConcurrency()
            {
                IsDefault = true;
                IsLegacy = false;
                UserConcurrency = DEFAULT_CONCURRENCY;
                ActualConcurrency = DEFAULT_CONCURRENCY;
            }

            public TestConcurrency(int userConcurrency, bool isLegacy)
            {
                UserConcurrency = userConcurrency;
                ActualConcurrency = isLegacy ? userConcurrency * CONCURRENCY_FACTOR : userConcurrency;
                IsLegacy = isLegacy;
                IsDefault = false;
            }
        }

        internal readonly TestConcurrency testConcurrency_;

        private readonly List<IVisualGridEyes> eyesToOpenList_ = new List<IVisualGridEyes>(200);
        internal readonly HashSet<IEyes> allEyes_ = new HashSet<IEyes>();
        private EyesServiceRunner eyesServiceRunner_;

        internal AutoResetEvent debugLock_ = null;

        internal static TimeSpan waitForResultTimeout_ = TimeSpan.FromMinutes(10);

        private RenderingInfo renderingInfo_;
        private bool wasConcurrencyLogSent_;
        private string suiteName_;

        RenderingInfo IVisualGridRunner.RenderingInfo => renderingInfo_;

        ConcurrentDictionary<string, byte> IVisualGridRunner.CachedBlobsURLs { get; } = new ConcurrentDictionary<string, byte>();
        ConcurrentDictionary<string, HashSet<string>> IVisualGridRunner.CachedResourceMapping { get; } = new ConcurrentDictionary<string, HashSet<string>>();
        ConcurrentDictionary<string, ResourceFuture> IVisualGridRunner.CachedResources { get; } = new ConcurrentDictionary<string, ResourceFuture>();
        ConcurrentDictionary<string, byte> IVisualGridRunner.PutResourceCache { get; } = new ConcurrentDictionary<string, byte>();
        ConcurrentDictionary<string, RGridResource> IVisualGridRunner.ResourcesCacheMap { get; } = new ConcurrentDictionary<string, RGridResource>();
        public IDebugResourceWriter DebugResourceWriter { get; set; }

        public VisualGridRunner(string suiteName = null, ILogHandler logHandler = null)
            : this(new RunnerOptions().TestConcurrency(DEFAULT_CONCURRENCY), suiteName, logHandler)
        {
            testConcurrency_ = new TestConcurrency();
        }

        public VisualGridRunner(int concurrentOpenSessions, string suiteName = null, ILogHandler logHandler = null)
            : this(new RunnerOptions().TestConcurrency(concurrentOpenSessions * CONCURRENCY_FACTOR), suiteName, logHandler)
        {
            testConcurrency_ = new TestConcurrency(concurrentOpenSessions, true);
        }

        public VisualGridRunner(RunnerOptions runnerOptions, string suiteName = null, ILogHandler logHandler = null)
        {
            runnerOptions_ = runnerOptions;
            testConcurrency_ = new TestConcurrency(((IRunnerOptionsInternal)runnerOptions).GetConcurrency(), false);
            if (logHandler != null) Logger.SetLogHandler(logHandler);
            Init(suiteName);
        }

        ~VisualGridRunner()
        {
            TaskScheduler.UnobservedTaskException -= TaskScheduler_UnobservedTaskException;
        }
        private void TaskScheduler_UnobservedTaskException(object sender, UnobservedTaskExceptionEventArgs e)
        {
            Logger.Log("Error: {0}", e);
        }

        private void Init(string suiteName)
        {
            TaskScheduler.UnobservedTaskException += TaskScheduler_UnobservedTaskException;
            int concurrentOpenSessions = ((IRunnerOptionsInternal)runnerOptions_).GetConcurrency();

            suiteName_ = suiteName;
            if (suiteName_ == null)
            {
                StackFrame frame = new StackFrame(2);
                suiteName_ = frame.GetMethod().DeclaringType.Name;
            }

            Logger.Log("runner created");
            IDictionary<string, RGridResource> resourcesCacheMap = ((IVisualGridRunner)this).ResourcesCacheMap;
            Ufg.IDebugResourceWriter drw = EyesSeleniumUtils.ConvertDebugResourceWriter(DebugResourceWriter);

            eyesServiceRunner_ = new EyesServiceRunner(Logger, ServerConnector, allEyes_, concurrentOpenSessions,
                drw, resourcesCacheMap);
            eyesServiceRunner_.Start();

            Logger.Verbose("rendering grid manager is built");
        }

        public void Open(IEyes eyes, IList<VisualGridRunningTest> newTests)
        {
            Logger.Verbose("enter");

            ApiKey = eyes.ApiKey;
            ServerUrl = eyes.ServerUrl;
            ServerConnector.Proxy = eyes.Proxy;

            if (renderingInfo_ == null)
            {
                renderingInfo_ = ServerConnector.GetRenderingInfo();
            }

            eyesServiceRunner_.SetRenderingInfo(renderingInfo_);

            allEyes_.Add(eyes);

            try
            {
                object logMessage = GetConcurrencyLog();
                if (logMessage != null)
                {
                    NetworkLogHandler.SendEvent(ServerConnector, TraceLevel.Notice, logMessage);
                }
            }
            catch (JsonException e)
            {
                Logger.Log("Error: {0}", e);
            }

            AddBatch(eyes.Batch.Id, (IBatchCloser)eyes);
            eyesServiceRunner_.OpenTests(newTests);
        }

        public void Check(FrameData domData, List<CheckTask> checkTasks)
        {
            eyesServiceRunner_.AddResourceCollectionTask(domData, checkTasks);
        }

        protected override TestResultsSummary GetAllTestResultsImpl(bool throwException)
        {
            bool isRunning = true;
            while (isRunning && eyesServiceRunner_.Error == null)
            {
                isRunning = false;
                foreach (IEyes eyes in AllEyes)
                {
                    isRunning = isRunning || !eyes.IsCompleted();
                }
                Thread.Sleep(500);
            }

            if (eyesServiceRunner_.Error != null)
            {
                throw new EyesException("Execution crashed", eyesServiceRunner_.Error);
            }

            eyesServiceRunner_.StopServices();

            Exception exception = null;
            List<TestResultContainer> allResults = new List<TestResultContainer>();
            foreach (IEyes eyes in AllEyes)
            {
                IList<TestResultContainer> eyesResults = eyes.GetAllTestResults();
                foreach (TestResultContainer result in eyesResults)
                {
                    if (exception == null && result.Exception != null)
                    {
                        exception = result.Exception;
                    }
                }

                allResults.AddRange(eyesResults);
            }

            if (throwException && exception != null)
            {
                throw new Exception("Error", exception);
            }
            TaskScheduler.UnobservedTaskException -= TaskScheduler_UnobservedTaskException;
            return new TestResultsSummary(allResults);
        }

        internal string PrintAllEyesFutures()
        {
            StringBuilder sb = new StringBuilder();
            foreach (IVisualGridEyes eyes in AllEyes)
            {
                sb.Append(eyes.PrintAllFutures());
            }
            return sb.ToString();
        }

        public bool IsServicesOn { get; set; }
        internal IEnumerable<IEyes> AllEyes { get { lock (LockObject) return allEyes_.ToArray(); } }

        public object GetConcurrencyLog()
        {
            if (wasConcurrencyLogSent_)
            {
                return null;
            }

            wasConcurrencyLogSent_ = true;
            string key = testConcurrency_.IsDefault ? "defaultConcurrency" : testConcurrency_.IsLegacy ? "concurrency" : "testConcurrency";
            return new Dictionary<string, object> {
                { "type", "runnerStarted" },
                { key, testConcurrency_.UserConcurrency }
            };
        }
    }
}
