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


using System;
using System.Collections;
using System.Threading;
using System.Windows.Forms;
using Ayende.NHibernateQueryAnalyzer.Model;
using Ayende.NHibernateQueryAnalyzer.UserInterface.Interfaces;
using Ayende.NHibernateQueryAnalyzer.Utilities;
using NHibernate.Mapping.Cfg;
using NHibernate.Mapping.Hbm;

namespace Ayende.NHibernateQueryAnalyzer.UserInterface.Presenters
{
	/// <summary>
	/// Summary description for MainPresenter.
	/// </summary>				 
	public class MainPresenter : IMainPresenter
	{
		protected IQueue queue;
		protected readonly IProjectsRepository repository;
		private Thread workThread;
		private ThreadedCommandExecutioner executioner;
		private IMainView view;
		private Project currentProject;
		private IProjectView currentPrjView;
        private NHibernate.Cfg.Configuration nHibernateConfiguration;

		public IProjectsRepository Repository
		{
			get { return repository; }
		}


		public MainPresenter(IMainView view, IProjectsRepository repository)
		{
			this.view = view;
			this.repository = repository;
			this.queue = new ThreadSafeQueue();
			this.executioner = new ThreadedCommandExecutioner(queue);
			this.workThread = new Thread(new ThreadStart(executioner.Run));
			this.workThread.IsBackground = true;
			this.workThread.Start();

		}

		internal virtual int GetLastDefaultProjectNamePostfix()
		{
			IList list = repository.GetProjectsStartingWith(MainForm.DefaultNamePrefix);
			if (list.Count == 0)
				return 0;
			int max = 0;
			int parsedProjectPostFix;
			foreach (Project project in list)
			{
				parsedProjectPostFix = int.Parse(project.Name.Substring(MainForm.DefaultNamePrefix.Length));
				max = Math.Max(max, parsedProjectPostFix);
			}
			return max;
		}


		public virtual Project CreateNewProject()
		{
            int lastId = GetLastDefaultProjectNamePostfix();
		    int newId = lastId + 1;
			return repository.CreateProject(MainForm.DefaultNamePrefix + newId);
		}

        public virtual IProjectPresenter DisplayNewProject()
		{
			if (CloseProject() == false)
				return null;
			Project newProject = CreateNewProject();
			return DisplayProject(newProject);
		}

		protected virtual IProjectPresenter DisplayProject(Project project)
		{
			CurrentProject = project;
			IProjectPresenter projectPresenter = new ProjectPresenter(project, this);
			projectPresenter.View.Closed+=new EventHandler(currentPrjCfg_Closed);
			projectPresenter.View.TitleChanged+=new EventHandler(prjCfg_TitleChanged);
			CurrentProjectView = projectPresenter.View;
			view.Display(projectPresenter.View);
			return projectPresenter;
		}

		public virtual bool CloseProject()
		{
			if (ShowUnsavedDialog() == false)
				return false;
			foreach (IView doc in view.Documents)
			{
				doc.Close(false);
			}
			return true;
		}

		protected bool ShowUnsavedDialog()
		{
			ArrayList unSavedList = new ArrayList();
			foreach (IView doc in view.Documents)
			{
				if (doc.HasChanges)
					unSavedList.Add(doc);
			}
			IView[] toSave, unsaved = (IView[]) unSavedList.ToArray(typeof (IView));
			if (unsaved.Length > 0)
			{
				toSave = view.ShowUnsavedDialog(unsaved);
				if ( toSave == null)
					return false;
				foreach (IView document in toSave)
				{
					document.Save();
				}
			}
			return true;
		}
	  
		public virtual void SaveDocument()
		{
			IView doc = view.ActiveDocument;
			if (doc != null)
				doc.Save();
		}

		public IProjectView CurrentProjectView
		{
			get { return currentPrjView; }
			set { currentPrjView = value; }
		}

		public Project CurrentProject
		{
			get { return currentProject; }
			set { currentProject = value; }
		}

		public IProjectPresenter OpenProject()
		{
			if (CloseProject() == false)
				return null;
			Project prj = view.SelectExistingProject();
			if (prj != null)
			{
                return DisplayProject(prj);
			}
            return null;
		}

		/// <summary>
		/// Executes the active query.
		/// This assume that the active windows is a QueryForm
		/// </summary>
		public void ExecuteActiveQuery()
		{
			IQueryView qv = view.ActiveDocument as IQueryView;
            if (qv != null)
            {
                //qv.QueryPresenter.NHibernateConfiguration = 
                qv.QueryPresenter.ExecuteQuery();
            }
		}

		public IQueue Queue
		{
			get { return this.queue; }
		}

		public void AddNewQuery()
		{
			IQueryPresenter qp = new QueryPresenter(this);
            //qp.NHibernateConfiguration = this.nHibernateConfiguration;
			view.Display(qp.View);
		}

		public void OpenQuery()
		{
			Query q = view.SelectProjectQuery(CurrentProject);
			if (q != null)
			{
				IQueryPresenter qp = new QueryPresenter(this,q);
				view.Display(qp.View);
			}
		}

		public void SaveDocumentAs()
		{
			IView doc = view.ActiveDocument as IView;
			if (doc != null)
				doc.SaveAs();
		}

		public void CloseCurrentDocument()
		{
			IView doc = view.ActiveDocument as IView;
			if (doc != null)
				doc.Close(true);
		}

		public bool DeleteProject(Project project)
		{
			if (view.AskYesNo("Are you sure you want to delete project: " + project.Name, "Delete Project?"))
			{
				Repository.RemoveProject(project);
				return true;
			}
			return false;
		}

		public bool CloseProjectChildren()
		{
			if(ShowUnsavedDialog()==false)
				return false;
			foreach (IView document in view.Documents)
			{
				if ((document is IProjectView) == false)
					document.Close(false);
			}
			return true;
		}

		public void EnqueueCommand(ICommand command)
		{
			queue.Enqueue(command);
		}

		public void CreateNewHbmDocument()
		{
			CreateSchemaDocument(typeof(hibernatemapping), View.HbmSaveDlg);
		}

		private void CreateSchemaDocument(Type t, SaveFileDialog dlg)
		{
			SchemaEditorView editor = new SchemaEditorView(View,dlg,t);
			View.Display(editor);
		}

		public void CreateNewCfgDocument()
		{
			CreateSchemaDocument(typeof(hibernateconfiguration), View.CfgSaveDlg);
		}

		public void OpenHbmDocument(string name)
		{
			OpenSchemaDocument(typeof(hibernatemapping), name, View.HbmSaveDlg);
		}

		public void OpenCfgDocument(string name)
		{
			OpenSchemaDocument(typeof(hibernateconfiguration), name, View.CfgSaveDlg);
		}


		private void OpenSchemaDocument(Type t, string name, SaveFileDialog dlg)
		{
			try
			{
				SchemaEditorView editor = new SchemaEditorView(
					View,
					dlg,
					t,
					name);
				View.Display(editor);
			}
			catch(Exception e)
			{
				View.AddException(e);
			}
		}

		public IMainView View
		{
			get { return view; }
		}

		private void currentPrjCfg_Closed(object sender, EventArgs e)
		{
			view.Title = null;
			CurrentProject = null;
			CurrentProjectView = null;
		}

		private void prjCfg_TitleChanged(object sender, EventArgs e)
		{
			if(CurrentProjectView!=null)
				view.Title = CurrentProjectView.Title;
		}

        public NHibernate.Cfg.Configuration NHibernateConfiguration
        {
            get { return nHibernateConfiguration; }
            set { nHibernateConfiguration = value; }
        }

        public ICollection MappingFiles
        {
            get { return currentProject.MappingFiles; }
        }
	}
}
