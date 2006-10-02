using System;
using System.IO;
using System.Xml;
using NHibernate;
using NHibernate.Cfg;
using Settings=Rhino.Commons.Properties.Settings;

namespace Rhino.Commons
{
	public static class UnitOfWork
	{
		public const string CurrentNHibernateSessionKey = "CurrentNHibernateSession.Key";
		public const string CurrentUnitOfWorkKey = "CurrentUnitOfWork.Key";

		private static ISessionFactory nhibernateSessionFactory;
        private static IUnitOfWork globalNonThreadSafeUnitOfwork;
	    
	    /// <summary>
	    /// Mostly intended to support mocking of the unit of work.
	    /// NOT thread safe!
	    /// </summary>
	    public static void RegisterGlobalUnitOfWork(IUnitOfWork global)
	    {
            globalNonThreadSafeUnitOfwork = global;
	    }
	    
		public static IUnitOfWork Start()
		{
			return Start(UnitOfWorkNestingOptions.ReturnExistingOrCreateUnitOfWork);
		}
		
		/// <summary>
		/// Start a Unit of Work
		/// is called
		/// </summary>
		/// <returns>
		/// An IUnitOfwork object that can be used to work with the current UoW.
		/// </returns>
		public static IUnitOfWork Start(UnitOfWorkNestingOptions nestingOptions)
		{
            if (globalNonThreadSafeUnitOfwork != null)
                return globalNonThreadSafeUnitOfwork;
			NHibernateUnitOfWorkAdapter data = (NHibernateUnitOfWorkAdapter)Local.Data[CurrentUnitOfWorkKey];
			if(nestingOptions == UnitOfWorkNestingOptions.ReturnExistingOrCreateUnitOfWork && 
			   data != null)
			{
				data.IncremementUsages();
				return data;
			}
		    ISession session = CreateSession();
		    session.FlushMode = FlushMode.Commit;
			CurrentNHibernateSession = session;
			data = new NHibernateUnitOfWorkAdapter(session, data);
			Local.Data[CurrentUnitOfWorkKey] = data;
			return data;
		}

	    private static ISession CreateSession()
	    {
	        if(IoC.Container.Kernel.HasComponent(typeof(IInterceptor)))
	        {
	            IInterceptor interceptor = IoC.Resolve<IInterceptor>();
                return NHibernateSessionFactory.OpenSession(interceptor);
	        }
	        return NHibernateSessionFactory.OpenSession();
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
				if (nhibernateSessionFactory == null)
				{
					//Yes, I do want to grab a lock on the type.
					lock (typeof(UnitOfWork))
					{
						if (nhibernateSessionFactory != null)
							return nhibernateSessionFactory;
						Configuration cfg = new Configuration();
						//if not this, assume loading from app.config
					    string hibernateConfig = Settings.Default.HibernateConfig;
					    //if not rooted, assume path from base directory
					    if(Path.IsPathRooted(hibernateConfig)==false)
					    {
                            hibernateConfig = Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
                                                           hibernateConfig);
					    }
					    if (File.Exists(hibernateConfig))
                            cfg.Configure(new XmlTextReader(hibernateConfig));
						nhibernateSessionFactory = cfg.BuildSessionFactory();
					}
				}
				return nhibernateSessionFactory;
			}
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
			Validation.NotNull(factory,"factory");
			ISessionFactory old = nhibernateSessionFactory;
			nhibernateSessionFactory = factory;
			if(old!=null)
			{
				old.Close();
			}
		}

		/// <summary>
		/// Called internally to clear the current UoW and move to the previous one.
		/// </summary>
		internal static void DisposeUnitOfWork(NHibernateUnitOfWorkAdapter unitOfWork)
		{
			NHibernateUnitOfWorkAdapter previous = unitOfWork.Previous;
			Local.Data[CurrentUnitOfWorkKey] = previous;
			ISession session = previous != null ? previous.Session : null;
			Local.Data[CurrentNHibernateSessionKey] = session;
		}
	}
}
