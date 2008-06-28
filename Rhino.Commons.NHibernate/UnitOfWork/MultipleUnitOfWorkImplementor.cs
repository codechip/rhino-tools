using System;
using System.Collections.Generic;
using System.Data;

namespace Rhino.Commons
{
    public class MultipleUnitsOfWorkImplementor : IUnitOfWorkImplementor
    {
    	private readonly List<IUnitOfWorkImplementor> unitOfWorkImplementors = new List<IUnitOfWorkImplementor>();

        public void Add(IUnitOfWorkImplementor unitOfWorkImplementor)
        {
            unitOfWorkImplementors.Add(unitOfWorkImplementor);
        }

        public void IncrementUsages()
        {
            unitOfWorkImplementors.ForEach(delegate(IUnitOfWorkImplementor implementor) { implementor.IncrementUsages(); });
        }

        public IUnitOfWorkImplementor Previous
        {
            get 
            {  
                MultipleUnitsOfWorkImplementor previousImplementors = new MultipleUnitsOfWorkImplementor();
                unitOfWorkImplementors.ForEach(delegate(IUnitOfWorkImplementor implementor) { previousImplementors.Add(implementor.Previous); });
                return previousImplementors;
            }
        }

        public void Flush()
        {
            unitOfWorkImplementors.ForEach(delegate(IUnitOfWorkImplementor implementor) { implementor.Flush(); });
        }

        public bool IsInActiveTransaction
        {
            get 
            {
                if (unitOfWorkImplementors.Count == 0)
                    return false;
                return unitOfWorkImplementors.TrueForAll(delegate(IUnitOfWorkImplementor implementor) { return implementor.IsInActiveTransaction; }); 
            }
        }

        public RhinoTransaction BeginTransaction()
        {
            MultipleNHibernateTransaction transaction = new MultipleNHibernateTransaction();
            unitOfWorkImplementors.ForEach(delegate(IUnitOfWorkImplementor implementor) { transaction.Add(implementor.BeginTransaction()); });
            return transaction;
        }

        public RhinoTransaction BeginTransaction(IsolationLevel isolationLevel)
        {
            MultipleNHibernateTransaction transaction = new MultipleNHibernateTransaction();
            unitOfWorkImplementors.ForEach(delegate(IUnitOfWorkImplementor implementor) { transaction.Add(implementor.BeginTransaction(isolationLevel)); });
            return transaction;
        }

        public void TransactionalFlush()
        {
            TransactionalFlush(IsolationLevel.ReadCommitted);
        }

        public void TransactionalFlush(IsolationLevel isolationLevel)
        {
            unitOfWorkImplementors.ForEach(delegate(IUnitOfWorkImplementor implementor) { implementor.TransactionalFlush(isolationLevel); });
        }

        public void Dispose()
        {
            unitOfWorkImplementors.ForEach(delegate(IUnitOfWorkImplementor implementor) { implementor.Dispose(); });
        }
    }
}
