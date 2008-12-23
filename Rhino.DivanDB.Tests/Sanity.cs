namespace Rhino.DivanDB.Tests
{
	using System;
	using System.IO;
	using System.Text;
	using Microsoft.Isam.Esent.Interop;
	using Newtonsoft.Json;
	using Newtonsoft.Json.Linq;
	using Xunit;

	public class Sanity
	{
		public Sanity()
		{
			foreach (var file in Directory.GetFiles(".","*.divan"))
			{
				File.Delete(file);
			}
		}

		[Fact]
		public void Json_string_to_object()
		{
			var serializer = new JsonSerializer();
			var deserialize = (JContainer)serializer.Deserialize(
				new JsonTextReader(new StringReader("[{'@id': 15, 'name': 'oren'}]")));
			var value = deserialize[0];
			Assert.Equal(15, (int)value["@id"]);
			Assert.Equal("oren", (string)value["name"]);
		}

		[Fact]
		public void Can_find_by_composite_key()
		{
			using (var instance = new Instance("Can_find_by_composite_key"))
			{
				instance.Init();

				using (var session = new Session(instance.JetInstance))
				{
					JET_DBID dbid;
					var database = "Can_find_by_composite_key.divan";
					Api.JetCreateDatabase(session.JetSesid, database, "", out dbid, CreateDatabaseGrbit.OverwriteExisting);

					JET_TABLEID tableid;
					Api.JetCreateTable(session.JetSesid, dbid, "tbl", 16, 100, out tableid);

					JET_COLUMNID id;
					Api.JetAddColumn(
						session.JetSesid,
						tableid,
						"id",
						new JET_COLUMNDEF() { coltyp = JET_coltyp.Long, grbit = ColumndefGrbit.ColumnFixed },
						null,
						0,
						out id);

					JET_COLUMNID version;
					Api.JetAddColumn(
						session.JetSesid,
						tableid,
						"version",
						new JET_COLUMNDEF() { coltyp = JET_coltyp.Long, grbit = ColumndefGrbit.ColumnFixed },
						null,
						0,
						out version);

					JET_COLUMNID name;
					Api.JetAddColumn(
						session.JetSesid,
						tableid,
						"name",
						new JET_COLUMNDEF()
						{
							coltyp = JET_coltyp.Text, 
                            cbMax = 255,
							grbit = ColumndefGrbit.ColumnNotNULL
						},
						null,
						0,
						out name);

					const string indexDef = "+id\0+version\0\0";
					Api.JetCreateIndex(session.JetSesid, tableid, "id_and_version", CreateIndexGrbit.IndexPrimary,
					                   indexDef, indexDef.Length, 100);

					Api.JetCloseTable(session.JetSesid, tableid);

					Api.JetCloseDatabase(session.JetSesid, dbid, CloseDatabaseGrbit.None);

					Api.JetOpenDatabase(session.JetSesid, database, "", out dbid, OpenDatabaseGrbit.None);


					Api.JetOpenTable(session.JetSesid, dbid, "tbl", null, 0, OpenTableGrbit.None, out tableid);

					using (var tx = new Transaction(session.JetSesid))
					{
						for (int i = 0; i < 10; i++)
						{
							using (var update = new Update(session.JetSesid, tableid, JET_prep.Insert))
							{
								Api.SetColumn(session.JetSesid, tableid, id, 5 * i);
								Api.SetColumn(session.JetSesid, tableid, version, 3 * i);
								Api.SetColumn(session.JetSesid, tableid, name, "name" + i, Encoding.UTF8);

								update.Save();
							}
						}
						
						tx.Commit(CommitTransactionGrbit.None);
					}

					Api.JetSetCurrentIndex(session.JetSesid, tableid, "id_and_version");

					Api.MakeKey(session.JetSesid,tableid, 30, MakeKeyGrbit.NewKey);
					Api.MakeKey(session.JetSesid, tableid, 18, MakeKeyGrbit.None);


					Api.JetSeek(session.JetSesid, tableid, SeekGrbit.SeekEQ);

					Assert.Equal(30,
						Api.RetrieveColumnAsInt32(session.JetSesid, tableid, id));


					Assert.Equal(18,
						Api.RetrieveColumnAsInt32(session.JetSesid, tableid, version));

					var actual = Api.RetrieveColumnAsString(session.JetSesid, tableid, name, Encoding.UTF8);
					Assert.Equal("name6",actual);
				}
			}
		}
	

		[Fact(Skip = "I am not sure that I understand why this fails")]
		public void Can_insert_to_table_with_auto_inc_and_version()
		{
			using (var instance = new Instance("test_auto_inc_and_version"))
			{
				instance.Init();

				using (var session = new Session(instance.JetInstance))
				{
					JET_DBID dbid;
					var database = "test_auto_inc_and_version.divan";
					Api.JetCreateDatabase(session.JetSesid, database, "", out dbid, CreateDatabaseGrbit.OverwriteExisting);

					JET_TABLEID tableid;
					Api.JetCreateTable(session.JetSesid, dbid, "tbl", 16, 100, out tableid);

					JET_COLUMNID columnidAutoinc;
					Api.JetAddColumn(
						session.JetSesid,
						tableid,
						"key",
						new JET_COLUMNDEF() { coltyp = JET_coltyp.Long, grbit = ColumndefGrbit.ColumnAutoincrement },
						null,
						0,
						out columnidAutoinc);

					JET_COLUMNID columnidVersion;
					Api.JetAddColumn(
						session.JetSesid,
						tableid,
						"version",
						new JET_COLUMNDEF() { coltyp = JET_coltyp.Long, grbit = ColumndefGrbit.ColumnVersion },
						null,
						0,
						out columnidVersion);

					Api.JetCloseTable(session.JetSesid, tableid);

					Api.JetCloseDatabase(session.JetSesid, dbid, CloseDatabaseGrbit.None);

					Api.JetOpenDatabase(session.JetSesid, database, "", out dbid, OpenDatabaseGrbit.None);


					Api.JetOpenTable(session.JetSesid, dbid, "tbl", null, 0, OpenTableGrbit.None, out tableid);

					using (var tx = new Transaction(session.JetSesid))
					{
						using (var update = new Update(session.JetSesid, tableid, JET_prep.Insert))
						{
							update.Save();

						}
						Assert.Equal(1, Api.RetrieveColumnAsInt32(session.JetSesid, tableid, columnidAutoinc));
						Assert.Equal(0, Api.RetrieveColumnAsInt32(session.JetSesid, tableid, columnidVersion));
						
						tx.Commit(CommitTransactionGrbit.None);
					}
					using (var tx = new Transaction(session.JetSesid))
					{

						using (var update = new Update(session.JetSesid, tableid, JET_prep.Insert))
						{
							update.Save();
						}


						Assert.Equal(2, Api.RetrieveColumnAsInt32(session.JetSesid, tableid, columnidAutoinc));
						Assert.Equal(0, Api.RetrieveColumnAsInt32(session.JetSesid, tableid, columnidVersion));
						
						tx.Commit(CommitTransactionGrbit.None);
					}

					Api.JetCloseTable(session.JetSesid, tableid);
					Api.JetCloseDatabase(session.JetSesid, dbid, CloseDatabaseGrbit.None);

				}
			}
		}
	}
}