using System.Data.SQLite;
using System.Threading;
using System.Transactions;
using MbUnit.Framework;

namespace Rhino.Queues.Tests
{
	[TestFixture]
	public class Sqlite_can_work_with_system_transactions
	{
		[SetUp]
		public void Setup()
		{
			CreateDatabase("test1");
			CreateDatabase("test2");
		}

		private void CreateDatabase(string db)
		{
			using (var connection = new SQLiteConnection("data source=" + db + ".db;New=true"))
			{
				connection.Open();
				using (var cmd = connection.CreateCommand())
				{
					cmd.CommandText = @"
DROP TABLE IF EXISTS Test;
CREATE TABLE Test( a nvarchar(255) );
";
					cmd.ExecuteNonQuery();
				}
			}
		}

		[Test]
		public void When_executing_command_and_committing_sys_transactions_will_save_to_db()
		{
			using (var tx = new TransactionScope())
			{
				InsertRow("test1");
				InsertRow("test2");
				tx.Complete();
			}

			Assert.AreEqual(1, GetRowCount("test1"));
			Assert.AreEqual(1, GetRowCount("test2"));

		}

		[Test]
		public void When_executing_command_and_rolling_back_sys_transactions_will_not_save_to_db()
		{
			using (new TransactionScope())
			{
				InsertRow("test1");
				InsertRow("test2");
				Transaction.Current.Rollback();
			}

			Assert.AreEqual(0, GetRowCount("test1"));
			Assert.AreEqual(0, GetRowCount("test2"));
		}

		[Test]
		public void When_executing_command_and_not_committing_sys_transactions_will_not_save_to_db()
		{
			using (new TransactionScope())
			{
				InsertRow("test1");
				InsertRow("test2");
			}

			Assert.AreEqual(0, GetRowCount("test1"));
			Assert.AreEqual(0, GetRowCount("test2"));
		}

		[Test]
		public void When_executing_command_and_but_before_committing_changes_are_not_visible_to_outside_world()
		{
			using (var tx = new TransactionScope())
			{
				InsertRow("test1");
				InsertRow("test2");
				var @event = new ManualResetEvent(false);
				var checkedOnAnotherThread = false;
				ThreadPool.QueueUserWorkItem(delegate
				{
					Assert.AreEqual(0, GetRowCount("test1"));
					Assert.AreEqual(0, GetRowCount("test2"));
					checkedOnAnotherThread = true;
					@event.Set();
				});
				@event.WaitOne();
				Assert.IsTrue(checkedOnAnotherThread);
				tx.Complete();
			}
		}



		private object GetRowCount(string db)
		{
			object actual;
			using (var connection = new SQLiteConnection("data source=" + db + ".db;New=true"))
			{
				connection.Open();
				using (var cmd = connection.CreateCommand())
				{
					cmd.CommandText = @"
SELECT COUNT(*) FROM Test
";
					actual = cmd.ExecuteScalar();
				}
			}
			return actual;
		}

		private void InsertRow(string db)
		{
			using (var connection = new SQLiteConnection("data source=" + db + ".db;"))
			{
				connection.Open();
				using (var cmd = connection.CreateCommand())
				{
					cmd.CommandText = @"
INSERT INTO Test (a) VALUES('blah');
";
					cmd.ExecuteNonQuery();
				}
			}
		}
	}
}