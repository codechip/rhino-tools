using System.Collections;
using Ayende.NHibernateQueryAnalyzer.Model;
using Ayende.NHibernateQueryAnalyzer.Tests.TestUtilities;
using MbUnit.Framework;

namespace Ayende.NHibernateQueryAnalyzer.Tests.Core
{
	[TestFixture]
	public class ProjectRepositoryTests
	{
		private Project current;
		private const string projectName = "NewProject";

		private IProjectsRepository repository;

		[SetUp]
		public void SetUp()
		{
			repository = TestDataUtil.CreateFileRepository();
			current = new Project(projectName);
			current.AddFile(TestDataUtil.TestConfigFile);
			current.AddFile(TestDataUtil.TestDllFile);
			current.AddFile(TestDataUtil.TestMappingFile);
			repository.SaveProject(current);
		}

		[Test]
		public void GetAllProjects()
		{
			Assert.AreEqual(1, repository.GetAllProejcts().Count);
			Project one = repository.CreateProject("count #1");
			repository.SaveProject(one);
			Assert.AreEqual(2, repository.GetAllProejcts().Count);
			Assert.AreEqual(one, repository.GetAllProejcts()[1]);
		}


		[Test]
		public void ProjectStateUdatedInDatabase()
		{
			repository.RemoveFromCache(current);
			Project prj = repository.GetProjectById(current.Id);
			Assert.IsNotNull(prj, "Project was not saved in the database");
			Assert.AreEqual(3, prj.Files.Count, "Files count is wrong");
			CollectionAssert.Contains(prj.Files,TestDataUtil.TestConfigFile);
			CollectionAssert.Contains( prj.Files, TestDataUtil.TestDllFile);
			CollectionAssert.Contains(prj.Files, TestDataUtil.TestMappingFile);
			current = prj;
		}

		[Test]
		public void SeveralProjectsWithQueries()
		{
			Project second = new Project("second");
			Query q = new Query("one", "two");
			second.AddQuery(q);
			repository.SaveProject(second);
			repository.RemoveProject(second);
		}

		[Test]
		public void SaveProjectQuery()
		{
			Query q = new Query("first query", "first query text");
			current.AddQuery(q);
			repository.SaveQuery(q);
			repository.SaveProject(current);
			repository.RemoveFromCache(current);
			Project prj = repository.GetProjectById(current.Id);
			Assert.AreEqual(1, prj.Queries.Count);
			Assert.IsNotNull(prj.GetQueryWithName(q.Name));
			Assert.AreEqual(q.OwnerProject.Id, prj.Id);
			Query queryWithName = prj.GetQueryWithName(q.Name);
			Assert.AreEqual(q.Text, queryWithName.Text);
			current = prj;
		}

		[Test]
		public void AddProject()
		{
			Assert.IsTrue(0 != current.Id, "New project id equal to zero - so it wasn't saved in database");
		}

		[Test]
		public void QuerySavedProjectbyId()
		{
			Project queried = repository.GetProjectById(current.Id);
			Assert.AreEqual(current.Id, queried.Id, "Queried project id was not equal to the saved project id.");
		}

		[Test]
		public void RemoveProject()
		{
			Project project = repository.CreateProject("Test Project #2");
			repository.SaveProject(project);
			repository.RemoveProject(project);
			Assert.IsNull(repository.GetProjectById(project.Id), "Project was not removed from the database");
		}

		[Test]
		public void QuerySavedProjectByName()
		{
			Project quried = repository.GetProjectByName(projectName);
			Assert.AreEqual(current.Id, quried.Id, "could not get the same project when quering by name");
		}

		[Test]
		public void QuerySavedProjectBySimilarName_SingleProject()
		{
			Project saved = repository.CreateProject("Test Project #1");
			repository.SaveProject(saved);
			IList list = repository.GetProjectsStartingWith("Test Project #");
			Assert.AreEqual(1, list.Count, "Wrong number of projects returned");
			Project quried = (Project) list[0];
			Assert.AreEqual(saved.Id, quried.Id, "could not get the same project when quering by similar name");
		}

		[Test]
		public void QuerySavedProjectBySimilarName_ThreeProjects()
		{
			Project first = repository.CreateProject("Project #1"), second = repository.CreateProject("Project #2"), third = repository.CreateProject("Project #3");
			repository.SaveProject(first);
			repository.SaveProject(second);
			repository.SaveProject(third);
			IList list = repository.GetProjectsStartingWith("Project #");
			Assert.AreEqual(3, list.Count, "Wrong number of projects returned");
			Project quried = (Project) list[0];
			Assert.AreEqual(first.Id, quried.Id, "could not get the same project when quering by similar name");
			quried = (Project) list[1];
			Assert.AreEqual(second.Id, quried.Id, "could not get the same project when quering by similar name");
			quried = (Project) list[2];
			Assert.AreEqual(third.Id, quried.Id, "could not get the same project when quering by similar name");
		}

		[Test]
		public void ProjectCanBeRemovedFromMemory()
		{
			repository.RemoveFromCache(current);
			Project prj = repository.GetProjectById(current.Id);
			Assert.AreNotSame(current, prj, "Returned the same object");
			current = prj;
		}

		[Test]
		public void ProjectFromNHibernate_HasContextSet()
		{
			repository.RemoveFromCache(current); //remove from session's cache
			Project prj = repository.GetProjectById(current.Id);
			Assert.AreNotSame(current, prj, "Returned the same object");
			Assert.IsNotNull(prj.Conext, "Project created by NHibernate should have a non null context");
			current = prj;
		}

		[Test]
		public void ProjectSaveToDtaabase()
		{
			Assert.AreEqual(projectName, current.Name, "Project name was not set correctly");
			Assert.IsFalse(0 == current.Id, "Project identity was not set!");
			Project loadedPrj = repository.GetProjectById(current.Id);
			Assert.IsNotNull(loadedPrj, "Project was not saved to database");
		}

		[Test]
		public void AskingForProjectThatDoesNotExistsReturnNull()
		{
			Assert.IsNull(repository.GetProjectByName("mesayeket"),"Didn't get a null when getting a project that doesn't exists");
		}

		[Test]
		public void GetUpdatedQuery()
		{
			Query q = new Query("MyTestQuery","Singing in the rain");
			current.AddQuery(q);
			repository.SaveQuery(q);
			repository.SaveProject(current);
            q.Name = "My Test Query";
			repository.SaveProject(current);
			repository.RemoveFromCache(current);
			Project prj = repository.GetProjectByName(current.Name);
			Query queryFromDb = prj.GetQueryWithName(q.Name);
			Assert.IsNotNull(queryFromDb,"Could not get query from project");
			Assert.AreEqual(q.Id, queryFromDb.Id,"Query Id is not identical");
            
		}
	}
}