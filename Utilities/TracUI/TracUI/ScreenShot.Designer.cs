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


namespace TracUI
{
	partial class ScreenShot
	{
		/// <summary> 
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary> 
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Component Designer generated code

		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ScreenShot));
			this.snapShotImg = new System.Windows.Forms.PictureBox();
			this.panel2 = new System.Windows.Forms.Panel();
			this.button2 = new System.Windows.Forms.Button();
			this.snapShotComment = new System.Windows.Forms.TextBox();
			this.lblComment = new System.Windows.Forms.Label();
			this.drawing = new System.Windows.Forms.Button();
			this.clearImg = new System.Windows.Forms.Button();
			((System.ComponentModel.ISupportInitialize)(this.snapShotImg)).BeginInit();
			this.panel2.SuspendLayout();
			this.SuspendLayout();
			// 
			// snapShotImg
			// 
			this.snapShotImg.BackColor = System.Drawing.Color.Transparent;
			this.snapShotImg.Location = new System.Drawing.Point(0, 0);
			this.snapShotImg.Name = "snapShotImg";
			this.snapShotImg.Size = new System.Drawing.Size(618, 323);
			this.snapShotImg.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
			this.snapShotImg.TabIndex = 3;
			this.snapShotImg.TabStop = false;
			this.snapShotImg.MouseDown += new System.Windows.Forms.MouseEventHandler(this.snapShotImg_MouseDown);
			this.snapShotImg.MouseMove += new System.Windows.Forms.MouseEventHandler(this.snapShotImg_MouseMove);
			this.snapShotImg.Paint += new System.Windows.Forms.PaintEventHandler(this.snapShotImg_Paint);
			this.snapShotImg.MouseUp += new System.Windows.Forms.MouseEventHandler(this.snapShotImg_MouseUp);
			// 
			// panel2
			// 
			this.panel2.Controls.Add(this.button2);
			this.panel2.Controls.Add(this.snapShotComment);
			this.panel2.Controls.Add(this.lblComment);
			this.panel2.Controls.Add(this.drawing);
			this.panel2.Controls.Add(this.clearImg);
			this.panel2.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.panel2.Location = new System.Drawing.Point(0, 334);
			this.panel2.Name = "panel2";
			this.panel2.Size = new System.Drawing.Size(621, 36);
			this.panel2.TabIndex = 2;
			// 
			// button2
			// 
			this.button2.Image = ((System.Drawing.Image)(resources.GetObject("button2.Image")));
			this.button2.Location = new System.Drawing.Point(6, 7);
			this.button2.Name = "button2";
			this.button2.Size = new System.Drawing.Size(25, 25);
			this.button2.TabIndex = 4;
			this.button2.UseVisualStyleBackColor = true;
			this.button2.Click += new System.EventHandler(this.TakeSnapshot_Click);
			// 
			// snapShotComment
			// 
			this.snapShotComment.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.snapShotComment.Enabled = false;
			this.snapShotComment.Location = new System.Drawing.Point(125, 7);
			this.snapShotComment.Name = "snapShotComment";
			this.snapShotComment.Size = new System.Drawing.Size(410, 20);
			this.snapShotComment.TabIndex = 3;
			// 
			// lblComment
			// 
			this.lblComment.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.lblComment.AutoSize = true;
			this.lblComment.Location = new System.Drawing.Point(65, 10);
			this.lblComment.Name = "lblComment";
			this.lblComment.Size = new System.Drawing.Size(54, 13);
			this.lblComment.TabIndex = 2;
			this.lblComment.Text = "Comment:";
			// 
			// drawing
			// 
			this.drawing.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.drawing.Enabled = false;
			this.drawing.Image = ((System.Drawing.Image)(resources.GetObject("drawing.Image")));
			this.drawing.Location = new System.Drawing.Point(37, 8);
			this.drawing.Name = "drawing";
			this.drawing.Size = new System.Drawing.Size(25, 25);
			this.drawing.TabIndex = 3;
			this.drawing.UseVisualStyleBackColor = true;
			this.drawing.Click += new System.EventHandler(this.drawing_Click);
			// 
			// clearImg
			// 
			this.clearImg.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.clearImg.Location = new System.Drawing.Point(541, 8);
			this.clearImg.Name = "clearImg";
			this.clearImg.Size = new System.Drawing.Size(75, 23);
			this.clearImg.TabIndex = 0;
			this.clearImg.Text = "Clear";
			this.clearImg.UseVisualStyleBackColor = true;
			this.clearImg.Click += new System.EventHandler(this.clearImg_Click);
			// 
			// ScreenShot
			// 
			this.Controls.Add(this.snapShotImg);
			this.Controls.Add(this.panel2);
			this.Name = "ScreenShot";
			this.Size = new System.Drawing.Size(621, 370);
			((System.ComponentModel.ISupportInitialize)(this.snapShotImg)).EndInit();
			this.panel2.ResumeLayout(false);
			this.panel2.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.PictureBox snapShotImg;
		private System.Windows.Forms.Panel panel2;
		private System.Windows.Forms.Button button2;
		private System.Windows.Forms.TextBox snapShotComment;
		private System.Windows.Forms.Label lblComment;
		private System.Windows.Forms.Button drawing;
		private System.Windows.Forms.Button clearImg;

	}
}
