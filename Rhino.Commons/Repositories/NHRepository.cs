using System;
using System.Collections.Generic;
using NHibernate;
using NHibernate.Expression;

namespace Rhino.Commons
{
    public class NHRepository<T> : IRepository<T>
    {
        public T Get(object id)
        {
            return (T) UnitOfWork.CurrentNHibernateSession.Get(typeof (T), id);
        }

        public T Load(object id)
        {
            return (T)UnitOfWork.CurrentNHibernateSession.Load(typeof(T), id);
        }

        public void Delete(T entity)
        {
            UnitOfWork.CurrentNHibernateSession.Delete(entity);
        }

        public void Save(T entity)
        {
            UnitOfWork.CurrentNHibernateSession.Save(entity);
        }

        public ICollection<T> FindAll(Order order, params ICriterion[] criteria)
        {
            ICriteria crit = CreateCriteriaFromArray(criteria);
            crit.AddOrder(order);
            return Collection.ToArray<T>(crit.List());
           
        }

        public ICollection<T> FindAll(Order[] orders, params ICriterion[] criteria)
        {
            ICriteria crit = CreateCriteriaFromArray(criteria);
            foreach (Order order in orders)
            {
                crit.AddOrder(order);
            }
            return Collection.ToArray<T>(crit.List());
        }

        public ICollection<T> FindAll(params ICriterion[] criteria)
        {
            ICriteria crit = CreateCriteriaFromArray(criteria);
            return Collection.ToArray<T>(crit.List());
        }

        public ICollection<T> FindAll(int firstResult, int numberOfResults, params ICriterion[] criteria)
        {
            ICriteria crit = CreateCriteriaFromArray(criteria);
            crit.SetFirstResult(firstResult)
                .SetMaxResults(numberOfResults);
            return Collection.ToArray<T>(crit.List());
        }

        public ICollection<T> FindAll(
            int firstResult, int numberOfResults, Order selectionOrder, params ICriterion[] criteria)
        {
            ICriteria crit = CreateCriteriaFromArray(criteria);
            crit.SetFirstResult(firstResult)
                .SetMaxResults(numberOfResults);
            crit.AddOrder(selectionOrder);
            return Collection.ToArray<T>(crit.List());
        }

        public ICollection<T> FindAll(
            int firstResult, int numberOfResults, Order[] selectionOrder, params ICriterion[] criteria)
        {
            ICriteria crit = CreateCriteriaFromArray(criteria);
            crit.SetFirstResult(firstResult)
                .SetMaxResults(numberOfResults);
            foreach (Order order in selectionOrder)
            {
                crit.AddOrder(order);
            }
            return Collection.ToArray<T>(crit.List());
        }

        public ICollection<T> FindAll(string namedQuery, params Parameter[] parameters)
        {
            IQuery query = CreateQuery(namedQuery, parameters);
            return Collection.ToArray<T>(query.List());
        }

        public ICollection<T> FindAll(
            int firstResult, int numberOfResults, string namedQuery, params Parameter[] parameters)
        {
            IQuery query = CreateQuery(namedQuery, parameters);
            query.SetFirstResult(firstResult)
                .SetMaxResults(numberOfResults);
            return Collection.ToArray<T>(query.List());
        }

        public T FindOne(params ICriterion[] criteria)
        {
            ICriteria crit = CreateCriteriaFromArray(criteria);
            return (T) crit.UniqueResult();
        }

        public T FindOne(string namedQuery, params Parameter[] parameters)
        {
            IQuery query = CreateQuery(namedQuery, parameters);
            return (T)query.UniqueResult();
        }

        private static ICriteria CreateCriteriaFromArray(ICriterion[] criteria)
        {
            ICriteria crit = UnitOfWork.CurrentNHibernateSession.CreateCriteria(typeof(T));
            foreach (ICriterion criterion in criteria)
            {
                //allow some fancy antics like returning possible return 
                // or null to ignore the criteria
                if(criterion == null)
                    continue;
                crit.Add(criterion);
            }
            AddCaching(crit);
            return crit;
        }

        private static void AddCaching(ICriteria crit)
        {
            if(With.Caching.ShouldForceCacheRefresh == false &&
               With.Caching.Enabled)
            {
                crit.SetCacheable(true);
            	if (With.Caching.CurrentCacheRegion!=null)
					crit.SetCacheRegion(With.Caching.CurrentCacheRegion);
            }
        }

        private static IQuery CreateQuery(string namedQuery, Parameter[] parameters)
        {
            IQuery query = UnitOfWork.CurrentNHibernateSession.GetNamedQuery(namedQuery);
            foreach (Parameter parameter in parameters)
            {
				if (parameter.Type == null)
					query.SetParameter(parameter.Name, parameter.Value);
				else
					query.SetParameter(parameter.Name, parameter.Value, parameter.Type);
            }
            AddCaching(query);
            return query;
        }

        private static void AddCaching(IQuery query)
        {
            if (With.Caching.ShouldForceCacheRefresh == false && With.Caching.Enabled)
            {
                query.SetCacheable(true);
              	if (With.Caching.CurrentCacheRegion!=null)
					query.SetCacheRegion(With.Caching.CurrentCacheRegion);
            }
            else if(With.Caching.ShouldForceCacheRefresh)
            {
                query.SetForceCacheRefresh(true);
            }
        }
    }
}
