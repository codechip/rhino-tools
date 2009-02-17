#region license
// Copyright (c) 2005 - 2007 Ayende Rahien (ayende@ayende.com)
// All rights reserved.
// 
// Redistribution and use in source and binary forms, with or without modification,
// are permitted provided that the following conditions are met:
// 
//     * Redistributions of source code must retain the above copyright notice,
//     this list of conditions and the following disclaimer.
//     * Redistributions in binary form must reproduce the above copyright notice,
//     this list of conditions and the following disclaimer in the documentation
//     and/or other materials provided with the distribution.
//     * Neither the name of Ayende Rahien nor the names of its
//     contributors may be used to endorse or promote products derived from this
//     software without specific prior written permission.
// 
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND
// ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
// WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
// DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS BE LIABLE
// FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL
// DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR
// SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER
// CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY,
// OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF
// THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
#endregion


using System;
using System.Collections.Generic;
using System.Threading;
using log4net;

namespace Rhino.Commons
{
	/// <summary>
	/// The code originate from this MSDN article: 
	/// http://msdn.microsoft.com/msdnmag/issues/06/03/NETMatters/
	/// with my own additions to it later on, such as CancelAll(), WorkStarted, WorkFinished, etc.
	/// </summary>
	public static class RhinoThreadPool
	{
		private static ILog logger = LogManager.GetLogger(typeof (RhinoThreadPool));
		private static LinkedList<WorkItem> _callbacks = new LinkedList<WorkItem>();

		/// <summary>
		/// Guranteed to be called before a work item starts executing
		/// </summary>
		public static event Proc<WorkItem> WorkStarted = delegate { };

		/// <summary>
		/// Guranteed to be called after a work item has finished executing (but not if the thread was forcefully aborted).
		/// </summary>
        public static event Proc<WorkItem> WorkFinished = delegate { };

		/// <summary>
		/// Any modification to this variable should be done under the _callbacks lock!
		/// </summary>
		private static Dictionary<WorkItem, Thread> _threads = new Dictionary<WorkItem, Thread>();

		private static AutoResetEvent _waitForAllThreadToComplete;

		public static WorkItem QueueUserWorkItem(WaitCallback callback)
		{
			return QueueUserWorkItem(callback, null);
		}

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

				ExecutionContext.Run(item.Context, ExecuteWorkItem, item);
			}
			finally
			{
				Local.Data.Clear();
				lock (_callbacks)
				{
					if (item != null)
					{
						_threads.Remove(item);
						if (_callbacks.Count == 0 && _threads.Count == 0 && _waitForAllThreadToComplete != null)
						{
							_waitForAllThreadToComplete.Set();
						}
					}
				}
			}
		}

		private static void ExecuteWorkItem(object state)
		{
			WorkItem item = (WorkItem) state;
			WorkStarted(item);
			try
			{
                if(item.Cancelled==false)
				    item.Callback(item.State);
			}
			catch (Exception e)
			{
				logger.Fatal(string.Format("An exception was raised when executing work item [{0}.{1}]. It will probably kill the application", 
					item.Callback.Method.DeclaringType, 
					item.Callback.Method), e);
				throw;
			}
			finally
			{
				WorkFinished(item);
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
				if (_callbacks.Count == 0 && _threads.Count == 0)
					return;
			}
			_waitForAllThreadToComplete = new AutoResetEvent(false);
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
