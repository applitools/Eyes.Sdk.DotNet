using System;

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

        public TaskListener(Action<T> onComplete, Action<Exception> onFail)
        {
            OnComplete = onComplete;
            OnFail = onFail;
        }
    }
}