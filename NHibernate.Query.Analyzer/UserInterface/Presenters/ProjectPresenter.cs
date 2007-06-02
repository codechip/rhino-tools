#region license
// Copyright (c) 2005 - 2007 Ayende Rahien (ayende@ayende.com)
// All rights reserved.
// 
// Redistribution and use in source and binary forms, with or without modification,
// are permitted provided that the following conditions are met:
// 
//     * Redistributions of source code must retain the above copyright notice,
//     this list of conditions and the following disclaimer.
//     * Redistributions in binary form must reproduce the above copyright notice,
//     this list of conditions and the following disclaimer in the documentation
//     and/or other materials provided with the distribution.
//     * Neither the name of Ayende Rahien nor the names of its
//     contributors may be used to endorse or promote products derived from this
//     software without specific prior written permission.
// 
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND
// ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
// WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
// DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS BE LIABLE
// FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL
// DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR
// SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER
// CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY,
// OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF
// THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
#endregion


using Ayende.NHibernateQueryAnalyzer.Core.Model;
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
			view.StartWait("Building project..", 5, 1000);
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