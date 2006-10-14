using System;
using System.Collections.Generic;
using System.Text;
using NHibernate.Property;
using System.Reflection;

namespace NHibernate.Generics
{
	internal class GenericGetter : IGetter
	{
		private readonly FieldInfo _field;
		private readonly System.Type _clazz;
		private readonly string _name;
		private readonly System.Type _returnType;

		public GenericGetter(FieldInfo field, System.Type clazz, string name, System.Type returnType)
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
			_returnType = returnType;
		}

		#region IGetter Members

		public object Get(object target)
		{
			try
			{
				object value = _field.GetValue(target);
				if (value == null)
					return null;
				IWrapper wrapper = value as IWrapper;
				if (wrapper == null)
					throw new InvalidOperationException(string.Format("Error while getting property {0} on type {1} becuase {2} is not an IWrapper.",
						_name, _field.FieldType.Name, value.GetType().Name));
	
				return wrapper.Value;
			}
			catch( Exception e )
			{
				throw new PropertyAccessException(e, "Exception occurred", false, _clazz, _name);
			}
		}

		public System.Type ReturnType
		{
			get 
			{
				return _returnType; 
			}
		}

		public string PropertyName
		{
			get { return null; }
		}

		///<summary>
		///
		///            When implemented by a class, gets the <see cref="T:System.Reflection.MethodInfo" /> for the 
		///<c>get</c>
		///            accessor of the property.
		///            
		///</summary>
		///
		///<remarks>
		///
		///            This is an optional operation - if the <see cref="T:NHibernate.Property.IGetter" /> is not 
		///            for a property 
		///<c>get</c> then 
		///<c>null</c> is an acceptable value to return.
		///            It is used by the proxies to determine which getter to intercept for the
		///            identifier property.
		///            
		///</remarks>
		///
		public MethodInfo Method
		{
			get { return null; }
		}

		public System.Reflection.PropertyInfo Property
		{
			get { return null; }
		}

		#endregion
	}
}
