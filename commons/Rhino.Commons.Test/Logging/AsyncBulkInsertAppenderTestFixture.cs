namespace Rhino.Commons.Test.Logging
{
	using System;
	using System.Configuration;
	using System.Data;
	using System.Data.SqlClient;
	using System.IO;
	using System.Reflection;
	using System.Threading;
	using log4net;
	using log4net.Config;
	using MbUnit.Framework;

	[TestFixture]
	public class AsyncBulkInsertAppenderTestFixture
	{
		private SqlConnection connection;
		private SqlTransaction transaction;

		private const string createTable =
			@"
IF OBJECT_ID('test_logging') IS NOT NULL 
	DROP TABLE test_logging

CREATE TABLE test_logging
(
	id int identity,
	exception nvarchar(2000),
	msg nvarchar(2000),
	date datetime,
	logger nvarchar(255)
)
";

		[SetUp]
		public void TestInitialize()
		{
			using (Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("Rhino.Commons.Test.Logging.BulkInsert.config"))
			{
				XmlConfigurator.Configure(stream);
			}
			RecreateTable();
		}

		[TearDown]
		public void TestCleanup()
		{
			LogManager.ResetConfiguration();
		}

		[Test]
		public void CanWriteToDatabase()
		{
			ILog logger = LogManager.GetLogger(GetType());
			for (int i = 0; i < 1001; i++)
			{
				logger.Info(i);
			}
            int count = GetCount_TakeIntoAccountDelays();
			Assert.AreEqual(1001, count);
		}

		[Test]
		public void CanWriteToLogWithoutWaitingIfTableIsLocked()
		{
			OpenTransactionAndLockTable();

			ILog logger = LogManager.GetLogger(GetType());
			for (int i = 0; i < 1001; i++)
			{
				logger.Info(i);
			}

			// will never get here if we are blocked by the transaction
			DisposeTransaction();
		    int count = GetCount_TakeIntoAccountDelays();
		    Assert.AreEqual(1001, count);

		}

	    private int GetCount_TakeIntoAccountDelays()
	    {
	        int count = 0;
	        for (int i = 0; i < 5; i++)
	        {
	            Thread.Sleep(2500);
	            count = GetRowCount();
	            if(count!=0)
	                break;
	        }
	        return count;
	    }

	    private void RecreateTable()
		{
			ExecuteInDb(delegate(SqlCommand command)
			{
				command.CommandText = createTable;
				command.ExecuteNonQuery();
			});
		}

		public void DisposeTransaction()
		{
			transaction.Rollback();
			transaction.Dispose();
			connection.Dispose();
		}

		public void OpenTransactionAndLockTable()
		{
			string connectionString = ConfigurationManager.ConnectionStrings["Rhino.Commons.Test.Properties.Settings.TestDatabase"].ConnectionString;
			connection = new SqlConnection(connectionString);
			connection.Open();
			transaction = connection.BeginTransaction(IsolationLevel.Serializable);
			using (SqlCommand sqlCommand = this.connection.CreateCommand())
			{
				sqlCommand.Transaction = transaction;
				sqlCommand.CommandText = "SELECT * FROM test_logging";
				using (SqlDataReader reader = sqlCommand.ExecuteReader())
				{
					while (reader.Read())
					{
						object tmp = reader[0];
						Assert.IsNotNull(tmp);
					}
				}
			}
		}

		public void ExecuteInDb(Action<SqlCommand> command)
		{
			string connectionString = ConfigurationManager.ConnectionStrings["Rhino.Commons.Test.Properties.Settings.TestDatabase"].ConnectionString;
			using (SqlConnection con = new SqlConnection(connectionString))
			using (SqlCommand sqlCommand = con.CreateCommand())
			{
				con.Open();
				command(sqlCommand);
			}
		}

		private int GetRowCount()
		{
			int val = 0;
			ExecuteInDb(delegate(SqlCommand command)
			{
				command.CommandText = "SELECT COUNT(*) FROM test_logging";
				val = (int)command.ExecuteScalar();
			});
			return val;
		}
	}
}