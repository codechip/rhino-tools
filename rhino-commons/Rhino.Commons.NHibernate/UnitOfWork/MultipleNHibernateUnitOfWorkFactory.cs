using System;
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
		    var unitOfWorkFactory = Find(delegate(NHibernateUnitOfWorkFactory factory)
		    {
		        return factory.NHibernateSessionFactory.GetAllClassMetadata().ContainsKey(typeOfEntity.FullName);
		    });
            if(unitOfWorkFactory == null)
                throw new InvalidOperationException("No session factory was registered that has "+ typeOfEntity +" registered.");
		    return unitOfWorkFactory.GetCurrentSessionFor(typeOfEntity);
		}

        public ISession GetCurrentSessionFor(string name)
        {
            var unitOfWorkFactory = Find(delegate(NHibernateUnitOfWorkFactory factory)
            {
                var settings = ((ISessionFactoryImplementor) factory.NHibernateSessionFactory).Settings;
                return settings.SessionFactoryName == name;
            });
            if (unitOfWorkFactory == null)
                throw new InvalidOperationException("No session factory was registered that has the name: " + name);
		  
            return unitOfWorkFactory.CurrentSession;
        }

        public void SetCurrentSessionFor(string name, ISession session)
        {
            var unitOfWorkFactory = Find(delegate(NHibernateUnitOfWorkFactory factory)
            {
                var settings = ((ISessionFactoryImplementor)factory.NHibernateSessionFactory).Settings;
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
		    var unitOfWorkFactory = Find(delegate(NHibernateUnitOfWorkFactory factory)
		    {
		        return factory.NHibernateSessionFactory.GetAllClassMetadata().ContainsKey(typeOfEntity.FullName);
		    });
            if (unitOfWorkFactory == null)
                throw new InvalidOperationException("No session factory was registered that has " + typeOfEntity + " registered.");
		    unitOfWorkFactory.CurrentSession = session;
		}

	    public void MoveUnitOfWorkFromAspSessionIntoRequestContext(out IUnitOfWork iUoW, out Guid? LongConversationId)
		{
			iUoW = (IUnitOfWork)HttpContext.Current.Session[UnitOfWork.CurrentUnitOfWorkKey];
			LongConversationId = (Guid?)HttpContext.Current.Session[UnitOfWork.CurrentLongConversationIdKey];
			ISession[] sessions = (ISession[])HttpContext.Current.Session[CurrentNHibernateSessionKey];
			for (int i = 0; i < Count - 1; i++)
			{
				this[i].CurrentSession = sessions[i];
			}


			//avoids the temptation to access UnitOfWork from the HttpSession!
			HttpContext.Current.Session[UnitOfWork.CurrentUnitOfWorkKey] = null;
			HttpContext.Current.Session[CurrentNHibernateSessionKey] = null;
			HttpContext.Current.Session[UnitOfWork.CurrentLongConversationIdKey] = null;
		}

		public void SaveUnitOfWorkToAspSession()
		{
			HttpContext.Current.Session[UnitOfWork.CurrentUnitOfWorkKey] = UnitOfWork.Current;
			HttpContext.Current.Session[CurrentNHibernateSessionKey] = ConvertAll<ISession>(FactoryToSession).ToArray();
			HttpContext.Current.Session[UnitOfWork.CurrentLongConversationIdKey] = UnitOfWork.CurrentLongConversationId;
		}

		private ISession FactoryToSession(IUnitOfWorkFactory factory)
		{
			return factory.CurrentSession;
		}

		private void ThereIsExactlyOneFactory()
		{
			if (Count == 0)
				throw new InvalidOperationException("You are not in a unit of work");
			if (Count > 1)
				throw new InvalidOperationException(string.Format("There are {0} unit(s) of work, pick one using GetCurrentSessionFor<TEntity>", Count));
		}
	}
}
