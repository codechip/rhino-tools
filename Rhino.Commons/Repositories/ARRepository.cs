using System;
using System.Collections.Generic;
using System.Data;
using Castle.ActiveRecord;
using NHibernate;
using NHibernate.Expression;

namespace Rhino.Commons
{
	public class ARRepository<T> : IRepository<T> where T : class
	{
		/// <summary>
		/// Get the entity from the persistance store, or return null
		/// if it doesn't exist.
		/// </summary>
		/// <param name="id">The entity's id</param>
		/// <returns>Either the entity that matches the id, or a null</returns>
		public virtual T Get(object id)
		{
			return ActiveRecordMediator<T>.FindByPrimaryKey(id, false);
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
			return ActiveRecordMediator<T>.FindByPrimaryKey(id, true);
		}

		/// <summary>
		/// Register the entity for deletion when the unit of work
		/// is completed. 
		/// </summary>
		/// <param name="entity">The entity to delete</param>
		public virtual void Delete(T entity)
		{
			ActiveRecordMediator<T>.Delete(entity);
		}

		/// <summary>
		/// Register te entity for save in the database when the unit of work
		/// is completed.
		/// </summary>
		/// <param name="entity">the entity to save</param>
		public virtual void Save(T entity)
		{
			ActiveRecordMediator<T>.Save(entity);
		}

		/// <summary>
		/// Loads all the entities that match the criteria
		/// by order
		/// </summary>
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
				ICriteria crit = RepositoryHelper<T>.GetExecutableCriteria(session, criteria, null);
				crit.SetMaxResults(1);
				return crit.UniqueResult<T>();
			}
			finally
			{
				ReleaseSession(session);
			}
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
					foreach (Parameter parameter in parameters)
					{
						IDbDataParameter sp_arg = command.CreateParameter();
						sp_arg.ParameterName = parameter.Name;
						sp_arg.Value = parameter.Value;
						command.Parameters.Add(sp_arg);
					}
					return (T) command.ExecuteScalar();
				}
			}
			finally
			{
				sessionFactory.ConnectionProvider.CloseConnection(connection);
			}
		}

		/// <summary>
		/// Check if there is any records in the db for <typeparamref name="T"/>
		/// </summary>
		/// <param name="id">the object id</param>
		/// <returns><c>true</c> if there's at least one row</returns>
		public bool Exists(object id)
		{
			return ActiveRecordMediator<T>.Exists(id);
		}

		/// <summary>
		/// Check if any instance matches the criteria.
		/// </summary>
		/// <returns><c>true</c> if an instance is found; otherwise <c>false</c>.</returns>
		public bool Exists(params ICriterion[] criterias)
		{
			return ActiveRecordMediator<T>.Exists(criterias);
		}

		/// <summary>
		/// Check if any instance matches the criteria.
		/// </summary>
		/// <returns><c>true</c> if an instance is found; otherwise <c>false</c>.</returns>
		public bool Exists(DetachedCriteria criteria)
		{
			ISession session = OpenSession();
			try
			{
				criteria.SetProjection(Projections.RowCount());
				ICriteria crit = RepositoryHelper<T>
					.GetExecutableCriteria(session, criteria, null);

				return 0 != crit.UniqueResult<long>();
			}
			finally
			{
				ReleaseSession(session);
			}
		}
	}
}
