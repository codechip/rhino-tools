using System;
using System.Collections;
using System.IO;
using Ayende.NHibernateQueryAnalyzer.Model;
using Ayende.NHibernateQueryAnalyzer.Tests.TestUtilities;
using Ayende.NHibernateQueryAnalyzer.UserInterface.Interfaces;
using Ayende.NHibernateQueryAnalyzer.UserInterface.Presenters;
using Ayende.NHibernateQueryAnalyzer.Utilities;
using NHibernate;
using MbUnit.Framework;
using Rhino.Mocks;

namespace Ayende.NHibernateQueryAnalyzer.UnitTests.UserInteface
{
	[TestFixture]
	public class QueryPresenterTests
	{
		private IQueryPresenter presenter;
		private IQueryView queryView;
		private IMainPresenter mainPresenter;
		private IProjectsRepository repository;
		private Project prj;
		private Query query;
		private MockRepository mocks;

		#region Strings


		/// <summary>
		/// Do not use for methods that call for NHibernate, as TestProject doesn't have Name or Average properties.
		/// This is just to test that NQA can recognize queries with several parameters
		/// </summary>
		public const string parametrizedThree = "from TestProject tp where tp.Id = :id and tp.Name = :name and tp.Average = :avg";		
		public const string nonParametrizedSql="select testproj0_._id as _id, testproj0_.Data as Data from TestFile testproj0_ where (testproj0_._id=1)";
		public const string parametrizedOne = "from TestProject tp where tp.Id = :id";
		public const string	nonParametrized = "from TestProject tp where tp.Id = 1";
		private string queryText = "query text";
		private string queryOldName = "old name";
		private string newName = "new name";
		private string oldName = "old name";
		private string askYesNoQuestion = "A query with thename 'new name' already exists, are you sure you want to overwrite it?";
		private string askYesNoTitle = "Overwrite query?";
		private readonly string askTitle = "Rename query to:";

		#endregion

		[SetUp]
		public void SetUp()
		{
			mocks = new MockRepository();
			mainPresenter = (IMainPresenter)mocks.CreateMock(typeof(IMainPresenter));
			queryView = (IQueryView)mocks.CreateMock(typeof(IQueryView));
			repository = (IProjectsRepository)mocks.CreateMock(typeof(IProjectsRepository));
			prj = BuildProject();
			query = new Query(queryOldName,queryText);
			prj.AddQuery(query);
			SetupResult.For(mainPresenter.CurrentProject).Return(prj);
			SetupResult.For(mainPresenter.Repository).Return(repository);
			QueryPresenterWithMockView.MockView = queryView;
			presenter = new QueryPresenterWithMockView(mainPresenter,query);
		}

		internal static Project BuildProject()
		{
			Project prj = new Project("Test Project");
			prj.AddFile(TestDataUtil.TestConfigFile);
			prj.AddFile(TestDataUtil.TestDllFile);
			prj.AddFile(TestDataUtil.TestMappingFile);
			return prj;
		}

		[TearDown]
		public void TearDown()
		{
			mocks.VerifyAll();
		}

		[Test]
		public void HasParameters_ReturnTrueWhenThereIsParameter()
		{
			mocks.ReplayAll();
			Assert.IsTrue(presenter.HasParameters(parametrizedOne),"Failed to recognized parametrized query");
		}

		[Test]
		public void HasParameters_ReturnTrueWhenThereAreSeveralParameters()
		{
			mocks.ReplayAll();
			Assert.IsTrue(presenter.HasParameters(parametrizedThree),"Failed to recognized parametrized query");			
		}

		[Test]
		public void HasParameters_ReturnFalseWhenNoParameterInQuery()
		{
			mocks.ReplayAll();
			Assert.IsFalse(presenter.HasParameters(nonParametrized),"Errorously recognized non parametrized query as parametrized");			
		}

		[Test]
		public void AllParametersSet_OneParameterSet_ReturnTrue()
		{
			Hashtable parameters = new Hashtable();
			parameters["id"] = new TypedParameter("id",typeof(int),1);
			SetupQueryView(parameters, parametrizedOne);

			mocks.ReplayAll();
			Assert.IsTrue(presenter.AllParametersSet(),"Didn't recognized that the parameter is set correctly");
		}

		[Test]
		public void AllParametersSet_ThreeParametersSet_ReturnTrue()
		{
			Hashtable parameters = new Hashtable();
			parameters["id"] = new TypedParameter("id",typeof(int),1);
			parameters["name"] = new TypedParameter("name",typeof(string),"just a name");
			parameters["avg"] = new TypedParameter("avg",typeof(float),85.4f);
			SetupQueryView(parameters, parametrizedThree);
			mocks.ReplayAll();
			Assert.IsTrue(presenter.AllParametersSet(),"Didn't recognized that all parameters are set correctly");
		}

		[Test]
		public void AllParametersSet_ReturnTrueIfNoParameters()
		{
			SetupResult.For(queryView.HqlQueryText).Return(nonParametrized);
			
			mocks.ReplayAll();
			Assert.IsTrue(presenter.AllParametersSet(),"Didn't recognized that there are no parameters");
		}

		[Test]
		public void AllParametersSet_ReturnFalseIfParameterNotSet()
		{
			Hashtable parameters = new Hashtable();
			parameters["id"] = new TypedParameter("id",typeof(int),1);
			parameters["name"] = new TypedParameter("name",typeof(string),"just a name");
			SetupQueryView(parameters, parametrizedThree);
			
			mocks.ReplayAll();
			Assert.IsFalse(presenter.AllParametersSet(),"Failed to recognized a parameter that wasn't set.");
		}

		[Test]
		public void ReplaceException_ReturnTrueIfExceptionsAreQueryException()
		{
			QueryException e1 = new QueryException("first"),e2 = new QueryException("second");
			
			mocks.ReplayAll();
			Assert.IsTrue(presenter.ReplaceException(e1,e2));
		}

		[Test]
		public void ReplaceException_ReturnTrueIfExceptionAreSameTypeAndSameMessage()
		{
			FileNotFoundException e1 = new FileNotFoundException("io error"),
				e2 = new FileNotFoundException("io error");

			mocks.ReplayAll();
			Assert.IsTrue(presenter.ReplaceException(e1,e2));
		}

		[Test]
		public void ReplaceException_ReturnFalseIfExceptionsDifferentTypes()
		{
			IOException e1 = new IOException("blah");
			FileNotFoundException e2 = new FileNotFoundException("blah2");

			mocks.ReplayAll();
			Assert.IsFalse(presenter.ReplaceException(e1,e2));
		}

		[Test]
		public void ReplaceException_ReturnFalseIfExceptionSameTypeDifferentMessage()
		{
			FileNotFoundException e1 = new FileNotFoundException("io error"),
				e2 = new FileNotFoundException("second io error");

			mocks.ReplayAll();
			Assert.IsFalse(presenter.ReplaceException(e1,e2));
		}

		[Test]
		public void SaveQueryAs_CanBeCanceled()
		{
			Expect.On(queryView).Call(queryView.Title).Return(oldName);
			Expect.On(queryView).Call(queryView.Ask(askTitle,oldName)).Return(null);

			mocks.ReplayAll();
			Assert.IsFalse(presenter.SaveQueryAs());
		}

		[Test]
		public void SaveQueryAs_NewNameWithoutConflicts()
		{
			Expect.On(queryView).Call(queryView.Ask(askTitle,oldName)).Return(newName);
			Expect.On(queryView).Call(queryView.Title).Return(queryOldName);
			queryView.Title = this.newName;
			queryView.HasChanges = false;
			repository.SaveQuery(query);
			
			mocks.ReplayAll();
			Assert.IsTrue(presenter.SaveQueryAs());
			Assert.AreEqual(this.newName,query.Name);
		}

		[Test]
		public void SaveQueryAs_WithNameConflict_UserCancel()
		{
			Expect.On(queryView).Call(queryView.Ask(askTitle,oldName)).Return(newName);
			Expect.On(queryView).Call(queryView.Title).Return(queryOldName);
			Query newQuery = new Query(newName,queryText);
			prj.AddQuery(newQuery);
			Expect.On(queryView).Call(queryView.AskYesNo(askYesNoQuestion,askYesNoTitle)).Return(false);
																																	
			mocks.ReplayAll();																													
			Assert.IsFalse(presenter.SaveQueryAs());
		}

		[Test]
		public void SaveQueryAs_WithNameConflict_UserApproves()
		{
			repository.SaveQuery(query);
			Expect.On(queryView).Call(queryView.Ask(askTitle,oldName)).Return(newName);
			Expect.On(queryView).Call(queryView.Title).Return(queryOldName);
			queryView.Title = this.newName;
			queryView.HasChanges = false;
			Query newQuery = new Query(newName,queryText);
			prj.AddQuery(newQuery);
			Expect.On(queryView).Call(queryView.AskYesNo(askYesNoQuestion,askYesNoTitle)).Return(true);
			
			mocks.ReplayAll();
			Assert.IsTrue(presenter.SaveQueryAs());
			Assert.AreEqual(newName,query.Name);
			Assert.AreEqual(1,prj.Queries.Count,"Two queries exists where one should've been removed");
		}

		[Test]
		public void SaveQuery_WithDefaultName_AskToChangeName()
		{
			repository.SaveQuery(query);
			query.Name = QueryPresenter.DefaultName;
			Expect.On(queryView).Call(queryView.Ask(askTitle,oldName)).Return(newName);
			Expect.On(queryView).Call(queryView.Title).Return(queryOldName);
			queryView.Title = this.newName;
			queryView.HasChanges = false;

			mocks.ReplayAll();
			Assert.IsTrue(presenter.SaveQuery());
			Assert.AreEqual(newName,query.Name);
		}

		[Test]
		public void SaveQuery_NamedQuery()
		{
			queryView.HasChanges = false;
			repository.SaveQuery(query);

			mocks.ReplayAll();
			Assert.IsTrue(presenter.SaveQuery());
		}

		private void SetupQueryView(Hashtable parameters, string hqlQuery)
		{
			SetupResult.For(queryView.Parameters).Return(parameters);
			SetupResult.For(queryView.HqlQueryText).Return(hqlQuery);
		}
	}
	public class QueryPresenterWithMockView: QueryPresenter
	{
		private static IQueryView mockView;

		public QueryPresenterWithMockView(IMainPresenter mainPresenter, Query query) : base(mainPresenter, query)
		{
		}

		public QueryPresenterWithMockView(IMainPresenter mainPresenter) : base(mainPresenter)
		{
		}

		protected override IQueryView CreateView()
		{
			if(mockView==null)
				throw new InvalidOperationException("QueryPresenterWithMockView was called without setting the MockView property!");

			//This is to ensure that we always gives QueryPresenter a non null value
			//or throw an exception. Otherwise, one test can set it up, and all the another may use this and we won't get the reuslt we want.
			IQueryView tmp = mockView;
			mockView = null;
			return tmp;
		}

		public static IQueryView MockView
		{
			set { mockView = value; }
		}
	}

}