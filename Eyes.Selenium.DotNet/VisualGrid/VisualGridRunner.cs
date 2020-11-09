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
        internal readonly HashSet<IVisualGridEyes> allEyes_ = new HashSet<IVisualGridEyes>();
        private readonly List<RenderingTask> renderingTaskList_ = new List<RenderingTask>();
        private readonly List<RenderRequestCollectionTask> renderRequestCollectionTaskList_ = new List<RenderRequestCollectionTask>();

        internal AutoResetEvent debugLock_ = null;

        private readonly AutoResetEvent openerServiceConcurrencyLock_ = new AutoResetEvent(true);
        private readonly AutoResetEvent openerServiceLock_ = new AutoResetEvent(true);
        private readonly AutoResetEvent closerServiceLock_ = new AutoResetEvent(true);
        private readonly AutoResetEvent checkerServiceLock_ = new AutoResetEvent(true);
        private readonly AutoResetEvent renderingServiceLock_ = new AutoResetEvent(true);
        private readonly AutoResetEvent renderRequestCollectionServiceLock_ = new AutoResetEvent(true);
        private readonly EyesListener eyesListener_;

        internal static TimeSpan waitForResultTimeout_ = TimeSpan.FromMinutes(10);

        public class RenderListener
        {
            public Action OnRenderSuccess { get; }
            public Action<Exception> OnRenderFailed { get; }

            public RenderListener()
            {
                OnRenderSuccess = () => { };
                OnRenderFailed = (e) => { };
            }

            public RenderListener(Action onRenderSuccess, Action<Exception> onRenderFailed)
            {
                OnRenderSuccess = onRenderSuccess;
                OnRenderFailed = onRenderFailed;
            }
        }

        private OpenerService eyesOpenerService_;
        private EyesService eyesCloserService_;
        private EyesService renderRequestCollectionService_;
        private RenderingGridService renderingGridService_;
        private EyesService eyesCheckerService_;
        private RenderingInfo renderingInfo_;
        private bool wasConcurrencyLogSent_;

        RenderingInfo IVisualGridRunner.RenderingInfo => renderingInfo_;

        ConcurrentDictionary<string, byte> IVisualGridRunner.CachedBlobsURLs { get; } = new ConcurrentDictionary<string, byte>();
        ConcurrentDictionary<string, HashSet<string>> IVisualGridRunner.CachedResourceMapping { get; } = new ConcurrentDictionary<string, HashSet<string>>();
        ConcurrentDictionary<string, ResourceFuture> IVisualGridRunner.CachedResources { get; } = new ConcurrentDictionary<string, ResourceFuture>();
        ConcurrentDictionary<string, byte> IVisualGridRunner.PutResourceCache { get; } = new ConcurrentDictionary<string, byte>();
        public IDebugResourceWriter DebugResourceWriter { get; set; }

        public VisualGridRunner(ILogHandler logHandler = null)
            : this(new RunnerOptions().TestConcurrency(DEFAULT_CONCURRENCY), logHandler)
        {
            testConcurrency_ = new TestConcurrency();
        }

        public VisualGridRunner(int concurrentOpenSessions, ILogHandler logHandler = null)
            : this(new RunnerOptions().TestConcurrency(concurrentOpenSessions * CONCURRENCY_FACTOR), logHandler)
        {
            testConcurrency_ = new TestConcurrency(concurrentOpenSessions, true);
        }

        public VisualGridRunner(RunnerOptions runnerOptions, ILogHandler logHandler = null)
        {
            runnerOptions_ = runnerOptions;
            testConcurrency_ = new TestConcurrency(((IRunnerOptionsInternal)runnerOptions).GetConcurrency(), false);
            if (logHandler != null) Logger.SetLogHandler(logHandler);
            eyesListener_ = new EyesListener(OnTaskComplete, OnRenderComplete);
            Init();
            Logger.Verbose("rendering grid manager is built");
            StartServices();
        }

        ~VisualGridRunner()
        {
            TaskScheduler.UnobservedTaskException -= TaskScheduler_UnobservedTaskException;
        }

        internal List<VisualGridTask> GetAllTasksByType(TaskType type)
        {
            List<VisualGridTask> allTasks = new List<VisualGridTask>();
            foreach (IVisualGridEyes eyes in AllEyes)
            {
                foreach (RunningTest runningTest in eyes.GetAllRunningTests())
                {
                    foreach (VisualGridTask visualGridTask in runningTest.TaskList)
                    {
                        if (visualGridTask.TaskType == type)
                        {
                            allTasks.Add(visualGridTask);
                        }
                    }
                }
            }
            return allTasks;
        }

        void OnTaskComplete(VisualGridTask task, IVisualGridEyes eyes)
        {
            Logger.Verbose("Enter with: {0}", task.TaskType);
            TaskType type = task.TaskType;
            try
            {
                switch (type)
                {
                    case TaskType.Open:
                        Logger.Verbose("locking eyesToOpenList");
                        lock (eyesToOpenList_)
                        {
                            Logger.Verbose("removing task {0}", task);
                            eyesToOpenList_.Remove(eyes);
                        }
                        Logger.Verbose("releasing eyesToOpenList");
                        break;
                    case TaskType.Abort:
                    case TaskType.Close:
                        Logger.Verbose("Task {0}", type);
                        eyesOpenerService_.DecrementConcurrency();
                        openerServiceConcurrencyLock_.Set();
                        break;
                    case TaskType.Check:
                        Logger.Verbose("Check complete.");
                        break;
                }
            }
            catch (Exception e)
            {
                Logger.Log("Error: " + e);
            }

            NotifyAllServices();
        }

        void OnRenderComplete()
        {
            NotifyAllServices();
        }

        private void StartServices()
        {
            Logger.Verbose("enter");
            eyesOpenerService_.Start();
            eyesCloserService_.Start();
            renderRequestCollectionService_.Start();
            renderingGridService_.Start();
            eyesCheckerService_.Start();
            IsServicesOn = true;
            Logger.Verbose("exit");
        }

        private void StopServices()
        {
            Logger.Verbose("enter");
            IsServicesOn = false;
            eyesOpenerService_.Stop();
            eyesCloserService_.Stop();
            renderRequestCollectionService_.Stop();
            renderingGridService_.Stop();
            eyesCheckerService_.Stop();
            Logger.Verbose("exit");
        }

        internal void NotifyAllServices()
        {
            NotifyOpenerService();
            NotifyCloserService();
            NotifyRenderRequestCollectionService();
            NotifyCheckerService();
            NotifyRenderingService();
        }

        private void NotifyRenderingService()
        {
            Logger.Verbose($"releasing {nameof(renderingServiceLock_)}.");
            renderingServiceLock_.Set();
        }

        private void NotifyCheckerService()
        {
            Logger.Verbose($"releasing {nameof(checkerServiceLock_)}.");
            checkerServiceLock_.Set();
        }

        private void NotifyCloserService()
        {
            Logger.Verbose($"releasing {nameof(closerServiceLock_)}.");
            closerServiceLock_.Set();
        }

        private void NotifyOpenerService()
        {
            Logger.Verbose($"releasing {nameof(openerServiceLock_)}.");
            openerServiceLock_.Set();
        }

        private void NotifyRenderRequestCollectionService()
        {
            Logger.Verbose($"releasing {nameof(renderRequestCollectionServiceLock_)}.");
            renderRequestCollectionServiceLock_.Set();
        }

        private void Init()
        {
            TaskScheduler.UnobservedTaskException += TaskScheduler_UnobservedTaskException;
            int concurrentOpenSessions = ((IRunnerOptionsInternal)runnerOptions_).GetConcurrency();
            eyesOpenerService_ = new OpenerService("eyesOpenerService", Logger, concurrentOpenSessions, openerServiceConcurrencyLock_,
                new EyesService.EyesServiceListener((tasker) => GetOrWaitForTask_(openerServiceLock_, tasker, "eyesOpenerService")),
                new EyesService.Tasker(() => GetNextTestToOpen_()));

            eyesCloserService_ = new EyesService("eyesCloserService", Logger, concurrentOpenSessions,
                new EyesService.EyesServiceListener((tasker) => GetOrWaitForTask_(closerServiceLock_, tasker, "eyesCloserService")),
                new EyesService.Tasker(() => GetNextTestToClose_()));

            renderRequestCollectionService_ = new EyesService("renderRequestCollectionService", Logger, concurrentOpenSessions,
                new EyesService.EyesServiceListener(
                    (tasker) => GetOrWaitForTask_(renderRequestCollectionServiceLock_, tasker, "renderRequestCollectionService")),
                new EyesService.Tasker(() => GetNextRenderRequestCollectionTask_()));

            renderingGridService_ = new RenderingGridService("renderingGridService", Logger, concurrentOpenSessions,
                new RenderingGridService.RGServiceListener(() =>
                {
                    RenderingTask nextTestToRender = GetNextRenderingTask_();
                    try
                    {
                        if (nextTestToRender == null)
                        {
                            Logger.Verbose("nextTestToRender is null. Waiting 300ms on {0}", nameof(renderingServiceLock_));
                            renderingServiceLock_.WaitOne(300);
                            nextTestToRender = GetNextRenderingTask_();
                        }
                    }
                    catch (Exception e)
                    {
                        Logger.Log("Error: " + e);
                    }
                    return nextTestToRender;
                }), renderingServiceLock_);

            eyesCheckerService_ = new EyesService("eyesCheckerService", Logger, concurrentOpenSessions,
                new EyesService.EyesServiceListener((tasker) => GetOrWaitForTask_(checkerServiceLock_, tasker, "eyesCheckerService")),
                new EyesService.Tasker(() => GetNextCheckTask_()));
        }

        private void TaskScheduler_UnobservedTaskException(object sender, UnobservedTaskExceptionEventArgs e)
        {
            Logger.Log("Error: " + e);
        }

        public void Close(IVisualGridEyes eyes)
        {
            NotifyAllServices();
        }

        private Task<TestResultContainer> GetNextCheckTask_()
        {
            ScoreTask bestScoreTask = null;
            int bestScore = -1;
            Logger.Verbose("looking for best test in a list of {0}", allEyes_.Count);

            foreach (IVisualGridEyes eyes in AllEyes)
            {
                Stopwatch sw = Stopwatch.StartNew();
                ScoreTask currentScoreTask = eyes.GetBestScoreTaskForCheck();
                Logger.Verbose("{0} - {1} took {2} seconds. Result null? {3}",
                    nameof(GetNextCheckTask_), nameof(eyes.GetBestScoreTaskForCheck), sw.Elapsed.TotalSeconds, currentScoreTask == null);
                if (currentScoreTask == null) continue;
                int currentTestMark = currentScoreTask.Score;
                if (bestScore < currentTestMark)
                {
                    bestScoreTask = currentScoreTask;
                    bestScore = currentTestMark;
                }
            }

            if (bestScoreTask == null)
            {
                Logger.Verbose("no test found.");
                return null;
            }
            Logger.Verbose("found test with score {0}", bestScore);
            VisualGridTask vgTask = bestScoreTask.Task;
            Task<TestResultContainer> task = new Task<TestResultContainer>(vgTask.Call, vgTask);
            Logger.Verbose("task id: {0}", task.Id);
            return task;
        }

        private Task<TestResultContainer> GetNextTestToOpen_()
        {
            ScoreTask bestScoreTask = null;
            int bestMark = -1;
            Logger.Verbose("GetNextTestToOpen_ - looking for best test in a list of {0} eyes.", allEyes_.Count);
            var allEyes = AllEyes;
            if (allEyes.Any((eyes) => eyes.IsServerConcurrencyLimitReached())) return null;

            foreach (IVisualGridEyes eyes in allEyes)
            {
                ScoreTask currentTestMark = eyes.GetBestScoreTaskForOpen();
                if (currentTestMark == null) continue;
                int currentScore = currentTestMark.Score;
                if (bestMark < currentScore)
                {
                    bestMark = currentScore;
                    bestScoreTask = currentTestMark;
                }
            }

            if (bestScoreTask == null)
            {
                Logger.Verbose("GetNextTestToOpen_ - no relevant test found in: {0}", allEyes.Concat(" ; "));
                return null;
            }

            Logger.Verbose("found test with mark {0}", bestMark);
            Logger.Verbose("calling getNextOpenTaskAndRemove on {0}", bestScoreTask);
            VisualGridTask nextOpenTask = bestScoreTask.Task;
            Task<TestResultContainer> task = new Task<TestResultContainer>(nextOpenTask.Call, nextOpenTask);
            Logger.Verbose("task id: {0}", task.Id);
            return task;
        }
        private RenderingTask GetNextRenderingTask_()
        {
            Logger.Verbose("GetNextRenderingTask_ - enter - renderingTaskList_.Count: {0}", renderingTaskList_.Count);
            lock (renderingTaskList_)
            {
                if (renderingTaskList_.Count == 0)
                {
                    return null;
                }

                RenderingTask finalRenderingTask = null;
                List<RenderingTask> chosenTasks = new List<RenderingTask>();
                foreach (RenderingTask renderingTask in renderingTaskList_)
                {
                    if (!renderingTask.IsReady)
                    {
                        continue;
                    }

                    if (finalRenderingTask == null)
                    {
                        finalRenderingTask = renderingTask;
                    }
                    else
                    {
                        finalRenderingTask.Merge(renderingTask);
                    }

                    chosenTasks.Add(renderingTask);
                }

                finalRenderingTask = finalRenderingTask != null && finalRenderingTask.IsReady ? finalRenderingTask : null;

                if (finalRenderingTask != null)
                {
                    Logger.Verbose("GetNextRenderingTask_ - Next rendering task contains {0} render requests", chosenTasks.Count);
                    foreach (var task in chosenTasks)
                    {
                        renderingTaskList_.Remove(task);
                    }
                }

                return finalRenderingTask;
            }
        }

        private Task<TestResultContainer> GetNextRenderRequestCollectionTask_()
        {
            Logger.Verbose("locking renderRequestCollectionTaskList_. Count: {0}", renderRequestCollectionTaskList_.Count);
            lock (renderRequestCollectionTaskList_)
            {
                if (renderRequestCollectionTaskList_.Count == 0)
                {
                    Logger.Verbose("releasing renderRequestCollectionTaskList_. returning null.");
                    return null;
                }

                RenderRequestCollectionTask renderRequestCollectionTask = renderRequestCollectionTaskList_[0];
                renderRequestCollectionTaskList_.RemoveAt(0);
                Task<TestResultContainer> task = new Task<TestResultContainer>(renderRequestCollectionTask.Call);
                Logger.Verbose("releasing renderRequestCollectionTaskList_. returning task.");
                return task;
            }
        }

        private Task<TestResultContainer> GetNextTestToClose_()
        {
            RunningTest runningTest;
            foreach (IVisualGridEyes eyes in AllEyes)
            {
                runningTest = eyes.GetNextTestToClose();
                if (runningTest != null)
                {
                    return runningTest.GetNextCloseTask();
                }
            }
            return null;
        }

        private Task<TestResultContainer> GetOrWaitForTask_(AutoResetEvent lockObject, EyesService.Tasker tasker, string serviceName)
        {
            Task<TestResultContainer> nextTestToOpen = null;
            try
            {
                nextTestToOpen = tasker.GetNextTask();
            }
            catch (Exception e)
            {
                Logger.Log("Error: " + e);
            }
            if (nextTestToOpen == null)
            {
                try
                {
                    Logger.Verbose("VisualGridRunner.GetOrWaitForTask_ - {0} waiting on lockObject #{1}", serviceName, lockObject.GetHashCode());
                    lockObject.WaitOne(500);
                    nextTestToOpen = tasker.GetNextTask();
                }
                catch (Exception e)
                {
                    Logger.Log("Error: " + e);
                }
            }
            return nextTestToOpen;
        }

        protected override TestResultsSummary GetAllTestResultsImpl(bool shouldThrowException)
        {
            Logger.Verbose("enter");
            Dictionary<IVisualGridEyes, ICollection<Task<TestResultContainer>>> allFutures = new Dictionary<IVisualGridEyes, ICollection<Task<TestResultContainer>>>();

            foreach (IVisualGridEyes eyes in AllEyes)
            {
                ICollection<Task<TestResultContainer>> futureList = eyes.Close();
                Logger.Verbose("adding a {0} items list to allFutures.", futureList.Count);
                if (allFutures.TryGetValue(eyes, out ICollection<Task<TestResultContainer>> futures) && futures != null)
                {
                    foreach (Task<TestResultContainer> future in futures)
                    {
                        futureList.Add(future);
                    }
                }
                allFutures.Add(eyes, futureList);
            }

            Exception exception = null;
            NotifyAllServices();
            List<TestResultContainer> allResults = new List<TestResultContainer>();
            Logger.Verbose("trying to call future.get on {0} future lists.", allFutures.Count);
            foreach (KeyValuePair<IVisualGridEyes, ICollection<Task<TestResultContainer>>> entry in allFutures)
            {
                ICollection<Task<TestResultContainer>> value = entry.Value;
                IVisualGridEyes key = entry.Key;
                key.TestResults.Clear();
                Logger.Verbose("trying to call future.get on {0} futures of {1}", value.Count, key);
                foreach (Task<TestResultContainer> future in value)
                {
                    Logger.Verbose("calling future.get on {0}", key);
                    TestResultContainer testResultContainer = null;
                    try
                    {
                        if (Task.WhenAny(future, Task.Delay(waitForResultTimeout_)).Result == future)
                        {
                            testResultContainer = future.Result;
                            if (testResultContainer.Exception != null && exception == null)
                            {
                                exception = testResultContainer.Exception;
                            }
                            allResults.Add(testResultContainer);
                            key.TestResults.Add(testResultContainer);
                        }
                    }
                    catch (Exception e)
                    {
                        Logger.Log("Error: " + e);
                        if (exception == null)
                        {
                            exception = e;
                        }
                    }
                    Logger.Verbose("got TestResultContainer: {0}", testResultContainer);
                }
            }

            //lock (lockObject_) allEyes_.Clear();
            //eyesToOpenList_.Clear();
            //renderingTaskList_.Clear();

            StopServices();
            NotifyAllServices();

            if (shouldThrowException && exception != null)
            {
                Logger.Log("Error: " + exception);
                throw exception;
            }
            Logger.Verbose("exit");
            TaskScheduler.UnobservedTaskException -= TaskScheduler_UnobservedTaskException;
            return new TestResultsSummary(allResults);
        }

        public void Open(IVisualGridEyes eyes, RenderingInfo renderingInfo)
        {
            Logger.Verbose("enter");
            if (renderingInfo_ == null)
            {
                renderingInfo_ = renderingInfo;
            }

            Logger.Verbose("locking eyesToOpenList_");
            lock (eyesToOpenList_)
            {
                eyesToOpenList_.Add(eyes);
            }
            Logger.Verbose("releasing eyesToOpenList_");

            if (allEyes_.Add(eyes))
            {
                if (allEyes_.Count == 1 && Logger.GetILogHandler() is NullLogHandler)
                {
                    ILogHandler handler = eyes.Logger.GetILogHandler();
                    if (handler != null)
                    {
                        Logger.SetLogHandler(handler);
                    }
                }
            }
            else
            {
                Logger.Verbose("eyes already in list.");
            }
            eyes.SetListener(eyesListener_);

            AddBatch(eyes.Batch.Id, eyes.GetBatchCloser());
            Logger.Log("exit");
        }

        public void Check(ICheckSettings settings, IDebugResourceWriter debugResourceWriter, FrameData domData,
                          IList<VisualGridSelector[]> regionSelectors, IUfgConnector connector, UserAgent userAgent,
                          List<VisualGridTask> checkTasks, RenderListener listener)
        {
            debugResourceWriter = debugResourceWriter ?? DebugResourceWriter ?? NullDebugResourceWriter.Instance;
            Logger.Verbose("enter");
            Logger.Verbose("connector type: {0}", connector.GetType().Name);
            RenderRequestCollectionTask resourceCollectionTask = new RenderRequestCollectionTask(this, domData, connector,
                userAgent, regionSelectors, settings, checkTasks, (Ufg.IDebugResourceWriter)debugResourceWriter,
                new RenderingTask.TaskListener<List<RenderingTask>>(
                    (renderingTasks) =>
                    {
                        Logger.Verbose("locking renderingTaskList_");
                        lock (renderingTaskList_)
                        {
                            renderingTaskList_.AddRange(renderingTasks);
                        }
                        Logger.Verbose("releasing renderingTaskList_");
                        NotifyAllServices();
                        debugLock_?.Set();
                    },
                    (e) =>
                    {
                        NotifyAllServices();
                    }
                    ),
                new RenderingTask.RenderTaskListener(
                    () =>
                    {
                        listener.OnRenderSuccess();
                        NotifyAllServices();
                    },
                    (e) =>
                    {
                        NotifyAllServices();
                        listener.OnRenderFailed(e);
                    })
            );

            Logger.Verbose("locking resourceCollectionTaskList_");
            lock (renderRequestCollectionTaskList_)
            {
                renderRequestCollectionTaskList_.Add(resourceCollectionTask);
            }
            Logger.Verbose("releasing resourceCollectionTaskList_");

            NotifyAllServices();
            //Logger.Verbose("exit");
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
        internal IEnumerable<IVisualGridEyes> AllEyes { get { lock (LockObject) return allEyes_.ToArray(); } }

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
