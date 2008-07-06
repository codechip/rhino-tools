using System.IO;
using FirebirdSql.Data.FirebirdClient;

namespace Rhino.Queues.Tests
{
	public static class TestEnvironment
	{
		public static void Clear(string path)
		{
			FbConnection.ClearAllPools();
			if (Directory.Exists(path))
				Directory.Delete(path, true);
			Directory.CreateDirectory(path);
		}
	}
}