using System;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using Applitools.Utils;

namespace Applitools.VisualGrid
{
    // Based on code from https://docs.microsoft.com/en-us/dotnet/api/system.threading.tasks.taskscheduler?view=netframework-4.7.2#Default

    // Provides a task scheduler that ensures a maximum concurrency level while 
    // running on top of the thread pool.
    public class LimitedConcurrencyLevelTaskScheduler : TaskScheduler
    {
        // Indicates whether the current thread is processing work items.
        [ThreadStatic]
        private static bool currentThreadIsProcessingItems_;

        // The list of tasks to be executed 
        private readonly LinkedList<Task> tasks_ = new LinkedList<Task>(); // protected by lock(_tasks)

        // The maximum concurrency level allowed by this scheduler. 
        private readonly int maxDegreeOfParallelism_;

        // Indicates whether the scheduler is currently processing work items. 
        private int delegatesQueuedOrRunning_ = 0;

        // Creates a new instance with the specified degree of parallelism. 
        public LimitedConcurrencyLevelTaskScheduler(int maxDegreeOfParallelism)
        {
            ArgumentGuard.GreaterThan(maxDegreeOfParallelism, 0, nameof(maxDegreeOfParallelism));
            maxDegreeOfParallelism_ = maxDegreeOfParallelism;
        }

        // Queues a task to the scheduler. 
        protected sealed override void QueueTask(Task task)
        {
            // Add the task to the list of tasks to be processed.  If there aren't enough 
            // delegates currently queued or running to process tasks, schedule another. 
            lock (tasks_)
            {
                tasks_.AddLast(task);
                if (delegatesQueuedOrRunning_ < maxDegreeOfParallelism_)
                {
                    ++delegatesQueuedOrRunning_;
                    NotifyThreadPoolOfPendingWork();
                }
            }
        }

        // Inform the ThreadPool that there's work to be executed for this scheduler. 
        private void NotifyThreadPoolOfPendingWork()
        {
            ThreadPool.UnsafeQueueUserWorkItem(_ =>
            {
                // Note that the current thread is now processing work items.
                // This is necessary to enable inlining of tasks into this thread.
                currentThreadIsProcessingItems_ = true;
                try
                {
                    // Process all available items in the queue.
                    while (true)
                    {
                        Task item;
                        lock (tasks_)
                        {
                            // When there are no more items to be processed,
                            // note that we're done processing, and get out.
                            if (tasks_.Count == 0)
                            {
                                --delegatesQueuedOrRunning_;
                                break;
                            }

                            // Get the next item from the queue
                            item = tasks_.First.Value;
                            tasks_.RemoveFirst();
                        }

                        // Execute the task we pulled out of the queue
                        base.TryExecuteTask(item);
                    }
                }
                // We're done processing items on the current thread
                finally { currentThreadIsProcessingItems_ = false; }
            }, null);
        }

        // Attempts to execute the specified task on the current thread. 
        protected sealed override bool TryExecuteTaskInline(Task task, bool taskWasPreviouslyQueued)
        {
            // If this thread isn't already processing a task, we don't support inlining
            if (!currentThreadIsProcessingItems_) return false;

            // If the task was previously queued, remove it from the queue
            if (taskWasPreviouslyQueued)
                // Try to run the task. 
                if (TryDequeue(task))
                    return base.TryExecuteTask(task);
                else
                    return false;
            else
                return base.TryExecuteTask(task);
        }

        // Attempt to remove a previously scheduled task from the scheduler. 
        protected sealed override bool TryDequeue(Task task)
        {
            lock (tasks_) return tasks_.Remove(task);
        }

        // Gets the maximum concurrency level supported by this scheduler. 
        public sealed override int MaximumConcurrencyLevel { get { return maxDegreeOfParallelism_; } }

        // Gets an enumerable of the tasks currently scheduled on this scheduler. 
        protected sealed override IEnumerable<Task> GetScheduledTasks()
        {
            bool lockTaken = false;
            try
            {
                Monitor.TryEnter(tasks_, ref lockTaken);
                if (lockTaken) return tasks_;
                else throw new NotSupportedException();
            }
            finally
            {
                if (lockTaken) Monitor.Exit(tasks_);
            }
        }
    }
}
