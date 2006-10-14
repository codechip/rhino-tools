using System;
using System.Reflection;
using Ayende.NHibernateQueryAnalyzer.Model;
using Ayende.NHibernateQueryAnalyzer.UserInterface.Interfaces;
using Ayende.NHibernateQueryAnalyzer.UserInterface.Presenters;
using NUnit.Framework;
using Rhino.Mocks;

namespace Ayende.NHibernateQueryAnalyzer.UnitTests.UserInteface
{
	[TestFixture]
	public class ProjectPresenterTests
	{
		private IMainPresenter mainPresenter;
		private IProjectsRepository repository;
		private IProjectView projectView;
		private IProjectPresenter presenter;
		private Project prj;
		private string newProjectName = "New Project Name";
		private MockRepository mocks;
		private readonly string overwriteProjecTitle = "Overwrite project?";
		private readonly string overwriteProject = "A project with the name 'New Project Name' already exists, are you sure you want to overwrite it?";
		private readonly string answer = "Test Project";
		private readonly string question = "Project name:";

		[SetUp]
		public void SetUp()
		{
			mocks = new MockRepository();
			prj = QueryPresenterTests.BuildProject();
			mainPresenter = (IMainPresenter)mocks.CreateMock(typeof(IMainPresenter));;
			repository = (IProjectsRepository)mocks.CreateMock(typeof(IProjectsRepository));;
			projectView = (IProjectView)mocks.CreateMock(typeof(IProjectView));
			ProjectPresenterWithMockView.MockView = projectView;
			SetupResult.On(mainPresenter).Call(mainPresenter.Repository).Return(repository);
			presenter = new ProjectPresenterWithMockView(prj,mainPresenter);
		}

		[TearDown]
		public void TearDown()
		{
			mocks.VerifyAll();
		}

		[Test]
		public void CurrentProjectConfigurationView_ReturnTheSameAsItGot()
		{
			mocks.ReplayAll();
			Assert.AreSame(projectView,this.presenter.View);
		}


		[Test]
		public void SaveProjectAs_CanBeCanceled()
		{
			Expect.On(projectView).Call(projectView.Title).Return(prj.Name);
			Expect.On(projectView).Call(projectView.Ask(question,answer)).Return(null);
			mocks.ReplayAll();
			Assert.IsFalse(presenter.SaveProjectAs());			
		}

		[Test]
		public void SaveProjectAs_NewNameWithoutConflicts()
		{
				Expect.On(projectView).
					Call(projectView.Ask(question,answer)).
					Return( newProjectName);
				Expect.On(projectView).
					Call(projectView.Title).
					Return(prj.Name);
				projectView.Title = newProjectName;
				Expect.On(repository).
					Call(repository.GetProjectByName(newProjectName)).
					Return(null);
				projectView.HasChanges = false;
				repository.SaveProject(prj);
				mocks.ReplayAll();
				Assert.IsTrue(presenter.SaveProjectAs());
				Assert.AreEqual(newProjectName,prj.Name);
		}

		[Test]
		public void SaveProjectAs_WithNameConflict_UserCancel()
		{
			Expect.On(projectView).
				Call(projectView.Ask(question,answer)).
				Return( newProjectName);
			Expect.On(projectView).
				Call(projectView.Title).
				Return(prj.Name);

			Expect.On(projectView).
				Call(projectView.AskYesNo(this.overwriteProject,this.overwriteProjecTitle)).
				Return(false);
				
			Expect.On(repository).
				Call(repository.GetProjectByName(newProjectName)).
				Return(prj);
			
			mocks.ReplayAll();
			Assert.IsFalse(presenter.SaveProjectAs());
		}

		[Test]
		public void SaveProjectAs_WithNameConflict_UserApprove()
		{
			Project oldProject = new Project(this.newProjectName);
			Expect.On(projectView).
				Call(projectView.Ask(question,answer)).
				Return( newProjectName);
			Expect.On(projectView).
				Call(projectView.Title).
				Return(prj.Name);
			projectView.Title = newProjectName;
			Expect.On(projectView).
				Call(projectView.AskYesNo(this.overwriteProject,this.overwriteProjecTitle)).
				Return(true);
			projectView.HasChanges = false;
				
			Expect.On(repository).
				Call(repository.GetProjectByName(newProjectName)).
				Return(oldProject);
			repository.RemoveProject(oldProject);
			repository.SaveProject(prj);
			mocks.ReplayAll();
			Assert.IsTrue(presenter.SaveProjectAs());
			Assert.AreEqual(newProjectName,prj.Name);
		}

		[Test]
		public void SaveProject_FirstTimeSave_AskForName_ThenCancel()
		{
			Expect.On(projectView).
				Call(projectView.Ask(question,answer)).
				Return( newProjectName);
			Expect.On(projectView).
				Call(projectView.Title).
				Return(prj.Name);
			projectView.Title = newProjectName;
			projectView.HasChanges = false;
			Expect.On(repository).
				Call(repository.GetProjectByName(newProjectName)).
				Return(null);
		
			repository.SaveProject(prj);
			mocks.ReplayAll();
			Assert.IsTrue(presenter.SaveProject());
			Assert.AreEqual(newProjectName,prj.Name);
		}

		[Test]
		public void SaveProject_SecondTimeSave_SavesSilently()
		{
			typeof(Project).GetField("id", BindingFlags.NonPublic|BindingFlags.GetField|BindingFlags.Instance).SetValue(prj,1);
			repository.SaveProject(prj);
			projectView.HasChanges = false;
			mocks.ReplayAll();
			Assert.IsTrue(presenter.SaveProject());
		}

		public class ProjectPresenterWithMockView : ProjectPresenter
		{
			private static IProjectView mockView;

			protected override IProjectView CreateView(IMainPresenter mainPresenter)
			{
				if(mockView==null)
					throw new InvalidOperationException("Mock View was not set for this instance");
				//avoid using the same instance of mocked view for several instaces of 
				//the mocked view
				IProjectView tmp = mockView;	
				mockView = null;
				return tmp;

			}

			public static IProjectView MockView
			{
				set
				{
					mockView = value;
				}
			}

			public ProjectPresenterWithMockView(Project prj, IMainPresenter mainPresenter) : base(prj, mainPresenter)
			{
			}
		}

		
	}
}
