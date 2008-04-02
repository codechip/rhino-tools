using System;
using System.Collections.Generic;
using NHibernate;
using NHibernate.Criterion;
using System.Collections;

namespace Rhino.Commons
{
    public class CriteriaBatch
    {
        private readonly ISession session;
        private DetachedCriteria currentCriteria;
        private int currentIndex = -1;
        private readonly List<Proc<IList>> collectionDelegates = new List<Proc<IList>>();
        private readonly List<Proc<object>> uniqueResultDelegate = new List<Proc<object>>();
        private readonly List<Proc<IList, int>> collectionAndCountDelegate = new List<Proc<IList, int>>();
        readonly List<DetachedCriteria> criteriaList = new List<DetachedCriteria>();

		public static event ProcessCriteriaDelegate ProcessCriteria;

		public CriteriaBatch()
		{
		}

        public CriteriaBatch(ISession session)
        {
            this.session = session;
        }
		
        public virtual CriteriaBatch Add(DetachedCriteria criteria, Order order)
        {
            return Add(criteria.AddOrder(order));
        }

        public virtual CriteriaBatch Add(DetachedCriteria criteria)
        {
            currentIndex += 1;
            criteriaList.Add(criteria);
            currentCriteria = criteria;
            collectionDelegates.Add(null);
            uniqueResultDelegate.Add(null);
            collectionAndCountDelegate.Add(null);
            return this;
        }

        public CriteriaBatch OnRead<T>(Proc<T> read)
        {
            uniqueResultDelegate[currentIndex] = delegate(object obj)
            {
                read((T)obj);
            };
            return this;
        }

        public CriteriaBatch OnRead<T>(Proc<ICollection<T>, int> read)
        {
            collectionAndCountDelegate[currentIndex] = delegate(IList list, int count)
            {
                read(Collection.ToArray<T>(list), count);
            };
            Add(CriteriaTransformer.TransformToRowCount(currentCriteria));
            return this;
        }

        public CriteriaBatch OnRead<T>(Proc<ICollection<T>> read)
        {
            collectionDelegates[currentIndex] = delegate(IList list)
            {
                read(Collection.ToArray<T>(list));
            };
            return this;
        }

        public virtual IList Execute()
        {
            return Execute(session ?? UnitOfWork.CurrentSession);
        }

        public virtual IList Execute(ISession theSession)
        {
            if (criteriaList.Count == 0) return new ArrayList();

            IMultiCriteria multiCriteria = theSession.CreateMultiCriteria();
            foreach (DetachedCriteria detachedCriteria in criteriaList)
            {
				multiCriteria.Add(CreateCriteria(theSession, detachedCriteria));
            }

            IList list = multiCriteria.List();
            int results = list.Count;

            for (int i = 0; i < results; i++)
            {
                if (collectionDelegates[i] != null)
                {
                    collectionDelegates[i]((IList)list[i]);
                }
                if (uniqueResultDelegate[i] != null)
                {
                    object single = Collection.Single((ICollection)list[i]);
                    uniqueResultDelegate[i](single);
                }
                if (collectionAndCountDelegate[i] != null)
                {
                    IList queryList = (IList)list[i];
                    int count = Convert.ToInt32(
                        Collection.Single((ICollection)list[i + 1])
                        );
                    collectionAndCountDelegate[i](queryList, count);
                    i += 1;//not a best practice, I will admit
                }
            }

            return list;
        }

        public CriteriaBatch Paging(int firstResult, int maxResults)
        {
            currentCriteria.SetFirstResult(firstResult);
            currentCriteria.SetMaxResults(maxResults);
            return this;
        }

		private ICriteria CreateCriteria(ISession theSession, DetachedCriteria detachedCriteria)
		{
			ICriteria criteria = detachedCriteria.GetExecutableCriteria(theSession);
			if (ProcessCriteria != null)
			{
				foreach (ProcessCriteriaDelegate process in ProcessCriteria.GetInvocationList())
				{
					criteria = process(criteria);
				}
			}
			return criteria;
		}
    }
}
