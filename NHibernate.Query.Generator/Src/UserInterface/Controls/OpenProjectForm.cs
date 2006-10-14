using System;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using Ayende.NHibernateQueryAnalyzer.Model;
using Ayende.NHibernateQueryAnalyzer.UserInterface.Interfaces;

namespace Ayende.NHibernateQueryAnalyzer.UserInterface.Controls
{
	/// <summary>
	/// Summary description for OpenProjectForm.
	/// </summary>
	public class OpenProjectForm : Form
	{
		private readonly IMainPresenter presenter;
		private readonly IProjectsRepository repository;
		private ColumnHeader prjNameCH;
		private ColumnHeader prjIdCh;
		private ListView projectsList;
		private ContextMenu projectsMenu;
		private MenuItem menu_DeleteProject;

		/// <summary>
		/// Required designer variable.
		/// </summary>
		private Container components = null;

		public OpenProjectForm(IMainPresenter presenter)
		{
			this.presenter = presenter;
			this.repository = presenter.Repository;
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			BindProjectList();
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				if (components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.projectsList = new System.Windows.Forms.ListView();
			this.prjNameCH = new System.Windows.Forms.ColumnHeader();
			this.prjIdCh = new System.Windows.Forms.ColumnHeader();
			this.projectsMenu = new System.Windows.Forms.ContextMenu();
			this.menu_DeleteProject = new System.Windows.Forms.MenuItem();
			this.SuspendLayout();
			// 
			// projectsList
			// 
			this.projectsList.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
				| System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.projectsList.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
																						   this.prjNameCH,
																						   this.prjIdCh});
			this.projectsList.ContextMenu = this.projectsMenu;
			this.projectsList.FullRowSelect = true;
			this.projectsList.GridLines = true;
			this.projectsList.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
			this.projectsList.HideSelection = false;
			this.projectsList.Location = new System.Drawing.Point(8, 8);
			this.projectsList.MultiSelect = false;
			this.projectsList.Name = "projectsList";
			this.projectsList.Size = new System.Drawing.Size(320, 266);
			this.projectsList.TabIndex = 0;
			this.projectsList.View = System.Windows.Forms.View.Details;
			this.projectsList.KeyDown += new System.Windows.Forms.KeyEventHandler(this.projectsList_KeyDown);
			this.projectsList.ItemActivate += new System.EventHandler(this.projectsList_ItemActivate);
			// 
			// prjNameCH
			// 
			this.prjNameCH.Text = "Name:";
			this.prjNameCH.Width = 250;
			// 
			// prjIdCh
			// 
			this.prjIdCh.Text = "ID:";
			// 
			// projectsMenu
			// 
			this.projectsMenu.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																						 this.menu_DeleteProject});
			this.projectsMenu.RightToLeft = System.Windows.Forms.RightToLeft.No;
			this.projectsMenu.Popup += new System.EventHandler(this.projectsMenu_Popup);
			// 
			// menu_DeleteProject
			// 
			this.menu_DeleteProject.Index = 0;
			this.menu_DeleteProject.Text = "&Delete Project";
			this.menu_DeleteProject.Click += new System.EventHandler(this.menu_DeleteProject_Click);
			// 
			// OpenProjectForm
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(338, 280);
			this.Controls.Add(this.projectsList);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "OpenProjectForm";
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Open Project";
			this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.OpenProjectForm_KeyDown);
			this.Load += new System.EventHandler(this.OpenProjectForm_Load);
			this.ResumeLayout(false);

		}

		#endregion

		private void BindProjectList()
		{
			projectsList.Items.Clear();
			IList list = repository.GetAllProejcts();
			foreach (Project project in list)
			{
				ListViewItem lvi = new ListViewItem(project.Name);
				lvi.SubItems.Add(project.Id.ToString());
				lvi.Tag = project;
				projectsList.Items.Add(lvi);
			}
		}

		private void projectsList_ItemActivate(object sender, EventArgs e)
		{
			DialogResult = DialogResult.OK;
			Close();
		}

		private void projectsList_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Escape || e.KeyCode == Keys.Delete)
				OpenProjectForm_KeyDown(sender, e);
		}

		private void OpenProjectForm_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Escape)
			{
				DialogResult = DialogResult.Cancel;
				Close();
			}
			else if (e.KeyCode == Keys.Delete && projectsList.SelectedItems.Count > 0)
			{
				DeleteSelectedProject();
			}
		}

		private void OpenProjectForm_Load(object sender, EventArgs e)
		{
			if (projectsList.Items.Count > 0)
				projectsList.Items[0].Selected = true;
		}

		private void projectsMenu_Popup(object sender, EventArgs e)
		{
			projectsMenu.MenuItems.Clear();
			if (projectsList.SelectedItems.Count > 0)
				projectsMenu.MenuItems.Add(menu_DeleteProject);
		}

		private void menu_DeleteProject_Click(object sender, EventArgs e)
		{
			DeleteSelectedProject();
		}

		private void DeleteSelectedProject()
		{
			Project prj = (Project) projectsList.SelectedItems[0].Tag;
			if (presenter.DeleteProject(prj))
				BindProjectList();
		}

		public Project SelectedProject
		{
			get
			{
				if (projectsList.SelectedItems.Count == 0)
					return null;
				return (Project) projectsList.SelectedItems[0].Tag;
			}
		}
	}
}