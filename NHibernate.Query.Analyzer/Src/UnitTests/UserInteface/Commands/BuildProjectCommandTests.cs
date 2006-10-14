using System.Collections;
using Ayende.NHibernateQueryAnalyzer.Model;
using Ayende.NHibernateQueryAnalyzer.Tests.TestUtilities;
using Ayende.NHibernateQueryAnalyzer.UnitTests.TestUtilities;
using Ayende.NHibernateQueryAnalyzer.UserInterface.Commands;
using Ayende.NHibernateQueryAnalyzer.UserInterface.Interfaces;
using NUnit.Framework;
using Rhino.Mocks;

namespace Ayende.NHibernateQueryAnalyzer.UnitTests.UserInteface.Commands
{	 
	[TestFixture]
	public class BuildProjectCommandTests
	{
		private IProjectsRepository repository;
		private IProjectView view;
		private Project prj;				   
		private MockRepository mocks;
		private ICommand build;

		[SetUp]
		public void SetUp()
		{
			mocks = new MockRepository();
			view = (IProjectView)mocks.CreateMock(typeof(IProjectView));
			repository = mocks.CreateMock(typeof(IProjectsRepository)) as IProjectsRepository;
			prj = mocks.CreateMock(typeof(Project), "Test Project") as Project;

			IList files = new ArrayList();
			files.Add(TestDataUtil.TestConfigFile);
			files.Add(TestDataUtil.TestDllFile);
			files.Add(TestDataUtil.TestMappingFile);

			SetupResult.On(prj).Call(prj.Name).Return("My Test Project");
			SetupResult.On(prj).Call(prj.Files).Return(files);

			build = new BuildProjectCommand(view,prj,repository);
		}

		[TearDown]
		public void TearDown()
		{
			mocks.VerifyAll();
		}

		[Test]
		public void BuildProjectReportExceptionOnBuild()
		{	
			string errorMessage = "Couldn't read mapping file";
			string expectedErrorMessage = errorMessage + "\r\n";
			ExecuteInUI eiui = new ExecuteInUI();
			view.ExecuteInUIThread(null);
			LastCall.On(view).
				Callback(new ExecuteInUI.ExecuteInUIDelegate(eiui.ExecuteInUIThread));
			
			repository.SaveProject(prj);
			prj.BuildProject();
			LastCall.On(prj).
				Throw(new NHibernate.MappingException(errorMessage));
			view.ShowError(expectedErrorMessage);
			view.EndWait(errorMessage);
			view.DisplayProjectState(true,false);
			mocks.ReplayAll();
			build.Execute();
		}

		[Test]
		public void BuildProjectCommandBuildsTheProject()
		{
			ExecuteInUI eiui = new ExecuteInUI();
			view.ExecuteInUIThread(null);
			LastCall.On(view).
				Callback(new ExecuteInUI.ExecuteInUIDelegate(eiui.ExecuteInUIThread));
			repository.SaveProject(prj);
			prj.BuildProject();
			view.DisplayProjectState(false,true);
			view.EndWait("Project was build successfully");
			mocks.ReplayAll();
			build.Execute();
		}

		[Test]
		public void BuildProjectInfoIsFull()
		{
			mocks.ReplayAll();
			string expectedInfo ="Project: My Test Project\r\n"+
				"Files:\r\n"+
				"\t" +TestDataUtil.TestConfigFile+"\r\n"+
				"\t" +TestDataUtil.TestDllFile+"\r\n"+
				"\t" +TestDataUtil.TestMappingFile+"\r\n";
			Assert.AreEqual(expectedInfo,build.Info);

		}

		[Test]
		public void NameIsCorrect()
		{
			mocks.ReplayAll();
			Assert.AreEqual("BuildProjectCommand",build.Name);
		}
	}
}