using System.Data;

namespace Ayende.NHibernateQueryAnalyzer.Model
{
	public interface IDataBaseSetupImpl : IDataBaseSetup
	{
		/// <summary>
		/// Ensures the database file exists.
		/// If it doesn't exist, it and the tables will be created
		/// </summary>
		void EnsureDatabaseFileExists(string path);

		/// <summary>
		/// Gets the data source file path.
		/// </summary>
		string GetDataSourceFilePath(string cnstr);

		/// <summary>
		/// Expend any envirnment variables in the connection string.
		/// </summary>
		string ExpandEnvironmentVariablesInConnectionString(NHibernate.Cfg.Configuration cfg);

		void CreateDatabase(IDbConnection con);
	}
}