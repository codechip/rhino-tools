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
using System.Windows.Forms;
using Ayende.NHibernateQueryAnalyzer.Model;

namespace Ayende.NHibernateQueryAnalyzer.UserInterface.Controls
{
	/// <summary>
	/// Summary description for OpenQueryForm.
	/// </summary>
	public class OpenQueryForm : Form
	{
		private readonly Project prj;
		private Panel queriesPanel;
		private TextBox queryText;
		private Splitter querySplitter;
		private ListBox queryList;
		private Button cancel;
		private Button ok;

		/// <summary>
		/// Required designer variable.
		/// </summary>
		private Container components = null;

		public OpenQueryForm(Project prj)
		{
			this.prj = prj;
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			BindQueries();
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
			this.queriesPanel = new System.Windows.Forms.Panel();
			this.queryText = new System.Windows.Forms.TextBox();
			this.querySplitter = new System.Windows.Forms.Splitter();
			this.queryList = new System.Windows.Forms.ListBox();
			this.cancel = new System.Windows.Forms.Button();
			this.ok = new System.Windows.Forms.Button();
			this.queriesPanel.SuspendLayout();
			this.SuspendLayout();
			// 
			// queriesPanel
			// 
			this.queriesPanel.Controls.Add(this.queryText);
			this.queriesPanel.Controls.Add(this.querySplitter);
			this.queriesPanel.Controls.Add(this.queryList);
			this.queriesPanel.Dock = System.Windows.Forms.DockStyle.Top;
			this.queriesPanel.Location = new System.Drawing.Point(0, 0);
			this.queriesPanel.Name = "queriesPanel";
			this.queriesPanel.Size = new System.Drawing.Size(400, 216);
			this.queriesPanel.TabIndex = 0;
			// 
			// queryText
			// 
			this.queryText.Dock = System.Windows.Forms.DockStyle.Fill;
			this.queryText.Location = new System.Drawing.Point(123, 0);
			this.queryText.Multiline = true;
			this.queryText.Name = "queryText";
			this.queryText.ReadOnly = true;
			this.queryText.Size = new System.Drawing.Size(277, 216);
			this.queryText.TabIndex = 5;
			this.queryText.Text = "";
			// 
			// querySplitter
			// 
			this.querySplitter.Location = new System.Drawing.Point(120, 0);
			this.querySplitter.Name = "querySplitter";
			this.querySplitter.Size = new System.Drawing.Size(3, 216);
			this.querySplitter.TabIndex = 4;
			this.querySplitter.TabStop = false;
			// 
			// queryList
			// 
			this.queryList.Dock = System.Windows.Forms.DockStyle.Left;
			this.queryList.Location = new System.Drawing.Point(0, 0);
			this.queryList.Name = "queryList";
			this.queryList.Size = new System.Drawing.Size(120, 212);
			this.queryList.TabIndex = 3;
			this.queryList.DoubleClick += new System.EventHandler(this.queryList_DoubleClick);
			this.queryList.SelectedIndexChanged += new System.EventHandler(this.queryList_SelectedIndexChanged);
			// 
			// cancel
			// 
			this.cancel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.cancel.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.cancel.Location = new System.Drawing.Point(312, 224);
			this.cancel.Name = "cancel";
			this.cancel.Size = new System.Drawing.Size(75, 25);
			this.cancel.TabIndex = 11;
			this.cancel.Text = "&Cancel";
			this.cancel.Click += new System.EventHandler(this.cancel_Click);
			// 
			// ok
			// 
			this.ok.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.ok.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.ok.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.ok.Location = new System.Drawing.Point(16, 224);
			this.ok.Name = "ok";
			this.ok.Size = new System.Drawing.Size(75, 25);
			this.ok.TabIndex = 9;
			this.ok.Text = "&Ok";
			this.ok.Click += new System.EventHandler(this.ok_Click);
			// 
			// OpenQueryForm
			// 
			this.AcceptButton = this.ok;
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.CancelButton = this.cancel;
			this.ClientSize = new System.Drawing.Size(400, 254);
			this.Controls.Add(this.cancel);
			this.Controls.Add(this.ok);
			this.Controls.Add(this.queriesPanel);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "OpenQueryForm";
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Open Query";
			this.queriesPanel.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private void BindQueries()
		{
			foreach (Query query in prj.Queries)
			{
				queryList.Items.Add(query.Name);
			}
		}

		private void queryList_SelectedIndexChanged(object sender, EventArgs e)
		{
			Query q = prj.GetQueryWithName(queryList.SelectedItem.ToString());
			if(q!=null)
				queryText.Text = q.Text;
		}

		private void ok_Click(object sender, EventArgs e)
		{
			if(queryList.SelectedItem==null)
			{
				DialogResult = DialogResult.None;
				return;
			}
			DialogResult = DialogResult.OK;
			this.Close();
		}

		private void queryList_DoubleClick(object sender, EventArgs e)
		{
			if (queryList.SelectedItem != null)
			{
				DialogResult = DialogResult.OK;
				this.Close();
			}
		}

		private void cancel_Click(object sender, System.EventArgs e)
		{
			this.Close();
		}

		public Query SelectedQuery
		{
			get { return prj.GetQueryWithName(queryList.SelectedItem.ToString()); }
		}
	}
}