using Applitools.Ufg.Model;
using Applitools.Utils;
using Applitools.Ufg;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Applitools.VisualGrid
{
    public class VisualGridRunner : EyesRunner, IVisualGridRunner
    {
        internal const int FACTOR = 5;
        internal readonly RunnerOptions runnerOptions_;

        private readonly List<IVisualGridEyes> eyesToOpenList_ = new List<IVisualGridEyes>(200);
        internal readonly HashSet<IVisualGridEyes> allEyes_ = new HashSet<IVisualGridEyes>();
        private readonly List<RenderingTask> renderingTaskList_ = new List<RenderingTask>();
        private readonly List<RenderRequestCollectionTask> renderRequestCollectionTaskList_ = new List<RenderRequestCollectionTask>();

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

        RenderingInfo IVisualGridRunner.RenderingInfo => renderingInfo_;

        ConcurrentDictionary<string, byte> IVisualGridRunner.CachedBlobsURLs { get; } = new ConcurrentDictionary<string, byte>();
        ConcurrentDictionary<string, IEnumerable<string>> IVisualGridRunner.CachedResourceMapping { get; } = new ConcurrentDictionary<string, IEnumerable<string>>();
        ConcurrentDictionary<string, ResourceFuture> IVisualGridRunner.CachedResources { get; } = new ConcurrentDictionary<string, ResourceFuture>();
        ConcurrentDictionary<string, byte> IVisualGridRunner.PutResourceCache { get; } = new ConcurrentDictionary<string, byte>();
        public IDebugResourceWriter DebugResourceWriter { get; set; }

        public VisualGridRunner(ILogHandler logHandler = null)
            : this(new RunnerOptions().TestConcurrency(FACTOR), logHandler)
        {
        }

        public VisualGridRunner(int concurrentOpenSessions, ILogHandler logHandler = null)
            : this(new RunnerOptions().TestConcurrency(concurrentOpenSessions * FACTOR), logHandler)
        {
        }

        public VisualGridRunner(RunnerOptions runnerOptions, ILogHandler logHandler = null)
        {
            runnerOptions_ = runnerOptions;

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
            lock (allEyes_)
            {
                foreach (IVisualGridEyes eyes in allEyes_)
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
                            Logger.Verbose("nextTestToRender is null. Waiting 300ms on lock object.");
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
            lock (allEyes_)
            {
                Logger.Verbose("looking for best test in a list of {0}", allEyes_.Count);
                //Logger.Verbose("locking allEyes_");

                foreach (IVisualGridEyes eyes in allEyes_)
                {
                    ScoreTask currentScoreTask = eyes.GetBestScoreTaskForCheck();
                    if (currentScoreTask == null) continue;
                    int currentTestMark = currentScoreTask.Score;
                    if (bestScore < currentTestMark)
                    {
                        bestScoreTask = currentScoreTask;
                        bestScore = currentTestMark;
                    }
                }
            }
            //Logger.Verbose("releasing allEyes_");

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
            lock (allEyes_)
            {
                Logger.Verbose("looking for best test in a list of {0} eyes.", allEyes_.Count);
                foreach (IVisualGridEyes eyes in allEyes_)
                {
                    if (eyes.IsServerConcurrencyLimitReached())
                    {
                        return null;
                    }
                    ScoreTask currentTestMark = eyes.GetBestScoreTaskForOpen();
                    if (currentTestMark == null) continue;
                    int currentScore = currentTestMark.Score;
                    if (bestMark < currentScore)
                    {
                        bestMark = currentScore;
                        bestScoreTask = currentTestMark;
                    }
                }
            }

            if (bestScoreTask == null)
            {
                lock (allEyes_)
                {
                    Logger.Verbose("no relevant test found in: {0}", allEyes_.Concat(" ; "));
                }
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
            Logger.Verbose("enter - renderingTaskList_.Count: {0}", renderingTaskList_.Count);
            RenderingTask renderingTask = null;
            if (renderingTaskList_.Count > 0)
            {
                Logger.Verbose("locking renderingTaskList_");
                lock (renderingTaskList_)
                {
                    if (renderingTaskList_.Count > 0)
                    {
                        renderingTask = renderingTaskList_[0];
                        if (!renderingTask.IsReady) return null;
                        renderingTaskList_.RemoveAt(0);
                        Logger.Verbose("rendering task: {0}", renderingTask);
                    }
                }
                Logger.Verbose("releasing renderingTaskList_");
            }
            Logger.Verbose("exit");
            return renderingTask;
        }

        private Task<TestResultContainer> GetNextRenderRequestCollectionTask_()
        {
            lock (renderRequestCollectionTaskList_)
            {
                if (renderRequestCollectionTaskList_.Count == 0)
                {
                    return null;
                }

                RenderRequestCollectionTask renderRequestCollectionTask = renderRequestCollectionTaskList_[0];
                renderRequestCollectionTaskList_.RemoveAt(0);
                Task<TestResultContainer> task = new Task<TestResultContainer>(renderRequestCollectionTask.Call);
                return task;
            }
        }

        private Task<TestResultContainer> GetNextTestToClose_()
        {
            RunningTest runningTest;
            Logger.Verbose("locking allEyes_. Count: {0}", allEyes_.Count);
            lock (allEyes_)
            {
                foreach (IVisualGridEyes eyes in allEyes_)
                {
                    runningTest = eyes.GetNextTestToClose();
                    if (runningTest != null)
                    {
                        return runningTest.GetNextCloseTask();
                    }
                }
            }
            Logger.Verbose("releasing allEyes_");
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
                    Logger.Verbose("waiting on lockObject #{0}", lockObject.GetHashCode());
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

            List<IVisualGridEyes> allEyes;
            lock (allEyes_)
            {
                allEyes = new List<IVisualGridEyes>(allEyes_);
            }

            foreach (IVisualGridEyes eyes in allEyes)
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

            //lock (allEyes_) allEyes_.Clear();
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

            Logger.Verbose("locking allEyes_");
            lock (allEyes_)
            {
                if (!allEyes_.Contains(eyes))
                {
                    if (allEyes_.Count == 0 && Logger.GetILogHandler() is NullLogHandler)
                    {
                        ILogHandler handler = eyes.Logger.GetILogHandler();
                        if (handler != null)
                        {
                            Logger.SetLogHandler(handler);
                        }
                    }
                    allEyes_.Add(eyes);
                }
                else
                {
                    Logger.Verbose("eyes already in list.");
                }
            }
            Logger.Verbose("releasing allEyes");
            eyes.SetListener(eyesListener_);

            AddBatch(eyes.Batch.Id, eyes.GetBatchCloser());
            Logger.Log("exit");
        }

        public void Check(ICheckSettings settings, IDebugResourceWriter debugResourceWriter, FrameData domData,
                          IList<VisualGridSelector[]> regionSelectors, IUfgConnector connector, UserAgent userAgent,
                          List<VisualGridTask> checkTasks, RenderListener listener)
        {
            debugResourceWriter = debugResourceWriter ?? DebugResourceWriter ?? NullDebugResourceWriter.Instance;

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
            lock (allEyes_)
            {
                foreach (IVisualGridEyes eyes in allEyes_)
                {
                    sb.Append(eyes.PrintAllFutures());
                }
            }
            return sb.ToString();
        }

        public bool IsServicesOn { get; set; }
    }
}
