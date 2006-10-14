using Ayende.NHibernateQueryAnalyzer.Model;
using Ayende.NHibernateQueryAnalyzer.UserInterface.Commands;
using Ayende.NHibernateQueryAnalyzer.UserInterface.Interfaces;

namespace Ayende.NHibernateQueryAnalyzer.UserInterface.Presenters
{
	public class ProjectPresenter : IProjectPresenter
	{
		private readonly IMainPresenter mainPresenter;
		private readonly Project prj;
		private IProjectView view;

		public ProjectPresenter(Project prj, IMainPresenter mainPresenter)
		{
			this.mainPresenter = mainPresenter;
			this.prj = prj;
			this.view  = CreateView(mainPresenter);
		}

		protected virtual IProjectView CreateView(IMainPresenter mainPresenter)
		{
			return new ProjectView(this,mainPresenter.View);
		}

		public void BuildProject()
		{
			ICommand buildProject = new BuildProjectCommand(view, prj, mainPresenter.Repository);
			mainPresenter.EnqueueCommand(buildProject);
			view.DisplayProjectState(false, false);
			view.StartWait("Building project..", 15, 100);
		}

		public void EditProject()
		{
			if (!mainPresenter.CloseProjectChildren())
				return;
			prj.ResetProject();
			view.DisplayProjectState(true, false);
		}

		public bool SaveProjectAs()
		{
			string name = view.Title;
			string newName = view.Ask("Project name:", name);
			if (newName != null)
			{
				Project existingProject = mainPresenter.Repository.GetProjectByName(newName);
				if (existingProject != null)
				{
					if (view.AskYesNo("A project with the name '" + newName + "' already exists, are you sure you want to overwrite it?", "Overwrite project?"))
						mainPresenter.Repository.RemoveProject(existingProject);
					else
						return false;
				}
				view.Title = newName;
				view.HasChanges = false;
				prj.Name = newName;
				mainPresenter.Repository.SaveProject(prj);
				return true;
			}
			return false;
		}

		public void ProjectViewDisposed()
		{
			prj.Dispose();
			mainPresenter.Repository.RemoveFromCache(prj);
		}

		public Project Project
		{
			get { return prj; }
		}

		public bool SaveProject()
		{
			if(prj.Id==0)
				return SaveProjectAs();
			mainPresenter.Repository.SaveProject(prj);
			view.HasChanges = false;
			return true;
		}

		public IProjectView View
		{
			get { return view; }
		}

	}
}