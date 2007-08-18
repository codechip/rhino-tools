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
using System.ComponentModel;
using System.Diagnostics;
using System.Text;
using System.Windows.Forms;
using Ayende.NHibernateQueryAnalyzer.Core.Model;
using Ayende.NHibernateQueryAnalyzer.UserInterface.Controls;
using Ayende.NHibernateQueryAnalyzer.UserInterface.Interfaces;
using Ayende.NHibernateQueryAnalyzer.UserInterface.Presenters;
using log4net;
using WeifenLuo.WinFormsUI;
using NHibernate.Cfg;

namespace Ayende.NHibernateQueryAnalyzer.UserInterface
{
	/// <summary>
	/// Summary description for MainForm.
	/// </summary>
	public class MainForm : Form, IMainView
	{
		#region Variables
		private ILog logger = LogManager.GetLogger(typeof(MainForm));
		public event EventHandler TitleChanged;
		private IMainPresenter presenter;

		private MenuItem fileMenu;
		private MenuItem fileMenu_Exit;
		private MenuItem helpMenu;
		private MenuItem helpMenu_About;
		private MenuItem fileMenu_Splitter;
		private MainMenu mainMenu;
		private DockPanel dockingZone;
		private MenuItem projectMenu_New;
		private MenuItem projectMenu_New_Hbm;
		private MenuItem fileMenu_SaveDocument;
		private MenuItem queryMenu;
		private MenuItem queryMenu_Execute;
		private StatusBar mainStatus;
		
		private StatusBarPanel messagesPanel;
		private StatusBarPanel progressPanel;
		private MenuItem queryMenu_OpenQuery;
		private MenuItem queryMenu_Splitter;
		private MenuItem fileMenu_New_Project;
		private MenuItem fileMenu_OpenProject;
		private MenuItem fileMenu_New_Query;
		private MenuItem fileMenu_SaveAs;
		private MenuItem helpMenu_OnlineHelp;
		private MenuItem windowMenu;
		private MenuItem windowMenu_NextWin;
		private MenuItem windowMenu_PrevWin;
		private MenuItem windowMenu_Splitter;
		private MenuItem windowMenu_CloseWin;
		private IProjectView currentPrjCfgView;
		private Wait wait;
		public IProjectsRepository repository;
		private System.Windows.Forms.MenuItem projectMenu_New_Cfg;
		private System.Windows.Forms.SaveFileDialog hbmSaveDlg;
		private System.Windows.Forms.OpenFileDialog hbmOpenDlg;
		private System.Windows.Forms.MenuItem menuItem2;
		private System.Windows.Forms.MenuItem fileMenu_OpenHbm;
		private System.Windows.Forms.MenuItem fileMenu_OpenCfg;
		private System.Windows.Forms.SaveFileDialog cfgSaveDlg;
		private System.Windows.Forms.OpenFileDialog cfgOpenDlg;
		private System.Windows.Forms.MenuItem help_ReportBug;
		public const string DefaultNamePrefix = "New Project #";

		#endregion

		#region c'tors

		private MainForm()
		{
			InitializeComponent();
		}

		public MainForm(IProjectsRepository repository) : this()
		{
			this.repository = repository;
			this.presenter = new MainPresenter(this, repository);
			
		}

		public MainForm(IMainPresenter presenter) : this()
		{
			this.presenter = presenter;
		}

		#endregion

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.mainMenu = new System.Windows.Forms.MainMenu();
			this.fileMenu = new System.Windows.Forms.MenuItem();
			this.projectMenu_New = new System.Windows.Forms.MenuItem();
			this.fileMenu_New_Project = new System.Windows.Forms.MenuItem();
			this.projectMenu_New_Hbm = new System.Windows.Forms.MenuItem();
			this.projectMenu_New_Cfg = new System.Windows.Forms.MenuItem();
			this.fileMenu_New_Query = new System.Windows.Forms.MenuItem();
			this.menuItem2 = new System.Windows.Forms.MenuItem();
			this.fileMenu_OpenProject = new System.Windows.Forms.MenuItem();
			this.fileMenu_OpenHbm = new System.Windows.Forms.MenuItem();
			this.fileMenu_OpenCfg = new System.Windows.Forms.MenuItem();
			this.fileMenu_SaveDocument = new System.Windows.Forms.MenuItem();
			this.fileMenu_SaveAs = new System.Windows.Forms.MenuItem();
			this.fileMenu_Splitter = new System.Windows.Forms.MenuItem();
			this.fileMenu_Exit = new System.Windows.Forms.MenuItem();
			this.queryMenu = new System.Windows.Forms.MenuItem();
			this.queryMenu_Execute = new System.Windows.Forms.MenuItem();
			this.queryMenu_Splitter = new System.Windows.Forms.MenuItem();
			this.queryMenu_OpenQuery = new System.Windows.Forms.MenuItem();
			this.windowMenu = new System.Windows.Forms.MenuItem();
			this.windowMenu_NextWin = new System.Windows.Forms.MenuItem();
			this.windowMenu_PrevWin = new System.Windows.Forms.MenuItem();
			this.windowMenu_Splitter = new System.Windows.Forms.MenuItem();
			this.windowMenu_CloseWin = new System.Windows.Forms.MenuItem();
			this.helpMenu = new System.Windows.Forms.MenuItem();
			this.helpMenu_OnlineHelp = new System.Windows.Forms.MenuItem();
			this.help_ReportBug = new System.Windows.Forms.MenuItem();
			this.helpMenu_About = new System.Windows.Forms.MenuItem();
			this.dockingZone = new WeifenLuo.WinFormsUI.DockPanel();
			this.mainStatus = new System.Windows.Forms.StatusBar();
			this.messagesPanel = new System.Windows.Forms.StatusBarPanel();
			this.progressPanel = new System.Windows.Forms.StatusBarPanel();
			this.hbmSaveDlg = new System.Windows.Forms.SaveFileDialog();
			this.hbmOpenDlg = new System.Windows.Forms.OpenFileDialog();
			this.cfgSaveDlg = new System.Windows.Forms.SaveFileDialog();
			this.cfgOpenDlg = new System.Windows.Forms.OpenFileDialog();
			((System.ComponentModel.ISupportInitialize)(this.messagesPanel)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.progressPanel)).BeginInit();
			this.SuspendLayout();
			// 
			// mainMenu
			// 
			this.mainMenu.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																					 this.fileMenu,
																					 this.queryMenu,
																					 this.windowMenu,
																					 this.helpMenu});
			// 
			// fileMenu
			// 
			this.fileMenu.Index = 0;
			this.fileMenu.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																					 this.projectMenu_New,
																					 this.menuItem2,
																					 this.fileMenu_SaveDocument,
																					 this.fileMenu_SaveAs,
																					 this.fileMenu_Splitter,
																					 this.fileMenu_Exit});
			this.fileMenu.Text = "&File";
			this.fileMenu.Popup += new System.EventHandler(this.fileMenu_Popup);
			// 
			// projectMenu_New
			// 
			this.projectMenu_New.Index = 0;
			this.projectMenu_New.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																							this.fileMenu_New_Project,
																							this.projectMenu_New_Hbm,
																							this.projectMenu_New_Cfg,
																							this.fileMenu_New_Query});
			this.projectMenu_New.Text = "&New";
			this.projectMenu_New.Popup += new System.EventHandler(this.projectMenu_New_Popup);
			// 
			// fileMenu_New_Project
			// 
			this.fileMenu_New_Project.Index = 0;
			this.fileMenu_New_Project.Shortcut = System.Windows.Forms.Shortcut.CtrlN;
			this.fileMenu_New_Project.Text = "&Project";
			this.fileMenu_New_Project.Click += new System.EventHandler(this.fileMenu_NewProject_Click);
			// 
			// projectMenu_New_Hbm
			// 
			this.projectMenu_New_Hbm.Index = 1;
			this.projectMenu_New_Hbm.Shortcut = System.Windows.Forms.Shortcut.CtrlM;
			this.projectMenu_New_Hbm.Text = "&Mapping (hbm.xml)";
			this.projectMenu_New_Hbm.Click += new System.EventHandler(this.projectMenu_New_Hbm_Click);
			// 
			// projectMenu_New_Cfg
			// 
			this.projectMenu_New_Cfg.Index = 2;
			this.projectMenu_New_Cfg.Text = "&Configuration (cfg.xml)";
			this.projectMenu_New_Cfg.Click += new System.EventHandler(this.projectMenu_New_Cfg_Click);
			// 
			// fileMenu_New_Query
			// 
			this.fileMenu_New_Query.Index = 3;
			this.fileMenu_New_Query.Shortcut = System.Windows.Forms.Shortcut.CtrlQ;
			this.fileMenu_New_Query.Text = "&Query";
			this.fileMenu_New_Query.Click += new System.EventHandler(this.fileMenu_New_Query_Click);
			// 
			// menuItem2
			// 
			this.menuItem2.Index = 1;
			this.menuItem2.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																					  this.fileMenu_OpenProject,
																					  this.fileMenu_OpenHbm,
																					  this.fileMenu_OpenCfg});
			this.menuItem2.Text = "&Open";
			// 
			// fileMenu_OpenProject
			// 
			this.fileMenu_OpenProject.Index = 0;
			this.fileMenu_OpenProject.Shortcut = System.Windows.Forms.Shortcut.CtrlO;
			this.fileMenu_OpenProject.Text = "Open &Project...";
			this.fileMenu_OpenProject.Click += new System.EventHandler(this.fileMenu_OpenProject_Click);
			// 
			// fileMenu_OpenHbm
			// 
			this.fileMenu_OpenHbm.Index = 1;
			this.fileMenu_OpenHbm.Shortcut = System.Windows.Forms.Shortcut.CtrlShiftO;
			this.fileMenu_OpenHbm.Text = "Open &Mapping...";
			this.fileMenu_OpenHbm.Click += new System.EventHandler(this.fileMenu_OpenHbm_Click);
			// 
			// fileMenu_OpenCfg
			// 
			this.fileMenu_OpenCfg.Index = 2;
			this.fileMenu_OpenCfg.Text = "Open &Configuration...";
			this.fileMenu_OpenCfg.Click += new System.EventHandler(this.fileMenu_OpenCfg_Click);
			// 
			// fileMenu_SaveDocument
			// 
			this.fileMenu_SaveDocument.Index = 2;
			this.fileMenu_SaveDocument.Shortcut = System.Windows.Forms.Shortcut.CtrlS;
			this.fileMenu_SaveDocument.Text = "&Save";
			this.fileMenu_SaveDocument.Click += new System.EventHandler(this.fileMenu_SaveDocument_Click);
			// 
			// fileMenu_SaveAs
			// 
			this.fileMenu_SaveAs.Index = 3;
			this.fileMenu_SaveAs.Shortcut = System.Windows.Forms.Shortcut.CtrlL;
			this.fileMenu_SaveAs.Text = "Save &As";
			this.fileMenu_SaveAs.Click += new System.EventHandler(this.fileMenu_SaveAs_Click);
			// 
			// fileMenu_Splitter
			// 
			this.fileMenu_Splitter.Index = 4;
			this.fileMenu_Splitter.Text = "-";
			// 
			// fileMenu_Exit
			// 
			this.fileMenu_Exit.Index = 5;
			this.fileMenu_Exit.Text = "E&xit";
			this.fileMenu_Exit.Click += new System.EventHandler(this.fileMenu_Exit_Click);
			// 
			// queryMenu
			// 
			this.queryMenu.Index = 1;
			this.queryMenu.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																					  this.queryMenu_Execute,
																					  this.queryMenu_Splitter,
																					  this.queryMenu_OpenQuery});
			this.queryMenu.Text = "&Query";
			this.queryMenu.Popup += new System.EventHandler(this.queryMenu_Popup);
			// 
			// queryMenu_Execute
			// 
			this.queryMenu_Execute.Index = 0;
			this.queryMenu_Execute.Shortcut = System.Windows.Forms.Shortcut.F5;
			this.queryMenu_Execute.Text = "&Execute Query";
			this.queryMenu_Execute.Click += new System.EventHandler(this.queryMenu_Execute_Click);
			// 
			// queryMenu_Splitter
			// 
			this.queryMenu_Splitter.Index = 1;
			this.queryMenu_Splitter.Text = "-";
			// 
			// queryMenu_OpenQuery
			// 
			this.queryMenu_OpenQuery.Index = 2;
			this.queryMenu_OpenQuery.Shortcut = System.Windows.Forms.Shortcut.CtrlShiftQ;
			this.queryMenu_OpenQuery.Text = "&Open Query";
			this.queryMenu_OpenQuery.Click += new System.EventHandler(this.queryMenu_OpenQuery_Click);
			// 
			// windowMenu
			// 
			this.windowMenu.Index = 2;
			this.windowMenu.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																					   this.windowMenu_NextWin,
																					   this.windowMenu_PrevWin,
																					   this.windowMenu_Splitter,
																					   this.windowMenu_CloseWin});
			this.windowMenu.Text = "&Window";
			// 
			// windowMenu_NextWin
			// 
			this.windowMenu_NextWin.Index = 0;
			this.windowMenu_NextWin.Shortcut = System.Windows.Forms.Shortcut.CtrlF6;
			this.windowMenu_NextWin.Text = "&Next Window";
			this.windowMenu_NextWin.Click += new System.EventHandler(this.windowMenu_NextWin_Click);
			// 
			// windowMenu_PrevWin
			// 
			this.windowMenu_PrevWin.Index = 1;
			this.windowMenu_PrevWin.Shortcut = System.Windows.Forms.Shortcut.CtrlShiftF6;
			this.windowMenu_PrevWin.Text = "&Previous Window";
			this.windowMenu_PrevWin.Click += new System.EventHandler(this.windowMenu_PrevWin_Click);
			// 
			// windowMenu_Splitter
			// 
			this.windowMenu_Splitter.Index = 2;
			this.windowMenu_Splitter.Text = "-";
			// 
			// windowMenu_CloseWin
			// 
			this.windowMenu_CloseWin.Index = 3;
			this.windowMenu_CloseWin.Shortcut = System.Windows.Forms.Shortcut.CtrlF4;
			this.windowMenu_CloseWin.Text = "&Close Window";
			this.windowMenu_CloseWin.Click += new System.EventHandler(this.windowMenu_CloseWin_Click);
			// 
			// helpMenu
			// 
			this.helpMenu.Index = 3;
			this.helpMenu.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																					 this.helpMenu_OnlineHelp,
																					 this.help_ReportBug,
																					 this.helpMenu_About});
			this.helpMenu.Text = "&Help";
			// 
			// helpMenu_OnlineHelp
			// 
			this.helpMenu_OnlineHelp.Index = 0;
			this.helpMenu_OnlineHelp.Text = "&Online Help";
			this.helpMenu_OnlineHelp.Click += new System.EventHandler(this.helpMenu_OnlineHelp_Click);
			// 
			// help_ReportBug
			// 
			this.help_ReportBug.Index = 1;
			this.help_ReportBug.Text = "Report a &Bug";
			this.help_ReportBug.Click += new System.EventHandler(this.help_ReportBug_Click);
			// 
			// helpMenu_About
			// 
			this.helpMenu_About.Index = 2;
			this.helpMenu_About.Shortcut = System.Windows.Forms.Shortcut.F1;
			this.helpMenu_About.Text = "&About";
			this.helpMenu_About.Click += new System.EventHandler(this.helpMenu_About_Click);
			// 
			// dockingZone
			// 
			this.dockingZone.ActiveAutoHideContent = null;
			this.dockingZone.BackColor = System.Drawing.SystemColors.Control;
			this.dockingZone.Dock = System.Windows.Forms.DockStyle.Fill;
			this.dockingZone.Font = new System.Drawing.Font("Tahoma", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.World);
			this.dockingZone.Location = new System.Drawing.Point(0, 0);
			this.dockingZone.Name = "dockingZone";
			this.dockingZone.Size = new System.Drawing.Size(792, 523);
			this.dockingZone.TabIndex = 0;
			// 
			// mainStatus
			// 
			this.mainStatus.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.mainStatus.Location = new System.Drawing.Point(0, 523);
			this.mainStatus.Name = "mainStatus";
			this.mainStatus.Panels.AddRange(new System.Windows.Forms.StatusBarPanel[] {
																						  this.messagesPanel,
																						  this.progressPanel});
			this.mainStatus.ShowPanels = true;
			this.mainStatus.Size = new System.Drawing.Size(792, 22);
			this.mainStatus.TabIndex = 1;
			// 
			// messagesPanel
			// 
			this.messagesPanel.AutoSize = System.Windows.Forms.StatusBarPanelAutoSize.Contents;
			this.messagesPanel.Text = "NHibernate Query Analyzer";
			this.messagesPanel.Width = 153;
			// 
			// progressPanel
			// 
			this.progressPanel.Alignment = System.Windows.Forms.HorizontalAlignment.Right;
			this.progressPanel.AutoSize = System.Windows.Forms.StatusBarPanelAutoSize.Spring;
			this.progressPanel.MinWidth = 0;
			this.progressPanel.Style = System.Windows.Forms.StatusBarPanelStyle.OwnerDraw;
			this.progressPanel.Width = 623;
			// 
			// hbmSaveDlg
			// 
			this.hbmSaveDlg.AddExtension = false;
			this.hbmSaveDlg.DefaultExt = "hbm.xml";
			this.hbmSaveDlg.Filter = "NHibernate Mapping Files|*.hbm.xml";
			this.hbmSaveDlg.Title = "Save the mapping file:";
			// 
			// hbmOpenDlg
			// 
			this.hbmOpenDlg.DefaultExt = "hbm.xml";
			this.hbmOpenDlg.Filter = "NHibernate Mapping Files|*.hbm.xml";
			// 
			// cfgSaveDlg
			// 
			this.cfgSaveDlg.AddExtension = false;
			this.cfgSaveDlg.DefaultExt = "cfg.xml";
			this.cfgSaveDlg.Filter = "NHibernate Configuration|*.cfg.xml";
			// 
			// cfgOpenDlg
			// 
			this.cfgOpenDlg.DefaultExt = "cfg.xml";
			this.cfgOpenDlg.Filter = "NHiberante Configuration|*.cfg.xml";
			// 
			// MainForm
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.BackColor = System.Drawing.SystemColors.Control;
			this.ClientSize = new System.Drawing.Size(792, 545);
			this.Controls.Add(this.dockingZone);
			this.Controls.Add(this.mainStatus);
			this.Menu = this.mainMenu;
			this.Name = "MainForm";
			this.Text = "MainForm";
			this.Closing += new System.ComponentModel.CancelEventHandler(this.MainForm_Closing);
			((System.ComponentModel.ISupportInitialize)(this.messagesPanel)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.progressPanel)).EndInit();
			this.ResumeLayout(false);

		}

		#endregion

		#region Event Handlers

		private void fileMenu_Exit_Click(object sender, EventArgs e)
		{
			Close();
		}

		private void fileMenu_NewProject_Click(object sender, EventArgs e)
		{
			presenter.DisplayNewProject();
		}


		private void fileMenu_OpenProject_Click(object sender, EventArgs e)
		{
			presenter.OpenProject();
		}

		private void fileMenu_SaveDocument_Click(object sender, EventArgs e)
		{
			try
			{
				presenter.SaveDocument();
			}
			catch(Exception ex)
			{
				MessageBox.Show("Can't save document because: "+ex.Message, "Document was not saved",MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		private void queryMenu_Popup(object sender, EventArgs e)
		{
			QueryForm qf = dockingZone.ActiveDocument as QueryForm;
			bool queryWindowIsActive = qf != null && qf.CanExecuteQuery;
			bool projectIsBuilt = ProjectIsBuilt();
			queryMenu_Execute.Enabled = queryWindowIsActive;
			queryMenu_OpenQuery.Enabled = projectIsBuilt;
		}

		private bool ProjectIsBuilt()
		{
			foreach (DockContent content in dockingZone.Documents)
			{
				if (content is IProjectView)
				{
					return ((IProjectView)content).ProjectPresenter.Project.IsProjectBuilt;
				}
			}
			return false;
		}

		public IMainPresenter MainPresenter
		{
			get { return presenter; }
		}

		public Query SelectProjectQuery(Project prj)
		{
			using (OpenQueryForm oqf = new OpenQueryForm(prj))
			{
				if (oqf.ShowDialog(this) == DialogResult.OK)
					return oqf.SelectedQuery;
				else
					return null;
			}
		}

		private void queryMenu_Execute_Click(object sender, EventArgs e)
		{
			try
			{
				presenter.ExecuteActiveQuery();
			}
			catch(Exception ex)
			{
				MessageBox.Show("Can't execute the currect query because: " +ex.Message,"Query error!",MessageBoxButtons.OK,MessageBoxIcon.Error);
			}
		}

		private void fileMenu_New_Query_Click(object sender, EventArgs e)
		{
			presenter.AddNewQuery();
		}

		private void queryMenu_OpenQuery_Click(object sender, EventArgs e)
		{
			presenter.OpenQuery();
		}

		private void windowMenu_NextWin_Click(object sender, EventArgs e)
		{
			if (dockingZone.Documents.Length > 0)
			{
				int index = Array.IndexOf(dockingZone.Documents, dockingZone.ActiveDocument);
				if (index + 1 < dockingZone.Documents.Length)
					dockingZone.Documents[index + 1].DockHandler.Activate();
				else
					dockingZone.Documents[0].DockHandler.Activate();
			}
		}

		private void windowMenu_PrevWin_Click(object sender, EventArgs e)
		{
			if (dockingZone.Documents.Length > 0)
			{
				int index = Array.IndexOf(dockingZone.Documents, dockingZone.ActiveDocument);
				if (index - 1 >= 0)
					dockingZone.Documents[index - 1].DockHandler.Activate();
				else
					dockingZone.Documents[dockingZone.Documents.Length - 1].DockHandler.Activate();
			}
		}

		private void fileMenu_SaveAs_Click(object sender, EventArgs e)
		{
			try
			{
				presenter.SaveDocumentAs();
			}
			catch(Exception ex)
			{
				MessageBox.Show("Can't save document because: "+ex.Message, "Document was not saved",MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		
		}

		private void projectMenu_New_Popup(object sender, EventArgs e)
		{
			fileMenu_New_Query.Enabled = CurrentProject != null && CurrentProject.IsProjectBuilt;
		}

		private void windowMenu_CloseWin_Click(object sender, EventArgs e)
		{
			presenter.CloseCurrentDocument();
		}

		private void fileMenu_Popup(object sender, EventArgs e)
		{
			fileMenu_SaveAs.Enabled = dockingZone.ActiveDocument != null;
			fileMenu_SaveDocument.Enabled = dockingZone.ActiveDocument != null;
		}

		private void MainForm_Closing(object sender, CancelEventArgs e)
		{
			e.Cancel = !presenter.CloseProject();
		}

		private void projectMenu_New_Hbm_Click(object sender, System.EventArgs e)
		{
			presenter.CreateNewHbmDocument();
		}

		private void projectMenu_New_Cfg_Click(object sender, System.EventArgs e)
		{
			presenter.CreateNewCfgDocument();
		}

		private void fileMenu_OpenHbm_Click(object sender, System.EventArgs e)
		{
			try
			{
				if(hbmOpenDlg.ShowDialog(this)!=DialogResult.Cancel)
				{
					presenter.OpenHbmDocument(hbmOpenDlg.FileName);
				}
			}
			catch(Exception ex)
			{
				MessageBox.Show("Can't open document because: "+ex.Message, "Problem openning document",MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		private void fileMenu_OpenCfg_Click(object sender, System.EventArgs e)
		{
			try
			{
				if(cfgOpenDlg.ShowDialog(this)!=DialogResult.Cancel)
				{
					presenter.OpenCfgDocument(cfgOpenDlg.FileName);
				}
			}
			catch(Exception ex)
			{
				MessageBox.Show("Can't open document because: "+ex.Message, "Problem openning document",MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		private void helpMenu_OnlineHelp_Click(object sender, System.EventArgs e)
		{
			ProcessStartInfo psi = new	ProcessStartInfo("http://www.ayende.com/projects/nhibernate-query-analyzer.aspx");
			Process.Start(psi);
		}

		private void help_ReportBug_Click(object sender, System.EventArgs e)
		{
            ProcessStartInfo psi = new ProcessStartInfo("http://groups.google.com/group/rhino-tools-dev");
			Process.Start(psi);		
		}

		private void helpMenu_About_Click(object sender, System.EventArgs e)
		{
			using(About about = new About())
				about.ShowDialog(this);
		}

		#endregion

		#region IMainView Implementation

		public Project CurrentProject
		{
			get { return presenter.CurrentProject; }
			set
			{
				presenter.CurrentProject = value;
				UpdateProjectName();
			}
		}

		public void UpdateProjectName()
		{
			if (presenter.CurrentProject!= null)
				Text = presenter.CurrentProject.Name + " - NHibernate Query Analyzer";
			else
				Text = "NHibernate Query Analyzer";
		}

		public IProjectView CurrentProjectView
		{
			get { return currentPrjCfgView; }
			set { currentPrjCfgView = value; }
		}

		public void Display(IView view)
		{
			NQADocument d = view as NQADocument;
			if(d!=null)
				d.Show(dockingZone,DockState.Document);
		}

		public SaveFileDialog HbmSaveDlg
		{
			get { return hbmSaveDlg; }
		}

		public SaveFileDialog CfgSaveDlg
		{
			get { return cfgSaveDlg; }
		}

		public IView ActiveDocument
		{
			get { return dockingZone.ActiveDocument as IView; }
		}

		public IEnumerable Documents
		{
			get { return dockingZone.Documents; }
		}

		public IView[] ShowUnsavedDialog(IView[] unsavedView)
		{
			using (UnsavedFilesForm uff = new UnsavedFilesForm())
			{
				uff.Views = unsavedView;
				if (uff.ShowDialog(this) == DialogResult.Cancel)
				{
					return null;
				}
				return uff.SelectedView;
			}
		}

		/// <summary>
		/// Starts the wait message by the UI. The view need to check every <c>checkInterval</c> 
		/// that the work was comleted (using <c>HasFinishedWork()</c> method).
		/// The work should finish in shouldWaitFor, but there is no gurantee about it.
		/// <c>EndWait</c> is called to end the wait.
		/// </summary>
		/// <param name="waitMessage">The Wait message.</param>
		/// <param name="checkInterval">Check interval.</param>
		/// <param name="shouldWaitFor">Should wait for.</param>
		public void StartWait(string waitMessage, int checkInterval, int shouldWaitFor)
		{
			wait = new Wait(waitMessage,checkInterval,shouldWaitFor);
			wait.ShowDialog(this);
		}

		public void EndWait(string endMessage)
		{
			wait.Close();
			wait.Dispose();
			wait = null;
			this.messagesPanel.Text = endMessage;
		}

		public void AddException(Exception ex)
		{
			ShowError(ex.ToString());
		}

		public void ShowError(string error)
		{
			MessageBox.Show(error, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
		}

		public void ExecuteInUIThread(Delegate d, params object[] parameters)
		{
			if (logger.IsDebugEnabled)
			{
				StringBuilder sb = new StringBuilder();
				sb.AppendFormat("Invoke Required: {0}, Executing method: {1} on {2}: ",InvokeRequired, d.Method.Name,  d.Method.DeclaringType.FullName);
				foreach (object parameter in parameters)
				{
					sb.Append(parameter != null ? parameter.ToString() : "null");
					sb.Append(", ");
				}
				logger.Debug(sb.ToString());
			}
			if (this.InvokeRequired)
			{
				Invoke(d, parameters);
			}
			else
			{
				d.DynamicInvoke(parameters);
			}
		}

		public IPresenter Presenter
		{
			get { return presenter; }
		}

		public bool AskYesNo(string question, string title)
		{
			return MessageBox.Show(this, question, title, MessageBoxButtons.YesNo) == DialogResult.Yes;
		}

		public string Ask(string question, string answer)
		{
			using (Input input = new Input())
			{
				return input.Ask(question, answer, this);
			}
		}

		public bool HasChanges
		{
			get { return true; }
			set
			{}
		}

		public bool Save()
		{
			return false;
		}

		public string Title
		{
			get { return Text; }
			set
			{
				if(value!=null)
					Text = "NHibernate Query Analyzer";
				else
					Text = value + " - NHibernate Query Analyzer";
				if(TitleChanged!=null)
					TitleChanged(this,EventArgs.Empty);
			}
		}

		public void Close(bool askToSave)
		{
			Close();
		}

		public bool SaveAs()
		{
			return true;
		}

		public Project SelectExistingProject()
		{
			using (OpenProjectForm opf = new OpenProjectForm(presenter))
			{
				if (opf.ShowDialog(this) == DialogResult.OK)
				{
					return opf.SelectedProject;
				}
				return null;
			}
		}

		#endregion

	}
}
