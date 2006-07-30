using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using NHibernate;
using NHibernate.Cfg;

namespace Rhino.Commons
{
    public static class UnitOfWork
    {
        public const string CurrentNHibernateSessionKey = "CurrentNHibernateSession.Key";
        public const string CurrentUnitOfWorkKey = "CurrentUnitOfWork.Key";
        
        private static ISessionFactory nhibernateSessionFactory;
        
        /// <summary>
        /// Start a Unit of Work, or return the current one.
        /// </summary>
        /// <returns>
        /// An IUnitOfwork object that can be used to work with the current UoW.
        /// </returns>
        public static IUnitOfWork Start()
        {
            IUnitOfWork data = (IUnitOfWork)Local.Data[CurrentUnitOfWorkKey];
            if(data==null)
            {
                ISession session = NHibernateSessionFactory.OpenSession();
                session.FlushMode = FlushMode.Commit;
                CurrentNHibernateSession = session;
                data = new NHibernateUnitOfWorkAdapter(session);
                Local.Data[CurrentUnitOfWorkKey] = data;
            }
            return data;
        }

        /// <summary>
        /// The current unit of work.
        /// </summary>
        public static IUnitOfWork Current
        {
            get
            {
                IUnitOfWork unitOfWork = (IUnitOfWork)Local.Data[CurrentUnitOfWorkKey];
                if (unitOfWork == null)
                    throw new InvalidOperationException("You are not in a unit of work");
                return unitOfWork;
            }
        }
        
        /// <summary>
        /// The current NHibernate session.
        /// Note that the flush mode is CommitOnly!
        /// Note: Prefer to avoid using this member.
        /// It is provided to support complex scenarios only.
        /// </summary>
        public static ISession CurrentNHibernateSession
        {
            get
            {
                ISession session = (ISession)Local.Data[CurrentNHibernateSessionKey];
                if (session == null)
                    throw new InvalidOperationException("You are not in a unit of work");
                return session;
            }
            set
            {
                Local.Data[CurrentNHibernateSessionKey] = value;
            }
        }
        
        /// <summary>
        /// The session factory for the application.
        /// Note: Prefer to avoid using the member.
        /// It is provided to support complex scenarios only, and it is possible
        /// to configure its behavior externally via configuration.
        /// </summary>
        public static ISessionFactory NHibernateSessionFactory
        {
            get
            {
                if (nhibernateSessionFactory==null)
                {
                    //Yes, I do want to grab a lock on the type.
                    lock(typeof(UnitOfWork))
                    {
                        if (nhibernateSessionFactory != null)
                            return nhibernateSessionFactory;
                        Configuration cfg = new Configuration();
                        nhibernateSessionFactory =cfg.BuildSessionFactory();
                    }
                }
                return nhibernateSessionFactory;
            }
        }
    }
}
