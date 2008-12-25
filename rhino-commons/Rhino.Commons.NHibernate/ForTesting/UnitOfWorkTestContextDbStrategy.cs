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
using System.Data;
using System.Data.SqlClient;
using Microsoft.Win32;
using NHibernate;
using NHibernate.Engine;
using NHibernate.Tool.hbm2ddl;
using Rhino.Commons.Helpers;
using Environment=NHibernate.Cfg.Environment;

namespace Rhino.Commons.ForTesting
{
    /// <summary>
    /// A strategy class that parameterizes a <see
    /// cref="UnitOfWorkTestContext"/> with database specific implementations
    /// </summary>
    /// <remarks>
    /// This class is a companion to <see cref="UnitOfWorkTestContext"/>. Its
    /// purpose is to encapsulate behind a common interface the database
    /// specific implementations of behaviour required to construct and manage
    /// the test context
    /// </remarks>
    public abstract class UnitOfWorkTestContextDbStrategy
    {
        public static string SQLiteDbName = ":memory:";

        /// <summary>
        /// Creates the physical database named <paramref name="databaseName"/>.
        /// </summary>
        /// <remarks>
        /// Use this method to create the physical database file. 
        /// <para>
        /// For MsSqlCe this will create a database file in the file system
        /// named <paramref name="databaseName"/>.sdf
        /// </para>
        /// <para>
        /// For MsSql2005 this will create a database named <paramref
        /// name="databaseName"/> in the (local) instance of Sql Server 2005 on
        /// this machine
        /// </para>
        /// </remarks>
        public static void CreatePhysicalDatabaseMediaFor(DatabaseEngine databaseEngine, string databaseName)
        {
            For(databaseEngine, databaseName, null).CreateDatabaseMedia();
        }

        public static UnitOfWorkTestContextDbStrategy For(DatabaseEngine databaseEngine, string databaseName) 
        {
            return For(databaseEngine, databaseName, null);
        }

        public static UnitOfWorkTestContextDbStrategy For(DatabaseEngine databaseEngine, string databaseName, IDictionary<string, string> properties)
        {
            UnitOfWorkTestContextDbStrategy strategy;
            switch (databaseEngine)
            {
                case DatabaseEngine.SQLite:
                    strategy = new SQlLiteUnitOfWorkTestContextDbStrategy();
                    break;
                case DatabaseEngine.MsSqlCe:
                    strategy = new MsSqlCeUnitOfWorkTestContextDbStrategy(databaseName);
                    break;
                case DatabaseEngine.MsSql2005:
                    strategy = new MsSql2005UnitOfWorkTestContextDbStrategy(databaseName);
                    break;
                case DatabaseEngine.MsSql2005Express:
                    strategy = new MsSql2005ExpressUnitOfWorkTestContextDbStrategy(databaseName);
                    break;
                default:
                    throw new ArgumentOutOfRangeException("databaseEngine");
            }
            if (properties != null)
                foreach (KeyValuePair<string,string> property in properties)
                    strategy.NHibernateProperties[property.Key] = property.Value;
            return strategy;
        }

        public static bool IsSqlServer2005OrAboveInstalled()
        {
            string sqlServerCurrentVsRegKey = @"SOFTWARE\Microsoft\MSSQLServer\MSSQLServer\CurrentVersion";
            RegistryKey regKey = Registry.LocalMachine.OpenSubKey(sqlServerCurrentVsRegKey);
            if (regKey == null)
                return false;

            string currentVersion = (string)regKey.GetValue("CurrentVersion");
            string[] versionNumbers = currentVersion.Split('.');

            return Int32.Parse(versionNumbers[0]) >= 9;
        }


        private UnitOfWorkTestContext testContext;
        private readonly string databaseName;


        public UnitOfWorkTestContextDbStrategy(string databaseName)
        {
            this.databaseName = databaseName;
        }


        public abstract DatabaseEngine DatabaseEngine { get; }


        public string DatabaseName
        {
            get { return databaseName; }
        }

        private IDictionary<string,string> properties = new Dictionary<string,string>();

        public IDictionary<string,string> NHibernateProperties { get { return properties; } }


        public UnitOfWorkTestContext TestContext
        {
            get { return testContext; }
            set { testContext = value; }
        }


        public virtual ISession CreateSession()
        {
            ISession session = TestContext.SessionFactory.OpenSession();
            return session;
        }


        public virtual void SetupDatabase(ISession currentSession)
        {
            CreateDatabaseMedia();
            CreateDatabaseSchema(currentSession);
        }


        protected abstract void CreateDatabaseMedia();


        protected virtual void CreateDatabaseSchema(ISession currentSession)
        {
            new SchemaExport(TestContext.Configuration).Execute(false, true, false, true);
        }



        private class MsSqlCeUnitOfWorkTestContextDbStrategy : UnitOfWorkTestContextDbStrategy
        {
            public MsSqlCeUnitOfWorkTestContextDbStrategy(string databaseName) : base(databaseName)
            {
                properties.Add(Environment.ConnectionDriver, "NHibernate.Driver.SqlServerCeDriver");
                properties.Add(Environment.Dialect, "NHibernate.Dialect.MsSqlCeDialect");
                properties.Add(Environment.ConnectionProvider,
                               "NHibernate.Connection.DriverConnectionProvider");
                string connectionString = string.Format("Data Source={0};", DatabaseName);
                properties.Add(Environment.ConnectionString, connectionString);
                properties.Add(Environment.ShowSql, "true");
                properties.Add(Environment.ReleaseConnections, "on_close");
            }


            public override DatabaseEngine DatabaseEngine
            {
                get { return DatabaseEngine.MsSqlCe; }
            }


            protected override void CreateDatabaseMedia() 
            {
                string filename;
                if (!databaseName.EndsWith(".sdf"))
                    filename = string.Format("{0}.sdf", databaseName);
                else
                    filename = databaseName;

                SqlCEDbHelper.CreateDatabaseFile(filename);
            }
        }

        private class MsSql2005UnitOfWorkTestContextDbStrategy : UnitOfWorkTestContextDbStrategy
        {
            public MsSql2005UnitOfWorkTestContextDbStrategy(string databaseName) : base(databaseName)
            {
                properties.Add(Environment.ConnectionDriver, "NHibernate.Driver.SqlClientDriver");
                properties.Add(Environment.Dialect, "NHibernate.Dialect.MsSql2005Dialect");
                properties.Add(Environment.ConnectionProvider,
                               "NHibernate.Connection.DriverConnectionProvider");
                properties.Add(Environment.ConnectionString, ConnectionStringFor(DatabaseName));
                properties.Add(Environment.ShowSql, "true");
                properties.Add(Environment.ReleaseConnections, "on_close");
                //by specifying a default schema, nhibernate's dynamic sql queries benefit from caching
                properties.Add(Environment.DefaultSchema, string.Format("{0}.dbo", DatabaseName));
            }


            public override DatabaseEngine DatabaseEngine
            {
                get { return DatabaseEngine.MsSql2005; }
            }


            protected override void CreateDatabaseMedia()
            {
                string sqlServerDataDirectory = GetSqlServerDataDirectory();
                string createDatabaseScript = "IF (SELECT DB_ID('" + DatabaseName + "')) IS NULL  "
                                              + " CREATE DATABASE " + DatabaseName
                                              + " ON PRIMARY "
                                              + " (NAME = " + DatabaseName + "_Data, "
                                              + @" FILENAME = '" + sqlServerDataDirectory + DatabaseName + ".mdf', "
                                              + " SIZE = 5MB,"
                                              + " FILEGROWTH =" + 10 + ") "
                                              + " LOG ON (NAME =" + DatabaseName + "_Log, "
                                              + @" FILENAME = '" + sqlServerDataDirectory + DatabaseName + ".ldf', "
                                              + " SIZE = 1MB, "
                                              + " FILEGROWTH =" + 5 + ")";

                ExecuteDbScript(createDatabaseScript, ConnectionStringFor("master"));
            }


            protected virtual string ConnectionStringFor(string databaseName)
            {
                return string.Format("Server=(local);initial catalog={0};Integrated Security=SSPI", databaseName);
            }


            private void ExecuteDbScript(string sqlScript, string connectionString)
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    using (SqlCommand command = new SqlCommand(sqlScript, conn))
                    {
                        conn.Open();
                        command.ExecuteNonQuery();
                    }
                }
            }


            protected virtual string GetSqlServerDataDirectory()
            {
                string sqlServerRegKey = @"SOFTWARE\Microsoft\Microsoft SQL Server\";
                string sqlServerInstanceName = Registry64.LocalMachine.GetValue(sqlServerRegKey + @"Instance Names\SQL",
                                                                                "MSSQLSERVER");
                string sqlServerInstanceSetupRegKey = sqlServerRegKey + sqlServerInstanceName + @"\Setup";
                return Registry64.LocalMachine.GetValue(sqlServerInstanceSetupRegKey, "SQLDataRoot") + @"\Data\";
            }
        }

        private class MsSql2005ExpressUnitOfWorkTestContextDbStrategy : MsSql2005UnitOfWorkTestContextDbStrategy
        {
            public MsSql2005ExpressUnitOfWorkTestContextDbStrategy(string databaseName)
                : base(databaseName)
            { }

            public override DatabaseEngine DatabaseEngine
            {
                get
                {
                    return DatabaseEngine.MsSql2005Express;
                }
            }

            protected override string ConnectionStringFor(string databaseName)
            {
                return string.Format(@"Server=(local)\SqlExpress;initial catalog={0};Integrated Security=SSPI", databaseName);
            }

            protected override string GetSqlServerDataDirectory()
            {
                string sqlServerRegKey = @"SOFTWARE\Microsoft\Microsoft SQL Server\";
                string sqlServerInstanceName =
                    (string)Registry.LocalMachine
                                 .OpenSubKey(sqlServerRegKey + @"Instance Names\SQL")
                                 .GetValue("SQLEXPRESS");
                string sqlServerInstanceSetupRegKey = sqlServerRegKey + sqlServerInstanceName + @"\Setup";
                return
                    (string)Registry.LocalMachine
                                 .OpenSubKey(sqlServerInstanceSetupRegKey)
                                 .GetValue("SQLDataRoot") + @"\Data\";
            }

        }

        private class SQlLiteUnitOfWorkTestContextDbStrategy : UnitOfWorkTestContextDbStrategy
        {
            public SQlLiteUnitOfWorkTestContextDbStrategy() : base(SQLiteDbName)
            {
                properties.Add(Environment.ConnectionDriver, "NHibernate.Driver.SQLite20Driver");
                properties.Add(Environment.Dialect, "NHibernate.Dialect.SQLiteDialect");
                properties.Add(Environment.ConnectionProvider,
                               "NHibernate.Connection.DriverConnectionProvider");
                string connectionString = string.Format("Data Source={0};Version=3;New=True;", DatabaseName);
                properties.Add(Environment.ConnectionString, connectionString);
                properties.Add(Environment.ShowSql, "true");
                properties.Add(Environment.ReleaseConnections, "on_close");
            }

            public override DatabaseEngine DatabaseEngine
            {
                get { return DatabaseEngine.SQLite; }
            }


            protected override void CreateDatabaseMedia()
            {
                // nothing to do
            }


            protected override void CreateDatabaseSchema(ISession currentSession)
            {
                new SchemaExport(TestContext.Configuration).Execute(false, true, false, true, currentSession.Connection, null);
            }


            public override ISession CreateSession()
            {
                //need to get our own connection, because NH will try to close it
                //as soon as possible, and we will lose the changes.
                IDbConnection dbConnection = ((ISessionFactoryImplementor)TestContext.SessionFactory).ConnectionProvider.GetConnection();
                ISession openSession = TestContext.SessionFactory.OpenSession(dbConnection);
                return openSession;
            }
        }
    }
}
