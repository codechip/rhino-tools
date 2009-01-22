using System;
using System.Diagnostics;
using System.IO;
using System.Web;
using System.Web.Caching;
using Microsoft.Isam.Esent.Interop;

namespace Rhino.DHT
{
    public class PersistentHashTable : IDisposable
    {
        private readonly Instance instance;
        private bool needToDisposeInstance;
        private readonly string database;
        public Action<InstanceParameters> Configure;
        private readonly string path;

        public PersistentHashTable(string database)
        {
            instance = new Instance(database);
            if (Path.IsPathRooted(database) == false)
                database = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, database);
            path = database;
            this.database = Path.Combine(database, Path.GetFileName(database));
        }

        public void Initialize()
        {
            instance.Parameters.CircularLog = true;
            instance.Parameters.CreatePathIfNotExist = true;
            instance.Parameters.TempDirectory = Path.Combine(path, "temp");
            instance.Parameters.SystemDirectory = Path.Combine(path, "system");
            instance.Parameters.LogFileDirectory = Path.Combine(path, "logs");

            if (Configure != null)
                Configure(instance.Parameters);

            instance.Init();
            needToDisposeInstance = true;

            using (var session = new Session(instance))
            {
                try
                {
                    Api.JetAttachDatabase(session, database, AttachDatabaseGrbit.None);
                    return;
                }
                catch (EsentException e)
                {
                    if (e.Error != JET_err.FileNotFound)
                        throw;
                }

                new SchemaCreator(session).Create(database);
                Api.JetAttachDatabase(session, database, AttachDatabaseGrbit.None);
            }
        }

        public void Dispose()
        {
            if (needToDisposeInstance)
            {
                using(var session = new Session(instance))
                {
                    Api.JetDetachDatabase(session, database);
                }
                instance.Dispose();
            }
        }

        public void Batch(Action<PersistentHashTableActions> action)
        {
            using(var pht = new PersistentHashTableActions(instance, database, HttpRuntime.Cache))
            {
                action(pht);
            }
        }
    }
}
