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
using Castle.ActiveRecord.Framework;
using NHibernate.Cfg;

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
        /// <see cref="string.Empty">string.Empty</see> a database with a name
        /// derived from the other parameters supplied will be created. See
        /// <see cref="DeriveDatabaseNameFrom"/>
        /// <para>
        /// Currently every UnitOfWorkContext created for the ActiveRecord
        /// framework within the scope of a running application must use the
        /// same database engine. An attempt to create another UnitOfWorkContext
        /// for a different database engine will throw an <see
        /// cref="InvalidOperationException"/> exception. This constraint may be
        /// able to be removed in future.
        /// </para>
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
                Contexts.Add(UnitOfWorkTestContext.For(framework, rhinoContainerConfig, dbStrategy, mappingInfo));
            }

            UnitOfWorkTestContext context =
                GetUnitOfWorkTestContextFor(framework, rhinoContainerConfig, databaseEngine, databaseName);

            if (!Equals(context, CurrentContext))
            {
                context.IntialiseContainerAndUowFactory();
            }

            CurrentContext = context;
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
    }
}