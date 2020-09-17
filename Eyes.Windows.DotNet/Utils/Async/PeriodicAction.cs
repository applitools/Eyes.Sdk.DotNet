namespace Applitools.Utils.Async
{
    using System;
    using System.Threading;

    /// <summary>
    /// Periodic and on-demand async action trigger.
    /// </summary>
    public sealed class PeriodicAction : IDisposable
    {
        #region Fields

        private readonly object lock_ = new object();
        private readonly Thread workerThread_;
        private readonly Action action_;
        private TimeSpan interval_;
        private volatile bool wait_;
        private volatile bool disposed_ = false;

        #endregion

        #region Constructors

        /// <summary>
        /// Triggers the input action periodically at the given interval.
        /// </summary>
        /// <param name="interval">Trigger interval. <c>TimeSpan.Zero</c> means never</param>
        /// <param name="action">Action to perform periodically</param>
        /// <param name="start">Whether or not to start dispatching actions</param>
        /// <param name="name">Name of dispatch thread</param>
        public PeriodicAction(
            TimeSpan interval, 
            Action action, 
            bool start,
            string name = "PeriodicAction")
        {
            ArgumentGuard.NotNull(action, nameof(action));
            ArgumentGuard.TrimNotEmpty(ref name, nameof(name));

            interval_ = interval;
            action_ = action;
            wait_ = true;

            workerThread_ = new Thread(TriggerLoop_);
            workerThread_.Name = name;
            workerThread_.IsBackground = true;
            if (start)
            {
                workerThread_.Start();
            }
        }
        
        #endregion
        
        #region Methods

        #region Public

        /// <summary>
        /// Starts this periodic action if it wasn't started already.
        /// </summary>
        public void Start()
        {
            lock (lock_)
            {
                if (disposed_)
                {
                    throw new ObjectDisposedException(GetType().Name);
                }

                if (workerThread_.IsAlive)
                {
                    return;
                }

                workerThread_.Start();
            }
        }

        /// <summary>
        /// Immediately triggers the action without blocking the caller and resets the interval 
        /// until the next periodic trigger.
        /// </summary>
        public void Trigger()
        {
            lock (lock_)
            {
                wait_ = false;
                Monitor.Pulse(lock_);
            }
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            lock (lock_)
            {
                if (disposed_)
                {
                    return;
                }

                disposed_ = true;
                
                if (!workerThread_.IsAlive)
                {
                    return;
                }
            }

            Trigger();
            workerThread_.Join();
        }

        #endregion

        #region Private

        private void TriggerLoop_()
        {
            while (!disposed_)
            {
                try
                {
                    lock (lock_)
                    {
                        if (wait_)
                        {
                            if (interval_ == TimeSpan.Zero)
                            {
                                Monitor.Wait(lock_);
                            }
                            else
                            {
                                Monitor.Wait(lock_, interval_);
                            }
                        }

                        if (wait_)
                        {
                            // Trigger wasn't called (wait_ == true) which means that wait 
                            // timed out but since the interval is zero we don't execute the 
                            // action.
                            if (interval_ == TimeSpan.Zero)
                            {
                                continue;
                            }
                        }

                        wait_ = true;
                    }

                    if (disposed_)
                    {
                        return;
                    }

                    action_();
                }
                catch
                {
                }
            }
        }

        #endregion

        #endregion
    }
}
