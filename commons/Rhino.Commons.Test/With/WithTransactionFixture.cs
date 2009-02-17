using System.IO;
using MbUnit.Framework;
using NHibernate;
using Rhino.Commons.ForTesting;
using Rhino.Commons.Test.Repository;

namespace Rhino.Commons.Test
{
	[TestFixture]
	public class WithTransactionFixture : DatabaseTestFixtureBase
	{

		[TestFixtureSetUp]
		public void OneTimeTestInitialize()
		{
			string path = Path.GetFullPath(@"Repository\Windsor.config");
			InitializeNHibernateAndIoC(PersistenceFramework.NHibernate, path, MappingInfo.FromAssemblyContaining<Parent>());
		}

		[SetUp]
		public void TestInitialize()
		{
			CurrentContext.CreateUnitOfWork();
		}


		[TearDown]
		public void TestCleanup()
		{
			CurrentContext.DisposeUnitOfWork();
		}

		protected static ISession session
		{
			get
			{
				return UnitOfWork.CurrentSession;
			}
		}

		[Test]
		public void NoTransactionDoesNotCommitParent()
		{
			Parent p = new Parent();
			p.Name = "Joe";
			session.Save(p);
			session.Clear();
			Parent p2 = session.Get<Parent>(p.Id);
			Assert.IsNull(p2);
		}


		[Test]
		public void WithTransactionCommitsParent()
		{
			Parent p = new Parent();
			p.Name = "Joe"; 
			With.Transaction(delegate()
			{
				session.Save(p);
			});
			session.Clear();
			Parent p2 = session.Get<Parent>(p.Id);
			Assert.IsNotNull(p2);

		}

		[Test]
		public void WithHeldTransactionDoesNotCommitParent()
		{
			Parent p = new Parent();
			p.Name = "Joe";
			With.AutoRollbackTransaction(delegate()
			{
				session.Save(p);
				session.Flush();
				session.Clear();
				Parent p2 = session.Get<Parent>(p.Id);
				Assert.IsNotNull(p2);
			});
			session.Clear();
			Parent p3 = session.Get<Parent>(p.Id);
			Assert.IsNull(p3);
		}

	}
}
