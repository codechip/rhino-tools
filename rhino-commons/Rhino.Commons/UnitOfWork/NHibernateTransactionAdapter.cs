namespace Rhino.Commons
{
	public class NHibernateTransactionAdapter : RhinoTransaction
	{
		private readonly NHibernate.ITransaction transaction;

		public NHibernateTransactionAdapter(NHibernate.ITransaction transaction)
		{
			this.transaction = transaction;
		}
		public void Commit()
		{
			transaction.Commit();
		}

		public void Rollback()
		{
			transaction.Rollback();
		}

		public void Dispose()
		{
			transaction.Dispose();
		}
	}
}