using System;
using System.Threading;

namespace Applitools
{
    public class TaskListener
    {
        public Action OnComplete;
        public Action<Exception> OnFail;

        public TaskListener(Action onComplete, Action<Exception> onFail)
        {
            OnComplete = onComplete;
            OnFail = onFail;
        }
    }

    public class TaskListener<T>
    {
        public Action<T> OnComplete;
        public Action<Exception> OnFail;

        protected TaskListener() { }

        public TaskListener(Action<T> onComplete, Action<Exception> onFail)
        {
            OnComplete = onComplete;
            OnFail = onFail;
        }
    }

    public class SyncTaskListener<T> : TaskListener<T>
    {
        private Action<T> onComplete_;
        private Action<Exception> onFail_;
        private AutoResetEvent sync_;
        private T result_;

        public SyncTaskListener(Action<T> onComplete = null, Action<Exception> onFail = null)
            : base(onComplete, onFail)
        {
            onComplete_ = onComplete;
            OnComplete = OnComplete_;
            onFail_ = onFail;
            OnFail = OnFail_;
            sync_ = new AutoResetEvent(false);
        }

        private void OnComplete_(T t)
        {
            onComplete_?.Invoke(t);
            result_ = t;
            sync_.Set();
        }

        private void OnFail_(Exception e)
        {
            Exception = e;
            onFail_?.Invoke(e);
            sync_.Set();
        }

        public T Get()
        {
            if (!sync_.WaitOne(0)) sync_.WaitOne();
            return result_;
        }

        public Exception Exception { get; private set; }
    }
}