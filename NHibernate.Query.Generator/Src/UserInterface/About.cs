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