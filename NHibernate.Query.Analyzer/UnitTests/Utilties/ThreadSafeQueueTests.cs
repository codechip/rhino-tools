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
using System.Threading;
using Ayende.NHibernateQueryAnalyzer.Utilities;
using MbUnit.Framework;

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