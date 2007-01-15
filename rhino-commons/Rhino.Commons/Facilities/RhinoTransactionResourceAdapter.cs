namespace Rhino.Commons.Facilities
{
	using Castle.Services.Transaction;
	using TransactionMode = Castle.Services.Transaction.TransactionMode;

	public class RhinoTransactionResourceAdapter : IResource
	{
		private RhinoTransaction rhinoTransaction;

		public RhinoTransactionResourceAdapter(TransactionMode transactionMode)
		{
		}

		public void Start()
		{
			rhinoTransaction = UnitOfWork.Current.BeginTransaction();
		}

		public void Commit()
		{
			rhinoTransaction.Commit();
			Dispose();
		}

		public void Rollback()
		{
			rhinoTransaction.Rollback();
			Dispose();
		}

		protected void Dispose()
		{
			rhinoTransaction.Dispose();
		}
	}
}
