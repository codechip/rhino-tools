using System;
using System.Collections;
using System.Runtime.InteropServices;
using System.Collections.Generic;

namespace BerkeleyDb
{
	/// <summary>
	/// Represent a queue in the environment. 
	/// Not thread safe.
	/// </summary>
	public class BerkeleyDbQueue : SerializationAndDisposableMiixin
	{
		private static readonly int sizeOfInt = Marshal.SizeOf(typeof(int));
		private readonly Db database;
		private readonly byte[] dataBuffer;
		private readonly byte[] keyBuffer;
		private readonly DbQueue queue;
		private readonly BerkeleyDbTransaction transaction;

		public BerkeleyDbQueue(BerkeleyDbTransaction transaction, Db database, DbQueue queue)
		{
			this.transaction = transaction;
			this.database = database;
			this.queue = queue;
			dataBuffer = new byte[database.RecLen];
			keyBuffer = new byte[sizeOfInt];
		}

		public string Name
		{
			get
			{
				string file;
				string databaseName;
				queue.GetName(out file, out databaseName);
				return file;
			}
		}

		public int MaxItemSize
		{
			get { return database.RecLen; }
		}

		public DbFile Inner
		{
			get { return queue; }
		}

		protected override void Dispose(bool disposing)
		{
			if (transaction == null)
				database.Dispose();
			else
				transaction.RegisterDisposeSyncronization(database.Close);
		}

		public void Append(object data)
		{
			ClearBuffers();

			Serialize(data).To(dataBuffer);

			DbEntry keyEntry = DbEntry.InOut(keyBuffer);

			DbEntry dataEntry = DbEntry.InOut(dataBuffer);
			queue.Append(transaction.InnerTransaction(), ref keyEntry, ref dataEntry);
			BitConverter.ToInt32(keyEntry.Buffer, 0);
		}

		public void AppendBytes(byte[] data)
		{
			ClearBuffers();

			DbEntry keyEntry = DbEntry.InOut(keyBuffer);
			var length = BitConverter.GetBytes(data.Length);
			var buffer = new byte[data.Length + length.Length];
			Buffer.BlockCopy(length, 0, buffer, 0, length.Length);
			Buffer.BlockCopy(data, 0, buffer, length.Length, data.Length);
			DbEntry dataEntry = DbEntry.InOut(buffer);

			queue.Append(transaction.InnerTransaction(), ref keyEntry, ref dataEntry);
			BitConverter.ToInt32(keyEntry.Buffer, 0);
		}

		private void ClearBuffers()
		{
			Array.Clear(keyBuffer, 0, keyBuffer.Length);
			Array.Clear(dataBuffer, 0, dataBuffer.Length);
		}

		public object Consume()
		{
			var entry = ConsumeInternal(false);
			if (entry == null)
				return null;
			return Deserialize.From(entry.Value.Buffer);
		}

		private DbEntry? ConsumeInternal(bool shouldWait)
		{
			ClearBuffers();
			DbEntry key = DbEntry.InOut(keyBuffer);
			DbEntry data = DbEntry.InOut(dataBuffer);
			var readStatus = queue.Consume(transaction.InnerTransaction(), ref key, ref data,
										   DbFile.ReadFlags.None, shouldWait);
			if (readStatus == ReadStatus.NotFound)
				return null;
			return data;
		}

		public object ConsumeWithWait()
		{
			var entry = ConsumeInternal(true);
			if (entry == null)
				return null;
			return Deserialize.From(entry.Value.Buffer);

		}

		public byte[] ConsumeBytes()
		{
			return ConsumeBytesInternal(false);
		}

		public byte[] ConsumeBytesWithWait()
		{
			return ConsumeBytesInternal(true);
		}

		private byte[] ConsumeBytesInternal(bool shouldWait)
		{
			var entry = ConsumeInternal(shouldWait);
			if (entry == null)
				return null;
			var length = BitConverter.ToInt32(entry.Value.Buffer, 0);
			var buffer = new byte[length];
			Buffer.BlockCopy(entry.Value.Buffer, 4, buffer, 0, length);
			return buffer;
		}

		public void Truncate()
		{
			queue.Truncate(transaction.InnerTransaction());
		}

		public IEnumerable Select()
		{
			return Select(o => true);
		}


		public IEnumerable<T> Select<T>()
		{
			foreach (var msg in Select(o => true))
			{
				yield return (T)msg;
			}
		}

		public IEnumerable Select(Predicate<object> predicate)
		{
			return SelectInternal(x => { }, predicate);
		}

		private IEnumerable SelectInternal(Action<DbQueueCursor> actOnCursor, Predicate<object> predicate)
		{
			var cursor = queue.OpenCursor(transaction.InnerTransaction(), DbFileCursor.CreateFlags.ReadCommitted);
			try
			{
				DbEntry key = DbEntry.InOut(keyBuffer);
				DbEntry data = DbEntry.InOut(dataBuffer);

				var status = cursor.Get(ref key, ref data, DbFileCursor.GetMode.First, DbFileCursor.ReadFlags.ReadUncommitted);
				if (status == ReadStatus.NotFound)
					yield break;
				do
				{
					if(status!=ReadStatus.KeyEmpty)
					{
						var result = Deserialize.From(data.Buffer);
						if (predicate(result))
						{
							actOnCursor(cursor);
							yield return result;
						}
					}
					status = cursor.Get(ref key, ref data, DbFileCursor.GetMode.Next, DbFileCursor.ReadFlags.ReadUncommitted);
				} while (status != ReadStatus.NotFound);
			}
			finally
			{
				cursor.Dispose();
			}
		}

		public IEnumerable SelectAndConsume()
		{
			return SelectAndConsume(o => true);
		}

		public IEnumerable SelectAndConsume(Predicate<object> predicate)
		{
			return SelectInternal(x => x.Delete(), predicate);
		}

		public IEnumerable<T> SelectAndConsume<T>()
		{
			return SelectAndConsume<T>(x => true);
		}

		public IEnumerable<T> SelectAndConsume<T>(Predicate<T> predicate)
		{
			foreach (var item in SelectInternal(x => x.Delete(), o => predicate((T)o)))
			{
				yield return (T)item;
			}
		}

		public IEnumerable<T> SelectAndConsumeFromAssociation<T>(BerkeleyDbTree tree)
					where T : class
		{
			return SelectAndConsumeFromAssociation<T>(tree, x => true, (c, t, k) =>
			{
				c.Delete();
				t.Delete(k);
			});
		}

		public IEnumerable<T> SelectAndConsumeFromAssociation<T>(BerkeleyDbTree tree, Func<T, bool> predicate) 
			where T : class
		{
			return SelectAndConsumeFromAssociation<T>(tree, predicate, (c, t, k) =>
			{
				c.Delete();
				t.Delete(k);
			});
		}

		private IEnumerable<T> SelectAndConsumeFromAssociation<T>(BerkeleyDbTree tree, Func<T, bool> predicate, Action<DbQueueCursor, BerkeleyDbTree, Guid> actOnSelection) 
			where T : class
		{
			var cursor = queue.OpenCursor(transaction.InnerTransaction(), DbFileCursor.CreateFlags.ReadCommitted);
			try
			{
				DbEntry key = DbEntry.InOut(keyBuffer);
				DbEntry data = DbEntry.InOut(dataBuffer);

				var status = cursor.Get(ref key, ref data, DbFileCursor.GetMode.First, DbFileCursor.ReadFlags.ReadUncommitted);
				if (status != ReadStatus.Success)
					yield break;
				do
				{
					var keyInTree = (Guid)Deserialize.From(data.Buffer);
					var value = (T)tree.Get(keyInTree);
					if(value == null) // probably index out of sync
					{
						cursor.Delete();
					}
					if (predicate(value))
					{
						actOnSelection(cursor, tree, keyInTree);
						yield return value;
					}
					status = cursor.Get(ref key, ref data, DbFileCursor.GetMode.Next, DbFileCursor.ReadFlags.ReadUncommitted);
				} while (status == ReadStatus.Success);
			}
			finally
			{
				cursor.Dispose();
			}
		}

		public Guid AppendAssociation(BerkeleyDbTree tree, object value)
		{
			var id = SequentialGuid.Next();
			tree.Put(id, value);
			Append(id);
			return id;
		}

		public IEnumerable<T> SelectFromAssociation<T>(BerkeleyDbTree tree) where T : class
		{
			return SelectAndConsumeFromAssociation<T>(tree, x => true, (c, t, k) =>
			{
			});
		}
	}
}