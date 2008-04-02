﻿#region license
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


using System.Data;
using NHibernate;
using NHibernate.Criterion;

namespace Rhino.Commons
{
	internal class RepositoryHelper<T>
	{
		public delegate ICriteria CriteriaCreator(ISession session);

		public static void AddCaching(IQuery query)
		{
			if (With.Caching.ShouldForceCacheRefresh == false && With.Caching.Enabled)
			{
				query.SetCacheable(true);
				if (With.Caching.CurrentCacheRegion != null)
					query.SetCacheRegion(With.Caching.CurrentCacheRegion);
			}
			else if (With.Caching.ShouldForceCacheRefresh)
			{
				query.SetCacheMode(CacheMode.Refresh);
			}
		}

		internal static IQuery CreateQuery(ISession session, string namedQuery, Parameter[] parameters)
		{
			IQuery query = session.GetNamedQuery(namedQuery);
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

		public static ICriteria GetExecutableCriteria(ISession session, DetachedCriteria criteria, 
			                                          params Order[] orders)
		{
			return GetExecutableCriteria(session, criteria,
				delegate { return session.CreateCriteria(typeof (T)); }, orders);
		}

		public static ICriteria GetExecutableCriteria(ISession session, DetachedCriteria criteria, 
			                                          CriteriaCreator creator, params Order[] orders)
		{
			ICriteria executableCriteria;
			if (criteria != null)
			{
				executableCriteria = criteria.GetExecutableCriteria(session);
			}
			else
			{
				executableCriteria = creator(session);
			}
			executableCriteria = ApplyFetchingStrategies(executableCriteria);
			AddCaching(executableCriteria);
			if (orders != null && orders.Length > 0)
			{
				foreach (Order order in orders)
				{
					executableCriteria.AddOrder(order);
				}
			}
			return executableCriteria;
		}

		private static ICriteria ApplyFetchingStrategies(ICriteria executableCriteria)
		{
			IFetchingStrategy<T>[] fetchingStrategies = IoC.ResolveAll<IFetchingStrategy<T>>();
			foreach (IFetchingStrategy<T> strategy in fetchingStrategies)
			{
				executableCriteria = strategy.Apply(executableCriteria);
			}
			return executableCriteria;
		}

		public static void AddCaching(ICriteria crit)
		{
			if (With.Caching.ShouldForceCacheRefresh == false &&
			    With.Caching.Enabled)
			{
				crit.SetCacheable(true);
				if (With.Caching.CurrentCacheRegion != null)
					crit.SetCacheRegion(With.Caching.CurrentCacheRegion);
			}
		}

		public static ICriteria CreateCriteriaFromArray(ISession session, ICriterion[] criteria,
			                                            params Order[] orders)
		{
			return CreateCriteriaFromArray(session, criteria,
				delegate { return session.CreateCriteria(typeof(T)); }, orders);
		}

		public static ICriteria CreateCriteriaFromArray(ISession session, ICriterion[] criteria, 
			                                            CriteriaCreator creator, params Order[] orders)
		{
			ICriteria crit = session.CreateCriteria(typeof (T));
			foreach (ICriterion criterion in criteria)
			{
				//allow some fancy antics like returning possible return 
				// or null to ignore the criteria
				if (criterion == null)
					continue;
				crit.Add(criterion);
			}
			crit = ApplyFetchingStrategies(crit); 
			AddCaching(crit);
            if (orders != null)
            {
                foreach (Order order in orders)
                {
                    crit.AddOrder(order);
                }
            }
			return crit;
		}

		public static void CreateDbDataParameters(IDbCommand command, Parameter[] parameters)
		{
			foreach (Parameter parameter in parameters)
			{
				IDbDataParameter sp_arg = command.CreateParameter();
				sp_arg.ParameterName = parameter.Name;
				sp_arg.Value = parameter.Value;
				command.Parameters.Add(sp_arg);
			}
		}
	}
}
