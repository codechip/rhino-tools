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
using System.Collections;
using System.Data;
using Ayende.NHibernateQueryAnalyzer.Model;
using Ayende.NHibernateQueryAnalyzer.ProjectLoader;
using Ayende.NHibernateQueryAnalyzer.UnitTests.TestUtilities;
using Ayende.NHibernateQueryAnalyzer.UnitTests.UserInteface;
using Ayende.NHibernateQueryAnalyzer.UserInterface.Interfaces;
using Ayende.NHibernateQueryAnalyzer.Utilities;
using NHibernate;
using MbUnit.Framework;
using Rhino.Mocks;

namespace Ayende.NHibernateQueryAnalyzer.UnitTests.IntegrationTests
{
	[TestFixture]
	//[Category("Integration")]
	public class QueryPresenterIntegrationTests
	{
		private Project prj;
		private IQueryView queryView;
		private IMainPresenter mainPresenter;
		private IQueryPresenter presenter;
		private string badQueryExceptionMessage = "query must begin with SELECT or FROM: this [this is a bad query]";
		private MockRepository mocks;
		private readonly string badHqlQuery = "this is a bad query";

		#region Sql & Hql statements

		public const string parametrizedOneSql = "select testprojec0_._id as column1_0_, testprojec0_.Data as Data0_ from TestFile testprojec0_ where (testprojec0_._id=@p0 )";
		public const string nonParametrizedSql = "select testprojec0_._id as column1_0_, testprojec0_.Data as Data0_ from TestFile testprojec0_ where (testprojec0_._id=1 )";
		public const string parametrizedOne = "from TestProject tp where tp.Id = :id";
		public const string nonParametrized = "from TestProject tp where tp.Id = 1";

		#endregion

		[TestFixtureSetUp]
		public void TestFixtureSetup()
		{
			prj = QueryPresenterTests.BuildProject();
			prj.BuildProject();
		}

		[TestFixtureTearDown]
		public void TestFixtureTearDown()
		{
			prj.Dispose();
		}

		[SetUp]
		public void SetUp()
		{
			mocks = new MockRepository();
			queryView = (IQueryView)mocks.CreateMock(typeof(IQueryView));
			mainPresenter = (IMainPresenter)mocks.CreateMock(typeof(IMainPresenter));
			SetupResult.On(mainPresenter).Call(mainPresenter.CurrentProject).Return(prj);
			QueryPresenterWithMockView.MockView = queryView;
			presenter = new QueryPresenterWithMockView(mainPresenter);
		}

		[TearDown]
		public void TearDown()
		{
			mocks.VerifyAll();
		}

		[Test]
		public void TranslateHql_NonParametrized()
		{
			SetupParameters(new Hashtable(),nonParametrized);
			queryView.SqlQueryText= nonParametrizedSql;

			mocks.ReplayAll();
			Assert.AreEqual(nonParametrizedSql, presenter.TranslateHql());
		}

		[Test]
		public void TranslateHql_Parametrized()
		{
			SetupMockParameter();
			SetupResult.On(queryView).Call(queryView.HqlQueryText).Return(parametrizedOne);
			
			queryView.SqlQueryText = parametrizedOneSql;

			mocks.ReplayAll();
			Assert.AreEqual(parametrizedOneSql,presenter.TranslateHql());
		}

		[Test]
		public void TranslateHql_BadQueryCallAddException()
		{
			SetupResult.On(queryView).Call(queryView.HqlQueryText).Return(badHqlQuery);
			SetupResult.On(queryView).Call(queryView.Parameters).Return(new Hashtable());
			queryView.AddException(null);
			LastCall.On(queryView).
				Constraints(Is.TypeOf(typeof(QueryException)) & 
					Property.Value("Message",badQueryExceptionMessage));

			mocks.ReplayAll();
			string result = presenter.TranslateHql();
			Assert.AreEqual(badQueryExceptionMessage,result);
		}

		[Test]
		public void ExecuteQuery_NoParameters()
		{
			ExecuteInUI eiu = new ExecuteInUI();
			SetupCallbackEnqueCommandAndExecuteInUI(eiu);
			SetupParameters(new Hashtable(),nonParametrized);
			queryView.StartWait("Executing query",100,1000);
			SetupCallbackForCheckingObjectGraphAndDataSet();
			queryView.ShowObjectGraph();
			queryView.EndWait("Finished executing query");
			
			mocks.ReplayAll();
			presenter.ExecuteQuery();
			Assert.IsTrue(eiu.ExecuteCommandCalled);
			Assert.IsTrue(eiu.ExecuteInUICalled);
		}

		private void SetupCallbackForCheckingObjectGraphAndDataSet()
		{
			queryView.DisplayObjectGraph(null);
			LastCall.On(queryView).
				Callback(new IListDelegate(checkExecuteQuery_NoParameters_ObjectGraph));
			queryView.DisplayDataSet(null);
			LastCall.On(queryView).
				Callback( new DataSetDelegate(checkExecuteQuery_NoParameters_DataSet));
		}

		[Test]
		public void ExecuteQuery_WithParameter()
		{
			ExecuteInUI eiu = new ExecuteInUI();
			SetupMockParameter();
			SetupCallbackEnqueCommandAndExecuteInUI(eiu);
			SetupResult.On(queryView).Call(queryView.HqlQueryText).Return(parametrizedOne);
			queryView.StartWait("Executing query",100,1000);
			SetupCallbackForCheckingObjectGraphAndDataSet();
			queryView.ShowObjectGraph();
			queryView.EndWait("Finished executing query");

			mocks.ReplayAll();
			presenter.ExecuteQuery();			
			
			Assert.IsTrue(eiu.ExecuteCommandCalled);
			Assert.IsTrue(eiu.ExecuteInUICalled);
		}

		private void SetupCallbackEnqueCommandAndExecuteInUI(ExecuteInUI eiu)
		{
			mainPresenter.EnqueueCommand(null);
			LastCall.On(mainPresenter).
				Callback(new ExecuteInUI.ExecuteCommandDelegate(eiu.ExecuteCommand));
			queryView.ExecuteInUIThread(null);
			LastCall.On(queryView).
				Callback(new ExecuteInUI.ExecuteInUIDelegate(eiu.ExecuteInUIThread));
		}

		[Test]
		public void ExceptionInQuery_DoesNotThrowButReportsException()
		{
			ExecuteInUI eiu = new ExecuteInUI();
			SetupCallbackEnqueCommandAndExecuteInUI(eiu);
			SetupParameters(new Hashtable(),badHqlQuery);
			queryView.StartWait("Executing query",100,1000);
			queryView.EndWait("An exception occured executing query");
			queryView.AddException(null);
			LastCall.On(queryView).
				Constraints(
					Is.TypeOf(typeof(QueryException)) &
					Property.Value("Message",badQueryExceptionMessage));
			mocks.ReplayAll();
			presenter.ExecuteQuery();
			
			Assert.IsTrue(eiu.ExecuteCommandCalled);
			Assert.IsTrue(eiu.ExecuteInUICalled);
		}

		[Test]
		public void ExceptionThatIsNotQuerExceptionDoesNotThrowButReports()
		{
			ExecuteInUI eiu = new ExecuteInUI();
			SetupCallbackEnqueCommandAndExecuteInUI(eiu);
			SetupParameters(new Hashtable(),"from NotExists");
			queryView.StartWait("Executing query",100,1000);
			queryView.EndWait("An exception occured executing query");
			queryView.AddException(null);
			LastCall.On(queryView).
				Constraints(
				Is.TypeOf(typeof(QueryException)) &
				Property.Value("Message","in expected: <end-of-text> (possibly an invalid or unmapped class name was used in the query) [from NotExists]"));
			mocks.ReplayAll();
			presenter.ExecuteQuery();
			
			Assert.IsTrue(eiu.ExecuteCommandCalled);
			Assert.IsTrue(eiu.ExecuteInUICalled);
		}

		#region Supporting Methods

		delegate bool DataSetDelegate(DataSet ds);

		private static bool checkExecuteQuery_NoParameters_DataSet(DataSet ds )
		{
			DataTable table = ds.Tables["TestFile"];
			Assert.IsNotNull(table);
			Assert.AreEqual(1,table.Rows.Count);
			Assert.IsNotNull(table.Columns[0]);
			Assert.AreEqual(1,table.Rows[0][0]);
			return true;
		}

		private void SetupMockParameter()
		{
			Hashtable parameters = new Hashtable();
			parameters["id"] = new TypedParameter("id",typeof(int),1);
			SetupResult.On(queryView).Call(queryView.Parameters).Return(parameters);
		}

		delegate bool IListDelegate(IList list);

		private bool checkExecuteQuery_NoParameters_ObjectGraph(IList list)
		{
			RemoteObject ro;
			Assert.AreEqual(1,list.Count);
			ro = list[0] as RemoteObject;
			Assert.IsNotNull(ro);
			Assert.AreEqual("TestProject.TestProject",ro.FullTypeName);
			Assert.AreEqual("1",ro["Id"].ToString());
			return true;
		}

		#endregion

		private void SetupParameters(IDictionary parameters, string query)
		{
			SetupResult.On(queryView).Call(queryView.Parameters).Return(parameters);
			SetupResult.On(queryView).Call(queryView.HqlQueryText).Return(query);
		}
	}
}