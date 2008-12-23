namespace Rhino.DivanDB.Impl
{
	using System;
	using System.Collections.Generic;
	using System.IO;
	using System.Text;
	using DSL;
	using Exceptions;
	using Microsoft.Isam.Esent.Interop;
	using Newtonsoft.Json;
	using Newtonsoft.Json.Linq;
	using System.Linq;

	public class DivanDatabase : IDivanDatabase
	{
		private readonly Session session;
		private readonly string databaseName;
		public event Action<string, DocumentId[]> NeedIndexing = delegate { };

		public DivanDatabase(Instance instance, Session session, string databaseName)
		{
			this.session = session;
			this.databaseName = databaseName;
			try
			{
				Api.JetAttachDatabase(session.JetSesid, databaseName, AttachDatabaseGrbit.None);
			}
			catch (EsentException e)
			{
				if (e.Error == JET_err.DatabaseNotFound ||
					e.Error == JET_err.FileNotFound)
				{
					new DivanDatabaseCreator(instance, databaseName).CreateDatabase();

					Api.JetAttachDatabase(session.JetSesid, databaseName, AttachDatabaseGrbit.None);
				}
				else
				{
					throw;
				}
			}
		}

		public IndexerBase[] Views
		{
			get
			{
				if(Directory.Exists(ViewDirectory)==false)
					return new IndexerBase[0];

				return Factory.Instance.CreateAll<IndexerBase>(ViewDirectory);
			}
		}

		private string ViewDirectory
		{
			get { return Path.Combine(Path.GetDirectoryName(databaseName), "Views"); }
		}

		public void AddView(string name, string function)
		{
			if (Directory.Exists(ViewDirectory) == false)
				Directory.CreateDirectory(ViewDirectory);

			File.WriteAllText(Path.Combine(ViewDirectory, name + ".boo"), function);
		}

		public IDivanView OpenView(string name)
		{
			throw new NotImplementedException();
		}

		private DocumentId AddDocumentToDatabase(
			Table table, 
			JToken document,
			Guid? documentId,
			int documentVersion)
		{
			var dictionary = Api.GetColumnDictionary(session.JetSesid, table.JetTableid);

			if (documentId.HasValue)
			{
				Api.MakeKey(session.JetSesid, table.JetTableid, documentId.Value, MakeKeyGrbit.NewKey);
				Api.JetSeek(session.JetSesid, table.JetTableid, SeekGrbit.SeekEQ);

				var existingDocumentVersion = (int)Api.RetrieveColumnAsInt32(session.JetSesid, table.JetTableid, dictionary["version"]);
				if (existingDocumentVersion != documentVersion)
					throw new OptimisticConcurrencyException("Existing version for " + documentId + " is " + existingDocumentVersion +
					                                         " but update version is " + documentVersion);
			}

			var prep = documentId.HasValue ? JET_prep.Replace : JET_prep.Insert;
			using (var update = new Update(session.JetSesid, table.JetTableid,prep))
			{
				if(documentId == null)
				{
					documentId = GuidCombGenerator.Generate();
					Api.SetColumn(session.JetSesid, table.JetTableid, dictionary["id"],
						documentId.Value.ToByteArray());
				}

				Api.SetColumn(session.JetSesid, table.JetTableid, dictionary["document"],
					DocumentAsString(document), Encoding.UTF8);

				update.Save();

				documentVersion = (int)Api.RetrieveColumnAsInt32(session.JetSesid, table.JetTableid, dictionary["version"]);
			}


			return new DocumentId
			{
				Id = documentId.Value,
				Version = documentVersion
			};
		}

		private static string DocumentAsString(JToken document)
		{
			using (var stringWriter = new StringWriter())
			using (var writer = new JsonTextWriter(stringWriter))
			{
				document.WriteTo(writer);
				writer.Flush();
				return stringWriter.GetStringBuilder().ToString();
			}
		}

		public JObject Find(Guid key)
		{
			var document = FindInternal(key);
			if(document==null)
				return null;
			return document.Payload;
		}

		private Document FindInternal(Guid key)
		{
			JET_DBID dbid;

			Api.JetAttachDatabase(session.JetSesid, databaseName, AttachDatabaseGrbit.None);
			Api.JetOpenDatabase(session.JetSesid, databaseName, null, out dbid, OpenDatabaseGrbit.None);
			try
			{
				using (var table = new Table(session.JetSesid, dbid, "documents", OpenTableGrbit.None))
				{

					Api.JetSetCurrentIndex(session.JetSesid, table.JetTableid, "by_id");
					Api.MakeKey(session.JetSesid, table.JetTableid, key.ToByteArray(), MakeKeyGrbit.NewKey);

					if (Api.TrySeek(session.JetSesid, table.JetTableid, SeekGrbit.SeekEQ) == false)
						return null;

					var dictionary = Api.GetColumnDictionary(session.JetSesid, table.JetTableid);

					var version = (int)Api.RetrieveColumnAsInt32(session.JetSesid, table.JetTableid, dictionary["version"]);
					
					var columnAsString = Api.RetrieveColumnAsString(session.JetSesid, table.JetTableid, dictionary["document"],
					                                                Encoding.UTF8);
					return new Document(
						new DocumentId
						{
							Id = key,
							Version = version
						},
						JObject.Parse(columnAsString)
						);
				}
			}
			finally
			{
				Api.JetCloseDatabase(session.JetSesid, dbid, CloseDatabaseGrbit.None);
			}
		}

		/// <summary>
		///  Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
		/// </summary>
		/// <filterpriority>2</filterpriority>
		public void Dispose()
		{
		}

		public DocumentId[] AddOrUpdate(params Document[] documents)
		{
			JET_DBID dbid;

			var docIds = new List<DocumentId>();
			Api.JetAttachDatabase(session.JetSesid, databaseName, AttachDatabaseGrbit.None);
			Api.JetOpenDatabase(session.JetSesid, databaseName, null, out dbid, OpenDatabaseGrbit.None);
			using (var tx = new Transaction(session.JetSesid))
			using (var documentsTable = new Table(session.JetSesid, dbid, "documents", OpenTableGrbit.None))
			using (var toBeIndexedTable = new Table(session.JetSesid, dbid, "ToBeIndexed", OpenTableGrbit.None))
			{
				Api.JetSetCurrentIndex(session.JetSesid, documentsTable.JetTableid, "by_id");

				foreach (var document in documents)
				{
					var documentId = AddDocumentToDatabase(
						documentsTable, 
						document.Payload, 
						document.DocumentId.Id, 
						document.DocumentId.Version);

					AddToIndexedQueue(toBeIndexedTable,documentId);

					docIds.Add(documentId);
				}
				tx.Commit(CommitTransactionGrbit.None);
			}

			var array = docIds.ToArray();

			NeedIndexing(databaseName, array);

			return array;
		}

		private void AddToIndexedQueue(Table table, DocumentId id)
		{
			using(var update = new Update(session.JetSesid, table.JetTableid,JET_prep.Insert))
			{
				var dictionary = Api.GetColumnDictionary(session.JetSesid, table.JetTableid);

				Api.SetColumn(session.JetSesid, table.JetTableid, dictionary["id"], id.Id.Value.ToByteArray());

				Api.SetColumn(session.JetSesid, table.JetTableid, dictionary["version"], id.Version);

				update.Save();
			}
		}

		public Document Find(DocumentId id)
		{
			var doc = FindInternal(id.Id.Value);
			if(doc==null)
				return null;
			if(id.Version!=doc.DocumentId.Version)
				return null;
			return doc;
		}
	}
}