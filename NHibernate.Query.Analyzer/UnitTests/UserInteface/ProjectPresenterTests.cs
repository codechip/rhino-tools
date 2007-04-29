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
using System.Reflection;
using Ayende.NHibernateQueryAnalyzer.Model;
using Ayende.NHibernateQueryAnalyzer.UserInterface.Interfaces;
using Ayende.NHibernateQueryAnalyzer.UserInterface.Presenters;
using MbUnit.Framework;
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
