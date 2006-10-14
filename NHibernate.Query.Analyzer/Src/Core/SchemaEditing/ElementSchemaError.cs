using System;
using Ayende.NHibernateQueryAnalyzer.Utilities;

namespace Ayende.NHibernateQueryAnalyzer.SchemaEditing
{
	/// <summary>
	/// Summary description for ElementSchemaError.
	/// </summary>
	public class ElementSchemaError : ISchemaError
	{
		NodeFieldReference field;

		ISchemaEditorNode parentNode;
		private const string oneOptionErrMsg = "The element '{0}' is required to appear on '{1}' {2} time(s), but appears {3} time(s)!",
			errMsg = "Some of the elements ({0}) are required to appear on '{1}' {2} time(s), but appears {3} time(s)!";

		public ElementSchemaError(NodeFieldReference field, ISchemaEditorNode parentNode)
		{
			this.field = field;
			this.parentNode = parentNode;
		}

		public NodeFieldReference Field
		{
			get { return field; }
		}

		public ISchemaEditorNode ParentNode
		{
			get { return parentNode; }
		}

		public string Message
		{
			get
			{
				if(field.Names.Length==1)
					return string.Format(oneOptionErrMsg,
						field.Name,
						parentNode.Name,
						field.AmountRequired, 
						field.AmountExisting);
				else
					return string.Format(errMsg,
						Text.Join(", ",field.Names),
						parentNode.Name,
						field.AmountRequired, 
						field.AmountExisting);
			}
		}

		IFieldReference ISchemaError.Field
		{
			get { return field; }
		}
	}
}
