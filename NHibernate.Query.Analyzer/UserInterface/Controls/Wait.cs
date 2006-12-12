using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace Ayende.NHibernateQueryAnalyzer.UserInterface.Controls
{
	/// <summary>
	/// Summary description for Wait.
	/// </summary>
	public class Wait : Form
	{
		private Label displayText;
		private ProgressBar progress;
		private System.ComponentModel.IContainer components;

		private Timer waitTimer;

		public Wait(string message, int checkInterval, int shouldWaitFor)
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
			displayText.Text = message;
			progress.Maximum = shouldWaitFor;
			progress.Step = checkInterval;
			this.waitTimer.Interval = checkInterval;
			this.waitTimer.Enabled = true;
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

		private void waitTimer_Tick(object sender, EventArgs e)
		{
			if (progress.Value == progress.Maximum)
				progress.Value = progress.Minimum;
			else
				progress.PerformStep();

		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			this.displayText = new System.Windows.Forms.Label();
			this.waitTimer = new System.Windows.Forms.Timer(this.components);
			this.progress = new System.Windows.Forms.ProgressBar();
			this.SuspendLayout();
			// 
			// displayText
			// 
			this.displayText.Dock = System.Windows.Forms.DockStyle.Top;
			this.displayText.Location = new System.Drawing.Point(0, 0);
			this.displayText.Name = "displayText";
			this.displayText.Size = new System.Drawing.Size(384, 32);
			this.displayText.TabIndex = 0;
			// 
			// waitTimer
			// 
			this.waitTimer.Tick += new System.EventHandler(this.waitTimer_Tick);
			// 
			// progress
			// 
			this.progress.Dock = System.Windows.Forms.DockStyle.Fill;
			this.progress.Location = new System.Drawing.Point(0, 32);
			this.progress.Name = "progress";
			this.progress.Size = new System.Drawing.Size(384, 22);
			this.progress.TabIndex = 1;
			// 
			// Wait
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(384, 54);
			this.ControlBox = false;
			this.Controls.Add(this.progress);
			this.Controls.Add(this.displayText);
			this.Name = "Wait";
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Working...";
			this.ResumeLayout(false);

		}

		#endregion
	}
}