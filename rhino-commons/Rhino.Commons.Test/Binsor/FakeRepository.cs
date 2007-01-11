using System;
using System.Collections.Generic;
using NHibernate.Expression;

namespace Rhino.Commons.Test.Binsor
{
	public class FakeRepository<T> : IRepository<T>
	{
		IRepository<T> inner;

		public IRepository<T> Inner
		{
			get { return inner; }
		}

		public FakeRepository(IRepository<T> inner)
		{
			this.inner = inner;
		}

		public T FindOne(params ICriterion[] criteria)
		{
			throw new NotImplementedException();
		}

		public T FindOne(DetachedCriteria criteria)
		{
			throw new NotImplementedException();
		}

			public ICollection<T> FindAll(
			int firstResult, int numberOfResults, Order[] selectionOrder, params ICriterion[] criteria)
		{
			throw new NotImplementedException();
		}

			public ICollection<T> FindAll(
			int firstResult, int numberOfResults, Order selectionOrder, params ICriterion[] criteria)
		{
			throw new NotImplementedException();
		}

			public ICollection<T> FindAll(int firstResult, int numberOfResults, params ICriterion[] criteria)
		{
			throw new NotImplementedException();
		}

			public ICollection<T> FindAll(params ICriterion[] criteria)
		{
			throw new NotImplementedException();
		}

		public ICollection<T> FindAll(Order[] orders, params ICriterion[] criteria)
		{
			throw new NotImplementedException();
		}

			public ICollection<T> FindAll(Order order, params ICriterion[] criteria)
		{
			throw new NotImplementedException();
		}

		public ICollection<T> FindAll(DetachedCriteria criteria, params Order[] orders)
		{
			throw new NotImplementedException();
		}

		public ICollection<T> FindAll(DetachedCriteria criteria, int firstResult, int maxResults, params Order[] orders)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Get the entity from the persistance store, or return null
		/// if it doesn't exist.
		/// </summary>
		/// <param name="id">The entity's id</param>
		/// <returns>Either the entity that matches the id, or a null</returns>
		public T Get(object id)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Load the entity from the persistance store
		/// Will throw an exception if there isn't an entity that matches
		/// the id.
		/// </summary>
		/// <param name="id">The entity's id</param>
		/// <returns>The entity that matches the id</returns>
		public T Load(object id)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Register the entity for deletion when the unit of work
		/// is completed. 
		/// </summary>
		/// <param name="entity">The entity to delete</param>
		public void Delete(T entity)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Register te entity for save in the database when the unit of work
		/// is completed.
		/// </summary>
		/// <param name="entity">the entity to save</param>
		public void Save(T entity)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Execute the named query and return all the results
		/// </summary>
		/// <param name="namedQuery">The named query to execute</param>
		/// <param name="parameters">Parameters for the query</param>
		/// <returns>The results of the query</returns>
		public ICollection<T> FindAll(string namedQuery, params Parameter[] parameters)
		{
			throw new NotImplementedException();
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
			throw new NotImplementedException();
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
			throw new NotImplementedException();
		}

		public T FindFirst(DetachedCriteria criteria, params Order[] orders)
		{
			throw new NotImplementedException();
		}

		public object ExecuteStoredProcedure(string sp_name, params Parameter[] parameters)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Check if there is any records in the db for <typeparamref name="T"/>
		/// </summary>
		/// <param name="id">the object id</param>
		/// <returns><c>true</c> if there's at least one row</returns>
		public bool Exists(object id)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Check if any instance matches the criteria.
		/// </summary>
		/// <returns><c>true</c> if an instance is found; otherwise <c>false</c>.</returns>
		public bool Exists(params ICriterion[] criterias)
		{
			throw new NotImplementedException();
		}

		public bool Exists(DetachedCriteria criteria)
		{
			throw new Exception("The method or operation is not implemented.");
		}
	}
}
