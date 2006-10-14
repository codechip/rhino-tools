using System;
using System.Collections;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using Ayende.NHibernateQueryAnalyzer.UserInterface.Controls;
using Ayende.NHibernateQueryAnalyzer.UserInterface.Interfaces;
using Ayende.NHibernateQueryAnalyzer.Utilities;
using Furty.Windows.Forms;
using Netron.Lithium;

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
		private const int maxExceptionCount = 35;

		#endregion

		public QueryForm(IQueryPresenter presenter,IView parentView) : base(parentView)
		{
			this.presenter = presenter;
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
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
			this.queryPanel = new System.Windows.Forms.Panel();
			this.queryText = new System.Windows.Forms.TextBox();
			this.paramsSplitter = new Furty.Windows.Forms.CollapsibleSplitter();
			this.paramsPanel = new System.Windows.Forms.Panel();
			this.delParam = new System.Windows.Forms.Button();
			this.addParam = new System.Windows.Forms.Button();
			this.suggestParam = new System.Windows.Forms.ComboBox();
			this.queryParams = new System.Windows.Forms.ListView();
			this.paramName = new System.Windows.Forms.ColumnHeader();
			this.prarmType = new System.Windows.Forms.ColumnHeader();
			this.paramValue = new System.Windows.Forms.ColumnHeader();
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
			this.paramsPanel.SuspendLayout();
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
			this.queryPanel.Controls.Add(this.queryText);
			this.queryPanel.Controls.Add(this.paramsSplitter);
			this.queryPanel.Controls.Add(this.paramsPanel);
			this.queryPanel.Dock = System.Windows.Forms.DockStyle.Top;
			this.queryPanel.Location = new System.Drawing.Point(0, 0);
			this.queryPanel.Name = "queryPanel";
			this.queryPanel.Size = new System.Drawing.Size(696, 168);
			this.queryPanel.TabIndex = 1;
			// 
			// queryText
			// 
			this.queryText.Dock = System.Windows.Forms.DockStyle.Top;
			this.queryText.Location = new System.Drawing.Point(0, 0);
			this.queryText.Multiline = true;
			this.queryText.Name = "queryText";
			this.queryText.Size = new System.Drawing.Size(454, 200);
			this.queryText.TabIndex = 0;
			this.queryText.Text = "";
			this.queryText.TextChanged += new System.EventHandler(this.queryText_TextChanged);
			// 
			// paramsSplitter
			// 
			this.paramsSplitter.ControlToHide = this.paramsPanel;
			this.paramsSplitter.Dock = System.Windows.Forms.DockStyle.Right;
			this.paramsSplitter.ExpandParentForm = false;
			this.paramsSplitter.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.paramsSplitter.Location = new System.Drawing.Point(454, 0);
			this.paramsSplitter.Name = "paramsSplitter";
			this.paramsSplitter.ParentForm = null;
			this.paramsSplitter.Size = new System.Drawing.Size(8, 166);
			this.paramsSplitter.TabIndex = 2;
			this.paramsSplitter.TabStop = false;
			this.paramsSplitter.VisualStyle = Furty.Windows.Forms.CollapsibleSplitter.VisualStyles.Mozilla;
			// 
			// paramsPanel
			// 
			this.paramsPanel.Controls.Add(this.delParam);
			this.paramsPanel.Controls.Add(this.addParam);
			this.paramsPanel.Controls.Add(this.suggestParam);
			this.paramsPanel.Controls.Add(this.queryParams);
			this.paramsPanel.Dock = System.Windows.Forms.DockStyle.Right;
			this.paramsPanel.Location = new System.Drawing.Point(462, 0);
			this.paramsPanel.Name = "paramsPanel";
			this.paramsPanel.Size = new System.Drawing.Size(232, 166);
			this.paramsPanel.TabIndex = 3;
			this.paramsPanel.Visible = false;
			// 
			// delParam
			// 
			this.delParam.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.delParam.Enabled = false;
			this.delParam.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.delParam.Location = new System.Drawing.Point(192, 8);
			this.delParam.Name = "delParam";
			this.delParam.Size = new System.Drawing.Size(32, 23);
			this.delParam.TabIndex = 6;
			this.delParam.Text = "Del";
			this.delParam.Click += new System.EventHandler(this.delParam_Click);
			// 
			// addParam
			// 
			this.addParam.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.addParam.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.addParam.Location = new System.Drawing.Point(146, 8);
			this.addParam.Name = "addParam";
			this.addParam.Size = new System.Drawing.Size(40, 23);
			this.addParam.TabIndex = 7;
			this.addParam.Text = "&Add";
			this.addParam.Click += new System.EventHandler(this.addParam_Click);
			// 
			// suggestParam
			// 
			this.suggestParam.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.suggestParam.ItemHeight = 13;
			this.suggestParam.Location = new System.Drawing.Point(8, 8);
			this.suggestParam.Name = "suggestParam";
			this.suggestParam.Size = new System.Drawing.Size(130, 21);
			this.suggestParam.TabIndex = 8;
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
			this.queryParams.Location = new System.Drawing.Point(0, 38);
			this.queryParams.MultiSelect = false;
			this.queryParams.Name = "queryParams";
			this.queryParams.RightToLeft = System.Windows.Forms.RightToLeft.No;
			this.queryParams.Size = new System.Drawing.Size(232, 128);
			this.queryParams.TabIndex = 0;
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
			// resultsTab
			// 
			this.resultsTab.Controls.Add(this.sqlTab);
			this.resultsTab.Controls.Add(this.exceptionTab);
			this.resultsTab.Controls.Add(this.objectGraphTab);
			this.resultsTab.Controls.Add(this.dbResultsTab);
			this.resultsTab.Dock = System.Windows.Forms.DockStyle.Fill;
			this.resultsTab.ItemSize = new System.Drawing.Size(42, 18);
			this.resultsTab.Location = new System.Drawing.Point(0, 176);
			this.resultsTab.Name = "resultsTab";
			this.resultsTab.SelectedIndex = 0;
			this.resultsTab.Size = new System.Drawing.Size(696, 182);
			this.resultsTab.TabIndex = 10;
			// 
			// sqlTab
			// 
			this.sqlTab.Controls.Add(this.sqlText);
			this.sqlTab.Location = new System.Drawing.Point(4, 22);
			this.sqlTab.Name = "sqlTab";
			this.sqlTab.Size = new System.Drawing.Size(688, 156);
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
			this.sqlText.Size = new System.Drawing.Size(688, 156);
			this.sqlText.TabIndex = 12;
			this.sqlText.Text = "";
			// 
			// exceptionTab
			// 
			this.exceptionTab.Controls.Add(this.exceptionsSplitter);
			this.exceptionTab.Controls.Add(this.exceptionsText);
			this.exceptionTab.Controls.Add(this.exceptionsList);
			this.exceptionTab.Controls.Add(this.errorsTabSplitter);
			this.exceptionTab.Location = new System.Drawing.Point(4, 22);
			this.exceptionTab.Name = "exceptionTab";
			this.exceptionTab.Size = new System.Drawing.Size(688, 156);
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
			this.exceptionsSplitter.Size = new System.Drawing.Size(688, 5);
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
			this.exceptionsText.Size = new System.Drawing.Size(688, 100);
			this.exceptionsText.TabIndex = 16;
			this.exceptionsText.Text = "";
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
			this.exceptionsList.Size = new System.Drawing.Size(688, 53);
			this.exceptionsList.TabIndex = 17;
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
			this.errorsTabSplitter.Size = new System.Drawing.Size(688, 3);
			this.errorsTabSplitter.TabIndex = 18;
			this.errorsTabSplitter.TabStop = false;
			// 
			// objectGraphTab
			// 
			this.objectGraphTab.Controls.Add(this.objectGraph);
			this.objectGraphTab.Location = new System.Drawing.Point(4, 22);
			this.objectGraphTab.Name = "objectGraphTab";
			this.objectGraphTab.Size = new System.Drawing.Size(688, 156);
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
			this.objectGraph.Size = new System.Drawing.Size(688, 156);
			this.objectGraph.TabIndex = 20;
			// 
			// dbResultsTab
			// 
			this.dbResultsTab.Controls.Add(this.sqlResultDataGrid);
			this.dbResultsTab.Location = new System.Drawing.Point(4, 22);
			this.dbResultsTab.Name = "dbResultsTab";
			this.dbResultsTab.Size = new System.Drawing.Size(688, 156);
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
			this.sqlResultDataGrid.Size = new System.Drawing.Size(688, 156);
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
			this.mainSplitter.Location = new System.Drawing.Point(0, 168);
			this.mainSplitter.Name = "mainSplitter";
			this.mainSplitter.ParentForm = null;
			this.mainSplitter.Size = new System.Drawing.Size(696, 8);
			this.mainSplitter.TabIndex = 23;
			this.mainSplitter.TabStop = false;
			this.mainSplitter.VisualStyle = Furty.Windows.Forms.CollapsibleSplitter.VisualStyles.Mozilla;
			// 
			// QueryForm
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(696, 358);
			this.Controls.Add(this.resultsTab);
			this.Controls.Add(this.mainSplitter);
			this.Controls.Add(this.queryPanel);
			this.Name = "QueryForm";
			this.Text = "Query";
			this.Load += new System.EventHandler(this.QueryForm_Load);
			this.queryPanel.ResumeLayout(false);
			this.paramsPanel.ResumeLayout(false);
			this.resultsTab.ResumeLayout(false);
			this.sqlTab.ResumeLayout(false);
			this.exceptionTab.ResumeLayout(false);
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
		}
	}
}