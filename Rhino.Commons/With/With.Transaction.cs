using System;
using System.ComponentModel;
using System.Data;
using NHibernate;

namespace Rhino.Commons
{
	public static partial class With
    {
    public static void Transaction(IsolationLevel level, Proc transactional)
        {
            ITransaction tx = UnitOfWork.Current.BeginTransaction(level);
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
        
        public static void Transaction(Proc transactional)
        {
            Transaction(IsolationLevel.ReadCommitted,transactional);
        }
    }
}
