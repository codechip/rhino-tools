using System;
using Ayende.NHibernateQueryAnalyzer.SchemaEditing;

namespace Ayende.NHibernateQueryAnalyzer.Tests.SchemaEditing
{
	/// <summary>
	/// Summary description for SchemaEditorNodeTestFactory.
	/// </summary>
	public class SchemaEditorNodeTestFactory : ISchemaEditorNodeFactory
	{
		/// <summary>
		/// Create a node that is an active node of the parent node and that contains the obj
		/// as the internal value.
		/// </summary>
		/// <returns>The newly created node, already registered as an active node of the parent node.</returns>
		public ISchemaEditorNode CreateNode(NodeFieldReference fieldReference, object obj, string name)
		{
			ISchemaEditorNode child = new SchemaEditorTestNode(name, obj, fieldReference);
			return child;
		}

		public ISchemaEditorNode CreateRoot(RootNodeFieldReference reference, object rootObj, string name)
		{
			return CreateNode(reference,rootObj, name);
		}

	}
}