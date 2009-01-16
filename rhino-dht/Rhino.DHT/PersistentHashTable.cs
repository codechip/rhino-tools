using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Isam.Esent.Interop;
using System.Linq;

namespace Rhino.DHT
{
    public class PersistentHashTable : IDisposable
    {
        private readonly Instance instance = new Instance("rhino-dht");

        private readonly string database;

        public PersistentHashTable(string database)
        {
            this.database = database;
        }

        public void Initialize()
        {
            instance.Parameters.CircularLog = true;
            instance.Init();

            using (var session = new Session(instance))
            {
                try
                {
                    Api.JetAttachDatabase(session, database, AttachDatabaseGrbit.None);
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
            instance.Dispose();
        }

        public void Batch(Action<PersistentHashTableActions> action)
        {
            using(var pht = new PersistentHashTableActions(instance, database))
            {
                action(pht);
            }
        }
    }
}
