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
using Castle.ActiveRecord;
using NHibernate;
using NHibernate.Expression;

namespace Rhino.Commons
{
    public class ARRepository<T> : RepositoryImplBase<T>, IRepository<T>
	{
		/// <summary>
		/// Get the entity from the persistance store, or return null
		/// if it doesn't exist.
		/// </summary>
		/// <param name="id">The entity's id</param>
		/// <returns>Either the entity that matches the id, or a null</returns>
		public virtual T Get(object id)
		{
			return (T)ActiveRecordMediator.FindByPrimaryKey(typeof(T),id, false);
		}

		/// <summary>
		/// Load the entity from the persistance store
		/// Will throw an exception if there isn't an entity that matches
		/// the id.
		/// </summary>
		/// <param name="id">The entity's id</param>
		/// <returns>The entity that matches the id</returns>
		public virtual T Load(object id)
		{
			return (T)ActiveRecordMediator.FindByPrimaryKey(typeof(T),id, true);
		}

		/// <summary>
		/// Register the entity for deletion when the unit of work
		/// is completed. 
		/// </summary>
		/// <param name="entity">The entity to delete</param>
		public virtual void Delete(T entity)
		{
			ActiveRecordMediator.Delete(entity);
		}

		/// <summary>
		/// Registers all entities for deletion when the unit of work
		/// is completed.
		/// </summary>
		public virtual void DeleteAll()
		{
			ActiveRecordMediator.DeleteAll(typeof (T));
		}

        /// <summary>
        /// Registers all entities for deletion that match the supplied
        /// criteria condition when the unit of work is completed.
        /// </summary>
        /// <param name="where">criteria condition to select the rows to be deleted</param>
        public void DeleteAll(DetachedCriteria where)
		{
            foreach (T entity in ActiveRecordMediator.FindAll(typeof(T), where))
            {
                ActiveRecordMediator.Delete(entity);
            }
		}

		/// <summary>
		/// Register te entity for save in the database when the unit of work
		/// is completed.
		/// </summary>
		/// <param name="entity">the entity to save</param>
		public virtual void Save(T entity)
		{
			ActiveRecordMediator.Create(entity);
		}

        /// <summary>
        /// Saves or update the entity, based on its usaved-value
        /// </summary>
        /// <param name="entity"></param>
	    public void SaveOrUpdate(T entity)
	    {
	        ActiveRecordMediator.Save(entity);
	    }

        /// <summary>
        /// Saves or update a copy of the entity, based on its usaved-value
        /// </summary>
        /// <param name="entity"></param>
        public T SaveOrUpdateCopy(T entity)
        {
            return (T) ActiveRecordMediator.SaveCopy(entity);
        }

	    /// <summary>
	    /// Register the entity for update in the database when the unit of work
	    /// is completed. (UPDATE)
	    /// </summary>
	    /// <param name="entity"></param>
	    public void Update(T entity)
	    {
	        ActiveRecordMediator.Update(entity);
	    }

	    /// <summary>
        /// Loads all the entities that match the criteria
        /// by order
        /// </summary>
        /// <param name="order">the order in which to bring the data</param>
        /// <param name="criteria">the criteria to look for</param>
        /// <returns>All the entities that match the criteria</returns>
		public ICollection<T> FindAll(Order order, params ICriterion[] criteria)
		{
			ISession session = OpenSession();
			try
			{
				ICriteria crit = RepositoryHelper<T>.CreateCriteriaFromArray(session, criteria);
				crit.AddOrder(order);
				return crit.List<T>();
			}
			finally
			{
				ReleaseSession(session);
			}
		}

		private void ReleaseSession(ISession session)
		{
			ActiveRecordMediator.GetSessionFactoryHolder().ReleaseSession(session);
		}

		private ISession OpenSession()
		{
			return ActiveRecordMediator.GetSessionFactoryHolder().CreateSession(typeof (T));
		}

		/// <summary>
		/// Loads all the entities that match the criteria
		/// by order
		/// </summary>
		/// <param name="criteria">the criteria to look for</param>
		/// <param name="orders"> the order to load the entities</param>
		/// <returns>All the entities that match the criteria</returns>
		public ICollection<T> FindAll(DetachedCriteria criteria, params Order[] orders)
		{
			ISession session = OpenSession();
			try
			{
				ICriteria crit = RepositoryHelper<T>.GetExecutableCriteria(session, criteria, orders);
				return crit.List<T>();
			}
			finally
			{
				ReleaseSession(session);
			}
		}

		/// <summary>
		/// Loads all the entities that match the criteria
		/// by order
		/// </summary>
		/// <param name="criteria">the criteria to look for</param>
		/// <param name="orders"> the order to load the entities</param>
		/// <param name="firstResult">the first result to load</param>
		/// <param name="maxResults">the number of result to load</param>
		/// <returns>All the entities that match the criteria</returns>
		public ICollection<T> FindAll(DetachedCriteria criteria, int firstResult, int maxResults, params Order[] orders)
		{
			ISession session = OpenSession();
			try
			{
				ICriteria crit = RepositoryHelper<T>.GetExecutableCriteria(session, criteria, orders);
				crit.SetFirstResult(firstResult)
					.SetMaxResults(maxResults);
				return crit.List<T>();
			}
			finally
			{
				ReleaseSession(session);
			}
		}

		/// <summary>
		/// Loads all the entities that match the criteria
		/// by order
		/// </summary>
		/// <param name="criteria">the criteria to look for</param>
		/// <returns>All the entities that match the criteria</returns>
		public ICollection<T> FindAll(Order[] orders, params ICriterion[] criteria)
		{
			ISession session = OpenSession();
			try
			{
				ICriteria crit = RepositoryHelper<T>.CreateCriteriaFromArray(session, criteria);
				foreach (Order order in orders)
				{
					crit.AddOrder(order);
				}
				return crit.List<T>();
			}
			finally
			{
				ReleaseSession(session);
			}
		}

		/// <summary>
		/// Loads all the entities that match the criteria
		/// </summary>
		/// <param name="criteria">the criteria to look for</param>
		/// <returns>All the entities that match the criteria</returns>
		public ICollection<T> FindAll(params ICriterion[] criteria)
		{
			ISession session = OpenSession();
			try
			{
				ICriteria crit = RepositoryHelper<T>.CreateCriteriaFromArray(session, criteria);
				return crit.List<T>();
			}
			finally
			{
				ReleaseSession(session);
			}
		}

		/// <summary>
		/// Loads all the entities that match the criteria, and allow paging.
		/// </summary>
		/// <param name="firstResult">The first result to load</param>
		/// <param name="numberOfResults">Total number of results to load</param>
		/// <param name="criteria">the cirteria to look for</param>
		/// <returns>number of Results of entities that match the criteria</returns>
		public ICollection<T> FindAll(int firstResult, int numberOfResults, params ICriterion[] criteria)
		{
			ISession session = OpenSession();
			try
			{
				ICriteria crit = RepositoryHelper<T>.CreateCriteriaFromArray(session, criteria);
				crit.SetFirstResult(firstResult)
					.SetMaxResults(numberOfResults);
				return crit.List<T>();
			}
			finally
			{
				ReleaseSession(session);
			}
		}

		/// <summary>
		/// Loads all the entities that match the criteria, with paging 
		/// and orderring by a single field.
		/// <param name="firstResult">The first result to load</param>
		/// <param name="numberOfResults">Total number of results to load</param>
		/// <param name="criteria">the cirteria to look for</param>
		/// <returns>number of Results of entities that match the criteria</returns>
		/// <param name="selectionOrder">The field the repository should order by</param>
		/// <returns>number of Results of entities that match the criteria</returns>
		public ICollection<T> FindAll(int firstResult, int numberOfResults, Order selectionOrder, params ICriterion[] criteria)
		{
			ISession session = OpenSession();
			try
			{
				ICriteria crit = RepositoryHelper<T>.CreateCriteriaFromArray(session, criteria);
				crit.SetFirstResult(firstResult)
					.SetMaxResults(numberOfResults)
					.AddOrder(selectionOrder);
				return crit.List<T>();
			}
			finally
			{
				ReleaseSession(session);
			}
		}

		/// <summary>
		/// Loads all the entities that match the criteria, with paging 
		/// and orderring by a multiply fields.
		/// </summary>
		/// <param name="firstResult">The first result to load</param>
		/// <param name="numberOfResults">Total number of results to load</param>
		/// <param name="criteria">the cirteria to look for</param>
		/// <returns>number of Results of entities that match the criteria</returns>
		/// <param name="selectionOrder">The fields the repository should order by</param>
		public ICollection<T> FindAll(int firstResult, int numberOfResults, Order[] selectionOrder, params ICriterion[] criteria)
		{
			ISession session = OpenSession();
			try
			{
				ICriteria crit = RepositoryHelper<T>.CreateCriteriaFromArray(session, criteria);
				crit.SetFirstResult(firstResult)
					.SetMaxResults(numberOfResults);
				foreach (Order order in selectionOrder)
				{
					crit.AddOrder(order);
				}
				return crit.List<T>();
			}
			finally
			{
				ReleaseSession(session);
			}
		}

		/// <summary>
		/// Execute the named query and return all the results
		/// </summary>
		/// <param name="namedQuery">The named query to execute</param>
		/// <param name="parameters">Parameters for the query</param>
		/// <returns>The results of the query</returns>
		public ICollection<T> FindAll(string namedQuery, params Parameter[] parameters)
		{
			ISession session = OpenSession();
			try
			{
				IQuery query = RepositoryHelper<T>.CreateQuery(session, namedQuery, parameters);
				return query.List<T>();
			}
			finally
			{
				ReleaseSession(session);
			}
		}

		/// <summary>
		/// Execute the named query and return paged results
		/// </summary>
		/// <param name="parameters">Parameters for the query</param>
		/// <param name="namedQuery">the query to execute</param>
		/// <param name="firstResult">The first result to return</param>
		/// <param name="numberOfResults">number of records to return</param>
		/// <returns>Paged results of the query</returns>
		public ICollection<T> FindAll(int firstResult, int numberOfResults, string namedQuery, params Parameter[] parameters)
		{
			ISession session = OpenSession();
			try
			{
				IQuery query = RepositoryHelper<T>.CreateQuery(session, namedQuery, parameters);
				query.SetFirstResult(firstResult)
					.SetMaxResults(numberOfResults);
				return query.List<T>();
			}
			finally
			{
				ReleaseSession(session);
			}
		}

		/// <summary>
		/// Find a single entity based on a criteria.
		/// Thorws is there is more than one result.
		/// </summary>
		/// <param name="criteria">The criteria to look for</param>
		/// <returns>The entity or null</returns>
		public T FindOne(params ICriterion[] criteria)
		{
			ISession session = OpenSession();
			try
			{
				ICriteria crit = RepositoryHelper<T>.CreateCriteriaFromArray(session, criteria);
				return crit.UniqueResult<T>();
			}
			finally
			{
				ReleaseSession(session);
			}
		}

		/// <summary>
		/// Find a single entity based on a criteria.
		/// Thorws is there is more than one result.
		/// </summary>
		/// <param name="criteria">The criteria to look for</param>
		/// <returns>The entity or null</returns>
		public T FindOne(DetachedCriteria criteria)
		{
			ISession session = OpenSession();
			try
			{
				ICriteria crit = RepositoryHelper<T>.GetExecutableCriteria(session, criteria, null);
				return crit.UniqueResult<T>();
			}
			finally
			{
				ReleaseSession(session);
			}
		}

		/// <summary>
		/// Find a single entity based on a named query.
		/// Thorws is there is more than one result.
		/// </summary>
		/// <param name="parameters">parameters for the query</param>
		/// <param name="namedQuery">the query to executre</param>
		/// <returns>The entity or null</returns>
		public T FindOne(string namedQuery, params Parameter[] parameters)
		{
			ISession session = OpenSession();
			try
			{
				IQuery query = RepositoryHelper<T>.CreateQuery(session, namedQuery, parameters);
				return query.UniqueResult<T>();
			}
			finally
			{
				ReleaseSession(session);
			}
		}

		/// <summary>
		/// Find the entity based on a criteria.
		/// </summary>
		/// <param name="criteria">The criteria to look for</param>
		/// <param name="orders">Optional orderring</param>
		/// <returns>The entity or null</returns>
		public T FindFirst(DetachedCriteria criteria, params Order[] orders)
		{
			ISession session = OpenSession();
			try
			{
				ICriteria crit = RepositoryHelper<T>.GetExecutableCriteria(session, criteria, orders);
				crit.SetMaxResults(1);
				return crit.UniqueResult<T>();
			}
			finally
			{
				ReleaseSession(session);
			}
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
			ISessionFactory sessionFactory = ActiveRecordMediator.GetSessionFactoryHolder().GetSessionFactory(typeof (T));
			IDbConnection connection = sessionFactory.ConnectionProvider.GetConnection();
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
				sessionFactory.ConnectionProvider.CloseConnection(connection);
			}
		}

		public ICollection<T2> ExecuteStoredProcedure<T2>(Converter<IDataReader, T2> converter, string sp_name,
		                                                  params Parameter[] parameters)
		{
			ISessionFactory sessionFactory = ActiveRecordMediator.GetSessionFactoryHolder().GetSessionFactory(typeof(T));
			IDbConnection connection = sessionFactory.ConnectionProvider.GetConnection();

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
				sessionFactory.ConnectionProvider.CloseConnection(connection);
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
		/// <returns></returns>
		public long Count(DetachedCriteria criteria)
		{
			ISession session = OpenSession();
			try
			{
				ICriteria crit = RepositoryHelper<T>.GetExecutableCriteria(session, criteria, null);
                crit.SetProjection(Projections.RowCount());
                object countMayBe_Int32_Or_Int64_DependingOnDatabase = crit.UniqueResult();
				return Convert.ToInt64(countMayBe_Int32_Or_Int64_DependingOnDatabase);
			}
			finally
			{
				ReleaseSession(session);
			}
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
            //create a new session that is disposed of at the end of a database fetch
            get { return new DisposableAction<ISession>(delegate(ISession s) { ReleaseSession(s); }, OpenSession()); }
        }
	}
}
