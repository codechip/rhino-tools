#region license
// Copyright (c) 2005 - 2007 Ayende Rahien (ayende@ayende.com)
// All rights reserved.
// 
// Redistribution and use in source and binary forms, with or without modification,
// are permitted provided that the following conditions are met:
// 
//     * Redistributions of source code must retain the above copyright notice,
//     this list of conditions and the following disclaimer.
//     * Redistributions in binary form must reproduce the above copyright notice,
//     this list of conditions and the following disclaimer in the documentation
//     and/or other materials provided with the distribution.
//     * Neither the name of Ayende Rahien nor the names of its
//     contributors may be used to endorse or promote products derived from this
//     software without specific prior written permission.
// 
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND
// ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
// WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
// DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS BE LIABLE
// FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL
// DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR
// SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER
// CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY,
// OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF
// THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
#endregion


using System;
using System.Data;
using System.Data.SQLite;
using System.IO;
using Ayende.NHibernateQueryAnalyzer.Model;
using Ayende.NHibernateQueryAnalyzer.Tests.TestUtilities;
using MbUnit.Framework;

namespace Ayende.NHibernateQueryAnalyzer.Tests.Core
{
	[TestFixture]
	//[Category("Integration")]
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