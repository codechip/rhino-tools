using NHibernate;

namespace Rhino.Commons
{
    public static class With
    {
        public static void Transaction(Proc transactional)
        {
            ITransaction tx = UnitOfWork.Current.BeginTransaction();
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
}
