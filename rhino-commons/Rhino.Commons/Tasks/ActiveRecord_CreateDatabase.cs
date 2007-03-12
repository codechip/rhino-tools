using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Castle.ActiveRecord;
using Castle.ActiveRecord.Framework.Config;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using NHibernate.Cfg;
using NHibernate.Tool.hbm2ddl;
using Environment = NHibernate.Cfg.Environment;

namespace Rhino.Commons.Tasks
{
    public class ActiveRecord_CreateDatabase : Task
	{
		private ITaskItem[] assemblies;
		private string connectionString;
		private string dialect;
		private string driver;
		private bool ploralizeTableNames = true;

		public bool PloralizeTableNames
		{
			get { return ploralizeTableNames; }
			set { ploralizeTableNames = value; }
		}

		[Required]
		public ITaskItem[] Assemblies
		{
			get { return assemblies; }
			set { assemblies = value; }
		}

		[Required]
		public string ConnectionString
		{
			get { return connectionString; }
			set { connectionString = value; }
		}

		[Required]
		public string Dialect
		{
			get { return dialect; }
			set { dialect = value; }
		}

		[Required]
		public string Driver
		{
			get { return driver; }
			set { driver = value; }
		}

		public override bool Execute()
		{
			AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;
			AppDomain.CurrentDomain.TypeResolve += CurrentDomain_AssemblyResolve;
			try
			{
				InPlaceConfigurationSource src = new InPlaceConfigurationSource();
				Hashtable cfg = new Hashtable();
				cfg[Environment.Dialect] = dialect;
				cfg[Environment.ConnectionDriver] = driver;
				cfg[Environment.ConnectionString] = connectionString;
				src.PluralizeTableNames = PloralizeTableNames;

				src.Add(typeof(ActiveRecordBase), cfg);

				List<Assembly> loadedAssemblies = new List<Assembly>();
				foreach (ITaskItem assembly in assemblies)
				{
					Assembly asm = Assembly.LoadFrom(assembly.ItemSpec);
					loadedAssemblies.Add(asm);
				}

				ActiveRecordStarter.ResetInitializationFlag();
				ActiveRecordStarter.Initialize(loadedAssemblies.ToArray(), src);
				Configuration configuration = ActiveRecordMediator.GetSessionFactoryHolder().GetConfiguration(typeof(ActiveRecordBase));
				new SchemaExport(configuration).Execute(true,true,false,true);
			}
			finally
			{
				AppDomain.CurrentDomain.AssemblyResolve -= CurrentDomain_AssemblyResolve;
				AppDomain.CurrentDomain.TypeResolve -= CurrentDomain_AssemblyResolve;
			}

			return true;
		}

		static Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
		{
			foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
			{
				if (assembly.GetName().Name == args.Name)
					return assembly;
			}
			return null;
		}
	}
}
