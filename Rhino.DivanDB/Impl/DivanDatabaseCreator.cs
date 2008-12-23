namespace Rhino.DivanDB.Impl
{
	using Microsoft.Isam.Esent.Interop;

	public class DivanDatabaseCreator
	{
		private readonly Instance instance;
		private readonly string databaseName;
		private Session session;
		private JET_DBID database;


		private const JET_coltyp JET_coltypGuid = (JET_coltyp)16;

		public DivanDatabaseCreator(Instance instance, string databaseName)
		{
			this.instance = instance;
			this.databaseName = databaseName;
		}

		public void CreateDatabase()
		{
			using (session = new Session(instance.JetInstance))
			{
				Api.JetCreateDatabase(session.JetSesid, databaseName, "", out database, CreateDatabaseGrbit.OverwriteExisting);
				using (var transaction = new Transaction(session.JetSesid))
				{
					CreateDocumentsTable();
					CreateToBeindexedTable();

					transaction.Commit(CommitTransactionGrbit.None);
				}
			}
		}

		private void CreateToBeindexedTable()
		{
			JET_TABLEID tableid;
			Api.JetCreateTable(session.JetSesid, database, "ToBeIndexed", 16, 100, out tableid);
			try
			{
				JET_COLUMNID idColumndId;
				Api.JetAddColumn(session.JetSesid, tableid, "id", new JET_COLUMNDEF
				{
					coltyp = JET_coltypGuid,
					grbit = ColumndefGrbit.ColumnFixed | ColumndefGrbit.ColumnNotNULL
				}, null, 0, out idColumndId);

				JET_COLUMNID versionColumndId;
				Api.JetAddColumn(session.JetSesid, tableid, "version", new JET_COLUMNDEF
				{
					coltyp = JET_coltyp.Long,
					grbit = ColumndefGrbit.ColumnFixed
				}, null, 0, out versionColumndId);

			}
			finally
			{
				Api.JetCloseTable(session.JetSesid, tableid);
			}
		}

		private void CreateDocumentsTable()
		{
			JET_TABLEID tableid;
			Api.JetCreateTable(session.JetSesid, database, "documents", 16, 100, out tableid);
			try
			{
				JET_COLUMNID idColumndId;
				Api.JetAddColumn(session.JetSesid, tableid, "id", new JET_COLUMNDEF
				{
					coltyp = JET_coltypGuid,
					grbit = ColumndefGrbit.ColumnFixed | ColumndefGrbit.ColumnNotNULL
				}, null, 0, out idColumndId);

				JET_COLUMNID versionColumndId;
				Api.JetAddColumn(session.JetSesid, tableid, "version", new JET_COLUMNDEF
				{
					coltyp = JET_coltyp.Long,
					grbit = ColumndefGrbit.ColumnFixed | ColumndefGrbit.ColumnVersion
				}, null, 0, out versionColumndId);

				JET_COLUMNID documentColumndId;
				Api.JetAddColumn(session.JetSesid, tableid, "document", new JET_COLUMNDEF
				{
					coltyp = JET_coltyp.LongText,
					cp = JET_CP.Unicode,
					cbMax = int.MaxValue - 10000,
					// 2 GB
					grbit = ColumndefGrbit.ColumnNotNULL
				}, null, 0, out documentColumndId);

				const string by_id_indexDef = "+id\0\0";
				Api.JetCreateIndex(session.JetSesid, tableid, "by_id",
					CreateIndexGrbit.IndexDisallowNull | CreateIndexGrbit.IndexPrimary | CreateIndexGrbit.IndexUnique,
					by_id_indexDef, by_id_indexDef.Length, 100);
			}
			finally
			{
				Api.JetCloseTable(session.JetSesid, tableid);
			}
		}
	}
}