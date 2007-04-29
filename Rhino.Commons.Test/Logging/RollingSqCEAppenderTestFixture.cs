#region license
// Copyright (c) 2005 - 2007 Ayende Rahien (ayende@ayende.com)
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
#endregion


using System;
using System.Data.SqlServerCe;
using System.IO;
using log4net;
using log4net.Appender;
using log4net.Core;
using MbUnit.Framework;
using Rhino.Commons.Logging;

namespace Rhino.Commons.Test.Logging
{
	[TestFixture]
	public class RollingSqCEAppenderTestFixture
	{
		private RollingSqlCEAppender appender;
		private LoggingEvent loggingEvent;
		string databaseFile;

		[SetUp]
		public void TestInitialize()
		{
			appender = new RollingSqlCEAppender();
			databaseFile = "test.log4net";
			appender.FileNameFormat = databaseFile;
			File.Delete(databaseFile);
			appender.ActivateOptions();
			LoggingEventData loggingEventData = new LoggingEventData();
			loggingEventData.Level = Level.Error;
			loggingEventData.Message = "foo";
			loggingEventData.TimeStamp = DateTime.Now;
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
			int count = GetEventsCount(appender.ConnectionString);

			Assert.AreEqual(1, count);
		}

		[Test]
		public void WillRollBackWhenReachingMaxLimit()
		{
			appender.BufferSize = 512;
			appender.MaxNumberOfRows = 512;
			for (int i = 0; i < 1023; i++)
			{
				appender.DoAppend(loggingEvent);
			}
			appender.Flush();

			string rolledBackFile = databaseFile + ".1";
			Assert.IsTrue(File.Exists(rolledBackFile));
			Assert.IsTrue(File.Exists(databaseFile));
			long eventsCount = GetEventsCount(appender.ConnectionString);
			Assert.AreEqual(510, eventsCount, "fresh file shoule have 510 events");
			eventsCount = GetEventsCount(appender.ConnectionString.Replace(databaseFile, rolledBackFile));
			Assert.AreEqual(513, eventsCount, "rolledback file shoule have 513 events");
		}

		[Test]
		public void WillCycleThroughThreeFiles()
		{
			string rolledBackFile1 = databaseFile + ".1";
			string rolledBackFile2 = databaseFile + ".2";
			string rolledBackFile3 = databaseFile + ".3";
			string rolledBackFile4_doesnotExists = databaseFile + ".4";
			File.Delete(rolledBackFile1);
			File.Delete(rolledBackFile2);
			File.Delete(rolledBackFile3);

			appender.BufferSize = 512;
			appender.MaxNumberOfRows = 512;
			appender.MaxNumberOfBackups = 3;

			for (int i = 0; i < 5000; i++)
			{
				appender.DoAppend(loggingEvent);
			}
			appender.Flush();

			Assert.IsTrue(File.Exists(rolledBackFile1));
			Assert.IsTrue(File.Exists(rolledBackFile2));
			Assert.IsTrue(File.Exists(rolledBackFile3));
			Assert.IsTrue(File.Exists(databaseFile));

			Assert.IsFalse(File.Exists(rolledBackFile4_doesnotExists), "Only file + 2 backups");

			long eventsCount = GetEventsCount(appender.ConnectionString);
			Assert.AreEqual(383, eventsCount, "fresh file shoule have 383 events");
			eventsCount = GetEventsCount(appender.ConnectionString.Replace(databaseFile, rolledBackFile1));
			Assert.AreEqual(513, eventsCount, "rolledback file shoule have 513 events");
			eventsCount = GetEventsCount(appender.ConnectionString.Replace(databaseFile, rolledBackFile2));
			Assert.AreEqual(513, eventsCount, "rolledback file shoule have 513 events");
			eventsCount = GetEventsCount(appender.ConnectionString.Replace(databaseFile, rolledBackFile3));
			Assert.AreEqual(513, eventsCount, "rolledback file shoule have 513 events");
		}

		[Test]
		public void CanIntegrateToLog4NetConfig()
		{
			log4net.Config.XmlConfigurator.Configure();
			ILog logger = LogManager.GetLogger("test");
			IAppender[] appenders = logger.Logger.Repository.GetAppenders();
			Assert.AreEqual(1, appenders.Length);
			Assert.AreEqual(typeof(RollingSqlCEAppender), appenders[0].GetType());
			IDisposable disposable = appenders[0] as IDisposable;
			disposable.Dispose();
		}

		private int GetEventsCount(string connectionString)
		{
			using (SqlCeConnection connection = new SqlCeConnection(connectionString))
			{
				connection.Open();
				using (SqlCeCommand command = connection.CreateCommand())
				{
					command.CommandText = "SELECT COUNT(*) FROM Logs";
					return (int)command.ExecuteScalar();
				}
			}
		}
	}
}