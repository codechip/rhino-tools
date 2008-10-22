using System;
using System.IO;
using log4net;
using log4net.Appender;
using log4net.Config;
using MbUnit.Framework;
using Rhino.Commons.ForTesting;

namespace Rhino.Commons.Test.UoW
{
	[TestFixture]
	public class UnitOfWorkAwareTests : DatabaseTestFixtureBase
	{
		[TestFixtureSetUp]
		public void OneTimeTestInitialize()
		{
			//turn on log4net logging (and supress output to console)
			BasicConfigurator.Configure(new MemoryAppender());

			string path = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"UoW\Windsor.config"));
			IntializeNHibernateAndIoC(PersistenceFramework.NHibernate,
									  path,
									  DatabaseEngine.SQLite,
									  MappingInfo.FromAssemblyContaining<SimpleObject>());
		}

		[TearDown]
		public void TestCleanup()
		{
			if(UnitOfWork.IsStarted)
				CurrentContext.DisposeUnitOfWork();
		}

		[Test]
		public void NotifiedOnUnitOfWorkStart()
		{
			IoC.Container.AddComponent("IUnitOfWorkAware", typeof(IUnitOfWorkAware), typeof(UnitOfWorkAwareImplementor));
			CurrentContext.CreateUnitOfWork();
			UnitOfWorkAwareImplementor resolve = (UnitOfWorkAwareImplementor)IoC.Resolve<IUnitOfWorkAware>();
			Assert.AreEqual(1, resolve.StartedCalled);
			Assert.AreEqual(0, resolve.DisposingCalled);
			Assert.AreEqual(0, resolve.DisposedCalled);
			CurrentContext.DisposeUnitOfWork();
			Assert.AreEqual(1, resolve.StartedCalled);
			Assert.AreEqual(1, resolve.DisposingCalled);
			Assert.AreEqual(1, resolve.DisposedCalled);
		}

		[Test]
		public void WhenNotRegisteredUnitOfWorkStarts()
		{
			CurrentContext.CreateUnitOfWork();
			Assert.IsTrue(UnitOfWork.IsStarted);
		}

		internal class UnitOfWorkAwareImplementor : IUnitOfWorkAware
		{
			public int StartedCalled { get; set; }
			public int DisposingCalled { get; set; }
			public int DisposedCalled { get; set; }


			public void UnitOfWorkStarted(IUnitOfWork unitOfWork)
			{
				StartedCalled++;
			}

			public void UnitOfWorkDisposing(IUnitOfWork unitOfWork)
			{
				DisposedCalled++;
				Assert.IsNotNull(UnitOfWork.Current);
			}

			public void UnitOfWorkDisposed(IUnitOfWork unitOfWork)
			{
				DisposingCalled++;
			}
		}
	}
}