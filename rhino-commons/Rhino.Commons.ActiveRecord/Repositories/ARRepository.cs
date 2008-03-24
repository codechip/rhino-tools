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
using Castle.ActiveRecord;
using NHibernate;
using NHibernate.Criterion;

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
			return (T)ActiveRecordMediator.FindByPrimaryKey(ConcreteType,id, false);
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
			return (T)ActiveRecordMediator.FindByPrimaryKey(ConcreteType,id, true);
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
			ActiveRecordMediator.DeleteAll(ConcreteType);
		}

        /// <summary>
        /// Registers all entities for deletion that match the supplied
        /// criteria condition when the unit of work is completed.
        /// </summary>
        /// <param name="where">criteria condition to select the rows to be deleted</param>
        public void DeleteAll(DetachedCriteria where)
		{
            foreach (T entity in ActiveRecordMediator.FindAll(ConcreteType, where))
            {
                ActiveRecordMediator.Delete(entity);
            }
		}

		/// <summary>
		/// Register te entity for save in the database when the unit of work
		/// is completed.
		/// </summary>
		/// <param name="entity">the entity to save</param>
		/// <returns>The saved entity</returns>
		public virtual T Save(T entity)
		{
			ActiveRecordMediator.Create(entity);
			return entity;
		}

        /// <summary>
        /// Saves or update the entity, based on its usaved-value
        /// </summary>
        /// <param name="entity"></param>
        /// <returns>The saved or updated entity</returns>
	    public T SaveOrUpdate(T entity)
	    {
	        ActiveRecordMediator.Save(entity);
        	return entity;
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


        private void ReleaseSession(ISession session)
		{
			ActiveRecordMediator.GetSessionFactoryHolder().ReleaseSession(session);
		}

		private ISession OpenSession()
		{
			return ActiveRecordMediator.GetSessionFactoryHolder().CreateSession(ConcreteType);
		}


        protected override ISessionFactory SessionFactory
        {
            get { return ActiveRecordMediator.GetSessionFactoryHolder().GetSessionFactory(ConcreteType); }
        }


        protected override DisposableAction<ISession> ActionToBePerformedOnSessionUsedForDbFetches
        {
            //create a new session that is disposed of at the end of a database fetch
            get { return new DisposableAction<ISession>(delegate(ISession s) {
                                                            ReleaseSession(s);
                                                        }, OpenSession()); }
        }
	}
}
