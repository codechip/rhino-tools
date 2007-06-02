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
using System.ComponentModel;
using System.IO;
using System.Windows.Forms;
using Ayende.NHibernateQueryAnalyzer.Core.Model;
using Ayende.NHibernateQueryAnalyzer.UserInterface.Interfaces;

namespace Ayende.NHibernateQueryAnalyzer.UserInterface
{
	/// <summary>
	/// Summary description for Configuration.
	/// </summary>
	public class ProjectView : NQADocument, IProjectView
	{
		private ListView filesList;
		private ColumnHeader fileName;
		private ColumnHeader filePath;

		/// <summary>
		/// Required designer variable.
		/// </summary>
		private Container components = null;

		private OpenFileDialog addFilesDlg;

		private readonly IProjectPresenter presenter;
		private System.Windows.Forms.Panel bottomPanel;
		private System.Windows.Forms.Button editProject;
		private System.Windows.Forms.Button buildProject;
		private System.Windows.Forms.Button removeFile;
		private System.Windows.Forms.Button addFile;
		public bool isEditable = true;

		public ProjectView(IProjectPresenter presenter,IMainView parentView) : base(parentView)
		{
			this.presenter = presenter;

			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
			BindProject();

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
			this.presenter.ProjectViewDisposed();
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.filesList = new System.Windows.Forms.ListView();
			this.fileName = new System.Windows.Forms.ColumnHeader();
			this.filePath = new System.Windows.Forms.ColumnHeader();
			this.addFilesDlg = new System.Windows.Forms.OpenFileDialog();
			this.bottomPanel = new System.Windows.Forms.Panel();
			this.editProject = new System.Windows.Forms.Button();
			this.buildProject = new System.Windows.Forms.Button();
			this.removeFile = new System.Windows.Forms.Button();
			this.addFile = new System.Windows.Forms.Button();
			this.bottomPanel.SuspendLayout();
			this.SuspendLayout();
			// 
			// filesList
			// 
			this.filesList.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.filesList.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.fileName,
            this.filePath});
			this.filesList.Dock = System.Windows.Forms.DockStyle.Fill;
			this.filesList.FullRowSelect = true;
			this.filesList.GridLines = true;
			this.filesList.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
			this.filesList.Location = new System.Drawing.Point(0, 0);
			this.filesList.MultiSelect = false;
			this.filesList.Name = "filesList";
			this.filesList.Size = new System.Drawing.Size(624, 278);
			this.filesList.TabIndex = 0;
			this.filesList.UseCompatibleStateImageBehavior = false;
			this.filesList.View = System.Windows.Forms.View.Details;
			this.filesList.SelectedIndexChanged += new System.EventHandler(this.filesList_SelectedIndexChanged);
			// 
			// fileName
			// 
			this.fileName.Text = "File Name:";
			this.fileName.Width = 150;
			// 
			// filePath
			// 
			this.filePath.Text = "Path";
			this.filePath.Width = 350;
			// 
			// addFilesDlg
			// 
			this.addFilesDlg.Filter = "All Supported Files | *.hbm.xml; *.cfg.xml; *.exe; *.dll; *.config; ";
			this.addFilesDlg.Multiselect = true;
			this.addFilesDlg.Title = "Add File(s)...";
			// 
			// bottomPanel
			// 
			this.bottomPanel.Controls.Add(this.editProject);
			this.bottomPanel.Controls.Add(this.buildProject);
			this.bottomPanel.Controls.Add(this.removeFile);
			this.bottomPanel.Controls.Add(this.addFile);
			this.bottomPanel.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.bottomPanel.Location = new System.Drawing.Point(0, 278);
			this.bottomPanel.Name = "bottomPanel";
			this.bottomPanel.Size = new System.Drawing.Size(624, 48);
			this.bottomPanel.TabIndex = 1;
			// 
			// editProject
			// 
			this.editProject.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.editProject.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.editProject.Location = new System.Drawing.Point(416, 12);
			this.editProject.Name = "editProject";
			this.editProject.Size = new System.Drawing.Size(88, 23);
			this.editProject.TabIndex = 14;
			this.editProject.Text = "Edit Project";
			this.editProject.Visible = false;
			this.editProject.Click += new System.EventHandler(this.editProject_Click);
			// 
			// buildProject
			// 
			this.buildProject.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.buildProject.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.buildProject.Location = new System.Drawing.Point(520, 12);
			this.buildProject.Name = "buildProject";
			this.buildProject.Size = new System.Drawing.Size(88, 23);
			this.buildProject.TabIndex = 13;
			this.buildProject.Text = "&Build Project";
			this.buildProject.Click += new System.EventHandler(this.buildProject_Click);
			// 
			// removeFile
			// 
			this.removeFile.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.removeFile.Enabled = false;
			this.removeFile.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.removeFile.Location = new System.Drawing.Point(136, 12);
			this.removeFile.Name = "removeFile";
			this.removeFile.Size = new System.Drawing.Size(120, 23);
			this.removeFile.TabIndex = 12;
			this.removeFile.Text = "&Remove File";
			this.removeFile.Click += new System.EventHandler(this.removeFile_Click);
			// 
			// addFile
			// 
			this.addFile.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.addFile.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.addFile.Location = new System.Drawing.Point(16, 12);
			this.addFile.Name = "addFile";
			this.addFile.Size = new System.Drawing.Size(104, 23);
			this.addFile.TabIndex = 11;
			this.addFile.Text = "&Add File(s)...";
			this.addFile.Click += new System.EventHandler(this.addFile_Click);
			// 
			// ProjectView
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(624, 326);
			this.Controls.Add(this.filesList);
			this.Controls.Add(this.bottomPanel);
			this.Name = "ProjectView";
			this.Text = "Configuration";
			this.Title = "Configuration";
			this.bottomPanel.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private void BindProject()
		{
			Title = presenter.Project.Name;
			BindProjectFileList();
		}

		private void BindProjectFileList()
		{
			filesList.BeginUpdate();
			filesList.Items.Clear();
			ListViewItem lvi;
			foreach (string fileName in presenter.Project.Files)
			{
				lvi = new ListViewItem(new string[] {Path.GetFileName(fileName), fileName});
				filesList.Items.Add(lvi);
			}
			filesList.EndUpdate();
		}

		private void filesList_SelectedIndexChanged(object sender, EventArgs e)
		{
			removeFile.Enabled = isEditable && filesList.SelectedIndices.Count > 0;
		}

		private void removeFile_Click(object sender, EventArgs e)
		{
			if(filesList.SelectedItems.Count==0)
				return;
			string fileName = filesList.SelectedItems[0].SubItems[1].Text;
			presenter.Project.RemoveFile(fileName);
			HasChanges = true;
			removeFile.Enabled = false;//disable the button as there is nothing now selected.
			BindProjectFileList();
		}

		private void addFile_Click(object sender, EventArgs e)
		{
			if (addFilesDlg.ShowDialog(this) == DialogResult.OK)
			{
				foreach (string file in addFilesDlg.FileNames)
				{
					presenter.Project.AddFile(file);
				}
				HasChanges = true;
				BindProjectFileList();
			}
		}

		private void buildProject_Click(object sender, EventArgs e)
		{
			presenter.BuildProject();
		}

		public void DisplayProjectState(bool isEditable, bool allowUserEdit)
		{
			this.isEditable = isEditable;
			removeFile.Enabled = isEditable;
			addFile.Enabled = isEditable;
			editProject.Visible = allowUserEdit;
			buildProject.Enabled = isEditable;
		}

		public IProjectPresenter ProjectPresenter
		{
			get { return presenter; }
		}

		public override bool Save()
		{
			return presenter.SaveProject();	
		}

		private void editProject_Click(object sender, EventArgs e)
		{
			presenter.EditProject();
		}

		protected override void OnClosing(CancelEventArgs e)
		{
			base.OnClosing(e);
		}

		public override bool SaveAs()
		{
			return presenter.SaveProjectAs();

		}


	}
}