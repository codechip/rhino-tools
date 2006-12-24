using Castle.ActiveRecord;

namespace Rhino.Commons
{
	internal class ActiveRecordTransactionAdapter : RhinoTransaction
	{
		internal bool IsActive = true;
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
			IsActive = false;
		}
	}
}