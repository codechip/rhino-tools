using System.Collections;

namespace Bumbler
{
	public interface ISchemaInspector
	{
		ITable[] GetTables();
	}
}