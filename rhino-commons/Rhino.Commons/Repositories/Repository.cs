using System;
using System.Collections.Generic;
using System.Text;
using NHibernate.Expression;

namespace Rhino.Commons
{
    public static class Repository<T> where T : class
    {
        private static IRepository<T> InternalRepository
        {
            get
            {
                return IoC.Resolve<IRepository<T>>();
            }
        }

        /// <summary>
        /// Get the entity from the persistance store, or return null
        /// if it doesn't exist.
        /// </summary>
        /// <param name="id">The entity's id</param>
        /// <returns>Either the entity that matches the id, or a null</returns>
        public static T Get(object id)
        {
            return InternalRepository.Get(id);
        }

        /// <summary>
        /// Load the entity from the persistance store
        /// Will throw an exception if there isn't an entity that matches
        /// the id.
        /// </summary>
        /// <param name="id">The entity's id</param>
        /// <returns>The entity that matches the id</returns>
        public static T Load(object id)
        {
            return InternalRepository.Load(id);
        }

        /// <summary>
        /// Register the entity for deletion when the unit of work
        /// is completed. 
        /// </summary>
        /// <param name="entity">The entity to delete</param>
        public static void Delete(T entity)
        {
            InternalRepository.Delete(entity);
        }

        /// <summary>
        /// Register te entity for save in the database when the unit of work
        /// is completed.
        /// </summary>
        /// <param name="entity">the entity to save</param>
        public static void Save(T entity)
        {
            InternalRepository.Save(entity);
        }

        /// <summary>
        /// Loads all the entities that match the criteria
        /// </summary>
        /// <param name="criteria">the criteria to look for</param>
        /// <returns>All the entities that match the criteria</returns>
        public static ICollection<T> FindAll(params ICriterion[] criteria)
        {
            return InternalRepository.FindAll(criteria);
        }

        /// <summary>
        /// Loads all the entities that match the criteria, and allow paging.
        /// </summary>
        /// <param name="firstResult">The first result to load</param>
        /// <param name="numberOfResults">Total number of results to load</param>
        /// <param name="criteria">the cirteria to look for</param>
        /// <returns>number of Results of entities that match the criteria</returns>
        public static ICollection<T> FindAll(int firstResult, int numberOfResults, params ICriterion[] criteria)
        {
            return InternalRepository.FindAll(firstResult, numberOfResults, criteria);
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
        public static ICollection<T> FindAll(int firstResult, int numberOfResults, Order selectionOrder, params ICriterion[] criteria)
        {
            return InternalRepository.FindAll(firstResult, numberOfResults, selectionOrder, criteria);
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
        public static ICollection<T> FindAll(int firstResult, int numberOfResults, Order[] selectionOrder, params ICriterion[] criteria)
        {
            return InternalRepository.FindAll(firstResult, numberOfResults, selectionOrder, criteria);
        }

        /// <summary>
        /// Execute the named query and return all the results
        /// </summary>
        /// <param name="namedQuery">The named query to execute</param>
        /// <param name="parameters">Parameters for the query</param>
        /// <returns>The results of the query</returns>
        public static ICollection<T> FindAll(string namedQuery, params Parameter[] parameters)
        {
            return InternalRepository.FindAll(namedQuery, parameters);
        }

        /// <summary>
        /// Execute the named query and return paged results
        /// </summary>
        /// <param name="parameters">Parameters for the query</param>
        /// <param name="namedQuery">the query to execute</param>
        /// <param name="firstResult">The first result to return</param>
        /// <param name="numberOfResults">number of records to return</param>
        /// <returns>Paged results of the query</returns>
        public static ICollection<T> FindAll(int firstResult, int numberOfResults, string namedQuery, params Parameter[] parameters)
        {
            return InternalRepository.FindAll(firstResult, numberOfResults, namedQuery, parameters);
        }

        /// <summary>
        /// Find a single entity based on a criteria.
        /// Thorws is there is more than one result.
        /// </summary>
        /// <param name="criteria">The criteria to look for</param>
        /// <returns>The entity or null</returns>
        public static T FindOne(params ICriterion[] criteria)
        {
            return InternalRepository.FindOne(criteria);
        }

        /// <summary>
        /// Find a single entity based on a named query.
        /// Thorws is there is more than one result.
        /// </summary>
        /// <param name="parameters">parameters for the query</param>
        /// <param name="namedQuery">the query to executre</param>
        /// <returns>The entity or null</returns>
        public static T FindOne(string namedQuery, params Parameter[] parameters)
        {
            return InternalRepository.FindOne(namedQuery, parameters);
        }
    }
}
