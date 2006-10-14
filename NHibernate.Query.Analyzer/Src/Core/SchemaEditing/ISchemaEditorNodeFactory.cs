namespace Ayende.NHibernateQueryAnalyzer.SchemaEditing
{
	public interface ISchemaEditorNodeFactory
	{
		/// <summary>
		/// Create a node  that contains the obj as the internal value.
		/// </summary>
		ISchemaEditorNode CreateNode(NodeFieldReference fieldReference, object obj, string name);

		ISchemaEditorNode CreateRoot(RootNodeFieldReference reference, object rootObj, string name);
	}
}