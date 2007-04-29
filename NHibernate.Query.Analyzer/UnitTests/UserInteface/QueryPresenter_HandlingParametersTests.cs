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