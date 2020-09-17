namespace Applitools.Utils.Async
{
    using System;
    using System.Collections.Generic;
    using System.Threading;

    /// <summary>
    /// <see cref="SerialDequeue&lt;T&gt;"/> dequeue handler signature.
    /// </summary>
    public delegate void SerialDequeueHandler<T>(T item);

    /// <summary>
    /// Dequeue items serially as soon as they are available.
    /// </summary>
    /// <remarks>Objects of this class are thread safe</remarks>
    public sealed class SerialDequeue<T> : IDisposable
    {
        #region Fields

        private LinkedList<T> queue_;
        private Thread thread_;
        private SerialDequeueHandler<T> dequeue_;
        private volatile bool disposed_;

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new <see cref="SerialDequeue&lt;T&gt;"/> instance.
        /// </summary>
        /// <param name="dequeue">Handles dequeue callbacks</param>
        public SerialDequeue(SerialDequeueHandler<T> dequeue)
        {
            ArgumentGuard.NotNull(dequeue, nameof(dequeue));

            dequeue_ = dequeue;
            queue_ = new LinkedList<T>();
            disposed_ = false;
            
            thread_ = new Thread(Dequeue_);
            thread_.Name = typeof(SerialDequeue<T>).Name;
            thread_.IsBackground = true;
            thread_.Start();
        }

        #endregion

        #region Methods

        /// <summary>
        /// Enqueue an item in the queue
        /// </summary>
        public void Enqueue(T item)
        {
            lock (queue_)
            {
                queue_.AddLast(item);
                if (queue_.Count == 1)
                {
                    Monitor.Pulse(queue_);
                }
            }
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            disposed_ = true;
            lock (queue_)
            {
                Monitor.PulseAll(queue_);
            }

            thread_.Join();
        }

        private void Dequeue_()
        {
            while (!disposed_)
            {
                LinkedListNode<T> first = null;
                lock (queue_)
                {
                    first = queue_.First;
                    if (first == null)
                    {
                        Monitor.Wait(queue_);
                        continue;
                    }

                    queue_.RemoveFirst();
                }

                try
                {
                    dequeue_(first.Value);
                }
                catch 
                {
                }
            }
        }

        #endregion
    }
}
