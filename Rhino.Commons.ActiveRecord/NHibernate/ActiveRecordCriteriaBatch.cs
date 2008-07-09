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
using System.Collections;
using Castle.ActiveRecord;
using NHibernate;
using NHibernate.Criterion;


namespace Rhino.Commons
{
	public class ActiveRecordCriteriaBatch : CriteriaBatch, IActiveRecordQuery<IList>
	{
		private Type rootType = null;

		public Type RootType
		{
			get { return rootType; }
		}

		public override CriteriaBatch Add(DetachedCriteria criteria)
		{
			EnsureRootTypeIsAvailable(criteria);
			return base.Add(criteria);
		}

		public override CriteriaBatch Add(DetachedCriteria criteria,
		                                  Order order)
		{
			EnsureRootTypeIsAvailable(criteria);
			return base.Add(criteria, order);
		}

		public override IList Execute()
		{
			return (IList) ActiveRecordBase.ExecuteQuery(this);
		}

		IList IActiveRecordQuery<IList>.Execute(ISession session)
		{
			return base.Execute(session);
		}

		public new object Execute(ISession session)
		{
			return ((IActiveRecordQuery<IList>) this).Execute(session);
		}

		public IEnumerable Enumerate(ISession session)
		{
			return ((IActiveRecordQuery<IList>) this).Execute(session);
		}

		private void EnsureRootTypeIsAvailable(DetachedCriteria criteria)
		{
			if (rootType == null)
			{
				rootType = criteria.GetRootEntityTypeIfAvailable();
			}
		}
	}
}
