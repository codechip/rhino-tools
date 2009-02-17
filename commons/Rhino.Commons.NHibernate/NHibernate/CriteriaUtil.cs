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
using NHibernate;
using NHibernate.Criterion;
using NHibernate.Engine;
using NHibernate.Impl;
using NHibernate.Persister.Entity;

namespace Rhino.Commons.NHibernate
{
	public static class CriteriaUtil
	{
		public static ISessionImplementor GetSession(ICriteria criteria)
		{
			return GetRootCriteria(criteria).Session;
		}

		private static CriteriaImpl GetRootCriteria(ICriteria criteria)
		{
			CriteriaImpl impl = criteria as CriteriaImpl;
			if (impl != null)
				return impl;
			return GetRootCriteria(((CriteriaImpl.Subcriteria)criteria).Parent);
		}

		public static Type GetRootType(ICriteria criteria)
		{
			Type rootType = criteria.GetRootEntityTypeIfAvailable();
			if (rootType != null)
			{
				return rootType;
			}

			CriteriaImpl impl = GetRootCriteria(criteria);
			if(impl.Session==null)
				throw new InvalidOperationException("Could not get root type on criteria that is not attached to a session");

			ISessionFactoryImplementor factory = impl.Session.Factory;
			IEntityPersister persister = factory.GetEntityPersister(impl.EntityOrClassName);
			if (persister == null)
				throw new InvalidOperationException("Could not find entity named: " + impl.EntityOrClassName);

			return persister.GetMappedClass(EntityMode.Poco);
		}

		public static Type GetRootType(DetachedCriteria criteria, ISession session)
		{
			Type rootType = criteria.GetRootEntityTypeIfAvailable();
			if (rootType != null)
			{
				return rootType;
			}

			ISessionFactoryImplementor factory = (ISessionFactoryImplementor)session.SessionFactory;
			IEntityPersister persister = factory.GetEntityPersister(criteria.EntityOrClassName);
			if (persister == null)
				throw new InvalidOperationException("Could not find entity named: " + criteria.EntityOrClassName);

			return persister.GetMappedClass(EntityMode.Poco);
		}
	}
}