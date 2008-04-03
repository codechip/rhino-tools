using System;
using System.Data;
using NHibernate;
using MbUnit.Framework;
using Rhino.Commons;

namespace Rhino.Commons.Test.UoW
{
    [TestFixture]
    public class GlobalNonThreadSafeUnitOfWorkTest
    {
		[Test]
		public void Should_be_in_a_unit_of_work()
		{
			using (UnitOfWork.RegisterGlobalUnitOfWork(new StubUnitOfWork()))
			{
				UnitOfWork.Start();
				Assert.IsTrue(UnitOfWork.IsStarted);
			}
		}
		
		[Test]
		public void Should_be_to_get_the_current_unit_of_work()
		{
			using (UnitOfWork.RegisterGlobalUnitOfWork(new StubUnitOfWork()))
			{
				IUnitOfWork stub = new StubUnitOfWork();
				UnitOfWork.RegisterGlobalUnitOfWork(stub);
				UnitOfWork.Start();
				Assert.AreEqual(stub, UnitOfWork.Current);
			}
		}
		
		private class StubUnitOfWork : IUnitOfWork
		{
			public void Flush() { }
			public bool IsInActiveTransaction { get { return false; } }

			public RhinoTransaction BeginTransaction() { return null; }
			public RhinoTransaction BeginTransaction(IsolationLevel isolationLevel) { return null; }
			public void TransactionalFlush() { }
			public void TransactionalFlush(IsolationLevel isolationLevel) { }
			
			public void Dispose() { }
		}
	}
}
