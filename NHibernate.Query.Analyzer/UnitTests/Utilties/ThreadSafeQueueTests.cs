using System;
using System.Threading;
using Ayende.NHibernateQueryAnalyzer.Utilities;
using NUnit.Framework;

namespace Ayende.NHibernateQueryAnalyzer.Tests.Utiltites
{
	[TestFixture]
	public class ThreadSafeQueueTests
	{
		private IQueue q;

		private ManualResetEvent flag;

		[SetUp]
		public void Setup()
		{
			q = new ThreadSafeQueue();
			flag = new ManualResetEvent(false);
		}

		[TearDown]
		public void Teardown()
		{
			flag.Close();
		}

		[Test]
		public void EnqueueShouldWork()
		{
			q.Enqueue(new object());
		}

		[Test]
		public void DequeueAfterEnqueueShouldReturnTheSameObject()
		{
			object o = new object();

			q.Enqueue(o);
			Assert.AreSame(o, q.Dequeue());
		}

		#region DequeueOnAnEmptyQueueShouldSleep

		[Test]
		public void DequeueOnAnEmptyQueueShouldSleep()
		{
			Thread t = new Thread(new ThreadStart(DequeueFromEmptyQueue));
			t.Start();

			flag.WaitOne(); // let other thread run
			Thread.Sleep(10);

			Assert.AreEqual(ThreadState.WaitSleepJoin, t.ThreadState);

			t.Abort(); // clean up
		}

		private void DequeueFromEmptyQueue()
		{
			flag.Set();
			q.Dequeue();
		}
		#endregion

		#region EnqueueShouldReleaseSleepingThread

		[Test]
		public void EnqueueShouldReleaseSleepingThread()
		{
			Thread t = new Thread(new ThreadStart(DequeueFromNonEmptyQueue));
			t.Start();

			flag.WaitOne(); // let other thread run
			Thread.Sleep(10);

			Assert.AreEqual(ThreadState.WaitSleepJoin, t.ThreadState);

			flag.Reset();

			q.Enqueue(1);

			flag.WaitOne(); // let other thread finish
			Thread.Sleep(10);

			Assert.IsTrue(true);
		}

		private void DequeueFromNonEmptyQueue()
		{
			flag.Set();
			object o = q.Dequeue();

			flag.Set();

			Assert.AreEqual(1, Convert.ToInt32(o));
		}
		#endregion
	}
}