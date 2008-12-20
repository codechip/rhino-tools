using System.Windows.Forms;

namespace Rhino.ServiceBus.Viewer
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
            System.Windows.Forms.TreeNode treeNode1 = new System.Windows.Forms.TreeNode("Subscriptions");
            System.Windows.Forms.TreeNode treeNode2 = new System.Windows.Forms.TreeNode("Messages");
            System.Windows.Forms.TreeNode treeNode3 = new System.Windows.Forms.TreeNode("Errors");
            this.Queues = new System.Windows.Forms.TreeView();
            this.splitter1 = new System.Windows.Forms.Splitter();
            this.QueueTree = new System.Windows.Forms.TreeView();
            this.splitter2 = new System.Windows.Forms.Splitter();
            this.MsgText = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // Queues
            // 
            this.Queues.Dock = System.Windows.Forms.DockStyle.Left;
            this.Queues.Location = new System.Drawing.Point(0, 0);
            this.Queues.Name = "Queues";
            this.Queues.Size = new System.Drawing.Size(308, 374);
            this.Queues.TabIndex = 0;
            this.Queues.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.Queues_AfterSelect);
            // 
            // splitter1
            // 
            this.splitter1.Location = new System.Drawing.Point(308, 0);
            this.splitter1.Name = "splitter1";
            this.splitter1.Size = new System.Drawing.Size(3, 374);
            this.splitter1.TabIndex = 1;
            this.splitter1.TabStop = false;
            // 
            // QueueTree
            // 
            this.QueueTree.Dock = System.Windows.Forms.DockStyle.Top;
            this.QueueTree.Location = new System.Drawing.Point(311, 0);
            this.QueueTree.Name = "QueueTree";
            treeNode1.Name = "Subscriptions";
            treeNode1.Text = "Subscriptions";
            treeNode2.Name = "Messages";
            treeNode2.Text = "Messages";
            treeNode3.Name = "Errors";
            treeNode3.Text = "Errors";
            this.QueueTree.Nodes.AddRange(new System.Windows.Forms.TreeNode[] {
            treeNode1,
            treeNode2,
            treeNode3});
            this.QueueTree.Size = new System.Drawing.Size(288, 138);
            this.QueueTree.TabIndex = 2;
            this.QueueTree.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.QueueTree_AfterSelect);
            // 
            // splitter2
            // 
            this.splitter2.Dock = System.Windows.Forms.DockStyle.Top;
            this.splitter2.Location = new System.Drawing.Point(311, 138);
            this.splitter2.Name = "splitter2";
            this.splitter2.Size = new System.Drawing.Size(288, 3);
            this.splitter2.TabIndex = 3;
            this.splitter2.TabStop = false;
            // 
            // MsgText
            // 
            this.MsgText.Dock = System.Windows.Forms.DockStyle.Fill;
            this.MsgText.Location = new System.Drawing.Point(311, 141);
            this.MsgText.Multiline = true;
            this.MsgText.Name = "MsgText";
            this.MsgText.Size = new System.Drawing.Size(288, 233);
            this.MsgText.TabIndex = 4;
            // 
            // MainView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(599, 374);
            this.Controls.Add(this.MsgText);
            this.Controls.Add(this.splitter2);
            this.Controls.Add(this.QueueTree);
            this.Controls.Add(this.splitter1);
            this.Controls.Add(this.Queues);
            this.Name = "MainView";
            this.Text = "MainView";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TreeView Queues;
        private System.Windows.Forms.Splitter splitter1;
        private TreeView QueueTree;
        private Splitter splitter2;
        private TextBox MsgText;
    }
}