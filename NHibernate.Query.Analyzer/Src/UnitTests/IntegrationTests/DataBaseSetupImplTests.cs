using System;
using System.Data;
using System.IO;
using Ayende.NHibernateQueryAnalyzer.Model;
using Ayende.NHibernateQueryAnalyzer.Tests.TestUtilities;
using Finisar.SQLite;
using NUnit.Framework;

namespace Ayende.NHibernateQueryAnalyzer.Tests.Core
{
	[TestFixture]
	[Category("Integration")]
	public class DataBaseSetupImplTests
	{
		[Test]
		public void SetConnectionString()
		{
			NHibernate.Cfg.Configuration cfg = new NHibernate.Cfg.Configuration();
			string shortConnectionString = @"%AppData%\Test\Test.data", expandedConnectionString = Environment.ExpandEnvironmentVariables(shortConnectionString);
			cfg.SetProperty(TestDataUtil.HibernateConnectionString, shortConnectionString);

			IDataBaseSetupImpl dataBaseSetup = new ProjectsRepository.DataBaseSetup();
			dataBaseSetup.ExpandEnvironmentVariablesInConnectionString(cfg);
			Assert.AreEqual(expandedConnectionString, cfg.GetProperty(TestDataUtil.HibernateConnectionString), "SetConnectionString doesn't expend environment variables properly.");
		}

		[Test]
		public void GetDataSourcePath()
		{
			string conStr = @"Data Source=C:\Data\Files\DB.sqlite;Version=3";
			string path = @"C:\Data\Files\DB.sqlite";
			string result;
			IDataBaseSetupImpl dataBaseSetup = new ProjectsRepository.DataBaseSetup();
			result = dataBaseSetup.GetDataSourceFilePath(conStr);
			Assert.AreEqual(path, result);
		}

		[Test]
		public void EnsureDatabaseFileExistsCreatesFile()
		{
			string path = Path.GetTempFileName();
			string connectionString = "Data Source={0};New=False;UTF8Encoding=True;Version=3";
			string fullConnectionString = string.Format(connectionString, path);
			File.Delete(path); //We need this because GetTempFileName() also create a file.
			//This force creating the file
			IDataBaseSetupImpl dataBaseSetup = new ProjectsRepository.DataBaseSetup();
			dataBaseSetup.EnsureDatabaseFileExists(path);
			Assert.IsTrue(File.Exists(path), "EnsureDB File Exists didn't create the file");
			EnsureDatabaseSchemaIsValid(fullConnectionString);
			File.Delete(path);
		}

		[Test]
		public void EnsureDataFileExistsCreatesFileAndDirectory()
		{
			string path = Path.GetTempFileName();
			File.Delete(path); //We need this because GetTempFileName() also create a file.
			path = Path.Combine(path,Path.GetFileName(path));//Create a directory
			string connectionString = "Data Source={0};New=False;UTF8Encoding=True;Version=3";
			string fullConnectionString = string.Format(connectionString, path);



			//This force creating the file
			IDataBaseSetupImpl dataBaseSetup = new ProjectsRepository.DataBaseSetup();
			dataBaseSetup.EnsureDatabaseFileExists(path);
			Assert.IsTrue(File.Exists(path), "EnsureDB File Exists didn't create the file or directory");
			EnsureDatabaseSchemaIsValid(fullConnectionString);
			Directory.Delete(Path.GetDirectoryName(path),true);
		}


		private static void EnsureDatabaseSchemaIsValid(string connectionString)
		{
			using (SQLiteConnection sc = new SQLiteConnection(connectionString))
			{
				sc.Open(); //If it fails, we'll get the exception and that will tell what happened.
				Assert.AreEqual(ConnectionState.Open, sc.State, "Could not open connection to database");
				SQLiteCommand command = sc.CreateCommand();
				command.CommandText = "select tbl_name from sqlite_master;";
				IDataReader reader = command.ExecuteReader();
				Assert.IsTrue(reader.Read(), "No tables were created during EnsureDataBaseFileExists()");
				Assert.AreEqual("Projects", reader.GetValue(0), "Table 'Projects' doesn't exist in database");
				Assert.IsTrue(reader.Read(), "Not enough tables were created during EnsureDataBaseFileExists()");
				Assert.AreEqual("Files", reader.GetValue(0), "Table 'Files' doesn't exist in database");
				Assert.IsTrue(reader.Read(), "Not enough tables were created during EnsureDataBaseFileExists()");
				Assert.AreEqual("Queries", reader.GetValue(0), "Table 'Queries' doesn't exist in database");
			}
		}


		[Test]
		public void EnsureDatabaseFileExistsKeepsData()
		{
			string connectionString = "Data Source={0};New=False;UTF8Encoding=True;Version=3";
			string path = Path.GetTempFileName();
			File.Delete(path);
			string fullConnectionString = string.Format(connectionString, path);
			IDataBaseSetupImpl dataBaseSetup = new ProjectsRepository.DataBaseSetup();
			dataBaseSetup.EnsureDatabaseFileExists(path);
			using (SQLiteConnection sc = new SQLiteConnection(fullConnectionString))
			{
				sc.Open();
				IDbCommand command = sc.CreateCommand();
				command.CommandText = "INSERT INTO Queries (project_id, name,text) VALUES (100,'Test Queries','from Tests As Success')";
				command.ExecuteNonQuery();
			}
			//Call it again to see it destroy data.
			dataBaseSetup.EnsureDatabaseFileExists(path);
			using (SQLiteConnection sc = new SQLiteConnection(fullConnectionString))
			{
				sc.Open();
				IDbCommand command = sc.CreateCommand();
				command.CommandText = "SELECT name,text FROM Queries WHERE project_id = 100";
				IDataReader reader = command.ExecuteReader();
				Assert.IsTrue(reader.Read(), "Data was no preserved when calling EnsureDatabaseFileExists twice");
				Assert.AreEqual("Test Queries", reader.GetValue(0), "Bad data when calling ensure db twice");
				Assert.AreEqual("from Tests As Success", reader.GetValue(1), "Bad data when calling ensure db twice");
			}
			File.Delete(path);
		}
	}
}