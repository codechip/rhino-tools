using System.Collections.Generic;

namespace Rhino.Commons
{
    public class MultipleNHibernateTransaction : List<RhinoTransaction>, RhinoTransaction
    {
        public void Commit()
        {
            ForEach(delegate(RhinoTransaction transaction) { transaction.Commit(); });
        }

        public void Rollback()
        {
            ForEach(delegate(RhinoTransaction transaction) { transaction.Rollback(); });
        }

        public void Dispose()
        {
            ForEach(delegate(RhinoTransaction transaction) { transaction.Dispose(); transaction = null; });
            Clear();
        }
    }
}
