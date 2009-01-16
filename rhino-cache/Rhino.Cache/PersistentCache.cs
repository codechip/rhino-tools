using System;
using System.Text;
using Microsoft.Isam.Esent.Interop;
using Rhino.Cache.Handlers;

namespace Rhino.Cache
{
    public class PersistentCache : IDisposable
    {
        private readonly string database;
        private readonly Instance instance;

        public int NumberOfDeletedRecordToKeepAround = 50000;

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
                        CreateDeletedItemsTable(session, dbid);
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

        private static void CreateDeletedItemsTable(Session session, JET_DBID dbid)
        {
            JET_TABLEID tableid;
            Api.JetCreateTable(session, dbid, "deletedItems", 16, 100, out tableid);
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
                Api.JetAddColumn(session, tableid, "index", new JET_COLUMNDEF
                {
                    coltyp = JET_coltyp.Long,
                    grbit = ColumndefGrbit.ColumnFixed | ColumndefGrbit.ColumnNotNULL | ColumndefGrbit.ColumnAutoincrement,
                }, null, 0, out columnid);

                var indexDef = "+key\0\0";
                Api.JetCreateIndex(session, tableid, "deletedItems_pk", CreateIndexGrbit.IndexPrimary, indexDef, indexDef.Length, 100);

                indexDef = "+index\0\0";
                Api.JetCreateIndex(session, tableid, "deletedItems_index", CreateIndexGrbit.IndexDisallowNull | CreateIndexGrbit.None, indexDef, indexDef.Length, 100);
            }
            finally
            {
                Api.JetCloseTable(session, tableid);
            }
        }

        public CacheOperation Get(string key)
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
                    //not found, let us see if this is deleted
                    if (Api.TrySeek(session, items, SeekGrbit.SeekEQ) == false)
                    {
                        using (var deletedItems = new Table(session, dbid, "deletedItems", OpenTableGrbit.ReadOnly))
                        {
                            Api.JetSetCurrentIndex(session, deletedItems, "deletedItems_pk");
                            Api.MakeKey(session, deletedItems, key, Encoding.UTF8, MakeKeyGrbit.NewKey);

                            if (Api.TrySeek(session, deletedItems, SeekGrbit.SeekEQ) == false)
                            {
                                // not found and wasn't deleted
                                return null;
                            }
                            return new RemoveFromCache { Key = key };
                        }
                    }
                    var columns = Api.GetColumnDictionary(session, items);
                    return new AddToCache
                    {
                        Key = key,
                        Type = Api.RetrieveColumnAsString(session, items, columns["contentType"], Encoding.UTF8),
                        Data = Api.RetrieveColumn(session, items, columns["data"])
                    };
                }
            }
        }

        public void Put(AddToCache operation)
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
                        //not found, this is a new item
                        var exists = Api.TrySeek(session, items, SeekGrbit.SeekEQ);
                        if (exists == false)
                        {
                            using (var deletedItems = new Table(session, dbid, "deletedItems", OpenTableGrbit.None))
                            {
                                Api.JetSetCurrentIndex(session, deletedItems, "deletedItems_pk");
                                Api.MakeKey(session, deletedItems, key, Encoding.UTF8, MakeKeyGrbit.NewKey);

                                if (Api.TrySeek(session, deletedItems, SeekGrbit.SeekEQ))
                                {
                                    // found on deleted, need to remove this
                                    Api.JetDelete(session, deletedItems);
                                }
                            }
                        }
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

        public void Put(RemoveFromCache operation)
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
                    using (var deletedItems = new Table(session, dbid, "deletedItems", OpenTableGrbit.None))
                    {
                        Api.JetSetCurrentIndex(session, items, "items_pk");
                        Api.MakeKey(session, items, key, Encoding.UTF8, MakeKeyGrbit.NewKey);
                        var exists = Api.TrySeek(session, items, SeekGrbit.SeekEQ);
                        if (exists)
                        {
                            Api.JetDelete(session, items);//delete item

                            Api.JetSetCurrentIndex(session, deletedItems, "deletedItems_pk");

                            int deletedRecordCount = GetDeletedRecordCount(session, deletedItems);

                            Api.MakeKey(session, deletedItems, key, Encoding.UTF8, MakeKeyGrbit.NewKey);

                            exists = Api.TrySeek(session, deletedItems, SeekGrbit.SeekEQ);
                            
                            var columns = Api.GetColumnDictionary(session, deletedItems);
                            var bookmark = new byte[Api.BookmarkMost];
                            int actualBookmarkSize;
                            using (var update = new Update(session, deletedItems, exists ? JET_prep.Replace : JET_prep.Insert))
                            {
                                Api.SetColumn(session, deletedItems, columns["key"], key, Encoding.UTF8);
                                update.Save(bookmark, bookmark.Length, out actualBookmarkSize);
                            }
                            Api.JetGotoBookmark(session, deletedItems, bookmark, actualBookmarkSize);

                            if (deletedRecordCount > NumberOfDeletedRecordToKeepAround)
                            {
                                var index = Api.RetrieveColumnAsInt32(session, deletedItems, columns["index"]).Value;

                                //delete old items delete markers
                                Api.JetSetCurrentIndex(session, deletedItems, "deletedItems_index");
                                var whereToStartRemoving = index - (deletedRecordCount-NumberOfDeletedRecordToKeepAround);
                                Api.MakeKey(session, deletedItems, whereToStartRemoving, MakeKeyGrbit.NewKey);
                                if (Api.TrySeek(session, deletedItems, SeekGrbit.SeekLE))
                                {
                                    do
                                    {
                                        Api.JetDelete(session, deletedItems);
                                    } while (Api.TryMovePrevious(session, deletedItems));
                                }
                            }
                        }
                    }

                    tx.Commit(CommitTransactionGrbit.LazyFlush);
                }
            }
        }

        private int GetDeletedRecordCount(Session session, Table deletedItems)
        {
            JET_INDEXLIST indexlist;
            Api.JetGetTableIndexInfo(session, deletedItems, null, out indexlist);

            return indexlist.cRecord;
        }

        public void Dispose()
        {
            instance.Dispose();
        }
    }
}