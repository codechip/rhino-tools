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
using System.Diagnostics;
using System.Reflection;
using Rhino.Commons;

namespace Rhino.Commons.ForTesting
{
    /// <summary>
    /// Responsible for creating the <see cref="UnitOfWorkTestContext"/> that a test requires
    /// and ensuring this context is current for the execution of that test
    /// </summary>
    public class TestFixtureBase
    {
        public static List<UnitOfWorkTestContext> Contexts = new List<UnitOfWorkTestContext>();
        public static UnitOfWorkTestContext CurrentContext;


        /// <summary>
        /// Initialize the persistence framework, build a session factory, and
        /// initialize the container. If <paramref name="rhinoContainerConfig"/>
        /// is <see langword="null" /> or <see
        /// cref="string.Empty">string.Empty</see> a <see
        /// cref="RhinoContainer">RhinoContainer</see> will not be initialized.
        /// </summary>
        /// <param name="framework">The persistence framework</param>
        /// <param name="rhinoContainerConfig">The configuration file to
        /// initialize a 
        /// <see cref="RhinoContainer">RhinoContainer</see> or <see langword="null" />.</param>
        /// <param name="databaseName">Name of the database or <see langword="null" />.</param>
        /// <param name="databaseEngine">The database engine that tests should be performed against</param>
        /// <param name="mappingInfo">Information used to map classes to database tables and queries.</param>
        /// <remarks>
        /// If <paramref name="databaseName"/> is <see langword="null" /> or
        /// <see cref="string.Empty"/> a database with a name
        /// derived from the other parameters supplied will be created. See
        /// <see cref="DeriveDatabaseNameFrom(Assembly)"/> and <see cref="DeriveDatabaseNameFrom(DatabaseEngine, Assembly)"/>
        /// </remarks>
        public static void FixtureInitialize(PersistenceFramework framework,
                                             string rhinoContainerConfig,
                                             DatabaseEngine databaseEngine,
                                             string databaseName,
                                             MappingInfo mappingInfo)
        {
            if (string.IsNullOrEmpty(databaseName))
            {
                databaseName = DeriveDatabaseNameFrom(databaseEngine, mappingInfo.MappingAssemblies[0]);
            }

            if (GetUnitOfWorkTestContextFor(framework, rhinoContainerConfig, databaseEngine, databaseName) == null)
            {
                UnitOfWorkTestContextDbStrategy dbStrategy =
                    UnitOfWorkTestContextDbStrategy.For(databaseEngine, databaseName);
                UnitOfWorkTestContext newContext =
                    UnitOfWorkTestContext.For(framework, rhinoContainerConfig, dbStrategy, mappingInfo);
                Contexts.Add(newContext);
                Debug.Print(string.Format("Created another UnitOfWorkContext for: {0}", newContext));
            }

            UnitOfWorkTestContext context =
                GetUnitOfWorkTestContextFor(framework, rhinoContainerConfig, databaseEngine, databaseName);

            if (!Equals(context, CurrentContext) || IsInversionOfControlContainerOutOfSynchWith(context))
            {
                context.IntialiseContainerAndUowFactory();
            }

            CurrentContext = context;
            Debug.Print(string.Format("CurrentContext is: {0}", CurrentContext));
        }


        private static bool IsInversionOfControlContainerOutOfSynchWith(UnitOfWorkTestContext context) 
        {
            return (IoC.IsInitialized == false) != (context.RhinoContainer == null);
        }


        private static UnitOfWorkTestContext GetUnitOfWorkTestContextFor(PersistenceFramework framework,
                                                                         string rhinoContainerConfigPath,
                                                                         DatabaseEngine databaseEngine,
                                                                         string databaseName)
        {
            Predicate<UnitOfWorkTestContext> criteria =
                delegate(UnitOfWorkTestContext x) {
                    return x.Framework == framework &&
                           x.RhinoContainerConfigPath == StringOrEmpty(rhinoContainerConfigPath) &&
                           x.DatabaseEngine == databaseEngine &&
                           x.DatabaseName == databaseName;
                };

            return Contexts.Find(criteria);
        }


        /// <summary>
        /// See <see cref="FixtureInitialize(PersistenceFramework,string,DatabaseEngine,string,MappingInfo)"/>
        /// </summary>
        public static void FixtureInitialize(PersistenceFramework framework, string rhinoContainerConfig, MappingInfo mappingInfo)
        {
            FixtureInitialize(framework, rhinoContainerConfig, DatabaseEngine.SQLite, null, mappingInfo);
        }

        /// <summary>
        /// See <see cref="FixtureInitialize(PersistenceFramework,string,DatabaseEngine,string,MappingInfo)"/>
        /// </summary>
        public static void FixtureInitialize(PersistenceFramework framework, string rhinoContainerConfig, DatabaseEngine databaseEngine, MappingInfo mappingInfo)
        {
            FixtureInitialize(framework, rhinoContainerConfig, databaseEngine, null, mappingInfo);
        }


        /// <summary>
        /// See <see cref="FixtureInitialize(PersistenceFramework,string,DatabaseEngine,string,MappingInfo)"/>
        /// </summary>
        public static void FixtureInitialize(PersistenceFramework framework, MappingInfo mappingInfo)
        {
            FixtureInitialize(framework, null, DatabaseEngine.SQLite, null, mappingInfo);
        }


        public static string DeriveDatabaseNameFrom(DatabaseEngine databaseEngine, Assembly assembly)
        {
            if (databaseEngine == DatabaseEngine.SQLite)
                return UnitOfWorkTestContextDbStrategy.SQLiteDbName;
            else if (databaseEngine == DatabaseEngine.MsSqlCe)
                return "TempDB.sdf";
            else
                return DeriveDatabaseNameFrom(assembly);
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


        private static string StringOrEmpty(string s)
        {
            return s ?? string.Empty;
        }


        /// <summary>
        /// Throw away all <see cref="UnitOfWorkTestContext"/> objects within <see cref="Contexts"/>
        /// and referenced by <see cref="CurrentContext"/>. WARNING: Subsequent calls to  <see
        /// cref="FixtureInitialize(PersistenceFramework,string,DatabaseEngine,string,MappingInfo)"/>
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
            Contexts.Clear();
        }
    }
}