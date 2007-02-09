using System;
using System.Data;
using System.IO;
using System.Xml;
using NHibernate;
using NHibernate.Cfg;
using Settings=Rhino.Commons.Properties.Settings;

namespace Rhino.Commons
{
	public class NHibernateUnitOfWorkFactory : IUnitOfWorkFactory
	{
		static object lockObj = new object();
		public const string CurrentNHibernateSessionKey = "CurrentNHibernateSession.Key";
		private static ISessionFactory sessionFactory;
		
		public void Init()
		{
			//we lazy initialize here
		}

		public IUnitOfWorkImplementor Create(IDbConnection maybeUserProvidedConnection,IUnitOfWorkImplementor previous)
		{
			ISession session = CreateSession(maybeUserProvidedConnection);
			session.FlushMode = FlushMode.Commit;
			Local.Data[CurrentNHibernateSessionKey] = session;
			return new NHibernateUnitOfWorkAdapter(this, session, (NHibernateUnitOfWorkAdapter)previous);
		}

		private static ISession CreateSession(IDbConnection maybeUserProvidedConnection)
		{
			if (IoC.Container.Kernel.HasComponent(typeof (IInterceptor)))
			{
				IInterceptor interceptor = IoC.Resolve<IInterceptor>();
				if(maybeUserProvidedConnection==null)
					return NHibernateSessionFactory.OpenSession(interceptor);
				return NHibernateSessionFactory.OpenSession(maybeUserProvidedConnection, interceptor);
			}
			if (maybeUserProvidedConnection == null)
				return NHibernateSessionFactory.OpenSession();
			return NHibernateSessionFactory.OpenSession(maybeUserProvidedConnection);
		}

		/// <summary>
		/// Replace the default implementation of the Session Factory (read from configuration)
		/// with a user supplied one.
		/// NOTE: This should be done at application start. 
		/// No attempt is made to make this thread safe!
		/// </summary>
		/// <param name="factory">
		/// </param>
		public static void RegisterSessionFactory(ISessionFactory factory)
		{
			Validation.NotNull(factory, "factory");
			ISessionFactory old = sessionFactory;
			sessionFactory = factory;
			if (old != null)
			{
				old.Close();
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
				if (sessionFactory == null)
				{
					lock (lockObj)
					{
						if (sessionFactory != null)
							return sessionFactory;
						Configuration cfg = new Configuration();
						//if not this, assume loading from app.config
						string hibernateConfig = Settings.Default.HibernateConfig;
						//if not rooted, assume path from base directory
						if (Path.IsPathRooted(hibernateConfig) == false)
						{
							hibernateConfig = Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
							                               hibernateConfig);
						}
						if (File.Exists(hibernateConfig))
							cfg.Configure(new XmlTextReader(hibernateConfig));
						sessionFactory = cfg.BuildSessionFactory();
					}
				}
				return sessionFactory;
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
				ISession session = (ISession) Local.Data[CurrentNHibernateSessionKey];
				if (session == null)
					throw new InvalidOperationException("You are not in a unit of work");
				return session;
			}
			set { Local.Data[CurrentNHibernateSessionKey] = value; }
		}

		public void DisposeUnitOfWork(NHibernateUnitOfWorkAdapter adapter)
		{
			ISession session = null;
			if(adapter.Previous!=null)
				session = adapter.Previous.Session;
			CurrentNHibernateSession = session;
			UnitOfWork.DisposeUnitOfWork(adapter);
		}
	}
}