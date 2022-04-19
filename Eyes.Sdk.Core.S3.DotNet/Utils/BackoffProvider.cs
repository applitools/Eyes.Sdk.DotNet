using System;
using System.Threading;

namespace Applitools.Utils
{
    public class BackoffProvider
    {
        private int attempt_ = 0;
        private int maxAttempts_ = 0;
        private readonly TimeSpan[] backoffTimes_ = new TimeSpan[]
            {
                TimeSpan.FromMilliseconds(500),
                TimeSpan.FromMilliseconds(500),
                TimeSpan.FromSeconds(1),
                TimeSpan.FromSeconds(2),
                TimeSpan.FromSeconds(4),
                TimeSpan.FromSeconds(8),
            };

        public BackoffProvider(int maxAttempts = 2)
        {
            maxAttempts_ = maxAttempts;
        }

        public bool ShouldWait => attempt_ < maxAttempts_;

        public TimeSpan GetNextWait()
        {
            return backoffTimes_[Math.Min(attempt_++, backoffTimes_.Length - 1)];
        }

        public void Wait()
        {
            Thread.Sleep(GetNextWait());
        }
    }
}
