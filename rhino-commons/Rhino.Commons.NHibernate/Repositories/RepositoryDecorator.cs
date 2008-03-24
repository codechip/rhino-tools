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
using System.Collections.Generic;
using System.Data;
using NHibernate.Criterion;

namespace Rhino.Commons
{
	public class RepositoryDecorator<T> : IRepository<T>
	{
		private IRepository<T> inner;

		public RepositoryDecorator()
		{
		}

		public RepositoryDecorator(IRepository<T> inner)
		{
			Inner = inner;
		}

		public IRepository<T> Inner
		{
			get { return inner; }
			set
			{
				if (value == null)
				{
					throw new ArgumentNullException("value");
				}

				inner = value;
			}
		}

		public virtual T Get(object id)
		{
			return inner.Get(id);
		}

		public virtual T Load(object id)
		{
			return inner.Load(id);
		}

		public virtual void Delete(T entity)
		{
			inner.Delete(entity);
		}

		public virtual void DeleteAll()
		{
			inner.DeleteAll();
		}

		public virtual void DeleteAll(DetachedCriteria criteria)
		{
			inner.DeleteAll(criteria);
		}

		public virtual T Save(T entity)
		{
			return inner.Save(entity);
		}

		public virtual T SaveOrUpdate(T entity)
		{
			return inner.SaveOrUpdate(entity);
		}

		public virtual T SaveOrUpdateCopy(T entity)
		{
			return inner.SaveOrUpdateCopy(entity);
		}

		public virtual void Update(T entity)
		{
			inner.Update(entity);
		}

		public virtual ICollection<T> FindAll(Order order, params ICriterion[] criteria)
		{
			return inner.FindAll(order, criteria);
		}

		public virtual ICollection<T> FindAll(DetachedCriteria criteria, params Order[] orders)
		{
			return inner.FindAll(criteria, orders);
		}

		public virtual ICollection<T> FindAll(DetachedCriteria criteria,
		                                      int firstResult, int maxResults,
		                                      params Order[] orders)
		{
			return inner.FindAll(criteria, firstResult, maxResults, orders);
		}

		public virtual ICollection<T> FindAll(Order[] orders, params ICriterion[] criteria)
		{
			return inner.FindAll(orders, criteria);
		}

		public virtual ICollection<T> FindAll(params ICriterion[] criteria)
		{
			return inner.FindAll(criteria);
		}

		public virtual ICollection<T> FindAll(int firstResult, int numberOfResults, params ICriterion[] criteria)
		{
			return inner.FindAll(firstResult, numberOfResults, criteria);
		}

		public virtual ICollection<T> FindAll(int firstResult, int numberOfResults,
		                                      Order selectionOrder,
		                                      params ICriterion[] criteria)
		{
			return inner.FindAll(firstResult, numberOfResults, selectionOrder, criteria);
		}

		public virtual ICollection<T> FindAll(int firstResult, int numberOfResults,
		                                      Order[] selectionOrder,
		                                      params ICriterion[] criteria)
		{
			return inner.FindAll(firstResult, numberOfResults, selectionOrder, criteria);
		}

		public virtual ICollection<T> FindAll(string namedQuery, params Parameter[] parameters)
		{
			return inner.FindAll(namedQuery, parameters);
		}

		public virtual ICollection<T> FindAll(int firstResult, int numberOfResults,
		                                      string namedQuery, params Parameter[] parameters)
		{
			return inner.FindAll(firstResult, numberOfResults, namedQuery, parameters);
		}

		public virtual T FindOne(params ICriterion[] criteria)
		{
			return inner.FindOne(criteria);
		}

		public virtual T FindOne(DetachedCriteria criteria)
		{
			return inner.FindOne(criteria);
		}

		public virtual T FindOne(string namedQuery, params Parameter[] parameters)
		{
			return inner.FindOne(namedQuery, parameters);
		}

		public virtual T FindFirst(DetachedCriteria criteria, params Order[] orders)
		{
			return inner.FindFirst(criteria, orders);
		}

		public virtual T FindFirst(params Order[] orders)
		{
			return inner.FindFirst(orders);
		}

		public virtual object ExecuteStoredProcedure(string sp_name, params Parameter[] parameters)
		{
			return inner.ExecuteStoredProcedure(sp_name, parameters);
		}

		public virtual ICollection<T2> ExecuteStoredProcedure<T2>(Converter<IDataReader, T2> converter,
		                                                          string sp_name, params Parameter[] parameters)
		{
			return inner.ExecuteStoredProcedure(converter, sp_name, parameters);
		}

		public virtual bool Exists(DetachedCriteria criteria)
		{
			return inner.Exists(criteria);
		}

		public virtual bool Exists()
		{
			return inner.Exists();
		}

		public virtual long Count(DetachedCriteria criteria)
		{
			return inner.Count(criteria);
		}

		public virtual long Count()
		{
			return inner.Count();
		}

		public virtual ProjT ReportOne<ProjT>(DetachedCriteria criteria,
		                                      ProjectionList projectionList)
		{
			return inner.ReportOne<ProjT>(criteria, projectionList);
		}

		public virtual ProjT ReportOne<ProjT>(ProjectionList projectionList,
		                                      params ICriterion[] criteria)
		{
			return inner.ReportOne<ProjT>(projectionList, criteria);
		}

		public virtual ICollection<ProjT> ReportAll<ProjT>(ProjectionList projectionList)
		{
			return ReportAll<ProjT>(projectionList);
		}

		public virtual ICollection<ProjT> ReportAll<ProjT>(DetachedCriteria criteria,
		                                                   ProjectionList projectionList)
		{
			return inner.ReportAll<ProjT>(criteria, projectionList);
		}

		public virtual ICollection<ProjT> ReportAll<ProjT>(DetachedCriteria criteria,
		                                                   ProjectionList projectionList,
		                                                   params Order[] orders)
		{
			return inner.ReportAll<ProjT>(criteria, projectionList, orders);
		}

		public virtual ICollection<ProjT> ReportAll<ProjT>(ProjectionList projectionList,
		                                                   params ICriterion[] criterion)
		{
			return inner.ReportAll<ProjT>(projectionList, criterion);
		}

		public virtual ICollection<ProjT> ReportAll<ProjT>(ProjectionList projectionList,
		                                                   Order[] orders,
		                                                   params ICriterion[] criteria)
		{
			return inner.ReportAll<ProjT>(projectionList, orders, criteria);
		}

		public virtual ICollection<ProjT> ReportAll<ProjT>(ProjectionList projectionList,
		                                                   params Order[] orders)
		{
			return inner.ReportAll<ProjT>(projectionList, orders);
		}

		public virtual ICollection<ProjT> ReportAll<ProjT>(ProjectionList projectionList,
		                                                   bool distinctResults)
		{
			return inner.ReportAll<ProjT>(projectionList, distinctResults);
		}

		public virtual ICollection<ProjJ> ReportAll<ProjJ>(string namedQuery,
		                                                   params Parameter[] parameters)
		{
			return inner.ReportAll<ProjJ>(namedQuery, parameters);
		}

		public virtual DetachedCriteria CreateDetachedCriteria()
		{
			return inner.CreateDetachedCriteria();
		}

		public virtual DetachedCriteria CreateDetachedCriteria(string alias)
		{
			return inner.CreateDetachedCriteria(alias);
		}

		public virtual T Create()
		{
			return inner.Create();
		}

		public virtual FutureValue<T> FutureGet(object id)
		{
			return inner.FutureGet(id);
		}

		public virtual FutureValue<T> FutureLoad(object id)
		{
			return inner.FutureLoad(id);
		}
	}
}
