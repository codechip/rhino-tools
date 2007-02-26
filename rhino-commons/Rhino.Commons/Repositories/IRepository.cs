using System;
using System.Collections.Generic;
using System.Data;
using NHibernate.Expression;

namespace Rhino.Commons
{
    public interface IRepository<T>
    {
        /// <summary>
        /// Get the entity from the persistance store, or return null
        /// if it doesn't exist.
        /// </summary>
        /// <param name="id">The entity's id</param>
        /// <returns>Either the entity that matches the id, or a null</returns>
        T Get(object id);

        /// <summary>
        /// Load the entity from the persistance store
        /// Will throw an exception if there isn't an entity that matches
        /// the id.
        /// </summary>
        /// <param name="id">The entity's id</param>
        /// <returns>The entity that matches the id</returns>
        T Load(object id);

        /// <summary>
        /// Register the entity for deletion when the unit of work
        /// is completed. 
        /// </summary>
        /// <param name="entity">The entity to delete</param>
        void Delete(T entity);

        /// <summary>
        /// Register te entity for save in the database when the unit of work
        /// is completed.
        /// </summary>
        /// <param name="entity">the entity to save</param>
        void Save(T entity);

        /// <summary>
        /// Loads all the entities that match the criteria
        /// by order
        /// </summary>
        /// <param name="criteria">the criteria to look for</param>
        /// <returns>All the entities that match the criteria</returns>
        ICollection<T> FindAll(Order order, params ICriterion[] criteria);

		/// <summary>
		/// Loads all the entities that match the criteria
		/// by order
		/// </summary>
		/// <param name="criteria">the criteria to look for</param>
		/// <param name="orders"> the order to load the entities</param>
		/// <returns>All the entities that match the criteria</returns>
		ICollection<T> FindAll(DetachedCriteria criteria, params Order []orders);

		/// <summary>
		/// Loads all the entities that match the criteria
		/// by order
		/// </summary>
		/// <param name="criteria">the criteria to look for</param>
		/// <param name="orders"> the order to load the entities</param>
		/// <param name="firstResult">the first result to load</param>
		/// <param name="maxResults">the number of result to load</param>
		/// <returns>All the entities that match the criteria</returns>
		ICollection<T> FindAll(DetachedCriteria criteria, 
		                       int firstResult, int maxResults,
		                       params Order[] orders);
    	
    	
        /// <summary>
        /// Loads all the entities that match the criteria
        /// by order
        /// </summary>
        /// <param name="criteria">the criteria to look for</param>
        /// <returns>All the entities that match the criteria</returns>
        ICollection<T> FindAll(Order[] orders, params ICriterion[] criteria);
    	
        /// <summary>
        /// Loads all the entities that match the criteria
        /// </summary>
        /// <param name="criteria">the criteria to look for</param>
        /// <returns>All the entities that match the criteria</returns>
        ICollection<T> FindAll(params ICriterion[] criteria);

        /// <summary>
        /// Loads all the entities that match the criteria, and allow paging.
        /// </summary>
        /// <param name="firstResult">The first result to load</param>
        /// <param name="numberOfResults">Total number of results to load</param>
        /// <param name="criteria">the cirteria to look for</param>
        /// <returns>number of Results of entities that match the criteria</returns>
        ICollection<T> FindAll(int firstResult, int numberOfResults, params ICriterion[] criteria);

        /// <summary>
        /// Loads all the entities that match the criteria, with paging 
        /// and orderring by a single field.
        /// <param name="firstResult">The first result to load</param>
        /// <param name="numberOfResults">Total number of results to load</param>
        /// <param name="criteria">the cirteria to look for</param>
        /// <returns>number of Results of entities that match the criteria</returns>
        /// <param name="selectionOrder">The field the repository should order by</param>
        /// <returns>number of Results of entities that match the criteria</returns>
        ICollection<T> FindAll(int firstResult, int numberOfResults, 
                               Order selectionOrder,
                               params ICriterion[] criteria);

        /// <summary>
        /// Loads all the entities that match the criteria, with paging 
        /// and orderring by a multiply fields.
        /// </summary>
        /// <param name="firstResult">The first result to load</param>
        /// <param name="numberOfResults">Total number of results to load</param>
        /// <param name="criteria">the cirteria to look for</param>
        /// <returns>number of Results of entities that match the criteria</returns>
        /// <param name="selectionOrder">The fields the repository should order by</param>
        ICollection<T> FindAll(int firstResult, int numberOfResults,
                               Order[] selectionOrder,
                               params ICriterion[] criteria);

        /// <summary>
        /// Execute the named query and return all the results
        /// </summary>
        /// <param name="namedQuery">The named query to execute</param>
        /// <param name="parameters">Parameters for the query</param>
        /// <returns>The results of the query</returns>
        ICollection<T> FindAll(string namedQuery, params Parameter[] parameters);
        
        /// <summary>
        /// Execute the named query and return paged results
        /// </summary>
        /// <param name="parameters">Parameters for the query</param>
        /// <param name="namedQuery">the query to execute</param>
        /// <param name="firstResult">The first result to return</param>
        /// <param name="numberOfResults">number of records to return</param>
        /// <returns>Paged results of the query</returns>
        ICollection<T> FindAll(int firstResult, int numberOfResults, string namedQuery, params Parameter[] parameters);

        /// <summary>
        /// Find a single entity based on a criteria.
        /// Thorws is there is more than one result.
        /// </summary>
        /// <param name="criteria">The criteria to look for</param>
        /// <returns>The entity or null</returns>
        T FindOne(params ICriterion[] criteria);

		/// <summary>
		/// Find a single entity based on a criteria.
		/// Thorws is there is more than one result.
		/// </summary>
		/// <param name="criteria">The criteria to look for</param>
		/// <returns>The entity or null</returns>
		T FindOne(DetachedCriteria criteria);
    	
        /// <summary>
        /// Find a single entity based on a named query.
        /// Thorws is there is more than one result.
        /// </summary>
        /// <param name="parameters">parameters for the query</param>
        /// <param name="namedQuery">the query to executre</param>
        /// <returns>The entity or null</returns>
        T FindOne(string namedQuery, params Parameter[] parameters);


		/// <summary>
		/// Find the entity based on a criteria.
		/// </summary>
		/// <param name="criteria">The criteria to look for</param>
		/// <param name="orders">Optional orderring</param>
		/// <returns>The entity or null</returns>
		T FindFirst(DetachedCriteria criteria, params Order[] orders);

		/// <summary>
		/// Find the first entity of type
		/// </summary>
		/// <param name="orders">Optional orderring</param>
		/// <returns>The entity or null</returns>
		T FindFirst(params Order[] orders);

		/// <summary>
		/// Execute the specified stored procedure with the given parameters
		/// and return the result.
		/// Note: only scalar values are supported using this approach.
		/// </summary>
		/// <param name="sp_name">The name of the stored procedure</param>
		/// <param name="parameters">parameters for the stored procedure</param>
		/// <returns>return value</returns>
    	object ExecuteStoredProcedure(string sp_name, params Parameter[] parameters);

		/// <summary>
		/// Execute the specified stored procedure with the given parameters and then converts
		/// the results using the supplied delegate.
		/// </summary>
		/// <typeparam name="T2">The collection type to return.</typeparam>
		/// <param name="converter">The delegate which converts the raw results.</param>
		/// <param name="sp_name">The name of the stored procedure.</param>
		/// <param name="parameters">Parameters for the stored procedure.</param>
		/// <returns></returns>
    	ICollection<T2> ExecuteStoredProcedure<T2>(Converter<IDataReader, T2> converter, string sp_name,
    	                                           params Parameter[] parameters);

		/// <summary>
		/// Check if any instance matches the criteria.
		/// </summary>
		/// <returns><c>true</c> if an instance is found; otherwise <c>false</c>.</returns>
		bool Exists(DetachedCriteria criteria);

		/// <summary>
		/// Check if any instance of the type exists
		/// </summary>
		/// <returns><c>true</c> if an instance is found; otherwise <c>false</c>.</returns>
		bool Exists();

		/// <summary>
		/// Counts the number of instances matching the criteria.
		/// </summary>
		/// <param name="criteria"></param>
		/// <returns></returns>
    	long Count(DetachedCriteria criteria);

		/// <summary>
		/// Counts the overall number of instances.
		/// </summary>
		/// <returns></returns>
    	long Count();
    }
}
