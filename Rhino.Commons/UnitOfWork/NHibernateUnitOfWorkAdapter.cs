using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using NHibernate;

namespace Rhino.Commons
{
	public class NHibernateUnitOfWorkAdapter : IUnitOfWorkImplementor
	{
		private ISession session;

		private readonly IUnitOfWorkImplementor previous;
		private int usageCount = 1;

		public IUnitOfWorkImplementor Previous
		{
			get { return previous; }
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
			UnitOfWork.DisposeUnitOfWork(this);
			session.Dispose();
		}

		public NHibernateUnitOfWorkAdapter(ISession session, IUnitOfWorkImplementor previous)
		{
			this.session = session;
			this.previous = previous;
		}

		/// <summary>
		/// Add another usage to this.
		/// Will increase the dispose count
		/// NOT THREAD SAFE
		/// </summary>
		public void IncremementUsages()
		{
			usageCount += 1;
		}
	}
}
