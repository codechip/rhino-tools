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
using System.IO;
using System.Windows.Forms;
using System.Xml;
using Ayende.NHibernateQueryAnalyzer.SchemaEditing;
using Ayende.NHibernateQueryAnalyzer.UserInterface.Controls;
using Ayende.NHibernateQueryAnalyzer.UserInterface.Interfaces;
using Ayende.NHibernateQueryAnalyzer.UserInterface.SchemaEditing;

namespace Ayende.NHibernateQueryAnalyzer.UserInterface
{
	/// <summary>
	/// Summary description for SchemaEditorView.
	/// </summary>
	public class SchemaEditorView : NQADocument
	{
		#region Variables
		private SaveFileDialog saveFileDlg;
		private string filename;
		private TabControl documentView;
		private TabPage graphView;
		private TabPage xmlView;
		private ContextMenuTreeView graphTree;
		private Splitter vSplit;
		//private ListView attributesList;  Replaced by property grid
		private TextBox xmlText;
		private IContainer components;
		private ColumnHeader attributeName;
		private ColumnHeader attributeValue;
		private ImageList images;
		private ContextMenu graphTreeContextMenu;
		private MenuItem addChild;
		private MenuItem removeChild;
		private MenuItem nothingToAdd;
		private MenuItem spacer;
		private SchemaEditor editor;
		/// <summary>
		/// Used by the context menu to hold the current node that the context menu is using
		/// </summary>
		private Hashtable menuItemToField;
		private Hashtable errorsDictionary = new Hashtable();
		private object objectFromEditedXml;
		private System.Windows.Forms.ToolTip errorTooltip;
		private System.Windows.Forms.ListView errorsList;
		private System.Windows.Forms.ColumnHeader SchemaErrors;
		private Furty.Windows.Forms.CollapsibleSplitter hSplitter;
        private PropertyGrid attributesPropertyGrid;
		private bool xmlTextEdited;

		#endregion
		
		#region Ugly Hack for Designer support

#if VS_DESIGNER_COULD_HANDLE_CLASSES_WITH_NON_DEFAULT_CONSTURCOTRS_LIFE_WOULD_BE_BETTER
		//for designer support
		private SchemaEditorView():base(null){}
#endif

		#endregion

		#region C'tors

		private SchemaEditorView(IView parentView, SaveFileDialog saveFileDlg):base(parentView)
		{
			this.saveFileDlg = saveFileDlg;
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
		}

		public SchemaEditorView(IView parentView, SaveFileDialog saveFileDlg, Type rootclassType, string pathToDocument):this(parentView, saveFileDlg)
		{
			editor = new SchemaEditor(rootclassType,new WinFormsSchemaEditorNodeFactory(this));
			if(pathToDocument!=null)
			{
				OpenFile(pathToDocument);
			}
			else
			{
				editor.CreateDefaultRoot();
				Title = "New "+SchemaEditor.GetTypeName(rootclassType);
			}
			foreach(TreeNode node in graphTree.Nodes)
				node.Expand();
		}

		public SchemaEditorView(IView parentView, SaveFileDialog saveFileDlg, Type rootClassType):
			this(parentView, saveFileDlg, rootClassType,null)
		{
		}

		#endregion

		#region Overriden methods

		public override bool Save()
		{
			if(filename==null)
			{
				if(SaveAs()==false)
					return false;
			}
			using(TextWriter writer = File.CreateText(filename))
			{
				editor.Write(writer);
			}
			HasChanges = false;
			return true;
		}

		public override bool SaveAs()
		{
			if(saveFileDlg.ShowDialog(this)==DialogResult.Cancel)
				return false;
			filename = AssureCorrectExtention(saveFileDlg.FileName);
			Title = Path.GetFileName(filename);
			return Save();
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if(components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#endregion

		#region Properties

		public TreeView GraphTree
		{
			get { return graphTree; }
		}

		private WinFormsSchemaEditorNode SelectedNode
		{
			get { return this.graphTree.SelectedNode as WinFormsSchemaEditorNode; }
		}

		#endregion

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SchemaEditorView));
            this.documentView = new System.Windows.Forms.TabControl();
            this.graphView = new System.Windows.Forms.TabPage();
            this.attributesPropertyGrid = new System.Windows.Forms.PropertyGrid();
            this.hSplitter = new Furty.Windows.Forms.CollapsibleSplitter();
            this.errorsList = new System.Windows.Forms.ListView();
            this.SchemaErrors = new System.Windows.Forms.ColumnHeader();
            //this.attributesList = new System.Windows.Forms.ListView(); 
            this.attributeName = new System.Windows.Forms.ColumnHeader();
            this.attributeValue = new System.Windows.Forms.ColumnHeader();
            this.images = new System.Windows.Forms.ImageList(this.components);
            this.vSplit = new System.Windows.Forms.Splitter();
            this.graphTree = new Ayende.NHibernateQueryAnalyzer.UserInterface.Controls.ContextMenuTreeView();
            this.graphTreeContextMenu = new System.Windows.Forms.ContextMenu();
            this.addChild = new System.Windows.Forms.MenuItem();
            this.nothingToAdd = new System.Windows.Forms.MenuItem();
            this.spacer = new System.Windows.Forms.MenuItem();
            this.removeChild = new System.Windows.Forms.MenuItem();
            this.xmlView = new System.Windows.Forms.TabPage();
            this.xmlText = new System.Windows.Forms.TextBox();
            this.errorTooltip = new System.Windows.Forms.ToolTip(this.components);
            this.documentView.SuspendLayout();
            this.graphView.SuspendLayout();
            this.xmlView.SuspendLayout();
            this.SuspendLayout();
            // 
            // documentView
            // 
            this.documentView.Alignment = System.Windows.Forms.TabAlignment.Bottom;
            this.documentView.Controls.Add(this.graphView);
            this.documentView.Controls.Add(this.xmlView);
            this.documentView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.documentView.Location = new System.Drawing.Point(0, 0);
            this.documentView.Name = "documentView";
            this.documentView.SelectedIndex = 0;
            this.documentView.Size = new System.Drawing.Size(792, 494);
            this.documentView.TabIndex = 0;
            this.documentView.SelectedIndexChanged += new System.EventHandler(this.documentView_SelectedIndexChanged);
            // 
            // graphView
            // 
            this.graphView.Controls.Add(this.attributesPropertyGrid);
            this.graphView.Controls.Add(this.hSplitter);
            this.graphView.Controls.Add(this.errorsList);
            //this.graphView.Controls.Add(this.attributesList);
            this.graphView.Controls.Add(this.vSplit);
            this.graphView.Controls.Add(this.graphTree);
            this.graphView.Location = new System.Drawing.Point(4, 4);
            this.graphView.Name = "graphView";
            this.graphView.Size = new System.Drawing.Size(784, 468);
            this.graphView.TabIndex = 0;
            this.graphView.Text = "Graph View";
            // 
            // attributesPropertyGrid
            // 
            this.attributesPropertyGrid.Dock = System.Windows.Forms.DockStyle.Fill;
            this.attributesPropertyGrid.Location = new System.Drawing.Point(179, 0);
            this.attributesPropertyGrid.Name = "attributesPropertyGrid";
            this.attributesPropertyGrid.Size = new System.Drawing.Size(605, 124);
            this.attributesPropertyGrid.TabIndex = 8;
            this.attributesPropertyGrid.PropertyValueChanged += new System.Windows.Forms.PropertyValueChangedEventHandler(this.attributesPropertyGrid_PropertyValueChanged);
            // 
            // hSplitter
            // 
            this.hSplitter.ControlToHide = this.errorsList;
            this.hSplitter.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.hSplitter.ExpandParentForm = false;
            this.hSplitter.Location = new System.Drawing.Point(179, 124);
            this.hSplitter.Name = "hSplitter";
            this.hSplitter.ParentForm = null;
            this.hSplitter.Size = new System.Drawing.Size(605, 8);
            this.hSplitter.TabIndex = 7;
            this.hSplitter.TabStop = false;
            this.hSplitter.VisualStyle = Furty.Windows.Forms.CollapsibleSplitter.VisualStyles.Mozilla;
            // 
            // errorsList
            // 
            this.errorsList.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.SchemaErrors});
            this.errorsList.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.errorsList.ForeColor = System.Drawing.Color.Maroon;
            this.errorsList.FullRowSelect = true;
            this.errorsList.GridLines = true;
            this.errorsList.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.errorsList.Location = new System.Drawing.Point(179, 132);
            this.errorsList.MultiSelect = false;
            this.errorsList.Name = "errorsList";
            this.errorsList.Size = new System.Drawing.Size(605, 75);
            this.errorsList.TabIndex = 6;
            this.errorsList.UseCompatibleStateImageBehavior = false;
            this.errorsList.View = System.Windows.Forms.View.Details;
            this.errorsList.Visible = false;
            // 
            // SchemaErrors
            // 
            this.SchemaErrors.Text = "Schema Errors:";
            this.SchemaErrors.Width = 592;
            // 
            // attributesList
            // 
            /*
            this.attributesList.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.attributeName,
            this.attributeValue});
            this.attributesList.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.attributesList.FullRowSelect = true;
            this.attributesList.GridLines = true;
            this.attributesList.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.attributesList.Location = new System.Drawing.Point(179, 207);
            this.attributesList.MultiSelect = false;
            this.attributesList.Name = "attributesList";
            this.attributesList.Size = new System.Drawing.Size(605, 261);
            this.attributesList.SmallImageList = this.images;
            this.attributesList.TabIndex = 2;
            this.attributesList.UseCompatibleStateImageBehavior = false;
            this.attributesList.View = System.Windows.Forms.View.Details;
            this.attributesList.ItemActivate += new System.EventHandler(this.attributesList_ItemActivate);
             */ //Replaced by property grid
            // 
            // attributeName
            // 
            this.attributeName.Text = "Name:";
            this.attributeName.Width = 109;
            // 
            // attributeValue
            // 
            this.attributeValue.Text = "Value:";
            this.attributeValue.Width = 405;
            // 
            // images
            // 
            this.images.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("images.ImageStream")));
            this.images.TransparentColor = System.Drawing.Color.Transparent;
            this.images.Images.SetKeyName(0, "");
            this.images.Images.SetKeyName(1, "");
            this.images.Images.SetKeyName(2, "");
            // 
            // vSplit
            // 
            this.vSplit.Location = new System.Drawing.Point(176, 0);
            this.vSplit.Name = "vSplit";
            this.vSplit.Size = new System.Drawing.Size(3, 468);
            this.vSplit.TabIndex = 1;
            this.vSplit.TabStop = false;
            // 
            // graphTree
            // 
            this.graphTree.ContextMenu = this.graphTreeContextMenu;
            this.graphTree.Dock = System.Windows.Forms.DockStyle.Left;
            this.graphTree.FullRowSelect = true;
            this.graphTree.HideSelection = false;
            this.graphTree.ImageIndex = 0;
            this.graphTree.ImageList = this.images;
            this.graphTree.Location = new System.Drawing.Point(0, 0);
            this.graphTree.Name = "graphTree";
            this.graphTree.SelectedImageIndex = 0;
            this.graphTree.Size = new System.Drawing.Size(176, 468);
            this.graphTree.TabIndex = 0;
            this.graphTree.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.graphTree_AfterSelect);
            this.graphTree.MouseHover += new System.EventHandler(this.graphTree_MouseHover);
            this.graphTree.MouseDown += new System.Windows.Forms.MouseEventHandler(this.graphTree_MouseDown);
            // 
            // graphTreeContextMenu
            // 
            this.graphTreeContextMenu.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.addChild,
            this.spacer,
            this.removeChild});
            this.graphTreeContextMenu.Popup += new System.EventHandler(this.graphTreeContextMenu_Popup);
            // 
            // addChild
            // 
            this.addChild.Index = 0;
            this.addChild.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.nothingToAdd});
            this.addChild.Text = "&Add";
            this.addChild.Popup += new System.EventHandler(this.addChild_Popup);
            // 
            // nothingToAdd
            // 
            this.nothingToAdd.Enabled = false;
            this.nothingToAdd.Index = 0;
            this.nothingToAdd.Text = "Nothing to add";
            // 
            // spacer
            // 
            this.spacer.Index = 1;
            this.spacer.Text = "-";
            // 
            // removeChild
            // 
            this.removeChild.Index = 2;
            this.removeChild.Text = "&Remove";
            this.removeChild.Click += new System.EventHandler(this.removeChild_Click);
            // 
            // xmlView
            // 
            this.xmlView.Controls.Add(this.xmlText);
            this.xmlView.Location = new System.Drawing.Point(4, 4);
            this.xmlView.Name = "xmlView";
            this.xmlView.Size = new System.Drawing.Size(784, 468);
            this.xmlView.TabIndex = 1;
            this.xmlView.Text = "Xml";
            this.xmlView.Validating += new System.ComponentModel.CancelEventHandler(this.xmlView_Validating);
            // 
            // xmlText
            // 
            this.xmlText.AcceptsReturn = true;
            this.xmlText.AcceptsTab = true;
            this.xmlText.Dock = System.Windows.Forms.DockStyle.Fill;
            this.xmlText.Location = new System.Drawing.Point(0, 0);
            this.xmlText.Multiline = true;
            this.xmlText.Name = "xmlText";
            this.xmlText.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.xmlText.Size = new System.Drawing.Size(784, 468);
            this.xmlText.TabIndex = 0;
            this.xmlText.TextChanged += new System.EventHandler(this.xmlText_TextChanged);
            // 
            // SchemaEditorView
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.ClientSize = new System.Drawing.Size(792, 494);
            this.Controls.Add(this.documentView);
            this.Name = "SchemaEditorView";
            this.Text = "SchemaEditorView";
            this.documentView.ResumeLayout(false);
            this.graphView.ResumeLayout(false);
            this.xmlView.ResumeLayout(false);
            this.xmlView.PerformLayout();
            this.ResumeLayout(false);

		}
		#endregion

		#region Event Handlers

		private void graphTree_AfterSelect(object sender, TreeViewEventArgs e)
		{
			WinFormsSchemaEditorNode node = this.graphTree.SelectedNode as WinFormsSchemaEditorNode;
            if (node != null)
            {
                //node.DisplayCurrentElementAttributes(attributesList); //Replaced by property grid
                node.DisplayCurrentElementAttributes(attributesPropertyGrid);
            }
		}

		private void attributesList_ItemActivate(object sender, EventArgs e)
		{
			/*
            if(attributesList.SelectedItems.Count==0)
				return;
			string attName;
			attName = attributesList.SelectedItems[0].SubItems[0].Text;
			EditAttributeValue(attName, SelectedNode);
			//refresh display
			SelectedNode.DisplayCurrentElementAttributes(attributesList);
            // should do nothing if I'm using the property grid....
             */ // Replaced by property grid
		}

		private void graphTree_MouseHover(object sender, System.EventArgs e)
		{
			WinFormsSchemaEditorNode node = (WinFormsSchemaEditorNode)graphTree.GetNodeAt(graphTree.PointToClient(Control.MousePosition));
			if(node==null || errorsDictionary.ContainsKey(node)==false)
			{
				errorTooltip.SetToolTip(graphTree,"");
				return;
			}
			errorTooltip.SetToolTip(graphTree, (string)errorsDictionary[node]);
		}

		private void addChild_Popup(object sender, System.EventArgs e)
		{
			addChild.MenuItems.Clear();
			menuItemToField = new Hashtable();
			IList items = new ArrayList(), requiredItems = new ArrayList();
			//selected node is guranteed that this will not be null by the contet menu
			foreach (NodeFieldReference field in SelectedNode.NodeFields)
			{
				if(CanAdd(field)==false)
					continue;
				foreach (string name in field.Names)
				{
					MenuItem item = new MenuItem(name);	
					item.Click+=new EventHandler(AddChild);
					menuItemToField.Add(item, new object[]{SelectedNode, field.TypeMatching(name)});
					if(field.IsRequired)
						requiredItems.Add(item);
					else
						items.Add(item);
				}
			}
			if(requiredItems.Count>0)
			{
				foreach (MenuItem item in requiredItems)
					addChild.MenuItems.Add(item);
				if(items.Count>0)
					addChild.MenuItems.Add("-");//seperator
			}
			foreach (MenuItem item in items)
				addChild.MenuItems.Add(item);
			if (addChild.MenuItems.Count==0)
				addChild.MenuItems.Add(nothingToAdd);
		}

		private void graphTreeContextMenu_Popup(object sender, System.EventArgs e)
		{
			graphTreeContextMenu.MenuItems.Clear();
			if(graphTree.SelectedNode!=null)
			{
				graphTreeContextMenu.MenuItems.Add(addChild);
				if(graphTree.SelectedNode.Parent!=null)//can't delete root element
				{
					graphTreeContextMenu.MenuItems.Add(spacer);
					graphTreeContextMenu.MenuItems.Add(removeChild);
				}
			}
		}

		private void graphTree_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			TreeNode node = graphTree.GetNodeAt(graphTree.PointToClient(Control.MousePosition));
			if (e.Button == MouseButtons.Right && node!=null)
				graphTree.SelectedNode = node;
		}

		private void documentView_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			if(documentView.SelectedTab==xmlView)
			{
				bool tmp = HasChanges;
				//from object to text, should always work
				xmlText.Text = editor.ToString();
				xmlTextEdited = false;
				HasChanges = tmp;//this avoid auto-setting to unsaved when switching to xmlview (Bug #27)
			}
			else
			{
				if(xmlTextEdited==false)
					return;
				xmlTextEdited = false;
				//replace the root, 
				graphTree.Nodes.RemoveAt(0);
				//objectFromEditedXml is filled when the user switch
				editor.ReadObjectRoot(objectFromEditedXml);
				//clear the attributes list
				//attributesList.Items.Clear(); // replaced by property grid

			}
		}

		private void xmlView_Validating(object sender, System.ComponentModel.CancelEventArgs e)
		{
			try
			{
				CreataObjectFromXml();
			}
			catch(Exception ex)
			{
				MessageBox.Show(ex.Message, "Invalid XML",MessageBoxButtons.OK, MessageBoxIcon.Error);
				e.Cancel = true;
			}
					
		}

		private void removeChild_Click(object sender, System.EventArgs e)
		{
			if(graphTree.SelectedNode!=null)
			{
				RemoveNode();
			}
		}

		private void xmlText_TextChanged(object sender, System.EventArgs e)
		{
			HasChanges = true;
			xmlTextEdited = true;
		}

		#endregion

		#region Private Methods

		private string AssureCorrectExtention(string name)
		{
			if(name.IndexOf('.')!=-1)//already has extention, so just return it
				return name;
			return name +"."+ saveFileDlg.DefaultExt;
		}

		private void OpenFile(string pathToDocument)
		{
			using(Stream str = File.OpenRead(pathToDocument))
			{
				XmlTextReader document = new XmlTextReader(str);
				editor.Read(document);
			}
			Title = Path.GetFileName(pathToDocument);
			this.filename = pathToDocument;
		}


		private void EditAttributeValue(string attName, WinFormsSchemaEditorNode node)
		{
			AttributeFieldReference attribute = (AttributeFieldReference)node.Attributes.GetByKey(attName);
			using(EditAttribute edit = new EditAttribute(attribute))
			{
				DialogResult result = edit.ShowDialog(this);
				if(result==DialogResult.Cancel) 
					return;
				if(result==DialogResult.Abort && node.ActiveAttributes.Contains(attribute))
				{
					attribute.SetToDefaultValue();
				}
				else
				{
					attribute.Value = edit.AttributeValue;
				}
				node.RefreshTitle();
				CheckSchema();
				HasChanges = true;
			}
		}

		private void CheckSchema()
		{
			errorsList.Items.Clear();
			errorsDictionary = new Hashtable();
			SchemaErrorCollection errors = SchemaEditor.CheckSchemaCompliance(editor.RootNode);
			foreach (ISchemaError schemaError in errors)
			{
				errorsList.Items.Add(schemaError.Message);
				errorsDictionary[schemaError.ParentNode] = schemaError.Message;
			}
			if(errors.Count>0)
				errorsList.Visible = true;
		}


		private void AddChild(object sender, EventArgs e)
		{
			object[] arr = (object[]) menuItemToField[sender];
			WinFormsSchemaEditorNode node = (WinFormsSchemaEditorNode)arr[0];
			Type type = (Type)arr[1];
			editor.CreateChild(node,type);
			CheckSchema();
			HasChanges = true;
			node.Expand();
		}

		// You can add if the field is an array,
		// or if it doesn't already have a value
		private bool CanAdd(NodeFieldReference field)
		{
			return field is ArrayNodeFieldReference || field.HasValue==false;
		}

		private void CreataObjectFromXml()
		{
			XmlTextReader reader = new XmlTextReader(new StringReader(xmlText.Text));
			objectFromEditedXml = editor.Serializer.Deserialize(reader);
		}

		private void RemoveNode()
		{
			DialogResult answer = MessageBox.Show("Are you certain you want to remove the node '"+graphTree.SelectedNode.Text+"'?",
			                                      "Remove node?",MessageBoxButtons.YesNo,MessageBoxIcon.Question);
			if(answer==DialogResult.No)
				return;
	
			editor.RemoveChild((ISchemaEditorNode)SelectedNode.Parent, SelectedNode);
			CheckSchema();
			HasChanges = true;
		}

		#endregion

        private void attributesPropertyGrid_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
        {
            CheckSchema();
            HasChanges = true;
        }

		
	}
}
