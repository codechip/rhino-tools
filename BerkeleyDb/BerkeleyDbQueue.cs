using System;
using System.Runtime.InteropServices;

namespace BerkeleyDb
{
	/// <summary>
	/// Represent a queue in the environment. 
	/// Not thread safe.
	/// </summary>
	public class BerkeleyDbQueue : DisposableMixin
	{
		private static readonly int sizeOfInt = Marshal.SizeOf(typeof (int));
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

		private static FluentSerializer Deserialize
		{
			get { return new FluentSerializer(null); }
		}

		protected override void Dispose(bool disposing)
		{
			if (transaction == null)
				database.Dispose();
			else
				transaction.RegisterSyncronization(database.Close);
		}

		public int Append(object data)
		{
			ClearBuffers();

			DbEntry keyEntry = DbEntry.InOut(keyBuffer);
			Serialize(data).To(dataBuffer);
			DbEntry dataEntry = DbEntry.InOut(dataBuffer);
			queue.Append(transaction.InnerTransaction(), ref keyEntry, ref dataEntry);
			return BitConverter.ToInt32(keyEntry.Buffer, 0);
		}

		private void ClearBuffers()
		{
			Array.Clear(keyBuffer, 0, keyBuffer.Length);
			Array.Clear(dataBuffer, 0, dataBuffer.Length);
		}

		public object Consume()
		{
			return ConsumeInternal(false);
		}

		private object ConsumeInternal(bool shouldWait)
		{
			ClearBuffers();
			DbEntry key = DbEntry.InOut(keyBuffer);
			DbEntry data = DbEntry.InOut(dataBuffer);
			var readStatus = queue.Consume(transaction.InnerTransaction(), ref key, ref data,
			                               DbFile.ReadFlags.None, shouldWait);
			if (readStatus == ReadStatus.NotFound)
				return null;
			return Deserialize.From(data.Buffer);
		}

		private static FluentSerializer Serialize(object o)
		{
			return new FluentSerializer(o);
		}

		public object ConsumeWithWait()
		{
			return ConsumeInternal(true);
		}
	}
}