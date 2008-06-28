using System;
using System.Collections.Generic;
using System.Data;
using System.Web;
using NHibernate;
using NHibernate.Cfg;
using NHibernate.Metadata;

namespace Rhino.Commons
{
	public class MultipleNHibernateUnitOfWorkFactory : IUnitOfWorkFactory
	{
		public const string CurrentNHibernateSessionKey = "CurrentMultipleNHibernateSession.Key";
		private readonly List<NHibernateUnitOfWorkFactory> unitOfWorkFactories = new List<NHibernateUnitOfWorkFactory>();

		public void Add(NHibernateUnitOfWorkFactory factory)
		{
			unitOfWorkFactories.Add(factory);
		}

		public void Init()
		{
			unitOfWorkFactories.ForEach(delegate(NHibernateUnitOfWorkFactory factory) { factory.Init(); });
		}

		public IUnitOfWorkImplementor Create(IDbConnection maybeUserProvidedConnection, IUnitOfWorkImplementor previous)
		{
			MultipleUnitsOfWorkImplementor implementor = new MultipleUnitsOfWorkImplementor();
			unitOfWorkFactories.ForEach(delegate(NHibernateUnitOfWorkFactory factory) { implementor.Add(factory.Create(maybeUserProvidedConnection, previous)); });
			return implementor;
		}

		public ISession CurrentSession
		{
			get
			{
				ThereIsExactlyOneFactory();
				return unitOfWorkFactories[0].CurrentSession;
			}
			set
			{
				ThereIsExactlyOneFactory();
				unitOfWorkFactories[0].CurrentSession = value;
			}
		}

		public ISession GetCurrentSessionFor<TEntity>()
		{
			return GetCurrentSessionFor(typeof (TEntity));
		}

		public ISession GetCurrentSessionFor(Type typeOfEntity)
		{
			return unitOfWorkFactories.Find(delegate(NHibernateUnitOfWorkFactory factory)
				{
					return factory.CurrentSession.SessionFactory.GetAllClassMetadata().ContainsKey(typeOfEntity.FullName);
				}).GetCurrentSessionFor(typeOfEntity);
		}

		public void SetCurrentSession(Type typeOfEntity, ISession session)
		{
			unitOfWorkFactories.Find(delegate(NHibernateUnitOfWorkFactory factory)
				{
					return factory.CurrentSession.SessionFactory.GetAllClassMetadata().ContainsKey(typeOfEntity.FullName);
				}).CurrentSession = session;
		}

		public void MoveUnitOfWorkFromAspSessionIntoRequestContext(out IUnitOfWork iUoW, out Guid? LongConversationId)
		{
			iUoW = (IUnitOfWork)HttpContext.Current.Session[UnitOfWork.CurrentUnitOfWorkKey];
			LongConversationId = (Guid?)HttpContext.Current.Session[UnitOfWork.CurrentLongConversationIdKey];
			ISession[] sessions = (ISession[])HttpContext.Current.Session[CurrentNHibernateSessionKey];
			for (int i = 0; i < unitOfWorkFactories.Count - 1; i++)
			{
				unitOfWorkFactories[i].CurrentSession = sessions[i];
			}


			//avoids the temptation to access UnitOfWork from the HttpSession!
			HttpContext.Current.Session[UnitOfWork.CurrentUnitOfWorkKey] = null;
			HttpContext.Current.Session[CurrentNHibernateSessionKey] = null;
			HttpContext.Current.Session[UnitOfWork.CurrentLongConversationIdKey] = null;
		}

		public void SaveUnitOfWorkToAspSession()
		{
			HttpContext.Current.Session[UnitOfWork.CurrentUnitOfWorkKey] = UnitOfWork.Current;
			HttpContext.Current.Session[CurrentNHibernateSessionKey] = unitOfWorkFactories.ConvertAll<ISession>(FactoryToSession).ToArray();
			HttpContext.Current.Session[UnitOfWork.CurrentLongConversationIdKey] = UnitOfWork.CurrentLongConversationId;
		}

		private ISession FactoryToSession(IUnitOfWorkFactory factory)
		{
			return factory.CurrentSession;
		}

		private void ThereIsExactlyOneFactory()
		{
			if (unitOfWorkFactories.Count == 0)
				throw new InvalidOperationException("You are not in a unit of work");
			if (unitOfWorkFactories.Count > 1)
				throw new InvalidOperationException(string.Format("There are {0} unit(s) of work, pick one using GetCurrentSessionFor<TEntity>", unitOfWorkFactories.Count));
		}
	}
}
