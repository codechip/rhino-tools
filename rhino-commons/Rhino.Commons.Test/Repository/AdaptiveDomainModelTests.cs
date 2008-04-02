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
using System.IO;
using System.Collections.Generic;
using MbUnit.Framework;
using NHibernate;
using NHibernate.Criterion;
using Rhino.Commons.ForTesting;

namespace Rhino.Commons.Test.Repository
{
	[TestFixture]
	public class AdaptiveDomainModelTests : RepositoryTestsBase
	{
		[SetUp]
		public void Setup()
		{
			base.OneTimeTestInitialize();
			string path = Path.GetFullPath(@"Repository\Windsor.config");
			FixtureInitialize(PersistenceFramework.NHibernate, path, MappingInfoForRepositoryTests);


			CurrentContext.CreateUnitOfWork();
			//we use a separate container here, because we care only about what is registered in the container
			//not anything else
			IoC.Container.AddComponent<IFetchingStrategy<IParent>, MyFetchingStrategy>();
			EntitiesToRepositories.Register(
				IoC.Container, 
				UnitOfWork.CurrentSession.SessionFactory, 
				typeof (NHRepository<>),
			    delegate(Type type)
			    {
			    	return typeof (IParent).Assembly == type.Assembly;
			    });
			MyFetchingStrategy.Apply_Called = false;
			CriteriaBatch.ProcessCriteria += RepositoryFetchingStrategy.ApplyFetchingRules;
		}

		[TearDown]
		public void TearDown()
		{
			CurrentContext.DisposeUnitOfWork();
			CriteriaBatch.ProcessCriteria -= RepositoryFetchingStrategy.ApplyFetchingRules;
			IoC.Reset();
		}

		[Test]
		[ExpectedException(typeof(ArgumentException),
			"Repository must be a type inheriting from IRepository<T>, and must be an open generic type. Sample: typeof(NHRepository<>).")]
		public void ShouldThrowIfPassedNonRepositoryType()
		{
			EntitiesToRepositories.Register(
				IoC.Container,
				UnitOfWork.CurrentSession.SessionFactory,
				typeof(int),
				delegate(Type type) { return false; });
		}

		[Test]
		[ExpectedException(typeof(ArgumentException),
			"Repository must be a type inheriting from IRepository<T>, and must be an open generic type. Sample: typeof(NHRepository<>).")]
		public void ShouldThrowIfPassedOpenRepositoryType()
		{
			EntitiesToRepositories.Register(
				IoC.Container,
				UnitOfWork.CurrentSession.SessionFactory,
				typeof(NHRepository<Parent>),
				delegate(Type type) { return false; });
		}

		[Test]
		public void CanRegisterInterfaceTypesFromSessionFactory()
		{
			Assert.IsNotNull(IoC.Resolve<IRepository<IParent>>());
		}

		[Test]
		public void WhenRequestingItemByInterface_WillReturnRepositoryImpl_OfInterface()
		{
			Assert.IsInstanceOfType(typeof(NHRepository<IParent>), IoC.Resolve<IRepository<IParent>>());
		}

		[Test]
		public void WhenRequestingItemByInterface_WillReturnRepositoryImpl_OfInterface_AndSetConcreteTypeToEntity()
		{
			NHRepository<IParent> repos = (NHRepository<IParent>)IoC.Resolve<IRepository<IParent>>();
			Assert.AreEqual(typeof(Parent), repos.ConcreteType);
		}

		[Test]
		public void WillCallFetchingStrategyWhenTheyExists_ICriterion()
		{
			IRepository<IParent> repository = IoC.Resolve<IRepository<IParent>>();
			repository.FindAll(Restrictions.IdEq(new Guid()));
			Assert.IsTrue(MyFetchingStrategy.Apply_Called);
		}

		[Test]
		public void WillCallFetchingStrategyWhenTheyExists_DetachedCriteria()
		{
			IRepository<IParent> repository = IoC.Resolve<IRepository<IParent>>();
			repository.FindAll(repository.CreateDetachedCriteria());
			Assert.IsTrue(MyFetchingStrategy.Apply_Called);
		}

		[Test]
		public void WillCallFetchingStrategyWhenTheyExists_CriteriaBatch()
		{
			IRepository<IParent> repository = IoC.Resolve<IRepository<IParent>>();

			bool gotResults = false;
			CriteriaBatch batch = new CriteriaBatch();
			batch.Add(repository.CreateDetachedCriteria())
				.OnRead<IParent>(delegate(ICollection<IParent> parents)
			{
				gotResults = true;
			}).Execute(UnitOfWork.CurrentSession);
			Assert.IsTrue(gotResults);
			Assert.IsTrue(MyFetchingStrategy.Apply_Called);
		}
	}

	public class MyFetchingStrategy : IFetchingStrategy<IParent>
	{
		public static bool Apply_Called;

		public ICriteria Apply(ICriteria criteria)
		{
			Apply_Called = true;
			return criteria;
		}
	}
}