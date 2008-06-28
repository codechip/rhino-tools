using System.Collections.Generic;

namespace Rhino.Commons
{
    public class MultipleNHibernateTransaction : RhinoTransaction
    {
        private readonly List<RhinoTransaction> transactions = new List<RhinoTransaction>();

        public void Add(RhinoTransaction transaction) 
        {
            transactions.Add(transaction);
        }

        public void Commit()
        {
            transactions.ForEach(delegate(RhinoTransaction transaction) { transaction.Commit(); });
        }

        public void Rollback()
        {
            transactions.ForEach(delegate(RhinoTransaction transaction) { transaction.Rollback(); });
        }

        public void Dispose()
        {
            transactions.ForEach(delegate(RhinoTransaction transaction) { transaction.Dispose(); });
        }
    }
}
