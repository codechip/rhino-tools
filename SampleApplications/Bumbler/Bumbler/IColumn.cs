namespace Bumbler
{
	public interface IColumn
	{
		bool IsFK { get; }

		string Name { get; }

		string FkTableName { get; }

		string ClrTypeName { get; }

		bool IsPK { get; }
	}
}