using System;
using System.Collections.Generic;
using System.Threading;

namespace Rhino.Commons
{
    /// <summary>
    /// The code originate from this MSDN article: 
    /// http://msdn.microsoft.com/msdnmag/issues/06/03/NETMatters/
    /// With minor improvemts: CancelAll(bool)
    /// </summary>
    public static class AbortableThreadPool
    {
        private static LinkedList<WorkItem> _callbacks = new LinkedList<WorkItem>();

        /// <summary>
        /// Any modification to this variable should be done under the _callbacks lock!
        /// </summary>
        private static Dictionary<WorkItem, Thread> _threads = new Dictionary<WorkItem, Thread>();

        private static AutoResetEvent _waitForAllThreadToComplete;

        public static WorkItem QueueUserWorkItem(WaitCallback callback, object state)
        {
            if (callback == null) throw new ArgumentNullException("callback");
            WorkItem item = new WorkItem(callback, state, ExecutionContext.Capture());
            lock (_callbacks)
                _callbacks.AddLast(item);
            ThreadPool.QueueUserWorkItem(new WaitCallback(HandleItem));
            return item;
        }

        private static void HandleItem(object ignored)
        {
            WorkItem item = null;
            try
            {
                lock (_callbacks)
                {
                    if (_callbacks.Count > 0)
                    {
                        item = _callbacks.First.Value;
                        _callbacks.RemoveFirst();
                    }
                    if (item == null) return;
                    _threads.Add(item, Thread.CurrentThread);
                }

                ExecutionContext.Run(item.Context, delegate
                {
                    item.Callback(item.State);
                }, null);
            }
            finally
            {
                lock (_callbacks)
                {
                    if (item != null)
                    {
                        _threads.Remove(item);
                        if (_threads.Count == 0 && _waitForAllThreadToComplete != null)
                        {
                            _waitForAllThreadToComplete.Set();
                        }
                    }
                }
            }
        }

        public static WorkItemStatus Cancel(WorkItem item, bool allowAbort)
        {
            if (item == null) throw new ArgumentNullException("item");

            lock (_callbacks)
            {
                LinkedListNode<WorkItem> node = _callbacks.Find(item);
                if (node != null)
                {
                    _callbacks.Remove(node);
                    return WorkItemStatus.Queued;
                }
                else if (_threads.ContainsKey(item))
                {
                    if (allowAbort)
                    {
                        _threads[item].Abort();
                        _threads.Remove(item);
                        return WorkItemStatus.Aborted;
                    }
                    else return WorkItemStatus.Executing;
                }
                else return WorkItemStatus.Completed;
            }
        }

        public static void CancelAll(bool allowAbort)
        {
            lock (_callbacks)
            {
                _callbacks.Clear();
                if (allowAbort)
                {
                    foreach (Thread thread in _threads.Values)
                    {
                        thread.Abort();
                    }
                    _threads.Clear();
                }
            }
        }

        public static void WaitForAllThreadsToFinish()
        {
            lock (_callbacks)
            {
                if (_threads.Count == 0)
                    return;
            }
            _waitForAllThreadToComplete = new AutoResetEvent(true);
            try
            {
                _waitForAllThreadToComplete.WaitOne();
            }
            finally
            {
                _waitForAllThreadToComplete.Close();
                _waitForAllThreadToComplete = null;
            }
        }
    }
}