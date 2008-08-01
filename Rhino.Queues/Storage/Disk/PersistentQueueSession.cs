namespace Rhino.Queues.Storage.Disk
{
	using System;
	using System.Collections.Generic;
	using System.IO;
	using System.Linq;
	using System.Threading;
	using System.Transactions;

	public class PersistentQueueSession : ISinglePhaseNotification, IPersistentQueueSession
	{
		private bool transactionFinished;
		private readonly List<Operation> operations = new List<Operation>();
		private readonly IList<Exception> pendingWritesFailures = new List<Exception>();
		private readonly IList<WaitHandle> pendingWritesHandles = new List<WaitHandle>();
		private Stream currentStream;
		private readonly int writeBufferSize;
		private readonly IPersistentQueueImpl queue;
		private readonly List<Stream> streamsToDisposeOnFlush = new List<Stream>();

		private Transaction currentTransaction;

		private readonly List<byte[]> buffer = new List<byte[]>();
		private int bufferSize;

		private const int MinSizeThatMakeAsyncWritePractical = 64 * 1024;

		public PersistentQueueSession(IPersistentQueueImpl queue, Stream currentStream, int writeBufferSize)
		{
			this.queue = queue;
			this.currentStream = currentStream;
			if (writeBufferSize < MinSizeThatMakeAsyncWritePractical)
				writeBufferSize = MinSizeThatMakeAsyncWritePractical;
			this.writeBufferSize = writeBufferSize;
		}

		private void TryJoinTransaction()
		{
			if (currentTransaction != null)
				return;// already in transaction
			if (Transaction.Current != null)
			{
				currentTransaction = Transaction.Current;
				currentTransaction.EnlistVolatile(this, EnlistmentOptions.None);
				return;
			}
			throw new InvalidOperationException("Cannot use session without a transaction");
		}

		public bool IsUsable
		{
			get { return transactionFinished == false; }
		}

		public void Enqueue(byte[] data)
		{
			TryJoinTransaction();
			buffer.Add(data);
			bufferSize += data.Length;
			if (bufferSize > writeBufferSize)
			{
				AsyncFlushBuffer();
			}
		}

		private void AsyncFlushBuffer()
		{
			queue.AcquireWriter(currentStream, AsyncWriteToStream, OnReplaceStream);
		}

		private void SyncFlushBuffer()
		{
			queue.AcquireWriter(currentStream, stream =>
			{
				byte[] data = ConcatenateBufferAndAddIndividualOperations(stream);
				stream.Write(data, 0, data.Length);
				return stream.Position;
			}, OnReplaceStream);
		}

		private long AsyncWriteToStream(Stream stream)
		{
			byte[] data = ConcatenateBufferAndAddIndividualOperations(stream);
			var resetEvent = new ManualResetEvent(false);
			pendingWritesHandles.Add(resetEvent);
			long positionAfterWrite = stream.Position + data.Length;
			stream.BeginWrite(data, 0, data.Length, delegate(IAsyncResult ar)
			{
				try
				{
					stream.EndWrite(ar);
				}
				catch (Exception e)
				{
					lock (pendingWritesFailures)
					{
						pendingWritesFailures.Add(e);
					}
				}
				finally
				{
					resetEvent.Set();
				}
			}, null);
			return positionAfterWrite;
		}

		private byte[] ConcatenateBufferAndAddIndividualOperations(Stream stream)
		{
			var data = new byte[bufferSize];
			var start = (int)stream.Position;
			var index = 0;
			foreach (var bytes in buffer)
			{
				operations.Add(new Operation(
					OperationType.Enqueue,
					queue.CurrentFileNumber,
					start,
					bytes.Length,
					bytes
				));
				Buffer.BlockCopy(bytes, 0, data, index, bytes.Length);
				start += bytes.Length;
				index += bytes.Length;
			}
			bufferSize = 0;
			buffer.Clear();
			return data;
		}

		private void OnReplaceStream(Stream newStream)
		{
			streamsToDisposeOnFlush.Add(currentStream);
			currentStream = newStream;
		}

		public byte[] Dequeue()
		{
			Action reverse;
			return ReversibleDequeue(out reverse);
		}

		public byte[] ReversibleDequeue(out Action reverse)
		{
			TryJoinTransaction();
			reverse = delegate { };
			var entry = queue.Dequeue();
			if (entry == null)
				return null;
			var operation = new Operation(
				OperationType.Dequeue,
				entry.FileNumber,
				entry.Start,
				entry.Length,
				null
				);
			operations.Add(operation);
			reverse = delegate
			{
				operations.Remove(operation);
				queue.Requeue(new[] { operation });
			};
			return entry.Data;
		}

		public void Flush()
		{
			try
			{
				while (pendingWritesHandles.Count != 0)
				{
					var handles = pendingWritesHandles.Take(64).ToArray();
					foreach (var handle in handles)
					{
						pendingWritesHandles.Remove(handle);
					}
					WaitHandle.WaitAll(handles);
					foreach (var handle in handles)
					{
						handle.Close();
					}
					AssertNoPendingWritesFailures();
				}
				SyncFlushBuffer();
			}
			finally
			{
				foreach (var stream in streamsToDisposeOnFlush)
				{
					stream.Flush();
					stream.Dispose();
				}
				streamsToDisposeOnFlush.Clear();
			}
			currentStream.Flush();
			queue.CommitTransaction(operations);
			operations.Clear();
		}

		private void AssertNoPendingWritesFailures()
		{
			lock (pendingWritesFailures)
			{
				if (pendingWritesFailures.Count == 0)
					return;

				var array = pendingWritesFailures.ToArray();
				pendingWritesFailures.Clear();
				throw new PendingWriteException(array);
			}
		}

		public void Dispose()
		{
		}

		private void ActualDispose()
		{
			DisposeStreams();
			GC.SuppressFinalize(this);
		}

		private void DisposeStreams()
		{
			foreach (var stream in streamsToDisposeOnFlush)
			{
				stream.Dispose();
			}
			currentStream.Dispose();
		}

		private void Rollback()
		{
			queue.Reinstate(operations);
			operations.Clear();
		}

		~PersistentQueueSession()
		{
			Dispose();
		}

		public void Prepare(PreparingEnlistment preparingEnlistment)
		{
			preparingEnlistment.Prepared();
		}

		public void Commit(Enlistment enlistment)
		{
			Flush();
			enlistment.Done();
			ActualDispose();
			transactionFinished = true;
		}

		public void Rollback(Enlistment enlistment)
		{
			Rollback();
			enlistment.Done();
			ActualDispose();
			transactionFinished = true;
		}

		public void InDoubt(Enlistment enlistment)
		{
			Rollback(enlistment);
		}

		public void SinglePhaseCommit(SinglePhaseEnlistment singlePhaseEnlistment)
		{
			Flush();
			singlePhaseEnlistment.Committed();
			ActualDispose();
			transactionFinished = true;
		}
	}
}