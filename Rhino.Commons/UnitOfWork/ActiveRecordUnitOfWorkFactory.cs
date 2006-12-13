using System;
using System.Data;
using System.Reflection;
using Castle.ActiveRecord;
using Castle.ActiveRecord.Framework.Config;
using NHibernate;

namespace Rhino.Commons
{
	public class ActiveRecordUnitOfWorkFactory : IUnitOfWorkFactory
	{
		private readonly Assembly[] assemblies;
		static object lockObj = new object();
		private static bool initialized = false;

		public ActiveRecordUnitOfWorkFactory(Assembly[] assemblies)
		{
			this.assemblies = assemblies;
		}

		public IUnitOfWorkImplementor Create(IUnitOfWorkImplementor previous)
		{
			InitializeIfNeccecary();
			SessionScope scope = new SessionScope(FlushAction.Never);
			return new ActiveRecordUnitOfWorkAdapter(scope, previous);
		}

		private void InitializeIfNeccecary()
		{
			if(!initialized)
			{
				lock(lockObj)
				{
					if(!initialized)
					{
						ActiveRecordStarter.Initialize(assemblies,ActiveRecordSectionHandler.Instance);
						initialized = true;
					}
				}
			}
		}
	}

	internal class ActiveRecordTransactionAdapter : ITransaction
	{
		private readonly TransactionScope transactionScope;

		public ActiveRecordTransactionAdapter(TransactionScope transactionScope)
		{
			this.transactionScope = transactionScope;
		}

		public void Commit()
		{
			transactionScope.VoteCommit();
		}

		public void Rollback()
		{
			transactionScope.VoteRollBack();
		}

		public void Dispose()
		{
			transactionScope.Dispose();
		}
	}
}