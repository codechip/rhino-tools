using Microsoft.Isam.Esent.Interop;

namespace Rhino.DHT
{
    public class SchemaCreator
    {
        private readonly Session session;

        public SchemaCreator(Session session)
        {
            this.session = session;
        }

        public void Create(string database)
        {
            JET_DBID dbid;
            Api.JetCreateDatabase(session, database, null, out dbid, CreateDatabaseGrbit.None);

            using (var tx = new Transaction(session))
            {
                CreateKeysTable(dbid);
                CreateDataTable(dbid);

                tx.Commit(CommitTransactionGrbit.None);
            }
        }

        private void CreateKeysTable(JET_DBID dbid)
        {
            JET_TABLEID tableid;
            Api.JetCreateTable(session, dbid, "keys", 16, 100, out tableid);
            JET_COLUMNID columnid;
            Api.JetAddColumn(session, tableid, "key", new JET_COLUMNDEF
            {
                cbMax = 255,
                coltyp = JET_coltyp.Text,
                cp = JET_CP.Unicode,
                grbit = ColumndefGrbit.ColumnNotNULL
            }, null, 0, out columnid);

            Api.JetAddColumn(session, tableid, "version", new JET_COLUMNDEF
            {
                coltyp = JET_coltyp.Long,
                grbit = ColumndefGrbit.ColumnFixed | ColumndefGrbit.ColumnNotNULL | ColumndefGrbit.ColumnAutoincrement
            }, null, 0, out columnid);


            Api.JetAddColumn(session, tableid, "expiresAt", new JET_COLUMNDEF
            {
                coltyp = JET_coltyp.DateTime,
                grbit = ColumndefGrbit.ColumnFixed
            }, null, 0, out columnid);

            var indexDef = "+key\0+version\0\0";
            Api.JetCreateIndex(session, tableid, "pk", CreateIndexGrbit.IndexPrimary, indexDef, indexDef.Length,
                               100);

            indexDef = "+key\0\0";
            Api.JetCreateIndex(session, tableid, "by_key", CreateIndexGrbit.IndexDisallowNull, indexDef, indexDef.Length,
                               100);

            indexDef = "+expiresAt\0\0";
            Api.JetCreateIndex(session, tableid, "by_expiry", CreateIndexGrbit.IndexIgnoreAnyNull, indexDef, indexDef.Length,
                               100);
        }

        private void CreateDataTable(JET_DBID dbid)
        {
            JET_TABLEID tableid;
            Api.JetCreateTable(session, dbid, "data", 16, 100, out tableid);
            JET_COLUMNID columnid;
            Api.JetAddColumn(session, tableid, "key", new JET_COLUMNDEF
            {
                cbMax = 255,
                coltyp = JET_coltyp.Text,
                cp = JET_CP.Unicode,
                grbit = ColumndefGrbit.ColumnNotNULL
            }, null, 0, out columnid);

            Api.JetAddColumn(session, tableid, "version", new JET_COLUMNDEF
            {
                coltyp = JET_coltyp.Long,
                grbit = ColumndefGrbit.ColumnNotNULL
            }, null, 0, out columnid);

            Api.JetAddColumn(session, tableid, "data", new JET_COLUMNDEF
            {
                coltyp = JET_coltyp.LongBinary,
                grbit = ColumndefGrbit.ColumnNotNULL
            }, null, 0, out columnid);

            Api.JetAddColumn(session, tableid, "expiresAt", new JET_COLUMNDEF
            {
                coltyp = JET_coltyp.DateTime,
                grbit = ColumndefGrbit.ColumnFixed
            }, null, 0, out columnid);

            Api.JetAddColumn(session, tableid, "sha256_hash", new JET_COLUMNDEF
            {
                coltyp = JET_coltyp.Binary,
                grbit = ColumndefGrbit.ColumnFixed,
                cbMax = 32
            }, null, 0, out columnid);

            Api.JetAddColumn(session, tableid, "parentVersions", new JET_COLUMNDEF
            {
                coltyp = JET_coltyp.Long,
                grbit = ColumndefGrbit.ColumnTagged | ColumndefGrbit.ColumnMultiValued
            }, null, 0, out columnid);


            var indexDef = "+key\0+version\0\0";
            Api.JetCreateIndex(session, tableid, "pk", CreateIndexGrbit.IndexPrimary, indexDef, indexDef.Length,
                               100);
        }

    }
}