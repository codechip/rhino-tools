using System;
using System.IO;
using System.Web;
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
        private Guid id;

        public Guid Id
        {
            get { return id; }
        }

        public PersistentHashTable(string database)
        {
            this.database = database;
            if (Path.IsPathRooted(database) == false)
                this.database = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, database);
            path = database;
            this.database = Path.Combine(this.database, Path.GetFileName(database));

            instance = new Instance(database + "_" + Guid.NewGuid());
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

            EnsureDatabaseIsCreatedAndAttachToDatabase();

            SetIdFromDb();
        }

        private void SetIdFromDb()
        {
            using (var session = new Session(instance))
            {
                JET_DBID dbid;
                Api.JetOpenDatabase(session, database, "", out dbid, OpenDatabaseGrbit.ReadOnly);
                try
                {
                    using (var details = new Table(session, dbid, "details", OpenTableGrbit.ReadOnly))
                    {
                        Api.JetMove(session, details, JET_Move.First, MoveGrbit.None);
                        var columnids = Api.GetColumnDictionary(session, details);
                        var column = Api.RetrieveColumn(session, details, columnids["id"]);
                        id = new Guid(column);
                    }
                }
                finally
                {
                    Api.JetCloseDatabase(session, dbid, CloseDatabaseGrbit.None);
                }
            }
        }

        private void EnsureDatabaseIsCreatedAndAttachToDatabase()
        {
            using (var session = new Session(instance))
            {
                try
                {
                    Api.JetAttachDatabase(session, database, AttachDatabaseGrbit.None);
                    return;
                }
                catch (EsentErrorException e)
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
                instance.Dispose();
            }
        }

        public void Batch(Action<PersistentHashTableActions> action)
        {
            using (var pht = new PersistentHashTableActions(instance, database, HttpRuntime.Cache, id))
            {
                action(pht);
            }
        }
    }
}
