using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using NHibernate;

namespace Rhino.Commons
{
    public class NHibernateUnitOfWorkAdapter : IUnitOfWork
    {
        private ISession session;

        public ISession Session
        {
            get { return session; }
        }

        public void Flush()
        {
            session.Flush();
        }

        public ITransaction BeginTransaction()
        {
            return session.BeginTransaction();
        }

        public ITransaction BeginTransaction(IsolationLevel isolationLevel)
        {
            return session.BeginTransaction(isolationLevel);
        }

        public void Dispose()
        {
			UnitOfWork.ClearCurrentUnitOfWork();
			session.Dispose();
        }

        public NHibernateUnitOfWorkAdapter(ISession session)
        {
            this.session = session;
        }
    }
}
