using System.Collections.Generic;
using NHibernate.Expression;

namespace CustomerFinder
{
    public interface IRepository
    {
        ICollection<T> FindByCriteria<T>(DetachedCriteria criteria) where T : class;
        ICollection<T> FindByCriteria<T>(params ICriterion[] criteria) where T : class;
    }
}