using System.Collections;
using System.Data;
using Ayende.NHibernateQueryAnalyzer.Model;
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
