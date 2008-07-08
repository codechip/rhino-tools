using System;
using System.Threading;
using MbUnit.Framework;
using Rhino.Queues.Threading;

namespace Rhino.Queues.Tests.Threading
{
	[TestFixture]
	public class BlockingQueueTests
	{
		[Test]
		public void When_item_exists_will_not_block()
		{
			var queue = new BlockingQueue<string>();
			queue.Enqueue("foo");
			string t;
			queue.Dequeue(TimeSpan.FromSeconds(5), out t);
			Assert.AreEqual("foo", t);
		}

		[Test]
		public void When_item_does_not_exists_will_block_until_item_arrives()
		{
			var queue = new BlockingQueue<string>();
			DateTime start = DateTime.MaxValue;
			ThreadPool.QueueUserWorkItem(state =>
			{
				Thread.Sleep(100);
				start = DateTime.Now;
				queue.Enqueue("foo");
			});
			string t;
			queue.Dequeue(TimeSpan.FromSeconds(5), out t);
			Assert.AreEqual("foo", t);
			Assert.LowerEqualThan(start, DateTime.Now);
		}
	}
}