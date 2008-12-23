namespace Rhino.DivanDB.Tests
{
	using System;
	using System.Collections.Generic;
	using System.IO;
	using Impl;
	using Microsoft.Isam.Esent.Interop;
	using Newtonsoft.Json.Linq;
	using Xunit;

	public class DivanIndexerTests
	{
		public DivanIndexerTests()
		{
			File.Delete("test.divan");
		}

		[Fact]
		public void Will_ask_view_to_index_documents()
		{
			using (var instance = new Instance("test"))
			{
				instance.Init();

				var indexer = new DivanIndexer(instance);
				using (var sesion = new Session(instance.JetInstance))
				{
					var database = new DivanDatabase(instance, sesion, "test.divan");

					database.AddView("simple", @"
map:
	emit( (doc.id, doc.version), 'ayende' )
");

					database.NeedIndexing += indexer.AddWork;

					database.AddOrUpdate(
						new Document(
							new DocumentId(),
							JObject.Parse("{'name': 'oren', 'email': 'ayende@ayende.com'"))
						);

					indexer.DoWork();

					Assert.False(true,
						"need to think about how we can actually save the data in a format that is indexable with ESENT");
				}
			}
		}
	}
}