#region license
// Copyright (c) 2005 - 2007 Ayende Rahien (ayende@ayende.com)
// All rights reserved.
// 
// Redistribution and use in source and binary forms, with or without modification,
// are permitted provided that the following conditions are met:
// 
//     * Redistributions of source code must retain the above copyright notice,
//     this list of conditions and the following disclaimer.
//     * Redistributions in binary form must reproduce the above copyright notice,
//     this list of conditions and the following disclaimer in the documentation
//     and/or other materials provided with the distribution.
//     * Neither the name of Ayende Rahien nor the names of its
//     contributors may be used to endorse or promote products derived from this
//     software without specific prior written permission.
// 
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND
// ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
// WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
// DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS BE LIABLE
// FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL
// DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR
// SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER
// CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY,
// OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF
// THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
#endregion

using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Reflection;
using System.Web;
using System.Xml;
using Iesi.Collections.Generic;
using NHibernate;
using NHibernate.Cfg;
using NHibernate.Mapping;
using Settings = Rhino.Commons.Properties.Settings;

namespace Rhino.Commons
{
	public class NHibernateUnitOfWorkFactory : IUnitOfWorkFactory
	{
		static readonly object lockObj = new object();
		public readonly string CurrentNHibernateSessionKey = Guid.NewGuid().ToString();
		private ISessionFactory sessionFactory;
		private Configuration cfg;
		private INHibernateInitializationAware[] initializationAware;
		private readonly string configurationFileName;
	    private Assembly[] assemblies;

	    public NHibernateUnitOfWorkFactory()
		{
		}

		public NHibernateUnitOfWorkFactory(string configurationFileName)
		{
			this.configurationFileName = configurationFileName;
		}

		public NHibernateUnitOfWorkFactory(Assembly[] assemblies)
		{
			this.assemblies = assemblies;
		}


	    public NHibernateUnitOfWorkFactory(Assembly[] assemblies, string configurationFileName) : this(configurationFileName)
	    {
	        this.assemblies = assemblies;
	    }

	    public INHibernateInitializationAware[] InitializationAware
		{
			get { return initializationAware; }
			set { initializationAware = value; }
		}

		public void Init()
		{
			if (InitializationAware == null)
			{
				if (IoC.Container.Kernel.HasComponent(typeof(INHibernateInitializationAware)))
					InitializationAware = IoC.ResolveAll<INHibernateInitializationAware>();
			}

			if (InitializationAware != null && cfg != null)
			{
				foreach (INHibernateInitializationAware initializer in InitializationAware)
					initializer.Initialized(cfg, NHibernateSessionFactory);
			}
		}

		public IUnitOfWorkImplementor Create(IDbConnection maybeUserProvidedConnection, IUnitOfWorkImplementor previous)
		{
			ISession session = CreateSession(maybeUserProvidedConnection);
			session.FlushMode = FlushMode.Commit;
			Local.Data[CurrentNHibernateSessionKey] = session;
			return new NHibernateUnitOfWorkAdapter(this, session, (NHibernateUnitOfWorkAdapter)previous);
		}

		private ISession CreateSession(IDbConnection maybeUserProvidedConnection)
		{
			if (IoC.Container.Kernel.HasComponent(typeof(IInterceptor)))
			{
				IInterceptor interceptor = IoC.Resolve<IInterceptor>();
				if (maybeUserProvidedConnection == null)
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
		public void RegisterSessionFactory(ISessionFactory factory)
		{
			Guard.Against<ArgumentNullException>(factory == null, "factory");
			RegisterSessionFactory(null, factory);
		}

		public void RegisterSessionFactory(Configuration configuration, ISessionFactory factory)
		{
			Guard.Against<ArgumentNullException>(factory == null, "factory");
			ISessionFactory old = sessionFactory;
			cfg = configuration;
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
		public ISessionFactory NHibernateSessionFactory
		{
			get
			{
				if (sessionFactory == null)
				{
					lock (lockObj)
					{
						if (sessionFactory != null)
							return sessionFactory;
						cfg = new Configuration();
						//if not this, assume loading from app.config
						string hibernateConfig = configurationFileName ?? Settings.Default.HibernateConfig;
						//if not rooted, assume path from base directory
						if (Path.IsPathRooted(hibernateConfig) == false)
						{
							hibernateConfig = Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
														   hibernateConfig);
						}

						InitializationAware = IoC.ResolveAll<INHibernateInitializationAware>();

						foreach (INHibernateInitializationAware initializer in InitializationAware)
							initializer.BeforeInitialization();

						if (File.Exists(hibernateConfig))
							cfg.Configure(new XmlTextReader(hibernateConfig));

					    if (assemblies != null && assemblies.Length > 0)
					    {
					        ISet<Assembly> loadedAssemblies = new HashedSet<Assembly>();
					        
                            foreach (PersistentClass mapping in cfg.ClassMappings)
					        {
					            loadedAssemblies.Add(mapping.MappedClass.Assembly);
					        }
					        foreach (Assembly assembly in assemblies)
					        {
					            if(loadedAssemblies.Contains(assembly) == false)
					                cfg.AddAssembly(assembly);
					        }
					    }

						foreach (INHibernateInitializationAware initializer in InitializationAware)
							initializer.Configured(cfg);

					    sessionFactory = cfg.BuildSessionFactory();

						foreach (INHibernateInitializationAware initializer in InitializationAware)
							initializer.Initialized(cfg, sessionFactory);
						
					}
				}
				return sessionFactory;
			}
		}


		/// <summary>
		/// The current NHibernate session.
		/// Note that the flush mode is CommitOnly!
		/// It is provided to support complex scenarios only.
		/// </summary>
		public ISession CurrentSession
		{
			get
			{
				ISession session = (ISession)Local.Data[CurrentNHibernateSessionKey];
				if (session == null)
					throw new InvalidOperationException("You are not in a unit of work");
				return session;
			}
			set { Local.Data[CurrentNHibernateSessionKey] = value; }
		}

		public void DisposeUnitOfWork(NHibernateUnitOfWorkAdapter adapter)
		{
			ISession session = null;
			if (adapter.Previous != null)
				session = adapter.Previous.Session;
			CurrentSession = session;
			UnitOfWork.DisposeUnitOfWork(adapter);
		}


		public void LoadUnitOfWorkFromHashtable(Hashtable hashtable, out IUnitOfWork iUoW, out Guid? LongConversationId)
		{
			iUoW = (IUnitOfWork)hashtable[UnitOfWork.CurrentUnitOfWorkKey];
			LongConversationId = (Guid?)hashtable[UnitOfWork.CurrentLongConversationIdKey];

			UnitOfWork.CurrentSession = (ISession)hashtable[CurrentNHibernateSessionKey];
		}

		public virtual void SaveUnitOfWorkToHashtable(Hashtable hashtable)
		{
			hashtable[UnitOfWork.CurrentUnitOfWorkKey] = UnitOfWork.Current;
			hashtable[CurrentNHibernateSessionKey] = UnitOfWork.CurrentSession;
			hashtable[UnitOfWork.CurrentLongConversationIdKey] = UnitOfWork.CurrentLongConversationId;
		}

        public ISession GetCurrentSessionFor(string name)
        {
            return CurrentSession;
        }

	    public IDisposable SetCurrentSessionName(string name)
	    {
            return new DisposableAction(delegate { });
	    }

	    public ISession GetCurrentSessionFor(Type typeOfEntity)
		{
			return CurrentSession;
		}

		public void SetCurrentSession(Type typeOfEntity, ISession session)
		{
			CurrentSession = session;
		}

	    public void Dispose()
	    {
            NHibernateSessionFactory.Dispose();
	    }
	}
}
