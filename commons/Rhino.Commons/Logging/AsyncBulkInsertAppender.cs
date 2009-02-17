namespace Rhino.Commons.Logging
{
	using System;
	using System.Collections.Generic;
	using System.Configuration;
	using System.Data;
	using System.Data.SqlClient;
	using System.Threading;
	using log4net.Appender;
	using log4net.Core;
	using log4net.Layout;
	using log4net.Util;

	///<summary>
	/// Writes to the database using SqlBulkCopy in async manner.
	/// This appender ensures that even if the database is down or takes a long time to respond
	/// the application is not being affected. 
	/// This include avoid trains in the thread pool, which would kill scalaiblity.
	///</summary>
	/// <example>
	/// An example of a SQL Server table that could be logged to:
	/// <code lang="SQL">
	/// CREATE TABLE [dbo].[Log] ( 
	///   [ID] [int] IDENTITY (1, 1) NOT NULL ,
	///   [Date] [datetime] NOT NULL ,
	///   [Thread] [varchar] (255) NOT NULL ,
	///   [Level] [varchar] (20) NOT NULL ,
	///   [Logger] [varchar] (255) NOT NULL ,
	///   [Message] [varchar] (4000) NOT NULL 
	/// ) ON [PRIMARY]
	/// </code>
	/// </example>
	/// <example>
	/// An example configuration to log to the above table.
	/// IMPORTANT: Column names are Case Sensitive!
	/// <code lang="XML" escaped="true">
	///<appender name="BulkInsertLogger"
	///		  type="Rhino.Commons.Logging.AsyncBulkInsertAppender, Rhino.Commons">
	///	<bufferSize value="1000" />
	///	<tableName value="Logs"/>
	///	<connectionStringName value="Logging"/>
	///
	///	<filter type="log4net.Filter.LoggerMatchFilter">
	///		<loggerToMatch  value="NHiberante.SQL" />
	///		<acceptOnMatch value="false" />
	///	</filter>
	///	<filter type="log4net.Filter.LoggerMatchFilter">
	///		<loggerToMatch  value="Rhino.Commons.HttpModules.PagePerformanceModule" />
	///		<acceptOnMatch value="false" />
	///	</filter>
	///	<filter type="log4net.Filter.LevelRangeFilter">
	///		<LevelMax value="WARN" />
	///		<LevelMin value="INFO" />
	///		<acceptOnMatch value="true" />
	///	</filter>
	///
	///	<mapping>
	///		<column value="Date" />
	///		<layout type="log4net.Layout.RawTimeStampLayout" />
	///	</mapping>
	///	<mapping>
	///		<column value="Thread" />
	///		<layout type="log4net.Layout.PatternLayout">
	///			<conversionPattern value="%thread" />
	///		</layout>
	///	</mapping>
	///	<mapping>
	///		<column value="Level" />
	///		<layout type="log4net.Layout.PatternLayout">
	///			<conversionPattern value="%level" />
	///		</layout>
	///	</mapping>
	///	<mapping>
	///		<column value="Logger" />
	///		<layout type="log4net.Layout.PatternLayout">
	///			<conversionPattern value="%logger" />
	///		</layout>
	///	</mapping>
	///	<mapping>
	///		<column value="Message" />
	///		<layout type="log4net.Layout.PatternLayout">
	///			<conversionPattern value="%message" />
	///		</layout>
	///	</mapping>
	///	<mapping>
	///		<column value="Exception" />
	///		<layout type="log4net.Layout.ExceptionLayout" />
	///	</mapping>
	///	
	///</appender>
	/// </code>
	/// </example>
	public class AsyncBulkInsertAppender : BufferingAppenderSkeleton
	{
		private string connectionStringName;
		private string connectionString;
		readonly List<BulkInsertMapping> mappings = new List<BulkInsertMapping>();
		private string tableName;
		private SqlBulkCopyOptions options = SqlBulkCopyOptions.KeepIdentity | SqlBulkCopyOptions.UseInternalTransaction;

		// we want to avoid taking up all the threads in the pool 
		// if we take undue amount to write to the DB.
		// we use the following three variables to write to the DB in this
		// way.
		readonly object syncLock = new object();
		readonly LinkedList<LoggingEvent[]> eventsList = new LinkedList<LoggingEvent[]>();
		private bool anotherThreadAlreadyHandlesLogging = false;

		public AsyncBulkInsertAppender()
		{
			Fix = FixFlags.All & ~FixFlags.LocationInfo;
		}

		public SqlBulkCopyOptions Options
		{
			get { return options; }
			set { options = value; }
		}

		public string TableName
		{
			get { return tableName; }
			set { tableName = value; }
		}

		public override void ActivateOptions()
		{
			foreach (BulkInsertMapping mapping in mappings)
			{
				mapping.Validate();
			}
			base.ActivateOptions();
		}

		/// <summary>
		/// Gets or sets the name of the connection string.
		/// </summary>
		/// <value>The name of the connection string.</value>
		public string ConnectionStringName
		{
			get { return connectionStringName; }
			set
			{
				ConnectionStringSettings con = ConfigurationManager.ConnectionStrings[value];
				if (con == null)
					throw new ConfigurationErrorsException("Could not find connections string named '" + value + "'");
				connectionString = con.ConnectionString;
				connectionStringName = value;
			}
		}

		protected override void SendBuffer(LoggingEvent[] events)
		{
			// we accept some additional complexity here
			// in favor of better concurrency. We don't want to
			// block all threads in the pool if we have an issue with
			// the database. Therefor, we perform thread sync to ensure
			// that only a single thread will write to the DB at any given point
			ThreadPool.QueueUserWorkItem(delegate
			{
				lock (syncLock)
				{
					eventsList.AddLast(events);
					if (anotherThreadAlreadyHandlesLogging)
						return;
						anotherThreadAlreadyHandlesLogging = true;
				}
				while (true)
				{
					LoggingEvent[] current;
					lock (syncLock)
					{
						if(eventsList.Count == 0)
						{
							anotherThreadAlreadyHandlesLogging = false;
							return;
						}
						current = eventsList.First.Value;
						eventsList.RemoveFirst();
					}
					PerformWriteToDatabase(current);
				}
			});
		}

		private void PerformWriteToDatabase(IEnumerable<LoggingEvent> events)
		{
			try
			{
				DataTable table = CreateTable(events);
				using (SqlBulkCopy bulk = new SqlBulkCopy(connectionString, options))
				{
					foreach (BulkInsertMapping mapping in mappings)
					{
						bulk.ColumnMappings.Add(mapping.Column, mapping.Column);
					}
					bulk.DestinationTableName = tableName;
					bulk.WriteToServer(table);
				}
			}
			catch (Exception ex)
			{
				LogLog.Error("Could not write logs to database in the background", ex);
			}
		}

		private DataTable CreateTable(IEnumerable<LoggingEvent> events)
		{
			DataTable table = new DataTable();
			BuildTableSchema(table);
			foreach (LoggingEvent le in events)
			{
				DataRow row = table.NewRow();
				foreach (BulkInsertMapping mapping in mappings)
				{
					mapping.AddValue(le, row);
				}
				table.Rows.Add(row);
			}
			return table;
		}

		private void BuildTableSchema(DataTable table)
		{
			foreach (BulkInsertMapping mapping in mappings)
			{
				table.Columns.Add(mapping.Column);
			}
		}

		public void AddMapping(BulkInsertMapping mapping)
		{
			mappings.Add(mapping);
		}

		public class BulkInsertMapping
		{
			private string column;
			private IRawLayout layout;

			public IRawLayout Layout
			{
				get { return layout; }
				set { layout = value; }
			}

			public string Column
			{
				get { return column; }
				set { column = value; }
			}

			public void Validate()
			{
				if (string.IsNullOrEmpty(Column))
					throw new InvalidOperationException("Must specify column name");
				if (Layout == null)
					throw new InvalidOperationException("Must specify layout");
			}

			public void AddValue(LoggingEvent loggingEvent, DataRow row)
			{
				row[Column] = layout.Format(loggingEvent);
			}
		}
	}
}