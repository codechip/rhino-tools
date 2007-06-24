using System;
using System.Collections.Generic;
using NHibernate;
using NHibernate.Expression;
using System.Collections;

namespace Rhino.Commons
{
    public class CriteriaBatch
    {
        private DetachedCriteria currentCriteria;
        private int currentIndex=-1;
        private readonly List<Proc<IList>> collectionDelegates = new List<Proc<IList>>();
        private readonly List<Proc<object>> uniqueResultDelegate = new List<Proc<object>>();
        private readonly List<Proc<IList, int>> collectionAndCountDelegate = new List<Proc<IList, int>>();

        private readonly IMultiCriteria multiCriteria;


        public CriteriaBatch(ISession session)
        {
            this.multiCriteria = session.CreateMultiCriteria();
        }

        public CriteriaBatch Add(DetachedCriteria criteria, Order order)
        {
            return Add(criteria.AddOrder(order));
        }

        public CriteriaBatch Add(DetachedCriteria criteria)
        {
            currentIndex += 1;
            multiCriteria.Add(criteria);
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
                read((T) obj);
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

        public void Execute()
        {
            IList list = multiCriteria.List();
            int results = list.Count;
            for (int i = 0; i < results; i++)
            {
                if(collectionDelegates[i]!=null)
                {
                    collectionDelegates[i]((IList)list[i]);
                }
                if (uniqueResultDelegate[i] != null)
                {
                    object single = Collection.Single((ICollection) list[i]);
                    uniqueResultDelegate[i](single);
                }
                if(collectionAndCountDelegate[i]!=null)
                {
                    IList queryList = (IList)list[i];
                    int count = Convert.ToInt32(
                        Collection.Single((ICollection) list[i + 1])
                        );
                    collectionAndCountDelegate[i](queryList, count);
                    i += 1;//not a best practice, I will admit
                }
            }
        }

        public CriteriaBatch Paging(int firstResult, int maxResults)
        {
            currentCriteria.SetFirstResult(firstResult);
            currentCriteria.SetMaxResults(maxResults);
            return this;
        }
    }
}
