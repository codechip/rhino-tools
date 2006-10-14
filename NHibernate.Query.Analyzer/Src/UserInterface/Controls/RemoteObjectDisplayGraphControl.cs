using System;
using System.Collections;
using System.Drawing;
using System.Windows.Forms;
using Ayende.NHibernateQueryAnalyzer.ProjectLoader;
using Ayende.NHibernateQueryAnalyzer.Utilities;
using Netron.Lithium;

namespace Ayende.NHibernateQueryAnalyzer.UserInterface.Controls
{
	/// <summary>
	/// Summary description for ObjectDisplayGraphControl.
	/// </summary>
	public class RemoteObjectDisplayGraphControl : GraphControl
	{
		private object obj;
		private bool firstExpand = true;
		private ArrayList childrenNotExpanded = new ArrayList();
		private ArrayList childrenNames = new ArrayList();

		public ListView Properties
		{
			get { return (ListView) Control; }
			set { Control = value; }
		}

		public RemoteObjectDisplayGraphControl()
		{
			Control = new ListView();
			Properties.Columns.Add("Name:", 100, HorizontalAlignment.Left);
			Properties.Columns.Add("Value", 100, HorizontalAlignment.Right);
			Properties.Size = new Size(230, 120);
			Properties.MultiSelect = false;
			Properties.GridLines = true;
			Properties.FullRowSelect = true;
			Properties.HeaderStyle = ColumnHeaderStyle.Nonclickable;
			Properties.View = View.Details;
		}

		public object DisplayedObject
		{
			get { return obj; }
		}

		public void DisplayObject(object value)
		{
			obj = value;
			Properties.Items.Clear();
			if (obj == null)
				return;
			if (obj is IList)
				DisplayList((IList) obj);
			else if (obj is IDictionary)
				DisplayDictionary((IDictionary) obj);
			else
				DisplayProperties((RemoteObject) obj);
		}

		private void DisplayList(IList list)
		{
			if (list.Count == 0)
			{
				Properties.Items.Add("Empty collection");
				return;
			}
			int index = 0;
			foreach (RemoteObject ro in list)
			{
				string nodeTitle = Text+"[" + index + "]";
				DisplayProperty(ro, nodeTitle, nodeTitle);
				index++;
			}

		}
		//TODO: Implement when I've a test for this
		private void DisplayDictionary(IDictionary dic)
		{
			if (dic.Count == 0 )
			{
				Properties.Items.Add("Empty collection");
				return;
			}
			int index = 0;
			foreach (DictionaryEntry de in dic)
			{
				string indexStr = Text+"[" + ReflectionUtil.GetName(de.Key) + "]";
				DisplayProperty(de.Value, indexStr, indexStr);
				index++;
			}
		}

		private void DisplayProperty(object obj, string nodeTitle, string listTitle)
		{
			if (obj is string)
			{
				Properties.Items.Add(new ListViewItem(new string[] {listTitle, obj.ToString()}));
			}
			else
			{
				AddChildObject(nodeTitle, obj);
			}
		}

		private void AddChildObject(string title, object childObject)
		{
			childrenNotExpanded.Add(childObject);
			childrenNames.Add(title);
		}

		private void CreateChild(string title, object childObject)
		{
			RemoteObjectDisplayGraphControl child = new RemoteObjectDisplayGraphControl();
			child.Text = title;
			this.AddChild(child);
			child.DisplayObject(childObject);
		}

		private void DisplayProperties(RemoteObject ro)
		{
			foreach (string prop in ro.Properties)
			{
				DisplayProperty(ro[prop], prop, prop);
			}
		}

		protected override void OnBeforeExpand()
		{
			if (firstExpand)
			{
				for (int i = 0; i < childrenNotExpanded.Count; i++)
				{
					CreateChild((string) childrenNames[i], childrenNotExpanded[i]);
				}
				firstExpand = false;
				childrenNotExpanded.Clear();
				childrenNames.Clear();
			}
		}

		protected override bool HasChildren
		{
			get { return childrenNotExpanded.Count > 0 || base.HasChildren; }
		}

		protected override bool CheveronActive
		{
			get { return Properties.Items.Count > 0; }
		}
	}
}