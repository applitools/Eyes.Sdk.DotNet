using System.Threading;
using System.Threading.Tasks;

namespace Applitools.VisualGrid
{
    class OpenerService : EyesService
    {
        private readonly AutoResetEvent concurrencyLock_;
        private int concurrentSessions_;

        public OpenerService(string serviceName, Logger logger, int threadPoolSize, AutoResetEvent openerServiceLock, EyesServiceListener listener, 
            AutoResetEvent innerDebugLock, AutoResetEvent outerDebugLock, Tasker tasker)
            : base(serviceName, logger, threadPoolSize, innerDebugLock, outerDebugLock, listener, tasker)
        {
            concurrencyLock_ = openerServiceLock;
        }

        public int DecrementConcurrency()
        {
            return Interlocked.Decrement(ref concurrentSessions_);
        }

        protected override void RunNextTask()
        {
            if (!isServiceOn_) return;
            //Logger.Verbose("threadPoolSize_: {0} ; concurrentSessions_: {1}", threadPoolSize_, concurrentSessions_);
            if (threadPoolSize_ > concurrentSessions_)
            {
                Task<TestResultContainer> task = listener_.GetNextTask(tasker_);
                if (task != null)
                {
                    Interlocked.Increment(ref concurrentSessions_);
                    PauseIfNeeded();
                    task.Start();
                }
            }
            else
            {
                Logger.Verbose("Waiting for concurrency to be free");
                concurrencyLock_.WaitOne();
                Logger.Verbose("concurrency free");
            }
        }

    }
}
