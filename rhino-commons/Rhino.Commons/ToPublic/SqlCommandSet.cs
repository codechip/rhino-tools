using System;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Reflection;

namespace Rhino.Commons
{
	/// <summary>
	/// Expose the batch functionality in ADO.Net 2.0
	/// Microsoft in its wisdom decided to make my life hard and mark it internal.
	/// Through the use of Reflection and some delegates magic, I opened up the functionality.
	/// 
	/// There is NO documentation for this, and likely zero support.
	/// Use at your own risk, etc...
	/// 
	/// Observable performance benefits are 50%+ when used, so it is really worth it.
	/// </summary>
	[ThereBeDragons("Not supported by Microsoft, but has major performance boost")]
	public class SqlCommandSet : IDisposable
	{
		private static Type sqlCmdSetType;
		private object instance;
		private PropSetter<SqlConnection> connectionSetter;
		private PropSetter<SqlTransaction> transactionSetter;
		private PropGetter<SqlConnection> connectionGetter;
		private AppendCommand doAppend;
		private ExecuteNonQueryCommand doExecuteNonQuery;
		private DisposeCommand doDispose;
		private int countOfCommands = 0;

		static SqlCommandSet()
		{
			Assembly sysData = Assembly.Load("System.Data, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089");
			sqlCmdSetType = sysData.GetType("System.Data.SqlClient.SqlCommandSet");
			Debug.Assert(sqlCmdSetType != null, "Could not find SqlCommandSet!");
		}

		public SqlCommandSet()
		{
			instance = Activator.CreateInstance(sqlCmdSetType, true);
			connectionSetter = (PropSetter<SqlConnection>)
							  Delegate.CreateDelegate(typeof(PropSetter<SqlConnection>),
													  instance, "set_Connection");
			transactionSetter = (PropSetter<SqlTransaction>)
								Delegate.CreateDelegate(typeof(PropSetter<SqlTransaction>),
														instance, "set_Transaction");
			connectionGetter = (PropGetter<SqlConnection>)
							  Delegate.CreateDelegate(typeof(PropGetter<SqlConnection>),
													  instance, "get_Connection");
			doAppend = (AppendCommand)Delegate.CreateDelegate(typeof(AppendCommand), instance, "Append");
			doExecuteNonQuery = (ExecuteNonQueryCommand)
							   Delegate.CreateDelegate(typeof(ExecuteNonQueryCommand),
													   instance, "ExecuteNonQuery");
			doDispose = (DisposeCommand)Delegate.CreateDelegate(typeof(DisposeCommand), instance, "Dispose");

		}

		/// <summary>
		/// Append a command to the batch
		/// </summary>
		/// <param name="command"></param>
		public void Append(SqlCommand command)
		{
			doAppend(command);
			countOfCommands++;
		}

		/// <summary>
		/// The number of commands batched in this instance
		/// </summary>
		public int CountOfCommands
		{
			get { return countOfCommands; }
		}

		/// <summary>
		/// Executes the batch
		/// </summary>
		/// <returns>
		/// This seems to be returning the total number of affected rows in all queries
		/// </returns>
		public int ExecuteNonQuery()
		{
			return doExecuteNonQuery();
		}

		public SqlConnection Connection
		{
			get { return connectionGetter(); }
			set { connectionSetter(value); }
		}

		public SqlTransaction Transaction
		{
			set { transactionSetter(value); }
		}

		///<summary>
		///Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
		///</summary>
		///<filterpriority>2</filterpriority>
		public void Dispose()
		{
			doDispose();
		}

		#region Delegate Definations
		private delegate void PropSetter<T>(T item);
		private delegate T PropGetter<T>();
		private delegate void AppendCommand(SqlCommand command);
		private delegate int ExecuteNonQueryCommand();
		private delegate void DisposeCommand();
		#endregion
	}
}
