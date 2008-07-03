using System;
using System.Collections.Generic;
using log4net;
using log4net.Appender;
using log4net.Core;
using log4net.Repository.Hierarchy;

namespace Rhino.Queues.Tests
{
	public class LogSpy : IDisposable
	{
		private readonly Level oldLevel;
		private readonly Logger loggerImpl;
		private readonly MemoryAppender appender;

		static LogSpy()
		{
			log4net.Config.BasicConfigurator.Configure();
		}

		public string[] Messages
		{
			get
			{
				var msgs = new List<string>();
				foreach (var loggingEvent in appender.GetEvents())
				{
					msgs.Add(loggingEvent.MessageObject.ToString());
				}
				return msgs.ToArray();
			}
		}

		public LogSpy(Type type)
		{
			var logger = LogManager.GetLogger(type);
			loggerImpl = (Logger)logger.Logger;
			oldLevel = loggerImpl.Level;
			loggerImpl.Level = Level.Debug;

			appender = new MemoryAppender();
			appender.Threshold = Level.Debug;
			
			loggerImpl.AddAppender(appender);
		}

		public void Dispose()
		{
			loggerImpl.Level = oldLevel;
			loggerImpl.RemoveAppender(appender);
		}
	}
}