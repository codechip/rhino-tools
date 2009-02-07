using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Web;
using Microsoft.Isam.Esent.Interop;
using System.Linq;

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

        private readonly HashSet<string> replicationDestinations = new HashSet<string>();
        private bool recordChangedForReplication;

        public string[] ReplicationDestinations
        {
            get
            {
                lock (replicationDestinations)
                {
                    return replicationDestinations.ToArray();
                }
            }
        }

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
            LoadReplicationDestinations();
        }

        private void LoadReplicationDestinations()
        {
            instance.WithDatabase(database, (session, dbid) =>
            {
                using (var details = new Table(session, dbid, "replication_destinations", OpenTableGrbit.ReadOnly))
                {
                    var columnids = Api.GetColumnDictionary(session, details);
                    if (Api.TryMoveFirst(session, details) == false)
                        return;
                    do
                    {
                        var destination = Api.RetrieveColumnAsString(session, details, columnids["destination"],
                                                                     Encoding.Unicode);
                        replicationDestinations.Add(destination);
                    } while (Api.TryMoveNext(session, details));
                    recordChangedForReplication = replicationDestinations.Count > 0;
                }
            });
        }

        private void SetIdFromDb()
        {
            instance.WithDatabase(database, (session, dbid) =>
            {
                using (var details = new Table(session, dbid, "details", OpenTableGrbit.ReadOnly))
                {
                    Api.JetMove(session, details, JET_Move.First, MoveGrbit.None);
                    var columnids = Api.GetColumnDictionary(session, details);
                    var column = Api.RetrieveColumn(session, details, columnids["id"]);
                    id = new Guid(column);
                }
            });
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

        public void AddReplicationDestination(string destination)
        {
            instance.WithDatabase(database, (session, dbid) =>
            {
                lock (replicationDestinations)
                {
                    if (replicationDestinations.Add(destination) == false)
                        return;

                    using(var tx = new Transaction(session))
                    using (var details = new Table(session, dbid, "replication_destinations", OpenTableGrbit.None))
                    using (var update = new Update(session, details, JET_prep.Insert))
                    {
                        var columns = Api.GetColumnDictionary(session, details);

                        Api.SetColumn(session, details, columns["destination"], destination, Encoding.Unicode);

                        update.Save();
                        tx.Commit(CommitTransactionGrbit.None);
                    }
                    recordChangedForReplication = replicationDestinations.Count > 0;
                }
            });
        }

        public void RemoveReplicationSource(string destination)
        {
            instance.WithDatabase(database, (session, dbid) =>
            {
                lock (replicationDestinations)
                {
                    if (replicationDestinations.Remove(destination) == false)
                        return;
                    
                    using (var tx = new Transaction(session))
                    using (var details = new Table(session, dbid, "replication_destinations", OpenTableGrbit.None))
                    {
                        var columns = Api.GetColumnDictionary(session, details);

                        if (Api.TryMoveFirst(session, details) == false)
                            return;
                        
                        do
                        {
                            var destinationInTable = 
                                Api.RetrieveColumnAsString(session, details, columns["destination"]);
                            
                            if (destinationInTable != destination) 
                                continue;

                            Api.JetDelete(session, details);

                        } while (Api.TryMoveNext(session, details));

                        tx.Commit(CommitTransactionGrbit.None);
                    }
                    recordChangedForReplication = replicationDestinations.Count > 0;
                }
            });
        }

        public void Batch(Action<PersistentHashTableActions> action)
        {
            using (var pht = new PersistentHashTableActions(instance, database, HttpRuntime.Cache, id, recordChangedForReplication))
            {
                action(pht);
            }
        }
    }
}
