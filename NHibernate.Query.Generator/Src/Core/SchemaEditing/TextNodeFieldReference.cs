using System;
using System.Reflection;

namespace Ayende.NHibernateQueryAnalyzer.SchemaEditing
{
	/// <summary>
	/// Summary description for TextNodeFieldReference.
	/// </summary>
	public class TextNodeFieldReference : AttributeFieldReference
	{
		public TextNodeFieldReference(object parent, FieldInfo field)
			:base(parent, field, null) {}

		public override string Name
		{
			get { return "text()"; }
		}

		public override object Value
		{
			get
			{
				object value = base.Value;
				if(value==null)
					return null;
				return ((string[])value)[0];
			}
			set
			{
				base.Value = new string[]{(string)value};
			}
		}

		public override Type Type
		{
			get { return typeof(string); }
		}
	}
}
