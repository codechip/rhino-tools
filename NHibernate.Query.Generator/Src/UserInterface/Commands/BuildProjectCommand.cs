using System;
using System.Text;
using Ayende.NHibernateQueryAnalyzer.Model;
using Ayende.NHibernateQueryAnalyzer.UserInterface.Interfaces;

namespace Ayende.NHibernateQueryAnalyzer.UserInterface.Commands
{
	public class BuildProjectCommand : ICommand
	{
		private readonly IProjectsRepository repository;
		private readonly Project prj;
		public IProjectView view;

		public BuildProjectCommand(IProjectView view, Project prj, IProjectsRepository repository)
		{
			this.repository = repository;
			this.view = view;
			this.prj = prj;

		}

		public void Execute()
		{
			try
			{
				prj.BuildProject();
				view.ExecuteInUIThread(new JobDoneSuccess(BuildingProjectFinishedSuccessfully), prj);
			}
			catch (Exception ex)
			{
				view.ExecuteInUIThread(new JobFailed(BuildingProjectFailed), ex);
			}
		}

		public string Info
		{
			get
			{
				StringBuilder sb = new StringBuilder();
				sb.Append("Project: ").Append(prj.Name).Append("\r\n");
				sb.Append("Files:\r\n");
				foreach (string file in prj.Files)
				{
					sb.Append("\t").Append(file).Append("\r\n");
				}
				return sb.ToString();
			}
		}

		public string Name
		{
			get { return GetType().Name; }
		}

		#region Implementation Details

		private delegate void JobDoneSuccess(Project prj);

		private delegate void JobFailed(Exception ex);

		private void BuildingProjectFinishedSuccessfully(Project prj)
		{
			repository.SaveProject(prj);//to save the isBuildSuccessfully flag.
			view.DisplayProjectState(false, true);
			view.EndWait("Project was build successfully");
		}

		private void BuildingProjectFailed(Exception ex)
		{
			Exception prev = ex, e = ex.InnerException;
			StringBuilder exceptionString = new StringBuilder();
			exceptionString.Append(ex.Message).Append("\r\n");
			while(e!=null)
			{
				if(e.Message != prev.Message)
					exceptionString.Append(e.Message).Append("\r\n");
				prev = e;
				e = e.InnerException;
			}
			repository.SaveProject(prj);//to save the isBuildSuccessfully flag.
			view.EndWait(ex.Message);
			view.ShowError(exceptionString.ToString());
			view.DisplayProjectState(true, false);
		}

		#endregion
	}
}