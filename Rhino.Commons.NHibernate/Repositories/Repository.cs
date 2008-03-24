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
    public static class Repository<T> 
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
		public static FutureValue<T> FutureGet(object id)
    	{
    		return InternalRepository.FutureGet(id);
    	}

		/// <summary>
		/// Get a future entity from the persistance store, or return null
		/// if it doesn't exist.
		/// Note that the null will be there when you resolve the FutureValue.Value property
		/// </summary>
		/// <param name="id">The entity's id</param>
		/// <returns>A future for the value</returns>
		public static FutureValue<T> FutureLoad(object id)
    	{
    		return InternalRepository.FutureLoad(id);
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
		/// Registers all entities for deletion when the unit of work
		/// is completed.
		/// </summary>
		public static void DeleteAll()
		{
			InternalRepository.DeleteAll();	
		}

        /// <summary>
        /// Registers all entities for deletion that match the supplied
        /// criteria condition when the unit of work is completed.
        /// </summary>
        /// <param name="where">criteria condition to select the rows to be deleted</param>
        public static void DeleteAll(DetachedCriteria where)
		{
			InternalRepository.DeleteAll(where);
		}

    	/// <summary>
    	/// Register te entity for save in the database when the unit of work
    	/// is completed. (INSERT)
    	/// </summary>
    	/// <param name="entity">the entity to save</param>
    	/// <returns>The saved entity</returns>
    	public static T Save(T entity)
    	{
    		return InternalRepository.Save(entity);
    	}

        /// <summary>
        /// Saves or update the entity, based on its usaved-value
        /// </summary>
        /// <param name="entity"></param>
        /// <returns>The saved or updated entity</returns>
        public static T SaveOrUpdate(T entity)
        {
            return InternalRepository.SaveOrUpdate(entity);
        }

        /// <summary>
        /// Saves or update the copy of entity, based on its usaved-value
        /// </summary>
        /// <param name="entity"></param>
        public static T SaveOrUpdateCopy(T entity)
        {
            return InternalRepository.SaveOrUpdateCopy(entity);
        }

        /// <summary>
        /// Register the entity for update in the database when the unit of work
        /// is completed. (UPDATE)
        /// </summary>
        /// <param name="entity"></param>
        public static void Update(T entity)
        {
            InternalRepository.Update(entity);
        }

        /// <summary>
    	/// Loads all the entities that match the criteria
    	/// by order
    	/// </summary>
    	/// <param name="order"></param>
    	/// <param name="criteria">the criteria to look for</param>
    	/// <returns>All the entities that match the criteria</returns>
    	public static ICollection<T> FindAll(Order order, params ICriterion[] criteria)
    	{
    		return InternalRepository.FindAll(order, criteria);
    	}

    	/// <summary>
    	/// Loads all the entities that match the criteria
    	/// by order
    	/// </summary>
    	/// <param name="criteria">the criteria to look for</param>
    	/// <param name="orders"> the order to load the entities</param>
    	/// <returns>All the entities that match the criteria</returns>
    	public static ICollection<T> FindAll(DetachedCriteria criteria, params Order[] orders)
    	{
    		return InternalRepository.FindAll(criteria, orders);
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
    	public static ICollection<T> FindAll(DetachedCriteria criteria, int firstResult, int maxResults, params Order[] orders)
    	{
    		return InternalRepository.FindAll(criteria, firstResult, maxResults, orders);
    	}

    	/// <summary>
    	/// Loads all the entities that match the criteria
    	/// by order
    	/// </summary>
    	/// <param name="orders"></param>
    	/// <param name="criteria">the criteria to look for</param>
    	/// <returns>All the entities that match the criteria</returns>
    	public static ICollection<T> FindAll(Order[] orders, params ICriterion[] criteria)
    	{
    		return InternalRepository.FindAll(orders, criteria);
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
    	/// </summary>
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
    	/// Find a single entity based on a criteria.
    	/// Thorws is there is more than one result.
    	/// </summary>
    	/// <param name="criteria">The criteria to look for</param>
    	/// <returns>The entity or null</returns>
    	public static T FindOne(DetachedCriteria criteria)
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

    	/// <summary>
    	/// Find the entity based on a criteria.
    	/// </summary>
    	/// <param name="criteria">The criteria to look for</param>
    	/// <param name="orders">Optional orderring</param>
    	/// <returns>The entity or null</returns>
    	public static T FindFirst(DetachedCriteria criteria, params Order[] orders)
    	{
    		return InternalRepository.FindFirst(criteria, orders);
    	}

		/// <summary>
		/// Find the first entity of type
		/// </summary>
		/// <param name="orders">Optional orderring</param>
		/// <returns>The entity or null</returns>
		public static T FindFirst(params Order[] orders)
		{
			return InternalRepository.FindFirst(orders);
		}

    	/// <summary>
    	/// Execute the specified stored procedure with the given parameters
    	/// and return the result.
    	/// Note: only scalar values are supported using this approach.
    	/// </summary>
    	/// <param name="sp_name">The name of the stored procedure</param>
    	/// <param name="parameters">parameters for the stored procedure</param>
    	/// <returns>return value</returns>
    	public static object ExecuteStoredProcedure(string sp_name, params Parameter[] parameters)
    	{
    		return InternalRepository.ExecuteStoredProcedure(sp_name, parameters);
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
    	public static ICollection<T2> ExecuteStoredProcedure<T2>(Converter<IDataReader, T2> converter, string sp_name, params Parameter[] parameters)
    	{
    		return InternalRepository.ExecuteStoredProcedure(converter, sp_name, parameters);
    	}

    	/// <summary>
    	/// Check if any instance matches the criteria.
    	/// </summary>
    	/// <returns><c>true</c> if an instance is found; otherwise <c>false</c>.</returns>
    	public static bool Exists(DetachedCriteria criteria)
    	{
    		return InternalRepository.Exists(criteria);
    	}

		/// <summary>
		/// Check if any instance of the type exists
		/// </summary>
		/// <returns><c>true</c> if an instance is found; otherwise <c>false</c>.</returns>
		public static bool Exists()
		{
			return InternalRepository.Exists();
		}

    	/// <summary>
    	/// Counts the number of instances matching the criteria.
    	/// </summary>
    	/// <param name="criteria"></param>
    	/// <returns></returns>
    	public static long Count(DetachedCriteria criteria)
    	{
    		return InternalRepository.Count(criteria);
    	}

		/// <summary>
		/// Counts the overall number of instances.
		/// </summary>
		/// <returns></returns>
    	public static long Count()
		{
			return InternalRepository.Count();
		}

        /// <summary>
        /// See <see cref="IRepository{T}.ReportOne{ProjT}(DetachedCriteria,ProjectionList)"/>
        /// </summary>
        public static ProjT ReportOne<ProjT>(DetachedCriteria criteria, ProjectionList projectionList)
        {
            return InternalRepository.ReportOne<ProjT>(criteria, projectionList);
        }


        /// <summary>
        /// See <see cref="IRepository{T}.ReportOne{ProjT}(DetachedCriteria,ProjectionList)"/>
        /// </summary>
        public static ProjT ReportOne<ProjT>(ProjectionList projectionList, params ICriterion[] criteria)
        {
            return InternalRepository.ReportOne<ProjT>(projectionList, criteria);
        }


        /// <summary>
        /// See <see cref="IRepository{T}.ReportAll{ProjT}(DetachedCriteria,ProjectionList,Order[])"/>
        /// </summary>
        public static ICollection<ProjT> ReportAll<ProjT>(ProjectionList projectionList)
        {
            return InternalRepository.ReportAll<ProjT>(projectionList);
        }


        public static ICollection<ProjT> ReportAll<ProjT>(ProjectionList projectionList, bool distinctResults)
        {
            return InternalRepository.ReportAll<ProjT>(projectionList, true);
        }


        /// <summary>
        /// See <see cref="IRepository{T}.ReportAll{ProjT}(DetachedCriteria,ProjectionList,Order[])"/>
        /// </summary>
        public static ICollection<ProjT> ReportAll<ProjT>(DetachedCriteria criteria, ProjectionList projectionList)
        {
            return InternalRepository.ReportAll<ProjT>(criteria, projectionList);
        }


        /// <summary>
        /// See <see cref="IRepository{T}.ReportAll{ProjT}(DetachedCriteria,ProjectionList,Order[])"/>
        /// </summary>
        public static ICollection<ProjT> ReportAll<ProjT>(ProjectionList projectionList, params ICriterion[] criteria)
        {
            return InternalRepository.ReportAll<ProjT>(projectionList, criteria);
        }


        /// <summary>
        /// See <see cref="IRepository{T}.ReportAll{ProjT}(DetachedCriteria,ProjectionList,Order[])"/>
        /// </summary>
        public static ICollection<ProjT> ReportAll<ProjT>(ProjectionList projectionList, params Order[] orders)
        {
            return InternalRepository.ReportAll<ProjT>(projectionList, orders);
        }


        /// <summary>
        /// See <see cref="IRepository{T}.ReportAll{ProjT}(DetachedCriteria,ProjectionList,Order[])"/>
        /// </summary>
        public static ICollection<ProjT> ReportAll<ProjT>(DetachedCriteria criteria, ProjectionList projectionList, params Order[] orders)
        {
            return InternalRepository.ReportAll<ProjT>(criteria, projectionList, orders);
        }


        /// <summary>
        /// See <see cref="IRepository{T}.ReportAll{ProjT}(DetachedCriteria,ProjectionList,Order[])"/>
        /// </summary>
        public static ICollection<ProjT> ReportAll<ProjT>(ProjectionList projectionList, Order[] orders, ICriterion criteria)
        {
            return InternalRepository.ReportAll<ProjT>(projectionList, orders, criteria);
        }


        /// <summary>
        /// See <see cref="IRepository{T}.ReportAll{ProjT}(string,Parameter[])"/>
        /// </summary>
        public static ICollection<ProjJ> ReportAll<ProjJ>(string namedQuery, params Parameter[] parameters)
        {
            return InternalRepository.ReportAll<ProjJ>(namedQuery, parameters);
        }

		/// <summary>
		/// See <see cref="IRepository{T}.CreateDetachedCriteria()"/>
		/// </summary>
		/// <returns></returns>
		public static DetachedCriteria CreateDetachedCriteria()
		{
			return InternalRepository.CreateDetachedCriteria();
		}

		/// <summary>
		/// See <see cref="IRepository{T}.CreateDetachedCriteria(string)"/>
		/// </summary>
		/// <returns></returns>
		public static DetachedCriteria CreateDetachedCriteria(string alias)
		{
			return InternalRepository.CreateDetachedCriteria(alias);
		}


		/// <summary>
		/// Create an instance of <typeparamref name="T"/>, mapping it to the concrete class 
		/// if needed
		/// </summary>
		/// <returns></returns>
		public static T Create()
    	{
    		return InternalRepository.Create();
    	}
    }
}
