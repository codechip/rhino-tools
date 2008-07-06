using System;
using System.IO;
using MbUnit.Framework;

namespace BerkeleyDb.Tests
{
	[TestFixture]
	public class VerificationTests
	{
		[SetUp]
		public void Setup()
		{

			if (Directory.Exists("test"))
				Directory.Delete("test", true);
			Directory.CreateDirectory("test");
		}

		[Test]
		public void Can_use_database_in_more_than_one_transaction()
		{
			Env env = new Env(EnvCreateFlags.None);
			env.ErrorCall=delegate(Env env1, string errpfx, string msg)
			{
				Console.WriteLine(errpfx + ": " + msg);
			};
			env.Open("test", BerkeleyDbEnvironment.CreationFlags, 0);
			var database = env.CreateDatabase(DbCreateFlags.None);
			var txn = env.TxnBegin(null, Txn.BeginFlags.ReadCommitted);
			database.RecLen = 16;
			var queue = (DbQueue)database.Open(txn, "foo", null, DbType.Queue, Db.OpenFlags.Create, 0);
			DbEntry key = DbEntry.InOut(new byte[4]);
			Guid id = Guid.NewGuid();
			DbEntry val = DbEntry.InOut(id.ToByteArray());
			queue.Append(txn, ref key, ref val);
			txn.Commit(Txn.CommitMode.None);
			txn.Dispose();

			txn = env.TxnBegin(null, Txn.BeginFlags.ReadCommitted);
			val = DbEntry.InOut(new byte[16]);
			queue.Consume(txn, ref key, ref val, DbFile.ReadFlags.None, false);
			txn.Commit(Txn.CommitMode.None);
			txn.Dispose();
			env.Dispose();
			Assert.AreEqual(id, new Guid(val.Buffer));

		}
	}
}