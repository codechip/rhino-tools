using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;

namespace Rhino.Commons.Test
{
	[TestFixture]
	public class UnitOfWorkTests
	{
		[Test]
		public void CanNestUnitOfWork()
		{
			IUnitOfWork first = UnitOfWork.Start();
			Assert.AreEqual(first, UnitOfWork.Current);
			Assert.AreEqual(((NHibernateUnitOfWorkAdapter)first).Session,
			                UnitOfWork.CurrentNHibernateSession);
			
			IUnitOfWork second = UnitOfWork.Start(UnitOfWorkNestingOptions.CreateNewOrNestUnitOfWork);
			Assert.AreEqual(second, UnitOfWork.Current);
			Assert.AreEqual(((NHibernateUnitOfWorkAdapter)second).Session,
			                UnitOfWork.CurrentNHibernateSession);
			
			second.Dispose();
			Assert.AreEqual(first, UnitOfWork.Current);
			Assert.AreEqual(((NHibernateUnitOfWorkAdapter)first).Session,
							UnitOfWork.CurrentNHibernateSession);
			first.Dispose();
			Assert.IsNull(Local.Data[UnitOfWork.CurrentNHibernateSessionKey]);
		}

		[Test]
		public void CanUseUnitOfWorkInMultiplyStarts()
		{
			using(IUnitOfWork first = UnitOfWork.Start())
			{
				Assert.AreEqual(first, UnitOfWork.Current);
			
				using(UnitOfWork.Start())
				{
					Assert.AreEqual(first, UnitOfWork.Current);
				}
				Assert.AreEqual(first, UnitOfWork.Current);
			}
			Assert.IsNull(Local.Data[UnitOfWork.CurrentNHibernateSessionKey]);
		
		}
	}
}
