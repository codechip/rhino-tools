using System.Collections;
using Ayende.NHibernateQueryAnalyzer.UserInterface.Interfaces;
using Ayende.NHibernateQueryAnalyzer.Utilities;
using MbUnit.Framework;
using Rhino.Mocks;

namespace Ayende.NHibernateQueryAnalyzer.UnitTests.UserInteface
{
	[TestFixture]
	public class QueryPresenter_HandlingParametersTests
	{
		private IQueryPresenter context;
		private Hashtable parameters;
		private IQueryView queryView;
		private MockRepository mocks;

		[SetUp]
		public void SetUp()
		{
			mocks = new MockRepository();
			queryView = mocks.CreateMock(typeof(IQueryView)) as IQueryView;
			
			parameters = new Hashtable();
			parameters["id"] = new TypedParameter("id",typeof(int),1);

			SetupResult.On(queryView).Call(queryView.Parameters).Return(parameters);

			QueryPresenterWithMockView.MockView = queryView;
			context = new QueryPresenterWithMockView(null);

		}

		[TearDown]
		public void TearDown()
		{
			mocks.VerifyAll();
		}


		[Test]
		public void QueryCanBeTranslated_OneParameter()
		{
			SetupResult.On(queryView).Call(queryView.HqlQueryText).
				Return(QueryPresenterTests.parametrizedOne);
			
			mocks.ReplayAll();
			Assert.IsTrue(context.QueryCanBeTranslated(),
				"A parametrized query with the parameter properly set was recognized as incomplete");
		}

		[Test]
		public void QueryCanBetranslated_ThreeParameters()
		{
			SetupResult.On(queryView).Call(queryView.HqlQueryText).
				Return(QueryPresenterTests.parametrizedThree);
			parameters["name"] = new TypedParameter("name",typeof(string),"ayende");
			parameters["avg"] = new TypedParameter("avg",typeof(float),4.5f);


			mocks.ReplayAll();
			Assert.IsTrue(context.QueryCanBeTranslated(),
				"A parametrized query with all parameters properly set was recognized as incomplete");
		}

		[Test]
		public void QueryCanBeTranslated_OneParameterMissing()
		{
			SetupResult.On(queryView).Call(queryView.HqlQueryText).
				Return(QueryPresenterTests.parametrizedOne);
			parameters.Clear();

			mocks.ReplayAll();
			Assert.IsFalse(context.QueryCanBeTranslated()
				, "A parametrized query with no parameter set was recognized as good");	
		}

		[Test]
		public void QueryCanBeTranslated_ThreeParameters_SomeMissing()
		{
			SetupResult.On(queryView).Call(queryView.HqlQueryText).
				Return(QueryPresenterTests.parametrizedThree);
			parameters["avg"] = new TypedParameter("avg",typeof(float),4.5f);

			mocks.ReplayAll();
			Assert.IsFalse(context.QueryCanBeTranslated(),
				"A parametrized query without all parameters set was recognized as good");
		}

		[Test]
		public void HandleParameters_RecognizeGoodParameter()
		{
			SetupResult.On(queryView).Call(queryView.HqlQueryText).
				Return(QueryPresenterTests.parametrizedOne);
			queryView.SetParameterMissing("id",false);

			mocks.ReplayAll();
			context.AnalyzeParameters();
		}

		[Test]
		public void HandleParameters_RecognizeMissingParameters()
		{
			SetupResult.On(queryView).Call(queryView.HqlQueryText).
				Return(QueryPresenterTests.parametrizedThree);
			queryView.SetParameterMissing("id",false);
			queryView.SuggestParameter("name");
			queryView.SuggestParameter("avg");

			mocks.ReplayAll();
			context.AnalyzeParameters();	
		}

		[Test]
		public void HandleParameters_RecognizedSurplusParameters()
		{
			SetupResult.On(queryView).Call(queryView.HqlQueryText).
				Return(QueryPresenterTests.parametrizedOne);
			parameters["name"] = new TypedParameter("name",typeof(string),"ayende");
			parameters["avg"] = new TypedParameter("avg",typeof(float),4.5f);
			
			queryView.SetParameterMissing("name",true);
			queryView.SetParameterMissing("id",false);
			queryView.SetParameterMissing("avg",true);
			

			mocks.ReplayAll();

			context.AnalyzeParameters();	
		}

	}
}