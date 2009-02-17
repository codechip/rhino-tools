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
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Web;
using log4net.Appender;
using log4net.Core;
using log4net.Layout;
using log4net.Util;

namespace Rhino.Commons.Logging
{
    /// <summary>
    /// Default parameters are set for:
    /// @Date, @Thread, @Level, @Logger, @Message, @Exception
    /// </summary>
    public abstract class RollingEmbeddedDatabaseAppender : AdoNetAppender, IDisposable
    {
        private long maxNumberOfBackups = 3;
		private string directory;
		private string connectionStringFormat = "Data Source={0};";
		private string fileNameFormat = "{0}.log4net";
		private string tableName = "Logs";
		private const string selectCount = "SELECT COUNT(*) FROM {0}";
		private int maxNumberOfRows = 50000;

		

		int numberOfRows = 0;
		private int curRollBackups = 0;
		private string currentFilePath;

        public RollingEmbeddedDatabaseAppender()
		{
		    directory = Environment.CurrentDirectory;
			if(HttpContext.Current!=null)
			{
			    directory = AppDomain.CurrentDomain.BaseDirectory;
                //this is apperantly required in order to work correctly under ASP.Net
                AppDomain.CurrentDomain.SetData("SQLServerCompactEditionUnderWebHosting", true);
			}
			m_usePreparedCommand = false;
			ReconnectOnError = true;
			CommandText =
				@"INSERT INTO Logs (Date, Thread, Level, Logger, Message, Exception) 
						VALUES (@Date, @Thread, @Level, @Logger, @Message, @Exception)";

			AdoNetAppenderParameter date = new AdoNetAppenderParameter();
			date.DbType = DbType.DateTime;
			date.ParameterName = "@Date";
			date.Layout = new RawTimeStampLayout();
			AddParameter(date);

			AdoNetAppenderParameter thread = new AdoNetAppenderParameter();
			thread.DbType = DbType.String;
			thread.ParameterName = "@Thread";
			thread.Size = 255;
			thread.Layout = new Layout2RawLayoutAdapter(new PatternLayout("%thread"));
			AddParameter(thread);

			AdoNetAppenderParameter level = new AdoNetAppenderParameter();
			level.DbType = DbType.String;
			level.ParameterName = "@Level";
			level.Size = 50;
			level.Layout = new Layout2RawLayoutAdapter(new PatternLayout("%level"));
			AddParameter(level);

			AdoNetAppenderParameter logger = new AdoNetAppenderParameter();
			logger.DbType = DbType.String;
			logger.ParameterName = "@Logger";
			logger.Size = 512;
			logger.Layout = new Layout2RawLayoutAdapter(new PatternLayout("%logger"));
			AddParameter(logger);

			AdoNetAppenderParameter message = new AdoNetAppenderParameter();
			message.DbType = DbType.String;
			message.ParameterName = "@Message";
			message.Size = 2000;
			message.Layout = new Layout2RawLayoutAdapter(new PatternLayout("%message"));
			AddParameter(message);

			AdoNetAppenderParameter exception = new AdoNetAppenderParameter();
			exception.DbType = DbType.String;
			exception.ParameterName = "@Exception";
			exception.Size = 4000;
			exception.Layout = new Layout2RawLayoutAdapter(new PatternLayout("%exception"));
			AddParameter(exception);
		}

        /// <summary>
		/// The name of the table (used when calculating row counts).
		/// Defaults to "Logs"
		/// </summary>
		public string TableName
		{
			get { return tableName; }
			set { tableName = value; }
		}

		/// <summary>
		/// The file format that will be created.
		/// The default is for:
		/// {0}.log4net
		/// Where {0} is the current process name
		/// </summary>
		public string FileNameFormat
		{
			get { return fileNameFormat; }
			set { fileNameFormat = value; }
		}

		/// <summary>
		/// The directory to put the logs files at.
		/// Defaults to the current directory.
		/// </summary>
		public string Directory
		{
			get { return directory; }
			set { directory = value; }
		}

		/// <summary>
		/// Max number of rows per file.
		/// Defaults to 50,000
		/// </summary>
		public int MaxNumberOfRows
		{
			get { return maxNumberOfRows; }
			set { maxNumberOfRows = value; }
		}

		/// <summary>
		/// Number of backup files to use.
		/// Defaults to 3
		/// </summary>
		public long MaxNumberOfBackups
		{
			get { return maxNumberOfBackups; }
			set { maxNumberOfBackups = value; }
		}

		protected override void SendBuffer(LoggingEvent[] events)
		{
			numberOfRows += events.Length;
			base.SendBuffer(events);
			AdjustFileAfterAppend();
		}

		private void AdjustFileAfterAppend()
		{
			if (numberOfRows < MaxNumberOfRows)
				return;

			Connection.Dispose();//close the file, so we can move it.
			RollOverRenameFiles(CurrentFilePath);

			ActivateOptions();
		}

		protected void RollOverRenameFiles(string baseFileName)
		{
			// If maxBackups <= 0, then there is no file renaming to be done.
			if (MaxNumberOfBackups != 0)
			{
				// Delete the oldest file, to keep Windows happy.
				if (curRollBackups == MaxNumberOfBackups)
				{
					DeleteFile(baseFileName + '.' + MaxNumberOfBackups);
					curRollBackups--;
				}

				// Map {(maxBackupIndex - 1), ..., 2, 1} to {maxBackupIndex, ..., 3, 2}
				for (int i = curRollBackups; i >= 1; i--)
				{
					RollFile((baseFileName + "." + i), (baseFileName + '.' + (i + 1)));
				}

				curRollBackups++;

				// Rename fileName to fileName.1
				RollFile(baseFileName, baseFileName + ".1");
			}
			else // no backups specified, just delete and start from scratch
			{
				DeleteFile(baseFileName);
			}
		}

		protected void RollFile(string fromFile, string toFile)
		{
			if (File.Exists(fromFile))
			{
				// Delete the toFile if it exists
				DeleteFile(toFile);
				int retries = 3;
				while (retries != 0)
				{
					// We may not have permission to move the file, or the file may be locked
					try
					{
						LogLog.Debug("RollingFileAppender: Moving [" + fromFile + "] -> [" + toFile + "]");
						File.Move(fromFile, toFile);
						retries = 0;
					}
					catch (Exception moveEx)
					{
						ErrorHandler.Error("Exception while rolling file [" + fromFile + "] -> [" + toFile + "]",
										   moveEx,
										   ErrorCode.GenericFailure);
						retries--;
					}
				}
			}
			else
			{
				LogLog.Warn("RollingFileAppender: Cannot RollFile [" + fromFile + "] -> [" + toFile + "]. Source does not exist");
			}
		}

		protected void DeleteFile(string file)
		{
			// We may not have permission to delete the file, or the file may be locked
			try
			{
				LogLog.Debug("RollingFileAppender: Deleting [" + file + "]");
				File.Delete(file);
			}
			catch (Exception moveEx)
			{
				ErrorHandler.Error("Exception while rolling file [" + file + "]", moveEx, ErrorCode.GenericFailure);
			}
		}

		public string CurrentFilePath
		{
			get
			{
				if (currentFilePath == null)
				{
					string processName = Process.GetCurrentProcess().ProcessName;
					string currentFile = string.Format(FileNameFormat, processName);
					currentFilePath = Path.Combine(directory, currentFile);
				}
				return currentFilePath;
			}
		}

		private void CheckNumberOfRows()
		{
			if (Connection == null || Connection.State != ConnectionState.Open)
			{
				LogLog.Debug(
					"RollingSqlCEAppender: could not check number of rows in database (usually because of an error in creating / talking to it), assuming 0 rows exists.");
				numberOfRows = 0;
				return;
			}
			using (IDbCommand cmd = Connection.CreateCommand())
			{
				cmd.CommandText = string.Format(selectCount, TableName);
				numberOfRows = Convert.ToInt32(cmd.ExecuteScalar());
			}
		}

		public override void ActivateOptions()
		{
			bool shouldCreateFile = !File.Exists(CurrentFilePath);
			ConnectionString = string.Format(connectionStringFormat, CurrentFilePath);
			if (shouldCreateFile)
			{
			    CreateDatabaseFile();
			    using (IDbConnection connection = (IDbConnection)Activator.CreateInstance(this.ResolveConnectionType()))
				{
					connection.ConnectionString = ConnectionString;
					connection.Open();
					using (IDbCommand cmd = connection.CreateCommand())
					{
						cmd.CommandText = CreateScript;
						cmd.ExecuteNonQuery();
					}
				}
				base.ActivateOptions();
				CheckNumberOfRows();
			}
			else
			{
				base.ActivateOptions();
			}

		}

        public void Dispose()
		{
			Close();
			if (Connection != null)
				Connection.Dispose();
		}

        public abstract string CreateScript { get; set; }
       
        protected abstract void CreateDatabaseFile();
    }
}