using System;
using System.Collections;
using System.Collections.Generic;
using NHibernate;
using NHibernate.Impl;

namespace Futures.Host
{
    public static class FutureExtensions
    {
        [ThreadStatic]
        private static FutureQueryBatch batch;

        private static FutureQueryBatch Batch
        {
            get
            {
                if (batch == null)
                    batch = new FutureQueryBatch();
                return batch;
            }
        }

        public static ISession GetSession(this ICriteria criteria)
        {
            var crit = criteria as CriteriaImpl;
            if (crit != null)
                return crit.Session.GetSession();
            return GetSession(((CriteriaImpl.Subcriteria) criteria).Parent);
        }

        public static IEnumerable<T> Future<T>(this ICriteria criteria)
        {
            Batch.Add(criteria);
            return Batch.GetEnumerator<T>();
        }

        public static IEnumerable<T> ToEnumerable<T>(this IList list)
        {
            foreach (T result in list)
            {
                yield return result;
            }
        }

        public class FutureQueryBatch
        {
            private int index;
            readonly List<ICriteria> criterias = new List<ICriteria>();
            private ISession session;
            private IList results;

            public IList Results
            {
                get
                {
                    if(results==null)
                    {
                        IMultiCriteria multiCriteria = session.CreateMultiCriteria();
                        foreach (var crit in criterias)
                        {
                            multiCriteria.Add(crit);
                        }
                        results = multiCriteria.List();
                    }
                    return results;
                }
            }

            public void Add(ICriteria criteria)
            {
                if (session == null)
                    session = criteria.GetSession();
                if(session!=criteria.GetSession())
                {
                    throw new InvalidOperationException("All queries in the batch must be on the same session");
                }

                criterias.Add(criteria);
                index = criterias.Count - 1;
            }

            public IEnumerable<T> GetEnumerator<T>()
            {
                int currentIndex = index;
                return new DelayedEnumerator<T>(() => ((IList)Results[currentIndex]).ToEnumerable<T>());
            }

            private class DelayedEnumerator<T> : IEnumerable<T>
            {
                public DelayedEnumerator(Func<IEnumerable<T>> enumerable)
                {
                    this.enumerable = enumerable;
                }

                public IEnumerable<T> Enumerable
                {
                    get { return enumerable(); }
                }

                private Func<IEnumerable<T>> enumerable;

                IEnumerator IEnumerable.GetEnumerator()
                {
                    return ((IEnumerable) Enumerable).GetEnumerator();
                }

                public IEnumerator<T> GetEnumerator()
                {
                    return Enumerable.GetEnumerator();
                }
            }
        }
    }
}