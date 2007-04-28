using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using Ayende.NHibernateQueryAnalyzer.UserInterface.Controls;
using Ayende.NHibernateQueryAnalyzer.UserInterface.Interfaces;
using Ayende.NHibernateQueryAnalyzer.Utilities;
using Furty.Windows.Forms;
using Netron.Lithium;
using NHibernate.Mapping;
using NHibernate.Cfg;


namespace Ayende.NHibernateQueryAnalyzer.UserInterface
{
	/// <summary>
	/// Summary description for QueryForm.
	/// </summary>
	public class QueryForm : NQADocument, IQueryView
	{
		#region Variables

		private readonly IQueryPresenter presenter;
		private Panel queryPanel;
		private TextBox queryText;
		private CollapsibleSplitter paramsSplitter;
		private TabControl resultsTab;
		private TabPage sqlTab;
		private TextBox sqlText;
		private TabPage exceptionTab;
		private ColumnHeader errors;
		private Splitter errorsTabSplitter;
		private TextBox exceptionsText;
		private ColumnHeader exeptionMessageText;
		private CollapsibleSplitter mainSplitter;
		private Hashtable parameters = new Hashtable();
		private Panel paramsPanel;
		private ListView queryParams;
		private ColumnHeader paramName;
		private ColumnHeader prarmType;
		private ColumnHeader paramValue;
		private ComboBox suggestParam;
		private Button addParam;
		private Button delParam;
		private ListView exceptionsList;
		private ColumnHeader exceptionsCH;
		private Splitter exceptionsSplitter;
		private TabPage objectGraphTab;
		private LithiumControl objectGraph;
		private TabPage dbResultsTab;
		private DataGrid sqlResultDataGrid;
        private SplitContainer EntityExplorerContainer;
        private TreeView EntityExplorer;
        private SplitContainer paramContainer;
        private ListView lstProperties;
        private ColumnHeader PropertyName;
        private ColumnHeader PropertyType;
		private const int maxExceptionCount = 35;
        SortedList<string, int> Entites = new SortedList<string, int>();
        SortedList<string, string> Properties = new SortedList<string, string>();
        SortedList<string, string> Aliases = new SortedList<string, string>();
        IList<Entity> entityList = new List<Entity>();
		private ImageList entitiesListingImgs;
		private System.ComponentModel.IContainer components;
        bool moveCursorOnePositionLess = false;
        

		#endregion

		public QueryForm(IQueryPresenter presenter,IView parentView) : base(parentView)
		{
			this.presenter = presenter;
            
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

            
		}

        private void GetAllEntitiesAndAddInEntityExplorer()
        {
            try
            {
                SortedList list = presenter.MappingFiles;
                TreeNode MainNode = new TreeNode("Entities");
                int KeyCounter = 1;
            	MainNode.ImageIndex = 4;
            	MainNode.SelectedImageIndex = 4;

                foreach (DictionaryEntry dictionaryEntity in list)
                {
                    MappingEntity entity = dictionaryEntity.Value as MappingEntity;
                    Entity entityForList = new Entity();
                    entityForList.Name = entity.EntityName;
                    Entites.Add(entity.EntityName, KeyCounter);
                    KeyCounter++;
                    
                    TreeNode entityNode = new TreeNode(entity.EntityName + " [" + entity.TableName + "]");
                    entityNode.Tag = entity.EntityName;
                	entityNode.ImageIndex = 0;
                	entityNode.SelectedImageIndex = 0;
                    foreach(System.Collections.Generic.KeyValuePair<string,MappingEntityProperty> m_property in entity.Properties)
                    {
                        MappingEntityProperty property = m_property.Value;
                        
                        TreeNode PropertyNode = new TreeNode(property.PropertyName + " [" + property.PropertyType + "]");
                    	PropertyNode.ImageIndex = 1;
                    	PropertyNode.SelectedImageIndex = 1;
                        PropertyNode.Tag = property.PropertyName;
                        entityNode.Nodes.Add(PropertyNode);
                        
                        entityForList.Properties.Add(property.PropertyName, property.ReturnClassName);

                    }


                    MainNode.Nodes.Add(entityNode);
                    entityList.Add(entityForList);
                }
                this.EntityExplorer.Nodes.Add(MainNode);
				MainNode.Expand();
                EntityExplorer.EndUpdate();
            }
            catch (Exception ex)
            {
                string s = ex.Message;  
            }
        }


            

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose(bool disposing)
		{
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(QueryForm));
			Netron.Lithium.Impl.ShapeLayout.TreeLayoutStrategy treeLayoutStrategy1 = new Netron.Lithium.Impl.ShapeLayout.TreeLayoutStrategy();
			this.queryPanel = new System.Windows.Forms.Panel();
			this.EntityExplorerContainer = new System.Windows.Forms.SplitContainer();
			this.EntityExplorer = new System.Windows.Forms.TreeView();
			this.entitiesListingImgs = new System.Windows.Forms.ImageList(this.components);
			this.queryText = new System.Windows.Forms.TextBox();
			this.paramsSplitter = new Furty.Windows.Forms.CollapsibleSplitter();
			this.paramsPanel = new System.Windows.Forms.Panel();
			this.paramContainer = new System.Windows.Forms.SplitContainer();
			this.lstProperties = new System.Windows.Forms.ListView();
			this.PropertyName = new System.Windows.Forms.ColumnHeader();
			this.PropertyType = new System.Windows.Forms.ColumnHeader();
			this.delParam = new System.Windows.Forms.Button();
			this.queryParams = new System.Windows.Forms.ListView();
			this.paramName = new System.Windows.Forms.ColumnHeader();
			this.prarmType = new System.Windows.Forms.ColumnHeader();
			this.paramValue = new System.Windows.Forms.ColumnHeader();
			this.suggestParam = new System.Windows.Forms.ComboBox();
			this.addParam = new System.Windows.Forms.Button();
			this.resultsTab = new System.Windows.Forms.TabControl();
			this.sqlTab = new System.Windows.Forms.TabPage();
			this.sqlText = new System.Windows.Forms.TextBox();
			this.exceptionTab = new System.Windows.Forms.TabPage();
			this.exceptionsSplitter = new System.Windows.Forms.Splitter();
			this.exceptionsText = new System.Windows.Forms.TextBox();
			this.exceptionsList = new System.Windows.Forms.ListView();
			this.exceptionsCH = new System.Windows.Forms.ColumnHeader();
			this.errorsTabSplitter = new System.Windows.Forms.Splitter();
			this.objectGraphTab = new System.Windows.Forms.TabPage();
			this.objectGraph = new Netron.Lithium.LithiumControl();
			this.dbResultsTab = new System.Windows.Forms.TabPage();
			this.sqlResultDataGrid = new System.Windows.Forms.DataGrid();
			this.exeptionMessageText = new System.Windows.Forms.ColumnHeader();
			this.errors = new System.Windows.Forms.ColumnHeader();
			this.mainSplitter = new Furty.Windows.Forms.CollapsibleSplitter();
			this.queryPanel.SuspendLayout();
			this.EntityExplorerContainer.Panel1.SuspendLayout();
			this.EntityExplorerContainer.Panel2.SuspendLayout();
			this.EntityExplorerContainer.SuspendLayout();
			this.paramsPanel.SuspendLayout();
			this.paramContainer.Panel1.SuspendLayout();
			this.paramContainer.Panel2.SuspendLayout();
			this.paramContainer.SuspendLayout();
			this.resultsTab.SuspendLayout();
			this.sqlTab.SuspendLayout();
			this.exceptionTab.SuspendLayout();
			this.objectGraphTab.SuspendLayout();
			this.dbResultsTab.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.sqlResultDataGrid)).BeginInit();
			this.SuspendLayout();
			// 
			// queryPanel
			// 
			this.queryPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.queryPanel.Controls.Add(this.EntityExplorerContainer);
			this.queryPanel.Controls.Add(this.paramsSplitter);
			this.queryPanel.Controls.Add(this.paramsPanel);
			this.queryPanel.Dock = System.Windows.Forms.DockStyle.Top;
			this.queryPanel.Location = new System.Drawing.Point(0, 0);
			this.queryPanel.Name = "queryPanel";
			this.queryPanel.Size = new System.Drawing.Size(908, 359);
			this.queryPanel.TabIndex = 1;
			// 
			// EntityExplorerContainer
			// 
			this.EntityExplorerContainer.Dock = System.Windows.Forms.DockStyle.Fill;
			this.EntityExplorerContainer.Location = new System.Drawing.Point(0, 0);
			this.EntityExplorerContainer.Name = "EntityExplorerContainer";
			// 
			// EntityExplorerContainer.Panel1
			// 
			this.EntityExplorerContainer.Panel1.Controls.Add(this.EntityExplorer);
			// 
			// EntityExplorerContainer.Panel2
			// 
			this.EntityExplorerContainer.Panel2.Controls.Add(this.queryText);
			this.EntityExplorerContainer.Size = new System.Drawing.Size(606, 357);
			this.EntityExplorerContainer.SplitterDistance = 202;
			this.EntityExplorerContainer.TabIndex = 4;
			// 
			// EntityExplorer
			// 
			this.EntityExplorer.Dock = System.Windows.Forms.DockStyle.Fill;
			this.EntityExplorer.ImageIndex = 4;
			this.EntityExplorer.ImageList = this.entitiesListingImgs;
			this.EntityExplorer.Location = new System.Drawing.Point(0, 0);
			this.EntityExplorer.Name = "EntityExplorer";
			this.EntityExplorer.SelectedImageIndex = 3;
			this.EntityExplorer.Size = new System.Drawing.Size(202, 357);
			this.EntityExplorer.TabIndex = 0;
			this.EntityExplorer.DragEnter += new System.Windows.Forms.DragEventHandler(this.EntityExplorer_DragEnter);
			this.EntityExplorer.ItemDrag += new System.Windows.Forms.ItemDragEventHandler(this.EntityExplorer_ItemDrag);
			// 
			// entitiesListingImgs
			// 
			this.entitiesListingImgs.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("entitiesListingImgs.ImageStream")));
			this.entitiesListingImgs.TransparentColor = System.Drawing.Color.Transparent;
			this.entitiesListingImgs.Images.SetKeyName(0, "");
			this.entitiesListingImgs.Images.SetKeyName(1, "");
			this.entitiesListingImgs.Images.SetKeyName(2, "bug.png");
			this.entitiesListingImgs.Images.SetKeyName(3, "file.png");
			this.entitiesListingImgs.Images.SetKeyName(4, "folder.png");
			// 
			// queryText
			// 
			this.queryText.AllowDrop = true;
			this.queryText.Dock = System.Windows.Forms.DockStyle.Fill;
			this.queryText.Location = new System.Drawing.Point(0, 0);
			this.queryText.Multiline = true;
			this.queryText.Name = "queryText";
			this.queryText.Size = new System.Drawing.Size(400, 357);
			this.queryText.TabIndex = 0;
			this.queryText.DragDrop += new System.Windows.Forms.DragEventHandler(this.queryText_DragDrop);
			this.queryText.DragEnter += new System.Windows.Forms.DragEventHandler(this.queryText_DragEnter);
			this.queryText.KeyUp += new System.Windows.Forms.KeyEventHandler(this.queryText_KeyUp);
			this.queryText.TextChanged += new System.EventHandler(this.queryText_TextChanged);
			this.queryText.KeyDown += new System.Windows.Forms.KeyEventHandler(this.queryText_KeyDown);
			// 
			// paramsSplitter
			// 
			this.paramsSplitter.ControlToHide = this.paramsPanel;
			this.paramsSplitter.Dock = System.Windows.Forms.DockStyle.Right;
			this.paramsSplitter.ExpandParentForm = false;
			this.paramsSplitter.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.paramsSplitter.Location = new System.Drawing.Point(606, 0);
			this.paramsSplitter.Name = "paramsSplitter";
			this.paramsSplitter.ParentForm = null;
			this.paramsSplitter.Size = new System.Drawing.Size(8, 357);
			this.paramsSplitter.TabIndex = 2;
			this.paramsSplitter.TabStop = false;
			this.paramsSplitter.VisualStyle = Furty.Windows.Forms.CollapsibleSplitter.VisualStyles.Mozilla;
			// 
			// paramsPanel
			// 
			this.paramsPanel.Controls.Add(this.paramContainer);
			this.paramsPanel.Dock = System.Windows.Forms.DockStyle.Right;
			this.paramsPanel.Location = new System.Drawing.Point(614, 0);
			this.paramsPanel.Name = "paramsPanel";
			this.paramsPanel.Size = new System.Drawing.Size(292, 357);
			this.paramsPanel.TabIndex = 3;
			this.paramsPanel.Visible = false;
			// 
			// paramContainer
			// 
			this.paramContainer.Dock = System.Windows.Forms.DockStyle.Fill;
			this.paramContainer.Location = new System.Drawing.Point(0, 0);
			this.paramContainer.Name = "paramContainer";
			this.paramContainer.Orientation = System.Windows.Forms.Orientation.Horizontal;
			// 
			// paramContainer.Panel1
			// 
			this.paramContainer.Panel1.Controls.Add(this.lstProperties);
			this.paramContainer.Panel1.RightToLeft = System.Windows.Forms.RightToLeft.No;
			// 
			// paramContainer.Panel2
			// 
			this.paramContainer.Panel2.Controls.Add(this.delParam);
			this.paramContainer.Panel2.Controls.Add(this.queryParams);
			this.paramContainer.Panel2.Controls.Add(this.suggestParam);
			this.paramContainer.Panel2.Controls.Add(this.addParam);
			this.paramContainer.Panel2.RightToLeft = System.Windows.Forms.RightToLeft.No;
			this.paramContainer.RightToLeft = System.Windows.Forms.RightToLeft.No;
			this.paramContainer.Size = new System.Drawing.Size(292, 357);
			this.paramContainer.SplitterDistance = 196;
			this.paramContainer.TabIndex = 9;
			// 
			// lstProperties
			// 
			this.lstProperties.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.PropertyName,
            this.PropertyType});
			this.lstProperties.Dock = System.Windows.Forms.DockStyle.Fill;
			this.lstProperties.FullRowSelect = true;
			this.lstProperties.GridLines = true;
			this.lstProperties.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
			this.lstProperties.Location = new System.Drawing.Point(0, 0);
			this.lstProperties.Name = "lstProperties";
			this.lstProperties.Size = new System.Drawing.Size(292, 196);
			this.lstProperties.TabIndex = 1;
			this.lstProperties.UseCompatibleStateImageBehavior = false;
			this.lstProperties.View = System.Windows.Forms.View.Details;
			// 
			// PropertyName
			// 
			this.PropertyName.Text = "Name";
			this.PropertyName.Width = 165;
			// 
			// PropertyType
			// 
			this.PropertyType.Text = "Type";
			this.PropertyType.Width = 120;
			// 
			// delParam
			// 
			this.delParam.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.delParam.Enabled = false;
			this.delParam.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.delParam.Location = new System.Drawing.Point(257, 12);
			this.delParam.Name = "delParam";
			this.delParam.Size = new System.Drawing.Size(32, 23);
			this.delParam.TabIndex = 6;
			this.delParam.Text = "Del";
			this.delParam.Click += new System.EventHandler(this.delParam_Click);
			// 
			// queryParams
			// 
			this.queryParams.Alignment = System.Windows.Forms.ListViewAlignment.Default;
			this.queryParams.AllowColumnReorder = true;
			this.queryParams.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.paramName,
            this.prarmType,
            this.paramValue});
			this.queryParams.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.queryParams.FullRowSelect = true;
			this.queryParams.GridLines = true;
			this.queryParams.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
			this.queryParams.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.queryParams.LabelWrap = false;
			this.queryParams.Location = new System.Drawing.Point(0, 39);
			this.queryParams.MultiSelect = false;
			this.queryParams.Name = "queryParams";
			this.queryParams.RightToLeft = System.Windows.Forms.RightToLeft.No;
			this.queryParams.Size = new System.Drawing.Size(292, 118);
			this.queryParams.TabIndex = 0;
			this.queryParams.UseCompatibleStateImageBehavior = false;
			this.queryParams.View = System.Windows.Forms.View.Details;
			this.queryParams.ItemActivate += new System.EventHandler(this.queryParams_ItemActivate);
			this.queryParams.SelectedIndexChanged += new System.EventHandler(this.queryParams_SelectedIndexChanged);
			// 
			// paramName
			// 
			this.paramName.Text = "Parameter:";
			this.paramName.Width = 82;
			// 
			// prarmType
			// 
			this.prarmType.Text = "Type:";
			this.prarmType.Width = 73;
			// 
			// paramValue
			// 
			this.paramValue.Text = "Value:";
			this.paramValue.Width = 70;
			// 
			// suggestParam
			// 
			this.suggestParam.ItemHeight = 13;
			this.suggestParam.Location = new System.Drawing.Point(3, 14);
			this.suggestParam.Name = "suggestParam";
			this.suggestParam.Size = new System.Drawing.Size(205, 21);
			this.suggestParam.TabIndex = 8;
			// 
			// addParam
			// 
			this.addParam.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.addParam.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.addParam.Location = new System.Drawing.Point(214, 12);
			this.addParam.Name = "addParam";
			this.addParam.Size = new System.Drawing.Size(40, 23);
			this.addParam.TabIndex = 7;
			this.addParam.Text = "&Add";
			this.addParam.Click += new System.EventHandler(this.addParam_Click);
			// 
			// resultsTab
			// 
			this.resultsTab.Controls.Add(this.sqlTab);
			this.resultsTab.Controls.Add(this.exceptionTab);
			this.resultsTab.Controls.Add(this.objectGraphTab);
			this.resultsTab.Controls.Add(this.dbResultsTab);
			this.resultsTab.Dock = System.Windows.Forms.DockStyle.Fill;
			this.resultsTab.ItemSize = new System.Drawing.Size(42, 18);
			this.resultsTab.Location = new System.Drawing.Point(0, 367);
			this.resultsTab.Name = "resultsTab";
			this.resultsTab.SelectedIndex = 0;
			this.resultsTab.Size = new System.Drawing.Size(908, 184);
			this.resultsTab.TabIndex = 10;
			// 
			// sqlTab
			// 
			this.sqlTab.Controls.Add(this.sqlText);
			this.sqlTab.Location = new System.Drawing.Point(4, 22);
			this.sqlTab.Name = "sqlTab";
			this.sqlTab.Size = new System.Drawing.Size(900, 158);
			this.sqlTab.TabIndex = 11;
			this.sqlTab.Text = "SQL:";
			// 
			// sqlText
			// 
			this.sqlText.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.sqlText.Dock = System.Windows.Forms.DockStyle.Fill;
			this.sqlText.Location = new System.Drawing.Point(0, 0);
			this.sqlText.Multiline = true;
			this.sqlText.Name = "sqlText";
			this.sqlText.ReadOnly = true;
			this.sqlText.Size = new System.Drawing.Size(900, 158);
			this.sqlText.TabIndex = 12;
			// 
			// exceptionTab
			// 
			this.exceptionTab.Controls.Add(this.exceptionsSplitter);
			this.exceptionTab.Controls.Add(this.exceptionsText);
			this.exceptionTab.Controls.Add(this.exceptionsList);
			this.exceptionTab.Controls.Add(this.errorsTabSplitter);
			this.exceptionTab.Location = new System.Drawing.Point(4, 22);
			this.exceptionTab.Name = "exceptionTab";
			this.exceptionTab.Size = new System.Drawing.Size(900, 158);
			this.exceptionTab.TabIndex = 14;
			this.exceptionTab.Text = "Exceptions:";
			// 
			// exceptionsSplitter
			// 
			this.exceptionsSplitter.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.exceptionsSplitter.Dock = System.Windows.Forms.DockStyle.Top;
			this.exceptionsSplitter.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.exceptionsSplitter.Location = new System.Drawing.Point(0, 56);
			this.exceptionsSplitter.Name = "exceptionsSplitter";
			this.exceptionsSplitter.Size = new System.Drawing.Size(900, 5);
			this.exceptionsSplitter.TabIndex = 15;
			this.exceptionsSplitter.TabStop = false;
			// 
			// exceptionsText
			// 
			this.exceptionsText.Dock = System.Windows.Forms.DockStyle.Fill;
			this.exceptionsText.Location = new System.Drawing.Point(0, 56);
			this.exceptionsText.Multiline = true;
			this.exceptionsText.Name = "exceptionsText";
			this.exceptionsText.ReadOnly = true;
			this.exceptionsText.Size = new System.Drawing.Size(900, 102);
			this.exceptionsText.TabIndex = 16;
			// 
			// exceptionsList
			// 
			this.exceptionsList.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.exceptionsList.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.exceptionsCH});
			this.exceptionsList.Dock = System.Windows.Forms.DockStyle.Top;
			this.exceptionsList.FullRowSelect = true;
			this.exceptionsList.GridLines = true;
			this.exceptionsList.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
			this.exceptionsList.Location = new System.Drawing.Point(0, 3);
			this.exceptionsList.Name = "exceptionsList";
			this.exceptionsList.Size = new System.Drawing.Size(900, 53);
			this.exceptionsList.TabIndex = 17;
			this.exceptionsList.UseCompatibleStateImageBehavior = false;
			this.exceptionsList.View = System.Windows.Forms.View.Details;
			this.exceptionsList.SelectedIndexChanged += new System.EventHandler(this.exceptionsList_SelectedIndexChanged);
			// 
			// exceptionsCH
			// 
			this.exceptionsCH.Text = "Exceptions:";
			this.exceptionsCH.Width = 600;
			// 
			// errorsTabSplitter
			// 
			this.errorsTabSplitter.Dock = System.Windows.Forms.DockStyle.Top;
			this.errorsTabSplitter.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.errorsTabSplitter.Location = new System.Drawing.Point(0, 0);
			this.errorsTabSplitter.Name = "errorsTabSplitter";
			this.errorsTabSplitter.Size = new System.Drawing.Size(900, 3);
			this.errorsTabSplitter.TabIndex = 18;
			this.errorsTabSplitter.TabStop = false;
			// 
			// objectGraphTab
			// 
			this.objectGraphTab.Controls.Add(this.objectGraph);
			this.objectGraphTab.Location = new System.Drawing.Point(4, 22);
			this.objectGraphTab.Name = "objectGraphTab";
			this.objectGraphTab.Size = new System.Drawing.Size(900, 158);
			this.objectGraphTab.TabIndex = 19;
			this.objectGraphTab.Text = "Object Graph:";
			// 
			// objectGraph
			// 
			this.objectGraph.BackColor = System.Drawing.SystemColors.ActiveCaption;
			this.objectGraph.BackColor2 = System.Drawing.SystemColors.InactiveCaption;
			this.objectGraph.ConnectionLayout = Netron.Lithium.ConnectionLayout.Traditional;
			this.objectGraph.Dock = System.Windows.Forms.DockStyle.Fill;
			this.objectGraph.HoverWait = 1500;
			this.objectGraph.Location = new System.Drawing.Point(0, 0);
			this.objectGraph.Name = "objectGraph";
			this.objectGraph.SelectedShape = null;
			treeLayoutStrategy1.BranchHeight = 30;
			treeLayoutStrategy1.WordSpacing = 30;
			this.objectGraph.ShapeLayoutStrategy = treeLayoutStrategy1;
			this.objectGraph.Size = new System.Drawing.Size(900, 158);
			this.objectGraph.TabIndex = 20;
			// 
			// dbResultsTab
			// 
			this.dbResultsTab.Controls.Add(this.sqlResultDataGrid);
			this.dbResultsTab.Location = new System.Drawing.Point(4, 22);
			this.dbResultsTab.Name = "dbResultsTab";
			this.dbResultsTab.Size = new System.Drawing.Size(900, 158);
			this.dbResultsTab.TabIndex = 21;
			this.dbResultsTab.Text = "DataBase Results:";
			// 
			// sqlResultDataGrid
			// 
			this.sqlResultDataGrid.DataMember = "";
			this.sqlResultDataGrid.Dock = System.Windows.Forms.DockStyle.Fill;
			this.sqlResultDataGrid.HeaderForeColor = System.Drawing.SystemColors.ControlText;
			this.sqlResultDataGrid.Location = new System.Drawing.Point(0, 0);
			this.sqlResultDataGrid.Name = "sqlResultDataGrid";
			this.sqlResultDataGrid.ReadOnly = true;
			this.sqlResultDataGrid.Size = new System.Drawing.Size(900, 158);
			this.sqlResultDataGrid.TabIndex = 22;
			// 
			// exeptionMessageText
			// 
			this.exeptionMessageText.Text = "ColumnHeader";
			this.exeptionMessageText.Width = 812;
			// 
			// errors
			// 
			this.errors.Text = "";
			this.errors.Width = 670;
			// 
			// mainSplitter
			// 
			this.mainSplitter.ControlToHide = this.queryPanel;
			this.mainSplitter.Dock = System.Windows.Forms.DockStyle.Top;
			this.mainSplitter.ExpandParentForm = false;
			this.mainSplitter.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.mainSplitter.Location = new System.Drawing.Point(0, 359);
			this.mainSplitter.Name = "mainSplitter";
			this.mainSplitter.ParentForm = null;
			this.mainSplitter.Size = new System.Drawing.Size(908, 8);
			this.mainSplitter.TabIndex = 23;
			this.mainSplitter.TabStop = false;
			this.mainSplitter.VisualStyle = Furty.Windows.Forms.CollapsibleSplitter.VisualStyles.Mozilla;
			// 
			// QueryForm
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(908, 551);
			this.Controls.Add(this.resultsTab);
			this.Controls.Add(this.mainSplitter);
			this.Controls.Add(this.queryPanel);
			this.Name = "QueryForm";
			this.Text = "Query";
			this.Load += new System.EventHandler(this.QueryForm_Load);
			this.queryPanel.ResumeLayout(false);
			this.EntityExplorerContainer.Panel1.ResumeLayout(false);
			this.EntityExplorerContainer.Panel2.ResumeLayout(false);
			this.EntityExplorerContainer.Panel2.PerformLayout();
			this.EntityExplorerContainer.ResumeLayout(false);
			this.paramsPanel.ResumeLayout(false);
			this.paramContainer.Panel1.ResumeLayout(false);
			this.paramContainer.Panel2.ResumeLayout(false);
			this.paramContainer.ResumeLayout(false);
			this.resultsTab.ResumeLayout(false);
			this.sqlTab.ResumeLayout(false);
			this.sqlTab.PerformLayout();
			this.exceptionTab.ResumeLayout(false);
			this.exceptionTab.PerformLayout();
			this.objectGraphTab.ResumeLayout(false);
			this.dbResultsTab.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.sqlResultDataGrid)).EndInit();
			this.ResumeLayout(false);

		}

		#endregion

		public bool ShowParams
		{
			get { return queryParams.Visible; }
			set
			{
				if (queryParams.Visible != value)
					paramsSplitter.ToggleState();
			}
		}

		private void exceptionsList_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (exceptionsList.SelectedItems.Count > 0)
			{
				exceptionsText.Text = exceptionsList.SelectedItems[0].Tag.ToString();
			}
			else
			{
				exceptionsText.Text = "";
			}
		}


		private void queryText_TextChanged(object sender, EventArgs e)
		{
			presenter.Query.Text = queryText.Text;
			HasChanges = true;
			presenter.TranslateHql();
            //IList  MappingFiles = presenter.MappingFiles;
			if (presenter.HasParameters(HqlQueryText))
			{
				HandleParameters();
			}
		}

		private void HandleParameters()
		{
			ShowParams = true;
			suggestParam.Items.Clear();
			suggestParam.Text = "";
			presenter.AnalyzeParameters();
		}

		#region IQueryView Implementation

		public string QueryText
		{
			get { return queryText.Text; }
			set { queryText.Text = value; }
		}

		public override bool Save()
		{
			return presenter.SaveQuery();
		}

		public string HqlQueryText
		{
			get { return queryText.Text; }
		}

		public IDictionary Parameters
		{
			get { return parameters; }
		}

		public string SqlQueryText
		{
			get { return sqlText.Text; }
			set
			{
				sqlText.Text = value;
				resultsTab.SelectedTab = sqlTab;
				queryText.Select();
			}
		}

		public override void AddException(Exception ex)
		{
			this.resultsTab.SelectedTab = exceptionTab;
			if (exceptionsList.Items.Count > 0 && presenter.ReplaceException(ex, (Exception) exceptionsList.Items[0].Tag))
			{
				exceptionsList.Items[0].Text = ex.Message;
				exceptionsList.Items[0].Tag = ex;
			}
			else
			{
				ListViewItem exception = new ListViewItem(ex.Message);
				exception.Tag = ex;
				exceptionsList.Items.Insert(0, exception);
				if (exceptionsList.Items.Count > maxExceptionCount)
					exceptionsList.Items.RemoveAt(exceptionsList.Items.Count - 1);
			}
			queryText.Select();
		}

		public void DisplayObjectGraph(IList list)
		{
			RemoteObjectDisplayGraphControl odgc = new RemoteObjectDisplayGraphControl();
			odgc.Text = "Query result: #";
			odgc.DisplayObject(list);
			odgc.Text = "Query returned " + list.Count + " results";
			if (objectGraph.Root != null)
				objectGraph.Root.Delete();
			objectGraph.AddRoot(odgc);
			odgc.Expand();
		}

		public void DisplayDataSet(DataSet ds)
		{
			if (ds.Tables.Count == 1)
				sqlResultDataGrid.DataSource = ds.Tables[0];
			else
				sqlResultDataGrid.DataSource = ds;

		}

		public void SuggestParameter(string name)
		{
			if (!suggestParam.Items.Contains(name))
				suggestParam.Items.Add(name);
			suggestParam.SelectedItem = name;
		}

		public void RemoveSuggestParameter(string name)
		{
			suggestParam.Items.Remove(name);
		}

		public void RemoveParameter(string name)
		{
			parameters.Remove(name);
			queryParams.SelectedItems[0].Remove();
			HandleParameters();
		}

		public void SetParameterMissing(string name, bool missingState)
		{
			foreach (ListViewItem item in queryParams.Items)
			{
				if (item.Text == name)
				{
					item.ForeColor = missingState ? Color.Red : Color.FromKnownColor(KnownColor.ControlText);
					break;
				}
			}
		}

		public void ShowObjectGraph()
		{
			resultsTab.SelectedTab = objectGraphTab;
			queryPanel.Visible = false;
		}

		public IQueryPresenter QueryPresenter
		{
			get { return presenter; }
		}

		private void queryParams_SelectedIndexChanged(object sender, EventArgs e)
		{
			delParam.Enabled = queryParams.SelectedItems.Count > 0;
		}

		private void addParam_Click(object sender, EventArgs e)
		{
			using(AddParameter ap = new AddParameter(suggestParam.Text))
			{
				if(ap.ShowDialog(this)==DialogResult.Cancel)
					return;
				RemoveTopSuggestParameter();
				SetQueryParameter(ap.Parameter);
			}
		}

		private void SetQueryParameter(TypedParameter parameter)
		{
			foreach (ListViewItem item in queryParams.Items)
			{
				if(item.Text==parameter.Name)
				{
					item.Remove();
					break;
				}
			}
			ListViewItem lvi = new ListViewItem(new string[] {parameter.Name, parameter.Type.Name, parameter.Value.ToString()});
			lvi.Tag = parameter;
			parameters[parameter.Name] = parameter;
			queryParams.Items.Add(lvi);
			presenter.TranslateHql();
			presenter.AnalyzeParameters();
		}

		private void RemoveTopSuggestParameter()
		{
			suggestParam.Items.Remove(suggestParam.Text);
			if (suggestParam.Items.Count > 0)
				suggestParam.SelectedItem = suggestParam.Items[0];
			else
				suggestParam.Text = "";
		}

		private void delParam_Click(object sender, EventArgs e)
		{
			string paramName = queryParams.SelectedItems[0].Text;
			RemoveParameter(paramName);
		}

		public bool CanExecuteQuery
		{
			get { return presenter.QueryCanBeTranslated(); }
		}

        

		#endregion

		public override bool SaveAs()
		{
			return presenter.SaveQueryAs();
		}

		private void queryParams_ItemActivate(object sender, System.EventArgs e)
		{
			using(AddParameter ap = new AddParameter(queryParams.SelectedItems[0].Tag as TypedParameter))
			{
				if(ap.ShowDialog(this)==DialogResult.OK)
				{
					SetQueryParameter(ap.Parameter);
				}
			}
		}

		private void QueryForm_Load(object sender, System.EventArgs e)
		{
			Title = presenter.Query.Name;
			queryText.Text = presenter.Query.Text;
			HasChanges = false;
            
            GetAllEntitiesAndAddInEntityExplorer();
        }

        #region Intellisence Code

        private void queryText_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.OemPeriod)
            {
                AddToAliasList();

            }

            if (e.Control == true && e.KeyCode == Keys.Space)
            {
                if (this.queryText.Text == "")
                    return;

                int endPosition;
                int startIndex;
                int enterKeyAppender;
                string Query;
                string PartialText = GetPartialText(out Query, out startIndex, out endPosition, out enterKeyAppender);
                if (SearchForEntity(PartialText))
                    DisplayCompleteNameOfEntity(PartialText, endPosition, startIndex, enterKeyAppender);
                else
                {
                    string[] parts = PartialText.Split('.');
                    if (parts.Length > 1)
                    {
                        int startPosition = Convert.ToInt32(queryText.SelectionStart);
                        string entity = string.Empty;
                        string entityNameFromAlias = Aliases[parts[parts.Length - 2]];
                        string CompletepropertyName = GetEntityUsingProperty(entityNameFromAlias, parts[parts.Length - 1], true);
                        if (CompletepropertyName == "")
                            return;
                        entity = GetEntityUsingProperty(entityNameFromAlias, parts[parts.Length - 1], false);
                        if (!Aliases.ContainsKey(CompletepropertyName))
                            Aliases.Add(CompletepropertyName, entity);
                        string CompleteText = PartialText.Substring(0, PartialText.Length - parts[parts.Length - 1].Length) + CompletepropertyName;
                        this.queryText.Text = this.queryText.Text.Replace(PartialText, CompleteText);
                        SetCursorToEnd((startPosition - parts[parts.Length - 1].Length) + CompletepropertyName.Length);
                        moveCursorOnePositionLess = true;

                    }
                }
            }
        }

        private void AddToAliasList()
        {
            int endPosition;
            int startIndex;
            int enterKeyAppender;
            string Query;
            string currentEntity = string.Empty;
            string PartialText = string.Empty;

            PartialText = GetPartialText(out Query, out startIndex, out endPosition, out enterKeyAppender);
            string[] parts = PartialText.Split('.');
            string alias = parts[0];
            string entity = string.Empty;
            string entityNameFromAlias = string.Empty;
            string CompletepropertyName = string.Empty;

            currentEntity = GetEntityNameUsingAlias(PartialText, alias, false);
            if (currentEntity.Contains("."))
            {
                string[] entityNamePart = currentEntity.Split('.');
                string key = entityNamePart[entityNamePart.Length - 1];
                if (Aliases.ContainsKey(key))
                    currentEntity = Aliases[key];
                else
                {
                    entityNameFromAlias = Aliases[entityNamePart[entityNamePart.Length - 2]];
                    currentEntity = GetEntityUsingProperty(entityNameFromAlias, entityNamePart[entityNamePart.Length - 1], true);
                    entity = GetEntityUsingProperty(entityNameFromAlias, parts[parts.Length - 1], false);
                    if (!Aliases.ContainsKey(alias))
                    {
                        Aliases.Add(alias, currentEntity);
                        DisplayEntityPropertiesInPropertyGrid(currentEntity);
                    }
                }

            }
            if (!Aliases.ContainsKey(alias))
            {
                Aliases.Add(alias, currentEntity);
                DisplayEntityPropertiesInPropertyGrid(currentEntity);
            }
            if (parts.Length > 1)
            {

                entityNameFromAlias = Aliases[parts[parts.Length - 2]];
                currentEntity = GetEntityUsingProperty(entityNameFromAlias, parts[parts.Length - 1], true);
                entity = GetEntityUsingProperty(entityNameFromAlias, parts[parts.Length - 1], false);
                if (!Aliases.ContainsKey(currentEntity))
                {
                    Aliases.Add(currentEntity, entity);

                }

            }
            DisplayEntityPropertiesInPropertyGrid(currentEntity);

            return;
        }

        private void DisplayEntityPropertiesInPropertyGrid(string EntityName)
        {
            foreach (Entity entity in entityList)
            {
                if (entity.Name == EntityName)
                {
                    this.lstProperties.Items.Clear();
                    foreach (KeyValuePair<string, string> key in entity.Properties)
                    {
                        ListViewItem item = new ListViewItem(key.Key);
                        item.SubItems.Add(key.Value);
                        this.lstProperties.Items.Add(item);

                    }
                    break;
                }
            }

        }

        private bool SearchForEntity(string partial)
        {
            if (Aliases.ContainsKey(partial) == false && partial.Contains("."))
                return false;
            return true;
        }

        private string GetEntityName(string Partial, string Text)
        {

            string entityName = string.Empty;
            string[] parts = Partial.Split('.');
            string alias = parts[0];


            entityName = GetEntityNameUsingAlias(Partial, alias, true);
            if (entityName.Contains("."))
            {
                string entityNameFromAlias = Aliases[entityName.Split('.')[0]];
                entityName = GetEntityUsingProperty(entityNameFromAlias, entityName.Split('.')[1], false);
                string propertyname = GetEntityUsingProperty(entityName, parts[1], true);
                Aliases.Add(alias, entityName);
                return propertyname;
            }
            Aliases.Clear();
            Aliases.Add(alias, entityName);
            for (int counter = 1; counter < parts.Length; counter++)
            {
                entityName = GetEntityUsingProperty(entityName, parts[counter], counter == parts.Length - 1);
            }
            return entityName;
        }

        private string GetEntityUsingProperty(string entityName, string propertyName, bool returnPropertyName)
        {
            foreach (Entity entity in entityList)
            {
                if (entity.Name == entityName)
                {
                    foreach (KeyValuePair<string, string> property in entity.Properties)
                    {
                        if (property.Key.ToUpper().StartsWith(propertyName.ToUpper()))
                            if (returnPropertyName)
                            {
                                return property.Key;
                            }
                            else
                            {
                                return property.Value;
                            }
                    }
                }
            }
            return "";
        }

        private string GetEntityNameUsingAlias(string Partial, string alias, bool continueSearching)
        {
            string query = RemoveReturnKeys(this.queryText.Text);
            string[] words = query.Substring(0, query.Length - Partial.Length).Split(' ');
            string[] parts = Partial.Split('.');
            for (int counter = 0; counter < words.Length; counter++)
            {
                if (words[counter].Equals(alias))
                {
                    return words[counter - 1];
                }
            }
            return "";
        }

        private void DisplayCompleteNameOfEntity(string Partial, int endPosition, int startIndex, int enterKeyAppender)
        {
            string CompleteEntityName = GetEntityNameUsingPartialEntityName(Partial);

            if (CompleteEntityName != "")
            {
                int startPosition = Convert.ToInt32(queryText.SelectionStart);
                this.queryText.Text = this.queryText.Text.Replace(Partial, CompleteEntityName);
                SetCursorToEnd((startPosition - Partial.Length) + CompleteEntityName.Length);
                return;
            }
        }

        private string GetPartialText(out string Query, out int startIndex, out int endPosition, out int enterKeyAppender)
        {
            Query = this.queryText.Text;
            Query = RemoveReturnKeys(Query);
            startIndex = 0;

            endPosition = GetCurrentCursorPosition();
            string Partial = string.Empty;

            // the current position of cursor..
            // 1 - could be in the middle. in that case it's less that Query.Lenght
            // 2 - at the end. in this case if user has pressed enter key, it gets addes so reset it back
            enterKeyAppender = 0;
            SetEndPosition(Query, ref endPosition, ref enterKeyAppender);
            Query = Query.Substring(0, endPosition); // this trims enter key in the end
            return GetPartialEntityName(Query, endPosition, out startIndex);
        }

        private static void SetEndPosition(string Query, ref int endPosition, ref int enterKeyAppender)
        {
            if (endPosition > Query.Length) // true if enter keys are pressed. 
            {
                enterKeyAppender = endPosition - Query.Length;
                endPosition = Query.Length;
            }
        }

        private string GetPartialEntityName(string Query, int endPosition, out int startIndex)
        {
            startIndex = 0;
            for (int counter = Query.Length - 1; counter != 0; counter--)
            {
                if (Query.Substring(counter, 1) == " ")
                    break;
                startIndex++;
            }
            return Query.Substring(endPosition - startIndex, startIndex);
        }

        private string RemoveReturnKeys(string text)
        {
            text = text.Replace("\r\n", " "); // removes all enter keys presses codes
            text = text.TrimEnd(); // gets rid of spaces in the end
            return text;
        }

        private int GetCurrentCursorPosition()
        {
            return this.queryText.SelectionStart;
        }

        private void SetCursorToEnd(int cursorPosition)
        {
            this.queryText.Focus();
            this.queryText.Select(this.queryText.Text.Length, 0);
            this.queryText.SelectionStart = cursorPosition;
        }

        private string GetEntityNameUsingPartialEntityName(string partialEntityName)
        {
            foreach (KeyValuePair<string, int> str in Entites)
            {
                if (str.Key.ToUpper().StartsWith(partialEntityName.ToUpper()))
                    return str.Key;

            }
            return "";
        }

        private void queryText_KeyUp(object sender, KeyEventArgs e)
        {
            if (moveCursorOnePositionLess)
            {
                int currentPosition = this.queryText.SelectionStart;
                currentPosition--;
                SetCursorToEnd(currentPosition);
                moveCursorOnePositionLess = false;
            }
        }

        #endregion

        private void queryText_DragDrop(object sender, DragEventArgs e)
        {
        	string data = (string) e.Data.GetData(typeof (string));
       		int currentPos = queryText.GetCharIndexFromPosition(
				queryText.PointToClient(MousePosition)
				);
        	string newText = queryText.Text.Insert(currentPos, data);
        	queryText.Text =newText;
        }

        private void queryText_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.Text))
                e.Effect = DragDropEffects.Copy;
            else
                e.Effect = DragDropEffects.None;
        }

        private void EntityExplorer_ItemDrag(object sender, ItemDragEventArgs e)
        {
        	object o = ((TreeNode)e.Item).Tag;
			if(o==null)
				return;
        	string EntityName = o.ToString();
            DoDragDrop(EntityName, DragDropEffects.Move | DragDropEffects.Copy);
        }

        private void EntityExplorer_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.Text))
                e.Effect = DragDropEffects.Copy;
            else
                e.Effect = DragDropEffects.None;
        }
    }
   
    public class Entity
    {
        private string _entityName;
        private SortedList<string, string> _propertyList;

        public Entity()
        {
            _propertyList = new SortedList<string, string>();
        }
        public Entity(string name)
            : base()
        {
            this.Name = name;
        }

        public string Name
        {
            get { return _entityName; }
            set { _entityName = value; }
        }

        public SortedList<string, string> Properties
        {
            get { return _propertyList; }
            set { _propertyList = value; }
        }


    }
}
