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
						anotherThreadAlreadyHandlesLogging = true;
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