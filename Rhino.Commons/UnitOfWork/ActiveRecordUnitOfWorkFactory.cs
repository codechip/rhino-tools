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
using System.Data;
using System.Reflection;
using Castle.ActiveRecord;
using Castle.ActiveRecord.Framework;
using Castle.ActiveRecord.Framework.Config;
using Castle.ActiveRecord.Framework.Scopes;
using NHibernate;
using NHibernate.Cfg;

namespace Rhino.Commons
{
	public class ActiveRecordUnitOfWorkFactory : IUnitOfWorkFactory
	{
		private readonly Assembly[] assemblies;
		private static object lockObj = new object();
		private static bool initialized = false;
		private readonly IConfigurationSource configurationSource;
		private INHibernateInitializationAware initializationAware;


		public INHibernateInitializationAware InitializationAware
		{
			get { return initializationAware; }
			set { initializationAware = value; }
		}

		public ActiveRecordUnitOfWorkFactory(Assembly[] assemblies)
		{
			this.assemblies = assemblies;
			this.configurationSource = ActiveRecordSectionHandler.Instance;
		}

        public ActiveRecordUnitOfWorkFactory(string[] assemblyNames)
            : this(assemblyNames, ActiveRecordSectionHandler.Instance)
        {
        }

		public ActiveRecordUnitOfWorkFactory(Assembly[] assemblies, IConfigurationSource configurationSource)
		{
			this.assemblies = assemblies;
			this.configurationSource = configurationSource;
		}

        public ActiveRecordUnitOfWorkFactory(string[] assemblyNames, IConfigurationSource configurationSource)
        {
            this.configurationSource = configurationSource;
            this.assemblies = new Assembly[assemblyNames.Length];
            for (int i = 0; i < assemblyNames.Length; i++)
                this.assemblies[i] = Assembly.Load(assemblyNames[i]);
        }

	    public IUnitOfWorkImplementor Create(IDbConnection maybeUserProvidedConnection, IUnitOfWorkImplementor previous)
		{
			InitializeIfNecessary();
			ISessionScope scope;
			if (maybeUserProvidedConnection == null)
				scope = new SessionScope(FlushAction.Never);
			else
				scope = new DifferentDatabaseScope(maybeUserProvidedConnection);
			return new ActiveRecordUnitOfWorkAdapter(scope, previous);
		}

		public void Init()
		{
			InitializeIfNecessary();
		}

		private void InitializeIfNecessary()
		{
			if (!initialized)
			{
				lock (lockObj)
				{
					if (!initialized)
					{
						ActiveRecordStarter.ResetInitializationFlag();
						ActiveRecordStarter.Initialize(assemblies, configurationSource);
						if (InitializationAware != null)
						{
							ISessionFactoryHolder holder = ActiveRecordMediator.GetSessionFactoryHolder();
							Configuration configuration = holder.GetConfiguration(typeof (ActiveRecordBase));
							ISessionFactory sessionFactory = holder.GetSessionFactory(typeof (ActiveRecordBase));
							InitializationAware.Initialized(configuration, sessionFactory);
						}
						initialized = true;
					}
				}
			}
		}

		public static ISession CurrentSession
		{
			get
			{
				ISessionScope scope = SessionScope.Current;
				if (scope == null)
					throw new InvalidOperationException("You are not in a unit of work");
				ISessionFactoryHolder holder = ActiveRecordMediator.GetSessionFactoryHolder();
				ISessionFactory sessionFactory = holder.GetSessionFactory(typeof (ActiveRecordBase));

				return holder.CreateSession(typeof (ActiveRecordBase));
			}
		}
	}
}
