namespace Rhino.DivanDB.DSL
{
	using System;
	using Boo.Lang;
	using Microsoft.Isam.Esent.Interop;

	public abstract class IndexerBase
	{
		protected IQuackFu doc;
		private Table table;


		protected abstract void map();

		public void Initialize(Table table)
		{
			this.table = table;
		}

		public void Map(Document document)
		{
			doc = new JsonAdapter(document);
			map();
		}

		protected void emit(object[] keys, object doc)
		{
			Console.WriteLine(keys);
			Console.WriteLine(doc);
		}

		protected void emit(object key, object doc)
		{
			emit(new[] {key}, doc);
		}

		public void Dispose()
		{
			if (table != null)
				table.Dispose();
		}
	}
}