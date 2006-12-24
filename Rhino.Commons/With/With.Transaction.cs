using System.Data;

namespace Rhino.Commons
{
	public static partial class With
	{
		public static void Transaction(IsolationLevel level, Proc transactional)
		{
			// If we are already in a transaction, don't start a new one
			if (UnitOfWork.Current.IsInActiveTransaction)
			{
				transactional();
			}
			else
			{
				RhinoTransaction tx = UnitOfWork.Current.BeginTransaction(level);
				try
				{
					transactional();
					tx.Commit();
				}
				catch
				{
					tx.Rollback();
					throw;
				}
				finally
				{
					tx.Dispose();
				}
			}
		}

		public static void Transaction(Proc transactional)
		{
			Transaction(IsolationLevel.ReadCommitted, transactional);
		}
	}
}