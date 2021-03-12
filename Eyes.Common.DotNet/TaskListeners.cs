using Applitools.Utils;
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
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

    public class SyncTaskListener : TaskListener
    {
        private readonly Action onComplete_;
        private readonly Action<Exception> onFail_;
        private readonly AutoResetEvent sync_;
        private readonly Logger logger_;
        private readonly string caller_;
        private readonly int callingThread_;
        private readonly string[] testIds_;
        private bool? result_;

        public SyncTaskListener(Action onComplete = null, Action<Exception> onFail = null,
            Logger logger = null, [CallerMemberName] string callingMember = null, params string[] testIds)
            : base(onComplete, onFail)
        {
            onComplete_ = onComplete;
            OnComplete = OnComplete_;
            onFail_ = onFail;
            OnFail = OnFail_;
            logger_ = logger;
            caller_ = callingMember;
            callingThread_ = Thread.CurrentThread.ManagedThreadId;
            testIds_ = testIds;
            logger_?.Log(TraceLevel.Notice, testIds, Stage.General, new { callingMember });
            sync_ = new AutoResetEvent(false);
        }

        private void OnComplete_()
        {
            try
            {
                logger_?.Log(TraceLevel.Notice, testIds_, Stage.General, StageType.Complete, new { caller_, callingThread_ });
                onComplete_?.Invoke();
            }
            finally
            {
                result_ = true;
                sync_.Set();
            }
        }

        private void OnFail_(Exception e)
        {
            try
            {
                CommonUtils.LogExceptionStackTrace(logger_, Stage.General, e, testIds_);
                Exception = e;
                onFail_?.Invoke(e);
            }
            finally
            {
                result_ = false;
                sync_.Set();
            }
        }

        public bool? Get()
        {
            logger_?.Log(TraceLevel.Notice, testIds_, Stage.General, new { message = $"Waiting for finish...", caller_ });
            Stopwatch sw = Stopwatch.StartNew();
            TimeSpan timeout = TimeSpan.FromMinutes(5);
            bool result;
            do
            {
                result = sync_.WaitOne(TimeSpan.FromMinutes(1));
                if (!result) logger_?.Log(TraceLevel.Notice, testIds_, Stage.General,
                        new { message = $"still waiting for finish...", caller_, sw.Elapsed });
            } while (!result && sw.Elapsed < timeout);

            if (result)
            {
                logger_?.Log(TraceLevel.Notice, testIds_, Stage.General, new { message = $"finished.", caller_, callingThread_ });
            }
            else
            {
                throw new EyesException("Failed waiting for finish.");
            }
            return result_;
        }

        public Exception Exception { get; private set; }
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

    public class LoggingListener<T> : TaskListener<T>
    {
        private readonly Action<T> onComplete_;
        private readonly Action<Exception> onFail_;
        private readonly Logger logger_;
        private readonly string message_;
        private readonly string caller_;
        private readonly int callingThread_;
        private readonly string[] testIds_;

        public LoggingListener(TaskListener<T> internalListener, Logger logger,
            string message, [CallerMemberName] string caller = null, params string[] testIds)
            : this(internalListener.OnComplete, internalListener.OnFail, logger, caller, message, testIds)
        {
        }

        public LoggingListener(Action<T> onComplete, Action<Exception> onFail, Logger logger,
            string message, [CallerMemberName] string caller = null, params string[] testIds)
            : base(onComplete, onFail)
        {
            logger_ = logger;
            message_ = message;
            caller_ = caller;
            callingThread_ = Thread.CurrentThread.ManagedThreadId;
            testIds_ = testIds;
            onComplete_ = onComplete;
            OnComplete = OnComplete_;
            onFail_ = onFail;
            OnFail = OnFail_;
        }

        private void OnComplete_(T t)
        {
            logger_?.Log(TraceLevel.Info, testIds_, Stage.General, StageType.Complete,
                new { caller_, callingThread_, message_, t });
            onComplete_?.Invoke(t);
        }

        private void OnFail_(Exception e)
        {
            CommonUtils.LogExceptionStackTrace(logger_, Stage.General, e, new { caller_, callingThread_ }, testIds_);
            onFail_?.Invoke(e);
        }
    }

    public class SyncTaskListener<T> : TaskListener<T>
    {
        private readonly Action<T> onComplete_;
        private readonly Action<Exception> onFail_;
        private readonly AutoResetEvent sync_;
        private readonly Logger logger_;
        private readonly TimeSpan timeout_;
        private readonly string caller_;
        private readonly int callingThread_;
        private readonly string[] testIds_;
        private T result_;

        public SyncTaskListener(Action<T> onComplete = null, Action<Exception> onFail = null,
            Logger logger = null, TimeSpan? timeout = null,
            [CallerMemberName] string callingMember = null, params string[] testIds)
            : base(onComplete, onFail)
        {
            onComplete_ = onComplete;
            OnComplete = OnComplete_;
            onFail_ = onFail;
            OnFail = OnFail_;
            logger_ = logger;
            timeout_ = timeout.HasValue ? timeout.Value : TimeSpan.FromMinutes(5);
            caller_ = callingMember;
            callingThread_ = Thread.CurrentThread.ManagedThreadId;
            testIds_ = testIds;
            sync_ = new AutoResetEvent(false);
        }

        private void OnComplete_(T t)
        {
            try
            {
                logger_?.Log(TraceLevel.Notice, testIds_, Stage.General, StageType.Complete, new { caller_, callingThread_ });
                onComplete_?.Invoke(t);
            }
            finally
            {
                result_ = t;
                sync_.Set();
            }
        }

        private void OnFail_(Exception e)
        {
            try
            {
                CommonUtils.LogExceptionStackTrace(logger_, Stage.General, e, new { caller_, callingThread_ }, testIds_);
                Exception = e;
                onFail_?.Invoke(e);
            }
            finally
            {
                sync_.Set();
            }
        }

        public T Get()
        {
            logger_?.Log(TraceLevel.Notice, testIds_, Stage.General, new { message = $"Waiting for results", caller_ });
            Stopwatch sw = Stopwatch.StartNew();
            bool result;
            do
            {
                result = sync_.WaitOne(Math.Min((int)timeout_.TotalMilliseconds, 60000));
                if (!result) logger_?.Log(TraceLevel.Notice, testIds_, Stage.General,
                        new { message = $"still waiting for finish...", caller_, sw.Elapsed });
            } while (!result && sw.Elapsed < timeout_);

            if (result)
            {
                logger_?.Log(TraceLevel.Notice, testIds_, Stage.General, new { message = $"finished.", caller_, callingThread_ });
            }
            else
            {
                throw new EyesException("Failed waiting for finish.");
            }
            logger_?.Log(TraceLevel.Info, testIds_, Stage.General, new { message = $"Results arrived", caller_, callingThread_, result_ });
            return result_;
        }

        public Exception Exception { get; private set; }
    }
}