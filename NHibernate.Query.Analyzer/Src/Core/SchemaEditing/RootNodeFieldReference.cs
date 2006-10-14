using System;
using System.Reflection;
using Ayende.NHibernateQueryAnalyzer.Utilities;

namespace Ayende.NHibernateQueryAnalyzer.SchemaEditing
{
	/// <summary>
	/// Summary description for RootFieldReference.
	/// </summary>
	public class RootNodeFieldReference : NodeFieldReference
	{
		private object val;

		public RootNodeFieldReference(object val)
		{
			if(ReflectionUtil.HasValue(val)==false)
				throw new ArgumentException("Val must have a valid value.");
			this.val = val;
			this.amountRequired = 1;
		}

		public override string[] Names
		{
			get { throw new InvalidOperationException("Root has no names"); }
		}

		public override Type[] Types
		{
			get { throw new InvalidOperationException("Root has no types");; }
		}

		public override object Value
		{
			get { return val; }
			set { throw new InvalidOperationException("Can't set root's value, it's immutable"); }
		}

		/// <summary>
		/// Return true if the inner value is an array with 
		/// elements or a non-null reference.
		/// </summary>
		public override bool HasValue
		{
			get { return true; }
		}

		public override ISchemaEditorNode ParentNode
		{
			get { return parentNode; }
			set { parentNode = value; }
		}

		public override ISchemaEditorNode AddValue(object obj)
		{
			throw new InvalidOperationException("Can't add value to root node");			
		}

		public override ISchemaEditorNode AddExistingValue(object obj)
		{
			throw new InvalidOperationException("Can't add value to root node");			
		}

		public override void RemoveValue(ISchemaEditorNode node)
		{
			throw new InvalidOperationException("Can't remove value from root node");
		}
	}
}
