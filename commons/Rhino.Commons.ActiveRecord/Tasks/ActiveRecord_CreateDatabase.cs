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
				IDictionary<string,string> cfg = new Dictionary<string,string>();
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
				new SchemaExport(configuration).Execute(true,true,false);
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
