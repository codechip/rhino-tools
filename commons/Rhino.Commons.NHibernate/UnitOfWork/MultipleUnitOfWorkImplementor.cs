using System;
using System.Collections.Generic;
using System.Data;

namespace Rhino.Commons
{
    public class MultipleUnitsOfWorkImplementor : List<IUnitOfWorkImplementor>, IUnitOfWorkImplementor
    {
        public IUnitOfWorkImplementor Previous
        {
            get 
            {  
                MultipleUnitsOfWorkImplementor previousImplementors = new MultipleUnitsOfWorkImplementor();
                ForEach(delegate(IUnitOfWorkImplementor implementor) { previousImplementors.Add(implementor.Previous); });
                return previousImplementors;
            }
        }

        public void IncrementUsages()
        {
            ForEach(delegate(IUnitOfWorkImplementor implementor) { implementor.IncrementUsages(); });
        }

        public void Flush()
        {
            ForEach(delegate(IUnitOfWorkImplementor implementor) { implementor.Flush(); });
        }

        public bool IsInActiveTransaction
        {
            get 
            {
                if (Count == 0)
                    return false;
                return TrueForAll(delegate(IUnitOfWorkImplementor implementor) { return implementor.IsInActiveTransaction; }); 
            }
        }

        public RhinoTransaction BeginTransaction()
        {
            MultipleNHibernateTransaction transaction = new MultipleNHibernateTransaction();
            ForEach(delegate(IUnitOfWorkImplementor implementor) { transaction.Add(implementor.BeginTransaction()); });
            return transaction;
        }

        public RhinoTransaction BeginTransaction(IsolationLevel isolationLevel)
        {
            MultipleNHibernateTransaction transaction = new MultipleNHibernateTransaction();
            ForEach(delegate(IUnitOfWorkImplementor implementor) { transaction.Add(implementor.BeginTransaction(isolationLevel)); });
            return transaction;
        }

        public void TransactionalFlush()
        {
            ForEach(delegate(IUnitOfWorkImplementor implementor) { implementor.TransactionalFlush(); });
        }

        public void TransactionalFlush(IsolationLevel isolationLevel)
        {
            ForEach(delegate(IUnitOfWorkImplementor implementor) { implementor.TransactionalFlush(isolationLevel); });
        }

        public void Dispose()
        {
            ForEach(delegate(IUnitOfWorkImplementor implementor) { implementor.Dispose(); });
            if (Count > 0 && TrueForAll(delegate(IUnitOfWorkImplementor implementor) { return implementor.Previous != null; }))
                UnitOfWork.DisposeUnitOfWork(this);
        }
    }
}
