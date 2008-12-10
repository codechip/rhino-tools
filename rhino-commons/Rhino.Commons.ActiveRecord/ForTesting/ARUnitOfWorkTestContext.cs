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

using Castle.ActiveRecord;
using Castle.ActiveRecord.Framework;
using Castle.ActiveRecord.Framework.Config;
using NHibernate;
using NHibernate.Cfg;
using AREnvironment = NHibernate.Cfg.Environment;

namespace Rhino.Commons.ForTesting
{
	using System;
	using Castle.Core.Configuration;

	internal class ARUnitOfWorkTestContext : UnitOfWorkTestContext
	{
		private IConfigurationSource activeRecordConfigs;
		private ISessionFactory sessionFactory;

		public ARUnitOfWorkTestContext(
			UnitOfWorkTestContextDbStrategy dbStrategy,
			string rhinoContainerConfigPath,
			MappingInfo assemblies)
			: base(dbStrategy, rhinoContainerConfigPath, assemblies)
		{
		}


		private IConfigurationSource ActiveRecordConfigs
		{
			get { return activeRecordConfigs = activeRecordConfigs ?? CreateActiveRecordConfigs(); }
		}

		public override Configuration Configuration
		{
            get { return ActiveRecordMediator.GetSessionFactoryHolder().GetConfiguration(typeof(ActiveRecordBase)); }
		}


		public override PersistenceFramework Framework
		{
			get { return PersistenceFramework.ActiveRecord; }
		}


		public override ISessionFactory SessionFactory
		{
            get
            { 
                //TODO: This is hack for rev 1748. there should be a better way. similiar to hack on UnitOfWorkTestContext.BuildSessionFactory()
                if (Configuration.GetProperty(AREnvironment.ProxyFactoryFactoryClass) == null)
                    Configuration.Properties[AREnvironment.ProxyFactoryFactoryClass] = "NHibernate.ByteCode.Castle.ProxyFactoryFactory, NHibernate.ByteCode.Castle";

                return sessionFactory = sessionFactory ?? Configuration.BuildSessionFactory();
            }
		}


		public override void InitializeContainerAndUowFactory()
		{
			if (IoC.IsInitialized) IoC.Reset();
			ActiveRecordStarter.ResetInitializationFlag();

			if (RhinoContainer != null)
			{
				IoC.Initialize(RhinoContainer);
			}

			if (IoC.IsInitialized && IoC.Container.Kernel.HasComponent(typeof(INHibernateInitializationAware)))
			{
				foreach (INHibernateInitializationAware initializer in IoC.ResolveAll<INHibernateInitializationAware>())
					initializer.BeforeInitialization();
			}

			CallInitializationAwareConfigureIfExists();

			ActiveRecordStarter.Initialize(MappingInfo.MappingAssemblies, ActiveRecordConfigs);

			if (IoC.IsInitialized && IoC.Container.Kernel.HasComponent(typeof(INHibernateInitializationAware)))
			{
				foreach (INHibernateInitializationAware initializer in IoC.ResolveAll<INHibernateInitializationAware>())
					initializer.Initialized(Configuration, SessionFactory);
			}

			ActiveRecordMediator.GetSessionFactoryHolder().RegisterSessionFactory(SessionFactory, typeof(ActiveRecordBase));
		}

		private void CallInitializationAwareConfigureIfExists()
		{
			SessionFactoryHolderDelegate holderDelegate = null;
			holderDelegate = delegate(ISessionFactoryHolder holder)
			{
				ActiveRecordStarter.MappingRegisteredInConfiguration -= holderDelegate;
				if (IoC.IsInitialized && IoC.Container.Kernel.HasComponent(typeof(INHibernateInitializationAware)))
				{
					INHibernateInitializationAware[] initializationAware = IoC.ResolveAll<INHibernateInitializationAware>();
					if (initializationAware != null)
					{
						foreach (Configuration configuration in holder.GetAllConfigurations())
						{
							foreach (INHibernateInitializationAware initializer in initializationAware)
								initializer.Configured(configuration);
						}
					}
				}
			};
			ActiveRecordStarter.MappingRegisteredInConfiguration += holderDelegate;
		}


		private InPlaceConfigurationSource_AlwaysLazy_AndPluralized CreateActiveRecordConfigs()
		{
			InPlaceConfigurationSource_AlwaysLazy_AndPluralized cfg = new InPlaceConfigurationSource_AlwaysLazy_AndPluralized();
			cfg.Add(typeof(ActiveRecordBase), DbStrategy.NHibernateProperties);
			return cfg;
		}


		private class InPlaceConfigurationSource_AlwaysLazy_AndPluralized : InPlaceConfigurationSource
		{
			public InPlaceConfigurationSource_AlwaysLazy_AndPluralized()
			{
				SetIsLazyByDefault(true);
				SetPluralizeTableNames(true);
			}
		}
	}
}
