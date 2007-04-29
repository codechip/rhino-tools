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
	partial class MainView
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

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainView));
			this.menu = new System.Windows.Forms.MenuStrip();
			this.milestonesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.versionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.severityToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.priorityToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.componentToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.submitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.tabControl1 = new System.Windows.Forms.TabControl();
			this.tabPage1 = new System.Windows.Forms.TabPage();
			this.tabPage2 = new System.Windows.Forms.TabPage();
			this.panel3 = new System.Windows.Forms.Panel();
			this.snapShotImg = new System.Windows.Forms.PictureBox();
			this.panel2 = new System.Windows.Forms.Panel();
			this.button2 = new System.Windows.Forms.Button();
			this.snapShotComment = new System.Windows.Forms.TextBox();
			this.lblComment = new System.Windows.Forms.Label();
			this.drawing = new System.Windows.Forms.Button();
			this.clearImg = new System.Windows.Forms.Button();
			this.panel1 = new System.Windows.Forms.Panel();
			this.summary = new System.Windows.Forms.TextBox();
			this.label1 = new System.Windows.Forms.Label();
			this.initMenues = new System.ComponentModel.BackgroundWorker();
			this.progressTimer = new System.Windows.Forms.Timer(this.components);
			this.statusStrip1 = new System.Windows.Forms.StatusStrip();
			this.pleaseWait = new System.Windows.Forms.ToolStripStatusLabel();
			this.progressBar = new System.Windows.Forms.ToolStripProgressBar();
			this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
			this.label2 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.label4 = new System.Windows.Forms.Label();
			this.expected = new System.Windows.Forms.TextBox();
			this.actual = new System.Windows.Forms.TextBox();
			this.repreduce = new System.Windows.Forms.TextBox();
			this.menu.SuspendLayout();
			this.tabControl1.SuspendLayout();
			this.tabPage1.SuspendLayout();
			this.tabPage2.SuspendLayout();
			this.panel3.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.snapShotImg)).BeginInit();
			this.panel2.SuspendLayout();
			this.panel1.SuspendLayout();
			this.statusStrip1.SuspendLayout();
			this.tableLayoutPanel1.SuspendLayout();
			this.SuspendLayout();
			// 
			// menu
			// 
			this.menu.Enabled = false;
			this.menu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.milestonesToolStripMenuItem,
            this.versionToolStripMenuItem,
            this.severityToolStripMenuItem,
            this.priorityToolStripMenuItem,
            this.componentToolStripMenuItem,
            this.submitToolStripMenuItem});
			this.menu.Location = new System.Drawing.Point(0, 0);
			this.menu.Name = "menu";
			this.menu.Size = new System.Drawing.Size(792, 24);
			this.menu.TabIndex = 0;
			// 
			// milestonesToolStripMenuItem
			// 
			this.milestonesToolStripMenuItem.Name = "milestonesToolStripMenuItem";
			this.milestonesToolStripMenuItem.Size = new System.Drawing.Size(64, 20);
			this.milestonesToolStripMenuItem.Text = "&Milestone";
			// 
			// versionToolStripMenuItem
			// 
			this.versionToolStripMenuItem.Name = "versionToolStripMenuItem";
			this.versionToolStripMenuItem.Size = new System.Drawing.Size(54, 20);
			this.versionToolStripMenuItem.Text = "&Version";
			// 
			// severityToolStripMenuItem
			// 
			this.severityToolStripMenuItem.Name = "severityToolStripMenuItem";
			this.severityToolStripMenuItem.Size = new System.Drawing.Size(59, 20);
			this.severityToolStripMenuItem.Text = "S&everity";
			// 
			// priorityToolStripMenuItem
			// 
			this.priorityToolStripMenuItem.Name = "priorityToolStripMenuItem";
			this.priorityToolStripMenuItem.Size = new System.Drawing.Size(53, 20);
			this.priorityToolStripMenuItem.Text = "&Priority";
			// 
			// componentToolStripMenuItem
			// 
			this.componentToolStripMenuItem.Name = "componentToolStripMenuItem";
			this.componentToolStripMenuItem.Size = new System.Drawing.Size(74, 20);
			this.componentToolStripMenuItem.Text = "&Component";
			// 
			// submitToolStripMenuItem
			// 
			this.submitToolStripMenuItem.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
			this.submitToolStripMenuItem.Enabled = false;
			this.submitToolStripMenuItem.Name = "submitToolStripMenuItem";
			this.submitToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.S)));
			this.submitToolStripMenuItem.ShowShortcutKeys = false;
			this.submitToolStripMenuItem.Size = new System.Drawing.Size(51, 20);
			this.submitToolStripMenuItem.Text = "&Submit";
			this.submitToolStripMenuItem.Click += new System.EventHandler(this.submitToolStripMenuItem_Click);
			// 
			// tabControl1
			// 
			this.tabControl1.Controls.Add(this.tabPage1);
			this.tabControl1.Controls.Add(this.tabPage2);
			this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tabControl1.Location = new System.Drawing.Point(0, 55);
			this.tabControl1.Name = "tabControl1";
			this.tabControl1.SelectedIndex = 0;
			this.tabControl1.Size = new System.Drawing.Size(792, 489);
			this.tabControl1.TabIndex = 1;
			// 
			// tabPage1
			// 
			this.tabPage1.Controls.Add(this.tableLayoutPanel1);
			this.tabPage1.Location = new System.Drawing.Point(4, 22);
			this.tabPage1.Name = "tabPage1";
			this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
			this.tabPage1.Size = new System.Drawing.Size(784, 463);
			this.tabPage1.TabIndex = 0;
			this.tabPage1.Text = "Details";
			this.tabPage1.UseVisualStyleBackColor = true;
			// 
			// tabPage2
			// 
			this.tabPage2.Controls.Add(this.panel3);
			this.tabPage2.Controls.Add(this.panel2);
			this.tabPage2.Location = new System.Drawing.Point(4, 22);
			this.tabPage2.Name = "tabPage2";
			this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
			this.tabPage2.Size = new System.Drawing.Size(784, 485);
			this.tabPage2.TabIndex = 1;
			this.tabPage2.Text = "Screen Shot";
			this.tabPage2.UseVisualStyleBackColor = true;
			// 
			// panel3
			// 
			this.panel3.AutoScroll = true;
			this.panel3.Controls.Add(this.snapShotImg);
			this.panel3.Dock = System.Windows.Forms.DockStyle.Fill;
			this.panel3.Location = new System.Drawing.Point(3, 3);
			this.panel3.Name = "panel3";
			this.panel3.Size = new System.Drawing.Size(778, 443);
			this.panel3.TabIndex = 2;
			// 
			// snapShotImg
			// 
			this.snapShotImg.BackColor = System.Drawing.Color.Transparent;
			this.snapShotImg.Location = new System.Drawing.Point(-1, 0);
			this.snapShotImg.Name = "snapShotImg";
			this.snapShotImg.Size = new System.Drawing.Size(618, 323);
			this.snapShotImg.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
			this.snapShotImg.TabIndex = 1;
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
			this.panel2.Location = new System.Drawing.Point(3, 446);
			this.panel2.Name = "panel2";
			this.panel2.Size = new System.Drawing.Size(778, 36);
			this.panel2.TabIndex = 1;
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
			this.snapShotComment.Size = new System.Drawing.Size(567, 20);
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
			this.clearImg.Location = new System.Drawing.Point(698, 8);
			this.clearImg.Name = "clearImg";
			this.clearImg.Size = new System.Drawing.Size(75, 23);
			this.clearImg.TabIndex = 0;
			this.clearImg.Text = "Clear";
			this.clearImg.UseVisualStyleBackColor = true;
			this.clearImg.Click += new System.EventHandler(this.clearImg_Click);
			// 
			// panel1
			// 
			this.panel1.Controls.Add(this.summary);
			this.panel1.Controls.Add(this.label1);
			this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
			this.panel1.Location = new System.Drawing.Point(0, 24);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(792, 31);
			this.panel1.TabIndex = 2;
			// 
			// summary
			// 
			this.summary.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.summary.Location = new System.Drawing.Point(72, 5);
			this.summary.Name = "summary";
			this.summary.Size = new System.Drawing.Size(710, 20);
			this.summary.TabIndex = 1;
			this.summary.TextChanged += new System.EventHandler(this.Mandatory_TextChanged);
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(12, 10);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(53, 13);
			this.label1.TabIndex = 0;
			this.label1.Text = "Summary:";
			// 
			// initMenues
			// 
			this.initMenues.DoWork += new System.ComponentModel.DoWorkEventHandler(this.initMenues_DoWork);
			this.initMenues.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.initMenues_RunWorkerCompleted);
			// 
			// progressTimer
			// 
			this.progressTimer.Enabled = true;
			this.progressTimer.Interval = 250;
			this.progressTimer.Tick += new System.EventHandler(this.progressTimer_Tick);
			// 
			// statusStrip1
			// 
			this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.pleaseWait,
            this.progressBar});
			this.statusStrip1.Location = new System.Drawing.Point(0, 544);
			this.statusStrip1.Name = "statusStrip1";
			this.statusStrip1.Size = new System.Drawing.Size(792, 22);
			this.statusStrip1.TabIndex = 3;
			this.statusStrip1.Text = "statusStrip1";
			// 
			// pleaseWait
			// 
			this.pleaseWait.Name = "pleaseWait";
			this.pleaseWait.Size = new System.Drawing.Size(75, 17);
			this.pleaseWait.Text = "Please Wait...";
			// 
			// progressBar
			// 
			this.progressBar.Name = "progressBar";
			this.progressBar.Size = new System.Drawing.Size(400, 16);
			this.progressBar.Step = 3;
			// 
			// tableLayoutPanel1
			// 
			this.tableLayoutPanel1.ColumnCount = 1;
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tableLayoutPanel1.Controls.Add(this.repreduce, 0, 5);
			this.tableLayoutPanel1.Controls.Add(this.label2, 0, 0);
			this.tableLayoutPanel1.Controls.Add(this.label3, 0, 2);
			this.tableLayoutPanel1.Controls.Add(this.expected, 0, 1);
			this.tableLayoutPanel1.Controls.Add(this.label4, 0, 4);
			this.tableLayoutPanel1.Controls.Add(this.actual, 0, 3);
			this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tableLayoutPanel1.Location = new System.Drawing.Point(3, 3);
			this.tableLayoutPanel1.Name = "tableLayoutPanel1";
			this.tableLayoutPanel1.RowCount = 6;
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 18F));
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 18F));
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 18F));
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
			this.tableLayoutPanel1.Size = new System.Drawing.Size(778, 457);
			this.tableLayoutPanel1.TabIndex = 7;
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(3, 0);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(93, 13);
			this.label2.TabIndex = 3;
			this.label2.Text = "Expected Results:";
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(3, 152);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(78, 13);
			this.label3.TabIndex = 6;
			this.label3.Text = "Actual Results:";
			// 
			// label4
			// 
			this.label4.AutoSize = true;
			this.label4.Location = new System.Drawing.Point(3, 304);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(109, 13);
			this.label4.TabIndex = 7;
			this.label4.Text = "Steps To Reproduce:";
			// 
			// expected
			// 
			this.expected.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.expected.Dock = System.Windows.Forms.DockStyle.Fill;
			this.expected.Location = new System.Drawing.Point(3, 21);
			this.expected.Multiline = true;
			this.expected.Name = "expected";
			this.expected.Size = new System.Drawing.Size(772, 128);
			this.expected.TabIndex = 8;
			this.expected.TextChanged += new System.EventHandler(this.Mandatory_TextChanged);
			// 
			// actual
			// 
			this.actual.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.actual.Dock = System.Windows.Forms.DockStyle.Fill;
			this.actual.Location = new System.Drawing.Point(3, 173);
			this.actual.Multiline = true;
			this.actual.Name = "actual";
			this.actual.Size = new System.Drawing.Size(772, 128);
			this.actual.TabIndex = 9;
			this.actual.TextChanged += new System.EventHandler(this.Mandatory_TextChanged);
			// 
			// repreduce
			// 
			this.repreduce.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.repreduce.Dock = System.Windows.Forms.DockStyle.Fill;
			this.repreduce.Location = new System.Drawing.Point(3, 325);
			this.repreduce.Multiline = true;
			this.repreduce.Name = "repreduce";
			this.repreduce.Size = new System.Drawing.Size(772, 129);
			this.repreduce.TabIndex = 10;
			this.repreduce.TextChanged += new System.EventHandler(this.Mandatory_TextChanged);
			// 
			// MainView
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(792, 566);
			this.Controls.Add(this.tabControl1);
			this.Controls.Add(this.panel1);
			this.Controls.Add(this.menu);
			this.Controls.Add(this.statusStrip1);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MainMenuStrip = this.menu;
			this.Name = "MainView";
			this.Text = "Trac UI";
			this.Load += new System.EventHandler(this.MainView_Load);
			this.menu.ResumeLayout(false);
			this.menu.PerformLayout();
			this.tabControl1.ResumeLayout(false);
			this.tabPage1.ResumeLayout(false);
			this.tabPage2.ResumeLayout(false);
			this.panel3.ResumeLayout(false);
			this.panel3.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.snapShotImg)).EndInit();
			this.panel2.ResumeLayout(false);
			this.panel2.PerformLayout();
			this.panel1.ResumeLayout(false);
			this.panel1.PerformLayout();
			this.statusStrip1.ResumeLayout(false);
			this.statusStrip1.PerformLayout();
			this.tableLayoutPanel1.ResumeLayout(false);
			this.tableLayoutPanel1.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.MenuStrip menu;
		private System.Windows.Forms.ToolStripMenuItem milestonesToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem versionToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem severityToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem priorityToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem componentToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem submitToolStripMenuItem;
		private System.Windows.Forms.TabControl tabControl1;
		private System.Windows.Forms.TabPage tabPage1;
		private System.Windows.Forms.TabPage tabPage2;
		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.TextBox summary;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Panel panel2;
		private System.Windows.Forms.Button clearImg;
		private System.Windows.Forms.Button button2;
		private System.Windows.Forms.TextBox snapShotComment;
		private System.Windows.Forms.Label lblComment;
		private System.Windows.Forms.Button drawing;
		private System.ComponentModel.BackgroundWorker initMenues;
		private System.Windows.Forms.Timer progressTimer;
		private System.Windows.Forms.Panel panel3;
		private System.Windows.Forms.PictureBox snapShotImg;
		private System.Windows.Forms.StatusStrip statusStrip1;
		private System.Windows.Forms.ToolStripStatusLabel pleaseWait;
		private System.Windows.Forms.ToolStripProgressBar progressBar;
		private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.TextBox expected;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.TextBox repreduce;
		private System.Windows.Forms.TextBox actual;
	}
}