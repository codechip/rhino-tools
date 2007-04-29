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