#region license
// Copyright (c) 2005 - 2007 Ayende Rahien (ayende@ayende.com)
// All rights reserved.
// 
// Redistribution and use in source and binary forms, with or without modification,
// are permitted provided that the following conditions are met:
// 
//     * Redistributions of source code must retain the above copyright notice,
//     this list of conditions and the following disclaimer.
//     * Redistributions in binary form must reproduce the above copyright notice,
//     this list of conditions and the following disclaimer in the documentation
//     and/or other materials provided with the distribution.
//     * Neither the name of Ayende Rahien nor the names of its
//     contributors may be used to endorse or promote products derived from this
//     software without specific prior written permission.
// 
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND
// ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
// WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
// DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS BE LIABLE
// FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL
// DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR
// SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER
// CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY,
// OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF
// THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
#endregion

using System;
using System.Collections;
using System.Collections.Generic;
using NHibernate;
using NHibernate.Expression;
using NHibernate.Transform;

namespace Rhino.Commons
{
    public abstract class RepositoryImplBase<T> {
        public ProjT ReportOne<ProjT>(DetachedCriteria criteria, ProjectionList projectionList)
        {
            using (DisposableAction<ISession> action = ActionToBePerformedOnSessionUsedForDbFetches)
            {
                ICriteria crit = RepositoryHelper<T>.GetExecutableCriteria(action.Value, criteria, null);
                return DoReportOne<ProjT>(crit, projectionList);
            }
        }


        public ProjT ReportOne<ProjT>(ProjectionList projectionList, params ICriterion[] criteria)
        {
            using (DisposableAction<ISession> action = ActionToBePerformedOnSessionUsedForDbFetches)
            {
                ICriteria crit = RepositoryHelper<T>.CreateCriteriaFromArray(action.Value, criteria);
                return DoReportOne<ProjT>(crit, projectionList);
            }
        }


        private static ProjT DoReportOne<ProjT>(ICriteria criteria, ProjectionList projectionList) {
            BuildProjectionCriteria<ProjT>(criteria, projectionList, false);
            return criteria.UniqueResult<ProjT>();
        }


        public ICollection<ProjT> ReportAll<ProjT>(ProjectionList projectionList)
        {
            using (DisposableAction<ISession> action = ActionToBePerformedOnSessionUsedForDbFetches)
            {
                ICriteria crit = RepositoryHelper<T>.GetExecutableCriteria(action.Value, null, null);
                return DoReportAll<ProjT>(crit, projectionList);
            }

        }


        public ICollection<ProjT> ReportAll<ProjT>(ProjectionList projectionList, params Order[] orders)
        {
            using (DisposableAction<ISession> action = ActionToBePerformedOnSessionUsedForDbFetches)
            {
                ICriteria crit = RepositoryHelper<T>.GetExecutableCriteria(action.Value, null, orders);
                return DoReportAll<ProjT>(crit, projectionList);
            }
        }


        public ICollection<ProjT> ReportAll<ProjT>(ProjectionList projectionList, bool distinctResults)
        {
            using (DisposableAction<ISession> action = ActionToBePerformedOnSessionUsedForDbFetches)
            {
                ICriteria crit = RepositoryHelper<T>.GetExecutableCriteria(action.Value, null, null);
                return DoReportAll<ProjT>(crit, projectionList, distinctResults);
            }
        }


        public ICollection<ProjT> ReportAll_original<ProjT>(DetachedCriteria criteria, ProjectionList projectionList)
        {
            using (DisposableAction<ISession> action = ActionToBePerformedOnSessionUsedForDbFetches)
            {
                ICriteria crit = RepositoryHelper<T>.GetExecutableCriteria(action.Value, criteria, null);
                return DoReportAll<ProjT>(crit, projectionList);
            }
        }


        public ICollection<ProjT> ReportAll<ProjT>(DetachedCriteria criteria, ProjectionList projectionList)
        {
            using (DisposableAction<ISession> action = ActionToBePerformedOnSessionUsedForDbFetches)
            {
                ICriteria crit = RepositoryHelper<T>.GetExecutableCriteria(action.Value, criteria, null);
                return DoReportAll<ProjT>(crit, projectionList);
            }
        }


        protected abstract DisposableAction<ISession> ActionToBePerformedOnSessionUsedForDbFetches
        {
            get;
        }


        public ICollection<ProjT> ReportAll<ProjT>(DetachedCriteria criteria, ProjectionList projectionList, params Order[] orders)
        {
            using (DisposableAction<ISession> action = ActionToBePerformedOnSessionUsedForDbFetches)
            {
                ICriteria crit = RepositoryHelper<T>.GetExecutableCriteria(action.Value, criteria, orders);
                return DoReportAll<ProjT>(crit, projectionList);
            }
        }


        public ICollection<ProjT> ReportAll<ProjT>(ProjectionList projectionList, params ICriterion[] criteria)
        {
            using (DisposableAction<ISession> action = ActionToBePerformedOnSessionUsedForDbFetches)
            {
                ICriteria crit = RepositoryHelper<T>.CreateCriteriaFromArray(action.Value, criteria);
                return DoReportAll<ProjT>(crit, projectionList);
            }
        }


        public ICollection<ProjT> ReportAll<ProjT>(ProjectionList projectionList, Order[] orders, params ICriterion[] criteria)
        {
            using (DisposableAction<ISession> action = ActionToBePerformedOnSessionUsedForDbFetches)
            {
                ICriteria crit = RepositoryHelper<T>.CreateCriteriaFromArray(action.Value, criteria);
                foreach (Order order in orders)
                {
                    crit.AddOrder(order);
                }
                return DoReportAll<ProjT>(crit, projectionList);
            }
        }


        public ICollection<ProjJ> ReportAll<ProjJ>(string namedQuery, params Parameter[] parameters)
        {
            using (DisposableAction<ISession> action = ActionToBePerformedOnSessionUsedForDbFetches)
            {
                IQuery query = RepositoryHelper<T>.CreateQuery(action.Value, namedQuery, parameters);
                return query.List<ProjJ>();
            }
        }


        private static ICollection<ProjT> DoReportAll<ProjT>(ICriteria criteria, ProjectionList projectionList) {
            return DoReportAll<ProjT>(criteria, projectionList, false);
        }


        private static ICollection<ProjT> DoReportAll<ProjT>(ICriteria criteria, ProjectionList projectionList, bool distinctResults) {
            BuildProjectionCriteria<ProjT>(criteria, projectionList, distinctResults);
            return criteria.List<ProjT>();
        }


        private static void BuildProjectionCriteria<ProjT>(ICriteria criteria, IProjection projectionList, bool distinctResults)
        {
            if (distinctResults)
                criteria.SetProjection(Projections.Distinct(projectionList));
            else
                criteria.SetProjection(projectionList);

            if (typeof(ProjT) != typeof(object[]))
            //we are not returning a tuple, so we need the result transformer
            {
                criteria.SetResultTransformer(new TypedResultTransformer<ProjT>());
            }
        }


        /// <summary>
        /// This is used to convert the resulting tuples into strongly typed objects.
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        private class TypedResultTransformer<T1> : IResultTransformer
        {            
            public object TransformTuple(object[] tuple, string[] aliases)
            {
                return Activator.CreateInstance(typeof(T1), tuple);
            }

            IList IResultTransformer.TransformList(IList collection)
            {
                return collection;
            }
        }

    }
}