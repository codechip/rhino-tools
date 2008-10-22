using System.Data;

namespace Rhino.Commons
{
	public static partial class With
	{
		public static void AutoRollbackTransaction(IsolationLevel level, Proc transactional)
		{
			AutoRollbackTransaction(level, transactional, UnitOfWorkNestingOptions.ReturnExistingOrCreateUnitOfWork);
		}

		public static void AutoRollbackTransaction(IsolationLevel level, Proc transactional, UnitOfWorkNestingOptions nestingOptions)
		{
			using (UnitOfWork.Start(nestingOptions))
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
						tx.Rollback();
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
		}

		public static void AutoRollbackTransaction(Proc transactional)
		{
			AutoRollbackTransaction(IsolationLevel.ReadCommitted, transactional);
		}
	}
}
