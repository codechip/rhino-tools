using System;
using System.Collections;
using System.IO;
using Ayende.NHibernateQueryAnalyzer.Exceptions;
using Ayende.NHibernateQueryAnalyzer.Model;
using NUnit.Framework;

namespace Ayende.NHibernateQueryAnalyzer.UnitTests.Core
{

	[TestFixture]
	public class ErrorHandlingTests
	{
		Project prj;
		private static readonly string fileDoesNotExists = "does not exists.hbm.xml";
		private static readonly string fileWithBadExtention = 
			Environment.ExpandEnvironmentVariables(@"%windir%\win.ini");

		private string query = "from TestProject";

		[SetUp]
		public void SetUp()
		{
			prj = new Project("My Test Project");
		}

		[Test]
		public void DoesntThrowOnAddFileThatDoesNotExist()
		{
			prj.AddFile(fileDoesNotExists);
		}

		[Test]
		[ExpectedException(typeof(FileLoadingException))]
		public void ThrowsWhenBuildingProjectWithMissingFiles()
		{
			prj.AddFile(fileDoesNotExists);
			try
			{
				prj.BuildProject();
			}
			catch (Exception e)
			{
				Assert.AreEqual("File " +Path.GetFullPath(fileDoesNotExists) +" does not exists",e.Message);
				throw;
			}
		}

		[Test]
		[ExpectedException(typeof(UnknownFileTypeException))]
		public void ThrowsWhenEncounterUnknownFileType()
		{
			prj.AddFile(fileWithBadExtention);

			try
			{
				prj.BuildProject();
			}
			catch (UnknownFileTypeException e)
			{
				Assert.AreEqual(fileWithBadExtention,e.Message);
				throw;
			}
		}

		[Test]
		[ExpectedException(typeof(InvalidOperationException),"Can't run queries if the project is not built.")]
		public void CantRunHqlUntilProjectIsBuilt()
		{
			prj.RunHql(query);
		}

		[Test]
		[ExpectedException(typeof(InvalidOperationException),"Can't run queries if the project is not built.")]
		public void CantRunHqlAsSqlUntilProjectIsBuilt()
		{
			prj.RunHqlAsRawSql(query);
		}

		[Test]
		[ExpectedException(typeof(InvalidOperationException),"Can't translate queries if the project was not built.")]
		public void CantTranslateQueryUntilProjectIsBuild()
		{
			prj.HqlToSql(query,new Hashtable());
		}

	}
}
