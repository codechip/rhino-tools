using System.Collections.Generic;
using Castle.ActiveRecord;
using NHibernate.Expression;

namespace CustomerFinder
{
    public class RepositoryImpl : IRepository
    {
        public ICollection<T> FindByCriteria<T>(DetachedCriteria criteria)
            where T : class
        {
            return ActiveRecordMediator<T>.FindAll(criteria);
        }

        public ICollection<T> FindByCriteria<T>(params ICriterion[] criteria)
               where T : class
        {
            return ActiveRecordMediator<T>.FindAll(criteria);
        }
    }
}