using System;
using System.Collections.Generic;
using NHibernate.Expression;

namespace Rhino.Commons.Test.Binsor
{
	public class FakeRepository<T> : IRepository<T>{
		/// <summary>
		/// Find a single entity based on a criteria.
		/// Thorws is there is more than one result.
		/// </summary>
		/// <param name="criteria">The criteria to look for</param>
		/// <returns>The entity or null</returns>
		public T FindOne(params ICriterion[] criteria)
		{
			throw new NotImplementedException();
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
		public ICollection<T> FindAll(
			int firstResult, int numberOfResults, Order[] selectionOrder, params ICriterion[] criteria)
		{
			throw new NotImplementedException();
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
		public ICollection<T> FindAll(
			int firstResult, int numberOfResults, Order selectionOrder, params ICriterion[] criteria)
		{
			throw new NotImplementedException();
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
			throw new NotImplementedException();
		}

		/// <summary>
		/// Loads all the entities that match the criteria
		/// </summary>
		/// <param name="criteria">the criteria to look for</param>
		/// <returns>All the entities that match the criteria</returns>
		public ICollection<T> FindAll(params ICriterion[] criteria)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Loads all the entities that match the criteria
		/// by order
		/// </summary>
		/// <param name="criteria">the criteria to look for</param>
		/// <returns>All the entities that match the criteria</returns>
		public ICollection<T> FindAll(Order[] orders, params ICriterion[] criteria)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Loads all the entities that match the criteria
		/// by order
		/// </summary>
		/// <param name="criteria">the criteria to look for</param>
		/// <returns>All the entities that match the criteria</returns>
		public ICollection<T> FindAll(Order order, params ICriterion[] criteria)
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
	}
}