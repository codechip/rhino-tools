using System;
using System.Collections;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Reflection;
using System.Windows.Forms;
using System.Xml.Serialization;
using Ayende.NHibernateQueryAnalyzer.SchemaEditing;

namespace Ayende.NHibernateQueryAnalyzer.UserInterface.Controls
{
	/// <summary>
	/// Summary description for EditAttribute.
	/// </summary>
	public class EditAttribute : Form
	{
		private Button cancel;
		private Button ok;
		private TextBox valueText;
		private Button clear;
		private System.Windows.Forms.Label valueLbl;
		private System.Windows.Forms.ComboBox enumList;
		private System.Windows.Forms.CheckBox boolCheck;

		/// <summary>
		/// Required designer variable.
		/// </summary>
		private Container components = null;

		private AttributeFieldReference attribute;
		private StringDictionary fieldNames;
		private bool changesMade = false;

		public EditAttribute(AttributeFieldReference attribute)
		{
			InitializeComponent();
			Text = "Edit Attribute: " + attribute.Name;
			this.attribute = attribute;
			DisplayValue();
			clear.Enabled = attribute.HasValue;
		}

		private void DisplayValue()
		{
			if(attribute.Type.IsEnum)
			{
				valueText.Visible = false;
				boolCheck.Visible = false;
				enumList.Visible = true;
				foreach(string val in GetEnumXmlNames(attribute.Type))
				{
					enumList.Items.Add(val);
				}
				enumList.SelectedItem = fieldNames[attribute.Value.ToString()];
				enumList.Select();
			}
			else if (attribute.Type == typeof(bool))
			{
				valueText.Visible = false;
				enumList.Visible = false;
				boolCheck.Visible = true;
				boolCheck.Text = attribute.Name;
				boolCheck.CheckState = ((bool)attribute.Value ) ? CheckState.Checked :  CheckState.Unchecked;				
				boolCheck.Select();
			}
			else
			{
				boolCheck.Visible = false;
				enumList.Visible = false;
				valueText.Visible = true;
				valueText.Text = attribute.Value ==null ? "" : attribute.Value.ToString();
				valueText.Select();
			}
		}

		private ICollection GetEnumXmlNames(Type type)
		{
			fieldNames = SchemaEditor.GetEnumFieldsAndXMLNames(type);
			return fieldNames.Values;
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

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.cancel = new System.Windows.Forms.Button();
			this.ok = new System.Windows.Forms.Button();
			this.valueText = new System.Windows.Forms.TextBox();
			this.valueLbl = new System.Windows.Forms.Label();
			this.clear = new System.Windows.Forms.Button();
			this.enumList = new System.Windows.Forms.ComboBox();
			this.boolCheck = new System.Windows.Forms.CheckBox();
			this.SuspendLayout();
			// 
			// cancel
			// 
			this.cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.cancel.Location = new System.Drawing.Point(464, 48);
			this.cancel.Name = "cancel";
			this.cancel.TabIndex = 15;
			this.cancel.Text = "&Cancel";
			this.cancel.Click += new System.EventHandler(this.cancel_Click);
			// 
			// ok
			// 
			this.ok.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.ok.Location = new System.Drawing.Point(368, 48);
			this.ok.Name = "ok";
			this.ok.TabIndex = 14;
			this.ok.Text = "&OK";
			this.ok.Click += new System.EventHandler(this.ok_Click);
			// 
			// valueText
			// 
			this.valueText.Location = new System.Drawing.Point(72, 16);
			this.valueText.Name = "valueText";
			this.valueText.Size = new System.Drawing.Size(464, 20);
			this.valueText.TabIndex = 9;
			this.valueText.Text = "";
			this.valueText.TextChanged += new System.EventHandler(this.valueText_TextChanged);
			// 
			// valueLbl
			// 
			this.valueLbl.Location = new System.Drawing.Point(16, 16);
			this.valueLbl.Name = "valueLbl";
			this.valueLbl.Size = new System.Drawing.Size(56, 23);
			this.valueLbl.TabIndex = 8;
			this.valueLbl.Text = "Value:";
			// 
			// clear
			// 
			this.clear.DialogResult = System.Windows.Forms.DialogResult.Abort;
			this.clear.Location = new System.Drawing.Point(16, 48);
			this.clear.Name = "clear";
			this.clear.Size = new System.Drawing.Size(120, 23);
			this.clear.TabIndex = 16;
			this.clear.Text = "&Default Value";
			this.clear.Click += new System.EventHandler(this.clear_Click);
			// 
			// enumList
			// 
			this.enumList.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.enumList.Location = new System.Drawing.Point(72, 16);
			this.enumList.Name = "enumList";
			this.enumList.Size = new System.Drawing.Size(464, 21);
			this.enumList.TabIndex = 17;
			this.enumList.Visible = false;
			this.enumList.SelectedIndexChanged += new System.EventHandler(this.enumList_SelectedIndexChanged);
			// 
			// boolCheck
			// 
			this.boolCheck.Location = new System.Drawing.Point(16, 16);
			this.boolCheck.Name = "boolCheck";
			this.boolCheck.Size = new System.Drawing.Size(520, 24);
			this.boolCheck.TabIndex = 18;
			this.boolCheck.Visible = false;
			this.boolCheck.CheckedChanged += new System.EventHandler(this.boolCheck_CheckedChanged);
			// 
			// EditAttribute
			// 
			this.AcceptButton = this.ok;
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.CancelButton = this.cancel;
			this.ClientSize = new System.Drawing.Size(552, 78);
			this.ControlBox = false;
			this.Controls.Add(this.boolCheck);
			this.Controls.Add(this.enumList);
			this.Controls.Add(this.clear);
			this.Controls.Add(this.cancel);
			this.Controls.Add(this.ok);
			this.Controls.Add(this.valueText);
			this.Controls.Add(this.valueLbl);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.Name = "EditAttribute";
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "EditAttribute";
			this.ResumeLayout(false);

		}

		#endregion

		private void clear_Click(object sender, EventArgs e)
		{
			valueText.Text = "";
			this.Close();
		}

		private void ok_Click(object sender, EventArgs e)
		{
			DialogResult = changesMade ? DialogResult.OK : DialogResult.Cancel;
			this.Close();
		}

		private void cancel_Click(object sender, EventArgs e)
		{
			this.Close();
		}

		private void valueText_TextChanged(object sender, System.EventArgs e)
		{
			changesMade = true;
		}

		private void boolCheck_CheckedChanged(object sender, System.EventArgs e)
		{
			ok.Enabled = true;
			changesMade = true;
		}

		private void enumList_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			changesMade = true;
			ok.Enabled = true;
		}

		public object AttributeValue
		{
			get
			{
				if(attribute.Type.IsEnum)
				{
					return GetEnumValue();
				}
				else if (attribute.Type == typeof(bool))
				{
					return boolCheck.CheckState == CheckState.Checked;
				}
				return valueText.Text == "" ? null : valueText.Text;
			}
		}

		private object GetEnumValue()
		{
			string fieldName = null;
			foreach (DictionaryEntry entry in fieldNames)
			{
				if(entry.Value == enumList.SelectedItem)
				{
					fieldName = (string)entry.Key;
					break;
				}
			}
			return Enum.Parse(attribute.Type, fieldName);
		}
	}
}