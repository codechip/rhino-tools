
namespace Rhino.Commons.Test.NHibernate
{
	using System;
	using System.IO;
	using global::NHibernate.Criterion;
	using MbUnit.Framework;
	using Repository;
	using Rhino.Commons.ForTesting;

	[TestFixture]
	public class FutureQueryOfFixture : TestFixtureBase
	{
		[SetUp]
		public void TestInitialize()
		{
			Environment.CurrentDirectory = AppDomain.CurrentDomain.BaseDirectory;
			FixtureInitialize(PersistenceFramework.NHibernate,
				Path.GetFullPath(@"Repository\Windsor.config"),
				MappingInfo.FromAssemblyContaining<Parent>());
			CurrentContext.CreateUnitOfWork();
		}

		[Test]
		public void CanExecuteQueryBatch()
		{
			FutureQueryOf<Parent> futureQueryOfParents = new FutureQueryOf<Parent>(DetachedCriteria.For<Parent>());
			FutureQueryOf<Child> futureQueryOfChildren = new FutureQueryOf<Child>(DetachedCriteria.For<Child>());
			Assert.AreEqual(0, futureQueryOfParents.Results.Count);

			//This also kills the database, because we use an in
			// memory one ,so we ensure that the code is not 
			// executing a second query
			CurrentContext.DisposeUnitOfWork();

			Assert.AreEqual(0,  futureQueryOfChildren.Results.Count);
		}
	}
}
