using System;
using System.Collections.Generic;
using System.Data;
using NHibernate;
using NHibernate.Connection;
using NHibernate.Expression;
using Rhino.Commons;

namespace Rhino.Commons
{
	public class NHRepository<T> : IRepository<T>
	{
		protected virtual ISession Session
		{
			get { return NHibernateUnitOfWorkFactory.CurrentNHibernateSession; }
		}

		public T Get(object id)
		{
			return (T) Session.Get(typeof (T), id);
		}

		public T Load(object id)
		{
			return (T) Session.Load(typeof (T), id);
		}

		public void Delete(T entity)
		{
			Session.Delete(entity);
		}

		public void Save(T entity)
		{
			Session.Save(entity);
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
			return (T) crit.UniqueResult();
		}

		public T FindOne(string namedQuery, params Parameter[] parameters)
		{
			IQuery query = RepositoryHelper<T>.CreateQuery(Session, namedQuery, parameters);
			return (T) query.UniqueResult();
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
			return (T) executableCriteria.UniqueResult();
		}

		public T FindFirst(DetachedCriteria criteria, params Order[] orders)
		{
			ICriteria executableCriteria = RepositoryHelper<T>.GetExecutableCriteria(Session, criteria, null);
			executableCriteria.SetFirstResult(0);
			executableCriteria.SetMaxResults(1);
			return (T) executableCriteria.UniqueResult();
		}

		public object ExecuteStoredProcedure(string sp_name, params Parameter[] parameters)
		{
			IConnectionProvider connectionProvider = NHibernateUnitOfWorkFactory.NHibernateSessionFactory.ConnectionProvider;
			IDbConnection connection = connectionProvider.GetConnection();
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
					return command.ExecuteScalar();
				}
			}
			finally
			{
				connectionProvider.CloseConnection(connection);
			}
		}

		/// <summary>
		/// Check if there is any records in the db for <typeparamref name="T"/>
		/// </summary>
		/// <param name="id">the object id</param>
		/// <returns><c>true</c> if there's at least one row</returns>
		public bool Exists(object id)
		{
			return 0 != Session.CreateCriteria(typeof(T))
				.Add(Expression.Eq("id", id))
				.SetProjection(Projections.RowCount())
				.UniqueResult<long>();
		}

		/// <summary>
		/// Check if any instance matches the criteria.
		/// </summary>
		/// <returns><c>true</c> if an instance is found; otherwise <c>false</c>.</returns>
		public bool Exists(params ICriterion[] criterias)
		{
			ICriteria criteria = Session.CreateCriteria(typeof(T));
			foreach (ICriterion criterion in criterias)
			{
				criteria.Add(criterion);
			}
			return 0 != criteria
				.SetProjection(Projections.RowCount())
				.UniqueResult<long>();
		}

		/// <summary>
		/// Check if any instance matches the criteria.
		/// </summary>
		/// <returns><c>true</c> if an instance is found; otherwise <c>false</c>.</returns>
		public bool Exists(DetachedCriteria criteria)
		{
			ICriteria executableCriteria = criteria.SetProjection(Projections.RowCount())
				.GetExecutableCriteria(Session);
			return 0 != executableCriteria.UniqueResult<long>();
		}
	}
}
