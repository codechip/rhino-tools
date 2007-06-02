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
using System.Collections;
using System.Reflection;

namespace Ayende.NHibernateQueryAnalyzer.Utilities
{
	using Nullables;
    using NHibernate;

	/// <summary>
	/// Summary description for ReflectionUtil.
	/// </summary>
	public sealed class ReflectionUtil
	{
		/// <summary>
		/// Gets the property value.
		/// </summary>
		/// <param name="obj">obj.</param>
		/// <param name="property">Property.</param>
		/// <returns></returns>
		public static object GetPropertyValue(object obj, string property)
		{
			PropertyInfo prop = GetProperty(obj, property);
			if (!PropertyHasValue(obj, prop))
				return "No value";
			return GetPropertyValueInternal(prop, obj);
		}

		private static object GetPropertyValueInternal(PropertyInfo prop, object obj)
		{
			try
			{
				return prop.GetValue(obj, null);
			}
			catch (Exception e)
			{
			    return string.Format("Error getting value! {0}", e.Message);
			}
		}

		/// <summary>
		/// Gets the PropertyInfo thus named.
		/// </summary>
		/// <param name="obj">obj.</param>
		/// <param name="property">Property.</param>
		/// <returns></returns>
		private static PropertyInfo GetProperty(object obj, string property)
		{
			Type type = obj.GetType();
			return type.GetProperty(property, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
		}

		/// <summary>
		/// Gets the FieldInfo thus named
		/// </summary>
		/// <param name="obj">obj.</param>
		/// <param name="field">Field.</param>
		/// <returns></returns>
		private static FieldInfo GetField(object obj, string field)
		{
			Type type = obj.GetType();
			return type.GetField(field, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.IgnoreCase);
		}

		/// <summary>
		/// Check if the property is not null and can be read and is no indexed.
		/// This is done to know if it can be read safely.
		/// </summary>
		/// <param name="obj">Obj.</param>
		/// <param name="prop">Prop.</param>
		/// <returns></returns>
		private static bool PropertyHasValue(object obj, PropertyInfo prop)
		{
			if (obj == null || prop == null || !prop.CanRead || prop.GetIndexParameters().Length > 0)
				return false;
			else
				return true;
		}

		/// <summary>
		/// Determines whether type is simple enough to need just ToString()
		/// to show its state.
		/// (string,int, bool, enums are simple.
		/// Anything else is false.
		/// </summary>
		public static bool IsSimpleType(Type type)
		{
			if (type.IsEnum || type.IsPrimitive || type == typeof (string)
				|| type==typeof(DateTime) || typeof(INullableType).IsAssignableFrom(type))
				return true;
			else
				return false;
		}

		/// <summary>
		/// Determines whether the object is simple.
		/// An object is simple if its type is simple or if it's null.
		/// </summary>
		public static bool IsSimpleObject(object obj)
		{
			if (obj == null) return true;
			return IsSimpleType(obj.GetType());
		}

		/// <summary>
		/// Gets the name of an object.
		/// The name of the object is it's type name or the value of
		/// its Name property or field
		/// </summary>
		public static string GetName(object obj)
		{
			string s = GetNameOrEmpty(obj);
			if (s != "")
				return s;
			else
				return "{" + obj.GetType().Name + "}";
		}

		/// <summary>
		/// Gets the field value from object, and return 
		/// </summary>
		public static object GetFieldValue(object obj, FieldInfo field)
		{
			if (obj == null || field == null)
				return "No value";
			return field.GetValue(obj);
		}

		/// <summary>
		/// Gets the value of the object, if the object is simple, the returned string is
		/// the object ToString(), otherwise, it's the object name (if it has one) or the object type.
		/// </summary>
		public static string GetValue(object obj)
		{
			if (obj == null)
				return "null";
			if (IsSimpleObject(obj))
				return obj.ToString();
			else
				return GetName(obj);
		}

		/// <summary>
		/// Gets all the fields from the object's type with specified attribute
		/// </summary>
		public static FieldInfoCollection GetFieldsWithAttribute(Type type, Type attribute)
		{
			FieldInfoCollection fields = new FieldInfoCollection();
			foreach (FieldInfo field in type.GetFields())
			{
				if (field.GetCustomAttributes(attribute, true).Length > 0)
					fields.Add(field);
			}
			return fields;
		}

		/// <summary>
		/// Converts from string to the type.
		/// Can covert from string, enums booleans, bytes, int32 and datetime
		/// </summary>
		/// <param name="text">Text.</param>
		/// <param name="type">Type.</param>
		/// <returns></returns>
		public static object ConvertTo(string text, Type type)
		{
			if (type == typeof (string))
				return text == "" ? null : text;
			else if (type.IsEnum)
				return Enum.Parse(type, text);
			else if (type == typeof (Boolean))
				return bool.Parse(text);
			else if (type == typeof (byte))
				return byte.Parse(text);
			else if (type == typeof (Int32))
				return Int32.Parse(text);
			else if (type == typeof (DateTime))
				return DateTime.Parse(text);
			else
				throw new NotSupportedException("Converting type " + type.FullName + " is not supported.");
		}

		public static void AddToArray(FieldInfo field, object instance, object val)
		{
			if (!field.FieldType.IsArray)
				throw new InvalidOperationException("Field's type is not an array!");
			Array c = (Array) field.GetValue(instance);
			ArrayList list = new ArrayList();
			if (c != null)
				list.AddRange(c);
			list.Add(val);
			field.SetValue(instance, list.ToArray(field.FieldType.GetElementType()));
		}

		/// <summary>
		/// Gets all the fields WITHOUT ANY of the specified attributes.
		/// </summary>
		public static FieldInfoCollection GetFieldsWithOutAttributes(Type type, params Type[] types)
		{
			FieldInfoCollection fields = new FieldInfoCollection();

			bool match;
			foreach (FieldInfo field in type.GetFields())
			{
				match = true;
				foreach (Type attType in types)
				{
					if (field.GetCustomAttributes(attType, true).Length != 0)
						match = false;
				}
				if (match)
					fields.Add(field);
			}

			return fields;
		}

		/// <summary>
		/// Gets the value of a property or field name in the object.
		/// Or return empty string if there aren't any.
		/// </summary>
		public static string GetNameOrEmpty(object obj)
		{
			if (obj == null)
				return "null";
			PropertyInfo prop = GetProperty(obj, "Name");
			object val = null;
			if (PropertyHasValue(obj, prop))
				val = GetPropertyValueInternal(prop,obj);
			else
			{
				FieldInfo field = GetField(obj, "Name");
				if (field != null)
					val = field.GetValue(obj);
			}
			return val != null ? val.ToString() : "";
		}

		/// <summary>
		/// Sets the name property or value of an object to the value of name.
		/// Does nothing if the object doesn't have any fields or properties named 'name'
		/// </summary>
		public static void SetName(object obj, string name)
		{
			PropertyInfo prop = GetProperty(obj, "Name");
			if (prop != null && prop.CanWrite)
				prop.SetValue(obj, name, null);
			else
			{
				FieldInfo field = GetField(obj, "Name");
				if (field != null)
					field.SetValue(obj, name);
			}
		}

		public static void RemoveFromArray(FieldInfo field, object instance, int index)
		{
			Array c = (Array) field.GetValue(instance);
			ArrayList list = new ArrayList(c.Length - 1);
			for (int i = 0; i < c.Length; i++)
			{
				if (i != index)
					list.Add(c.GetValue(i));
			}
			field.SetValue(instance, list.ToArray(field.FieldType.GetElementType()));
		}

		public static PropertyInfo [] GetProperties(object obj)
		{
			return obj.GetType().
				GetProperties(BindingFlags.Instance|
				BindingFlags.GetProperty|BindingFlags.Public|
				BindingFlags.NonPublic);
		}

		public static object GetPropertyValue(PropertyInfo property, object obj)
		{
			if (property.CanRead && property.GetIndexParameters().Length == 0)
			{
				if (ReflectionUtil.IsSimpleType(property.PropertyType))
					return GetPropertyValueInternal(property,obj);
				else
					return GetPropertyValueInternal(property,obj);
			}
			return "{indexed or write only property}";
		}

		/// <summary>
		/// Gets the readable (non indexed) properties names and values.
		/// The keys holds the names of the properties.
		/// The values are the values of the properties
		/// </summary>
		public static IDictionary GetPropertiesDictionary(object obj)
		{
			Hashtable ht = new Hashtable();
			if(obj==null)
			{
				ht.Add("Value", "null");
				return ht;
			}
			if(obj is ValueType || obj is string)
			{
				ht.Add("ToString()",obj.ToString());
			}
			foreach (PropertyInfo property in obj.GetType().
				GetProperties(BindingFlags.Instance|
				BindingFlags.GetProperty|BindingFlags.Public|
				BindingFlags.NonPublic))
			{
				if (property.CanRead && property.GetIndexParameters().Length == 0)
				{
					if (ReflectionUtil.IsSimpleType(property.PropertyType))
					{
						object value = GetNullableObject(property,obj);
						if (value==null)
						{
							ht[property.Name] = "null";
						}
						else
						{
							ht[property.Name] = value.ToString();
						}
					}
					else
					{
						ht[property.Name] = GetPropertyValueInternal(property,obj);
					}
				}
			}
			return ht;
		}

		private static object GetNullableObject(PropertyInfo property, object obj)
		{
            if (NHibernateUtil.IsInitialized(obj) == false)
                NHibernateUtil.Initialize(obj);
			object value = GetPropertyValueInternal(property,obj);
			INullableType nullable = value as INullableType;
			if (value==null || (nullable!=null && nullable.HasValue==false))
			{
				return null;
			}
			else
			{
				return GetPropertyValueInternal(property,obj);
			}
		}

		/// <summary>
		/// Gets the fields names and values.
		/// The keys holds the names of the fields.
		/// The values hold the value of the field if it's a simple type, 
		/// or the name of the field's type.
		/// </summary>
		public static IDictionary GetFieldsDictionary(object obj)
		{
			Hashtable ht = new Hashtable();
			foreach (FieldInfo field in obj.GetType().GetFields())
			{
				if (ReflectionUtil.IsSimpleType(field.FieldType))
					ht[field.Name] = field.GetValue(obj);
				else
					ht[field.Name] = field.FieldType.Name;
			}
			return ht;
		}

		/// <summary>
		/// An object has value if it's not null, 
		/// an collection containing elements and a non-empty string
		/// </summary>
		public static bool HasValue(object value)
		{
			if (value == null)
				return false;
			ICollection c = value as ICollection;
			if (c != null)
				return c.Count > 0;
			string s = value as string;
			if (s != null)
				return s.Length > 0;
			return true;
		}
	}
}