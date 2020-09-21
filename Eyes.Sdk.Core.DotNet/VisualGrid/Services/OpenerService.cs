using System.Threading;
using System.Threading.Tasks;

namespace Applitools.VisualGrid
{
    class OpenerService : EyesService
    {
        private readonly AutoResetEvent concurrencyLock_;
        private int currentTestsAmount_;

        public OpenerService(string serviceName, Logger logger, int testsPoolSize, AutoResetEvent openerServiceLock, EyesServiceListener listener, 
            AutoResetEvent innerDebugLock, AutoResetEvent outerDebugLock, Tasker tasker)
            : base(serviceName, logger, testsPoolSize, innerDebugLock, outerDebugLock, listener, tasker)
        {
            concurrencyLock_ = openerServiceLock;
        }

        public int DecrementConcurrency()
        {
            return Interlocked.Decrement(ref currentTestsAmount_);
        }

        protected override void RunNextTask()
        {
            if (!isServiceOn_) return;
            //Logger.Verbose("threadPoolSize_: {0} ; concurrentSessions_: {1}", threadPoolSize_, concurrentSessions_);
            if (testsPoolSize_ > currentTestsAmount_)
            {
                Task<TestResultContainer> task = listener_.GetNextTask(tasker_);
                if (task != null)
                {
                    Interlocked.Increment(ref currentTestsAmount_);
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
