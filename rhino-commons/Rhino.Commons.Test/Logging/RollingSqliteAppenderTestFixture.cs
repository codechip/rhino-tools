using System.Data.SQLite;
using System.Diagnostics;
using System.IO;
using log4net.Core;
using NUnit.Framework;
using Rhino.Commons.Logging;

namespace Rhino.Commons.Test.Logging
{
	[TestFixture]
	public class RollingSqliteAppenderTestFixture
	{
		private RollingSqliteAppender appender;
		private LoggingEvent loggingEvent;
		string databaseFile = Process.GetCurrentProcess().ProcessName + ".log4net";

		[SetUp]
		public void TestInitialize()
		{
			File.Delete(databaseFile);
			appender = new RollingSqliteAppender();
			appender.ActivateOptions();
			LoggingEventData loggingEventData = new LoggingEventData();
			loggingEventData.Level = Level.Error;
			loggingEventData.Message = "foo";
			loggingEvent = new LoggingEvent(loggingEventData);
		}

		[TearDown]
		public void TestCleanup()
		{
			appender.Dispose();
			File.Delete(databaseFile);
		}

		[Test]
		public void CanAppendToAppender_WillNotThrow()
		{
			appender.DoAppend(loggingEvent);
		}

		[Test]
		public void AppendingCreatesSqliteDatabaseFile()
		{
			appender.DoAppend(loggingEvent);
			Assert.IsTrue(File.Exists(databaseFile));
		}

		[Test]
		public void AppendingEventAddToFile()
		{
			appender.BufferSize = 0;
			appender.DoAppend(loggingEvent);
			long count = GetEventsCount(appender.ConnectionString);

			Assert.AreEqual(1L, count);
		}

		[Test]
		public void WillRollBackWhenReachingMaxLimit()
		{
			appender.BufferSize = 512;
			appender.MaxNumberOfRows = 512;
			for (int i = 0; i < 1026; i++)
			{
				appender.DoAppend(loggingEvent);
			}

			string rolledBackFile = databaseFile + ".1";
			Assert.IsTrue(File.Exists(rolledBackFile));
			Assert.IsTrue(File.Exists(databaseFile));
			long eventsCount = GetEventsCount(appender.ConnectionString);
			Assert.AreEqual(513, eventsCount, "fresh file shoule have 512 events" );
			eventsCount = GetEventsCount(appender.ConnectionString.Replace(databaseFile, rolledBackFile));
			Assert.AreEqual(513, eventsCount, "rolledback file shoule have 512 events");
		}

		[Test]
		public void WillCycleThroughThreeFiles()
		{
			appender.BufferSize = 10;
			appender.MaxNumberOfRows = 10;
			appender.MaxNumberOfBackups = 3;

			for (int i = 0; i < 70; i++)
			{
				appender.DoAppend(loggingEvent);
			}

			string rolledBackFile1 = databaseFile + ".1";
			string rolledBackFile2 = databaseFile + ".2";
			string rolledBackFile3 = databaseFile + ".3";
			string rolledBackFile4_doesnotExists = databaseFile + ".4";
			Assert.IsTrue(File.Exists(rolledBackFile1));
			Assert.IsTrue(File.Exists(rolledBackFile2));
			Assert.IsTrue(File.Exists(rolledBackFile3));
			Assert.IsTrue(File.Exists(databaseFile));

			Assert.IsFalse(File.Exists(rolledBackFile4_doesnotExists) , "Only three backups");
			
			long eventsCount = GetEventsCount(appender.ConnectionString);
			Assert.AreEqual(11, eventsCount, "fresh file shoule have 512 events");
			eventsCount = GetEventsCount(appender.ConnectionString.Replace(databaseFile, rolledBackFile1));
			Assert.AreEqual(11, eventsCount, "rolledback file shoule have 512 events");
			eventsCount = GetEventsCount(appender.ConnectionString.Replace(databaseFile, rolledBackFile2));
			Assert.AreEqual(11, eventsCount, "rolledback file shoule have 512 events");
			eventsCount = GetEventsCount(appender.ConnectionString.Replace(databaseFile, rolledBackFile3));
			Assert.AreEqual(11, eventsCount, "rolledback file shoule have 512 events");
	
		}
		
		private long GetEventsCount(string connectionString)
		{
			using (SQLiteConnection connection = new SQLiteConnection(connectionString))
			{
				connection.Open();
				using (SQLiteCommand command = connection.CreateCommand())
				{
					command.CommandText = "SELECT COUNT(*) FROM Logs";
					return (long)command.ExecuteScalar();
				}
			}
		}
	}
}
