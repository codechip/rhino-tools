using System;
using System.Data;
using NHibernate;

namespace Rhino.Commons
{
    public interface IUnitOfWork : IDisposable
    {
        void Flush();

        ITransaction BeginTransaction();
        ITransaction BeginTransaction(IsolationLevel isolationLevel);
    }
}