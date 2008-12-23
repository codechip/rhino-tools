namespace Rhino.DivanDB
{
	using System;
	using DSL;
	using Newtonsoft.Json.Linq;

	public interface IDivanDatabase : IDisposable
	{
		IndexerBase[] Views { get; }

		void AddView(string name, string function);

		IDivanView OpenView(string name);

		Document Find(DocumentId id);

		JObject Find(Guid guid);

		DocumentId[] AddOrUpdate(params Document[] document);
	}
}