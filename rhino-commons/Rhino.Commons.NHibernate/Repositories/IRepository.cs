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
		/// Get a future entity from the persistance store, or return null
		/// if it doesn't exist.
		/// Note that the null will be there when you resolve the FutureValue.Value property
		/// </summary>
		/// <param name="id">The entity's id</param>
		/// <returns>A future for the value</returns>
		FutureValue<T> FutureGet(object id);

		/// <summary>
		/// A future of the entity loaded from the persistance store
		/// Will throw an exception if there is no entity with a matching id.
		/// </summary>
		/// <param name="id">The entity's id</param>
		/// <returns>The entity that matches the id</returns>
		FutureValue<T> FutureLoad(object id);

        /// <summary>
        /// Load the entity from the persistance store
        /// Will throw an exception if there is no entity with a matching id.
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
		/// Registers all entities for deletion when the unit of work
		/// is completed.
		/// </summary>
    	void DeleteAll();

		/// <summary>
		/// Registers all entities for deletion that match the supplied
        /// criteria condition when the unit of work is completed.
		/// </summary>
		/// <param name="where">criteria condition to select the rows to be deleted</param>
        void DeleteAll(DetachedCriteria where);

        /// <summary>
        /// Register te entity for save in the database when the unit of work
        /// is completed. (INSERT)
        /// </summary>
        /// <param name="entity">the entity to save</param>
        /// <returns>The saved entity</returns>
        T Save(T entity);

        /// <summary>
        /// Saves or updates the entity, based on its unsaved-value.
        /// </summary>
        /// <param name="entity">The entity to save or update.</param>
        /// <returns>The saved or updated entity</returns>
        T SaveOrUpdate(T entity);

        /// <summary>
        /// Saves or updates a copy of entity, based on its unsaved-value.
        /// </summary>
        /// <param name="entity"></param>
        /// <returns>The saved entity</returns>
        T SaveOrUpdateCopy(T entity);

        /// <summary>
        /// Register the entity for update in the database when the unit of work
        /// is completed. (UPDATE)
        /// </summary>
        /// <param name="entity"></param>
        void Update(T entity);

        /// <summary>
        /// Loads all the entities that match the criteria
        /// by order
        /// </summary>
        /// <param name="order"></param>
        /// <param name="criteria">the criteria to look for</param>
        /// <returns>All the entities that match the criteria</returns>
        ICollection<T> FindAll(Order order, params ICriterion[] criteria);

		/// <summary>
		/// Loads all the entities that match the criteria
		/// by order.
		/// </summary>
		/// <param name="criteria">the criteria to look for</param>
		/// <param name="orders"> the order to load the entities</param>
		/// <returns>All the entities that match the criteria</returns>
		ICollection<T> FindAll(DetachedCriteria criteria, params Order []orders);

		/// <summary>
		/// Loads all the entities that match the criteria
		/// by order.
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
        /// by order.
        /// </summary>
        /// <param name="orders"></param>
        /// <param name="criteria">the criteria to look for</param>
        /// <returns>All the entities that match the criteria</returns>
        ICollection<T> FindAll(Order[] orders, params ICriterion[] criteria);
    	
        /// <summary>
        /// Loads all the entities that match the criteria.
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
        /// and ordering by a single field.
        /// <param name="firstResult">The first result to load</param>
        /// <param name="numberOfResults">Total number of results to load</param>
        /// <param name="criteria">the cirteria to look for</param>
        /// <returns>number of Results of entities that match the criteria</returns>
        /// <param name="selectionOrder">The field the repository should order by</param>
        /// <returns>number of Results of entities that match the criteria</returns>
        /// </summary>
        ICollection<T> FindAll(int firstResult, int numberOfResults, 
                               Order selectionOrder,
                               params ICriterion[] criteria);

        /// <summary>
        /// Loads all the entities that match the criteria, with paging 
        /// and ordering by a multiple fields.
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
        /// Throws is there is more than one result.
        /// </summary>
        /// <param name="criteria">The criteria to look for</param>
        /// <returns>The entity or null</returns>
        T FindOne(params ICriterion[] criteria);

		/// <summary>
		/// Find a single entity based on a criteria.
		/// Throws is there is more than one result.
		/// </summary>
		/// <param name="criteria">The criteria to look for</param>
		/// <returns>The entity or null</returns>
		T FindOne(DetachedCriteria criteria);
    	
        /// <summary>
        /// Find a single entity based on a named query.
		/// Throws is there is more than one result.
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

        /// <summary>
        /// Create the project of type <typeparamref name="ProjT"/> (ie a
        /// DataTransferObject) that satisfy the criteria supplied. Throws a
        /// NHibernate.NonUniqueResultException if there is more than one
        /// result.
        /// </summary>
        /// <param name="criteria">The criteria to look for</param>
        /// <param name="projectionList">Maps the properties from the object 
        /// graph satisfiying <paramref name="criteria"/>  to the DTO 
        /// <typeparamref name="ProjT"/></param>
        /// <returns>The DTO or null</returns>
        /// <remarks>
        /// The intent is for <paramref name="criteria"/> to be based (rooted)
        /// on <typeparamref name="T"/>. This is not enforced but is a
        /// convention that should be followed
        /// </remarks>
        ProjT ReportOne<ProjT>(DetachedCriteria criteria, ProjectionList projectionList);


        /// <summary>
        /// <seealso cref="ReportOne{ProjT}(DetachedCriteria,ProjectionList)"/>
        /// </summary>
        ProjT ReportOne<ProjT>(ProjectionList projectionList, params ICriterion[] criteria);


        /// <summary>
        /// <seealso cref="ReportAll{ProjT}(DetachedCriteria,ProjectionList,Order[])"/>
        /// </summary>
        ICollection<ProjT> ReportAll<ProjT>(ProjectionList projectionList);


        /// <summary>
        /// <seealso cref="ReportAll{ProjT}(DetachedCriteria,ProjectionList,Order[])"/>
        /// </summary>
        ICollection<ProjT> ReportAll<ProjT>(DetachedCriteria criteria, ProjectionList projectionList);


        /// <summary>
        /// Create the projects of type <typeparamref name="ProjT"/> (ie
        /// DataTransferObject(s)) that satisfy the criteria supplied.
        /// </summary>
        /// <param name="criteria">The criteria to look for</param>
        /// <param name="projectionList">Maps the properties from the object 
        /// graph satisfiying <paramref name="criteria"/>  to the DTO 
        /// <typeparamref name="ProjT"/></param>
        /// <param name="orders">The fields the repository should order by</param>
        /// <returns>The projection result (DTO's) built from the object graph 
        /// satisfying <paramref name="criteria"/></returns>
        /// <remarks>
        /// The intent is for <paramref name="criteria"/> to be based (rooted)
        /// on <typeparamref name="T"/>. This is not enforced but is a
        /// convention that should be followed
        /// </remarks>
        ICollection<ProjT> ReportAll<ProjT>(DetachedCriteria criteria,
                                            ProjectionList projectionList,
                                            params Order[] orders);


        /// <summary>
        /// <seealso cref="ReportAll{ProjT}(DetachedCriteria,ProjectionList,Order[])"/>
        /// </summary>
        ICollection<ProjT> ReportAll<ProjT>(ProjectionList projectionList,
                                            params ICriterion[] criterion);


        /// <summary>
        /// <seealso cref="ReportAll{ProjT}(DetachedCriteria,ProjectionList,Order[])"/>
        /// </summary>
        ICollection<ProjT> ReportAll<ProjT>(ProjectionList projectionList,
                                            Order[] orders,
                                            params ICriterion[] criteria);


        /// <summary>
        /// <seealso cref="ReportAll{ProjT}(DetachedCriteria,ProjectionList,Order[])"/>
        /// </summary>
        ICollection<ProjT> ReportAll<ProjT>(ProjectionList projectionList, params Order[] orders);


        /// <summary>
        /// <seealso cref="ReportAll{ProjT}(DetachedCriteria,ProjectionList,Order[])"/>
        /// </summary>
        ICollection<ProjT> ReportAll<ProjT>(ProjectionList projectionList, bool distinctResults);


        /// <summary>
        /// Execute the named query and return all the resulting DTO's
        /// (projection)
        /// <seealso cref="ReportAll{ProjT}(DetachedCriteria,ProjectionList,Order[])"/>
        /// </summary>
        /// <typeparam name="ProjJ">the type returned</typeparam>
        /// <param name="namedQuery">the query to execute in the *.hbm 
        /// mapping files</param>
        /// <param name="parameters">parameters for the query</param>
        ICollection<ProjJ> ReportAll<ProjJ>(string namedQuery, params Parameter[] parameters);

		/// <summary>
		/// Creates a <see cref="DetachedCriteria"/> compatible with this Repository
		/// </summary>
		/// <returns>The <see cref="DetachedCriteria"/></returns>
    	DetachedCriteria CreateDetachedCriteria();

		/// <summary>
		/// Creates an aliase <see cref="DetachedCriteria"/> compatible with this Repository
		/// </summary>
		/// <param name="alias">the alias</param>
		/// <returns>The <see cref="DetachedCriteria"/></returns>
		DetachedCriteria CreateDetachedCriteria(string alias);

		/// <summary>
		/// Create an instance of <typeparamref name="T"/>, mapping it to the concrete class 
		/// if needed
		/// </summary>
		/// <returns></returns>
    	T Create(); 
    }
}
