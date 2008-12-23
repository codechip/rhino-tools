namespace Rhino.DivanDB.Impl
{
	using System;
	using System.Collections.Generic;
	using DSL;
	using Microsoft.Isam.Esent.Interop;

	public class DivanIndexer
	{
		private readonly Instance instance;

		private readonly LinkedList<Action> workToBeDone = new LinkedList<Action>();

		public DivanIndexer(Instance instance)
		{
			this.instance = instance;
		}

		public void AddWork(string database, DocumentId[] documentIds)
		{
			lock (workToBeDone)
			{
				workToBeDone.AddLast(() => { IndexDocuments(database, documentIds); });
			}
		}

		public bool DoWork()
		{
			Action value;
			lock (workToBeDone)
			{
				if (workToBeDone.First == null)
					return false;
				value = workToBeDone.First.Value;
				workToBeDone.RemoveFirst();
			}

			value();

			return true;
		}

		private void IndexDocuments(string database, IEnumerable<DocumentId> ids)
		{
			using (var session = new Session(instance.JetInstance))
			using (var db = new DivanDatabase(instance, session, database))
			{
				JET_DBID dbid;
				Api.JetOpenDatabase(session.JetSesid, database, "", out dbid, OpenDatabaseGrbit.None);
				try
				{
					var views = db.Views;
					foreach (var view in views)
					{
						var table = new Table(session.JetSesid, dbid, view.ToString(), OpenTableGrbit.None);
						view.Initialize(table);
					}
					try
					{
						foreach (var id in ids)
						{
							var doc = db.Find(id);
							if (doc == null)
								continue;

							foreach (var view in views)
							{
								try
								{
									view.Map(doc);
								}
								catch (Exception e)
								{
									Console.WriteLine(e);
								}
							}
						}
					}
					finally
					{
						foreach (var view in views)
						{
							view.Dispose();
						}
					}
				}
				finally
				{
					Api.JetCloseDatabase(session.JetSesid, dbid, CloseDatabaseGrbit.None);
				}
			}
		}
	}
}