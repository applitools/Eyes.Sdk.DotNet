using Applitools.Utils;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Applitools.VisualGrid
{
    public class RunningTest
    {
        private readonly IEyesConnector eyes_;

#pragma warning disable IDE0052 // Remove unread private members
#pragma warning disable CS0414 // Filed is assigned but its value is never used
        private bool isTestInExceptionMode_ = false;
        private Exception exception_;
#pragma warning restore CS0414 // Filed is assigned but its value is never used
#pragma warning restore IDE0052 // Remove unread private members

        private Dictionary<VisualGridTask, Task<TestResultContainer>> taskToFutureMapping_ = new Dictionary<VisualGridTask, Task<TestResultContainer>>();
        private readonly RunningTestListener listener_;
        private readonly VisualGridTask.TaskListener taskListener_;

        internal bool isCloseTaskIssued_;
        internal VisualGridTask closeTask_;

        public string TestName { get; internal set; }

        internal VisualGridTask openTask_;

        public Task<TestResultContainer> Abort(Exception exception)
        {
            Logger.Verbose("enter");
            RemoveAllCheckTasks_();
            openTask_?.SetException(exception);
            if (closeTask_ != null)
            {
                Logger.Verbose("closeTask_ isn't null. Its type is {0}", closeTask_.TaskType);
                if (closeTask_.TaskType == TaskType.Close)
                {
                    closeTask_.SetExceptionAndAbort(exception);
                }
            }
            else
            {
                Logger.Verbose("closeTask_ is null. Setting it to be a new Abort task.");
                VisualGridTask abortTask = new VisualGridTask(null, null, eyes_, TaskType.Abort, taskListener_, null, null, this, null);
                lock (TaskList)
                {
                    TaskList.Add(abortTask);
                }
                closeTask_ = abortTask;
                Task<TestResultContainer> futureTask = new Task<TestResultContainer>(abortTask.Call, abortTask);
                taskToFutureMapping_[abortTask] = futureTask;
                isCloseTaskIssued_ = true;
            }
            Logger.Verbose("exit");
            return taskToFutureMapping_[closeTask_];
        }

        private void RemoveAllCheckTasks_()
        {
            lock (TaskList)
            {
                for (int i = TaskList.Count - 1; i >= 0; --i)
                {
                    VisualGridTask item = TaskList[i];
                    if (item.TaskType == TaskType.Check)
                    {
                        TaskList.RemoveAt(i);
                    }
                }
            }
        }

        public class RunningTestListener
        {
            public RunningTestListener(Action<VisualGridTask, RunningTest> onTaskComplete, Action onRenderComplete)
            {
                OnTaskComplete = onTaskComplete;
                OnRenderComplete = onRenderComplete;
            }

            public Action<VisualGridTask, RunningTest> OnTaskComplete { get; }
            public Action OnRenderComplete { get; }
        }

        internal RunningTest(RenderBrowserInfo browserInfo, Logger logger)
        {
            BrowserInfo = browserInfo;
            Logger = logger;
        }

        public RunningTest(IEyesConnector eyes, RenderBrowserInfo browserInfo, Logger logger, RunningTestListener listener, string testId)
        {
            eyes_ = eyes;
            BrowserInfo = browserInfo;
            Logger = logger;
            listener_ = listener;
            taskListener_ = new VisualGridTask.TaskListener(OnTaskComplete, OnTaskFailed, OnRenderComplete);
            TestId = testId;
        }

        public bool IsTestOpen { get; set; }
        public bool IsTestClose { get; set; }
        public bool IsTestReadyToClose
        {
            get
            {
                if (TaskList.Count != 1) return false;

                foreach (VisualGridTask task in TaskList)
                {
                    if (task.TaskType == TaskType.Close || task.TaskType == TaskType.Abort) return true;
                }
                return false;
            }
        }

        public bool IsOpenTaskIssued { get => openTask_ != null; }
        public bool IsCloseTaskIssued { get => closeTask_ != null; }

        public Logger Logger { get; set; }

        private RenderBrowserInfo browserInfo_;
        public RenderBrowserInfo BrowserInfo 
        { 
            get => browserInfo_;
            set 
            {
                ObsoleteAttribute obsoleteAttr = value?.BrowserType.GetAttribute<ObsoleteAttribute>();
                if (obsoleteAttr != null)
                {
                    Console.WriteLine("WARNING: " + obsoleteAttr.Message);
                }
                browserInfo_ = value;
            }
        }

        public VisualGridTask Open(Configuration configuration)
        {
            Logger.Verbose("adding Open task to test #{0}...", GetHashCode());
            VisualGridTask vgTask = new VisualGridTask(configuration, null, eyes_, TaskType.Open, taskListener_, null, null, this, null);
            TestName = configuration.TestName;
            openTask_ = vgTask;
            Task<TestResultContainer> task = new Task<TestResultContainer>(vgTask.Call, vgTask);
            Logger.Verbose("task id: {0}", task.Id);
            taskToFutureMapping_.Add(vgTask, task);
            //Logger.Verbose("locking taskList");
            lock (TaskList)
            {
                TaskList.Add(vgTask);
                Logger.Verbose("Open task #{0} was added to list #{1}: {2}", vgTask.GetHashCode(), TaskList.GetHashCode(), vgTask);
                Logger.Verbose("tasks in taskList: {0}", TaskList.Count);
            }
            //Logger.Verbose("releasing taskList");
            return vgTask;
        }

        public Task<TestResultContainer> Close(Configuration configuration, bool throwException)
        {
            VisualGridTask lastTask;
            if (TaskList.Count > 0)
            {
                lastTask = TaskList[TaskList.Count - 1];
                if (lastTask.TaskType == TaskType.Close || lastTask.TaskType == TaskType.Abort)
                {
                    closeTask_ = lastTask;
                    return taskToFutureMapping_[lastTask];
                }
            }
            else
            {
                if (closeTask_ != null)
                {
                    return taskToFutureMapping_[closeTask_];
                }
            }

            Logger.Verbose("adding close task...");

            closeTask_ = new VisualGridTask(configuration, null, eyes_, TaskType.Close, taskListener_, null, null, this, null, throwException);
            Task<TestResultContainer> futureTask = new Task<TestResultContainer>(closeTask_.Call, closeTask_);
            isCloseTaskIssued_ = true;
            taskToFutureMapping_.Add(closeTask_, futureTask);
            Logger.Verbose("futureTask id: {0}", futureTask.Id);

            //Logger.Verbose("locking taskList");
            lock (TaskList)
            {
                TaskList.Add(closeTask_);
                Logger.Verbose("Close task was added: {0}", closeTask_);
                Logger.Verbose("tasks in taskList: {0}", TaskList.Count);
            }
            //Logger.Verbose("releasing taskList");
            //taskToFutureMapping_.TryGetValue(vgTask, out Task<TestResultContainer> value);
            return futureTask;
        }

        public VisualGridTask Check(IConfiguration configuration, ICheckSettings checkSettings, IList<VisualGridSelector[]> regionSelectors, string source)
        {
            Logger.Verbose("adding check task...");
            VisualGridTask vgTask = new VisualGridTask(configuration, null, eyes_, TaskType.Check, taskListener_, checkSettings, regionSelectors, this, source);
            //Logger.Verbose("locking taskList");
            lock (TaskList)
            {
                TaskList.Add(vgTask);
                Logger.Verbose("Check Task was added: {0}", vgTask);
                Logger.Verbose("tasks in taskList: {0}", TaskList.Count);
            }
            //Logger.Verbose("releasing taskList");
            //taskToFutureMapping_.TryGetValue(vgTask, out Task<TestResultContainer> value);
            return vgTask;
        }

        public void OnTaskComplete(VisualGridTask task)
        {
            Logger.Verbose("enter task #{0} with type {1}; locking runningTest.taskList #{2} in test #{3}", task.GetHashCode(), task.TaskType, TaskList.GetHashCode(), GetHashCode());
            lock (TaskList)
            {
                bool success = TaskList.Remove(task);
                Logger.Verbose("Task removal successful: {0}", success);
            }
            //Logger.Verbose("releasing runningTest.taskList");
            switch (task.TaskType)
            {
                case TaskType.Open:
                    IsTestOpen = true;
                    break;
                case TaskType.Close:
                case TaskType.Abort:
                    IsTestClose = true;
                    break;
            }
            listener_?.OnTaskComplete(task, this);
        }

        public void OnTaskFailed(Exception e, VisualGridTask task)
        {
            SetTestInExceptionMode(e);
            listener_.OnTaskComplete(task, this);
        }
        public void OnRenderComplete()
        {
            Logger.Verbose("enter");
            listener_.OnRenderComplete();
            Logger.Verbose("exit");
        }

        public void SetTestInExceptionMode(Exception e)
        {
            isTestInExceptionMode_ = true;
            exception_ = e;

            if (closeTask_ != null)
            {
                lock (TaskList)
                {
                    RemoveAllCheckTasks_();
                    if (!TaskList.Contains(closeTask_))
                    {
                        TaskList.Add(closeTask_);
                    }
                    closeTask_.SetExceptionAndAbort(e);
                }
            }

            if (openTask_ != null)
            {
                openTask_.SetExceptionAndAbort(e);
            }
        }


        public List<VisualGridTask> TaskList { get; } = new List<VisualGridTask>();

        public string TestId { get; private set; }

        internal Task<TestResultContainer> GetNextCloseTask()
        {
            lock (TaskList)
            {
                if (TaskList.Count > 0 && isCloseTaskIssued_)
                {
                    VisualGridTask vgTask = TaskList[0];
                    TaskType taskType = vgTask.TaskType;
                    if (taskType != TaskType.Close && taskType != TaskType.Abort) return null;
                    TaskList.RemoveAt(0);
                    return taskToFutureMapping_[vgTask];
                }
            }
            return null;
        }

        public ScoreTask GetScoreTaskObjectByType(TaskType taskType)
        {
            int score = 0;
            VisualGridTask chosenTask;
            lock (TaskList)
            {
                foreach (VisualGridTask task in TaskList)
                {
                    if (task.IsTaskReadyToCheck && task.TaskType == TaskType.Check)
                    {
                        score++;
                    }
                }

                if (TaskList.Count == 0)
                {
                    //Logger.Verbose("task list empty.");
                    return null;
                }

                chosenTask = TaskList[0];
                if (chosenTask.TaskType != taskType || chosenTask.IsSent || (taskType == TaskType.Open && !chosenTask.IsTaskReadyToCheck))
                {
                    Logger.Verbose("No relevant tasks in list. List content: {0}", TaskList.Concat(" ; "));
                    return null;
                }
            }
            Logger.Verbose("creating a new ScoreTask with score {0} and Task: {1}", score, chosenTask);
            return new ScoreTask(chosenTask, score);
        }

        public Task<TestResultContainer> AbortIfNotClosed()
        {
            if (isCloseTaskIssued_) return null;
            return Abort(null);
        }

        public IBatchCloser GetBatchCloser()
        {
            return (IBatchCloser)eyes_;
        }
    }
}