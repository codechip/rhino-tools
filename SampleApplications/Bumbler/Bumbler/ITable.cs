using System.Collections;

namespace Bumbler
{
	public interface ITable
	{
		IColumn[] Columns { get; }

		string Name { get; }

		bool HasSingleColumnPrimaryKey { get; }
	}
}