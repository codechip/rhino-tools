using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using NHibernate;

namespace Rhino.Commons
{
	public class NHibernateUnitOfWorkAdapter : IUnitOfWorkImplementor
	{
		private readonly NHibernateUnitOfWorkFactory factory;
		private ISession session;

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
		public void IncremementUsages()
		{
			usageCount += 1;
		}
	}
}
