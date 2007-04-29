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


using System.ComponentModel;
using System.Windows.Forms;
using Ayende.NHibernateQueryAnalyzer.UserInterface.Interfaces;

namespace Ayende.NHibernateQueryAnalyzer.UserInterface.Controls
{
	/// <summary>
	/// Summary description for Input.
	/// </summary>
	public class Input : Form
	{
		private Label question;
		private Button cancel;
		private Button ok;
		private TextBox answer;

		/// <summary>
		/// Required designer variable.
		/// </summary>
		private Container components = null;

		public Input()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

		}

		public string Answer
		{
			get { return answer.Text; }
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

		public string Ask(string message, string defaultAnswer, IView parent)
		{
			question.Text = message;
			answer.Text = defaultAnswer;
			answer.SelectAll();
			answer.Select();
			if (ShowDialog((IWin32Window) parent) == DialogResult.OK)
				return Answer;
			else
				return null;
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.question = new System.Windows.Forms.Label();
			this.cancel = new System.Windows.Forms.Button();
			this.ok = new System.Windows.Forms.Button();
			this.answer = new System.Windows.Forms.TextBox();
			this.SuspendLayout();
			// 
			// question
			// 
			this.question.Dock = System.Windows.Forms.DockStyle.Top;
			this.question.Location = new System.Drawing.Point(0, 0);
			this.question.Name = "question";
			this.question.Size = new System.Drawing.Size(384, 24);
			this.question.TabIndex = 0;
			// 
			// cancel
			// 
			this.cancel.Anchor = ((System.Windows.Forms.AnchorStyles) (((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
			this.cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.cancel.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.cancel.Location = new System.Drawing.Point(296, 50);
			this.cancel.Name = "cancel";
			this.cancel.Size = new System.Drawing.Size(75, 29);
			this.cancel.TabIndex = 7;
			this.cancel.Text = "Cancel";
			// 
			// ok
			// 
			this.ok.Anchor = ((System.Windows.Forms.AnchorStyles) (((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
			this.ok.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.ok.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.ok.Location = new System.Drawing.Point(16, 50);
			this.ok.Name = "ok";
			this.ok.Size = new System.Drawing.Size(75, 29);
			this.ok.TabIndex = 5;
			this.ok.Text = "Ok";
			// 
			// answer
			// 
			this.answer.Dock = System.Windows.Forms.DockStyle.Top;
			this.answer.Location = new System.Drawing.Point(0, 24);
			this.answer.Name = "answer";
			this.answer.Size = new System.Drawing.Size(384, 20);
			this.answer.TabIndex = 8;
			this.answer.Text = "";
			// 
			// Input
			// 
			this.AcceptButton = this.ok;
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.CancelButton = this.cancel;
			this.ClientSize = new System.Drawing.Size(384, 88);
			this.ControlBox = false;
			this.Controls.Add(this.answer);
			this.Controls.Add(this.cancel);
			this.Controls.Add(this.ok);
			this.Controls.Add(this.question);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.Name = "Input";
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Please Answer:";
			this.ResumeLayout(false);

		}

		#endregion
	}
}