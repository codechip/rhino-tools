using System;
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
        private string name;

        public PersistentHashTable(string database)
        {
            instance = new Instance(database);
            this.name = database;
            this.database = Path.Combine(database, Path.GetFileName(database));
        }

        public void Initialize()
        {
            instance.Parameters.CircularLog = true;
            instance.Parameters.CreatePathIfNotExist = true;
            instance.Parameters.TempDirectory = Path.Combine(name, "temp");
            instance.Parameters.SystemDirectory = Path.Combine(name, "system");
            instance.Parameters.LogFileDirectory = Path.Combine(name, "logs");

            if (Configure != null)
                Configure(instance.Parameters);

            instance.Init();
            needToDisposeInstance = true;

            using (var session = new Session(instance))
            {
                try
                {
                    Api.JetAttachDatabase(session, database, AttachDatabaseGrbit.None);
                    Api.JetDetachDatabase(session, database);
                    return;
                }
                catch (EsentException e)
                {
                    if (e.Error != JET_err.FileNotFound)
                        throw;
                }

                new SchemaCreator(session).Create(database);
            }
        }

        public void Dispose()
        {
            if (needToDisposeInstance)
                instance.Dispose();
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
