using System;
using System.Collections.Generic;
using System.Data;
using System.Web;
using NHibernate;
using NHibernate.Cfg;
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
				ThereIsExactlyOneFactory();
				return this[0].CurrentSession;
			}
			set
			{
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
			return Find(delegate(NHibernateUnitOfWorkFactory factory)
				{
					return factory.CurrentSession.SessionFactory.GetAllClassMetadata().ContainsKey(typeOfEntity.FullName);
				}).GetCurrentSessionFor(typeOfEntity);
		}

        public void SetCurrentSession<TEntity>(ISession session)
        {
            SetCurrentSession(typeof(TEntity), session);
        }

		public void SetCurrentSession(Type typeOfEntity, ISession session)
		{
			Find(delegate(NHibernateUnitOfWorkFactory factory)
				{
					return factory.CurrentSession.SessionFactory.GetAllClassMetadata().ContainsKey(typeOfEntity.FullName);
				}).CurrentSession = session;
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
