using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Web;
using NHibernate;
using NHibernate.Cfg;
using NHibernate.Engine;
using NHibernate.Metadata;

namespace Rhino.Commons
{
	public class MultipleNHibernateUnitOfWorkFactory : List<NHibernateUnitOfWorkFactory>, IUnitOfWorkFactory
	{
        public const string USER_PROVIDED_CONNECTION_EXCEPTION_MESSAGE = "MultipleNHibernateUnitOfWorkFactory does not support user supplied connections because it cannot associate the connection to a factory.";
		public const string CurrentNHibernateSessionKey = "CurrentMultipleNHibernateSession.Key";

		public void Init()
		{
			ForEach(delegate(NHibernateUnitOfWorkFactory factory) { factory.Init(); });
		}

		public IUnitOfWorkImplementor Create(IDbConnection maybeUserProvidedConnection, IUnitOfWorkImplementor previous)
		{
            Guard.Against<NotSupportedException>(maybeUserProvidedConnection != null, USER_PROVIDED_CONNECTION_EXCEPTION_MESSAGE);

            MultipleUnitsOfWorkImplementor previousImplementors = previous as MultipleUnitsOfWorkImplementor;
            MultipleUnitsOfWorkImplementor currentImplementor = new MultipleUnitsOfWorkImplementor();
            for (int i = 0; i < Count; i++)
            {
                IUnitOfWorkImplementor previousImplementor = null;
                if(previousImplementors != null)
                    previousImplementor = previousImplementors[i];

                currentImplementor.Add(this[i].Create(null, previousImplementor));
            }
            return currentImplementor;
		}

		public ISession CurrentSession
		{
			get
			{
                if (CurrentName != null)
                    return GetCurrentSessionFor(CurrentName);
				ThereIsExactlyOneFactory();
				return this[0].CurrentSession;
			}
			set
			{
                if (CurrentName != null)
                {
                    SetCurrentSessionFor(CurrentName, value);
                    return;
                }
				ThereIsExactlyOneFactory();
                this[0].CurrentSession = value;
			}
		}

		public ISession GetCurrentSessionFor<TEntity>()
		{
			return GetCurrentSessionFor(typeof (TEntity));
		}

		public ISession GetCurrentSessionFor(Type typeOfEntity)
		{
		    NHibernateUnitOfWorkFactory unitOfWorkFactory = Find(delegate(NHibernateUnitOfWorkFactory factory)
		    {
		        return factory.NHibernateSessionFactory.GetAllClassMetadata().ContainsKey(typeOfEntity.FullName);
		    });
            if(unitOfWorkFactory == null)
                throw new InvalidOperationException("No session factory was registered that has "+ typeOfEntity +" registered.");
		    return unitOfWorkFactory.GetCurrentSessionFor(typeOfEntity);
		}

        public ISession GetCurrentSessionFor(string name)
        {
            NHibernateUnitOfWorkFactory unitOfWorkFactory = Find(delegate(NHibernateUnitOfWorkFactory factory)
            {
                Settings settings = ((ISessionFactoryImplementor)factory.NHibernateSessionFactory).Settings;
                return settings.SessionFactoryName == name;
            });
            if (unitOfWorkFactory == null)
                throw new InvalidOperationException("No session factory was registered that has the name: " + name);
		  
            return unitOfWorkFactory.CurrentSession;
        }

        public void SetCurrentSessionFor(string name, ISession session)
        {
            NHibernateUnitOfWorkFactory unitOfWorkFactory = Find(delegate(NHibernateUnitOfWorkFactory factory)
            {
                Settings settings = ((ISessionFactoryImplementor)factory.NHibernateSessionFactory).Settings;
                return settings.SessionFactoryName == name;
            });
            if (unitOfWorkFactory == null)
                throw new InvalidOperationException("No session factory was registered that has the name: " + name);

            unitOfWorkFactory.CurrentSession = session;
        }

	    public IDisposable SetCurrentSessionName(string name)
	    {
	        CurrentName = name;
	        return new DisposableAction(delegate { CurrentName = null; });
	    }

	    private string CurrentName
	    {
            get { return (string) Local.Data[this]; }
	        set { Local.Data[this] = value; }
	    }

	    public void SetCurrentSession<TEntity>(ISession session)
        {
            SetCurrentSession(typeof(TEntity), session);
        }

		public void SetCurrentSession(Type typeOfEntity, ISession session)
		{
		    NHibernateUnitOfWorkFactory unitOfWorkFactory = Find(delegate(NHibernateUnitOfWorkFactory factory)
		    {
		        return factory.NHibernateSessionFactory.GetAllClassMetadata().ContainsKey(typeOfEntity.FullName);
		    });
            if (unitOfWorkFactory == null)
                throw new InvalidOperationException("No session factory was registered that has " + typeOfEntity + " registered.");
		    unitOfWorkFactory.CurrentSession = session;
		}

	    public void LoadUnitOfWorkFromHashtable(Hashtable hashtable, out IUnitOfWork iUoW, out Guid? LongConversationId)
		{
			iUoW = (IUnitOfWork)hashtable[UnitOfWork.CurrentUnitOfWorkKey];
			LongConversationId = (Guid?)hashtable[UnitOfWork.CurrentLongConversationIdKey];
			ISession[] sessions = (ISession[])hashtable[CurrentNHibernateSessionKey];
			for (int i = 0; i < Count - 1; i++)
			{
				this[i].CurrentSession = sessions[i];
			}
		}

		public void SaveUnitOfWorkToHashtable(Hashtable hashtable)
		{
			hashtable[UnitOfWork.CurrentUnitOfWorkKey] = UnitOfWork.Current;
			hashtable[CurrentNHibernateSessionKey] = ConvertAll<ISession>(FactoryToSession).ToArray();
			hashtable[UnitOfWork.CurrentLongConversationIdKey] = UnitOfWork.CurrentLongConversationId;
		}

		private ISession FactoryToSession(IUnitOfWorkFactory factory)
		{
			return factory.CurrentSession;
		}

		private void ThereIsExactlyOneFactory()
		{
			if (Count != 1)
				throw new InvalidOperationException(string.Format("There are {0} unit(s) of work, pick one using GetCurrentSessionFor<TEntity>", Count));
		}

	    public void Dispose()
	    {
            foreach (NHibernateUnitOfWorkFactory factory in this)
            {
                factory.Dispose();
            }
	    }
	}
}
