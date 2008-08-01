// Copyright (c) 2005 - 2008 Ayende Rahien (ayende@ayende.com)
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
namespace Rhino.Queues.Storage.Disk
{
	using System;
	using System.Collections.Generic;
	using System.IO;
	using System.Linq;

	public class PersistentQueue : IPersistentQueueImpl
	{
		private readonly HashSet<Entry> checkedOutEntries = new HashSet<Entry>();

		private readonly Dictionary<int, int> countOfItemsPerFile =
			new Dictionary<int, int>();

		private readonly LinkedList<Entry> entries = new LinkedList<Entry>();

		private readonly string path;

		private readonly object transactionLogLock = new object();
		private readonly object writerLock = new object();
		private FileStream fileLock;

		public int SuggestedReadBuffer { get; set; }
		public int SuggestedWriteBuffer { get; set; }
		public long SuggestedMaxTransactionLogSize { get; set; }

		public PersistentQueue(string path, int maxFileSize)
		{
			SuggestedMaxTransactionLogSize = 32 * 1024 * 1024;
			SuggestedReadBuffer = 1024 * 1024;
			SuggestedWriteBuffer = 1024 * 1024;

			this.path = Path.GetFullPath(path);
			MaxFileSize = maxFileSize;
			try
			{
				fileLock = new FileStream(Path.Combine(path, "lock"), FileMode.Create, FileAccess.ReadWrite, FileShare.None);
			}
			catch (IOException)
			{
				GC.SuppressFinalize(this); //avoid finalizing invalid instance
				throw new InvalidOperationException("Another instance of the queue is already in action");
			}

			try
			{
				ReadMetaState();
				ReadTransactionLog();
			}
			catch (Exception)
			{
				GC.SuppressFinalize(this); //avoid finalizing invalid instance
				fileLock.Dispose();
				throw;
			}
		}

		public PersistentQueue(string path)
			: this(path, 32 * 1024 * 1024)
		{
			this.path = path;
		}

		/// <summary>
		/// Gets the estimated count of items in queue.
		/// This is estimated number because it is not synced across threads
		/// </summary>
		/// <value>The estimated count of items in queue.</value>
		public int EstimatedCountOfItemsInQueue
		{
			get { return entries.Count + checkedOutEntries.Count; }
		}

		private int CurrentCountOfItemsInQueue
		{
			get
			{
				lock (entries)
				{
					return entries.Count + checkedOutEntries.Count;
				}
			}
		}

		public int MaxFileSize { get; private set; }
		public long CurrentFilePosition { get; private set; }

		private string TransactionLog
		{
			get { return Path.Combine(path, "transaction.log"); }
		}

		private string Meta
		{
			get { return Path.Combine(path, "meta.state"); }
		}

		#region IPersistentQueueImpl Members

		public int CurrentFileNumber { get; private set; }

		public void Dispose()
		{
			lock (transactionLogLock)
			{
				FlushTrimmedTransactionLog();
			}
			if (fileLock != null)
				fileLock.Dispose();
			fileLock = null;
			GC.SuppressFinalize(this);
		}

		public void AcquireWriter(
			Stream stream,
			Func<Stream, long> action,
			Action<Stream> onReplaceStream)
		{
			lock (writerLock)
			{
				if (stream.Position != CurrentFilePosition)
				{
					stream.Position = CurrentFilePosition;
				}
				CurrentFilePosition = action(stream);
				if (CurrentFilePosition < MaxFileSize)
					return;
				CurrentFileNumber += 1;
				var writer = CreateWriter();
				// we assume same size messages, or near size messages
				// that gives us a good heuroistic for creating the size of 
				// the new file, so it wouldn't be fragmented
				writer.SetLength(CurrentFilePosition);
				CurrentFilePosition = 0;
				onReplaceStream(writer);
			}
		}

		public void CommitTransaction(ICollection<Operation> operations)
		{
			if (operations.Count == 0)
				return;

			byte[] transactionBuffer = GenerateTransactionBuffer(operations);

			lock (transactionLogLock)
			{
				long txLogSize;
				using (
					var stream = new FileStream(TransactionLog,
												FileMode.Append,
												FileAccess.Write,
												FileShare.None,
												transactionBuffer.Length,
												FileOptions.SequentialScan | FileOptions.WriteThrough)
					)
				{
					stream.Write(transactionBuffer, 0, transactionBuffer.Length);
					txLogSize = stream.Position;
					stream.Flush();
				}

				ApplyTransactionOperations(operations);
				TrimTransactionLogIfNeeded(txLogSize);

				Atomic.Write(Meta, stream =>
				{
					var bytes = BitConverter.GetBytes(CurrentFileNumber);
					stream.Write(bytes, 0, bytes.Length);
					bytes = BitConverter.GetBytes(CurrentFilePosition);
					stream.Write(bytes, 0, bytes.Length);
				});
			}
		}

		public Entry Dequeue()
		{
			lock (entries)
			{
				var first = entries.First;
				if (first == null)
					return null;
				if (first.Value.Data == null)
				{
					ReadAhead();
				}
				entries.RemoveFirst();
				checkedOutEntries.Add(first.Value);
				return first.Value;
			}
		}

		/// <summary>
		/// Assumes that entries has at least one entry
		/// </summary>
		private void ReadAhead()
		{
			long currentBufferSize = 0;
			var firstEntry = entries.First.Value;
			Entry lastEntry = firstEntry;
			foreach (var entry in entries)
			{
				// we can't read ahead to another file or
				// if we have unordered queue
				if (entry.FileNumber != lastEntry.FileNumber ||
					entry.Start < lastEntry.Start)
					break;
				if (currentBufferSize + entry.Length > SuggestedReadBuffer)
					break;
				lastEntry = entry;
				currentBufferSize += entry.Length;
			}
			if (lastEntry == firstEntry)
				currentBufferSize = lastEntry.Length;

			byte[] buffer = ReadEntriesFromFile(firstEntry, currentBufferSize);

			var index = 0;
			foreach (var entry in entries)
			{
				entry.Data = new byte[entry.Length];
				Buffer.BlockCopy(buffer, index, entry.Data, 0, entry.Length);
				index += entry.Length;
				if (entry == lastEntry)
					break;
			}
		}

		private byte[] ReadEntriesFromFile(Entry firstEntry, long currentBufferSize)
		{
			var buffer = new byte[currentBufferSize];
			using (var reader = new FileStream(GetDataPath(firstEntry.FileNumber),
											   FileMode.OpenOrCreate, FileAccess.Read, FileShare.ReadWrite))
			{
				reader.Position = firstEntry.Start;
				var totalRead = 0;
				do
				{
					var bytesRead = reader.Read(buffer, totalRead, buffer.Length - totalRead);
					if (bytesRead == 0)
						throw new InvalidOperationException("End of file reached while trying to read queue item");
					totalRead += bytesRead;
				} while (totalRead < buffer.Length);
			}
			return buffer;
		}

		public IPersistentQueueSession OpenSession()
		{
			return new PersistentQueueSession(this, CreateWriter(), SuggestedWriteBuffer);
		}

		public void Reinstate(IEnumerable<Operation> reinstatedOperations)
		{
			lock (entries)
			{
				ApplyTransactionOperations(
					from entry in reinstatedOperations
					where entry.Type == OperationType.Dequeue
					select new Operation(
						OperationType.Reinstate,
						entry.FileNumber,
						entry.Start,
						entry.Length,
						entry.Data
						)
					);
			}
		}

		#endregion

		private void ReadTransactionLog()
		{
			var requireTxLogTrimming = false;
			Atomic.Read(TransactionLog, stream =>
			{
				using (var binaryReader = new BinaryReader(stream))
				{
					try
					{
						while (true)
						{
							// this code ensures that we read the full transaction
							// before we start to apply it. The last truncated transaction will be
							// ignored automatically.
							AssertTransactionSeperator(binaryReader);
							var opsCount = binaryReader.ReadInt32();
							var txOps = new List<Operation>(opsCount);
							for (var i = 0; i < opsCount; i++)
							{
								AssertOperationSeparator(binaryReader);
								var operation = new Operation(
									(OperationType)binaryReader.ReadByte(),
									binaryReader.ReadInt32(),
									binaryReader.ReadInt32(),
									binaryReader.ReadInt32(),
									null
								);
								txOps.Add(operation);
								//if we have non enqueue entries, this means 
								// that we have not closed properly, so we need
								// to trim the log
								if (operation.Type != OperationType.Enqueue)
									requireTxLogTrimming = true;
							}
							ApplyTransactionOperations(txOps);
						}
					}
					catch (EndOfStreamException)
					{
						return;
					}
				}
			});
			if (requireTxLogTrimming)
				FlushTrimmedTransactionLog();
		}

		private void FlushTrimmedTransactionLog()
		{
			byte[] transactionBuffer;
			using (var ms = new MemoryStream())
			{
				ms.Write(Constants.TransactionSeparator, 0, Constants.TransactionSeparator.Length);

				var count = BitConverter.GetBytes(EstimatedCountOfItemsInQueue);
				ms.Write(count, 0, count.Length);

				foreach (var entry in checkedOutEntries)
				{
					WriteEntryToTransactionLog(ms, entry, OperationType.Enqueue);
				}
				foreach (var entry in entries)
				{
					WriteEntryToTransactionLog(ms, entry, OperationType.Enqueue);
				}
				ms.Flush();
				transactionBuffer = ms.ToArray();
			}
			Atomic.Write(TransactionLog, stream =>
			{
				stream.SetLength(transactionBuffer.Length);
				stream.Write(transactionBuffer, 0, transactionBuffer.Length);
			});
		}

		private static void WriteEntryToTransactionLog(Stream ms, Entry entry, OperationType operationType)
		{
			ms.Write(Constants.OperationSeparatorBytes, 0, Constants.OperationSeparatorBytes.Length);

			ms.WriteByte((byte)operationType);

			var fileNumber = BitConverter.GetBytes(entry.FileNumber);
			ms.Write(fileNumber, 0, fileNumber.Length);

			var start = BitConverter.GetBytes(entry.Start);
			ms.Write(start, 0, start.Length);

			var length = BitConverter.GetBytes(entry.Length);
			ms.Write(length, 0, length.Length);
		}

		private static void AssertOperationSeparator(BinaryReader reader)
		{
			var separator = reader.ReadInt32();
			if (separator != Constants.OperationSeparator)
				throw new InvalidOperationException(
					"Unexpected data in transaction log. Expected to get transaction separator but got unknonwn data");
		}

		public int[] ApplyTransactionOperationsInMemory(IEnumerable<Operation> operations)
		{
			var filesToRemove = new List<int>();
			foreach (var operation in operations)
			{
				switch (operation.Type)
				{
					case OperationType.Enqueue:
						var entryToAdd = new Entry(operation);
						entries.AddLast(entryToAdd);
						var itemCountAddition = countOfItemsPerFile.GetValueOrDefault(entryToAdd.FileNumber);
						countOfItemsPerFile[entryToAdd.FileNumber] = itemCountAddition + 1;
						break;

					case OperationType.Dequeue:
						var entryToRemove = new Entry(operation);
						checkedOutEntries.Remove(entryToRemove);
						var itemCountRemoval = countOfItemsPerFile.GetValueOrDefault(entryToRemove.FileNumber);
						countOfItemsPerFile[entryToRemove.FileNumber] = itemCountRemoval - 1;
						if (itemCountRemoval == 1)
						{
							countOfItemsPerFile.Remove(entryToRemove.FileNumber);
							filesToRemove.Add(entryToRemove.FileNumber);
						}
						break;

					case OperationType.Reinstate:
						var entryToReistate = new Entry(operation);
						entries.AddFirst(entryToReistate);
						checkedOutEntries.Remove(entryToReistate);
						break;
				}
			}
			return filesToRemove.ToArray();
		}

		private static void AssertTransactionSeperator(BinaryReader binaryReader)
		{
			var bytes = binaryReader.ReadBytes(16);
			if (bytes.Length == 0)
				throw new EndOfStreamException();
			if (bytes.Length != 16)
				throw new InvalidOperationException(
					"Unexpected data in transaction log. Expected to get transaction separator but got truncated data");
			var separator = new Guid(bytes);
			if (separator != Constants.TransactionSeparatorGuid)
				throw new InvalidOperationException(
					"Unexpected data in transaction log. Expected to get transaction separator but got unknonwn data");
		}

		private void ReadMetaState()
		{
			Atomic.Read(Meta, stream =>
			{
				using (var binaryReader = new BinaryReader(stream))
				{
					try
					{
						CurrentFileNumber = binaryReader.ReadInt32();
						CurrentFilePosition = binaryReader.ReadInt64();
					}
					catch (EndOfStreamException)
					{
					}
				}
			});
		}


		private void TrimTransactionLogIfNeeded(long txLogSize)
		{
			if (txLogSize < SuggestedMaxTransactionLogSize)// it is not big enough to care
				return;
			var optimalSize = GetOptimalTransactionLogSize();
			if (txLogSize < (optimalSize * 2))   // not enough disparity to bother trimming
				return;
			FlushTrimmedTransactionLog();
		}

		private void ApplyTransactionOperations(IEnumerable<Operation> operations)
		{
			int[] filesToRemove;
			lock (entries)
			{
				filesToRemove = ApplyTransactionOperationsInMemory(operations);
			}
			foreach (var fileNumber in filesToRemove)
			{
				if (CurrentFileNumber == fileNumber)
					continue;
				File.Delete(GetDataPath(fileNumber));
			}
		}

		private static byte[] GenerateTransactionBuffer(ICollection<Operation> operations)
		{
			byte[] transactionBuffer;
			using (var ms = new MemoryStream())
			{
				ms.Write(Constants.TransactionSeparator, 0, Constants.TransactionSeparator.Length);

				var count = BitConverter.GetBytes(operations.Count);
				ms.Write(count, 0, count.Length);

				foreach (var operation in operations)
				{
					WriteEntryToTransactionLog(ms, new Entry(operation), operation.Type);
				}
				ms.Flush();
				transactionBuffer = ms.ToArray();
			}
			return transactionBuffer;
		}

		private FileStream CreateWriter()
		{
			return new FileStream(
				GetDataPath(CurrentFileNumber),
				FileMode.OpenOrCreate,
				FileAccess.Write,
				FileShare.ReadWrite,
				0x10000,
				FileOptions.Asynchronous | FileOptions.SequentialScan | FileOptions.WriteThrough);
		}

		public string GetDataPath(int index)
		{
			return Path.Combine(path, "data." + index);
		}

		~PersistentQueue()
		{
			Dispose();
		}

		private long GetOptimalTransactionLogSize()
		{
			long size = 0;
			size += 16 /*sizeof(guid)*/; //	initial tx separator
			size += sizeof(int); // 	operation count

			size +=
				( // entry size == 16
				sizeof(int) + // 		operation separator
				sizeof(int) + // 		file number
				sizeof(int) + //		start
				sizeof(int) //		length
				)
				*
				(CurrentCountOfItemsInQueue);

			return size;
		}
	}
}