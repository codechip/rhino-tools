using System;
using System.Data;
using System.Reflection;
using Castle.ActiveRecord;
using Castle.ActiveRecord.Framework;
using Castle.ActiveRecord.Framework.Config;
using Castle.ActiveRecord.Framework.Scopes;
using NHibernate;

namespace Rhino.Commons
{
	public class ActiveRecordUnitOfWorkFactory : IUnitOfWorkFactory
	{
		private readonly Assembly[] assemblies;
		static object lockObj = new object();
		private static bool initialized = false;
		private readonly IConfigurationSource configurationSource;

		public ActiveRecordUnitOfWorkFactory(Assembly[] assemblies)
		{
			this.assemblies = assemblies;
			this.configurationSource = ActiveRecordSectionHandler.Instance;
		}

		public ActiveRecordUnitOfWorkFactory(Assembly[] assemblies, IConfigurationSource configurationSource)
		{
			this.assemblies = assemblies;
			this.configurationSource = configurationSource;
		}

		public IUnitOfWorkImplementor Create(IDbConnection maybeUserProvidedConnection, IUnitOfWorkImplementor previous)
		{
			InitializeIfNeccecary();
			ISessionScope scope;
			if (maybeUserProvidedConnection == null)
				scope = new SessionScope(FlushAction.Never);
			else
				scope = new DifferentDatabaseScope(maybeUserProvidedConnection);
			return new ActiveRecordUnitOfWorkAdapter(scope, previous);
		}

		private void InitializeIfNeccecary()
		{
			if(!initialized)
			{
				lock(lockObj)
				{
					if(!initialized)
					{
						ActiveRecordStarter.Initialize(assemblies, configurationSource);
						
						initialized = true;
					}
				}
			}
		}
	}
}
