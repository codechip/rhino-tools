namespace Rhino.DivanDB.Tests
{
	using System;
	using System.IO;
	using Exceptions;
	using Impl;
	using Microsoft.Isam.Esent.Interop;
	using Newtonsoft.Json.Linq;
	using Xunit;

	public class DivanDatabaseTest
	{
		public DivanDatabaseTest()
		{
			File.Delete("test.divan");
		}

		[Fact]
		public void Can_add_document_to_database()
		{
			using (var instance = new Instance("test"))
			{
				instance.Init();

				using (var sesion = new Session(instance.JetInstance))
				{
					var database = new DivanDatabase(instance, sesion, "test.divan");
					DocumentId[] add = database.AddOrUpdate(
						new Document(
							new DocumentId(),
							JObject.Parse("{'name': 'oren', 'email': 'ayende@ayende.com'"))
						);

					var doc = database.Find(add[0].Id.Value);

					Assert.Equal("oren", (string)doc["name"]);
					Assert.Equal("ayende@ayende.com", (string)doc["email"]);
				}
			}
		}

		[Fact]
		public void Can_update_document_to_database()
		{
			using (var instance = new Instance("test"))
			{
				instance.Init();

				using (var sesion = new Session(instance.JetInstance))
				{
					var database = new DivanDatabase(instance, sesion, "test.divan");
					var add = database.AddOrUpdate(
						new Document(
							new DocumentId(),
							JObject.Parse("{'name': 'oren', 'email': 'ayende@ayende.com'"))
						);

					DocumentId[] update = database.AddOrUpdate(
						new Document(
							add[0],
							JObject.Parse("{'name': 'ayende', 'email': 'blog@ayende.com'"))
						);

					Assert.Equal(add[0].Id, update[0].Id);
					Assert.Equal(add[0].Version + 1, update[0].Version);
					
					var doc = database.Find(add[0].Id.Value);

					Assert.Equal("ayende", (string) doc["name"]);
					Assert.Equal("blog@ayende.com", (string) doc["email"]);
				}
			}
		}

		[Fact]
		public void If_version_does_not_match_then_throw_exception()
		{
			using (var instance = new Instance("test"))
			{
				instance.Init();

				using (var sesion = new Session(instance.JetInstance))
				{
					var database = new DivanDatabase(instance, sesion, "test.divan");
					var add = database.AddOrUpdate(
						new Document(
							new DocumentId(),
							JObject.Parse("{'name': 'oren', 'email': 'ayende@ayende.com'"))
						);

					Assert.Throws<OptimisticConcurrencyException>(() =>
					{
						database.AddOrUpdate(
							new Document(
								new DocumentId
								{
									Id = add[0].Id,
									Version = -1
								},
								JObject.Parse("{'name': 'ayende', 'email': 'blog@ayende.com'"))
							);
					});
				}
			}
		}

		[Fact]
		public void When_adding_item_will_add_it_to_the_to_be_indexed_queue()
		{
			using (var instance = new Instance("test"))
			{
				instance.Init();

				using (var sesion = new Session(instance.JetInstance))
				{
					var database = new DivanDatabase(instance, sesion, "test.divan");
					var add = database.AddOrUpdate(
						new Document(
							new DocumentId(),
							JObject.Parse("{'name': 'oren', 'email': 'ayende@ayende.com'"))
						);

					JET_DBID dbid;
					Api.JetOpenDatabase(sesion.JetSesid, "test.divan", "", out dbid, OpenDatabaseGrbit.None);
					try
					{
						using (var tbl = new Table(sesion.JetSesid, dbid, "ToBeIndexed", OpenTableGrbit.None))
						{
							var dictionary = Api.GetColumnDictionary(sesion.JetSesid, tbl.JetTableid);

							Assert.True(Api.TryMoveFirst(sesion.JetSesid, tbl.JetTableid));

							var bytes = Api.RetrieveColumn(sesion.JetSesid, tbl.JetTableid, dictionary["id"]);

							Assert.Equal(add[0].Id, new Guid(bytes));
						}
					}
					finally
					{
						Api.JetCloseDatabase(sesion.JetSesid, dbid, CloseDatabaseGrbit.None);
					}

				}
			}
		}


		[Fact]
		public void When_transaction_is_committed_will_raise_index_view_events()
		{
			using (var instance = new Instance("test"))
			{
				instance.Init();

				using (var sesion = new Session(instance.JetInstance))
				{
					var database = new DivanDatabase(instance, sesion, "test.divan");

					string eventDatabase = null;
					DocumentId[] documentIds = null;
					database.NeedIndexing += (s, ids) =>
					{
						eventDatabase = s;
						documentIds = ids;
					};

					var add = database.AddOrUpdate(
						new Document(
							new DocumentId(),
							JObject.Parse("{'name': 'oren', 'email': 'ayende@ayende.com'"))
						);


					Assert.Equal("test.divan", eventDatabase);
					Assert.Equal(1, documentIds.Length);
					Assert.Equal(add[0].Id, documentIds[0].Id);
				}
			}
		}
	}
}