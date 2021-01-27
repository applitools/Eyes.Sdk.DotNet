using System;
using System.Threading;

namespace Applitools
{
    internal class MockAsyncResult : IAsyncResult
    {
        public MockAsyncResult(object state)
        {
            AsyncState = state;
        }

        public object AsyncState { get; }

        public WaitHandle AsyncWaitHandle { get; } = new AutoResetEvent(true);

        public bool CompletedSynchronously => true;

        public bool IsCompleted => true;
    }
}