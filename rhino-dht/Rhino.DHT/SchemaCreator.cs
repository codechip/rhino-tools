using Microsoft.Isam.Esent.Interop;

namespace Rhino.DHT
{
	using System;

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
				CreateDetailsTable(dbid);
                CreateKeysTable(dbid);
                CreateDataTable(dbid);

                tx.Commit(CommitTransactionGrbit.None);
            }
        }

		private void CreateDetailsTable(JET_DBID dbid)
    	{
			JET_TABLEID tableid;
			Api.JetCreateTable(session, dbid, "details", 16, 100, out tableid);
			JET_COLUMNID columnid;
			Api.JetAddColumn(session, tableid, "id", new JET_COLUMNDEF
			{
				cbMax = 16,
				coltyp = JET_coltyp.Binary,
				grbit = ColumndefGrbit.ColumnNotNULL|ColumndefGrbit.ColumnFixed
			}, null, 0, out columnid);


			using(var update = new Update(session, tableid, JET_prep.Insert))
			{
				Api.SetColumn(session, tableid, columnid, Guid.NewGuid().ToByteArray());
				update.Save();
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

            Api.JetAddColumn(session, tableid, "version_instance_id", new JET_COLUMNDEF
            {
                coltyp = JET_coltyp.Binary,
                cbMax = 16,
                grbit = ColumndefGrbit.ColumnFixed | ColumndefGrbit.ColumnNotNULL
            }, null, 0, out columnid);

            Api.JetAddColumn(session, tableid, "version_number", new JET_COLUMNDEF
            {
                coltyp = JET_coltyp.Long,
                grbit = ColumndefGrbit.ColumnFixed | ColumndefGrbit.ColumnNotNULL | ColumndefGrbit.ColumnAutoincrement
            }, null, 0, out columnid);

            Api.JetAddColumn(session, tableid, "expiresAt", new JET_COLUMNDEF
            {
                coltyp = JET_coltyp.DateTime,
                grbit = ColumndefGrbit.ColumnFixed
            }, null, 0, out columnid);

            var indexDef = "+key\0+version_number\0+version_instance_id\0\0";
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

            Api.JetAddColumn(session, tableid, "version_instance_id", new JET_COLUMNDEF
            {
                coltyp = JET_coltyp.Binary,
                cbMax = 16,
                grbit = ColumndefGrbit.ColumnFixed | ColumndefGrbit.ColumnNotNULL
            }, null, 0, out columnid);

            Api.JetAddColumn(session, tableid, "version_number", new JET_COLUMNDEF
            {
                coltyp = JET_coltyp.Long,
                grbit = ColumndefGrbit.ColumnFixed | ColumndefGrbit.ColumnNotNULL
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
                coltyp = JET_coltyp.Binary,
                cbMax = 20, /*16 + 4*/
                grbit = ColumndefGrbit.ColumnTagged | ColumndefGrbit.ColumnMultiValued
            }, null, 0, out columnid);


            var indexDef = "+key\0+version_number\0+version_instance_id\0\0";
            Api.JetCreateIndex(session, tableid, "pk", CreateIndexGrbit.IndexPrimary, indexDef, indexDef.Length,
                               100);
        }

    }
}