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
using NHibernate;
using NHibernate.Connection;
using NHibernate.Expression;
using Rhino.Commons;

namespace Rhino.Commons
{
	public class NHRepository<T> : RepositoryImplBase<T>, IRepository<T>
	{
		protected virtual ISession Session
		{
            get { return UnitOfWork.CurrentSession; }
		}

		public T Get(object id)
		{
			return (T)Session.Get(typeof(T), id);
		}

		public T Load(object id)
		{
			return (T)Session.Load(typeof(T), id);
		}

		public void Delete(T entity)
		{
			Session.Delete(entity);
		}

		public void DeleteAll()
		{
			Session.Delete(String.Format("from {0}", typeof(T).Name));
		}

		public void DeleteAll(DetachedCriteria where)
		{
            foreach (object entity in where.GetExecutableCriteria(Session).List())
            {
                Session.Delete(entity);
            }
		}

		public void Save(T entity)
		{
			Session.Save(entity);
		}


	    /// <summary>
	    /// Saves or update the entity, based on its usaved-value
	    /// </summary>
	    /// <param name="entity"></param>
	    public void SaveOrUpdate(T entity)
	    {
	        Session.SaveOrUpdate(entity);
	    }

        public T SaveOrUpdateCopy(T entity)
        {
            return (T) Session.SaveOrUpdateCopy(entity);
        }

	    public void Update(T entity)
	    {
	        Session.Update(entity);
	    }

	    public ICollection<T> FindAll(Order order, params ICriterion[] criteria)
		{
			ICriteria crit = RepositoryHelper<T>.CreateCriteriaFromArray(Session, criteria);
			crit.AddOrder(order);
			return crit.List<T>();
		}

		public ICollection<T> FindAll(Order[] orders, params ICriterion[] criteria)
		{
			ICriteria crit = RepositoryHelper<T>.CreateCriteriaFromArray(Session, criteria);
			foreach (Order order in orders)
			{
				crit.AddOrder(order);
			}
			return crit.List<T>();
		}

		public ICollection<T> FindAll(params ICriterion[] criteria)
		{
			ICriteria crit = RepositoryHelper<T>.CreateCriteriaFromArray(Session, criteria);
			return crit.List<T>();
		}

		public ICollection<T> FindAll(int firstResult, int numberOfResults, params ICriterion[] criteria)
		{
			ICriteria crit = RepositoryHelper<T>.CreateCriteriaFromArray(Session, criteria);
			crit.SetFirstResult(firstResult)
				.SetMaxResults(numberOfResults);
			return crit.List<T>();
		}

		public ICollection<T> FindAll(
			int firstResult, int numberOfResults, Order selectionOrder, params ICriterion[] criteria)
		{
			ICriteria crit = RepositoryHelper<T>.CreateCriteriaFromArray(Session, criteria);
			crit.SetFirstResult(firstResult)
				.SetMaxResults(numberOfResults);
			crit.AddOrder(selectionOrder);
			return crit.List<T>();
		}

		public ICollection<T> FindAll(
			int firstResult, int numberOfResults, Order[] selectionOrder, params ICriterion[] criteria)
		{
			ICriteria crit = RepositoryHelper<T>.CreateCriteriaFromArray(Session, criteria);
			crit.SetFirstResult(firstResult)
				.SetMaxResults(numberOfResults);
			foreach (Order order in selectionOrder)
			{
				crit.AddOrder(order);
			}
			return crit.List<T>();
		}

		public ICollection<T> FindAll(string namedQuery, params Parameter[] parameters)
		{
			IQuery query = RepositoryHelper<T>.CreateQuery(Session, namedQuery, parameters);
			return query.List<T>();
		}

		public ICollection<T> FindAll(
			int firstResult, int numberOfResults, string namedQuery, params Parameter[] parameters)
		{
			IQuery query = RepositoryHelper<T>.CreateQuery(Session, namedQuery, parameters);
			query.SetFirstResult(firstResult)
				.SetMaxResults(numberOfResults);
			return query.List<T>();
		}

		public T FindOne(params ICriterion[] criteria)
		{
			ICriteria crit = RepositoryHelper<T>.CreateCriteriaFromArray(Session, criteria);
			return (T)crit.UniqueResult();
		}

		public T FindOne(string namedQuery, params Parameter[] parameters)
		{
			IQuery query = RepositoryHelper<T>.CreateQuery(Session, namedQuery, parameters);
			return (T)query.UniqueResult();
		}

		public ICollection<T> FindAll(DetachedCriteria criteria, params Order[] orders)
		{
			ICriteria executableCriteria = RepositoryHelper<T>.GetExecutableCriteria(Session, criteria, orders);
			return executableCriteria.List<T>();
		}

		public ICollection<T> FindAll(DetachedCriteria criteria, int firstResult, int maxResults, params Order[] orders)
		{
			ICriteria executableCriteria = RepositoryHelper<T>.GetExecutableCriteria(Session, criteria, orders);
			executableCriteria.SetFirstResult(firstResult);
			executableCriteria.SetMaxResults(maxResults);
			return executableCriteria.List<T>();
		}

		public T FindOne(DetachedCriteria criteria)
		{
			ICriteria executableCriteria = RepositoryHelper<T>.GetExecutableCriteria(Session, criteria, null);
			return (T)executableCriteria.UniqueResult();
		}

		public T FindFirst(DetachedCriteria criteria, params Order[] orders)
		{
			ICriteria executableCriteria = RepositoryHelper<T>.GetExecutableCriteria(Session, criteria, orders);
			executableCriteria.SetFirstResult(0);
			executableCriteria.SetMaxResults(1);
			return (T)executableCriteria.UniqueResult();
		}

		/// <summary>
		/// Find the first entity of type
		/// </summary>
		/// <param name="orders">Optional orderring</param>
		/// <returns>The entity or null</returns>
		public T FindFirst(params Order[] orders)
		{
			return FindFirst(null, orders);
		}

		public object ExecuteStoredProcedure(string sp_name, params Parameter[] parameters)
		{
            IConnectionProvider connectionProvider = UnitOfWork.CurrentSession
                .GetSessionImplementation()
                .Factory
                .ConnectionProvider;
			IDbConnection connection = connectionProvider.GetConnection();
			try
			{
				using (IDbCommand command = connection.CreateCommand())
				{
					command.CommandText = sp_name;
					command.CommandType = CommandType.StoredProcedure;

					RepositoryHelper<T>.CreateDbDataParameters(command, parameters);

					return command.ExecuteScalar();
				}
			}
			finally
			{
				connectionProvider.CloseConnection(connection);
			}
		}

		/// <summary>
		/// Execute the specified stored procedure with the given parameters and then converts
		/// the results using the supplied delegate.
		/// </summary>
		/// <typeparam name="T2">The collection type to return.</typeparam>
		/// <param name="converter">The delegate which converts the raw results.</param>
		/// <param name="sp_name">The name of the stored procedure.</param>
		/// <param name="parameters">Parameters for the stored procedure.</param>
		/// <returns></returns>
		public ICollection<T2> ExecuteStoredProcedure<T2>(Converter<IDataReader, T2> converter, string sp_name,
														  params Parameter[] parameters)
		{
            IConnectionProvider connectionProvider = UnitOfWork.CurrentSession
                    .GetSessionImplementation()
                    .Factory
                    .ConnectionProvider;
			IDbConnection connection = connectionProvider.GetConnection();

			try
			{
				using (IDbCommand command = connection.CreateCommand())
				{
					command.CommandText = sp_name;
					command.CommandType = CommandType.StoredProcedure;

					RepositoryHelper<T>.CreateDbDataParameters(command, parameters);
					IDataReader reader = command.ExecuteReader();
					ICollection<T2> results = new List<T2>();

					while (reader.Read())
						results.Add(converter(reader));

					reader.Close();

					return results;
				}
			}
			finally
			{
				connectionProvider.CloseConnection(connection);
			}
		}


		/// <summary>
		/// Check if any instance matches the criteria.
		/// </summary>
		/// <returns><c>true</c> if an instance is found; otherwise <c>false</c>.</returns>
		public bool Exists(DetachedCriteria criteria)
		{
			return 0 != Count(criteria);
		}

		/// <summary>
		/// Check if any instance of the type exists
		/// </summary>
		/// <returns><c>true</c> if an instance is found; otherwise <c>false</c>.</returns>
		public bool Exists()
		{
			return Exists(null);
		}

		/// <summary>
		/// Counts the number of instances matching the criteria.
		/// </summary>
		/// <param name="criteria"></param>
		/// <returns></returns>
		public long Count(DetachedCriteria criteria)
		{
			ICriteria crit = RepositoryHelper<T>.GetExecutableCriteria(Session, criteria, null);
        		crit.SetProjection(Projections.RowCount());
	        	object countMayBe_Int32_Or_Int64_DependingOnDatabase = crit.UniqueResult();
			return Convert.ToInt64(countMayBe_Int32_Or_Int64_DependingOnDatabase);
		}

		/// <summary>
		/// Counts the overall number of instances.
		/// </summary>
		/// <returns></returns>
		public long Count()
		{
			return Count(null);
		}

        protected override DisposableAction<ISession> ActionToBePerformedOnSessionUsedForDbFetches
        {
            get { return new DisposableAction<ISession>(delegate(ISession s) { ; }, Session); }
        }
	}
}
