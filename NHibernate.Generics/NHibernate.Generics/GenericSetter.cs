#region license
// Copyright (c) 2005 - 2007 Ayende Rahien (ayende@ayende.com)
// All rights reserved.
// 
// Redistribution and use in source and binary forms, with or without modification,
// are permitted provided that the following conditions are met:
// 
//     * Redistributions of source code must retain the above copyright notice,
//     this list of conditions and the following disclaimer.
//     * Redistributions in binary form must reproduce the above copyright notice,
//     this list of conditions and the following disclaimer in the documentation
//     and/or other materials provided with the distribution.
//     * Neither the name of Ayende Rahien nor the names of its
//     contributors may be used to endorse or promote products derived from this
//     software without specific prior written permission.
// 
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND
// ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
// WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
// DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS BE LIABLE
// FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL
// DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR
// SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER
// CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY,
// OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF
// THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
#endregion


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

		///<summary>
		///
		///            When implemented by a class, gets the <see cref="T:System.Reflection.MethodInfo" /> for the 
		///<c>set</c>
		///            accessor of the property.
		///            
		///</summary>
		///
		///<remarks>
		///
		///            This is an optional operation - if the <see cref="T:NHibernate.Property.ISetter" /> is not 
		///            for a property 
		///<c>set</c> then 
		///<c>null</c> is an acceptable value to return.
		///            It is used by the proxies to determine which setter to intercept for the
		///            identifier property.
		///            
		///</remarks>
		///
		public MethodInfo Method
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
