using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;

namespace Rhino.Commons.Helpers
{
	/// <summary>
	/// this class is here so I can avoid having a reference to the System.Data.SqlServerCe.dll if I don't need it.
	/// </summary>
	internal static class SqlCEDbHelper
	{
		private static string engineTypeName = "System.Data.SqlServerCe.SqlCeEngine, System.Data.SqlServerCe";
		private static Type type;
		private static PropertyInfo localConnectionString;
		private static MethodInfo createDatabase;

		internal static void CreateDatabaseFile(string filename)
		{
			if (File.Exists(filename))
				File.Delete(filename);
			if (type == null)
			{
				type = Type.GetType(engineTypeName);
				localConnectionString = type.GetProperty("LocalConnectionString");
				createDatabase = type.GetMethod("CreateDatabase");
			}
			object engine = Activator.CreateInstance(type);
			localConnectionString
				.SetValue(engine, string.Format("Data Source='{0}';", filename), null);
			createDatabase
				.Invoke(engine, new object[0]);
		}
	}
}