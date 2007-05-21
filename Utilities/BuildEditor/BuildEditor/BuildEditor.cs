using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Xml;
using System.Diagnostics;

namespace BuildEditor
{
	public partial class BuildEditor : Form
	{
		private string fileName;
		FileSystemWatcher watcher;

		public BuildEditor()
		{
			InitializeComponent();
		}

		private Dictionary<string, string> options = new Dictionary<string, string>();
		private Dictionary<string, string> originalOptions = new Dictionary<string, string>();

		private void OpenToolBarItem_Click(object sender, EventArgs e)
		{
			using (OpenFileDialog ofd = new OpenFileDialog())
			{
				ofd.Multiselect = false;
				ofd.Filter = "Build Files | *.xml;*.build";
				if (ofd.ShowDialog(this) != DialogResult.OK)
					return;
				if (watcher != null)
					watcher.Dispose();
				fileName = ofd.FileName;
				fileNameLabel.Text = fileName;
				LoadFile();
				watcher = new FileSystemWatcher(Path.GetDirectoryName(fileName), Path.GetFileName(fileName));
				watcher.Changed += delegate { Invoke((Proc)LoadFile); };
				watcher.Created += delegate { Invoke((Proc)LoadFile); };
				watcher.EnableRaisingEvents = true;
			}
		}

		public delegate void Proc();
		private void LoadFile()
		{
			Environment.CurrentDirectory = Path.GetDirectoryName(fileName);
			options.Clear();

			XmlDocument xdoc = new XmlDocument();
			xdoc.Load(fileName);

			XmlNamespaceManager nsMgr = new XmlNamespaceManager(xdoc.NameTable);
			nsMgr.AddNamespace("build", "http://schemas.microsoft.com/developer/msbuild/2003");

			XmlNode node = xdoc.SelectSingleNode("/build:Project/build:PropertyGroup", nsMgr);
			if (node == null)
				return;
			parametersTable.AutoScroll = false;
			parametersTable.SuspendLayout();
			foreach (Control control in new ArrayList(parametersTable.Controls))
			{
				if(parametersTable.GetRow(control)!=0)
					parametersTable.Controls.Remove(control);
			}
			foreach (XmlNode xmlNode in node.ChildNodes)
			{
				if (xmlNode.NodeType != XmlNodeType.Element)
					continue;
				Label name = new Label();
				string nodeName = xmlNode.Name;
				name.Text = SplitPascalCase(nodeName).Replace("_", " ");
				name.Width = 190;
				bool isBoolean, initial;
				isBoolean = bool.TryParse(xmlNode.InnerText, out initial);
				Control val;
				BuildSpecialCase specialCase = GetSpecialCaseControl(xmlNode.Name);
				if (isBoolean)
				{
					CheckBox ck = new CheckBox();
					originalOptions[nodeName] = initial.ToString();
					ck.CheckedChanged += delegate
											{
												options[nodeName] = ck.Checked.ToString();
												Changed();
											};
					ck.Checked = initial;
					val = ck;
				}
				else if (specialCase != null)
				{
					Panel panel = new Panel();
					panel.Dock = DockStyle.Fill;
					TextBox t = new TextBox();
					originalOptions[nodeName] = xmlNode.InnerText;
					t.TextChanged += delegate
										{
											options[nodeName] = t.Text;
											Changed();
										};
					t.Text = xmlNode.InnerText;
					t.Dock = DockStyle.Fill;


					panel.Controls.Add(t);
					panel.Controls.Add(specialCase(t));
					panel.Height = t.Height;
					val = panel;
				}
				else
				{
					TextBox t = new TextBox();
					originalOptions[nodeName] = xmlNode.InnerText;
					t.TextChanged += delegate
										{
											options[nodeName] = t.Text;
											Changed();
										};
					t.Text = xmlNode.InnerText;
					t.Dock = DockStyle.Fill;
					val = t;
				}
				this.parametersTable.RowCount += 1;
				this.parametersTable.Controls.Add(name, 0, this.parametersTable.RowCount - 1);
				this.parametersTable.Controls.Add(val, 1, this.parametersTable.RowCount - 1);
			}
			this.parametersTable.RowCount += 1;
			parametersTable.ResumeLayout(true);
			parametersTable.AutoScroll = true;
		}

		public delegate Control BuildSpecialCase(TextBox textbox);

		private BuildSpecialCase GetSpecialCaseControl(string name)
		{
			if (name.IndexOf("ConnectionString", StringComparison.InvariantCultureIgnoreCase) != -1)
			{
				return delegate(TextBox t)
				{
					Button change = new Button();
					change.Click += delegate { t.Text = PromptForConnectionString(t.Text); };
					change.Dock = DockStyle.Right;
					change.Text = "...";
					change.Width = 30;
					return change;
				};
			}
			if (name.IndexOf("File", StringComparison.InvariantCultureIgnoreCase) != -1)
			{
				return delegate(TextBox t)
				{
					Button change = new Button();
					change.Click += delegate
					{
						OpenFileDialog dialog = new OpenFileDialog();
						dialog.Multiselect = false;
						dialog.FileName = t.Text;
						if (dialog.ShowDialog(this) == DialogResult.OK)
						{
							t.Text = dialog.FileName;
						}
					};
					change.Dock = DockStyle.Right;
					change.Text = "...";
					change.Width = 30;
					return change;
				};
			}
			if (name.IndexOf("Folder", StringComparison.InvariantCultureIgnoreCase) != -1 ||
				name.IndexOf("Dir", StringComparison.InvariantCultureIgnoreCase) != -1)
			{
				return delegate(TextBox t)
				{
					Button change = new Button();
					change.Click += delegate
					{
						FolderBrowserDialog dialog = new FolderBrowserDialog();
						dialog.SelectedPath = t.Text;
						if (dialog.ShowDialog(this) == DialogResult.OK)
						{
							t.Text = dialog.SelectedPath;
						}
					};
					change.Dock = DockStyle.Right;
					change.Text = "...";
					change.Width = 30;
					return change;
				};
			}
			return null;
		}

		private void Changed()
		{
			command.Text = BuildCommandArguments();
		}

		private string BuildCommandArguments()
		{
			StringBuilder sb = new StringBuilder();
			sb.Append(@"c:\WINDOWS\Microsoft.NET\Framework\v2.0.50727\MsBuild ");
			sb.Append(fileName).Append(" ");
			foreach (KeyValuePair<string, string> pair in options)
			{
				if (pair.Value == originalOptions[pair.Key])
					continue;
				sb.Append(" /p:\"").Append(pair.Key)
					.Append("=")
					.Append(pair.Value)
					.Append("\" ");
			}
			return sb.ToString();
		}


		/// <summary>
		/// Split a PascalCase string into Pascal Case words.
		/// Note that if the string contains spaces, we assume it is already formatted
		/// http://weblogs.asp.net/jgalloway/archive/2005/09/27/426087.aspx
		/// </summary>
		private static string SplitPascalCase(string input)
		{
			if (input.Contains(" "))
				return input;
			string result = Regex.Replace(input, "((?<![A-Z])[A-Z])", " $1", RegexOptions.Compiled);
			return result;
		}

		private void exec_Click(object sender, EventArgs e)
		{
			Process ppc = new Process();
			ppc.StartInfo.FileName = "cmd.exe";
			ppc.StartInfo.Arguments = "/k " + BuildCommandArguments();
			ppc.Start();
		}

		private void BuildEditor_Shown(object sender, EventArgs e)
		{
			OpenToolBarItem_Click(sender, e);
		}

		/// <summary>
		/// Displays a Connection String Builder (DataLinks) dialog.
		/// 
		/// Credits:
		/// http://www.codeproject.com/cs/database/DataLinks.asp
		/// http://www.codeproject.com/cs/database/DataLinks.asp?df=100&forumid=33457&select=1560237#xx1560237xx
		/// 
		/// Required COM references:
		/// %PROGRAMFILES%\Microsoft.NET\Primary Interop Assemblies\adodb.dll
		/// %PROGRAMFILES%\Common Files\System\Ole DB\OLEDB32.DLL
		/// </summary>
		/// <param name="currentConnectionString">Previous database connection string</param>
		/// <returns>Selected connection string</returns>
		private string PromptForConnectionString(string currentConnectionString)
		{
			MSDASC.DataLinks dataLinks = new MSDASC.DataLinksClass();
			ADODB.Connection dialogConnection;
			string generatedConnectionString = string.Empty;

			if (currentConnectionString == String.Empty)
			{
				dialogConnection = (ADODB.Connection)dataLinks.PromptNew();
				generatedConnectionString = dialogConnection.ConnectionString.ToString();
			}
			else
			{
				dialogConnection = new ADODB.Connection();
				dialogConnection.Provider = "SQLOLEDB.1";
				ADODB.Property persistProperty = dialogConnection.Properties["Persist Security Info"];
				persistProperty.Value = true;

				dialogConnection.ConnectionString = currentConnectionString;
				dataLinks = new MSDASC.DataLinks();

				object objConn = dialogConnection;
				if (dataLinks.PromptEdit(ref objConn))
				{
					generatedConnectionString = dialogConnection.ConnectionString.ToString();
				}
				else
				{
					return currentConnectionString;
				}
			}
			generatedConnectionString = generatedConnectionString.Replace("Provider=SQLOLEDB.1;", string.Empty);
			if (
					!generatedConnectionString.Contains("Integrated Security=SSPI")
					&& !generatedConnectionString.Contains("Trusted_Connection=True")
					&& !generatedConnectionString.Contains("Password=")
					&& !generatedConnectionString.Contains("Pwd=")
				)
			{
				if (dialogConnection.Properties["Password"] != null)
					generatedConnectionString += ";Password=" + dialogConnection.Properties["Password"].Value;
			}

			return generatedConnectionString;
		}

	}
}