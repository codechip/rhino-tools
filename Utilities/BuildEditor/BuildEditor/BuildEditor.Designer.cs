namespace BuildEditor
{
	partial class BuildEditor
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(BuildEditor));
			this.toolStrip1 = new System.Windows.Forms.ToolStrip();
			this.OpenToolBarItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripLabel1 = new System.Windows.Forms.ToolStripLabel();
			this.fileNameLabel = new System.Windows.Forms.ToolStripLabel();
			this.parametersTable = new System.Windows.Forms.TableLayoutPanel();
			this.label2 = new System.Windows.Forms.Label();
			this.label1 = new System.Windows.Forms.Label();
			this.saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
			this.splitter1 = new System.Windows.Forms.Splitter();
			this.panel1 = new System.Windows.Forms.Panel();
			this.exec = new System.Windows.Forms.Button();
			this.command = new System.Windows.Forms.TextBox();
			this.toolStrip1.SuspendLayout();
			this.parametersTable.SuspendLayout();
			this.panel1.SuspendLayout();
			this.SuspendLayout();
			// 
			// toolStrip1
			// 
			this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.OpenToolBarItem,
            this.toolStripLabel1,
            this.fileNameLabel});
			this.toolStrip1.Location = new System.Drawing.Point(0, 0);
			this.toolStrip1.Name = "toolStrip1";
			this.toolStrip1.Size = new System.Drawing.Size(1000, 25);
			this.toolStrip1.TabIndex = 0;
			this.toolStrip1.Text = "toolStrip1";
			// 
			// OpenToolBarItem
			// 
			this.OpenToolBarItem.Image = ((System.Drawing.Image)(resources.GetObject("OpenToolBarItem.Image")));
			this.OpenToolBarItem.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.OpenToolBarItem.Name = "OpenToolBarItem";
			this.OpenToolBarItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.O)));
			this.OpenToolBarItem.Size = new System.Drawing.Size(61, 25);
			this.OpenToolBarItem.Text = "&Open";
			this.OpenToolBarItem.Click += new System.EventHandler(this.OpenToolBarItem_Click);
			// 
			// toolStripLabel1
			// 
			this.toolStripLabel1.Name = "toolStripLabel1";
			this.toolStripLabel1.Size = new System.Drawing.Size(0, 22);
			// 
			// fileNameLabel
			// 
			this.fileNameLabel.Name = "fileNameLabel";
			this.fileNameLabel.Size = new System.Drawing.Size(0, 22);
			// 
			// parametersTable
			// 
			this.parametersTable.AutoScroll = true;
			this.parametersTable.AutoSize = true;
			this.parametersTable.ColumnCount = 2;
			this.parametersTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 200F));
			this.parametersTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.parametersTable.Controls.Add(this.label2, 1, 0);
			this.parametersTable.Controls.Add(this.label1, 0, 0);
			this.parametersTable.Dock = System.Windows.Forms.DockStyle.Fill;
			this.parametersTable.Location = new System.Drawing.Point(0, 25);
			this.parametersTable.Name = "parametersTable";
			this.parametersTable.RowCount = 2;
			this.parametersTable.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.parametersTable.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
			this.parametersTable.Size = new System.Drawing.Size(1000, 416);
			this.parametersTable.TabIndex = 1;
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(203, 0);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(93, 13);
			this.label2.TabIndex = 1;
			this.label2.Text = "Parameter Values:";
			this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(3, 0);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(94, 13);
			this.label1.TabIndex = 0;
			this.label1.Text = "Parameter Names:";
			// 
			// splitter1
			// 
			this.splitter1.BackColor = System.Drawing.SystemColors.Control;
			this.splitter1.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.splitter1.Location = new System.Drawing.Point(0, 441);
			this.splitter1.Name = "splitter1";
			this.splitter1.Size = new System.Drawing.Size(1000, 12);
			this.splitter1.TabIndex = 3;
			this.splitter1.TabStop = false;
			// 
			// panel1
			// 
			this.panel1.Controls.Add(this.exec);
			this.panel1.Controls.Add(this.command);
			this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.panel1.Location = new System.Drawing.Point(0, 453);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(1000, 45);
			this.panel1.TabIndex = 4;
			// 
			// exec
			// 
			this.exec.Dock = System.Windows.Forms.DockStyle.Right;
			this.exec.Location = new System.Drawing.Point(925, 0);
			this.exec.Name = "exec";
			this.exec.Size = new System.Drawing.Size(75, 45);
			this.exec.TabIndex = 4;
			this.exec.Text = "Execute";
			this.exec.UseVisualStyleBackColor = true;
			this.exec.Click += new System.EventHandler(this.exec_Click);
			// 
			// command
			// 
			this.command.Dock = System.Windows.Forms.DockStyle.Fill;
			this.command.Location = new System.Drawing.Point(0, 0);
			this.command.Multiline = true;
			this.command.Name = "command";
			this.command.ReadOnly = true;
			this.command.Size = new System.Drawing.Size(1000, 45);
			this.command.TabIndex = 3;
			// 
			// BuildEditor
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.AutoScroll = true;
			this.ClientSize = new System.Drawing.Size(1000, 498);
			this.Controls.Add(this.parametersTable);
			this.Controls.Add(this.splitter1);
			this.Controls.Add(this.toolStrip1);
			this.Controls.Add(this.panel1);
			this.Name = "BuildEditor";
			this.Text = "Build Editor";
			this.Shown += new System.EventHandler(this.BuildEditor_Shown);
			this.toolStrip1.ResumeLayout(false);
			this.toolStrip1.PerformLayout();
			this.parametersTable.ResumeLayout(false);
			this.parametersTable.PerformLayout();
			this.panel1.ResumeLayout(false);
			this.panel1.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.ToolStrip toolStrip1;
		private System.Windows.Forms.ToolStripMenuItem OpenToolBarItem;
		private System.Windows.Forms.TableLayoutPanel parametersTable;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.SaveFileDialog saveFileDialog1;
		private System.Windows.Forms.Splitter splitter1;
		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.Button exec;
		private System.Windows.Forms.TextBox command;
		private System.Windows.Forms.ToolStripLabel toolStripLabel1;
		private System.Windows.Forms.ToolStripLabel fileNameLabel;
	}
}

