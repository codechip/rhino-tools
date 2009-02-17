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
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Reflection;
using Castle.Windsor;

namespace Rhino.Commons.ForTesting
{
	/// <summary>
	/// Responsible for creating the <see cref="UnitOfWorkTestContext"/> that a test requires
	/// and ensuring this context is current for the execution of that test
	/// </summary>
	public class DatabaseTestFixtureBase
	{
		public static List<UnitOfWorkTestContext> Contexts = new List<UnitOfWorkTestContext>();
		public static UnitOfWorkTestContext CurrentContext;

		public static bool IsRunningInTestMode
		{
			get
			{
				return (bool)(Local.Data["DatabaseTestFixtureBase.IsRunningInTestMode"] ?? false);
			}
			private set
			{
				Local.Data["DatabaseTestFixtureBase.IsRunningInTestMode"] = value;
			}
		}

        /// <summary>
        /// Initialize the persistence framework, build a session factory, and
        /// initialize the container. If <paramref name="rhinoContainerConfig"/>
        /// is <see langword="null" /> or <see cref="string.Empty">string.Empty</see>
        ///  a <see cref="RhinoContainer">RhinoContainer</see> will not be initialized.
        /// </summary>
        /// <param name="framework">The persistence framework</param>
        /// <param name="rhinoContainerConfig">The configuration file to initialize a 
        /// <see cref="RhinoContainer">RhinoContainer</see> or <see langword="null" />.</param>
        /// <param name="databaseName">Name of the database or <see langword="null" />.</param>
        /// <param name="databaseEngine">The database engine that tests should be performed against</param>
        /// <param name="mappingInfo">Information used to map classes to database tables and queries.</param>
        /// <remarks>
        /// If <paramref name="databaseName"/> is <see langword="null" /> or
        /// <see cref="string.Empty"/> a database with a name
        /// derived from the other parameters supplied will be created. See
        /// <see cref="NHibernateInitializer.DeriveDatabaseNameFrom(Assembly)"/> and <see cref="NHibernateInitializer.DeriveDatabaseNameFrom(DatabaseEngine, Assembly)"/>
        /// </remarks>
        public static void InitializeNHibernateAndIoC(PersistenceFramework framework,
                                                      string rhinoContainerConfig,
                                                      DatabaseEngine databaseEngine,
                                                      string databaseName,
                                                      MappingInfo mappingInfo)
        {
            NHibernateInitializer.Initialize(framework, mappingInfo).Using(databaseEngine, databaseName).AndIoC(
                rhinoContainerConfig);
        }

        /// <summary>
        /// See <see cref="InitializeNHibernateAndIoC(PersistenceFramework,string,DatabaseEngine,string,MappingInfo)" />
        /// </summary>
	    public static void InitializeNHibernateAndIoC(PersistenceFramework framework,
	                                                  string rhinoContainerConfig,
	                                                  DatabaseEngine databaseEngine,
	                                                  string databaseName,
	                                                  MappingInfo mappingInfo,
	                                                  IDictionary<string, string> properties)
	    {
	        NHibernateInitializer.Initialize(framework, mappingInfo)
	            .Using(databaseEngine, databaseName)
	            .ConfiguredBy(properties)
	            .AndIoC(rhinoContainerConfig);
	    }

        /// <summary>
        /// See <see cref="InitializeNHibernateAndIoC(PersistenceFramework,string,DatabaseEngine,string,MappingInfo)"/>
        /// </summary>
        public static void InitializeNHibernateAndIoC(PersistenceFramework framework, string rhinoContainerConfig, MappingInfo mappingInfo)
        {
            NHibernateInitializer.Initialize(framework, mappingInfo).AndIoC(rhinoContainerConfig);
        }

        /// <summary>
        /// See <see cref="InitializeNHibernateAndIoC(PersistenceFramework,string,DatabaseEngine,string,MappingInfo)"/>
        /// </summary>
        public static void InitializeNHibernateAndIoC(PersistenceFramework framework, string rhinoContainerConfig, DatabaseEngine databaseEngine, MappingInfo mappingInfo)
        {
            NHibernateInitializer.Initialize(framework, mappingInfo).Using(databaseEngine, null).AndIoC(rhinoContainerConfig);
        }

	    /// <summary>
        /// See <see cref="InitializeNHibernateAndIoC(PersistenceFramework,string,DatabaseEngine,string,MappingInfo)"/>
        /// </summary>
        public static void InitializeNHibernate(PersistenceFramework framework, MappingInfo mappingInfo)
        {
            NHibernateInitializer.Initialize(framework, mappingInfo);
        }

	    /// <summary>
		/// Throw away all <see cref="UnitOfWorkTestContext"/> objects within <see cref="Contexts"/>
		/// and referenced by <see cref="CurrentContext"/>. WARNING: Subsequent calls to  <see
		/// cref="InitializeNHibernateAndIoC(PersistenceFramework,string,DatabaseEngine,string,MappingInfo)"/>
		/// and all its overloads will now take considerably longer as the persistent framework will
		/// be initialised a fresh.
		/// </summary>
		/// <remarks>
		/// This method should be used vary sparingly. It is highly unlikely that you will need to
		/// call this method between every test.
		/// <para>
		/// Calling this method will dispose of <see cref="UnitOfWorkTestContext"/> objects within
		/// <see cref="Contexts"/>. Each context maintains a reference to a 
		///  <see cref="RhinoContainer"/>. If this container object is
		/// referenced by <see cref="IoC"/>.<see cref="IoC.Container"/> then any subsequent calls to <see
		/// cref="IoC"/>.<see cref="IoC.Resolve(Type)"/> and any of the overloads will throw.
		/// </para>
		/// </remarks>
		public static void DisposeAndRemoveAllUoWTestContexts()
		{
			foreach (UnitOfWorkTestContext context in Contexts)
				context.Dispose();

			CurrentContext = null;
			IsRunningInTestMode = false;
			Contexts.Clear();
        }

	    #region Obsolete methods

        [Obsolete("Use InitializeNHibernateAndIoC instead, this method has a typo in its name")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static void IntializeNHibernateAndIoC(PersistenceFramework framework,
                                                     string rhinoContainerConfig,
                                                     DatabaseEngine databaseEngine,
                                                     string databaseName,
                                                     MappingInfo mappingInfo)
        {
            InitializeNHibernateAndIoC(framework, rhinoContainerConfig, databaseEngine, databaseName, mappingInfo);
        }

        [Obsolete("Use InitializeNHibernateAndIoC instead, this method has a typo in its name")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static void IntializeNHibernateAndIoC(PersistenceFramework framework,
                                                     string rhinoContainerConfig,
                                                     DatabaseEngine databaseEngine,
                                                     string databaseName,
                                                     MappingInfo mappingInfo,
                                                     IDictionary<string, string> properties)
        {
            InitializeNHibernateAndIoC(framework, rhinoContainerConfig, databaseEngine, databaseName, mappingInfo, properties);
        }

        [Obsolete("Use InitializeNHibernateAndIoC instead, this method has a typo in its name")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static void IntializeNHibernateAndIoC(PersistenceFramework framework, string rhinoContainerConfig, MappingInfo mappingInfo)
        {
            InitializeNHibernateAndIoC(framework, rhinoContainerConfig, mappingInfo);
        }

        [Obsolete("Use InitializeNHibernateAndIoC instead, this method has a typo in its name")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static void IntializeNHibernateAndIoC(PersistenceFramework framework, string rhinoContainerConfig, DatabaseEngine databaseEngine, MappingInfo mappingInfo)
        {
            InitializeNHibernateAndIoC(framework, rhinoContainerConfig, databaseEngine, mappingInfo);
        }

        [Obsolete("Use InitializeNHibernateAndIoC instead, this method has a typo in its name")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static void IntializeNHibernate(PersistenceFramework framework, MappingInfo mappingInfo)
        {
            InitializeNHibernate(framework, mappingInfo);
        }

        #endregion

	    /// <summary>
		/// Initializes Nhibernate and/or IoC using Fluent Builder.
		/// </summary>
		/// <param name="framework">The framework.</param>
		/// <param name="mappingInfo">The mapping info.</param>
		/// <returns></returns>
		public static NHibernateInitializer Initialize(PersistenceFramework framework, MappingInfo mappingInfo)
		{
			return new NHibernateInitializer(framework,mappingInfo);
		}

	    public class NHibernateInitializer
		{
			private readonly MappingInfo mappingInfo;
			private readonly PersistenceFramework framework;
			private DatabaseEngine databaseEngine=DatabaseEngine.SQLite;
			private string databaseName;
			private IDictionary<string, string> nhibernateConfigurationProperties = new Dictionary<string, string>();
			private readonly IoCInitializer ioc;

			protected internal NHibernateInitializer(PersistenceFramework framework, MappingInfo mappingInfo)
			{
				Guard.Against<ArgumentNullException>(mappingInfo == null, "MappingInfo is required.");
				this.framework = framework;
				this.mappingInfo = mappingInfo;
				ioc=new IoCInitializer(this);
			}

			public static NHibernateInitializer Initialize(PersistenceFramework framework, MappingInfo mappingInfo)
			{
				NHibernateInitializer initializer = new NHibernateInitializer(framework,mappingInfo);
				return initializer;
			}

			
			public PersistenceFramework PersistenceFramework
			{
				get { return framework; }
			}

			public MappingInfo MappingInfo
			{
				get { return mappingInfo; }
			}

			public DatabaseEngine DatabaseEngine
			{
				get { return databaseEngine; }
			}

			public string DatabaseName
			{
				get { return databaseName; }
			}

			public IDictionary<string, string> NHibernateConfigurationProperties
			{
				get { return nhibernateConfigurationProperties; }
			}


			public NHibernateInitializer Using(DatabaseEngine databaseEngine, string databaseName)
			{
				this.databaseEngine = databaseEngine;
				this.databaseName = databaseName;

				return this;
			}
			public NHibernateInitializer ConfiguredBy(IDictionary<string, string> nhibernateConfigurationProperties)
			{
				this.nhibernateConfigurationProperties = nhibernateConfigurationProperties;
				return this;
			}

			public static string DeriveDatabaseNameFrom(DatabaseEngine databaseEngine, Assembly assembly)
			{
				if (databaseEngine == DatabaseEngine.SQLite)
					return UnitOfWorkTestContextDbStrategy.SQLiteDbName;
				else if (databaseEngine == DatabaseEngine.MsSqlCe)
					return "TempDB.sdf";
				else // we want to have a test DB and a real db, and we really don't want to override on by mistake
					return DeriveDatabaseNameFrom(assembly) + "_Test";
			}


			private static string DeriveDatabaseNameFrom(Assembly assembly)
			{
				string[] assemblyNameParts = assembly.GetName().Name.Split('.');
				if (assemblyNameParts.Length > 1)
					//assumes that the first part of the assmebly name is the Company name
					//and the second part is the Project name
					return assemblyNameParts[1];
				else
					return assemblyNameParts[0];
			}

			private static bool IsInversionOfControlContainerOutOfSynchWith(UnitOfWorkTestContext context)
			{
				return (IoC.IsInitialized == false) != (context.RhinoContainer == null);
			}

			public void Now()
			{
				InternalComplete();
			}

			/// <summary>
			/// Initializes from the input file path
			/// </summary>
			/// <param name="rhinoContainerConfigPath">The rhino container config path.</param>
			/// <returns></returns>
			public void AndIoC(string rhinoContainerConfigPath)
			{
				ioc.With(rhinoContainerConfigPath);
				InternalComplete();
			}
			/// <summary>
			/// Initializes with the container instance
			/// </summary>
			/// <param name="container">The container.</param>
			/// <returns></returns>
			public void AndIoC(IWindsorContainer container)
			{
				ioc.With(container);
				InternalComplete();
			}
			private void InternalComplete()
			{
				if (string.IsNullOrEmpty(databaseName))
				{
					databaseName = DeriveDatabaseNameFrom(databaseEngine, mappingInfo.MappingAssemblies[0]);
				}
				UnitOfWorkTestContext context = ioc.GetUnitOfWorkTestContext();

				IsRunningInTestMode = true;

				if (!Equals(context, CurrentContext) || IsInversionOfControlContainerOutOfSynchWith(context))
				{
					context.InitializeContainerAndUowFactory();
				}
				CurrentContext = context;
				Debug.Print(string.Format("CurrentContext is: {0}", CurrentContext));
			}
		
		}

	    protected class IoCInitializer
		{
			private readonly NHibernateInitializer root;
			private string rhinoContainerConfigPath;
			private IWindsorContainer container;

			protected internal IoCInitializer(NHibernateInitializer nHibernateInitializer)
			{
				this.root = nHibernateInitializer;
			}

			protected internal NHibernateInitializer With(string rhinoContainerConfigPath)
			{
				this.rhinoContainerConfigPath = rhinoContainerConfigPath;
				return root;
			}

			protected internal NHibernateInitializer With(IWindsorContainer container)
			{
				this.container = container;
				return root;
			}
			protected internal UnitOfWorkTestContext GetUnitOfWorkTestContext()
			{
				Predicate<UnitOfWorkTestContext> criteria = null;
				if(container==null)
				{
					criteria = delegate(UnitOfWorkTestContext x)
					{
						return x.Framework == root.PersistenceFramework &&
								x.RhinoContainerConfigPath == StringOrEmpty(rhinoContainerConfigPath) &&
								x.DatabaseEngine == root.DatabaseEngine &&
								x.DatabaseName == root.DatabaseName;
					};
				}
				else
				{
					criteria = delegate(UnitOfWorkTestContext x)
					{
						return x.Framework == root.PersistenceFramework &&
								x.RhinoContainer==container &&
								  x.DatabaseEngine == root.DatabaseEngine &&
								  x.DatabaseName == root.DatabaseName;
					};
				}
					

				UnitOfWorkTestContext context= Contexts.Find(criteria);
				if(context==null)
				{
					UnitOfWorkTestContextDbStrategy dbStrategy =
						UnitOfWorkTestContextDbStrategy.For(root.DatabaseEngine, root.DatabaseName, root.NHibernateConfigurationProperties);
					if(container!=null)
					{
						context= UnitOfWorkTestContext.For(root.PersistenceFramework, container, dbStrategy, root.MappingInfo);
					}
					else
					{
						context=UnitOfWorkTestContext.For(root.PersistenceFramework, rhinoContainerConfigPath, dbStrategy, root.MappingInfo);
					}
					Contexts.Add(context);
					Debug.Print(string.Format("Created another UnitOfWorkContext for: {0}", context));
				}
				return context;
			}

			private static string StringOrEmpty(string s)
			{
				return s ?? string.Empty;
			}


		}
	}
}
