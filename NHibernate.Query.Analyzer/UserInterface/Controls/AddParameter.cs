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
using System.Windows.Forms;
using Ayende.NHibernateQueryAnalyzer.Utilities;

namespace Ayende.NHibernateQueryAnalyzer.UserInterface.Controls
{
	/// <summary>
	/// Summary description for AddParameter.
	/// </summary>
	public class AddParameter : System.Windows.Forms.Form
	{
		private System.Windows.Forms.Label nameLbl;
		private System.Windows.Forms.TextBox nameText;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.ComboBox typesList;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.TextBox valueText;
		private System.Windows.Forms.Button ok;
		private System.Windows.Forms.Button cancel;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		private TypedParameter param;
		private Hashtable typesDic;

		public AddParameter(string paramName)
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
			CreateTypesDictionary();
			nameText.Text = paramName;
		}

		public AddParameter(TypedParameter parameter) : this(parameter.Name)
		{
			typesList.SelectedText = parameter.Type.Name;
			valueText.Text = parameter.Value.ToString();
		}

		private void CreateTypesDictionary()
		{
			typesDic = new Hashtable();
			typesDic["DateTime"] = typeof(DateTime);
			typesDic["boolean"] = typeof(bool);
			typesDic["string"] = typeof(string);
			typesDic["int"] = typeof(int);
			typesList.Items.AddRange(new string[]{"int","string","boolean","DateTime"});
			typesList.SelectedItem = "int";
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

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.nameLbl = new System.Windows.Forms.Label();
			this.nameText = new System.Windows.Forms.TextBox();
			this.label2 = new System.Windows.Forms.Label();
			this.typesList = new System.Windows.Forms.ComboBox();
			this.label3 = new System.Windows.Forms.Label();
			this.valueText = new System.Windows.Forms.TextBox();
			this.ok = new System.Windows.Forms.Button();
			this.cancel = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// nameLbl
			// 
			this.nameLbl.Location = new System.Drawing.Point(8, 8);
			this.nameLbl.Name = "nameLbl";
			this.nameLbl.Size = new System.Drawing.Size(56, 23);
			this.nameLbl.TabIndex = 0;
			this.nameLbl.Text = "Name:";
			// 
			// nameText
			// 
			this.nameText.Location = new System.Drawing.Point(72, 8);
			this.nameText.Name = "nameText";
			this.nameText.Size = new System.Drawing.Size(112, 20);
			this.nameText.TabIndex = 1;
			this.nameText.Text = "";
			this.nameText.TextChanged += new System.EventHandler(this.parameterInfoChanged);
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(200, 8);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(40, 23);
			this.label2.TabIndex = 2;
			this.label2.Text = "Type:";
			// 
			// typesList
			// 
			this.typesList.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.typesList.Location = new System.Drawing.Point(248, 8);
			this.typesList.Name = "typesList";
			this.typesList.Size = new System.Drawing.Size(121, 21);
			this.typesList.TabIndex = 3;
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(384, 8);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(40, 23);
			this.label3.TabIndex = 4;
			this.label3.Text = "Value:";
			// 
			// valueText
			// 
			this.valueText.Location = new System.Drawing.Point(440, 8);
			this.valueText.Name = "valueText";
			this.valueText.Size = new System.Drawing.Size(104, 20);
			this.valueText.TabIndex = 5;
			this.valueText.Text = "";
			this.valueText.TextChanged += new System.EventHandler(this.parameterInfoChanged);
			// 
			// ok
			// 
			this.ok.Enabled = false;
			this.ok.Location = new System.Drawing.Point(376, 40);
			this.ok.Name = "ok";
			this.ok.TabIndex = 6;
			this.ok.Text = "&OK";
			this.ok.Click += new System.EventHandler(this.ok_Click);
			// 
			// cancel
			// 
			this.cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.cancel.Location = new System.Drawing.Point(472, 40);
			this.cancel.Name = "cancel";
			this.cancel.TabIndex = 7;
			this.cancel.Text = "&Cancel";
			// 
			// AddParameter
			// 
			this.AcceptButton = this.ok;
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.CancelButton = this.cancel;
			this.ClientSize = new System.Drawing.Size(560, 70);
			this.ControlBox = false;
			this.Controls.Add(this.cancel);
			this.Controls.Add(this.ok);
			this.Controls.Add(this.valueText);
			this.Controls.Add(this.nameText);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.typesList);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.nameLbl);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.Name = "AddParameter";
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Add Parameter:";
			this.ResumeLayout(false);

		}
		#endregion

		private void parameterInfoChanged(object sender, System.EventArgs e)
		{
			ok.Enabled = nameText.Text!="" && 
				valueText.Text!="";
		}

		private void ok_Click(object sender, System.EventArgs e)
		{
			object val=null;
			Type type = (Type)typesDic[typesList.Text];
			try
			{
				val = Convert.ChangeType(valueText.Text,type);
			}
			catch(Exception )
			{
				MessageBox.Show("Can't convert '"+valueText.Text+"' to "+ typesList.Text);
				return;
			}
			param = new TypedParameter(nameText.Text,
				type,val);
			DialogResult = DialogResult.OK;
			this.Close();
		}

		public TypedParameter Parameter
		{
			get
			{
				return param;
			}
		}
	}
}
