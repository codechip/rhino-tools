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
using System.Data;
using Ayende.NHibernateQueryAnalyzer.Core.Model;
using Ayende.NHibernateQueryAnalyzer.UnitTests.TestUtilities;
using Ayende.NHibernateQueryAnalyzer.UserInterface.Commands;
using Ayende.NHibernateQueryAnalyzer.UserInterface.Interfaces;
using Ayende.NHibernateQueryAnalyzer.Utilities;
using MbUnit.Framework;
using Rhino.Mocks;

namespace Ayende.NHibernateQueryAnalyzer.UnitTests.UserInteface.Commands
{
	[TestFixture]
	public class ExecuteQueryCommandTests
	{
		private ICommand command;
		private TypedParameter[] parameters;
		private string query;
		private IQueryView view;
		private Project prj;
		private TypedParameter param;
		private MockRepository mocks;

		[SetUp]
		public void SetUp()
		{
			mocks = new MockRepository();
			prj = mocks.CreateMock(typeof(Project),"New Project") as Project;
			view = mocks.CreateMock(typeof(IQueryView)) as IQueryView;
			query = "from TestProject tp where tp.Id = :id";
			param = new TypedParameter("id",typeof(int),1);
			parameters = new TypedParameter[]
				{  param,	};
			command = new ExecuteQueryCommand(this.view,this.prj,this.query,this.parameters);
		}

		[TearDown]
		public void TearDown()
		{
			mocks.VerifyAll();
		}

		[Test]
		public void NameIsCorrect()
		{
			mocks.ReplayAll();
			Assert.AreEqual("ExecuteQueryCommand",command.Name);
		}

		[Test]
		public void InfoIsCorrect()
		{
			mocks.ReplayAll();
			Assert.AreEqual("Query: " +query,command.Info);
		}

		[Test]
		public void ExecuteQuerySendsResultToUI()
		{
			ExecuteInUI eiu = new ExecuteInUI();
			IList objectGraph = new ArrayList();
			DataSet ds = new DataSet();
			Expect.On(prj).Call(prj.RunHql(query,parameters)).Return(objectGraph);
			Expect.On(prj).Call(prj.RunHqlAsRawSql(query,parameters)).Return(ds);
			view.ExecuteInUIThread(null);
			LastCall.On(view).Callback(new ExecuteInUI.ExecuteInUIDelegate(eiu.ExecuteInUIThread));
			view.DisplayDataSet(ds);
			view.DisplayObjectGraph(objectGraph);
			view.ShowObjectGraph();
			view.EndWait("Finished executing query");

			mocks.ReplayAll();
			command.Execute();
		}

		[Test]
		public void ExecuteQueryReportsFailure()
		{
			string errorMsg = "Database connection closed";
			DataException exception = new DataException(errorMsg);
			ExecuteInUI eiu = new ExecuteInUI();
			IList objectGraph = new ArrayList();
			Expect.On(prj).Call(prj.RunHql(query,parameters)).Return(objectGraph);
			Expect.On(prj).Call(prj.RunHqlAsRawSql(query,parameters)).Throw(exception);

			view.ExecuteInUIThread(null);
			LastCall.On(view).Callback(new ExecuteInUI.ExecuteInUIDelegate(eiu.ExecuteInUIThread));
			view.AddException(exception);

			view.EndWait("An exception occured executing query");

			mocks.ReplayAll();
			command.Execute();
		}
	}
}
