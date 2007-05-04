using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.SqlServer.Management.Smo;

namespace Bumbler
{
	public class SqlServerTable : ITable
	{
		private readonly Table table;

		public SqlServerTable(Table table)
		{
			this.table = table;
		}

		public IColumn[] Columns
		{
			get
			{
				List<IColumn> columns = new List<IColumn>();
				foreach (Column column in table.Columns)
				{
					columns.Add(new SqlServerColumn(table, column));
				}
				return columns.ToArray();
			}
		}

		public string Name
		{
			get { return table.Name; }
		}

		public bool HasSingleColumnPrimaryKey
		{
			get
			{
				foreach (Index index in table.Indexes)
				{
					if(index.IndexKeyType == IndexKeyType.DriPrimaryKey)
					{
						return index.IndexedColumns.Count == 1;
					}
				}
				return false;
			}
		}
	}
}