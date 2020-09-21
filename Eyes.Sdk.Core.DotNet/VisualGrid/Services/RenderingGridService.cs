using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Applitools.VisualGrid
{
    internal class RenderingGridService
    {
        private readonly string serviceName_;
        private readonly Logger logger_;
        private readonly AutoResetEvent innerDebugLock_;
        private readonly AutoResetEvent outerDebugLock_;
        private readonly RGServiceListener listener_;
        private readonly Thread thread_;
        private bool isServiceOn_ = true;
        private readonly List<Task<RenderStatusResults>> activeRenderTasks_ = new List<Task<RenderStatusResults>>();

        public class RGServiceListener
        {
            public Func<RenderingTask> GetNextTask { get; }

            public RGServiceListener(Func<RenderingTask> getNextTask)
            {
                GetNextTask = getNextTask;
            }
        }

        public RenderingGridService(string serviceName, Logger logger, int threadPoolSize, AutoResetEvent innerDebugLock, AutoResetEvent outerDebugLock, RGServiceListener listener)
        {
            serviceName_ = serviceName;
            logger_ = logger;
            innerDebugLock_ = innerDebugLock;
            outerDebugLock_ = outerDebugLock;
            listener_ = listener;
            thread_ = new Thread(Run);
            thread_.IsBackground = true;
            thread_.Name = serviceName;
        }

        private void Run()
        {
            logger_.Log("Service '" + thread_.Name + "' is starting");
            while (isServiceOn_)
            {
                PauseIfNeeded_();
                RunNextTask_();
            }
            logger_.Log("Service '" + thread_.Name + "' is finished");
        }

        private void PauseIfNeeded_()
        {
            innerDebugLock_?.WaitOne();
            outerDebugLock_?.Reset();
        }

        private void RunNextTask_()
        {
            if (!isServiceOn_) return;
            logger_.Verbose("enter");
            RenderingTask task = listener_.GetNextTask();
            if (task == null || !task.IsReady())
            {
                return;
            }

            logger_.Verbose("adding listener to task");
            task.AddListener(new RenderingTask.RenderTaskListener(() =>
                {
                    DebugNotify_();
                }, (e) =>
                {
                    DebugNotify_();
                }));

            try
            {
                Task<RenderStatusResults> resultTask = Task.Run(task.CallAsync);
            }
            catch (Exception e)
            {
                logger_.Verbose("Exception in - this.executor.submit(task);");
                if (e.Message.Contains("Read timed out"))
                {
                    logger_.Log("Read timed out");
                }
                logger_.Log("Error: " + e);
            }
            logger_.Verbose("exit");
        }

        internal void Start()
        {
            thread_.Start();
        }

        internal void Stop()
        {
            logger_.Verbose("stopping service {0}", thread_.Name);
            isServiceOn_ = false;
        }

        private void DebugNotify_()
        {
            innerDebugLock_?.Set();
        }

        internal void DebugPauseService()
        {
            innerDebugLock_?.Reset();
        }
    }
}