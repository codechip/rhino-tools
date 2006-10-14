using System;
using System.Windows.Forms;
using Ayende.NHibernateQueryAnalyzer.SchemaEditing;


namespace Ayende.NHibernateQueryAnalyzer.UserInterface.SchemaEditing
{
	public class WinFormsSchemaEditorNodeFactory:ISchemaEditorNodeFactory
	{

		SchemaEditorView view;

		public WinFormsSchemaEditorNodeFactory(SchemaEditorView view)
		{
			this.view = view;
		}

		/// <summary>
		/// Create a node  that contains the obj as the internal value.
		/// </summary>
		public ISchemaEditorNode CreateNode(NodeFieldReference fieldReference, object obj, string name)
		{
			WinFormsSchemaEditorNode node = new WinFormsSchemaEditorNode(view, fieldReference,obj,name);

			return node;
		}

		public ISchemaEditorNode CreateRoot(RootNodeFieldReference reference, object rootObj, string name)
		{
			ISchemaEditorNode node = CreateNode(reference, rootObj, name);
			view.GraphTree.Nodes.Add((TreeNode)node);
			return node;
		}
	}
}
