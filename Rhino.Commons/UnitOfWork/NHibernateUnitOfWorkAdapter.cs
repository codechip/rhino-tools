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

using System.Data;
using NHibernate;

namespace Rhino.Commons
{
	public class NHibernateUnitOfWorkAdapter : BaseUnitOfWorkFactory, IUnitOfWorkImplementor
	{
		private readonly NHibernateUnitOfWorkFactory factory;
		private readonly ISession session;

		private readonly NHibernateUnitOfWorkAdapter previous;
		private int usageCount = 1;

		IUnitOfWorkImplementor IUnitOfWorkImplementor.Previous
		{
			get { return previous; }
		}


		public NHibernateUnitOfWorkAdapter Previous
		{
			get { return previous; }
		}

		public NHibernateUnitOfWorkFactory Factory
		{
			get { return factory; }
		}

		public bool IsInActiveTransaction
		{
			get
			{
				return session.Transaction.IsActive;
			}
		}
		
		public ISession Session
		{
			get { return session; }
		}

		public void Flush()
		{
			session.Flush();
		}

		public RhinoTransaction BeginTransaction()
		{
			return new NHibernateTransactionAdapter(session.BeginTransaction());
		}

		public RhinoTransaction BeginTransaction(IsolationLevel isolationLevel)
		{
			return new NHibernateTransactionAdapter(session.BeginTransaction(isolationLevel));
		}

	    public void Dispose()
		{
			usageCount -= 1;
			if (usageCount != 0)
				return;
			factory.DisposeUnitOfWork(this);
			session.Dispose();
		}

		public NHibernateUnitOfWorkAdapter(NHibernateUnitOfWorkFactory factory, ISession session, NHibernateUnitOfWorkAdapter previous)
		{
			this.factory = factory;
			this.session = session;
			this.previous = previous;
		}

		/// <summary>
		/// Add another usage to this.
		/// Will increase the dispose count
		/// NOT THREAD SAFE
		/// </summary>
		public void IncrementUsages()
		{
			usageCount += 1;
		}
	}
}
