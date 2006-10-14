using System;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using Ayende.NHibernateQueryAnalyzer.UserInterface.Interfaces;

namespace Ayende.NHibernateQueryAnalyzer.UserInterface.Controls
{
	/// <summary>
	/// Summary description for UnsavedFilesForm.
	/// </summary>
	public class UnsavedFilesForm : Form
	{
		private ListView filesList;
		private ColumnHeader filesColumnHeader;
		private Button yes;
		private Button no;
		private Button cancel;

		/// <summary>
		/// Required designer variable.
		/// </summary>
		private Container components = null;

		public IView[] Views
		{
			get { return views; }
			set { views = value; }
		}

		public IView[] SelectedView
		{
			get { return selectedView; }
			set { selectedView = value; }
		}

		private IView[] views, selectedView = new IView[0];
		private Label titleLabel;

		public UnsavedFilesForm()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

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
			this.filesList = new System.Windows.Forms.ListView();
			this.filesColumnHeader = new System.Windows.Forms.ColumnHeader();
			this.yes = new System.Windows.Forms.Button();
			this.titleLabel = new System.Windows.Forms.Label();
			this.no = new System.Windows.Forms.Button();
			this.cancel = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// filesList
			// 
			this.filesList.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
																						this.filesColumnHeader});
			this.filesList.Dock = System.Windows.Forms.DockStyle.Top;
			this.filesList.FullRowSelect = true;
			this.filesList.GridLines = true;
			this.filesList.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
			this.filesList.HideSelection = false;
			this.filesList.Location = new System.Drawing.Point(0, 23);
			this.filesList.Name = "filesList";
			this.filesList.Size = new System.Drawing.Size(304, 177);
			this.filesList.TabIndex = 2;
			this.filesList.View = System.Windows.Forms.View.Details;
			this.filesList.SelectedIndexChanged += new System.EventHandler(this.filesList_SelectedIndexChanged);
			// 
			// filesColumnHeader
			// 
			this.filesColumnHeader.Text = "Files:";
			this.filesColumnHeader.Width = 300;
			// 
			// yes
			// 
			this.yes.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
				| System.Windows.Forms.AnchorStyles.Left)));
			this.yes.DialogResult = System.Windows.Forms.DialogResult.Yes;
			this.yes.Enabled = false;
			this.yes.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.yes.Location = new System.Drawing.Point(8, 216);
			this.yes.Name = "yes";
			this.yes.Size = new System.Drawing.Size(75, 25);
			this.yes.TabIndex = 0;
			this.yes.Text = "&Yes";
			this.yes.Click += new System.EventHandler(this.Button_Click);
			// 
			// titleLabel
			// 
			this.titleLabel.Dock = System.Windows.Forms.DockStyle.Top;
			this.titleLabel.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.titleLabel.Location = new System.Drawing.Point(0, 0);
			this.titleLabel.Name = "titleLabel";
			this.titleLabel.Size = new System.Drawing.Size(304, 23);
			this.titleLabel.TabIndex = 1;
			this.titleLabel.Text = "Save changes to the following files?";
			// 
			// no
			// 
			this.no.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
				| System.Windows.Forms.AnchorStyles.Left)));
			this.no.DialogResult = System.Windows.Forms.DialogResult.No;
			this.no.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.no.Location = new System.Drawing.Point(96, 216);
			this.no.Name = "no";
			this.no.Size = new System.Drawing.Size(75, 25);
			this.no.TabIndex = 3;
			this.no.Text = "&No";
			this.no.Click += new System.EventHandler(this.Button_Click);
			// 
			// cancel
			// 
			this.cancel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.cancel.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.cancel.Location = new System.Drawing.Point(224, 216);
			this.cancel.Name = "cancel";
			this.cancel.Size = new System.Drawing.Size(75, 25);
			this.cancel.TabIndex = 4;
			this.cancel.Text = "&Cancel";
			this.cancel.Click += new System.EventHandler(this.Button_Click);
			// 
			// UnsavedFilesForm
			// 
			this.AcceptButton = this.yes;
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.CancelButton = this.cancel;
			this.ClientSize = new System.Drawing.Size(304, 256);
			this.ControlBox = false;
			this.Controls.Add(this.cancel);
			this.Controls.Add(this.no);
			this.Controls.Add(this.filesList);
			this.Controls.Add(this.yes);
			this.Controls.Add(this.titleLabel);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "UnsavedFilesForm";
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Save Files:";
			this.Closing += new System.ComponentModel.CancelEventHandler(this.UnsavedFilesForm_Closing);
			this.Load += new System.EventHandler(this.UnsavedFilesForm_Load);
			this.ResumeLayout(false);

		}

		#endregion

		private void UnsavedFilesForm_Load(object sender, EventArgs e)
		{
			DocumentsToList();
		}

		private void DocumentsToList()
		{
			filesList.BeginUpdate();
			filesList.Items.Clear();
			ListViewItem lvi;
			foreach (IView doc in views)
			{
				lvi = new ListViewItem(doc.Title);
				lvi.Tag = doc;
				filesList.Items.Add(lvi);
				lvi.Selected = true;
			}
			filesList.EndUpdate();
		}

		private void UnsavedFilesForm_Closing(object sender, CancelEventArgs e)
		{
			if (DialogResult == DialogResult.Yes)
				SelectedItemsInListToSelectedDocuments();
		}

		private void SelectedItemsInListToSelectedDocuments()
		{
			ArrayList selected = new ArrayList();
			foreach (ListViewItem item in filesList.SelectedItems)
			{
				selected.Add(item.Tag);
			}
			selectedView = (IView[]) selected.ToArray(typeof (IView));
		}

		private void Button_Click(object sender, EventArgs e)
		{
			Close();
		}

		private void filesList_SelectedIndexChanged(object sender, EventArgs e)
		{
			yes.Enabled = filesList.SelectedItems.Count > 0;
		}
	}
}