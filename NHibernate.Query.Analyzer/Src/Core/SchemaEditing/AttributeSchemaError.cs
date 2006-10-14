using System;

namespace Ayende.NHibernateQueryAnalyzer.SchemaEditing
{
	/// <summary>
	/// Summary description for AttributeSchemaError.
	/// </summary>
	public class AttributeSchemaError : ISchemaError
	{
		IFieldReference attribute;
		ISchemaEditorNode parentNode;
		private const string errMsg = "Attribute '{0}' on element '{1}' is required!";

		public IFieldReference Field
		{
			get { return attribute; }
		}

		public ISchemaEditorNode ParentNode
		{
			get { return parentNode; }
		}

		public string Message
		{
			get { return string.Format(errMsg, attribute.Name, parentNode.Name); }
		}

		public AttributeSchemaError(IFieldReference attribute, ISchemaEditorNode parentNode)
		{
			this.attribute = attribute;
			this.parentNode = parentNode;
		}
	}
}
