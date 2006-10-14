using System;
using System.Collections;
using System.Data;
using Ayende.NHibernateQueryAnalyzer.Model;
using Ayende.NHibernateQueryAnalyzer.UserInterface.Interfaces;
using Ayende.NHibernateQueryAnalyzer.Utilities;

namespace Ayende.NHibernateQueryAnalyzer.UserInterface.Commands
{
	public class ExecuteQueryCommand : ICommand
	{
		private IQueryView view;
		private Project prj;
		public string hqlQueryText;

		public ExecuteQueryCommand(IQueryView view, Project prj, string hqlQueryText, TypedParameter[] typedParameters)
		{
			this.view = view;
			this.prj = prj;
			this.hqlQueryText = hqlQueryText;
			this.typedParameters = typedParameters;
		}

		public TypedParameter[] typedParameters;

		#region ICommand Members

		public void Execute()
		{
			try
			{
				IList list = prj.RunHql(this.hqlQueryText, this.typedParameters);
				DataSet ds = prj.RunHqlAsRawSql(this.hqlQueryText, this.typedParameters);
				view.ExecuteInUIThread(new JobFinishedSuccessfully(QueryFinishedSuccessfully), list, ds);
			}
			catch (Exception ex)
			{
				view.ExecuteInUIThread(new JobFailed(QueryFailed), ex);
			}

		}

		public string Info
		{
			get { return "Query: " + hqlQueryText; }
		}

		public string Name
		{
			get { return GetType().Name; }
		}

		#endregion

		#region Implementation

		private delegate void JobFailed(Exception ex);

		private delegate void JobFinishedSuccessfully(IList list, DataSet ds);

		private void QueryFailed(Exception ex)
		{
			view.AddException(ex);
			view.EndWait("An exception occured executing query");
		}

		private void QueryFinishedSuccessfully(IList list, DataSet ds)
		{
			view.DisplayObjectGraph(list);
			view.DisplayDataSet(ds);
			view.ShowObjectGraph();
			view.EndWait("Finished executing query");
		}

		#endregion
	}
}