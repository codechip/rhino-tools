using System;
using System.Text;
using Microsoft.Isam.Esent.Interop;
using Rhino.DHT.Handlers;
using Rhino.DHT.Handlers;

namespace Rhino.DHT
{
    public class PersistentCache : IDisposable
    {
        private readonly string database;
        private readonly Instance instance;

        public PersistentCache(string database)
        {
            this.database = database;
            instance = new Instance("rhino-cache");
            instance.Parameters.CircularLog = true;
            instance.Init();
            CreateSchemaIfNeeded();
        }

        private void CreateSchemaIfNeeded()
        {
            using (var session = new Session(instance))
            {
                try
                {
                    Api.JetAttachDatabase(session, database, AttachDatabaseGrbit.ReadOnly);
                    Api.JetDetachDatabase(session, database);
                    return;//database already exists
                }
                catch (EsentException e)
                {
                    if (e.Error != JET_err.FileNotFound)
                        throw;
                }

                JET_DBID dbid;
                Api.JetCreateDatabase(session, database, null, out dbid, CreateDatabaseGrbit.None);
                try
                {
                    using (var transaction = new Transaction(session))
                    {
                        CreateItemsTable(session, dbid);
                        transaction.Commit(CommitTransactionGrbit.None);
                    }
                }
                finally
                {
                    Api.JetCloseDatabase(session, dbid, CloseDatabaseGrbit.None);
                }
            }
        }

        private static void CreateItemsTable(Session session, JET_DBID dbid)
        {
            JET_TABLEID tableid;
            Api.JetCreateTable(session, dbid, "items", 16, 100, out tableid);
            try
            {
                JET_COLUMNID columnid;
                Api.JetAddColumn(session, tableid, "key", new JET_COLUMNDEF
                {
                    cbMax = 255,
                    coltyp = JET_coltyp.Text,
                    grbit = ColumndefGrbit.ColumnFixed | ColumndefGrbit.ColumnNotNULL,
                    cp = JET_CP.Unicode
                }, null, 0, out columnid);
                Api.JetAddColumn(session, tableid, "contentType", new JET_COLUMNDEF
                {
                    cbMax = 255,
                    coltyp = JET_coltyp.Text,
                    grbit = ColumndefGrbit.ColumnNotNULL,
                    cp = JET_CP.Unicode
                }, null, 0, out columnid);
                Api.JetAddColumn(session, tableid, "data", new JET_COLUMNDEF
                {
                    cbMax = int.MaxValue,
                    coltyp = JET_coltyp.LongBinary,
                    grbit = ColumndefGrbit.ColumnNotNULL,
                }, null, 0, out columnid);

                var indexDef = "+key\0\0";
                Api.JetCreateIndex(session, tableid, "items_pk", CreateIndexGrbit.IndexPrimary, indexDef, indexDef.Length, 100);
            }
            finally
            {
                Api.JetCloseTable(session, tableid);
            }
        }

        public CacheItem Get(string key)
        {
            using (var session = new Session(instance))
            {
                JET_DBID dbid;

                Api.JetAttachDatabase(session, database, AttachDatabaseGrbit.ReadOnly);
                Api.JetOpenDatabase(session, database, null, out dbid, OpenDatabaseGrbit.ReadOnly);

                using (var items = new Table(session, dbid, "items", OpenTableGrbit.ReadOnly))
                {
                    Api.JetSetCurrentIndex(session, items, "items_pk");
                    Api.MakeKey(session, items, key, Encoding.UTF8, MakeKeyGrbit.NewKey);
                    if (Api.TrySeek(session, items, SeekGrbit.SeekEQ) == false)
                    {
                        return null;
                    }
                    var columns = Api.GetColumnDictionary(session, items);
                    return new CacheItem
                    {
                        Key = key,
                        Type = Api.RetrieveColumnAsString(session, items, columns["contentType"], Encoding.UTF8),
                        Data = Api.RetrieveColumn(session, items, columns["data"])
                    };
                }
            }
        }

        public void Put(CacheItem operation)
        {
            var key = operation.Key;
            using (var session = new Session(instance))
            {
                Api.JetAttachDatabase(session, database, AttachDatabaseGrbit.None);

                using (var tx = new Transaction(session))
                {
                    JET_DBID dbid;

                    Api.JetOpenDatabase(session, database, null, out dbid, OpenDatabaseGrbit.None);

                    using (var items = new Table(session, dbid, "items", OpenTableGrbit.None))
                    {
                        Api.JetSetCurrentIndex(session, items, "items_pk");
                        Api.MakeKey(session, items, key, Encoding.UTF8, MakeKeyGrbit.NewKey);
                        
                        var exists = Api.TrySeek(session, items, SeekGrbit.SeekEQ);
                        
                        var columns = Api.GetColumnDictionary(session, items);
                        using (var update = new Update(session, items, exists ? JET_prep.Replace : JET_prep.Insert))
                        {
                            Api.SetColumn(session, items, columns["key"], key, Encoding.UTF8);
                            Api.SetColumn(session, items, columns["contentType"], operation.Type, Encoding.UTF8);
                            Api.SetColumn(session, items, columns["data"], operation.Data);
                            update.Save();
                        }
                    }

                    tx.Commit(CommitTransactionGrbit.LazyFlush);
                }
            }
        }

        public void Remove(string key)
        {
            using (var session = new Session(instance))
            {
                Api.JetAttachDatabase(session, database, AttachDatabaseGrbit.None);

                using (var tx = new Transaction(session))
                {
                    JET_DBID dbid;

                    Api.JetOpenDatabase(session, database, null, out dbid, OpenDatabaseGrbit.None);

                    using (var items = new Table(session, dbid, "items", OpenTableGrbit.None))
                    {
                        Api.JetSetCurrentIndex(session, items, "items_pk");
                        Api.MakeKey(session, items, key, Encoding.UTF8, MakeKeyGrbit.NewKey);
                        var exists = Api.TrySeek(session, items, SeekGrbit.SeekEQ);
                        if (exists)
                        {
                            Api.JetDelete(session, items);//delete item
                        }
                    }

                    tx.Commit(CommitTransactionGrbit.LazyFlush);
                }
            }
        }

        public void Dispose()
        {
            instance.Dispose();
        }
    }
}