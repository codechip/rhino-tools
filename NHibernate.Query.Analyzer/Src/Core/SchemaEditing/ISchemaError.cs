namespace Ayende.NHibernateQueryAnalyzer.SchemaEditing
{
	public interface ISchemaError
	{
		IFieldReference Field { get; }

		ISchemaEditorNode ParentNode { get; }

		string Message { get; }
	}
}//