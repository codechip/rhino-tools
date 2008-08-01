namespace Rhino.Queues.Tests.Storage.Disk
{
	using System;
	using System.IO;
	using System.Transactions;
	using MbUnit.Framework;
	using Mocks;
	using Rhino.Queues.Storage.Disk;

	[TestFixture]
	public class PersistentQueueSessionTests : PersistentQueueTestsBase
	{
		[Test]
		[ExpectedException(typeof(PendingWriteException), @"Error during pending writes:
 - Memory stream is not expandable.")]
		public void Errors_raised_during_pending_write_will_be_thrown_on_flush()
		{
			var limitedSizeStream = new MemoryStream(new byte[4]);
			var queueStub = MockRepository.GenerateStub<IPersistentQueueImpl>();
			queueStub.Stub(x => x.AcquireWriter(null, null, null))
				.IgnoreArguments()
				.Do(invocation =>
				{
					((Func<Stream, long>)invocation.Arguments[1])(limitedSizeStream);
				});
			using (var session = new PersistentQueueSession(queueStub, limitedSizeStream, 1024 * 1024))
			using (var tx = new TransactionScope())
			{
				session.Enqueue(new byte[64 * 1024 * 1024 + 1]);
				tx.Complete();
			}
		}

		[Test]
		[ExpectedException(typeof(NotSupportedException), @"Memory stream is not expandable.")]
		public void Errors_raised_during_flush_write_will_be_thrown_as_is()
		{
			var limitedSizeStream = new MemoryStream(new byte[4]);
			var queueStub = MockRepository.GenerateStub<IPersistentQueueImpl>();
			queueStub.Stub(x => x.AcquireWriter(null, null, null))
				.IgnoreArguments()
				.Do(invocation =>
				{
					((Func<Stream, long>)invocation.Arguments[1])(limitedSizeStream);
				});
			using (var session = new PersistentQueueSession(queueStub, limitedSizeStream, 1024 * 1024))
			using (var tx = new TransactionScope())
			{
				session.Enqueue(new byte[64]);
				tx.Complete();
			}
		}

		[Test]
		[ExpectedException(typeof(InvalidOperationException), "End of file reached while trying to read queue item")]
		public void If_data_stream_is_truncated_will_raise_error()
		{
			using (var queue = new PersistentQueue(path))
			using (var session = queue.OpenSession())
			using (var tx = new TransactionScope())
			{
				session.Enqueue(new byte[] { 1, 2, 3, 4 });
				tx.Complete();
			}
			using (var fs = new FileStream(Path.Combine(path, "data.0"), FileMode.Open))
			{
				fs.SetLength(2);//corrupt the file
			}
			using (var queue = new PersistentQueue(path))
			using (var session = queue.OpenSession())
			using (var tx = new TransactionScope())
			{
				session.Dequeue();
				tx.Complete();
			}
		}
	}
}