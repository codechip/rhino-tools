using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Smo;

namespace Bumbler
{
	public class SqlServerSchemaInspector : ISchemaInspector
	{
		private Database database;

		private const string CONNECTION_STRING_PATTERN
			= @"=(?<sqlServerName>.+);Initial Catalog=(?<databaseName>\w+);";

		private static readonly Regex ConnectionString_Regex
			= new Regex(CONNECTION_STRING_PATTERN, RegexOptions.IgnoreCase);

		public SqlServerSchemaInspector(string connectionString)
		{
			Server server = new Server();
			server.ConnectionContext.ConnectionString = connectionString;
			server.Refresh();
			string databaseName = ConnectionString_Regex.Match(connectionString).Groups["databaseName"].Value;
			database = server.Databases[databaseName];
		}

		public ITable[] GetTables()
		{
			List<ITable> tables = new List<ITable>();
			foreach (Table table in database.Tables)
			{
				tables.Add(new SqlServerTable(table));
			}
			return tables.ToArray();
		}
	}
}
