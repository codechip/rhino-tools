using System;
using System.ComponentModel;
using System.Reflection;
using System.Xml.Serialization;
using Ayende.NHibernateQueryAnalyzer.Utilities;

namespace Ayende.NHibernateQueryAnalyzer.SchemaEditing
{
	/// <summary>
	/// Used to reference a field in a class, can get it's value and set it.
	/// Handle arrays and simple values (string, enums) only.
	/// </summary>
	public class AttributeFieldReference : IFieldReference
	{
		#region Variables

		public delegate void AttributeFieldReferenceEventHandler(AttributeFieldReference sender, AttributeFieldReferenceEventArgs e);

		public event AttributeFieldReferenceEventHandler HasValueChanged;

		protected bool isRequired;
		protected object parent;
		protected FieldInfo field, fieldSpecified;
		protected string name;
		private object defaultValue;

		#endregion

		#region Constructor

		public AttributeFieldReference(object parent, FieldInfo field, FieldInfo fieldSpecified)
		{
			Validation.NotNull(parent, field);
			this.parent = parent;
			this.field = field;
			this.fieldSpecified = fieldSpecified;
			this.isRequired = IsRequiredAttribute(field);
			SetNameAndType(field);
		}

		private static bool IsRequiredAttribute(FieldInfo field)
		{
			return field.GetCustomAttributes(typeof(RequiredTagAttribute),false).Length>0;
		}

		#endregion

		#region Properties

		public object DefaultValue
		{
			get { return defaultValue; }
		}

		public virtual bool IsRequired
		{
			get { return isRequired; }
		}

		public virtual object Value
		{
			get { return field.GetValue(parent); }
			set
			{
				bool old = HasValue;
				field.SetValue(parent, value);
				if (old != HasValue)
					OnHasValueChanged();
			}
		}

		public virtual string Name
		{
			get { return name; }
		}

		/// <summary>
		/// Return true if the inner value is an array with 
		/// elements or a non-null reference or is a default value.
		/// </summary>
		public bool HasValue
		{
			get
			{
				if(fieldSpecified!= null)
					return (bool)fieldSpecified.GetValue(parent);
				bool hasValue = ReflectionUtil.HasValue(Value) && Value.Equals(defaultValue) == false;
				return hasValue;
			}
		}

		public virtual Type Type
		{
			get { return field.FieldType; }
		}

		#endregion

		#region Methods

		public void SetToDefaultValue()
		{
			Value = defaultValue;
		}

		private void OnHasValueChanged()
		{
			if (HasValueChanged == null)
				return;
			HasValueChanged(this, new AttributeFieldReferenceEventArgs(HasValue));
		}

		protected void SetNameAndType(FieldInfo field)
		{
			XmlAttributeAttribute[] att = (XmlAttributeAttribute[]) field.GetCustomAttributes(typeof (XmlAttributeAttribute), false);
			DefaultValueAttribute[] def = (DefaultValueAttribute[]) field.GetCustomAttributes(typeof (DefaultValueAttribute), false);
			if (att.Length == 0 || att[0].AttributeName == "")
				this.name = field.Name;
			else
				this.name = att[0].AttributeName;
			if (def.Length > 0)
				defaultValue = def[0].Value;
		}

		#endregion
	}

	public class AttributeFieldReferenceEventArgs : EventArgs
	{
		private bool hasValue;

		public bool HasValue
		{
			get { return hasValue; }
		}

		public AttributeFieldReferenceEventArgs(bool hasValue)
		{
			this.hasValue = hasValue;
		}
	}
}