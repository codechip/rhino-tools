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

		#region IRepository<T> Members

		public FutureValue<T> FutureGet(object id)
		{
			throw new NotImplementedException();
		}

		public FutureValue<T> FutureLoad(object id)
		{
			throw new NotImplementedException();
		}

		#endregion

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
		/// Registers all entities for deletion when the unit of work
		/// is completed.
		/// </summary>
		public virtual void DeleteAll()
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Registers all entities for deletion that match the supplied
		/// HQL condition when the unit of work is completed.
		/// </summary>
		/// <param name="where">HQL condition to select the rows to be deleted</param>
		public void DeleteAll(DetachedCriteria where)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Register te entity for save in the database when the unit of work
		/// is completed.
		/// </summary>
		/// <param name="entity">the entity to save</param>
		public T Save(T entity)
		{
			throw new NotImplementedException();
		}

	    /// <summary>
	    /// Saves or update the entity, based on its usaved-value
	    /// </summary>
	    /// <param name="entity"></param>
	    public T SaveOrUpdate(T entity)
	    {
	        throw new NotImplementedException();
	    }

        /// <summary>
        /// Saves or update a copy of the entity, based on its usaved-value
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public T SaveOrUpdateCopy(T entity)
        {
            throw new NotImplementedException();
        }

	    /// <summary>
        /// Updates the specified entity.
        /// </summary>
        /// <param name="entity">The entity.</param>
	    public void Update(T entity)
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

		/// <summary>
		/// Find the first entity of type
		/// </summary>
		/// <param name="orders">Optional orderring</param>
		/// <returns>The entity or null</returns>
		public T FindFirst(params Order[] orders)
		{
			throw new NotImplementedException();
		}

		public object ExecuteStoredProcedure(string sp_name, params Parameter[] parameters)
		{
			throw new NotImplementedException();
		}

		public ICollection<T2> ExecuteStoredProcedure<T2>(Converter<IDataReader, T2> converter, string sp_name,
		                                                  params Parameter[] parameters)
		{
			throw new NotImplementedException();
		}


		public bool Exists(DetachedCriteria criteria)
		{
			throw new Exception("The method or operation is not implemented.");
		}

		/// <summary>
		/// Check if any instance of the type exists
		/// </summary>
		/// <returns><c>true</c> if an instance is found; otherwise <c>false</c>.</returns>
		public bool Exists()
		{
			throw new NotImplementedException();
		}

		public long Count(DetachedCriteria criteria)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Counts the overall number of instances.
		/// </summary>
		/// <returns></returns>
		public long Count()
		{
			throw new NotImplementedException();
		}

        public ProjT ReportOne<ProjT>(DetachedCriteria criteria, ProjectionList projectionList)
        {
            throw new NotImplementedException();
        }


        public ProjT ReportOne<ProjT>(ProjectionList projectionList, params ICriterion[] criteria)
        {
            throw new NotImplementedException();
        }


        public ICollection<ProjT> ReportAll<ProjT>(ProjectionList projectionList)
        {
            throw new NotImplementedException();
        }


        public ICollection<ProjT> ReportAll<ProjT>(DetachedCriteria criteria, ProjectionList projectionList)
        {
            throw new NotImplementedException();
        }


        public ICollection<ProjT> ReportAll<ProjT>(ProjectionList projectionList, params ICriterion[] criteria)
        {
            throw new NotImplementedException();
        }


        public ICollection<ProjT> ReportAll<ProjT>(ProjectionList projectionList, params Order[] orders)
        {
            throw new NotImplementedException();
        }


        public ICollection<ProjT> ReportAll<ProjT>(ProjectionList projectionList, bool distinctResults)
        {
            throw new NotImplementedException();
        }


        public ICollection<ProjJ> ReportAll<ProjJ>(string namedQuery, params Parameter[] parameters)
        {
            throw new NotImplementedException();
        }


        public ICollection<ProjT> ReportAll<ProjT>(DetachedCriteria criteria, ProjectionList projectionList, params Order[] orders)
        {
            throw new NotImplementedException();
        }


        public ICollection<ProjT> ReportAll<ProjT>(ProjectionList projectionList, Order[] orders, params ICriterion[] criteria)
        {
            throw new NotImplementedException();
        }

		public DetachedCriteria CreateDetachedCriteria()
		{
			return DetachedCriteria.For<T>();
		}

		public DetachedCriteria CreateDetachedCriteria(string alias)
		{
			return DetachedCriteria.For<T>(alias);
		}

		public K Create<K>()
		{
			throw new NotImplementedException();
		}

		public T Create()
		{
			throw new NotImplementedException();
		}
	}
}
