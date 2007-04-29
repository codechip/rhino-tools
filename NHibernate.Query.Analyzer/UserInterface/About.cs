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
using System.Diagnostics;
using System.Windows.Forms;

namespace Ayende.NHibernateQueryAnalyzer.UserInterface
{
	/// <summary>
	/// Summary description for About.
	/// </summary>
	public class About : Form
	{
		private Button close;
		private PictureBox pictureBox1;
		private LinkLabel homePage;
		private LinkLabel nhiberante;
		private LinkLabel reportBug;

		/// <summary>
		/// Required designer variable.
		/// </summary>
		private Container components = null;

		public About()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			//
			// TODO: Add any constructor code after InitializeComponent call
			//
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
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof (About));
			this.close = new System.Windows.Forms.Button();
			this.pictureBox1 = new System.Windows.Forms.PictureBox();
			this.homePage = new System.Windows.Forms.LinkLabel();
			this.nhiberante = new System.Windows.Forms.LinkLabel();
			this.reportBug = new System.Windows.Forms.LinkLabel();
			this.SuspendLayout();
			// 
			// close
			// 
			this.close.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.close.Location = new System.Drawing.Point(320, 392);
			this.close.Name = "close";
			this.close.TabIndex = 0;
			this.close.Text = "Close";
			this.close.Click += new System.EventHandler(this.close_Click);
			// 
			// pictureBox1
			// 
			this.pictureBox1.Image = ((System.Drawing.Image) (resources.GetObject("pictureBox1.Image")));
			this.pictureBox1.Location = new System.Drawing.Point(8, 8);
			this.pictureBox1.Name = "pictureBox1";
			this.pictureBox1.Size = new System.Drawing.Size(400, 416);
			this.pictureBox1.TabIndex = 1;
			this.pictureBox1.TabStop = false;
			// 
			// homePage
			// 
			this.homePage.Location = new System.Drawing.Point(8, 16);
			this.homePage.Name = "homePage";
			this.homePage.Size = new System.Drawing.Size(112, 32);
			this.homePage.TabIndex = 2;
			this.homePage.TabStop = true;
			this.homePage.Text = "NHibernate Query Analyzer Homepage";
			this.homePage.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.homePage_LinkClicked);
			// 
			// nhiberante
			// 
			this.nhiberante.Location = new System.Drawing.Point(8, 384);
			this.nhiberante.Name = "nhiberante";
			this.nhiberante.Size = new System.Drawing.Size(120, 24);
			this.nhiberante.TabIndex = 3;
			this.nhiberante.TabStop = true;
			this.nhiberante.Text = "NHibernate Homepage";
			this.nhiberante.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.nhiberante_LinkClicked);
			// 
			// reportBug
			// 
			this.reportBug.Location = new System.Drawing.Point(320, 16);
			this.reportBug.Name = "reportBug";
			this.reportBug.Size = new System.Drawing.Size(72, 24);
			this.reportBug.TabIndex = 4;
			this.reportBug.TabStop = true;
			this.reportBug.Text = "Report a bug";
			this.reportBug.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.reportBug_LinkClicked);
			// 
			// About
			// 
			this.AcceptButton = this.close;
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.CancelButton = this.close;
			this.ClientSize = new System.Drawing.Size(408, 424);
			this.ControlBox = false;
			this.Controls.Add(this.reportBug);
			this.Controls.Add(this.nhiberante);
			this.Controls.Add(this.homePage);
			this.Controls.Add(this.close);
			this.Controls.Add(this.pictureBox1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
			this.Name = "About";
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "About NHiberante Query Analyzer";
			this.ResumeLayout(false);

		}

		#endregion

		private void close_Click(object sender, EventArgs e)
		{
			this.Close();
		}

		private void homePage_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			ProcessStartInfo psi = new ProcessStartInfo("http://www.ayende.com/projects/nhiberante-query-analyzer.aspx");
			Process.Start(psi);
		}

		private void nhiberante_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			ProcessStartInfo psi = new ProcessStartInfo("http://www.nhibernate.org");
			Process.Start(psi);
		}

		private void reportBug_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			ProcessStartInfo psi = new ProcessStartInfo("http://bugs.ayende.com/?do=newtask&project=3");
			Process.Start(psi);
		}
	}
}