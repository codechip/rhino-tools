using System;
using System.Data;
using Castle.ActiveRecord;

namespace Rhino.Commons
{
	public class ActiveRecordUnitOfWorkAdapter : IUnitOfWorkImplementor
	{
		private readonly ISessionScope scope;
		private IUnitOfWorkImplementor previous;
		private int usageCount = 1;
		private ActiveRecordTransactionAdapter transactionAdapter;

		public ActiveRecordUnitOfWorkAdapter(ISessionScope scope, IUnitOfWorkImplementor previous)
		{
			this.scope = scope;
			this.previous = previous;
		}

		public bool IsInActiveTransaction
		{
			get
			{
				return transactionAdapter != null && transactionAdapter.IsActive;
			}
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

		public RhinoTransaction BeginTransaction()
		{
			return BeginTransaction(IsolationLevel.ReadCommitted);
		}

		public RhinoTransaction BeginTransaction(IsolationLevel isolationLevel)
		{
			TransactionScope transactionScope = new TransactionScope(TransactionMode.New, isolationLevel);
			transactionAdapter = new ActiveRecordTransactionAdapter(transactionScope);
			return transactionAdapter;
		}

		///<summary>
		///Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
		///</summary>
		///<filterpriority>2</filterpriority>
		public void Dispose()
		{
			usageCount -= 1;
			if(usageCount==0)
			{
				scope.Dispose();
			}
		}
	}
}