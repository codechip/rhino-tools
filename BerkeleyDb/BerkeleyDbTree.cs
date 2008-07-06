using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace BerkeleyDb
{
	public class BerkeleyDbTree : SerializationAndDisposableMiixin
	{
		private readonly BerkeleyDbTransaction transaction;
		private readonly Db database;
		private readonly DbBTree tree;
		byte[] buffer = new byte[0];

		public DbBTree Inner
		{
			get
			{
				return tree;
			}
		}

		public BerkeleyDbTree(BerkeleyDbTransaction transaction, Db database, DbBTree tree)
		{
			this.transaction = transaction;
			this.database = database;
			this.tree = tree;
		}

		public void Put(object key, object value)
		{
			PutInternal(Serialize(key).ToBuffer(), value);
		}


		private void PutInternal(byte[] keyBytes, object value)
		{
			var bytes = Serialize(value).ToBuffer();
			DbEntry key = DbEntry.InOut(keyBytes);
			DbEntry data = DbEntry.InOut(bytes);

			tree.Put(transaction.InnerTransaction(), ref key, ref data);
		}

		protected override void Dispose(bool disposing)
		{
			if (transaction == null)
				database.Dispose();
			else
				transaction.RegisterDisposeSyncronization(database.Close);
		}

		private object GetInternal(byte[] keyBytes)
		{
			DbEntry key = DbEntry.InOut(keyBytes);
			DbEntry value = DbEntry.InOut(buffer);

			var status = tree.Get(transaction.InnerTransaction(),
								  ref key, ref value, DbFile.ReadFlags.ReadCommitted
				);
			if (status == ReadStatus.BufferSmall)
			{
				buffer = new byte[value.Size];
				value = DbEntry.InOut(buffer);
				status = tree.Get(transaction.InnerTransaction(),
								  ref key, ref value, DbFile.ReadFlags.ReadCommitted
				);
			}
			if (status == ReadStatus.NotFound)
				return null;
			return Deserialize.From(buffer);
		}

		public void Truncate()
		{
			tree.Truncate(transaction.InnerTransaction());
		}
		public void Delete(object key)
		{
			DeleteInternal(Serialize(key).ToBuffer());
		}

		private void DeleteInternal(byte[] keyBytes)
		{
			var key = DbEntry.InOut(keyBytes);
			tree.Delete(transaction.InnerTransaction(), ref key);
		}

		public object Get(object key)
		{
			return GetInternal(Serialize(key).ToBuffer());
		}

		public IEnumerable<DictionaryEntry> SelectAndConsume()
		{
			return SelectInternal(x => x.Delete());
		}

		private IEnumerable<DictionaryEntry> SelectInternal(Action<DbBTreeCursor> actOnCursor)
		{
			var cursor = tree.OpenCursor(transaction.InnerTransaction(), DbFileCursor.CreateFlags.ReadCommitted);
			try
			{
				var keyBuffer = new byte[0];
				var dataBuffer = new byte[0];
				DbEntry key = DbEntry.InOut(keyBuffer);
				DbEntry data = DbEntry.InOut(dataBuffer);
				DbEntry previousKey = DbEntry.EmptyOut();

				ReadStatus status = cursor.Get(ref key, ref data,
											   DbFileCursor.GetMode.First,
											   DbFileCursor.ReadFlags.ReadUncommitted);
				if (status == ReadStatus.BufferSmall)
				{
					AdjustBuffers(keyBuffer, dataBuffer, ref key, ref data);
					status = cursor.Get(ref key, ref data,
										DbFileCursor.GetMode.First,
										DbFileCursor.ReadFlags.None);
				}
				previousKey = DbEntry.InOut((byte[])key.Buffer.Clone());
				while (status != ReadStatus.NotFound)
				{
					if (status == ReadStatus.BufferSmall)
					{
						cursor.GetAt(ref previousKey, ref data, DbFileCursor.GetAtMode.Set, DbFileCursor.ReadFlags.None);
						AdjustBuffers(keyBuffer, dataBuffer, ref key, ref data);
					}
					else if (status == ReadStatus.Success)
					{
						actOnCursor(cursor);
						yield return new DictionaryEntry(
							Deserialize.From(key.Buffer),
							Deserialize.From(data.Buffer)
							);
					}
					previousKey = DbEntry.InOut((byte[])key.Buffer.Clone());
					status = cursor.Get(ref key, ref data,
										DbFileCursor.GetMode.Next,
										DbFileCursor.ReadFlags.ReadUncommitted);
				}
			}
			finally
			{
				cursor.Dispose();
			}
		}

		private static void AdjustBuffers(byte[] keyBuffer, byte[] dataBuffer, ref DbEntry key, ref DbEntry data)
		{
			if (keyBuffer.Length < key.Size)
				keyBuffer = new byte[key.Size];
			if (dataBuffer.Length < data.Size)
				dataBuffer = new byte[data.Size];
			key = DbEntry.InOut(keyBuffer);
			data = DbEntry.InOut(dataBuffer);
		}

		public IEnumerable<DictionaryEntry> Select()
		{
			return SelectInternal(x => { });
		}
	}
}