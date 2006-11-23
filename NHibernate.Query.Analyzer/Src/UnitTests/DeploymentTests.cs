using System;
using System.IO;
using System.Reflection;
using Ayende.NHibernateQueryAnalyzer.UnitTests.Asserts;
using NUnit.Framework;

namespace Ayende.NHibernateQueryAnalyzer.Tests
{
	[TestFixture]
	public class DeploymentTests
	{
		private string basePath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().CodeBase.Substring(8));

		[Test]
		public void FrameWorkVersion()
		{
			Assert.IsTrue(new Version(1, 1) < Environment.Version, "Incompatible framework version.");
		}

		[Test]
		public void FilesForTestsExists()
		{
			#region Required Files

				string[] neededFiles = {@"TestProject\Ayende.NHibernateQueryAnalyzer.TestProject.dll", @"TestProject\Ayende.NHibernateQueryAnalyzer.TestProject.dll.config", @"TestProject\TestProject.db", @"TestProject\Ayende.NHibernateQueryAnalyzer.TestProject.hbm.xml"};

			#endregion

			foreach (string file in neededFiles)
			{
				Console.WriteLine("Verifing existance of: " + file);
				FileAssert.Exists(Path.Combine(basePath,file));
			}
		}

		[Test]
		public void RequiredAssemblies()
		{
			#region Required Assemblies

			string[] requiredAssemblies = {
											  "Ayende.NHibernateQueryAnalyzer.Core.dll",
											  "Ayende.NHibernateQueryAnalyzer.ProjectLoader.dll",
											  "Ayende.NHibernateQueryAnalyzer.UnitTests.dll",
											  "Ayende.NHibernateQueryAnalyzer.Utilities.dll",
											  "Castle.DynamicProxy.dll",
											  "Iesi.Collections.dll",
											  "log4net.dll",
											  "NHibernate.dll",
											  "System.Data.Sqlite.dll",
											  "WeifenLuo.WinFormsUI.Docking.dll"
										  };

			#endregion

			foreach (string file in requiredAssemblies)
			{
				FileAssert.Exists(Path.Combine(basePath, file), file + " does not exists");
			}
		}
	}
}