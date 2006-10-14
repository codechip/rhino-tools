using System;
using System.Collections.Generic;
using System.Text;
using NHibernate.Property;
using System.Reflection;

namespace NHibernate.Generics
{
	internal class GenericSetter : ISetter
	{
		private readonly FieldInfo _field;
		private readonly System.Type _clazz;
		private readonly string _name;

		/// <summary>
		/// Initializes a new instance of <see cref="FieldSetter"/>.
		/// </summary>
		/// <param name="clazz">The <see cref="System.Type"/> that contains the Field to use for the Property <c>set</c>.</param>
		/// <param name="field">The <see cref="FieldInfo"/> for reflection.</param>
		/// <param name="name">The name of the Field.</param>
		public GenericSetter(FieldInfo field, System.Type clazz, string name)
		{
			if (clazz == null)
				throw new ArgumentNullException("clazz");
			if (field == null)
				throw new ArgumentNullException("field");
			if (name == null)
				throw new ArgumentNullException("name");
			if (name.Length == 0)
				throw new ArgumentException("field name can't be empty", "name");
			this._field = field;
			this._clazz = clazz;
			this._name = name;
		}

		#region ISetter Members

		/// <summary>
		/// Sets the value of the Field on the object.
		/// </summary>
		/// <param name="target">The object to set the Field value in.</param>
		/// <param name="value">The value to set the Field to.</param>
		/// <exception cref="PropertyAccessException">
		/// Thrown when there is a problem setting the value in the target.
		/// </exception>
		public void Set( object target, object value )
		{
			try
			{
				IWrapper wrapper = (IWrapper)_field.GetValue(target);
                if (wrapper == null)
                    throw new InvalidOperationException(string.Format("Error while setting property {0} on type {1} becuase {2} is null when it should have a value.",
                        _name, _field.DeclaringType.Name, _field.Name));
				wrapper.Value = value;
			}
			catch( ArgumentException ae )
			{
				throw new PropertyAccessException( ae, "ArgumentException while setting the field value by reflection", true, _clazz, _name );
			}
			catch( Exception e )
			{
				throw new PropertyAccessException( e, "could not set a field value by reflection", true, _clazz, _name );
			}
		}

		/// <summary>
		/// Gets the name of the Property.
		/// </summary>
		/// <value><c>null</c> since this is a Field - not a Property.</value>
		public string PropertyName
		{
			get { return null; }
		}

		/// <summary>
		/// Gets the <see cref="PropertyInfo"/> for the Property.
		/// </summary>
		/// <value><c>null</c> since this is a Field - not a Property.</value>
		public PropertyInfo Property
		{
			get { return null; }
		}

		#endregion
	}
}
