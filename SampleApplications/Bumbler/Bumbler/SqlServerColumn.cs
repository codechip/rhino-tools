using System;
using Microsoft.SqlServer.Management.Smo;

namespace Bumbler
{
	public class SqlServerColumn : IColumn
	{
		private readonly Table table;
		private readonly Column column;

		public SqlServerColumn(Table table, Column column)
		{
			this.table = table;
			this.column = column;
		}

		public bool IsFK
		{
			get
			{
				foreach (ForeignKey foreignKey in table.ForeignKeys)
				{
					foreach (ForeignKeyColumn foreignKeyColumn in foreignKey.Columns)
					{
						if(foreignKeyColumn.Name == column.Name)
							return true;
					}
				}
				return false;
			}
		}

		public string Name
		{
			get { return column.Name; }
		}

		public string FkTableName
		{
			get
			{
				foreach (ForeignKey foreignKey in table.ForeignKeys)
				{
					foreach (ForeignKeyColumn foreignKeyColumn in foreignKey.Columns)
					{
						if(foreignKeyColumn.Name == column.Name)
							return foreignKey.ReferencedTable;
					}
				}
				return null;
			}
		}

		public string ClrTypeName
		{
			get
			{
				//quick and direty impl
				switch (column.DataType.SqlDataType)
				{
					case SqlDataType.NVarChar:
					case SqlDataType.NVarCharMax:
					case SqlDataType.Char:
					case SqlDataType.VarChar:
					case SqlDataType.NText:
					case SqlDataType.Text:
					case SqlDataType.NChar:
					case SqlDataType.VarCharMax:
						return typeof(string).FullName;
					case SqlDataType.Float:
					case SqlDataType.Real:
						return typeof(float).FullName;
					case SqlDataType.Int:
					case SqlDataType.TinyInt:
						return typeof(int).FullName;
					case SqlDataType.DateTime:
					case SqlDataType.SmallDateTime:
						return typeof(DateTime).FullName;
					case SqlDataType.Decimal:
					case SqlDataType.Money:
						return typeof(decimal).FullName;
					case SqlDataType.Image:
					case SqlDataType.VarBinary:
					case SqlDataType.Binary:
					case SqlDataType.VarBinaryMax:
						return typeof(byte[]).FullName;
					case SqlDataType.BigInt:
						return typeof(long).FullName;
					case SqlDataType.SmallInt:
						return typeof(short).FullName;
					case SqlDataType.Bit:
						return typeof(bool).FullName;
					case SqlDataType.UniqueIdentifier:
						return typeof(Guid).FullName;
					default:
						throw new NotSupportedException(column.DataType.SqlDataType.ToString());
				}
			}
		}

		public bool IsPK
		{
			get
			{
				foreach (Index index in table.Indexes)
				{
					if(index.IndexKeyType != IndexKeyType.DriPrimaryKey)
						continue;
					foreach (IndexedColumn indexedColumn in index.IndexedColumns)
					{
						if(indexedColumn.Name == column.Name)
							return true;
					}
				}
				return false;
			}
		}
	}
}