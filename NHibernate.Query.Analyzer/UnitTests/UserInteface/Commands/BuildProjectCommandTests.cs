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


using System.Collections;
using Ayende.NHibernateQueryAnalyzer.Model;
using Ayende.NHibernateQueryAnalyzer.Tests.TestUtilities;
using Ayende.NHibernateQueryAnalyzer.UnitTests.TestUtilities;
using Ayende.NHibernateQueryAnalyzer.UserInterface.Commands;
using Ayende.NHibernateQueryAnalyzer.UserInterface.Interfaces;
using MbUnit.Framework;
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
			ExecuteInUI eiui = new ExecuteInUI();
			view.ExecuteInUIThread(null);
			LastCall.On(view).
				Callback(new ExecuteInUI.ExecuteInUIDelegate(eiui.ExecuteInUIThread));
			
			repository.SaveProject(prj);
			prj.BuildProject();
			LastCall.On(prj).
				Throw(new NHibernate.MappingException(errorMessage));
			view.ShowError(null);
			LastCall.Constraints(Text.Contains(errorMessage));
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