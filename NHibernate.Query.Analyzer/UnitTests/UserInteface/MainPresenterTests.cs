using System;
using System.Collections;
using Ayende.NHibernateQueryAnalyzer.Model;
using Ayende.NHibernateQueryAnalyzer.UserInterface;
using Ayende.NHibernateQueryAnalyzer.UserInterface.Interfaces;
using Ayende.NHibernateQueryAnalyzer.UserInterface.Presenters;
using MbUnit.Framework;
using Rhino.Mocks;

namespace Ayende.NHibernateQueryAnalyzer.UnitTests.UserInteface
{
	[TestFixture]
	public class MainPresenterTests
	{
		#region Variables
		private IProjectsRepository repository;
		private IMainView view;
		private MockRepository mocks;
		private IMainPresenter presenter;

		#endregion

		[SetUp]
		public void SetUp()
		{
			mocks = new MockRepository();
			view = mocks.CreateMock(typeof (IMainView)) as IMainView;
			repository = mocks.CreateMock(typeof (IProjectsRepository)) as IProjectsRepository;
			presenter = new MainPresenter(this.view, this.repository);
		}

		[TearDown]
		public void TearDown()
		{
            //This make sure that all the finalizers are called before we move 
		    //the mocks to verified state. Otherwise we might get random test
		    //failure, depending how fast the test is run.
		    GC.WaitForPendingFinalizers();
		    
		    mocks.VerifyAll();    
		}

		[Test]
		public void NewProjectsAreCreatesWithDifferentNames()
		{
			ArrayList projects = new ArrayList();
			ExpectCreateNewProject(projects, MainForm.DefaultNamePrefix + "1");
			ExpectCreateNewProject(projects, MainForm.DefaultNamePrefix + "2");
			mocks.ReplayAll();
			Project prj1 = this.presenter.CreateNewProject();
			projects.Add(prj1);
			this.presenter.CreateNewProject();
		}

		[Test]
		public void DisplayNewProjetctCreatesAndDisplayProject()
		{
			string projectName = MainForm.DefaultNamePrefix + "1";
			ExpectCreateNewProject(new ArrayList(), projectName);
			view.Display(null);
			LastCall.On(view).Constraints(
				Is.TypeOf(typeof(IProjectView)) & 
				Property.Value("Title", projectName));
			SetupResult.For(view.Documents).Return(new ArrayList());

			mocks.ReplayAll();
            IProjectPresenter projectPresenter = presenter.DisplayNewProject();

            DisposePresenterView(projectPresenter);
		}

        private static void DisposePresenterView(IProjectPresenter projectPresenter)
        {
            //Need to do this here because this actually create a Control
            //which will be registered on the Finalizer thread, etc.
            //Otherwise, we get exception in the finalizer thread, and all hell breaks loose.
            projectPresenter.View.Dispose();
        }

		[Test]
		public void CloseProjectWithNoDocuments()
		{
			SetupResult.For(view.Documents).Return(new ArrayList());
			mocks.ReplayAll();
			Assert.IsTrue(presenter.CloseProject());
		}

		[Test]
		public void CloseProjectWithThreeDocumentsWithNoUnsavedData()
		{
			CreatesMockedViewsExpectingToBeClosed(3, false);
			mocks.ReplayAll();
			Assert.IsTrue(presenter.CloseProject());
		}

		[Test]
		public void CloseProjectWithOneDocumentWithUnsavedChangesCanBeCanceled()
		{
			IView[] views = CreateMockedViewsotExpectingToBeClosed();
			view.ShowUnsavedDialog(views);
			LastCall.On(view).
				Constraints(
				Property.Value("Length",1)).
				Return(null);
			
			mocks.ReplayAll();
			Assert.IsFalse(presenter.CloseProject());
		}

		[Test]
		public void CloseProjectWithDocumentContainingUnsavedChangesAskUserAndSave()
		{
			IView[] views = CreateThreeViewsWithOneUnsaved();

			Expect.On(view).
				Call(view.ShowUnsavedDialog(null)).
				Constraints(List.IsIn(views[1]) & Property.Value("Length",1)).
				Return(new IView[]{views[1]});

			Expect.On(views[1]).Call(views[1].Save()).Return(true);

			mocks.ReplayAll();

            Assert.IsTrue(presenter.CloseProject());
		}

		[Test]
		public void CloseProjectWithDocumentContainingUnsavedChangesButAskNotToSaveAnything()
		{
			IView [] views = CreateThreeViewsWithOneUnsaved();

			Expect.On(view).
				Call(view.ShowUnsavedDialog(null)).
				Constraints(List.IsIn(views[1]) & Property.Value("Length",1)).
				Return(new IView[0]);


			mocks.ReplayAll();

			Assert.IsTrue(presenter.CloseProject());
		}

		[Test]
		public void OpeningNewProjectCloseExistingOne()
		{
			CreatesMockedViewsExpectingToBeClosed(1,false);

			Expect.On(view).Call(view.SelectExistingProject()).Return(null);

			mocks.ReplayAll();
			presenter.OpenProject();
		}

		[Test]
		public void OpenProjectGetListOfAllProjectsAndShowThemToUser()
		{
			CreatesMockedViewsExpectingToBeClosed(0,false);

			string projectName = "Open Project";
			Project project = new Project(projectName);
			Expect.On(view).Call(view.SelectExistingProject()).Return(project);
			view.Display(null);
			LastCall.On(view).Constraints(
				Is.TypeOf(typeof(IProjectView)) & 
				Property.Value("Title", projectName));
		    repository.RemoveFromCache(project);
			mocks.ReplayAll();

			IProjectPresenter projectPresenter = presenter.OpenProject();
            DisposePresenterView(projectPresenter);
		}

		[Test]
		public void OpenProjectCanBeCanceledIfHasUnsavedDocuments()
		{
			CreateMockedViewsotExpectingToBeClosed();
			
			view.ShowUnsavedDialog(null);
			LastCall.On(view).IgnoreArguments().
				Return(null);

			mocks.ReplayAll();

			presenter.OpenProject();
		    
		    
		}

		[Test]
		public void SaveDocumentWhenActiveDocumentIsNullDoesNothign()
		{
			Expect.On(view).Call(view.ActiveDocument).Return(null);
			mocks.ReplayAll();
			presenter.SaveDocument();
		}

		[Test]
		public void SaveDocumentSavesTheActiveDocument()
		{
			IView activeDocument = (IView)mocks.CreateMock(typeof(IView));
			Expect.On(activeDocument).Call(activeDocument.Save()).Return(true);
			Expect.On(view).Call(view.ActiveDocument).Return(activeDocument);
			mocks.ReplayAll();
			presenter.SaveDocument();
		}

		#region Implementation Details


		private IView[] CreatesMockedViewsExpectingToBeClosed(int count, bool hasChanges)
		{
			IView [] views = new IView[count];
			for (int i = 0; i < count; i++)
			{
				views[i] = mocks.CreateMock(typeof(IView)) as IView;
				SetupResult.On(views[i]).Call(views[i].HasChanges).Return(hasChanges);
				views[i].Close(false);
			}
			SetupResult.On(view).Call(view.Documents).Return(views);
			return views;
		}

		private IView[] CreateThreeViewsWithOneUnsaved()
		{
			IView[] views = new IView[]
				{
					mocks.CreateMock(typeof(IView)) as IView,
					mocks.CreateMock(typeof(IView)) as IView,
					mocks.CreateMock(typeof(IView)) as IView
				};
			
			foreach (IView v in views)
				v.Close(false);

			SetupResult.On(views[0]).Call(views[0].HasChanges).Return(false);
			SetupResult.On(views[2]).Call(views[2].HasChanges).Return(false);
			SetupResult.On(views[1]).Call(views[1].HasChanges).Return(true);
			SetupResult.On(view).Call(view.Documents).Return(views);
			return views;
		}

		private void ExpectCreateNewProject(ArrayList projects, string projectName)
		{
			repository.GetProjectsStartingWith(MainForm.DefaultNamePrefix);
			LastCall.On(repository).Return(projects);
			repository.CreateProject(projectName);
		    Project project = new Project(projectName);
		    LastCall.On(repository).Return(project);
		    
            repository.RemoveFromCache(project);
            LastCall.Repeat.Times(0, 1);
		}

		private IView[] CreateMockedViewsotExpectingToBeClosed()
		{
			IView[] views = new IView[]{ (IView)mocks.CreateMock(typeof(IView))};
			Expect.On(views[0]).Call(views[0].HasChanges).Return(true);
			SetupResult.On(view).Call(view.Documents).Return(views);
			return views;
		}


		#endregion 
	}
}