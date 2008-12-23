namespace Rhino.DivanDB.Impl
{
	using System;
	using System.Text;
	using Microsoft.Isam.Esent.Interop;
	using Newtonsoft.Json.Linq;

	public class DivanView : IDivanView
	{
		private readonly Session session;
		private readonly Table table;

		public DivanView(Session session, Table table)
		{
			this.session = session;
			this.table = table;
		}

		/// <summary>
		/// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
		/// </summary>
		/// <filterpriority>2</filterpriority>
		public void Dispose()
		{
			table.Dispose();
		}

		
	}
}