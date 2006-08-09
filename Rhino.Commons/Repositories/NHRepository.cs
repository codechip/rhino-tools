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
            return (T) query.UniqueResult();
        }

        private static ICriteria CreateCriteriaFromArray(ICriterion[] criteria)
        {
            ICriteria crit = UnitOfWork.CurrentNHibernateSession.CreateCriteria(typeof(T));
            foreach (ICriterion criterion in criteria)
            {
                crit.Add(criterion);
            }
            return crit;
        }

        private static IQuery CreateQuery(string namedQuery, Parameter[] parameters)
        {
            IQuery query = UnitOfWork.CurrentNHibernateSession.GetNamedQuery(namedQuery);
            foreach (Parameter parameter in parameters)
            {
                query.SetParameter(parameter.Name, parameter.Value);
            }
            return query;
        }
    }
}
