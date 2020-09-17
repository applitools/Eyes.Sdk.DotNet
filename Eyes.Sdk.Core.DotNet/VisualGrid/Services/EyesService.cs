using System;
using System.Threading;
using System.Threading.Tasks;

namespace Applitools.VisualGrid
{
    internal class EyesService
    {
        private readonly Thread thread_;
        protected EyesServiceListener listener_;
        private readonly AutoResetEvent innerDebugLock_;
        private readonly AutoResetEvent outerDebugLock_;
        protected Tasker tasker_;
        protected bool isServiceOn_ = true;
        protected readonly int threadPoolSize_;

        protected Logger Logger { get; private set; }

        public class Tasker
        {
            public readonly Func<Task<TestResultContainer>> GetNextTask;

            public Tasker(Func<Task<TestResultContainer>> getNextTask)
            {
                GetNextTask = getNextTask;
            }
        }

        public class EyesServiceListener
        {
            public Func<Tasker, Task<TestResultContainer>> GetNextTask { get; }

            public EyesServiceListener(Func<Tasker, Task<TestResultContainer>> getNextTask)
            {
                GetNextTask = getNextTask;
            }
        }

        public EyesService(string serviceName, Logger logger, int threadPoolSize, 
            AutoResetEvent innerDebugLock, AutoResetEvent outerDebugLock, 
            EyesServiceListener listener, Tasker tasker)
        {
            thread_ = new Thread(Run);
            thread_.IsBackground = true;
            thread_.Name = serviceName;

            threadPoolSize_ = threadPoolSize;
            listener_ = listener;
            Logger = logger;
            innerDebugLock_ = innerDebugLock;
            outerDebugLock_ = outerDebugLock;
            tasker_ = tasker;
        }

        public virtual void Run()
        {
            Logger.Log("Service '" + thread_.Name + "' is starting");
            while (isServiceOn_)
            {
                PauseIfNeeded();
                RunNextTask();
            }
            Logger.Log("Service '" + thread_.Name + "' is finished");
        }

        protected void PauseIfNeeded()
        {
            innerDebugLock_?.WaitOne();
            outerDebugLock_?.Reset();
        }

        protected virtual void RunNextTask()
        {
            //Logger.Verbose("enter. isServiceOn_: {0}", isServiceOn_);

            if (!isServiceOn_) return;
            Task<TestResultContainer> task = listener_.GetNextTask(tasker_);
            if (task != null)
            {
                PauseIfNeeded();
                task.Start();
            }
        }

        private void DebugNotify_()
        {
            innerDebugLock_?.Set();
        }

        public void DebugPauseService()
        {
            innerDebugLock_?.Reset();
        }

        internal void Start()
        {
            thread_.Start();
        }

        internal void Stop()
        {
            Logger.Verbose("stopping service {0}", thread_.Name);
            isServiceOn_ = false;
        }
    }
}