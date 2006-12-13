using System;
using System.Data;
using Castle.ActiveRecord;

namespace Rhino.Commons
{
	internal class ActiveRecordUnitOfWorkAdapter : IUnitOfWorkImplementor
	{
		private readonly ISessionScope scope;
		private IUnitOfWorkImplementor previous;
		private int usageCount = 1;

		public ActiveRecordUnitOfWorkAdapter(ISessionScope scope, IUnitOfWorkImplementor previous)
		{
			this.scope = scope;
			this.previous = previous;
		}

		public void IncremementUsages()
		{
			usageCount++;
		}

		public IUnitOfWorkImplementor Previous
		{
			get { return previous; }
		}

		public void Flush()
		{
			scope.Flush();
		}

		public ITransaction BeginTransaction()
		{
			TransactionScope transactionScope = new TransactionScope(TransactionMode.New);
			return new ActiveRecordTransactionAdapter(transactionScope);
		}

		public ITransaction BeginTransaction(IsolationLevel isolationLevel)
		{
			throw new NotImplementedException();
		}

		///<summary>
		///Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
		///</summary>
		///<filterpriority>2</filterpriority>
		public void Dispose()
		{
			throw new NotImplementedException();
		}
	}
}